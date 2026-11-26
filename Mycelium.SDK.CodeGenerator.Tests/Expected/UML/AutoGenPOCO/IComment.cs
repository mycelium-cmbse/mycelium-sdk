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
    public partial interface IComment : IMessage
    {
        /// <summary>
        /// Contains threaded reply instances nested under this comment.
        /// </summary>
        List<IReply> Replies { get; set; }

        /// <summary>
        /// Defines the current status of the current Comment.
        /// </summary>
        CommentStatus Status { get; set; }

        /// <summary>
        /// The <see cref="Guid" /> of the SysML v2 model element this comment is attached to in the Concurrent
        /// Server.
        /// </summary>
        Guid TargetElementId { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
