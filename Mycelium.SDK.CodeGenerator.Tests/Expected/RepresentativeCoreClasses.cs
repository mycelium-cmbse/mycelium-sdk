// ------------------------------------------------------------------------------------------------
//  <copyright file="RepresentativeCoreClasses.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Expected
{
    using System.Collections;

    // The concrete INTERESTING CLASSIFIERS of the uml4net model inspector report on the SysML and KerML packages.
    public sealed class RepresentativeCoreClasses : IEnumerable<string>
    {
        private static readonly string[] Names =
        [
            "AcceptActionUsage",
            "AnnotatingElement",
            "Annotation",
            "Association",
            "AttributeUsage",
            "BooleanExpression",
            "CalculationUsage",
            "CaseUsage",
            "ConjugatedPortDefinition",
            "ConjugatedPortTyping",
            "ConnectionUsage",
            "Connector",
            "Dependency",
            "EnumerationDefinition",
            "Feature",
            "FeatureTyping",
            "FeatureValue",
            "Flow",
            "FramedConcernMembership",
            "InterfaceUsage",
            "LiteralInteger",
            "LiteralRational",
            "Membership",
            "MultiplicityRange",
            "Namespace",
            "OccurrenceUsage",
            "OwningMembership",
            "ReferenceSubsetting",
            "RequirementUsage",
            "SelectExpression",
            "Subclassification",
            "TextualRepresentation",
            "TriggerInvocationExpression",
            "Usage",
            "VariantMembership",
            "VerificationCaseDefinition",
            "ViewpointDefinition"
        ];

        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)Names).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
