// ------------------------------------------------------------------------------------------------
//  <copyright file="SafeContextHelper.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.HandleBarHelpers
{
    using HandlebarsDotNet;

    using uml4net.Classification;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides a bounded Handlebars context for property partial templates.
    /// </summary>
    public static class SafeContextHelper
    {
        /// <summary>
        /// Registers the helper that exposes a UML property and its owning class
        /// to a block template.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the helper is registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterSafeContextHelper(
            this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "withPropertyClassContext",
                (output, options, _, arguments) =>
                {
                    if (arguments.Length != 2)
                    {
                        throw new HandlebarsException(
                            "{{#withPropertyClassContext}} requires exactly two arguments.");
                    }

                    if (arguments[0] is not IProperty property)
                    {
                        throw new HandlebarsException(
                            "{{#withPropertyClassContext}} requires an IProperty as its first argument.");
                    }

                    if (arguments[1] is not IClass classContext)
                    {
                        throw new HandlebarsException(
                            "{{#withPropertyClassContext}} requires an IClass as its second argument.");
                    }

                    var safeContext = new
                    {
                        property,
                        classContext
                    };

                    options.Template(output, safeContext);
                });
        }
    }
}
