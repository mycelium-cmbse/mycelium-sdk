// ------------------------------------------------------------------------------------------------
//  <copyright file="Reply.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.DTO
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a specific reply for a specific comment.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class Reply : IReply
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The textual content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="IAuditableThing" />.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// References a quoted <see cref="IMessage" />
        /// </summary>
        public Guid? Quotes { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="IAuditableThing" />.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
