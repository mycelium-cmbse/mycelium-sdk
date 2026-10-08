// ------------------------------------------------------------------------------------------------
//  <copyright file="GeneratedOutput.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests
{
    /// <summary>
    /// Provides the queries that every generator fixture runs over a directory of generated files.
    /// </summary>
    internal static class GeneratedOutput
    {
        /// <summary>
        /// Queries the manifest of a directory of generated files.
        /// </summary>
        /// <param name="directory">
        /// The staging, golden or committed directory whose manifest is queried.
        /// </param>
        /// <returns>
        /// The paths of the files it holds, relative to it and ordered.
        /// </returns>
        /// <remarks>
        /// The order is imposed here so that two manifests can be compared as ordered lists, which is
        /// what makes a generated file appearing or disappearing a failure rather than a silent change.
        /// </remarks>
        internal static string[] QueryRelativeFileNames(DirectoryInfo directory)
        {
            return directory.GetFiles("*", SearchOption.AllDirectories)
                .Select(file => Path.GetRelativePath(directory.FullName, file.FullName))
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
