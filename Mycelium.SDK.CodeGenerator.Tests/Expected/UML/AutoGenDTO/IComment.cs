// ------------------------------------------------------------------------------------------------
//  <copyright file="IComment.cs" company="Starion Group S.A.">
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
    /// Represents a user comment attached to a <see cref="Review" />, targeting a specific model element.
    /// Supports threaded replies.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IComment : IAuditableThing
    {
        /// <summary>
        /// References the <see cref="ProjectMember" /> who authored this <see cref="Comment" />.
        /// </summary>
        Guid author { get; }

        /// <summary>
        /// Defines the current status of the current Comment.
        /// </summary>
        CommentStatus commentStatus { get; }

        /// <summary>
        /// The textual content of the comment.
        /// </summary>
        string content { get; }

        /// <summary>
        /// References a quoted <see cref="Comment" />
        /// </summary>
        Guid? quotes { get; }
        
        /// <summary>
        /// Contains threaded reply <see cref="Comment" /> instances nested under this comment.
        /// </summary>
        List<Guid> replies { get; }

        /// <summary>
        /// The <see cref="Guid" /> of the SysML v2 model element this comment is attached to in the Concurrent Server.
        /// </summary>
        Guid targetElementId { get; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
