// ------------------------------------------------------------------------------------------------
//  <copyright file="PimOperationHelper.cs" company="Starion Group S.A.">
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

    /// <summary>
    /// Provides Handlebars support for the PIM operations that a service member is generated from.
    /// </summary>
    public static class PimOperationHelper
    {
        /// <summary>
        /// Registers the operation helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the operation helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterPimOperationHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper("Operation.WriteMethodName", (writer, _, arguments) =>
            {
                var operation = arguments.QuerySingle<IOperation>("{{Operation.WriteMethodName}}");

                writer.WriteSafeString(operation.QueryMethodName());
            });

            handlebars.RegisterHelper("Operation.QueryInputParameters", (_, arguments) =>
            {
                var operation = arguments.QuerySingle<IOperation>("{{Operation.QueryInputParameters}}");

                return operation.QueryInputParameters();
            });

            handlebars.RegisterHelper("Operation.QueryReturnParameters", (_, arguments) =>
            {
                var operation = arguments.QuerySingle<IOperation>("{{Operation.QueryReturnParameters}}");

                return operation.QueryReturnParameters();
            });

            handlebars.RegisterHelper("Operation.QueryReturnsMany", (_, arguments) =>
            {
                var operation = arguments.QuerySingle<IOperation>("{{Operation.QueryReturnsMany}}");

                return operation.QueryReturnsMany();
            });
        }
    }
}
