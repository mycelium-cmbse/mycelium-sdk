// ------------------------------------------------------------------------------------------------
//  <copyright file="EnumerationContractTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Tests.Enumeration
{
    using System;
    using System.Linq;

    using Mycelium.SDK;

    /// <summary>
    /// Verifies generated enumeration runtime behavior without maintaining a model inventory.
    /// </summary>
    [TestFixture]
    public class EnumerationContractTestFixture
    {
        private const string EnumerationNamespace = "Mycelium.SDK";

        [Test]
        public void Verify_that_generated_enumerations_are_public_and_round_trip_their_names()
        {
            var enumerationTypes = typeof(ActivationStatus).Assembly
                .GetTypes()
                .Where(
                    type => type.IsEnum
                            && type.Namespace == EnumerationNamespace)
                .OrderBy(type => type.Name, StringComparer.Ordinal);

            foreach (var enumerationType in enumerationTypes)
            {
                Assert.That(
                    enumerationType.IsPublic,
                    Is.True,
                    $"Generated enumeration '{enumerationType.Name}' is not public.");

                foreach (var literalName in Enum.GetNames(enumerationType))
                {
                    var parsed = Enum.TryParse(
                        enumerationType,
                        literalName,
                        ignoreCase: false,
                        out var value);

                    Assert.That(
                        parsed,
                        Is.True,
                        $"Enumeration '{enumerationType.Name}' did not parse its declared literal '{literalName}'.");

                    if (!parsed)
                    {
                        continue;
                    }

                    Assert.That(
                        Enum.GetName(enumerationType, value!),
                        Is.EqualTo(literalName),
                        $"Enumeration '{enumerationType.Name}' did not preserve literal '{literalName}'.");
                }
            }
        }
    }
}
