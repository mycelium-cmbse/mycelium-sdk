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
    using System.Text;

    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Packages;
    using uml4net.xmi.Readers;

    [SetUpFixture]
    public sealed class GeneratorSetupFixture
    {
        private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        public static DirectoryInfo ResourcesDirectory =>
            new(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources"));

        public static XmiReaderResult ReadFunctionalData() => XmiReaderResultExtensions.ReadFunctionalData(ResourcesDirectory);

        public static async Task<string> ReadSourceAsync(string path)
        {
            var bytes = await File.ReadAllBytesAsync(path);
            var offset = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;

            return StrictUtf8.GetString(bytes, offset, bytes.Length - offset);
        }

        public static string[] QueryRelativeFileNames(DirectoryInfo directory)
        {
            return directory.GetFiles("*", SearchOption.AllDirectories)
                .Select(file => Path.GetRelativePath(directory.FullName, file.FullName))
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }

        public static IPackage QueryFunctionalDataPackage(XmiReaderResult xmiReaderResult)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);

            return xmiReaderResult.QueryFunctionalDataPackage();
        }
    }
}
