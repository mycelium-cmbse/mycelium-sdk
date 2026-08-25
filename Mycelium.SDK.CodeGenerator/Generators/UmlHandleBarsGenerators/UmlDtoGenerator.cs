// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlDtoGenerator.cs" company="Starion Group S.A.">
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
    /// Generates the FunctionalData DTO interfaces and concrete implementations.
    /// </summary>
    public sealed class UmlDtoGenerator : UmlClassHandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for concrete DTO classes.
        /// </summary>
        private const string ClassTemplateName = "dto-class-uml-template";
        
        /// <summary>
        /// The registered Handlebars template name used for DTO interfaces.
        /// </summary>
        private const string InterfaceTemplateName = "dto-interface-uml-template";

        /// <inheritdoc />
        protected override string ArtifactName => "DTO";

        /// <inheritdoc />
        protected override string ClassTemplate => ClassTemplateName;

        /// <inheritdoc />
        protected override string InterfaceTemplate => InterfaceTemplateName;

        /// <summary>
        /// Generates one DTO interface.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated DTO interface is written.
        /// </param>
        /// <param name="umlClass">
        /// The UML class for which the DTO interface is generated.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous DTO interface generation operation. The task result
        /// contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory"/> or <paramref name="umlClass"/> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier or the template
        /// renders empty source.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class lacks information required to generate its DTO interface, contains an
        /// unsupported property type, or the rendered or formatted source contains invalid C# syntax.
        /// </exception>
        public Task<string> GenerateDataTransferObjectInterfaceAsync(DirectoryInfo outputDirectory, IClass umlClass)
        {
            return this.GenerateInterfaceAsync(outputDirectory, umlClass);
        }

        /// <summary>
        /// Generates one concrete DTO implementation.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated DTO implementation is written.
        /// </param>
        /// <param name="umlClass">
        /// The UML class for which the DTO implementation is generated.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous DTO implementation generation operation. The task result
        /// contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory"/> or <paramref name="umlClass"/> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier or the template
        /// renders empty source.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class is abstract, lacks information required to generate its concrete DTO,
        /// contains an unsupported property type, or the rendered or formatted source contains invalid C#
        /// syntax.
        /// </exception>
        public Task<string> GenerateDataTransferObjectClassAsync(DirectoryInfo outputDirectory, IClass umlClass)
        {
            return this.GenerateClassAsync(outputDirectory, umlClass);
        }

        /// <inheritdoc />
        protected override void RegisterArtifactHelpers()
        {
            ClassHelper.RegisterDtoClassHelper(this.Handlebars);
            PropertyHelper.RegisterDtoPropertyHelper(this.Handlebars);
        }
    }
}