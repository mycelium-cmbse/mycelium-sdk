// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlPocoGenerator.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Generates the FunctionalData POCO interfaces and concrete implementations.
    /// </summary>
    public sealed class UmlPocoGenerator : UmlClassHandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for concrete POCO classes.
        /// </summary>
        private const string ClassTemplateName = "poco-class-uml-template";
        
        /// <summary>
        /// The registered Handlebars template name used for POCO interfaces.
        /// </summary>
        private const string InterfaceTemplateName = "poco-interface-uml-template";

        /// <inheritdoc />
        protected override string ArtifactName => "POCO";

        /// <inheritdoc />
        protected override string ClassTemplate => ClassTemplateName;

        /// <inheritdoc />
        protected override string InterfaceTemplate => InterfaceTemplateName;

        /// <summary>
        /// Generates one POCO interface.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated POCO interface is written.
        /// </param>
        /// <param name="umlClass">
        /// The UML class for which the POCO interface is generated.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous POCO interface generation operation. The task result
        /// contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory"/> or <paramref name="umlClass"/> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class lacks information required to generate its POCO interface or contains
        /// an unsupported property type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier.
        /// </exception>
        public Task<string> GeneratePocoInterfaceAsync(DirectoryInfo outputDirectory, IClass umlClass)
        {
            return this.GenerateInterfaceAsync(outputDirectory, umlClass);
        }

        /// <summary>
        /// Generates one concrete POCO implementation.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated POCO implementation is written.
        /// </param>
        /// <param name="umlClass">
        /// The UML class for which the POCO implementation is generated.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous POCO implementation generation operation. The task result
        /// contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory"/> or <paramref name="umlClass"/> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class is abstract, lacks information required to generate its concrete POCO,
        /// or contains an unsupported property type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier.
        /// </exception>
        public Task<string> GeneratePocoClassAsync(DirectoryInfo outputDirectory, IClass umlClass)
        {
            return this.GenerateClassAsync(outputDirectory, umlClass);
        }

        /// <inheritdoc />
        protected override void RegisterArtifactHelpers()
        {
            ClassHelper.RegisterPocoClassHelper(this.Handlebars);
            PropertyHelper.RegisterPocoPropertyHelper(this.Handlebars);
        }
    }
}