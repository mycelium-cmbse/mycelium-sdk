// ------------------------------------------------------------------------------------------------
//  <copyright file="RepresentativeElementClasses.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Expected
{
    using System.Collections;

    /// <summary>
    /// Supplies the SysML v2 and KerML metaclass names selected for representative generated-source golden comparisons.
    /// The selection mirrors the INTERESTING CLASSES sections of the uml4net model inspector report of SysML_only_xmi.uml (packages SysML and KerML).
    /// </summary>
    public sealed class RepresentativeElementClasses : IEnumerable<string>
    {
        /// <summary>
        /// The selected metaclass names in test-case enumeration order.
        /// </summary>
        private static readonly string[] Names =
        [
            "AcceptActionUsage",
            "AnnotatingElement",
            "Annotation",
            "Association",
            "BooleanExpression",
            "ConnectionUsage",
            "Dependency",
            "Element",
            "EnumerationDefinition",
            "Expose",
            "Feature",
            "FeatureChainExpression",
            "FeatureMembership",
            "FeatureTyping",
            "Flow",
            "LiteralInteger",
            "LiteralRational",
            "Membership",
            "MultiplicityRange",
            "Namespace",
            "OccurrenceUsage",
            "OwningMembership",
            "PortConjugation",
            "ReferenceSubsetting",
            "Relationship",
            "RequirementConstraintMembership",
            "RequirementUsage",
            "Subclassification",
            "TextualRepresentation",
            "Usage",
            "VerificationCaseDefinition",
            "ViewpointDefinition"
        ];

        /// <summary>
        /// Returns an enumerator over the selected metaclass names.
        /// </summary>
        /// <returns>
        /// An enumerator over the metaclass names used for representative golden comparisons.
        /// </returns>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)Names).GetEnumerator();

        /// <summary>
        /// Returns an enumerator over the selected metaclass names.
        /// </summary>
        /// <returns>
        /// An enumerator over the metaclass names used for representative golden comparisons.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
