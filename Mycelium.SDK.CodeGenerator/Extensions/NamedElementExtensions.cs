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
    using uml4net.CommonStructure;

    /// <summary>
    /// Provides Mycelium-specific queries for UML named elements.
    /// </summary>
    public static class NamedElementExtensions
    {
        extension(INamedElement namedElement)
        {
            /// <summary>
            /// Returns a readable description of a UML named element.
            /// </summary>
            /// <returns>
            /// The element name when available; otherwise, its XMI identifier.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the named element is <see langword="null"/>.
            /// </exception>
            public string Describe()
            {
                ArgumentNullException.ThrowIfNull(namedElement);
                return string.IsNullOrWhiteSpace(namedElement.Name) ? namedElement.XmiId : namedElement.Name;
            }
        }
    }
}
