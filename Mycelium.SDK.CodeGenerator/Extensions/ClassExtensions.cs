// ------------------------------------------------------------------------------------------------
//  <copyright file="ClassExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using System.Text.RegularExpressions;

    using HandlebarsDotNet.Helpers.Utils;

    using uml4net.Classification;
    using uml4net.CommonStructure;
    using uml4net.Extensions;
    using uml4net.Packages;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    /// <summary>
    /// Provides artifact-specific queries for UML classes.
    /// </summary>
    public static partial class ClassExtensions
    {
        /// <summary>
        /// The OCL operation that, together with <see cref="IsUniqueOperation" />, marks an invariant as stating uniqueness across every instance
        /// of a class.
        /// </summary>
        private const string AllInstancesOperation = "allInstances()";

        /// <summary>
        /// The OCL operation that, together with <see cref="AllInstancesOperation" />, marks an invariant as stating uniqueness across every
        /// instance of a class.
        /// </summary>
        private const string IsUniqueOperation = "isUnique";

        /// <summary>
        /// Matches one uniqueness invariant in either recognized form: the explicit iterator
        /// <c>inv &lt;name&gt;: &lt;Class&gt;.allInstances()-&gt;isUnique(v | v.&lt;property&gt;)</c> or the implicit iterator
        /// <c>inv &lt;name&gt;: &lt;Class&gt;.allInstances()-&gt;isUnique(&lt;property&gt;)</c>.
        /// </summary>
        /// <returns>
        /// The compiled pattern, with the <c>class</c> and <c>property</c> groups.
        /// </returns>
        [GeneratedRegex(@"^inv\s+\w+\s*:\s*(?<class>\w+)\.allInstances\(\)\s*->\s*isUnique\(\s*" +
            @"(?:(?<variable>\w+)\s*\|\s*\k<variable>\.)?(?<property>\w+)\s*\)$", RegexOptions.CultureInvariant)]
        private static partial Regex UniquenessInvariantPattern();

        /// <summary>
        /// Matches the position at which each invariant of an OCL body starts, so that a body holding several invariants is read one invariant
        /// at a time.
        /// </summary>
        /// <returns>
        /// The compiled zero-width pattern.
        /// </returns>
        [GeneratedRegex(@"(?=\binv\s+\w+\s*:)", RegexOptions.CultureInvariant)]
        private static partial Regex InvariantStartPattern();

        /// <summary>
        /// Queries the invariants of a constraint that state uniqueness across every instance of a class: those containing both
        /// <c>allInstances()</c> and <c>isUnique</c>. Each body is decoded once, as documentation is; its OCL comment lines are removed and its
        /// remaining lines are joined, so an invariant may span several lines. The text is only compared; no OCL is parsed or evaluated.
        /// </summary>
        /// <param name="constraint">
        /// The constraint whose opaque-expression bodies are read.
        /// </param>
        /// <returns>
        /// The text of each uniqueness invariant, on one line.
        /// </returns>
        private static IEnumerable<string> QueryUniquenessInvariants(IConstraint constraint)
        {
            return constraint.Specification.OfType<IOpaqueExpression>()
                .SelectMany(expression => expression.Body)
                .SelectMany(QueryInvariants)
                .Where(invariant => invariant.Contains(AllInstancesOperation, StringComparison.Ordinal) && invariant.Contains(IsUniqueOperation, StringComparison.Ordinal));
        }

        /// <summary>
        /// Splits an OCL body into its invariants, each on one line and without comment lines.
        /// </summary>
        /// <param name="body">
        /// The body as exported, possibly holding OCL comment lines and invariants that span several lines.
        /// </param>
        /// <returns>
        /// The text of each invariant of the body.
        /// </returns>
        private static IEnumerable<string> QueryInvariants(string body)
        {
            var statementLines = HtmlUtils.HtmlDecode(body)
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => line.Length > 0 && !line.StartsWith("--", StringComparison.Ordinal));

            return InvariantStartPattern()
                .Split(string.Join(" ", statementLines))
                .Select(invariant => invariant.Trim())
                .Where(invariant => invariant.Length > 0);
        }

        /// <summary>
        /// Resolves a uniqueness invariant to the «index» property whose uniqueness it states.
        /// </summary>
        /// <param name="umlClass">
        /// The class that owns the invariant.
        /// </param>
        /// <param name="constraint">
        /// The constraint that holds the invariant, named in the failure message.
        /// </param>
        /// <param name="invariant">
        /// The text of the uniqueness invariant, on one line.
        /// </param>
        /// <returns>
        /// The «index» property declared by <paramref name="umlClass" /> that the invariant names.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the invariant has neither recognized form, names another class, or names a property that is not an «index» property
        /// declared by <paramref name="umlClass" />.
        /// </exception>
        private static IProperty QueryUniquelyIndexedProperty(IClass umlClass, IConstraint constraint, string invariant)
        {
            var match = UniquenessInvariantPattern().Match(invariant);

            var property = match.Success && string.Equals(match.Groups["class"].Value, umlClass.Name, StringComparison.Ordinal)
                ? umlClass.QueryDtoInterfaceProperties()
                    .FirstOrDefault(candidate => string.Equals(candidate.Name, match.Groups["property"].Value, StringComparison.Ordinal) && candidate.QueryIsIndexed())
                : null;

            return property ?? throw new InvalidOperationException(
                $"Invariant '{constraint.Describe()}' of class '{umlClass.Describe()}' states uniqueness across all instances ('{invariant}'), but does not " +
                $"resolve to an «index» property declared by that class. The recognized forms are '{umlClass.Name}.allInstances()->isUnique(v | v.<property>)' " +
                $"and '{umlClass.Name}.allInstances()->isUnique(<property>)'.");
        }

        /// <summary>
        /// Queries every class of the model that contains an element, by walking up to the outermost package and through its nested packages.
        /// </summary>
        /// <param name="element">
        /// An element of the model whose classes are queried.
        /// </param>
        /// <returns>
        /// The classes of every package of the model; empty when the element is not contained in a package.
        /// </returns>
        private static IEnumerable<IClass> QueryModelClasses(IElement element)
        {
            var root = element;

            while (root.Owner != null)
            {
                root = root.Owner;
            }

            return root is IPackage package
                ? package.QueryPackages().SelectMany(nestedPackage => nestedPackage.PackagedElement.OfType<IClass>())
                : [];
        }

        /// <summary>
        /// Determines whether a reference declared by a class makes the objects of an inheritance chain require a composite owner, by the rule
        /// of the schema's mandatory-owner check: a part-held reference to a composite owner is mandatory when it is not optional, and an
        /// owner-held composite reference stored in a column or a link table is mandatory when its owner end has a lower bound of at least one.
        /// </summary>
        /// <param name="declaringClass">
        /// The class that declares the reference.
        /// </param>
        /// <param name="property">
        /// The reference declared by <paramref name="declaringClass" />.
        /// </param>
        /// <param name="chain">
        /// The inheritance chain of the class whose ownership is queried.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the reference is a mandatory ownership whose part type belongs to <paramref name="chain" />.
        /// </returns>
        private static bool QueryIsMandatoryOwnership(IClass declaringClass, IProperty property, IReadOnlyList<IClass> chain)
        {
            if (property.QueryIsEnumerable() && !property.QuerySqlRequiresLinkTable())
            {
                return false;
            }

            var ownerEnd = property.QueryIsOwnerEnd();

            if (!ownerEnd && !property.IsComposite)
            {
                return false;
            }

            var partType = ownerEnd ? declaringClass : property.Type;
            var required = ownerEnd ? !property.QueryIsNullable() : property.Opposite is { Lower: > 0 };

            return required && chain.Any(chainClass => string.Equals(chainClass.XmiId, partType?.XmiId, StringComparison.Ordinal));
        }

        /// <summary>
        /// Deduplicates and deterministically orders resolved direct UML generalizations.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose direct generalizations are queried.
        /// </param>
        /// <returns>
        /// The distinct direct UML classes in deterministic order.
        /// </returns>
        private static IClass[] QueryDirectGeneralizations(IClass umlClass)
        {
            return umlClass.SuperClass
                .GroupBy(generalClass => generalClass.XmiId, StringComparer.Ordinal)
                .Select(group => group.First())
                .OrderBy(generalClass => generalClass.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(generalClass => generalClass.Name, StringComparer.Ordinal)
                .ThenBy(generalClass => generalClass.XmiId, StringComparer.Ordinal)
                .ToArray();
        }

        /// <summary>
        /// Determines whether a property belongs to the generated DTO contract.
        /// </summary>
        /// <param name="property">
        /// The UML property to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the property is neither derived nor a
        /// derived union; otherwise, <see langword="false" />.
        /// </returns>
        private static bool IsDtoProperty(IProperty property) => !property.IsDerived && !property.IsDerivedUnion;

        /// <summary>
        /// Removes duplicate properties and orders them deterministically for generation.
        /// </summary>
        /// <param name="properties">
        /// The properties to deduplicate and order.
        /// </param>
        /// <returns>
        /// The deterministically ordered properties.
        /// </returns>
        private static IEnumerable<IProperty> OrderProperties(IEnumerable<IProperty> properties)
        {
            var propertyIds = new HashSet<string>(StringComparer.Ordinal);

            var distinctProperties = properties
                .Where(property => propertyIds.Add(property.XmiId))
                .ToArray();

            return distinctProperties
                .OrderBy(property => string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(property => property.Name, StringComparer.Ordinal)
                .ThenBy(property => property.XmiId, StringComparer.Ordinal);
        }

        /// <param name="umlClass">
        /// The UML class that is queried.
        /// </param>
        extension(IClass umlClass)
        {
            /// <summary>
            /// Queries the named references declared directly by the class for relational SQL storage.
            /// </summary>
            /// <returns>
            /// The non-derived class-typed properties, including scalar and collection references,
            /// deduplicated by XMI identifier and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QuerySqlReferenceProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return OrderProperties(umlClass.OwnedAttribute.Where(property =>
                        property.Type is IClass && !string.IsNullOrWhiteSpace(property.Name) && !property.IsDerived && !property.IsDerivedUnion))
                    .ToArray();
            }

            /// <summary>
            /// Queries the scalar references declared directly by the class that are stored as UUID columns of its SQL table.
            /// </summary>
            /// <returns>
            /// The single-valued SQL reference properties, in the deterministic order of <c>QuerySqlReferenceProperties</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QuerySqlColumnProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QuerySqlReferenceProperties()
                    .Where(property => !property.QueryIsEnumerable())
                    .ToArray();
            }

            /// <summary>
            /// Queries the collections declared directly by the class that are stored in their own SQL link tables.
            /// </summary>
            /// <returns>
            /// The many-valued SQL reference properties without an eligible scalar opposite, in the deterministic order of
            /// <c>QuerySqlReferenceProperties</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QuerySqlLinkProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QuerySqlReferenceProperties()
                    .Where(property => property.QuerySqlRequiresLinkTable())
                    .ToArray();
            }

            /// <summary>
            /// Queries the class and its ancestors in the order their SQL table rows are inserted, from the root down to the class.
            /// </summary>
            /// <returns>
            /// The single-inheritance chain from the root class to this class, both included.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IClass> QuerySqlInheritanceChain()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryAllGeneralClassifiers()
                    .OfType<IClass>()
                    .Reverse()
                    .ToArray();
            }

            /// <summary>
            /// Queries every SQL reference property of the class and its ancestors, which the DAO keeps out of the stored JSON document.
            /// </summary>
            /// <returns>
            /// The SQL reference properties of each class of the inheritance chain, from the root down to the class.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QuerySqlChainReferenceProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QuerySqlInheritanceChain()
                    .SelectMany(chainClass => chainClass.QuerySqlReferenceProperties())
                    .ToArray();
            }

            /// <summary>
            /// Queries every reference of the class and its ancestors that the class's DAO writes relationally: the scalar reference columns and the
            /// link-table collections. Collections stored through a scalar opposite are excluded, because the DAO of the opposite class writes them.
            /// </summary>
            /// <returns>
            /// The scalar column and link-table properties of each class of the inheritance chain, from the root down to the class; empty when the
            /// DAO has no relational state to update.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QuerySqlChainRelationProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QuerySqlInheritanceChain()
                    .SelectMany(chainClass => chainClass.QuerySqlColumnProperties().Concat(chainClass.QuerySqlLinkProperties()))
                    .ToArray();
            }

            /// <summary>
            /// Queries the scalar reference columns declared directly by the class that an update writes: every column except those of managed
            /// references assigned on creation only, which keep their stored value.
            /// </summary>
            /// <returns>
            /// The updatable column properties, in the deterministic order of <c>QuerySqlColumnProperties</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QuerySqlUpdatableColumnProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QuerySqlColumnProperties()
                    .Where(property => !property.QueryIsAssignedOnCreateOnly())
                    .ToArray();
            }

            /// <summary>
            /// Queries every reference of the class and its ancestors that the class's DAO writes relationally on update: the updatable scalar
            /// reference columns and the link-table collections.
            /// </summary>
            /// <returns>
            /// The updatable column and link-table properties of each class of the inheritance chain, from the root down to the class; empty when
            /// an update has no relational state to write.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QuerySqlChainUpdatableRelationProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QuerySqlInheritanceChain()
                    .SelectMany(chainClass => chainClass.QuerySqlUpdatableColumnProperties().Concat(chainClass.QuerySqlLinkProperties()))
                    .ToArray();
            }

            /// <summary>
            /// Queries the managed references of the class and its ancestors to which the server assigns the acting user's identifier when an
            /// object is created.
            /// </summary>
            /// <returns>
            /// The single managed references whose <c>onAction</c> includes <c>OnCreate</c>, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when a managed property with an <c>onAction</c> value is neither a single reference nor a single date and time value.
            /// </exception>
            public IReadOnlyList<IProperty> QueryManagedReferencesAssignedOnCreate()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryDtoImplementationProperties()
                    .Where(property => property.QueryIsAssignedOnCreate() && property.QueryIsAssignedReference())
                    .ToArray();
            }

            /// <summary>
            /// Queries the managed references of the class and its ancestors to which the server assigns the acting user's identifier when an
            /// object is updated.
            /// </summary>
            /// <returns>
            /// The single managed references whose <c>onAction</c> includes <c>OnUpdate</c>, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when a managed property with an <c>onAction</c> value is neither a single reference nor a single date and time value.
            /// </exception>
            public IReadOnlyList<IProperty> QueryManagedReferencesAssignedOnUpdate()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryDtoImplementationProperties()
                    .Where(property => property.QueryIsAssignedOnUpdate() && property.QueryIsAssignedReference())
                    .ToArray();
            }

            /// <summary>
            /// Queries the managed date and time values of the class and its ancestors to which the database assigns the transaction time when an
            /// object is created.
            /// </summary>
            /// <returns>
            /// The single managed date and time values whose <c>onAction</c> includes <c>OnCreate</c>, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when a managed property with an <c>onAction</c> value is neither a single reference nor a single date and time value.
            /// </exception>
            public IReadOnlyList<IProperty> QueryManagedTimestampsAssignedOnCreate()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryDtoImplementationProperties()
                    .Where(property => property.QueryIsAssignedOnCreate() && property.QueryIsAssignedTimestamp())
                    .ToArray();
            }

            /// <summary>
            /// Queries the managed date and time values of the class and its ancestors that have an <c>onAction</c> value. An update writes each of
            /// them in the JSON document: the transaction time when the value is assigned on update, and its stored value when it is assigned on
            /// creation only.
            /// </summary>
            /// <returns>
            /// The single managed date and time values that have an <c>onAction</c> value, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when a managed property with an <c>onAction</c> value is neither a single reference nor a single date and time value.
            /// </exception>
            public IReadOnlyList<IProperty> QueryManagedTimestamps()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryDtoImplementationProperties()
                    .Where(property => property.QueryIsAssignedTimestamp())
                    .ToArray();
            }

            /// <summary>
            /// Queries the «index» properties declared by the class whose uniqueness one of its invariants states across every instance of the
            /// class. Every invariant of the class that contains both <c>allInstances()</c> and <c>isUnique</c> must resolve to such a property,
            /// in the explicit form <c>&lt;Class&gt;.allInstances()-&gt;isUnique(v | v.&lt;property&gt;)</c> or the implicit form
            /// <c>&lt;Class&gt;.allInstances()-&gt;isUnique(&lt;property&gt;)</c>. The invariant is compared as text; no OCL is parsed or evaluated.
            /// </summary>
            /// <returns>
            /// The uniquely indexed properties, in the order of the invariants that name them; empty when the class states no such uniqueness.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when a uniqueness invariant of the class has neither recognized form, names another class, or names a property that is not
            /// an «index» property declared by the class.
            /// </exception>
            public IReadOnlyList<IProperty> QueryUniquelyIndexedProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.OwnedRule
                    .SelectMany(constraint => QueryUniquenessInvariants(constraint)
                        .Select(invariant => QueryUniquelyIndexedProperty(umlClass, constraint, invariant)))
                    .ToArray();
            }

            /// <summary>
            /// Queries the root class of the inheritance chain, whose table holds the identity, discriminator and JSON document of every object.
            /// </summary>
            /// <returns>
            /// The single ancestor without a parent, or the class itself when it has no parent.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the class's ancestors do not lead to exactly one class without a parent.
            /// </exception>
            public IClass QuerySqlRootClass()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                var roots = umlClass.QueryAllGeneralClassifiers()
                    .OfType<IClass>()
                    .Where(ancestor => ancestor.Generalization.Count == 0)
                    .DistinctBy(ancestor => ancestor.XmiId, StringComparer.Ordinal)
                    .ToArray();

                return roots.Length == 1
                    ? roots[0]
                    : throw new InvalidOperationException(
                        $"Class '{umlClass.Name}' must lead to exactly one root class, but its ancestors lead to {roots.Length}: " +
                        $"{string.Join(", ", roots.Select(root => root.Name))}.");
            }

            /// <summary>
            /// Queries the names of every parameter that the class's DAO sends, failing when two of them would be the same.
            /// </summary>
            /// <returns>
            /// The identity, identifier-list, index-value, discriminator and document parameter names, followed by the scalar reference column and link-table collection parameter
            /// names of the inheritance chain, from the root down to the class.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when two parameters of the DAO would have the same name; the message names both model elements.
            /// </exception>
            public IReadOnlyList<string> QuerySqlParameterNames()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                (string Name, string Element)[] fixedParameters =
                [
                    (NamedElementExtensions.SqlIdentityColumnName, "the identity"),
                    (NamedElementExtensions.SqlIdentitiesParameterName, "the identifier list"),
                    (NamedElementExtensions.SqlIndexValueParameterName, "the searched index value"),
                    (NamedElementExtensions.SqlClassKindColumnName, "the discriminator"),
                    (NamedElementExtensions.SqlDataColumnName, "the JSON document")
                ];

                var referenceParameters = umlClass.QuerySqlChainRelationProperties()
                    .Select(property => (Name: property.QuerySqlParameterName(), Element: $"property '{((IClass)property.Owner).Name}.{property.Name}'"));

                var parameters = fixedParameters.Concat(referenceParameters).ToArray();

                var collision = parameters.GroupBy(parameter => parameter.Name, StringComparer.Ordinal)
                    .Select(group => group.ToArray())
                    .FirstOrDefault(group => group.Length > 1);

                if (collision != null)
                {
                    throw new InvalidOperationException(
                        $"The DAO parameter '{collision[0].Name}' of class '{umlClass.Name}' is generated for both {collision[0].Element} and {collision[1].Element}. " +
                        "Rename one of them in the model; the generator does not add disambiguating suffixes.");
                }

                return [.. parameters.Select(parameter => parameter.Name)];
            }

            /// <summary>
            /// Queries the «index» properties of the class and its ancestors, which a read by index searches by their modeled name.
            /// </summary>
            /// <returns>
            /// The indexed properties, deduplicated and deterministically ordered; empty when the class has none.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryIndexedProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryDtoImplementationProperties()
                    .Where(property => property.QueryIsIndexed())
                    .ToArray();
            }

            /// <summary>
            /// Queries the references of the class and its ancestors that point to a composite owner: the single navigable opposites of composite
            /// properties.
            /// </summary>
            /// <returns>
            /// The owner references, deduplicated and deterministically ordered; empty when the class has none.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryOwnerReferenceProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryDtoImplementationProperties()
                    .Where(property => property.Type is IClass && property.QueryIsOwnerEnd())
                    .ToArray();
            }

            /// <summary>
            /// Queries the composite properties of the class and its ancestors, which hold the class's parts.
            /// </summary>
            /// <returns>
            /// The composite references, scalar or collection, deduplicated and deterministically ordered; empty when the class has none.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryPartProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryDtoImplementationProperties()
                    .Where(property => property.Type is IClass && property.IsComposite)
                    .ToArray();
            }

            /// <summary>
            /// Queries the references of the class and its ancestors that are neither composite nor a reference to a composite owner.
            /// </summary>
            /// <returns>
            /// The other references, deduplicated and deterministically ordered; empty when the class has none.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryNonCompositionReferenceProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryDtoImplementationProperties()
                    .Where(property => property.Type is IClass && !property.IsComposite && !property.QueryIsOwnerEnd())
                    .ToArray();
            }

            /// <summary>
            /// Determines whether an object of the class is required by modeled multiplicity to have a composite owner, by the rule of the schema's
            /// mandatory-owner check applied to every reference of the model.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when a mandatory ownership of the model has the class or one of its ancestors as part type.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public bool QueryRequiresOwner()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                var chain = umlClass.QuerySqlInheritanceChain();

                return QueryModelClasses(umlClass)
                    .Any(modelClass => modelClass.QuerySqlReferenceProperties().Any(property => QueryIsMandatoryOwnership(modelClass, property, chain)));
            }

            /// <summary>
            /// Queries the composite properties of the class and its ancestors whose lower bound requires at least one part, which the server
            /// creates when an object is created without them. Every required value of each part must have a source.
            /// </summary>
            /// <returns>
            /// The required composite properties, deduplicated and deterministically ordered; empty when the class has none.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when a required composite property has an abstract type, or when a required value of its part has no source; the message names
            /// the property.
            /// </exception>
            public IReadOnlyList<IProperty> QueryRequiredPartProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                var requiredParts = umlClass.QueryPartProperties()
                    .Where(property => property.Lower > 0)
                    .ToArray();

                foreach (var property in requiredParts)
                {
                    _ = property.QueryCreatedPartValueProperties();
                }

                return requiredParts;
            }

            /// <summary>
            /// Queries the properties declared directly by the class.
            /// </summary>
            /// <returns>
            /// The directly owned properties, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryDtoInterfaceProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return OrderProperties(umlClass.OwnedAttribute.Where(IsDtoProperty))
                    .ToArray();
            }

            /// <summary>
            /// Queries the properties declared directly by the class for the POCO interface contract.
            /// </summary>
            /// <returns>
            /// The directly owned POCO properties, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryPocoInterfaceProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return OrderProperties(umlClass.OwnedAttribute)
                    .ToArray();
            }

            /// <summary>
            /// Queries every property that a concrete DTO implementation must
            /// implement, including inherited properties.
            /// </summary>
            /// <returns>
            /// The direct and inherited properties, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryDtoImplementationProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return OrderProperties(umlClass.QueryAllProperties()
                    .Where(IsDtoProperty))
                    .ToArray();
            }

            /// <summary>
            /// Queries every property that a concrete POCO implementation must implement, including inherited
            /// properties.
            /// </summary>
            /// <returns>
            /// The complete direct and inherited POCO property contract, deduplicated and deterministically
            /// ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryPocoImplementationProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return OrderProperties(umlClass.QueryAllProperties())
                    .ToArray();
            }

            /// <summary>
            /// Queries the direct UML generalizations used for generated interface inheritance.
            /// </summary>
            /// <returns>
            /// The distinct direct generalizations in deterministic order.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IClass> QueryGeneralizations()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return QueryDirectGeneralizations(umlClass);
            }

            /// <summary>
            /// Queries the non-derived attributes of a class and of its generalizations.
            /// </summary>
            /// <returns>
            /// The attributes in declaration order, own attributes before inherited ones.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IEnumerable<IProperty> QueryNonDerivedAttributes()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.OwnedAttribute
                    .Where(property => !property.IsDerived)
                    .Concat(umlClass.Generalization
                        .Select(generalization => generalization.General)
                        .OfType<IClass>()
                        .SelectMany(general => general.QueryNonDerivedAttributes()));
            }

            /// <summary>
            /// Queries a non-derived attribute of a class or of its generalizations, the most specific declaration first.
            /// </summary>
            /// <param name="attributeName">
            /// The attribute name.
            /// </param>
            /// <returns>
            /// The attribute, or <see langword="null" /> when the class declares none by that name.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IProperty QueryNonDerivedAttribute(string attributeName)
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryNonDerivedAttributes().FirstOrDefault(property => string.Equals(property.Name, attributeName, StringComparison.Ordinal));
            }
        }
    }
}
