// ------------------------------------------------------------------------------------------------
//  <copyright file="HandleBarsGenerator.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators
{
    using System.Text;

    using HandlebarsDotNet;

    /// <summary>
    /// Base class for Handlebars-backed generators.
    /// </summary>
    public abstract class HandleBarsGenerator : Generator
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HandleBarsGenerator" /> class.
        /// </summary>
        /// <param name="templateSubfolder">
        /// The optional template subdirectory.
        /// </param>
        protected HandleBarsGenerator(string templateSubfolder = null) : base(templateSubfolder)
        {
            this.Templates = new Dictionary<string, HandlebarsTemplate<object, object>>(StringComparer.Ordinal);
            this.Handlebars = HandlebarsDotNet.Handlebars.CreateSharedEnvironment();

            this.Register();
        }

        /// <summary>
        /// Gets the Handlebars environment.
        /// </summary>
        protected IHandlebars Handlebars { get; }

        /// <summary>
        /// Gets the registered compiled templates.
        /// </summary>
        public Dictionary<string, HandlebarsTemplate<object, object>> Templates { get; }

        /// <summary>
        /// Registers generator-specific helpers.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected abstract void RegisterHelpers();

        /// <summary>
        /// Registers generator-specific templates.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected abstract void RegisterTemplates();

        /// <summary>
        /// Loads, compiles, and registers a template using its filename without
        /// the <c>.hbs</c> extension.
        /// </summary>
        /// <param name="name">
        /// The case-sensitive template name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="name" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="name" /> is empty or a template with the same name is already registered.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown when the template file cannot be read.
        /// </exception>
        /// <exception cref="HandlebarsException">
        /// Thrown when the template cannot be compiled.
        /// </exception>
        protected void RegisterTemplate(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);

            var templatePath = Path.Combine(this.TemplateFolderPath, $"{name}.hbs");

            var template = File.ReadAllText(templatePath, Encoding.UTF8);

            this.Templates.Add(name, this.Handlebars.Compile(template));
        }

        /// <summary>
        /// Registers the generator-specific helpers and templates.
        /// </summary>
        private void Register()
        {
            this.RegisterHelpers();
            this.RegisterTemplates();
        }
    }
}
