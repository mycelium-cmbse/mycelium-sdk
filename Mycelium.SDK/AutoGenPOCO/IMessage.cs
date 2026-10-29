// ------------------------------------------------------------------------------------------------
//  <copyright file="IMessage.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.POCO
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a user message.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IMessage : IAuditableThing
    {
        /// <summary>
        /// The textual content of the message.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// References a quoted <see cref="IMessage" />
        /// </summary>
        IMessage Quotes { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
