// ------------------------------------------------------------------------------------------------
//  <copyright file="SqlSchemaScriptsTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Dal.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mycelium.SDK.Dal;

    [TestFixture]
    public class SqlSchemaScriptsTestFixture
    {
        private const string InitialScriptName = "0001_initial.sql";

        private const string ScriptNamePattern = @"^\d{4}_[a-z0-9_.]+\.sql$";

        [Test]
        public void VerifyThatGetAllReturnsTheInitialScriptAndOnlyWellFormedScriptsInOrdinalOrder()
        {
            var scripts = SqlSchemaScripts.GetAll();
            var names = scripts.Select(script => script.Name).ToList();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(names, Does.Contain(InitialScriptName), $"Mycelium.SDK.Dal must embed '{InitialScriptName}' under AutoGenSql/.");
                Assert.That(names, Has.All.Match(ScriptNamePattern), "Every embedded script name must be a four-digit sequence number followed by a lowercase name.");
                Assert.That(names, Is.Ordered.Using((IComparer<string>)StringComparer.Ordinal), "Scripts must be returned in ordinal name order so they can be applied in sequence.");

                foreach (var (name, sql) in scripts)
                {
                    Assert.That(sql, Is.Not.Empty, $"Embedded script '{name}' is empty.");
                    Assert.That(sql[0], Is.Not.EqualTo('﻿'), $"Embedded script '{name}' starts with a byte-order mark.");
                    Assert.That(sql, Does.Not.Contain("\r"), $"Embedded script '{name}' must be returned with LF line endings only.");
                    Assert.That(sql, Does.Not.Contain("{{"), $"Embedded script '{name}' contains an unresolved Handlebars expression.");
                }
            }
        }
    }
}
