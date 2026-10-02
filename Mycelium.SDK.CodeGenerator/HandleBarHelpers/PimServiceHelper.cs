// ------------------------------------------------------------------------------------------------
//  <copyright file="PimServiceHelper.cs" company="Starion Group S.A.">
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
    /// Provides Handlebars support for the PIM classes that a service interface is generated from.
    /// </summary>
    public static class PimServiceHelper
    {
        /// <summary>
        /// Registers the service helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the service helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterPimServiceHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper("Service.WriteInterfaceName", (writer, _, arguments) =>
            {
                var serviceClass = arguments.QuerySingle<IClass>("{{Service.WriteInterfaceName}}");

                writer.WriteSafeString(serviceClass.QueryServiceInterfaceName());
            });

            handlebars.RegisterHelper("Service.QueryName", (_, arguments) =>
            {
                var serviceClass = arguments.QuerySingle<IClass>("{{Service.QueryName}}");

                return serviceClass.Name;
            });

            handlebars.RegisterHelper("Service.QueryOperations", (_, arguments) =>
            {
                var serviceClass = arguments.QuerySingle<IClass>("{{Service.QueryOperations}}");

                return serviceClass.QueryServiceOperations();
            });
        }
    }
}
