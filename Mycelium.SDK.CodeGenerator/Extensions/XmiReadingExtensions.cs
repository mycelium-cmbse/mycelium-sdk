// ------------------------------------------------------------------------------------------------
//  <copyright file="XmiReadingExtensions.cs" company="Starion Group S.A.">
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
    using uml4net.xmi.Resources;
    using uml4net.xmi.Settings;

    /// <summary>
    /// Provides the canonical offline path by which any model under <c>Resources/</c> is read and selected.
    /// </summary>
    public static class XmiReadingExtensions
    {
        /// <summary>
        /// The file to which uml4net's embedded UML primitive types are written once, so that the Eclipse path map can point at them.
        /// </summary>
        private static readonly Lazy<string> EmbeddedPrimitiveTypesPath = new(WriteEmbeddedPrimitiveTypes);

        /// <summary>
        /// Registers the shared C# mappings for the model's UUID and URI value types.
        /// </summary>
        static XmiReadingExtensions()
        {
            TypeExtensions.AddOrOverwriteCSharpTypeMappings(("UUID", "Guid"), ("URI", "Uri"));
        }

        /// <param name="resourcesDirectory">
        /// The directory that holds the XMI resources.
        /// </param>
        extension(DirectoryInfo resourcesDirectory)
        {
            /// <summary>
            /// Creates the canonical offline XMI reader settings.
            /// </summary>
            /// <param name="useStrictReading">
            /// <see langword="true" /> to throw when an unknown XMI element or attribute is encountered;
            /// <see langword="false" /> to ignore it and log a warning.
            /// </param>
            /// <returns>
            /// Reader settings that resolve XMI dependencies from local files or embedded resources, mapping the Eclipse UML primitive library onto
            /// uml4net's embedded <c>PrimitiveTypes.xmi</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the resources directory is <see langword="null" />.
            /// </exception>
            public DefaultSettings CreateReaderSettings(bool useStrictReading)
            {
                ArgumentNullException.ThrowIfNull(resourcesDirectory);

                return new DefaultSettings
                {
                    LocalReferenceBasePath = resourcesDirectory.FullName,
                    UseStrictReading = useStrictReading,
                    PathMaps = new Dictionary<string, string>
                    {
                        [XmiResources.UmlPrimitiveTypesPathMap] = EmbeddedPrimitiveTypesPath.Value
                    }
                };
            }

            /// <summary>
            /// Reads a model through the canonical offline production path.
            /// </summary>
            /// <param name="fileName">
            /// The file name of the export to read, relative to the resources directory.
            /// </param>
            /// <param name="useStrictReading">
            /// <see langword="true" /> to throw when an unknown XMI element or attribute is encountered;
            /// <see langword="false" /> to ignore it and log a warning.
            /// </param>
            /// <returns>
            /// The loaded XMI reader result.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the resources directory is <see langword="null" />.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when <paramref name="fileName" /> is empty.
            /// </exception>
            /// <exception cref="DirectoryNotFoundException">
            /// Thrown when the resources directory does not exist.
            /// </exception>
            /// <exception cref="FileNotFoundException">
            /// Thrown when the requested export does not exist.
            /// </exception>
            public XmiReaderResult ReadModel(string fileName, bool useStrictReading)
            {
                ArgumentNullException.ThrowIfNull(resourcesDirectory);
                ArgumentException.ThrowIfNullOrEmpty(fileName);

                if (!resourcesDirectory.Exists)
                {
                    throw new DirectoryNotFoundException($"The resources directory '{resourcesDirectory.FullName}' does not exist.");
                }

                var resourcePath = Path.Combine(resourcesDirectory.FullName, fileName);

                if (!File.Exists(resourcePath))
                {
                    throw new FileNotFoundException($"Required resource '{fileName}' was not found.", resourcePath);
                }

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var readerBuilder = XmiReaderBuilder.Create()
                    .UsingSettings(resourcesDirectory.CreateReaderSettings(useStrictReading))
                    .WithLogger(NullLoggerFactory.Instance)
                    .WithExtender<EnterpriseArchitectExtenderReader>()
                    .WithExtensionContentReaderFacade<ExtensionContentReaderFacade>();

                using var reader = readerBuilder.Build();

                return reader.Read(resourcePath);
            }
        }

        /// <param name="xmiReaderResult">
        /// The result of reading an XMI model.
        /// </param>
        extension(XmiReaderResult xmiReaderResult)
        {
            /// <summary>
            /// Queries the single package of a loaded model that carries the supplied name.
            /// </summary>
            /// <param name="packageName">
            /// The exact name of the package to select.
            /// </param>
            /// <returns>
            /// The unique package carrying that name.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the reader result is <see langword="null" />.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when <paramref name="packageName" /> is empty.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the model does not contain exactly one package carrying that name.
            /// </exception>
            public IPackage QueryPackage(string packageName)
            {
                ArgumentNullException.ThrowIfNull(xmiReaderResult);
                ArgumentException.ThrowIfNullOrEmpty(packageName);

                return xmiReaderResult.Packages.SelectMany(package => package.QueryPackages())
                    .Single(package => string.Equals(package.Name, packageName, StringComparison.Ordinal));
            }
        }

        /// <summary>
        /// Writes uml4net's embedded UML primitive types to a file of this process, since uml4net resolves a path map only to a file.
        /// </summary>
        /// <returns>
        /// The path of the written file.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when uml4net does not embed the UML primitive types.
        /// </exception>
        private static string WriteEmbeddedPrimitiveTypes()
        {
            if (!new ResourceLoader().TryLoadKnownResource(XmiResources.UmlPrimitiveTypesUri, out var primitiveTypes) || primitiveTypes is null)
            {
                throw new InvalidOperationException($"uml4net embeds no '{XmiResources.UmlPrimitiveTypesUri}'.");
            }

            var path = Path.Combine(Path.GetTempPath(), $"uml4net-PrimitiveTypes-{Environment.ProcessId}.xmi");

            using (primitiveTypes)
            using (var file = File.Create(path))
            {
                primitiveTypes.CopyTo(file);
            }

            return path;
        }
    }
}
