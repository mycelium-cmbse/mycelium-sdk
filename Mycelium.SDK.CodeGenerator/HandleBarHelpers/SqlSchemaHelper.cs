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
    using HandlebarsDotNet;

    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides model-derived SQL names and relational rendering data for the Fabric schema template, and the fixed SQL names that the schema and
    /// DAO templates share.
    /// </summary>
    public static class SqlSchemaHelper
    {
        /// <summary>
        /// The prefix that keys a class table by the XMI identifier of its class.
        /// </summary>
        private const string ClassIdentityPrefix = "class:";

        /// <summary>
        /// The prefix that keys a link table by the XMI identifier of the property it stores.
        /// </summary>
        private const string LinkIdentityPrefix = "link:";

        /// <summary>
        /// The schema of every generated table and function, from the shared SQL names.
        /// </summary>
        private const string Schema = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.SqlSchemaName;

        /// <summary>
        /// The identity column of every class table, from the shared SQL names.
        /// </summary>
        private const string IdentityColumn = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.SqlIdentityColumnName;

        /// <summary>
        /// The discriminator column of the root table, from the shared SQL names.
        /// </summary>
        private const string ClassKindColumn = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.SqlClassKindColumnName;

        /// <summary>
        /// The JSON document column of the root table, from the shared SQL names.
        /// </summary>
        private const string DataColumn = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.SqlDataColumnName;

        /// <summary>
        /// The link-table column holding the owner object's identity, from the shared SQL names.
        /// </summary>
        private const string SourceColumn = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.SqlLinkSourceColumnName;

        /// <summary>
        /// The link-table column holding the target object's identity, from the shared SQL names.
        /// </summary>
        private const string TargetColumn = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.SqlLinkTargetColumnName;

        /// <summary>
        /// The link-table column holding an occurrence's position, from the shared SQL names.
        /// </summary>
        private const string PositionColumn = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.SqlLinkPositionColumnName;

        /// <summary>
        /// The scope in which PostgreSQL requires table and index names to be unique.
        /// </summary>
        private const string SchemaScope = $"relations of schema {Schema}";

        /// <summary>
        /// Registers only the relational-model helper consumed by the schema template.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the helper is registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterSqlSchemaHelpers(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterSqlNameHelpers();

            handlebars.RegisterHelper("SQL.Model", (_, arguments) =>
                QueryModel(arguments.QuerySingle<IReadOnlyCollection<IClass>>("{{SQL.Model}}")));
        }

        /// <summary>
        /// Registers the helpers that write the fixed SQL names shared by the schema and the DAO templates, so that neither template repeats them as
        /// literals.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterSqlNameHelpers(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            RegisterName(handlebars, "SQL.WriteSchemaName", Schema);
            RegisterName(handlebars, "SQL.WriteIdentityColumnName", IdentityColumn);
            RegisterName(handlebars, "SQL.WriteClassKindColumnName", ClassKindColumn);
            RegisterName(handlebars, "SQL.WriteDataColumnName", DataColumn);
            RegisterName(handlebars, "SQL.WriteLinkSourceColumnName", SourceColumn);
            RegisterName(handlebars, "SQL.WriteLinkTargetColumnName", TargetColumn);
            RegisterName(handlebars, "SQL.WriteLinkPositionColumnName", PositionColumn);
        }

        /// <summary>
        /// Registers a helper that writes one fixed SQL name.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the helper is registered.
        /// </param>
        /// <param name="helperName">
        /// The helper name used in the templates.
        /// </param>
        /// <param name="sqlName">
        /// The SQL name the helper writes.
        /// </param>
        private static void RegisterName(IHandlebars handlebars, string helperName, string sqlName) =>
            handlebars.RegisterHelper(helperName, (writer, _, _) => writer.WriteSafeString(sqlName));

        /// <summary>
        /// Queries the single root class shared by every class, through the same class query the DAO generator uses rather than by name.
        /// </summary>
        /// <param name="classes">
        /// The deterministically ordered model classes.
        /// </param>
        /// <returns>
        /// The class at the top of every class's parent chain, whose table holds the identity, discriminator and JSON document.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the classes do not share exactly one root class; the message names the roots found.
        /// </exception>
        private static IClass QueryRootClass(IReadOnlyCollection<IClass> classes)
        {
            var roots = classes.Select(umlClass => umlClass.QuerySqlRootClass())
                .DistinctBy(root => root.XmiId, StringComparer.Ordinal)
                .ToArray();

            return roots.Length == 1
                ? roots[0]
                : throw new InvalidOperationException(
                    $"The SQL schema needs exactly one root class shared by every class, but found {roots.Length}: " +
                    $"{string.Join(", ", roots.Select(root => root.Name))}.");
        }

        /// <summary>
        /// Constructs deterministic table, inheritance and ownership descriptions, including every SQL object name, without rendering SQL statements.
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
        /// Thrown when the classes do not share exactly one root class, or when two modeled elements produce the same SQL name.
        /// </exception>
        private static object QueryModel(IReadOnlyCollection<IClass> classes)
        {
            ArgumentNullException.ThrowIfNull(classes);

            var orderedClasses = classes.DistinctBy(umlClass => umlClass.XmiId, StringComparer.Ordinal)
                .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                .ThenBy(umlClass => umlClass.XmiId, StringComparer.Ordinal)
                .ToArray();

            var references = orderedClasses.ToDictionary(umlClass => umlClass.XmiId, umlClass => umlClass.QuerySqlReferenceProperties(), StringComparer.Ordinal);
            var root = QueryRootClass(orderedClasses);
            var claims = new Dictionary<(string Scope, string Name), string>();
            var tableNames = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var umlClass in orderedClasses)
            {
                var table = umlClass.QuerySqlIdentifier();
                tableNames.Add(ClassIdentityPrefix + umlClass.XmiId, ClaimName(claims, $"the table of class '{umlClass.Name}'", table, SchemaScope));
            }

            foreach (var umlClass in orderedClasses)
            {
                foreach (var property in references[umlClass.XmiId].Where(property => property.QuerySqlRequiresLinkTable()))
                {
                    var table = property.QuerySqlLinkTableName();
                    var element = $"the link table of property '{umlClass.Name}.{property.Name}'";
                    tableNames.Add(LinkIdentityPrefix + property.XmiId, ClaimName(claims, element, table, SchemaScope));
                }
            }

            var rootTable = tableNames[ClassIdentityPrefix + root.XmiId];
            var tables = new List<object>();
            var roles = new List<OwnershipRole>();

            foreach (var umlClass in orderedClasses)
            {
                var table = tableNames[ClassIdentityPrefix + umlClass.XmiId];
                var isRoot = umlClass.XmiId == root.XmiId;
                string[] fixedColumns = isRoot ? [IdentityColumn, ClassKindColumn, DataColumn] : [IdentityColumn];

                foreach (var fixedColumn in fixedColumns)
                {
                    ClaimName(claims, $"the fixed column '{fixedColumn}'", fixedColumn, ColumnScope(table));
                }

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
                    var targetTable = tableNames[ClassIdentityPrefix + target.XmiId];
                    var ownerEnd = property.QueryIsOwnerEnd();
                    var element = $"property '{umlClass.Name}.{property.Name}'";
                    string storageTable;
                    string column;

                    if (isLink)
                    {
                        storageTable = tableNames[LinkIdentityPrefix + property.XmiId];
                        column = TargetColumn;
                        links.Add(QueryLink(claims, element, property, storageTable, targetTable));
                    }
                    else
                    {
                        storageTable = table;
                        column = ClaimName(claims, $"the column of {element}", property.QuerySqlIdentifier(), ColumnScope(table));
                        var exclusive = property.IsComposite;

                        columns.Add(new
                        {
                            Column = column, TargetTable = targetTable, Required = !property.QueryIsNullable(), Cascade = ownerEnd, Exclusive = exclusive,
                            ForeignKey = ClaimName(claims, $"the foreign key of {element}", ObjectName(table, "fkey", column), ConstraintScope(table)),
                            UniqueKey = exclusive ? ClaimName(claims, $"the unique key of {element}", ObjectName(table, "key", column), SchemaScope, ConstraintScope(table)) : null,
                            IndexName = exclusive ? null : ClaimName(claims, $"the index of {element}", ObjectName(table, "idx", column), SchemaScope)
                        });
                    }

                    if (!property.IsComposite && !ownerEnd)
                    {
                        continue;
                    }

                    var partType = ownerEnd ? umlClass : target;
                    var (owner, part) = (isLink, ownerEnd) switch
                    {
                        (true, _) => (SourceColumn, TargetColumn),
                        (false, true) => (column, IdentityColumn),
                        (false, false) => (IdentityColumn, column)
                    };

                    var required = ownerEnd ? !property.QueryIsNullable() : property.Opposite is { Lower: > 0 };
                    roles.Add(new OwnershipRole(storageTable, isLink, owner, part, required, QueryConcreteKinds(orderedClasses, partType), element));
                }

                var parents = new List<object>();

                foreach (var parent in umlClass.QueryGeneralizations())
                {
                    var element = $"the generalization of class '{umlClass.Name}' to '{parent.Name}'";

                    parents.Add(new
                    {
                        Table = tableNames[ClassIdentityPrefix + parent.XmiId],
                        ForeignKey = ClaimName(claims, element, ObjectName(table, "fkey", IdentityColumn), ConstraintScope(table))
                    });
                }

                tables.Add(new
                {
                    Table = table, RootTable = rootTable, IsRoot = isRoot, Kinds = QueryConcreteKinds(orderedClasses, umlClass), Columns = columns, Links = links,
                    Parents = parents,
                    PrimaryKey = ClaimName(claims, $"the primary key of class '{umlClass.Name}'", ObjectName(table, "pkey"), SchemaScope, ConstraintScope(table)),
                    ClassKindCheck = isRoot ? ClaimName(claims, "the discriminator check", ObjectName(table, "check", ClassKindColumn), ConstraintScope(table)) : null,
                    DataCheck = isRoot ? ClaimName(claims, "the payload metadata check", ObjectName(table, "check", DataColumn), ConstraintScope(table)) : null
                });
            }

            return new
            {
                Tables = tables, RootTable = rootTable, Ownerships = QueryOwnerships(claims, roles),
                ConcreteKinds = string.Join(", ", orderedClasses.Where(umlClass => !umlClass.IsAbstract).Select(umlClass => SqlLiteral(umlClass.Name)))
            };
        }

        /// <summary>
        /// Describes one link table with its fixed columns and the names of its keys and index.
        /// </summary>
        /// <param name="claims">
        /// The SQL names already generated, keyed by uniqueness scope and name.
        /// </param>
        /// <param name="element">
        /// A description of the modeled property stored in the link table, used in collision messages.
        /// </param>
        /// <param name="property">
        /// The modeled collection property stored in the link table.
        /// </param>
        /// <param name="table">
        /// The link-table name.
        /// </param>
        /// <param name="targetTable">
        /// The table of the property's declared target class.
        /// </param>
        /// <returns>
        /// Rendering data for the link table.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a generated name collides with a name generated for another modeled element.
        /// </exception>
        private static object QueryLink(Dictionary<(string Scope, string Name), string> claims, string element, IProperty property, string table, string targetTable)
        {
            var position = property.QuerySqlLinkHasPosition();
            var pairUnique = position && property.IsUnique;
            var exclusive = property.IsComposite && property.IsUnique;

            return new
            {
                Table = table, Source = SourceColumn, Target = TargetColumn, TargetTable = targetTable, Position = position, PairUnique = pairUnique, Exclusive = exclusive,
                PrimaryKey = ClaimName(claims, $"the primary key of {element}", ObjectName(table, "pkey"), SchemaScope, ConstraintScope(table)),
                PairUniqueKey = pairUnique
                    ? ClaimName(claims, $"the pair key of {element}", ObjectName(table, "key", SourceColumn, TargetColumn), SchemaScope, ConstraintScope(table))
                    : null,
                SourceForeignKey = ClaimName(claims, $"the owner foreign key of {element}", ObjectName(table, "fkey", SourceColumn), ConstraintScope(table)),
                TargetForeignKey = ClaimName(claims, $"the target foreign key of {element}", ObjectName(table, "fkey", TargetColumn), ConstraintScope(table)),
                TargetUniqueKey = exclusive
                    ? ClaimName(claims, $"the part key of {element}", ObjectName(table, "key", TargetColumn), SchemaScope, ConstraintScope(table))
                    : null,
                TargetIndexName = exclusive ? null : ClaimName(claims, $"the target index of {element}", ObjectName(table, "idx", TargetColumn), SchemaScope)
            };
        }

        /// <summary>
        /// Names the ownership triggers of every composition relationship, preserving model order. A relationship stored on a class table suffixes its
        /// trigger names with its storing column; one stored in a link table, which holds only that relationship, takes no suffix. A trigger name therefore
        /// never depends on other relationships stored on the same table.
        /// </summary>
        /// <param name="claims">
        /// The SQL names already generated, keyed by uniqueness scope and name.
        /// </param>
        /// <param name="roles">
        /// The composition relationships in model order.
        /// </param>
        /// <returns>
        /// Rendering data for each ownership relationship, including its tracking and validation trigger names.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a generated trigger name collides with another trigger on the same table.
        /// </exception>
        private static object[] QueryOwnerships(Dictionary<(string Scope, string Name), string> claims, IReadOnlyCollection<OwnershipRole> roles)
        {
            var ownerships = new List<object>();

            foreach (var role in roles)
            {
                var relationshipColumn = string.Equals(role.Part, IdentityColumn, StringComparison.Ordinal) ? role.Owner : role.Part;
                var suffix = role.IsLink ? string.Empty : "_" + relationshipColumn;
                var trackTrigger = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.ShortenSqlIdentifier("t10_track_ownership" + suffix);
                var validateTrigger = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.ShortenSqlIdentifier("t80_validate_ownership" + suffix);

                ownerships.Add(new
                {
                    role.Table, role.Owner, role.Part, role.Required, role.PartKinds,
                    TrackTrigger = ClaimName(claims, $"the ownership tracking trigger of {role.Element}", trackTrigger, TriggerScope(role.Table)),
                    ValidateTrigger = ClaimName(claims, $"the ownership validation trigger of {role.Element}", validateTrigger, TriggerScope(role.Table))
                });
            }

            return [.. ownerships];
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
        /// Records a generated SQL name in every scope in which PostgreSQL requires it to be unique, failing instead of disambiguating.
        /// </summary>
        /// <param name="claims">
        /// The SQL names already generated, keyed by uniqueness scope and name, with the modeled element that produced each.
        /// </param>
        /// <param name="element">
        /// A description of the modeled element that produces the name, used in collision messages.
        /// </param>
        /// <param name="name">
        /// The generated SQL name.
        /// </param>
        /// <param name="scopes">
        /// The uniqueness scopes the name belongs to.
        /// </param>
        /// <returns>
        /// The unchanged <paramref name="name" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when another modeled element already produced the same name in one of the scopes.
        /// </exception>
        private static string ClaimName(Dictionary<(string Scope, string Name), string> claims, string element, string name, params string[] scopes)
        {
            var collidingScope = scopes.FirstOrDefault(scope => claims.ContainsKey((scope, name)));

            if (collidingScope != null)
            {
                throw new InvalidOperationException(
                    $"The SQL name '{name}' is generated for both {claims[(collidingScope, name)]} and {element} in the {collidingScope}. " +
                    "Rename one of them in the model; the generator does not add disambiguating suffixes.");
            }

            foreach (var scope in scopes)
            {
                claims.Add((scope, name), element);
            }

            return name;
        }

        /// <summary>
        /// Builds a constraint or index name from its own table, its own columns and its PostgreSQL purpose suffix.
        /// </summary>
        /// <param name="table">
        /// The table the object belongs to.
        /// </param>
        /// <param name="suffix">
        /// The PostgreSQL purpose suffix: <c>pkey</c>, <c>fkey</c>, <c>key</c>, <c>idx</c> or <c>check</c>.
        /// </param>
        /// <param name="columns">
        /// The participating columns of the table, in declaration order.
        /// </param>
        /// <returns>
        /// The readable name, hashed only when it exceeds the identifier limit.
        /// </returns>
        private static string ObjectName(string table, string suffix, params string[] columns) =>
            Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions.ShortenSqlIdentifier(string.Join("_", [table, .. columns, suffix]));

        /// <summary>
        /// Describes the scope in which a table's column names must be unique.
        /// </summary>
        /// <param name="table">
        /// The table name.
        /// </param>
        /// <returns>
        /// The column scope of <paramref name="table" />.
        /// </returns>
        private static string ColumnScope(string table) => $"columns of table {Schema}.{table}";

        /// <summary>
        /// Describes the scope in which a table's constraint names must be unique.
        /// </summary>
        /// <param name="table">
        /// The table name.
        /// </param>
        /// <returns>
        /// The constraint scope of <paramref name="table" />.
        /// </returns>
        private static string ConstraintScope(string table) => $"constraints of table {Schema}.{table}";

        /// <summary>
        /// Describes the scope in which a table's trigger names must be unique.
        /// </summary>
        /// <param name="table">
        /// The table name.
        /// </param>
        /// <returns>
        /// The trigger scope of <paramref name="table" />.
        /// </returns>
        private static string TriggerScope(string table) => $"triggers of table {Schema}.{table}";

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

        /// <summary>
        /// One composition relationship as stored in a table, before its trigger names are assigned.
        /// </summary>
        /// <param name="Table">
        /// The table that stores the relationship.
        /// </param>
        /// <param name="IsLink">
        /// Whether the relationship is stored in its own link table rather than in a column of a class table.
        /// </param>
        /// <param name="Owner">
        /// The column holding the owner's identity.
        /// </param>
        /// <param name="Part">
        /// The column holding the part's identity.
        /// </param>
        /// <param name="Required">
        /// Whether the modeled multiplicity requires every part to have an owner.
        /// </param>
        /// <param name="PartKinds">
        /// SQL string literals of the concrete part types.
        /// </param>
        /// <param name="Element">
        /// A description of the modeled property, used in collision messages.
        /// </param>
        private sealed record OwnershipRole(string Table, bool IsLink, string Owner, string Part, bool Required, string PartKinds, string Element);
    }
}
