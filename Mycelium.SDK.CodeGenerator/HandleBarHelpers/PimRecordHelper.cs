// ------------------------------------------------------------------------------------------------
//  <copyright file="PimRecordHelper.cs" company="Starion Group S.A.">
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

    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides Handlebars support for the PIM classes that a repository interface is generated from.
    /// </summary>
    public static class PimRecordHelper
    {
        /// <summary>
        /// Registers the record helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the record helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterPimRecordHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper("Record.WriteInterfaceName", (writer, _, arguments) =>
            {
                var recordClass = arguments.QuerySingle<IClass>("{{Record.WriteInterfaceName}}");

                writer.WriteSafeString(recordClass.QueryRepositoryInterfaceName());
            });

            handlebars.RegisterHelper("Record.WriteTypeName", (writer, _, arguments) =>
            {
                var recordClass = arguments.QuerySingle<IClass>("{{Record.WriteTypeName}}");

                writer.WriteSafeString(recordClass.QueryRecordTypeName());
            });

            handlebars.RegisterHelper("Record.QueryName", (_, arguments) =>
            {
                var recordClass = arguments.QuerySingle<IClass>("{{Record.QueryName}}");

                return recordClass.Name;
            });
        }
    }
}
