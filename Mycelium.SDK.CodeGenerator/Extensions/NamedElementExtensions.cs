// ------------------------------------------------------------------------------------------------
//  <copyright file="NamedElementExtensions.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using System;

    using uml4net.CommonStructure;

    /// <summary>
    /// Provides Mycelium-specific queries for UML named elements.
    /// </summary>
    public static class NamedElementExtensions
    {
        /// <summary>
        /// Returns a readable description of a UML named element.
        /// </summary>
        /// <param name="namedElement">
        /// The UML named element to describe.
        /// </param>
        /// <returns>
        /// The element name when available; otherwise, its XMI identifier.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="namedElement"/> is <see langword="null"/>.
        /// </exception>
        public static string Describe(this INamedElement namedElement)
        {
            ArgumentNullException.ThrowIfNull(namedElement);
            return string.IsNullOrWhiteSpace(namedElement.Name) ? namedElement.XmiId : namedElement.Name;
        }
    }
}
