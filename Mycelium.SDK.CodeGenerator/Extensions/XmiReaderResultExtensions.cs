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
    using System.Text;

    using Microsoft.Extensions.Logging.Abstractions;

    using uml4net.Extensions;
    using uml4net.Packages;
    using uml4net.xmi;
    using uml4net.xmi.Extensions.EnterpriseArchitect.Extender;
    using uml4net.xmi.Extensions.EnterpriseArchitect.Structure.Readers;
    using uml4net.xmi.Readers;
    using uml4net.xmi.Settings;

    /// <summary>
    /// Provides the canonical offline FunctionalData loading and package-selection path.
    /// </summary>
    public static class XmiReaderResultExtensions
    {
        /// <summary>
        /// The exact name of the UML package containing the FunctionalData model.
        /// </summary>
        public const string FunctionalDataPackageName = "FunctionalData";

        /// <summary>
        /// The canonical URI used by the FunctionalData model to reference the standard UML primitive types.
        /// </summary>
        private const string PrimitiveTypesUri = "http://www.omg.org/spec/UML/20161101/PrimitiveTypes.xmi";

        /// <summary>
        /// The XMI resource file names required for canonical offline FunctionalData loading.
        /// </summary>
        private static readonly string[] RequiredResourceFileNames =
        [
            "CSharp_Primitives.xmi",
            "FunctionalData.xmi",
            "PrimitiveTypes.xmi"
        ];

        /// <summary>
        /// Creates the canonical offline XMI reader settings for the FunctionalData model.
        /// </summary>
        /// <param name="resourcesDirectory">
        /// The directory containing the FunctionalData XMI resources.
        /// </param>
        /// <returns>
        /// Reader settings that resolve every FunctionalData XMI dependency locally.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="resourcesDirectory" /> is <see langword="null" />.
        /// </exception>
        public static DefaultSettings CreateFunctionalDataReaderSettings(DirectoryInfo resourcesDirectory)
        {
            ArgumentNullException.ThrowIfNull(resourcesDirectory);

            return new DefaultSettings
            {
                LocalReferenceBasePath = resourcesDirectory.FullName,
                PathMaps =
                {
                    [PrimitiveTypesUri] = Path.Combine(resourcesDirectory.FullName, "PrimitiveTypes.xmi")
                },
                UseStrictReading = true
            };
        }

        /// <summary>
        /// Loads the FunctionalData model through the canonical offline production path.
        /// </summary>
        /// <param name="resourcesDirectory">
        /// The directory containing the FunctionalData XMI resources.
        /// </param>
        /// <returns>
        /// The loaded XMI reader result containing exactly one FunctionalData package.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="resourcesDirectory" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown when <paramref name="resourcesDirectory" /> does not exist.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown when a required FunctionalData XMI resource does not exist.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the loaded model does not contain exactly one package named <c>FunctionalData</c>.
        /// </exception>
        public static XmiReaderResult ReadFunctionalData(DirectoryInfo resourcesDirectory)
        {
            ArgumentNullException.ThrowIfNull(resourcesDirectory);

            if (!resourcesDirectory.Exists)
            {
                throw new DirectoryNotFoundException(
                    $"The FunctionalData resources directory '{resourcesDirectory.FullName}' does not exist.");
            }

            foreach (var resourceFileName in RequiredResourceFileNames)
            {
                var resourcePath = Path.Combine(resourcesDirectory.FullName, resourceFileName);

                if (!File.Exists(resourcePath))
                {
                    throw new FileNotFoundException(
                        $"Required FunctionalData resource '{resourceFileName}' was not found.",
                        resourcePath);
                }
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var settings = CreateFunctionalDataReaderSettings(resourcesDirectory);

            var readerBuilder = XmiReaderBuilder.Create()
                .UsingSettings(settings)
                .WithLogger(NullLoggerFactory.Instance)
                .WithExtender<EnterpriseArchitectExtenderReader>()
                .WithExtensionContentReaderFacade<ExtensionContentReaderFacade>();

            using var reader = readerBuilder.Build();

            var result = reader.Read(Path.Combine(resourcesDirectory.FullName, "FunctionalData.xmi"));

            _ = result.QueryFunctionalDataPackage();

            return result;
        }

        /// <summary>
        /// Queries the unique package named <c>FunctionalData</c> from a loaded UML model.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The loaded UML model from which the FunctionalData package is queried.
        /// </param>
        /// <returns>
        /// The unique FunctionalData package.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model does not contain exactly one package named <c>FunctionalData</c>.
        /// </exception>
        public static IPackage QueryFunctionalDataPackage(this XmiReaderResult xmiReaderResult)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);

            return xmiReaderResult.Packages
                .SelectMany(package => package.QueryPackages())
                .Single(package => string.Equals(
                    package.Name,
                    FunctionalDataPackageName,
                    StringComparison.Ordinal));
        }
    }
}
