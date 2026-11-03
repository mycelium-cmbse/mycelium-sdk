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
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides Mycelium-specific queries for the PIM operations that a service member is generated from.
    /// </summary>
    public static class PimOperationExtensions
    {
        /// <summary>
        /// The suffix appended to every generated service member name.
        /// </summary>
        private const string MethodNameSuffix = "Async";

        /// <param name="operation">
        /// The PIM operation that is queried.
        /// </param>
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
            /// Queries the record that replaces the covered inputs of an operation.
            /// </summary>
            /// <returns>
            /// The record class, or <see langword="null" /> when the operation keeps its declared inputs.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when an input names a one-way attribute of a different type.
            /// </exception>
            public IClass QueryInputRecordClass()
            {
                ArgumentNullException.ThrowIfNull(operation);

                return operation.QueryInputParameters().Any(operation.QueryPopulatesOneWayAttribute) ? operation.QueryResultRecordClass() : null;
            }

            /// <summary>
            /// Determines whether an input is dropped because the record parameter covers it.
            /// </summary>
            /// <param name="parameter">
            /// The input parameter of the operation.
            /// </param>
            /// <returns>
            /// <see langword="true" /> when the operation takes its record and the input names an attribute of it
            /// that does not collect parts created with the record, or is typed by the container that a
            /// back-reference attribute of it points to.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation or <paramref name="parameter" /> is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when an input names a one-way attribute of a different type.
            /// </exception>
            public bool QueryFoldsIntoRecord(IParameter parameter)
            {
                ArgumentNullException.ThrowIfNull(operation);
                ArgumentNullException.ThrowIfNull(parameter);

                var recordClass = operation.QueryInputRecordClass();

                if (recordClass is null)
                {
                    return false;
                }

                if (operation.QueryMatchedAttribute(parameter) is { } attribute)
                {
                    return !attribute.QueryIsComposedPart();
                }

                return recordClass.QueryNonDerivedAttributes().Any(recordAttribute => ReferenceEquals(recordAttribute.Type, parameter.Type) && recordAttribute.QueryIsBackReference());
            }

            /// <summary>
            /// Determines whether the record parameter takes the place of an input in the generated signature.
            /// </summary>
            /// <param name="parameter">
            /// The input parameter of the operation.
            /// </param>
            /// <returns>
            /// <see langword="true" /> when the input is the first, in document order, that the record covers.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation or <paramref name="parameter" /> is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when an input names a one-way attribute of a different type.
            /// </exception>
            public bool QueryIsRecordPosition(IParameter parameter)
            {
                ArgumentNullException.ThrowIfNull(operation);
                ArgumentNullException.ThrowIfNull(parameter);

                return ReferenceEquals(operation.QueryInputParameters().FirstOrDefault(operation.QueryFoldsIntoRecord), parameter);
            }

            /// <summary>
            /// Determines whether an input typed by a record carries content rather than an identifier.
            /// </summary>
            /// <param name="parameter">
            /// The input parameter of the operation.
            /// </param>
            /// <returns>
            /// <see langword="true" /> when the input collects parts created with the returned record.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation or <paramref name="parameter" /> is <see langword="null" />.
            /// </exception>
            public bool QueryCarriesRecordContent(IParameter parameter)
            {
                ArgumentNullException.ThrowIfNull(operation);
                ArgumentNullException.ThrowIfNull(parameter);

                return operation.QueryMatchedAttribute(parameter) is { IsReadOnly: false } attribute && attribute.QueryIsComposedPart();
            }

            /// <summary>
            /// Determines whether an operation changes an existing record rather than creating one.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when an input names a read-only attribute of the returned record, which
            /// identifies the record being changed.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            public bool QueryUpdatesRecord()
            {
                ArgumentNullException.ThrowIfNull(operation);

                return operation.QueryInputParameters().Any(parameter => operation.QueryMatchedAttribute(parameter) is { IsReadOnly: true });
            }

            /// <summary>
            /// Queries the legal C# identifier of the record parameter.
            /// </summary>
            /// <returns>
            /// The camel-cased record name.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the operation keeps its declared inputs, or when the name is already taken by another input.
            /// </exception>
            public string QueryRecordParameterName()
            {
                ArgumentNullException.ThrowIfNull(operation);

                var recordClass = operation.QueryInputRecordClass() ?? throw new InvalidOperationException($"Operation '{operation.Name}' does not take a record.");
                var recordParameterName = ReservedCSharpNameMapper.Map(recordClass.Name.LowerCaseFirstLetter());

                var collidingParameter = operation.QueryInputParameters()
                    .Where(parameter => !operation.QueryFoldsIntoRecord(parameter))
                    .FirstOrDefault(parameter => string.Equals(parameter.Name, recordParameterName, StringComparison.Ordinal));

                return collidingParameter is null ? recordParameterName : throw new InvalidOperationException($"Operation '{operation.Name}' already declares parameter '{collidingParameter.Name}', so the record parameter cannot take that name.");
            }

            /// <summary>
            /// Queries the record attributes that the caller supplies to an operation.
            /// </summary>
            /// <returns>
            /// The writable attributes named by the covered inputs, in document order of those inputs, joined into a
            /// readable list.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when an input names a one-way attribute of a different type.
            /// </exception>
            public string QueryRecordMemberNames()
            {
                ArgumentNullException.ThrowIfNull(operation);

                var memberNames = operation.QueryInputParameters()
                    .Where(operation.QueryFoldsIntoRecord)
                    .Select(operation.QueryMatchedAttribute)
                    .Where(attribute => attribute is { IsReadOnly: false } && !attribute.QueryIsBackReference())
                    .Select(attribute => $"<c>{attribute.Name}</c>")
                    .ToArray();

                return memberNames.Length < 2 ? string.Concat(memberNames) : $"{string.Join(", ", memberNames[..^1])} and {memberNames[^1]}";
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
            /// Determines whether an input populates a writable attribute of the returned record that is navigable
            /// one way only.
            /// </summary>
            /// <param name="parameter">
            /// The input parameter of the operation.
            /// </param>
            /// <returns>
            /// <see langword="true" /> when the input names such an attribute.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the input is typed differently from the attribute it names.
            /// </exception>
            private bool QueryPopulatesOneWayAttribute(IParameter parameter)
            {
                var attribute = operation.QueryMatchedAttribute(parameter);

                if (attribute is null || attribute.IsReadOnly || attribute.QueryNavigableOpposite() is not null)
                {
                    return false;
                }

                return ReferenceEquals(attribute.Type, parameter.Type) ? true : throw new InvalidOperationException($"Parameter '{parameter.Name}' of operation '{operation.Name}' names attribute '{attribute.Name}' of '{attribute.Type?.Name}', but is typed by '{parameter.Type?.Name}'.");
            }

            /// <summary>
            /// Queries the record that an operation returns.
            /// </summary>
            /// <returns>
            /// The record class, or <see langword="null" /> when the result is absent, a collection, or not a
            /// persisted record.
            /// </returns>
            private IClass QueryResultRecordClass()
            {
                var resultParameter = operation.QueryReturnParameters().SingleOrDefault();

                return resultParameter is { Type: IClass resultClass } && !resultParameter.QueryIsEnumerable() && resultClass.QueryIsPersistedRecord() ? resultClass : null;
            }

            /// <summary>
            /// Queries the attribute of the returned record that an input names.
            /// </summary>
            /// <param name="parameter">
            /// The input parameter of the operation.
            /// </param>
            /// <returns>
            /// The attribute named by the input, or by the input with the record name stripped from its front;
            /// <see langword="null" /> when there is none.
            /// </returns>
            private IProperty QueryMatchedAttribute(IParameter parameter)
            {
                var recordClass = operation.QueryResultRecordClass();

                if (recordClass is null)
                {
                    return null;
                }

                var recordPrefix = recordClass.Name.LowerCaseFirstLetter();

                if (recordClass.QueryNonDerivedAttribute(parameter.Name) is { } attribute)
                {
                    return attribute;
                }

                return parameter.Name.Length > recordPrefix.Length && parameter.Name.StartsWith(recordPrefix, StringComparison.Ordinal) ? recordClass.QueryNonDerivedAttribute(parameter.Name[recordPrefix.Length..].LowerCaseFirstLetter()) : null;
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
