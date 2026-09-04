// ------------------------------------------------------------------------------------------------
//  <copyright file="Comment.cs" company="Starion Group S.A.">
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
    /// Represents a user comment attached to a <see cref="Review" />, targeting a specific model element.
    /// Supports threaded replies.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class Comment : IComment
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// References the <see cref="ProjectMember" /> who authored this <see cref="Comment" />.
        /// </summary>
        public IUser Author { get; set; }

        /// <summary>
        /// Defines the current status of the current Comment.
        /// </summary>
        public CommentStatus CommentStatus { get; set; }

        /// <summary>
        /// The textual content of the comment.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="IAuditableThing" />.
        /// </summary>
        public IUser CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// References a quoted <see cref="Comment" />
        /// </summary>
        public IComment Quotes { get; set; }

        /// <summary>
        /// Contains threaded reply <see cref="Comment" /> instances nested under this comment.
        /// </summary>
        public List<IComment> Replies { get; set; } = [];

        /// <summary>
        /// The <see cref="Guid" /> of the SysML v2 model element this comment is attached to in the Concurrent
        /// Server.
        /// </summary>
        public Guid TargetElementId { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="IAuditableThing" />.
        /// </summary>
        public IUser UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
