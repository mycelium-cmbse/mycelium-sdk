// ------------------------------------------------------------------------------------------------
//  <copyright file="GeneratorSetupFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Packages;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Provides the canonical FunctionalData model to generator tests.
    /// </summary>
    [SetUpFixture]
    public sealed class GeneratorSetupFixture
    {
        /// <summary>
        /// Gets the directory containing the copied FunctionalData XMI resources.
        /// </summary>
        /// <value>
        /// The directory containing the copied FunctionalData XMI resources.
        /// </value>
        public static DirectoryInfo ResourcesDirectory =>
            new(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources"));

        /// <summary>
        /// Loads a fresh FunctionalData model through the production loading path.
        /// </summary>
        /// <returns>
        /// A fresh XMI reader result.
        /// </returns>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown when the copied resource directory does not exist.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown when a required XMI resource is missing.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when exactly one FunctionalData package cannot be selected.
        /// </exception>
        public static XmiReaderResult ReadFunctionalData()
        {
            return XmiReaderResultExtensions.ReadFunctionalData(ResourcesDirectory);
        }

        /// <summary>
        /// Queries the unique FunctionalData package from a loaded model.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The loaded UML model.
        /// </param>
        /// <returns>
        /// The unique FunctionalData package.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when exactly one FunctionalData package cannot be selected.
        /// </exception>
        public static IPackage QueryFunctionalDataPackage(XmiReaderResult xmiReaderResult)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);

            return xmiReaderResult.QueryFunctionalDataPackage();
        }
    }
}
