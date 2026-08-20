// ------------------------------------------------------------------------------------------------
//  <copyright file="XmiReaderResultExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using System.Globalization;
    using System.Text;

    using Microsoft.Extensions.Logging.Abstractions;

    using uml4net;
    using uml4net.Classification;
    using uml4net.CommonStructure;
    using uml4net.Extensions;
    using uml4net.Packages;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi;
    using uml4net.xmi.Extensions.EnterpriseArchitect.Extender;
    using uml4net.xmi.Extensions.EnterpriseArchitect.Structure.Readers;
    using uml4net.xmi.Readers;
    using uml4net.xmi.Settings;

    /// <summary>
    /// Provides the canonical FunctionalData loading, package-selection, and semantic-validation path.
    /// </summary>
    public static class XmiReaderResultExtensions
    {
        /// <summary>
        /// The exact name of the reviewed UML package containing the FunctionalData model.
        /// </summary>
        public const string FunctionalDataPackageName = "FunctionalData";

        /// <summary>
        /// The canonical URI used by the FunctionalData model to reference the standard UML primitive types.
        /// </summary
        private const string PrimitiveTypesUri = "http://www.omg.org/spec/UML/20161101/PrimitiveTypes.xmi";

        /// <summary>
        /// The exact XMI resource file names required for canonical offline FunctionalData loading.
        /// </summary>
        private static readonly string[] RequiredResourceFileNames =
        [
            "CSharp_Primitives.xmi",
            "FunctionalData.xmi",
            "PrimitiveTypes.xmi"
        ];

        /// <summary>
        /// The reviewed names of all UML classes expected in the FunctionalData model.
        /// </summary>
        private static readonly string[] ExpectedClassNames =
        [
            "AuditableThing",
            "BranchProtectionRule",
            "Comment",
            "FunctionalProject",
            "FunctionalProjectPolicy",
            "Organization",
            "OrganizationMember",
            "OrganizationPolicy",
            "Ownership",
            "ProjectMember",
            "Review",
            "Thing",
            "User"
        ];

        /// <summary>
        /// The reviewed names of the abstract UML classes expected in the FunctionalData model.
        /// </summary>
        private static readonly string[] ExpectedAbstractClassNames =
        [
            "AuditableThing",
            "Thing"
        ];

        /// <summary>
        /// The reviewed names of the concrete UML classes expected in the FunctionalData model.
        /// </summary>
        private static readonly string[] ExpectedConcreteClassNames =
        [
            "BranchProtectionRule",
            "Comment",
            "FunctionalProject",
            "FunctionalProjectPolicy",
            "Organization",
            "OrganizationMember",
            "OrganizationPolicy",
            "Ownership",
            "ProjectMember",
            "Review",
            "User"
        ];

        private static readonly string[] ExpectedEnumerationNames =
        [
            "ActivationStatus",
            "CommentStatus",
            "OrganizationMembershipRole",
            "ProjectLifecycleKind",
            "ProjectMemberRole",
            "ProjectMode",
            "ProjectVisibility",
            "ReviewStatus"
        ];

        private static readonly IReadOnlyDictionary<string, string[]> ExpectedLiteralNames =
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                ["ActivationStatus"] =
                [
                    "Active",
                    "Pending",
                    "Suspended",
                    "Archived",
                    "Deleted"
                ],
                ["CommentStatus"] =
                [
                    "Open",
                    "Resolved"
                ],
                ["OrganizationMembershipRole"] =
                [
                    "Administrator",
                    "Member",
                    "Owner"
                ],
                ["ProjectLifecycleKind"] =
                [
                    "Preparation",
                    "Open",
                    "Review",
                    "Archived"
                ],
                ["ProjectMemberRole"] =
                [
                    "Administrator",
                    "Participant",
                    "Viewer"
                ],
                ["ProjectMode"] =
                [
                    "Regular",
                    "Concurrent"
                ],
                ["ProjectVisibility"] =
                [
                    "Private",
                    "Organization",
                    "Public"
                ],
                ["ReviewStatus"] =
                [
                    "Draft",
                    "Ready",
                    "Approved",
                    "ChangesRequested",
                    "Closed"
                ]
            };

        private static readonly IReadOnlyDictionary<string, string> ExpectedPrimitiveMappings =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Boolean"] = "bool",
                ["Integer"] = "int",
                ["Real"] = "double",
                ["String"] = "string",
                ["UnlimitedNatural"] = "string",
                ["DateTime"] = "DateTime",
                ["Dictionary<string,string>"] = "Dictionary<string,string>",
                ["Guid"] = "Guid",
                ["Uri"] = "Uri"
            };

        private static readonly string[] ExpectedStandardPrimitiveNames =
        [
            "Boolean",
            "Integer",
            "Real",
            "String",
            "UnlimitedNatural"
        ];

        private static readonly string[] ExpectedCustomPrimitiveNames =
        [
            "DateTime",
            "Dictionary<string,string>",
            "Guid",
            "Uri"
        ];

        private static readonly string[] ExpectedAssociationSignatures =
        [
            CreateAssociationSignature("AuditableThing", null, "User", "updatedBy"),
            CreateAssociationSignature("AuditableThing", null, "User", "createdBy"),
            CreateAssociationSignature("BranchProtectionRule", null, "User", "defaultReviewers"),
            CreateAssociationSignature("FunctionalProject", null, "BranchProtectionRule", "branchRules"),
            CreateAssociationSignature("Comment", null, "User", "author"),
            CreateAssociationSignature("Comment", null, "Comment", "quotes"),
            CreateAssociationSignature("Comment", null, "Comment", "replies"),
            CreateAssociationSignature("Review", null, "Comment", "comments"),
            CreateAssociationSignature("Organization", "belongsTo", "FunctionalProject", "projects"),
            CreateAssociationSignature("FunctionalProject", null, "Ownership", "defines"),
            CreateAssociationSignature("FunctionalProject", "isPartOf", "ProjectMember", "involves"),
            CreateAssociationSignature("FunctionalProject", null, "FunctionalProjectPolicy", "policy"),
            CreateAssociationSignature("FunctionalProject", null, "Review", "reviews"),
            CreateAssociationSignature("Organization", "organization", "OrganizationMember", "involvedUser"),
            CreateAssociationSignature("Organization", null, "OrganizationPolicy", "policy"),
            CreateAssociationSignature("OrganizationMember", "isPartOfOrganizations", "User", "user"),
            CreateAssociationSignature("Ownership", "owns", "ProjectMember", null),
            CreateAssociationSignature("Ownership", "activeOwnership", "ProjectMember", null),
            CreateAssociationSignature("ProjectMember", "isPartOfProjects", "User", "user"),
            CreateAssociationSignature("Review", null, "User", "reviewers"),
            CreateAssociationSignature("Review", null, "User", "author")
        ];

        /// <summary>
        /// Creates the canonical offline reader settings for FunctionalData.
        /// </summary>
        /// <param name="resourcesDirectory">
        /// The directory containing the three reviewed XMI resources.
        /// </param>
        /// <returns>
        /// Reader settings that resolve every FunctionalData dependency locally.
        /// </returns>
        public static DefaultSettings CreateFunctionalDataReaderSettings(
            DirectoryInfo resourcesDirectory)
        {
            ArgumentNullException.ThrowIfNull(resourcesDirectory);

            return new DefaultSettings
            {
                LocalReferenceBasePath = resourcesDirectory.FullName,
                PathMaps =
                {
                    [PrimitiveTypesUri] =
                        Path.Combine(resourcesDirectory.FullName, "PrimitiveTypes.xmi")
                },
                UseStrictReading = false
            };
        }

        /// <summary>
        /// Loads and validates the reviewed FunctionalData model through the canonical production path.
        /// </summary>
        /// <param name="resourcesDirectory">
        /// The directory containing the reviewed XMI resources.
        /// </param>
        /// <returns>
        /// The validated XMI reader result.
        /// </returns>
        public static XmiReaderResult ReadFunctionalData(
            DirectoryInfo resourcesDirectory)
        {
            ArgumentNullException.ThrowIfNull(resourcesDirectory);

            if (!resourcesDirectory.Exists)
            {
                throw new DirectoryNotFoundException(
                    $"The FunctionalData resources directory '{resourcesDirectory.FullName}' does not exist.");
            }

            foreach (var resourceFileName in RequiredResourceFileNames)
            {
                var resourcePath =
                    Path.Combine(resourcesDirectory.FullName, resourceFileName);

                if (!File.Exists(resourcePath))
                {
                    throw new FileNotFoundException(
                        $"Required FunctionalData resource '{resourceFileName}' was not found.",
                        resourcePath);
                }
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var settings =
                CreateFunctionalDataReaderSettings(resourcesDirectory);

            var readerBuilder = XmiReaderBuilder.Create()
                .UsingSettings(settings)
                .WithLogger(NullLoggerFactory.Instance)
                .WithExtender<EnterpriseArchitectExtenderReader>()
                .WithExtensionContentReaderFacade<ExtensionContentReaderFacade>();

            using var reader = readerBuilder.Build();

            var result = reader.Read(
                Path.Combine(resourcesDirectory.FullName, "FunctionalData.xmi"));

            result.ValidateFunctionalData();

            return result;
        }

        /// <summary>
        /// Selects exactly one package named <c>FunctionalData</c>.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The loaded UML model.
        /// </param>
        /// <returns>
        /// The unique FunctionalData package.
        /// </returns>
        public static IPackage QueryFunctionalDataPackage(
            this XmiReaderResult xmiReaderResult)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);

            return QueryAllPackages(xmiReaderResult)
                .Single(
                    package => string.Equals(
                        package.Name,
                        FunctionalDataPackageName,
                        StringComparison.Ordinal));
        }

        /// <summary>
        /// Validates the complete reviewed FunctionalData semantic contract.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The loaded UML model.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a required package is missing or non-unique, or when a primitive type, class,
        /// generalization, property, enumeration, association, multiplicity, XMI identifier, or reviewed
        /// inventory is invalid.
        /// </exception>
        public static void ValidateFunctionalData(
            this XmiReaderResult xmiReaderResult)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);

            var allPackages = QueryAllPackages(xmiReaderResult);
            var functionalData = xmiReaderResult.QueryFunctionalDataPackage();

            var standardPrimitiveTypes = ValidatePrimitivePackage(
                allPackages,
                "PrimitiveTypes",
                ExpectedStandardPrimitiveNames);

            var customPrimitiveTypes = ValidatePrimitivePackage(
                allPackages,
                "Primitives",
                ExpectedCustomPrimitiveNames);

            var generationPackages = functionalData.QueryPackages();

            var classes = generationPackages
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .ToArray();

            var enumerations = generationPackages
                .SelectMany(package => package.PackagedElement.OfType<IEnumeration>())
                .ToArray();

            var associations = generationPackages
                .SelectMany(package => package.PackagedElement.OfType<IAssociation>())
                .ToArray();

            var validClasses =
                new HashSet<IClass>(
                    classes,
                    ReferenceEqualityComparer.Instance);

            var validTypes =
                new HashSet<IType>(
                    classes
                        .Cast<IType>()
                        .Concat(enumerations.Cast<IType>())
                        .Concat(standardPrimitiveTypes.Cast<IType>())
                        .Concat(customPrimitiveTypes.Cast<IType>()),
                    ReferenceEqualityComparer.Instance);

            foreach (var umlClass in classes)
            {
                ValidateClass(umlClass, validClasses, validTypes);
            }

            foreach (var enumeration in enumerations)
            {
                ValidateEnumeration(enumeration);
            }

            var actualAssociationSignatures = associations
                .Select(
                    association =>
                        ValidateAssociation(
                            association,
                            validClasses,
                            validTypes))
                .ToArray();

            ThrowIfUnexpectedNames(
                "class",
                classes.Select(umlClass => umlClass.Name),
                ExpectedClassNames);

            ThrowIfUnexpectedNames(
                "abstract class",
                classes
                    .Where(umlClass => umlClass.IsAbstract)
                    .Select(umlClass => umlClass.Name),
                ExpectedAbstractClassNames);

            ThrowIfUnexpectedNames(
                "concrete class",
                classes
                    .Where(umlClass => !umlClass.IsAbstract)
                    .Select(umlClass => umlClass.Name),
                ExpectedConcreteClassNames);

            ThrowIfUnexpectedNames(
                "enumeration",
                enumerations.Select(enumeration => enumeration.Name),
                ExpectedEnumerationNames);

            ThrowIfUnexpectedAssociationSignatures(
                actualAssociationSignatures);
        }

        private static IPackage[] QueryAllPackages(
            XmiReaderResult xmiReaderResult)
        {
            return xmiReaderResult.Packages
                .SelectMany(package => package.QueryPackages())
                .Distinct(ReferenceEqualityComparer.Instance)
                .ToArray();
        }

        private static IPrimitiveType[] ValidatePrimitivePackage(
            IEnumerable<IPackage> allPackages,
            string packageName,
            IReadOnlyCollection<string> expectedNames)
        {
            var package = allPackages.Single(
                candidate => string.Equals(
                    candidate.Name,
                    packageName,
                    StringComparison.Ordinal));

            var primitiveTypes = package.PackagedElement
                .OfType<IPrimitiveType>()
                .ToArray();

            ThrowIfUnexpectedNames(
                $"{packageName} primitive",
                primitiveTypes.Select(primitiveType => primitiveType.Name),
                expectedNames);

            foreach (var primitiveType in primitiveTypes)
            {
                ValidateRequiredXmiIdentifier(
                    primitiveType,
                    $"Primitive type '{primitiveType.Name}'");

                var actualMapping = primitiveType.QueryCSharpTypeName();

                if (!ExpectedPrimitiveMappings.TryGetValue(
                        primitiveType.Name,
                        out var expectedMapping)
                    || !string.Equals(
                        actualMapping,
                        expectedMapping,
                        StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Primitive type '{primitiveType.Name}' maps to "
                        + $"'{actualMapping}' instead of '{expectedMapping}'.");
                }
            }

            return primitiveTypes;
        }

        /// <exception cref="ArgumentException">
        /// Thrown when the class name, its generated interface identifier, or a property name cannot be
        /// represented as a legal C# identifier.
        /// </exception>
        private static void ValidateClass(
            IClass umlClass,
            ISet<IClass> validClasses,
            ISet<IType> validTypes)
        {
            ValidateRequiredXmiIdentifier(
                umlClass,
                $"Class '{umlClass.Name}'");

            if (string.IsNullOrWhiteSpace(umlClass.Name))
            {
                throw new InvalidOperationException(
                    $"Class '{umlClass.XmiId}' has no name.");
            }

            _ = ReservedCSharpNameMapper.Map(umlClass.Name);
            _ = ReservedCSharpNameMapper.Map($"I{umlClass.Name}");

            foreach (var generalization in umlClass.Generalization)
            {
                if (generalization.General is not IClass generalClass
                    || !validClasses.Contains(generalClass))
                {
                    throw new InvalidOperationException(
                        $"Class '{umlClass.Name}' has an unresolved or invalid generalization.");
                }
            }

            _ = umlClass.QueryGeneralizations();

            foreach (var property in umlClass.OwnedAttribute)
            {
                ValidateProperty(property, validTypes);
            }

            var effectiveProperties =
                umlClass.QueryPocoImplementationProperties();

            var duplicateGeneratedPropertyName = effectiveProperties
                .Select(property => property.QueryPropertyName())
                .GroupBy(name => name, StringComparer.Ordinal)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateGeneratedPropertyName is not null)
            {
                throw new InvalidOperationException(
                    $"Class '{umlClass.Name}' exposes duplicate generated property "
                    + $"identifier '{duplicateGeneratedPropertyName}'.");
            }
        }

        private static void ValidateProperty(
            IProperty property,
            ISet<IType> validTypes)
        {
            ValidateRequiredXmiIdentifier(
                property,
                $"Property '{property.Name}'");

            if (string.IsNullOrWhiteSpace(property.Name))
            {
                throw new InvalidOperationException(
                    $"Property '{property.XmiId}' has no name.");
            }

            ValidatePropertyType(property, validTypes);
            ValidateMultiplicity(
                property,
                $"Property '{property.Describe()}'");

            _ = property.QueryPropertyName();
            _ = property.QueryDtoTypeName();
            _ = property.QueryPocoTypeName();
        }

        private static void ValidatePropertyType(
            IProperty property,
            ISet<IType> validTypes)
        {
            if (property.Type is null)
            {
                throw new InvalidOperationException(
                    $"Property '{property.Describe()}' has no resolved type.");
            }

            if (!validTypes.Contains(property.Type))
            {
                throw new InvalidOperationException(
                    $"Property '{property.Describe()}' references a type outside "
                    + "the validated FunctionalData model.");
            }

            if (property.Type is not IClass
                && property.Type is not IEnumeration
                && property.Type is not IPrimitiveType)
            {
                throw new InvalidOperationException(
                    $"Property '{property.Describe()}' has unsupported UML type "
                    + $"'{property.Type.Name}'.");
            }

            if (string.IsNullOrWhiteSpace(property.Type.Name))
            {
                throw new InvalidOperationException(
                    $"Property '{property.Describe()}' has an unnamed type.");
            }
        }

        private static void ValidateEnumeration(
            IEnumeration enumeration)
        {
            ValidateRequiredXmiIdentifier(
                enumeration,
                $"Enumeration '{enumeration.Name}'");

            if (string.IsNullOrWhiteSpace(enumeration.Name))
            {
                throw new InvalidOperationException(
                    $"Enumeration '{enumeration.XmiId}' has no name.");
            }

            _ = ReservedCSharpNameMapper.Map(enumeration.Name);

            var actualLiteralNames = new List<string>();
            var mappedIdentifiers = new HashSet<string>(StringComparer.Ordinal);

            foreach (var literal in enumeration.OwnedLiteral)
            {
                ValidateRequiredXmiIdentifier(
                    literal,
                    $"Enumeration literal '{literal.Name}'");

                if (string.IsNullOrWhiteSpace(literal.Name))
                {
                    throw new InvalidOperationException(
                        $"Enumeration '{enumeration.Name}' contains an unnamed literal.");
                }

                var mappedIdentifier =
                    ReservedCSharpNameMapper.Map(literal.Name);

                if (!mappedIdentifiers.Add(mappedIdentifier))
                {
                    throw new InvalidOperationException(
                        $"Enumeration '{enumeration.Name}' contains duplicate C# literal "
                        + $"identifier '{mappedIdentifier}'.");
                }

                actualLiteralNames.Add(literal.Name);
            }

            if (!ExpectedLiteralNames.TryGetValue(
                    enumeration.Name,
                    out var expectedNames))
            {
                throw new InvalidOperationException(
                    $"Enumeration '{enumeration.Name}' is not present in the reviewed inventory.");
            }

            if (!actualLiteralNames.SequenceEqual(
                    expectedNames,
                    StringComparer.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Enumeration '{enumeration.Name}' does not match its reviewed "
                    + "literal spelling, casing, and order.");
            }
        }

        private static string ValidateAssociation(
            IAssociation association,
            ISet<IClass> validClasses,
            ISet<IType> validTypes)
        {
            ValidateRequiredXmiIdentifier(
                association,
                $"Association '{association.Name}'");

            if (association.MemberEnd.Count != 2)
            {
                throw new InvalidOperationException(
                    $"Association '{association.XmiId}' must have exactly two member ends.");
            }

            foreach (var associationEnd in association.MemberEnd)
            {
                if (associationEnd is null)
                {
                    throw new InvalidOperationException(
                        $"Association '{association.XmiId}' contains an unresolved member end.");
                }

                ValidateRequiredXmiIdentifier(
                    associationEnd,
                    $"Association end '{associationEnd.Name}'");

                ValidatePropertyType(associationEnd, validTypes);
                ValidateMultiplicity(
                    associationEnd,
                    $"Association end '{DescribeAssociationEnd(associationEnd)}'");

                if (string.IsNullOrWhiteSpace(associationEnd.Name))
                {
                    continue;
                }

                if (associationEnd.Owner is not IClass owningClass
                    || !validClasses.Contains(owningClass))
                {
                    throw new InvalidOperationException(
                        $"Named association end '{DescribeAssociationEnd(associationEnd)}' "
                        + "is not owned by a validated FunctionalData class.");
                }

                var isQueryable = owningClass
                    .QueryAllProperties()
                    .Any(
                        property =>
                            ReferenceEquals(property, associationEnd)
                            || string.Equals(
                                property.XmiId,
                                associationEnd.XmiId,
                                StringComparison.Ordinal));

                if (!isQueryable)
                {
                    throw new InvalidOperationException(
                        $"Named association end '{DescribeAssociationEnd(associationEnd)}' "
                        + "is not queryable through its owning class.");
                }
            }

            var firstEnd = association.MemberEnd[0];
            var secondEnd = association.MemberEnd[1];

            return CreateAssociationSignature(
                firstEnd.Type.Name,
                firstEnd.Name,
                secondEnd.Type.Name,
                secondEnd.Name);
        }

        private static void ValidateMultiplicity(
            IMultiplicityElement multiplicity,
            string description)
        {
            if (multiplicity.Lower < 0)
            {
                throw new InvalidOperationException(
                    $"{description} has invalid lower multiplicity "
                    + $"'{multiplicity.Lower}'.");
            }

            if (string.Equals(
                    multiplicity.Upper,
                    "*",
                    StringComparison.Ordinal))
            {
                return;
            }

            if (!int.TryParse(
                    multiplicity.Upper,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var upper))
            {
                throw new InvalidOperationException(
                    $"{description} has invalid upper multiplicity "
                    + $"'{multiplicity.Upper}'.");
            }

            if (upper < multiplicity.Lower)
            {
                throw new InvalidOperationException(
                    $"{description} has upper multiplicity '{upper}' below lower "
                    + $"multiplicity '{multiplicity.Lower}'.");
            }
        }

        private static void ValidateRequiredXmiIdentifier(
            IXmiElement element,
            string description)
        {
            if (string.IsNullOrWhiteSpace(element.XmiId))
            {
                throw new InvalidOperationException(
                    $"{description} has no XMI identifier.");
            }
        }

        private static void ThrowIfUnexpectedNames(
            string inventoryName,
            IEnumerable<string> actualNames,
            IReadOnlyCollection<string> expectedNames)
        {
            var actual = actualNames.ToArray();

            if (actual.Any(string.IsNullOrWhiteSpace))
            {
                throw new InvalidOperationException(
                    $"The {inventoryName} inventory contains an unnamed element.");
            }

            var duplicateName = actual
                .GroupBy(name => name, StringComparer.Ordinal)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateName is not null)
            {
                throw new InvalidOperationException(
                    $"The {inventoryName} inventory contains duplicate name "
                    + $"'{duplicateName}'.");
            }

            var orderedActual = actual
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

            var orderedExpected = expectedNames
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

            if (!orderedActual.SequenceEqual(
                    orderedExpected,
                    StringComparer.Ordinal))
            {
                throw new InvalidOperationException(
                    $"The {inventoryName} inventory does not match the reviewed contract."
                    + $"{Environment.NewLine}Expected: {string.Join(", ", orderedExpected)}"
                    + $"{Environment.NewLine}Actual: {string.Join(", ", orderedActual)}");
            }
        }

        private static void ThrowIfUnexpectedAssociationSignatures(
            IReadOnlyCollection<string> actualSignatures)
        {
            var duplicateSignature = actualSignatures
                .GroupBy(signature => signature, StringComparer.Ordinal)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateSignature is not null)
            {
                throw new InvalidOperationException(
                    $"The FunctionalData model contains duplicate association "
                    + $"signature '{duplicateSignature}'.");
            }

            var orderedActual = actualSignatures
                .OrderBy(signature => signature, StringComparer.Ordinal)
                .ToArray();

            var orderedExpected = ExpectedAssociationSignatures
                .OrderBy(signature => signature, StringComparer.Ordinal)
                .ToArray();

            if (!orderedActual.SequenceEqual(
                    orderedExpected,
                    StringComparer.Ordinal))
            {
                throw new InvalidOperationException(
                    "The association-signature inventory does not match the reviewed contract."
                    + $"{Environment.NewLine}Expected: {string.Join(", ", orderedExpected)}"
                    + $"{Environment.NewLine}Actual: {string.Join(", ", orderedActual)}");
            }
        }

        private static string CreateAssociationSignature(
            string firstType,
            string firstRole,
            string secondType,
            string secondRole)
        {
            var firstEnd = $"{firstType}:{firstRole ?? string.Empty}";
            var secondEnd = $"{secondType}:{secondRole ?? string.Empty}";

            return string.CompareOrdinal(firstEnd, secondEnd) <= 0
                ? $"{firstEnd}|{secondEnd}"
                : $"{secondEnd}|{firstEnd}";
        }

        private static string DescribeAssociationEnd(
            IProperty associationEnd)
        {
            var typeName =
                associationEnd.Type?.Name ?? "<unresolved>";

            var roleName =
                associationEnd.Name ?? "<unnamed>";

            return $"{typeName}:{roleName}";
        }
    }
}