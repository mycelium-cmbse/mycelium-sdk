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
    /// Provides SQL naming and representation helpers for the Fabric PostgreSQL schema template.
    /// </summary>
    public static class SqlSchemaHelper
    {
        /// <summary>
        /// Registers the helpers used to render class tables, references and inheritance deletion objects.
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

            handlebars.RegisterHelper("SQL.Parents", (_, arguments) =>
                arguments.QuerySingle<IClass>("{{SQL.Parents}}").QueryGeneralizations());

            handlebars.RegisterHelper("SQL.IsCollection", (_, arguments) =>
                arguments.QuerySingle<IProperty>("{{SQL.IsCollection}}").QueryIsEnumerable());

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
                var owner = arguments.QueryFirst<IClass>("{{SQL.LinkTable}}");
                var property = arguments.Length == 2 && arguments[1] is IProperty reference
                    ? reference
                    : throw new HandlebarsException("{{SQL.LinkTable}} requires an IClass and an IProperty argument.");

                writer.WriteSafeString(Qualify($"{owner.Name}_{property.Name.LowerCaseFirstLetter()}__{property.Type.Name}"));
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
