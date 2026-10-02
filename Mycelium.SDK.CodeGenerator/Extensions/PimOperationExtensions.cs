// ------------------------------------------------------------------------------------------------
//  <copyright file="PimOperationExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using Humanizer;

    using uml4net.Classification;
    using uml4net.Extensions;

    /// <summary>
    /// Provides Mycelium-specific queries for the PIM operations that a service member is generated from.
    /// </summary>
    public static class PimOperationExtensions
    {
        /// <summary>
        /// The suffix appended to every generated service member name.
        /// </summary>
        private const string MethodNameSuffix = "Async";

        extension(IOperation operation)
        {
            /// <summary>
            /// Queries the name of the generated service member.
            /// </summary>
            /// <returns>
            /// The member name - for example <c>GetProjectByIdAsync</c> for the <c>getProjectById</c>
            /// operation.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the operation has no name.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when the derived name cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryMethodName()
            {
                ArgumentNullException.ThrowIfNull(operation);

                return string.IsNullOrWhiteSpace(operation.Name) ? throw new InvalidOperationException($"Operation '{operation.XmiId}' has no name, so the generated member would have no name.") : ReservedCSharpNameMapper.Map($"{operation.Name.Pascalize()}{MethodNameSuffix}");
            }

            /// <summary>
            /// Queries the result parameter of an operation as a collection of zero or one.
            /// </summary>
            /// <returns>
            /// The result parameter, or an empty collection when the operation declares no result.
            /// </returns>
            /// <remarks>
            /// Returning a collection lets a template branch on presence with <c>each</c> and its
            /// <c>else</c>, which the Handlebars compiler cannot do with a helper used as an argument.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IParameter> QueryReturnParameters()
            {
                ArgumentNullException.ThrowIfNull(operation);

                var returnParameter = operation.QueryReturnParameter();

                return returnParameter is null ? [] : [returnParameter];
            }

            /// <summary>
            /// Queries the parameters that the generated service member accepts.
            /// </summary>
            /// <returns>
            /// Every owned parameter except the one carrying the result, in document order.
            /// </returns>
            /// <remarks>
            /// Document order is the declared signature order and is preserved; only the result parameter,
            /// which the PIM interleaves with the inputs, is removed.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when an owned parameter has no name.
            /// </exception>
            public IReadOnlyList<IParameter> QueryInputParameters()
            {
                ArgumentNullException.ThrowIfNull(operation);

                var returnParameter = operation.QueryReturnParameter();

                var inputParameters = operation.OwnedParameter
                    .Where(parameter => !ReferenceEquals(parameter, returnParameter))
                    .ToArray();

                var unnamedParameter = inputParameters.FirstOrDefault(parameter => string.IsNullOrWhiteSpace(parameter.Name));

                return unnamedParameter is not null ? throw new InvalidOperationException($"Operation '{operation.DescribeQualified()}' declares parameter '{unnamedParameter.XmiId}' which has no name.") : inputParameters;
            }

            /// <summary>
            /// Determines whether an operation returns more than one element.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the result parameter is a collection.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            public bool QueryReturnsMany()
            {
                ArgumentNullException.ThrowIfNull(operation);

                return operation.QueryReturnParameter()
                    ?.QueryIsEnumerable() == true;
            }

            /// <summary>
            /// Queries the parameter that carries the result of an operation.
            /// </summary>
            /// <returns>
            /// The last parameter declared with return direction, or <see langword="null" /> when the
            /// operation declares no result.
            /// </returns>
            /// <remarks>
            /// UML permits several return parameters and the PIM declares two on <c>getQueryById</c>, where
            /// the identifier of the query being retrieved carries return direction. C# has one result, so
            /// the last return parameter in document order is the result and every earlier one is an input;
            /// nothing is dropped.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            private IParameter QueryReturnParameter()
            {
                ArgumentNullException.ThrowIfNull(operation);

                return operation.OwnedParameter.LastOrDefault(parameter => parameter.Direction == ParameterDirectionKind.Return);
            }

            /// <summary>
            /// Returns a readable description of an operation for use in error messages.
            /// </summary>
            /// <returns>
            /// The owning service and operation names.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            private string DescribeQualified()
            {
                ArgumentNullException.ThrowIfNull(operation);

                return $"{operation.Class?.Name}.{operation.Describe()}";
            }
        }
    }
}
