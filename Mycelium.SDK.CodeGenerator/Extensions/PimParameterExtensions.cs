// ------------------------------------------------------------------------------------------------
//  <copyright file="PimParameterExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides Mycelium-specific queries for the PIM parameters of a generated service member.
    /// </summary>
    public static class PimParameterExtensions
    {
        /// <summary>
        /// The name of the abstract PIM class that gives a specialization a server-assigned identifier.
        /// </summary>
        private const string IdentityBearingClassName = "Record";

        /// <summary>
        /// The C# type of a parameter reduced to an identifier.
        /// </summary>
        private const string IdentifierTypeName = "Guid";

        /// <summary>
        /// The suffix appended to a parameter reduced to a single identifier.
        /// </summary>
        private const string IdentifierNameSuffix = "Id";

        /// <summary>
        /// The suffix appended to a parameter reduced to a collection of identifiers.
        /// </summary>
        private const string IdentifierCollectionNameSuffix = "Ids";

        /// <summary>
        /// The C# types that the types of the PIM map onto, registered with
        /// <see cref="TypeExtensions.AddOrOverwriteCSharpTypeMappings" />.
        /// </summary>
        /// <remarks>
        /// The UML primitives are already carried by the uml4net default mapping; only the types the PIM
        /// declares for itself are added here. <c>Data</c> is fully qualified because the two services that
        /// use it are the only ones that would import its namespace.
        /// </remarks>
        private static readonly (string Key, string Value)[] PimCSharpTypeMappings =
        [
            ("UUID", "Guid"),
            ("ISO8601DateTime", "DateTime"),
            ("IRI", "Uri"),
            ("Data", "SysML2.NET.Common.IData")
        ];

        /// <summary>
        /// The C# value types that an optional scalar parameter must be made nullable for.
        /// </summary>
        private static readonly HashSet<string> ValueTypeNames = new(StringComparer.Ordinal)
        {
            "bool",
            "byte",
            "sbyte",
            "short",
            "ushort",
            "int",
            "uint",
            "long",
            "ulong",
            "char",
            "float",
            "double",
            "decimal",
            "DateTime",
            "DateTimeOffset",
            "Guid",
            "TimeSpan"
        };

        /// <summary>
        /// Registers the C# type mappings that the data types of the PIM require.
        /// </summary>
        /// <remarks>
        /// The uml4net mapping is process-wide. The three names registered here occur in no other model this
        /// repository generates from, so registering them cannot change the output of another pipeline.
        /// </remarks>
        public static void RegisterPimCSharpTypeMappings() => TypeExtensions.AddOrOverwriteCSharpTypeMappings(PimCSharpTypeMappings);

        /// <summary>
        /// Determines whether a class specializes the abstract identity-bearing PIM class.
        /// </summary>
        /// <param name="umlClass">
        /// The class whose generalization hierarchy is walked.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the class is, or specializes, the identity-bearing class.
        /// </returns>
        private static bool QuerySpecializesIdentityBearingClass(IClass umlClass)
        {
            if (string.Equals(umlClass.Name, IdentityBearingClassName, StringComparison.Ordinal))
            {
                return true;
            }

            return umlClass.Generalization
                .Select(generalization => generalization.General)
                .OfType<IClass>()
                .Any(QuerySpecializesIdentityBearingClass);
        }

        /// <summary>
        /// Maps the cross-document reference of an unresolved parameter type onto a C# type name.
        /// </summary>
        /// <param name="parameter">
        /// The parameter whose type could not be resolved.
        /// </param>
        /// <param name="hyperlinkMappings">
        /// The <c>href</c> and C# type name pairs to map with.
        /// </param>
        /// <returns>
        /// The mapped C# type name.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the pairs are malformed, or when the reference has no mapping.
        /// </exception>
        private static string MapHyperlinkReference(IParameter parameter, IReadOnlyList<string> hyperlinkMappings)
        {
            if (hyperlinkMappings.Count % 2 != 0)
            {
                throw new InvalidOperationException($"Parameter '{parameter.DescribeQualified()}' was supplied an odd number of hyperlink-reference mappings.");
            }

            var hyperlinkReference = parameter.QueryTypeHyperlinkReference();

            for (var index = 0; index < hyperlinkMappings.Count; index += 2)
            {
                if (string.Equals(hyperlinkMappings[index], hyperlinkReference, StringComparison.Ordinal))
                {
                    return hyperlinkMappings[index + 1];
                }
            }

            throw new InvalidOperationException($"Parameter '{parameter.DescribeQualified()}' has no resolved type and its reference '{hyperlinkReference}' has no mapping.");
        }

        extension(IParameter parameter)
        {
            /// <summary>
            /// Determines whether only the identifier of a parameter travels to the service.
            /// </summary>
            /// <param name="operation">
            /// The operation that declares the parameter.
            /// </param>
            /// <param name="contentCarryingNames">
            /// The <c>operation.parameter</c> names that carry content despite being typed by an
            /// identity-bearing class.
            /// </param>
            /// <returns>
            /// <see langword="true" /> when the parameter is reduced to its identifier.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter, <paramref name="operation" /> or
            /// <paramref name="contentCarryingNames" /> is <see langword="null" />.
            /// </exception>
            public bool QueryIsIdentifier(IOperation operation, IReadOnlyCollection<string> contentCarryingNames)
            {
                ArgumentNullException.ThrowIfNull(parameter);
                ArgumentNullException.ThrowIfNull(operation);
                ArgumentNullException.ThrowIfNull(contentCarryingNames);

                return parameter.QueryBearsIdentity() && !contentCarryingNames.Contains(parameter.QueryQualifiedName(operation), StringComparer.Ordinal);
            }

            /// <summary>
            /// Determines whether a parameter requires nullable value-type syntax.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the parameter is an optional scalar whose mapped type is a C#
            /// value type.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter is <see langword="null" />.
            /// </exception>
            public bool QueryIsOptionalValueType()
            {
                ArgumentNullException.ThrowIfNull(parameter);

                return !parameter.QueryIsEnumerable() && parameter.QueryIsNullable() && parameter.QueryIsValueType();
            }

            /// <summary>
            /// Queries the legal C# identifier of a parameter.
            /// </summary>
            /// <returns>
            /// The parameter name, escaped when it is a reserved C# keyword.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the parameter has no name.
            /// </exception>
            public string QueryParameterName()
            {
                ArgumentNullException.ThrowIfNull(parameter);

                return string.IsNullOrWhiteSpace(parameter.Name) ? throw new InvalidOperationException($"Parameter '{parameter.XmiId}' has no name.") : ReservedCSharpNameMapper.Map(parameter.Name);
            }

            /// <summary>
            /// Queries the legal C# identifier of a parameter that is reduced to an identifier.
            /// </summary>
            /// <returns>
            /// The parameter name suffixed with <c>Id</c>, or with <c>Ids</c> when the parameter is a
            /// collection, unless the modeled name already carries that suffix.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the parameter has no name.
            /// </exception>
            public string QueryIdentifierParameterName()
            {
                ArgumentNullException.ThrowIfNull(parameter);

                var parameterName = parameter.QueryParameterName();
                var suffix = parameter.QueryIsEnumerable() ? IdentifierCollectionNameSuffix : IdentifierNameSuffix;

                return parameterName.EndsWith(suffix, StringComparison.Ordinal) ? parameterName : ReservedCSharpNameMapper.Map($"{parameterName}{suffix}");
            }

            /// <summary>
            /// Queries the C# type of a parameter, including collection syntax.
            /// </summary>
            /// <param name="hyperlinkMappings">
            /// The <c>href</c> and C# type name pairs that resolve a cross-document reference the reader
            /// could not resolve.
            /// </param>
            /// <returns>
            /// The corresponding C# type name.
            /// </returns>
            /// <remarks>
            /// Nullability is not applied here: an optional input takes it, a result never does, because a
            /// result that is absent travels as an error rather than as a null value.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter or <paramref name="hyperlinkMappings" /> is
            /// <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the parameter type is unresolved and unmapped, unnamed, or has no C#
            /// representation.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when a modeled type name cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryCSharpTypeName(IReadOnlyList<string> hyperlinkMappings)
            {
                ArgumentNullException.ThrowIfNull(parameter);

                var elementTypeName = parameter.QueryCSharpElementTypeName(hyperlinkMappings);

                return parameter.QueryIsEnumerable() ? $"IReadOnlyList<{elementTypeName}>" : elementTypeName;
            }

            /// <summary>
            /// Queries the C# type of a parameter that is reduced to an identifier.
            /// </summary>
            /// <returns>
            /// <c>Guid</c> or <c>IReadOnlyList&lt;Guid&gt;</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter is <see langword="null" />.
            /// </exception>
            public string QueryIdentifierTypeName()
            {
                ArgumentNullException.ThrowIfNull(parameter);

                return parameter.QueryIsEnumerable() ? $"IReadOnlyList<{IdentifierTypeName}>" : IdentifierTypeName;
            }

            /// <summary>
            /// Queries the scalar C# type name that a parameter type maps onto, before multiplicity and
            /// nullability syntax are applied.
            /// </summary>
            /// <param name="hyperlinkMappings">
            /// The <c>href</c> and C# type name pairs that resolve a cross-document reference the reader
            /// could not resolve.
            /// </param>
            /// <returns>
            /// The corresponding C# type name.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter or <paramref name="hyperlinkMappings" /> is
            /// <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the parameter type is unresolved and unmapped, unnamed, or has no C#
            /// representation.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when a modeled type name cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryCSharpElementTypeName(IReadOnlyList<string> hyperlinkMappings)
            {
                ArgumentNullException.ThrowIfNull(parameter);
                ArgumentNullException.ThrowIfNull(hyperlinkMappings);

                if (parameter.Type is null)
                {
                    return MapHyperlinkReference(parameter, hyperlinkMappings);
                }

                if (string.IsNullOrWhiteSpace(parameter.Type.Name))
                {
                    throw new InvalidOperationException($"Parameter '{parameter.DescribeQualified()}' has an unnamed type.");
                }

                var typeName = parameter.Type.QueryCSharpTypeName();

                // A registered mapping already yields a legal C# type, keyword aliases included.
                if (!string.Equals(typeName, parameter.Type.Name, StringComparison.Ordinal))
                {
                    return typeName;
                }

                // A classifier keeps its modeled name, but a plain data type has no C# form of its own, so
                // one that mapped onto itself was never registered.
                return parameter.Type is IDataType and not (IPrimitiveType or IEnumeration) ? throw new InvalidOperationException($"Parameter '{parameter.DescribeQualified()}' uses data type '{parameter.Type.Name}', for which no C# type mapping is registered.") : ReservedCSharpNameMapper.Map(typeName);
            }

            /// <summary>
            /// Determines whether the type of a parameter carries a server-assigned identifier.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the parameter type specializes the abstract <c>Record</c> class.
            /// </returns>
            /// <remarks>
            /// A <c>Record</c> specialization is server-managed state that the caller names rather than
            /// supplies, so the service layer accepts its identifier instead of the whole object. The types
            /// that realize <c>Data</c> are versioned content and travel whole.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter is <see langword="null" />.
            /// </exception>
            private bool QueryBearsIdentity()
            {
                ArgumentNullException.ThrowIfNull(parameter);

                return parameter.Type is IClass umlClass && QuerySpecializesIdentityBearingClass(umlClass);
            }

            /// <summary>
            /// Queries the name of a parameter qualified by the operation that declares it.
            /// </summary>
            /// <param name="operation">
            /// The operation that declares the parameter.
            /// </param>
            /// <returns>
            /// The <c>operation.parameter</c> name.
            /// </returns>
            /// <remarks>
            /// The declaring operation is supplied rather than read from <see cref="IParameter.Operation" />,
            /// which the XMI reader does not populate.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter or <paramref name="operation" /> is <see langword="null" />.
            /// </exception>
            private string QueryQualifiedName(IOperation operation)
            {
                ArgumentNullException.ThrowIfNull(parameter);
                ArgumentNullException.ThrowIfNull(operation);

                return $"{operation.Name}.{parameter.Name}";
            }

            /// <summary>
            /// Queries the cross-document reference that a parameter type could not be resolved from.
            /// </summary>
            /// <returns>
            /// The unresolved <c>href</c>, or an empty string when the parameter type resolved.
            /// </returns>
            /// <remarks>
            /// The PIM types four <c>ElementNavigationService</c> results by <c>href</c> into a
            /// <c>KerML Abstract Syntax.xml</c> companion export that OMG does not publish, so the reader
            /// leaves those types unresolved and the reference itself is the only usable key.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter is <see langword="null" />.
            /// </exception>
            private string QueryTypeHyperlinkReference()
            {
                ArgumentNullException.ThrowIfNull(parameter);

                if (parameter.Type is not null)
                {
                    return string.Empty;
                }

                return parameter.SingleValueReferencePropertyIdentifiers.TryGetValue("type", out var hyperlinkReference) ? hyperlinkReference : string.Empty;
            }

            /// <summary>
            /// Determines whether the scalar C# type of a parameter is a value type, and therefore requires
            /// nullable syntax when the parameter is optional.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the mapped scalar type is a C# value type.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter is <see langword="null" />.
            /// </exception>
            private bool QueryIsValueType()
            {
                ArgumentNullException.ThrowIfNull(parameter);

                if (parameter.QueryBearsIdentity() || parameter.Type is IEnumeration)
                {
                    return true;
                }

                return parameter.Type is IDataType && ValueTypeNames.Contains(parameter.QueryCSharpElementTypeName([]));
            }

            /// <summary>
            /// Returns a readable description of a parameter for use in error messages.
            /// </summary>
            /// <returns>
            /// The parameter name and its XMI identifier.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the parameter is <see langword="null" />.
            /// </exception>
            private string DescribeQualified()
            {
                ArgumentNullException.ThrowIfNull(parameter);

                return $"{parameter.Describe()} ({parameter.XmiId})";
            }
        }
    }
}
