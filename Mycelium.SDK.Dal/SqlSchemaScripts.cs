// ------------------------------------------------------------------------------------------------
//  <copyright file="SqlSchemaScripts.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Dal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Provides the PostgreSQL schema scripts embedded in the Mycelium.SDK.Dal assembly.
    /// </summary>
    public static class SqlSchemaScripts
    {
        /// <summary>
        /// The manifest resource name prefix shared by every embedded schema script.
        /// </summary>
        private const string ResourcePrefix = "Mycelium.SDK.Dal.AutoGenSql.";

        /// <summary>
        /// Reads every embedded schema script, ordered by name so they can be applied in sequence.
        /// </summary>
        /// <returns>
        /// The scripts as (file name, SQL text) pairs, such as <c>0001_initial.sql</c>, ordered ordinally by file name.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an embedded script cannot be opened.
        /// </exception>
        /// <exception cref="DecoderFallbackException">
        /// Thrown when an embedded script is not valid UTF-8.
        /// </exception>
        public static IReadOnlyList<(string Name, string Sql)> GetAll()
        {
            var assembly = typeof(SqlSchemaScripts).Assembly;

            return assembly.GetManifestResourceNames()
                .Where(resourceName => resourceName.StartsWith(ResourcePrefix, StringComparison.Ordinal))
                .OrderBy(resourceName => resourceName, StringComparer.Ordinal)
                .Select(resourceName => (resourceName[ResourcePrefix.Length..], ReadResource(assembly, resourceName)))
                .ToList();
        }

        /// <summary>
        /// Reads the text of a manifest resource as strict UTF-8 with LF line endings.
        /// </summary>
        /// <param name="assembly">The assembly that embeds the resource.</param>
        /// <param name="resourceName">The manifest resource name.</param>
        /// <returns>
        /// The resource text with LF line endings. A byte-order mark is not removed, so that it remains detectable.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the resource cannot be opened.
        /// </exception>
        private static string ReadResource(Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"The embedded schema script '{resourceName}' could not be opened from '{assembly.GetName().Name}'.");

            using var reader = new StreamReader(stream, new UTF8Encoding(false, true), false);

            return reader.ReadToEnd().Replace("\r\n", "\n");
        }
    }
}
