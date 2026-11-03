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
    using System.Security.Cryptography;
    using System.Text;

    using HandlebarsDotNet;

    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides SQL naming and representation helpers for the Fabric PostgreSQL schema template.
    /// </summary>
    public static class SqlSchemaHelper
    {
        /// <summary>
        /// The PostgreSQL identifier limit in UTF-8 bytes.
        /// </summary>
        private const int MaximumIdentifierBytes = 63;

        /// <summary>
        /// The number of hexadecimal digest characters retained in generated object names.
        /// </summary>
        private const int IdentifierHashLength = 16;

        /// <summary>
        /// Registers the helpers used to render class tables, named constraints and ownership and inheritance deletion objects.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the SQL helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterSqlSchemaHelpers(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper("SQL.IsThing", (_, arguments) =>
                string.Equals(arguments.QuerySingle<IClass>("{{SQL.IsThing}}").Name, "Thing", StringComparison.Ordinal));

            handlebars.RegisterHelper("SQL.References", (_, arguments) =>
                arguments.QuerySingle<IClass>("{{SQL.References}}").QuerySqlReferenceProperties());

            handlebars.RegisterHelper("SQL.Compositions", (_, arguments) =>
                arguments.QuerySingle<IClass>("{{SQL.Compositions}}").QuerySqlReferenceProperties()
                    .Where(property => property.QuerySqlRequiresCompositionTrigger()).ToArray());

            handlebars.RegisterHelper("SQL.Parents", (_, arguments) =>
                arguments.QuerySingle<IClass>("{{SQL.Parents}}").QueryGeneralizations());

            handlebars.RegisterHelper("SQL.IsCollection", (_, arguments) =>
                arguments.QuerySingle<IProperty>("{{SQL.IsCollection}}").QueryIsEnumerable());

            handlebars.RegisterHelper("SQL.RequiresLinkTable", (_, arguments) =>
                arguments.QuerySingle<IProperty>("{{SQL.RequiresLinkTable}}").QuerySqlRequiresLinkTable());

            handlebars.RegisterHelper("SQL.Cascades", (_, arguments) =>
                arguments.QuerySingle<IProperty>("{{SQL.Cascades}}").QueryIsOwnerEnd());

            handlebars.RegisterHelper("SQL.IsRequired", (_, arguments) =>
                !arguments.QuerySingle<IProperty>("{{SQL.IsRequired}}").QueryIsNullable());

            handlebars.RegisterHelper("SQL.HasPosition", (_, arguments) =>
            {
                var property = arguments.QuerySingle<IProperty>("{{SQL.HasPosition}}");

                return property.IsOrdered || !property.IsUnique;
            });

            handlebars.RegisterHelper("SQL.Table", (writer, _, arguments) =>
            {
                var umlClass = arguments.QuerySingle<IClass>("{{SQL.Table}}");

                writer.WriteSafeString(Qualify(umlClass.Name));
            });

            handlebars.RegisterHelper("SQL.Column", (writer, _, arguments) =>
            {
                var property = arguments.QuerySingle<IProperty>("{{SQL.Column}}");

                writer.WriteSafeString(Quote(property.Name.LowerCaseFirstLetter()));
            });

            handlebars.RegisterHelper("SQL.TargetTable", (writer, _, arguments) =>
            {
                var property = arguments.QuerySingle<IProperty>("{{SQL.TargetTable}}");

                writer.WriteSafeString(Qualify(property.Type.Name));
            });

            handlebars.RegisterHelper("SQL.LinkTable", (writer, _, arguments) =>
            {
                var (owner, property) = QueryReferenceArguments(arguments, "{{SQL.LinkTable}}");

                writer.WriteSafeString(Qualify(QueryLinkTableName(owner, property)));
            });

            handlebars.RegisterHelper("SQL.LinkName", (_, arguments) =>
            {
                var (owner, property) = QueryReferenceArguments(arguments, "{{SQL.LinkName}}");

                return QueryLinkTableName(owner, property);
            });

            handlebars.RegisterHelper("SQL.Name", (writer, _, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    throw new HandlebarsException("{{SQL.Name}} requires an object kind and at least one identifying name.");
                }

                var components = new string[arguments.Length];

                for (var index = 0; index < arguments.Length; index++)
                {
                    components[index] = arguments[index] is string component
                        ? component
                        : throw new HandlebarsException("{{SQL.Name}} requires string arguments.");
                }

                writer.WriteSafeString(Quote(CreateObjectName(components)));
            });

            handlebars.RegisterHelper("SQL.SourceColumn", (writer, _, arguments) =>
            {
                var owner = arguments.QuerySingle<IClass>("{{SQL.SourceColumn}}");

                writer.WriteSafeString(Quote($"source{owner.Name}"));
            });

            handlebars.RegisterHelper("SQL.TargetColumn", (writer, _, arguments) =>
            {
                var property = arguments.QuerySingle<IProperty>("{{SQL.TargetColumn}}");

                writer.WriteSafeString(Quote($"target{property.Type.Name}"));
            });

            handlebars.RegisterHelper("SQL.DeleteFunction", (writer, _, arguments) =>
            {
                var umlClass = arguments.QuerySingle<IClass>("{{SQL.DeleteFunction}}");

                writer.WriteSafeString(Qualify($"{umlClass.Name.ToLowerInvariant()}_delete"));
            });

            handlebars.RegisterHelper("SQL.DeleteTrigger", (writer, _, arguments) =>
            {
                var umlClass = arguments.QuerySingle<IClass>("{{SQL.DeleteTrigger}}");

                writer.WriteSafeString(Quote($"trg_{umlClass.Name.ToLowerInvariant()}_delete"));
            });
        }

        /// <summary>
        /// Reads a declaring class and reference property from a SQL helper invocation.
        /// </summary>
        /// <param name="arguments">
        /// The helper arguments containing the owner and reference.
        /// </param>
        /// <param name="helperName">
        /// The helper name included in argument diagnostics.
        /// </param>
        /// <returns>
        /// The declaring owner and its reference property.
        /// </returns>
        /// <exception cref="HandlebarsException">
        /// Thrown when the invocation does not supply exactly one class followed by one property.
        /// </exception>
        private static (IClass Owner, IProperty Property) QueryReferenceArguments(Arguments arguments, string helperName)
        {
            if (arguments.Length != 2 || arguments[0] is not IClass owner || arguments[1] is not IProperty property)
            {
                throw new HandlebarsException($"{helperName} requires an IClass and an IProperty argument.");
            }

            return (owner, property);
        }

        /// <summary>
        /// Queries the unquoted link-table name shared by table rendering and object naming.
        /// </summary>
        /// <param name="owner">
        /// The class declaring the collection reference.
        /// </param>
        /// <param name="property">
        /// The collection reference identifying the target class.
        /// </param>
        /// <returns>
        /// The modeled owner, property and target joined using the link-table naming convention.
        /// </returns>
        private static string QueryLinkTableName(IClass owner, IProperty property) =>
            $"{owner.Name}_{property.Name.LowerCaseFirstLetter()}__{property.Type.Name}";

        /// <summary>
        /// Creates a stable object name with a digest of its complete identity and a UTF-8-safe readable prefix.
        /// </summary>
        /// <param name="components">
        /// The object kind followed by its table, column and optional target or role names.
        /// </param>
        /// <returns>
        /// An unquoted identifier within PostgreSQL's byte limit whose suffix distinguishes ambiguous or truncated readable prefixes.
        /// </returns>
        private static string CreateObjectName(IReadOnlyList<string> components)
        {
            var identityBytes = Encoding.UTF8.GetBytes(string.Join("\0", components));
            var suffix = Convert.ToHexString(SHA256.HashData(identityBytes))[..IdentifierHashLength].ToLowerInvariant();
            var availableBytes = MaximumIdentifierBytes - IdentifierHashLength - 1;
            var prefix = new StringBuilder();
            var prefixBytes = 0;

            foreach (var rune in string.Join("_", components).EnumerateRunes())
            {
                if (prefixBytes + rune.Utf8SequenceLength > availableBytes)
                {
                    break;
                }

                prefix.Append(rune.ToString());
                prefixBytes += rune.Utf8SequenceLength;
            }

            return $"{prefix}_{suffix}";
        }

        /// <summary>
        /// Qualifies a quoted SQL object name with the Fabric schema.
        /// </summary>
        /// <param name="name">
        /// The unquoted object name.
        /// </param>
        /// <returns>
        /// The schema-qualified SQL identifier.
        /// </returns>
        private static string Qualify(string name) => $"\"Fabric\".{Quote(name)}";

        /// <summary>
        /// Quotes a PostgreSQL identifier without changing its modeled spelling.
        /// </summary>
        /// <param name="name">
        /// The unquoted identifier.
        /// </param>
        /// <returns>
        /// The identifier surrounded by double quotes, with embedded double quotes escaped.
        /// </returns>
        private static string Quote(string name) => $"\"{name.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }
}
