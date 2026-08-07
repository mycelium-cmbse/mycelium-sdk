// ------------------------------------------------------------------------------------------------
//  <copyright file="HandlebarsPayload.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using uml4net.Packages;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Contains the deterministically selected UML elements used by generators.
    /// </summary>
    public sealed class HandlebarsPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandlebarsPayload"/> class.
        /// </summary>
        /// <param name="rootPackage">
        /// The root UML package selected for generation.
        /// </param>
        /// <param name="packages">
        /// The UML packages available to the generators.
        /// </param>
        /// <param name="enumerations">
        /// The UML enumerations selected for generation.
        /// </param>
        /// <param name="primitiveTypes">
        /// The UML primitive types available for type mapping.
        /// </param>
        /// <param name="dataTypes">
        /// The UML data types selected for generation.
        /// </param>
        /// <param name="classes">
        /// The UML classes selected for generation.
        /// </param>
        /// <param name="interfaces">
        /// The UML interfaces selected for generation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any constructor argument is <see langword="null" />.
        /// </exception>
        public HandlebarsPayload(
            IPackage rootPackage,
            IEnumerable<IPackage> packages,
            IEnumerable<IEnumeration> enumerations,
            IEnumerable<IPrimitiveType> primitiveTypes,
            IEnumerable<IDataType> dataTypes,
            IEnumerable<IClass> classes,
            IEnumerable<IInterface> interfaces)
        {
            ArgumentNullException.ThrowIfNull(rootPackage);
            ArgumentNullException.ThrowIfNull(packages);
            ArgumentNullException.ThrowIfNull(enumerations);
            ArgumentNullException.ThrowIfNull(primitiveTypes);
            ArgumentNullException.ThrowIfNull(dataTypes);
            ArgumentNullException.ThrowIfNull(classes);
            ArgumentNullException.ThrowIfNull(interfaces);

            this.RootPackage = rootPackage;
            this.Packages = packages.ToArray();
            this.Enumerations = enumerations.ToArray();
            this.PrimitiveTypes = primitiveTypes.ToArray();
            this.DataTypes = dataTypes.ToArray();
            this.Classes = classes.ToArray();
            this.Interfaces = interfaces.ToArray();
        }

        /// <summary>
        /// Gets the root UML package selected for generation.
        /// </summary>
        public IPackage RootPackage { get; }

        /// <summary>
        /// Gets the UML packages available to the generators.
        /// </summary>
        public IPackage[] Packages { get; }

        /// <summary>
        /// Gets the UML enumerations selected for generation.
        /// </summary>
        public IEnumeration[] Enumerations { get; }

        /// <summary>
        /// Gets the UML primitive types available for type mapping.
        /// </summary>
        public IPrimitiveType[] PrimitiveTypes { get; }

        /// <summary>
        /// Gets the UML data types selected for generation.
        /// </summary>
        public IDataType[] DataTypes { get; }

        /// <summary>
        /// Gets the UML classes selected for generation.
        /// </summary>
        public IClass[] Classes { get; }

        /// <summary>
        /// Gets the UML interfaces selected for generation.
        /// </summary>
        public IInterface[] Interfaces { get; }
    }
}
