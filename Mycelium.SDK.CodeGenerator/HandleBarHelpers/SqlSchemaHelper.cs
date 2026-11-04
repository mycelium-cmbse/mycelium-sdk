// ------------------------------------------------------------------------------------------------
//  <copyright file="SqlSchemaHelper.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.HandleBarHelpers
{
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    using HandlebarsDotNet;

    using Humanizer;

    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides model-derived SQL names and relational rendering data for the Fabric schema template.
    /// </summary>
    public static class SqlSchemaHelper
    {
        /// <summary>
        /// The PostgreSQL identifier limit in bytes; generated identifiers contain only ASCII characters.
        /// </summary>
        private const int MaximumIdentifierBytes = 63;

        /// <summary>
        /// The digest length used to distinguish shortened or ambiguous identifiers.
        /// </summary>
        private const int IdentifierHashLength = 16;

        /// <summary>
        /// PostgreSQL 18 reserved identifiers, including keywords reserved for particular SQL contexts.
        /// Source: https://www.postgresql.org/docs/18/sql-keywords-appendix.html.
        /// </summary>
        private static readonly HashSet<string> ReservedIdentifiers = (
            "all analyse analyze and any array as asc asymmetric authorization binary both case cast check collate collation column concurrently " +
            "constraint create cross current_catalog current_date current_role current_schema current_time current_timestamp current_user default " +
            "deferrable desc distinct do else end except false fetch for foreign freeze from full grant group having ilike in initially inner " +
            "intersect into is isnull join lateral leading left like limit localtime localtimestamp natural not notnull null offset on only or " +
            "order outer overlaps placing primary references returning right select session_user similar some symmetric system_user table " +
            "tablesample then to trailing true union unique user using variadic verbose when where window with")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.Ordinal);

        /// <summary>
        /// Registers only the relational-model and SQL-object-name helpers consumed by the schema template.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterSqlSchemaHelpers(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper("SQL.Model", (_, arguments) =>
                QueryModel(arguments.QuerySingle<IReadOnlyCollection<IClass>>("{{SQL.Model}}")));

            handlebars.RegisterHelper("SQL.Name", (writer, _, arguments) =>
            {
                var components = arguments.QueryStringsFrom(0, "{{SQL.Name}}");

                if (components.Length < 2)
                {
                    throw new HandlebarsException("{{SQL.Name}} requires an object kind and at least one identifying name.");
                }

                writer.WriteSafeString(CreateObjectName(components));
            });
        }

        /// <summary>
        /// Constructs deterministic table, inheritance and ownership descriptions without rendering SQL statements.
        /// </summary>
        /// <param name="classes">
        /// The canonical model classes selected by the generator.
        /// </param>
        /// <returns>
        /// Rendering data for all class tables and stored relationships, including required ownership roles.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="classes" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the canonical class selection does not contain exactly one root Thing class.
        /// </exception>
        private static object QueryModel(IReadOnlyCollection<IClass> classes)
        {
            ArgumentNullException.ThrowIfNull(classes);

            var orderedClasses = classes.DistinctBy(umlClass => umlClass.XmiId, StringComparer.Ordinal)
                .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                .ThenBy(umlClass => umlClass.XmiId, StringComparer.Ordinal)
                .ToArray();

            var references = orderedClasses.ToDictionary(umlClass => umlClass.XmiId, umlClass => umlClass.QuerySqlReferenceProperties(), StringComparer.Ordinal);
            var root = orderedClasses.Single(umlClass => string.Equals(umlClass.Name, "Thing", StringComparison.Ordinal));
            var tableCandidates = orderedClasses.Where(umlClass => umlClass.XmiId != root.XmiId)
                .Select(umlClass => (Identity: "class:" + umlClass.XmiId, Name: umlClass.Name))
                .ToList();

            foreach (var umlClass in orderedClasses)
            {
                tableCandidates.AddRange(references[umlClass.XmiId]
                    .Where(property => property.QuerySqlRequiresLinkTable())
                    .Select(property => (Identity: "link:" + property.XmiId, Name: $"{umlClass.Name}_{property.Name}_{property.Type.Name}")));
            }

            var tableNames = MapIdentifiers(tableCandidates, ["thing"]);
            tableNames.Add("class:" + root.XmiId, "thing");
            var tables = new List<object>();
            var ownerships = new List<object>();

            foreach (var umlClass in orderedClasses)
            {
                var table = tableNames["class:" + umlClass.XmiId];
                string[] reservedColumns = umlClass.XmiId == root.XmiId ? ["id", "class_kind", "data"] : ["id"];
                var columnNames = MapIdentifiers(references[umlClass.XmiId]
                    .Where(property => !property.QueryIsEnumerable())
                    .Select(property => (Identity: property.XmiId, Name: property.Name)), reservedColumns);

                var columns = new List<object>();
                var links = new List<object>();

                foreach (var property in references[umlClass.XmiId])
                {
                    var isLink = property.QuerySqlRequiresLinkTable();

                    if (property.QueryIsEnumerable() && !isLink)
                    {
                        continue;
                    }

                    var target = (IClass)property.Type;
                    var targetTable = tableNames["class:" + target.XmiId];
                    var storageTable = isLink ? tableNames["link:" + property.XmiId] : table;
                    var source = MapIdentifier("source_" + umlClass.Name);
                    var targetColumn = MapIdentifier("target_" + target.Name);
                    var column = isLink ? targetColumn : columnNames[property.XmiId];
                    var ownerEnd = property.QueryIsOwnerEnd();

                    if (isLink)
                    {
                        links.Add(new
                        {
                            Table = storageTable, Source = source, Target = targetColumn, TargetTable = targetTable,
                            Position = property.IsOrdered || !property.IsUnique, PairUnique = property.IsUnique,
                            Exclusive = property.IsComposite && property.IsUnique
                        });
                    }
                    else
                    {
                        columns.Add(new
                        {
                            Column = column, TargetTable = targetTable, Required = !property.QueryIsNullable(),
                            Cascade = ownerEnd, Exclusive = property.IsComposite
                        });
                    }

                    if (!property.IsComposite && !ownerEnd)
                    {
                        continue;
                    }

                    var partType = ownerEnd ? umlClass : target;
                    var owner = isLink ? source : ownerEnd ? column : "id";
                    var part = isLink ? targetColumn : ownerEnd ? "id" : column;

                    ownerships.Add(new
                    {
                        Identity = property.XmiId, Table = storageTable, Owner = owner, Part = part,
                        Required = ownerEnd ? !property.QueryIsNullable() : property.Opposite is { Lower: > 0 },
                        PartKinds = QueryConcreteKinds(orderedClasses, partType),
                        GuardTrigger = CreateObjectName(["guard", storageTable, owner, part, property.XmiId]),
                        CheckTrigger = CreateObjectName(["ownership", storageTable, owner, part, property.XmiId])
                    });
                }

                tables.Add(new
                {
                    Table = table, Root = umlClass.XmiId == root.XmiId,
                    Kinds = QueryConcreteKinds(orderedClasses, umlClass), Columns = columns, Links = links,
                    Parents = umlClass.QueryGeneralizations()
                        .Select(parent => new { Table = tableNames["class:" + parent.XmiId] })
                        .ToArray()
                });
            }

            return new
            {
                Tables = tables, Ownerships = ownerships,
                ConcreteKinds = string.Join(", ", orderedClasses.Where(umlClass => !umlClass.IsAbstract).Select(umlClass => SqlLiteral(umlClass.Name)))
            };
        }

        /// <summary>
        /// Queries concrete model types whose class-table rows include the specified ancestor.
        /// </summary>
        /// <param name="classes">
        /// The deterministically ordered model classes.
        /// </param>
        /// <param name="ancestor">
        /// The class whose own and inherited membership is required.
        /// </param>
        /// <returns>
        /// SQL string literals for concrete descendants, including a concrete ancestor itself.
        /// </returns>
        private static string QueryConcreteKinds(IEnumerable<IClass> classes, IClass ancestor)
        {
            return string.Join(", ", classes.Where(umlClass => !umlClass.IsAbstract &&
                    umlClass.QueryAllGeneralClassifiers().Any(general => string.Equals(general.XmiId, ancestor.XmiId, StringComparison.Ordinal)))
                .Select(umlClass => SqlLiteral(umlClass.Name)));
        }

        /// <summary>
        /// Maps identifiers together within one SQL namespace, reserving fixed infrastructure column names.
        /// </summary>
        /// <param name="candidates">
        /// Stable model identities paired with their requested SQL names.
        /// </param>
        /// <param name="reserved">
        /// Names already occupied by fixed columns in this namespace.
        /// </param>
        /// <returns>
        /// A mapping indexed by stable model identity, with collisions resolved deterministically.
        /// </returns>
        private static Dictionary<string, string> MapIdentifiers(IEnumerable<(string Identity, string Name)> candidates, IReadOnlyCollection<string> reserved)
        {
            var entries = candidates.Select(candidate => new
            {
                candidate.Identity, Name = MapIdentifier(candidate.Name)
            }).ToArray();

            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            var occupied = entries.Select(entry => entry.Name).Concat(reserved).ToHashSet(StringComparer.Ordinal);

            foreach (var group in entries.GroupBy(entry => entry.Name, StringComparer.Ordinal).OrderBy(group => group.Key, StringComparer.Ordinal))
            {
                var needsSuffix = group.Count() > 1 || reserved.Contains(group.Key, StringComparer.Ordinal);

                foreach (var entry in group.OrderBy(entry => entry.Identity, StringComparer.Ordinal))
                {
                    var name = entry.Name;

                    if (needsSuffix)
                    {
                        var attempt = 0;

                        do
                        {
                            name = CreateObjectName([entry.Name, entry.Identity, attempt.ToString(CultureInfo.InvariantCulture)]);
                            attempt++;
                        }
                        while (!occupied.Add(name));
                    }

                    result.Add(entry.Identity, name);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts a modeled name to lowercase snake_case and handles reserved, invalid and overlong identifiers.
        /// </summary>
        /// <param name="name">
        /// The modeled class, property or relationship name.
        /// </param>
        /// <returns>
        /// A valid unquoted PostgreSQL identifier, retaining a readable name whenever possible.
        /// </returns>
        private static string MapIdentifier(string name)
        {
            var snake = name.Underscore();
            var safe = Regex.Replace(snake, "[^a-z0-9_]", "_");
            var replacedCharacters = !string.Equals(snake, safe, StringComparison.Ordinal);

            if (safe.Length == 0 || char.IsDigit(safe[0]) || ReservedIdentifiers.Contains(safe))
            {
                safe = "model_" + safe;
            }

            return safe.Length > MaximumIdentifierBytes || replacedCharacters
                ? CreateObjectName([safe, name])
                : safe;
        }

        /// <summary>
        /// Creates a deterministic SQL object name from its full identity, escaping no identifiers.
        /// </summary>
        /// <param name="components">
        /// Object kind, table, columns and any additional identity needed to distinguish roles.
        /// </param>
        /// <returns>
        /// A safe lowercase identifier with one digest suffix and at most 63 ASCII bytes.
        /// </returns>
        private static string CreateObjectName(IReadOnlyList<string> components)
        {
            var identity = string.Join("\0", components);
            var suffix = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)))[..IdentifierHashLength].ToLowerInvariant();
            var prefix = string.Join("_", components).Underscore();
            prefix = Regex.Replace(prefix, "[^a-z0-9_]", "_");

            if (prefix.Length == 0 || char.IsDigit(prefix[0]))
            {
                prefix = "object_" + prefix;
            }

            var prefixLimit = MaximumIdentifierBytes - IdentifierHashLength - 1;

            return $"{prefix[..Math.Min(prefix.Length, prefixLimit)]}_{suffix}";
        }

        /// <summary>
        /// Escapes a modeled discriminator as a PostgreSQL string literal without changing its value.
        /// </summary>
        /// <param name="value">
        /// The modeled concrete class name.
        /// </param>
        /// <returns>
        /// A SQL string literal with embedded apostrophes doubled.
        /// </returns>
        private static string SqlLiteral(string value) => "'" + value.Replace("'", "''", StringComparison.Ordinal) + "'";
    }
}
