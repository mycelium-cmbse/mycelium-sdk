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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Packages;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Provides the canonical FunctionalData model and shared generator-test directory operations.
    /// </summary>
    [SetUpFixture]
    public sealed class GeneratorSetupFixture
    {
        /// <summary>
        /// Gets the directory containing the copied FunctionalData XMI resources.
        /// </summary>
        public static DirectoryInfo ResourcesDirectory =>
            new(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources"));

        /// <summary>
        /// Loads a fresh, validated FunctionalData model through the production loading path.
        /// </summary>
        /// <returns>
        /// A fresh validated XMI reader result.
        /// </returns>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown when the copied resource directory does not exist.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown when a required XMI resource is missing.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the loaded model does not satisfy the reviewed semantic contract.
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

        /// <summary>
        /// Creates a clean test-output directory descriptor without creating the directory.
        /// </summary>
        /// <param name="directoryName">
        /// The directory name below the isolated UML test-output directory.
        /// </param>
        /// <returns>
        /// The clean output-directory descriptor.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="directoryName" /> is empty.
        /// </exception>
        public static DirectoryInfo QueryFreshOutputDirectory(string directoryName)
        {
            ArgumentException.ThrowIfNullOrEmpty(directoryName);

            var directory = new DirectoryInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", directoryName));

            if (directory.Exists)
            {
                directory.Delete(recursive: true);
            }

            return directory;
        }

        /// <summary>
        /// Reads the complete byte-for-byte contents of an existing directory.
        /// </summary>
        /// <param name="directory">
        /// The directory to snapshot.
        /// </param>
        /// <returns>
        /// The relative filenames and bytes in ordinal filename order.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="directory" /> is <see langword="null" />.
        /// </exception>
        public static async Task<IReadOnlyDictionary<string, byte[]>> QueryDirectorySnapshotAsync(
            DirectoryInfo directory)
        {
            ArgumentNullException.ThrowIfNull(directory);

            var snapshot = new SortedDictionary<string, byte[]>(StringComparer.Ordinal);

            foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(directory.FullName, file.FullName);

                snapshot.Add(relativePath, await File.ReadAllBytesAsync(file.FullName));
            }

            return snapshot;
        }

        /// <summary>
        /// Verifies that a directory still matches a previously captured byte-for-byte snapshot.
        /// </summary>
        /// <param name="directory">
        /// The existing directory to verify.
        /// </param>
        /// <param name="expectedSnapshot">
        /// The expected complete directory snapshot.
        /// </param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an argument is <see langword="null" />.
        /// </exception>
        public static async Task AssertDirectoryMatchesSnapshotAsync(
            DirectoryInfo directory,
            IReadOnlyDictionary<string, byte[]> expectedSnapshot)
        {
            ArgumentNullException.ThrowIfNull(directory);
            ArgumentNullException.ThrowIfNull(expectedSnapshot);

            var actualSnapshot = await QueryDirectorySnapshotAsync(directory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    actualSnapshot.Keys,
                    Is.EqualTo(expectedSnapshot.Keys),
                    "Preflight failure changed the destination manifest.");

                foreach (var expectedFile in expectedSnapshot)
                {
                    var exists = actualSnapshot.TryGetValue(expectedFile.Key, out var actualBytes);

                    Assert.That(
                        exists,
                        Is.True,
                        $"Existing destination file '{expectedFile.Key}' was removed.");

                    if (exists)
                    {
                        Assert.That(
                            actualBytes,
                            Is.EqualTo(expectedFile.Value),
                            $"Existing destination file '{expectedFile.Key}' was modified.");
                    }
                }
            }
        }
    }
}