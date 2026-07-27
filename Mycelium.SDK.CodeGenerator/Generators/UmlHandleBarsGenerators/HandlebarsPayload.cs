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

        public IPackage RootPackage { get; }

        public IPackage[] Packages { get; }

        public IEnumeration[] Enumerations { get; }

        public IPrimitiveType[] PrimitiveTypes { get; }

        public IDataType[] DataTypes { get; }

        public IClass[] Classes { get; }

        public IInterface[] Interfaces { get; }
    }
}
