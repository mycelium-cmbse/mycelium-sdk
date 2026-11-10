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
        /// <param name="namedElement">
        /// The UML named element that is described.
        /// </param>
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

            /// <summary>
            /// Queries the C# namespace that a named element is declared in, relative to its model.
            /// </summary>
            /// <returns>
            /// The packages between the model and the element, dot-separated - for example <c>Systems.Parts</c> for
            /// <c>SysML::Systems::Parts::PartUsage</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the named element is <see langword="null"/>.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the element is not nested in a package of a model.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when a package name cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryNamespace()
            {
                ArgumentNullException.ThrowIfNull(namedElement);

                var segments = namedElement.QualifiedName?.Split("::") ?? [];

                return segments.Length < 3
                    ? throw new InvalidOperationException($"'{namedElement.Describe()}' has the qualified name '{namedElement.QualifiedName}', which names no package within a model.")
                    : string.Join('.', segments[1..^1].Select(ReservedCSharpNameMapper.Map));
            }
        }
    }
}
