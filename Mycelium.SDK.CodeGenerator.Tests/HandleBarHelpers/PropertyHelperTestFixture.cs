// ------------------------------------------------------------------------------------------------
//  <copyright file="PropertyHelperTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.HandleBarHelpers
{
    using HandlebarsDotNet;
    using HandlebarsDotNet.Helpers;

    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;
    using Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators;

    using uml4net.Classification;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    [TestFixture]
    public class PropertyHelperTestFixture
    {
        private IClass[] classes = [];
        private IHandlebars jsonSerializerHandlebars;
        private IHandlebars pocoHandlebars;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var result = GeneratorSetupFixture.ReadFunctionalData();
            var functionalData = GeneratorSetupFixture.QueryFunctionalDataPackage(result);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToArray();

            this.pocoHandlebars = Handlebars.CreateSharedEnvironment();
            HandlebarsHelpers.Register(this.pocoHandlebars);
            this.pocoHandlebars.RegisterPocoPropertyHelper();

            this.jsonSerializerHandlebars = Handlebars.CreateSharedEnvironment();

            HandlebarsHelpers.Register(this.jsonSerializerHandlebars);

            this.jsonSerializerHandlebars.RegisterJsonSerializerPropertyHelper();

            this.jsonSerializerHandlebars.RegisterSafeContextHelper();
        }

        [Test]
        public void Verify_that_Dto_and_Poco_property_helpers_can_be_registered_independently()
        {
            var dtoHandlebars = Handlebars.CreateSharedEnvironment();
            HandlebarsHelpers.Register(dtoHandlebars);
            dtoHandlebars.RegisterDtoPropertyHelper();

            var property = this.QueryProperty("ProjectMember", "activeOwnership");
            var dtoTemplate = dtoHandlebars.Compile("{{ #Property.WriteDtoInterfaceDeclaration this }}");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(dtoTemplate(property), Is.EqualTo("Guid? ActiveOwnership { get; set; }"));

                Assert.That(this.RenderPocoInterface(property), Is.EqualTo("IOwnership ActiveOwnership { get; set; }"));
            }
        }

        [Test]
        public void Verify_that_Json_serializer_property_helpers_match_the_FunctionalData_contract()
        {
            var identifier = this.QueryProperty("Thing", "id");

            var ordinaryProperty = this.QueryProperty("BranchProtectionRule", "name");

            var dictionary = this.QueryProperty("FunctionalProject", "sharedPreferences");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(this.RenderJsonSerializerProperty("{{#if (Property.QueryIsIdentifier this)}}identifier{{else}}ordinary{{/if}}", identifier), Is.EqualTo("identifier"));

                Assert.That(this.RenderJsonSerializerProperty("{{#if (Property.QueryIsIdentifier this)}}identifier{{else}}ordinary{{/if}}", ordinaryProperty),
                    Is.EqualTo("ordinary"));

                Assert.That(this.RenderJsonSerializerProperty("{{#if (Property.QueryIsStringDictionary this)}}dictionary{{else}}ordinary{{/if}}", dictionary),
                    Is.EqualTo("dictionary"));

                Assert.That(this.RenderJsonSerializerProperty("{{#if (Property.QueryIsStringDictionary this)}}dictionary{{else}}ordinary{{/if}}", ordinaryProperty),
                    Is.EqualTo("ordinary"));

                Assert.That(this.RenderJsonSerializerProperty("{{Property.WritePropertyName this}}", this.QueryProperty("ProjectMember", "role")), Is.EqualTo("Role"));
            }
        }

        [Test]
        public void Verify_that_Poco_declarations_preserve_nullable_value_types()
        {
            var property = new Property { XmiId = "optional-count", Name = "optionalCount", Type = new PrimitiveType { XmiId = "integer-type", Name = "Integer" } };

            property.LowerValue.Add(new LiteralInteger { Value = 0 });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(this.RenderPocoInterface(property), Is.EqualTo("int? OptionalCount { get; set; }"));

                Assert.That(this.RenderPocoImplementation(property), Is.EqualTo("public int? OptionalCount { get; set; }"));
            }
        }

        [Test]
        public void Verify_that_Poco_derived_union_declarations_delegate_to_computation()
        {
            var property = new Property { XmiId = "derived-union", Name = "derivedUnion", IsDerivedUnion = true, Type = new PrimitiveType { XmiId = "boolean-type", Name = "Boolean" } };

            property.LowerValue.Add(new LiteralInteger { Value = 1 });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(this.RenderPocoInterface(property), Is.EqualTo("bool DerivedUnion { get; }"));

                Assert.That(this.RenderPocoImplementation(property), Is.EqualTo("public bool DerivedUnion => this.ComputeDerivedUnion();"));
            }
        }

        [Test]
        public void Verify_that_Poco_implementation_declarations_match_the_FunctionalData_contract()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(this.RenderPocoImplementation(this.QueryProperty("ProjectMember", "activeOwnership")), Is.EqualTo("public IOwnership ActiveOwnership { get; set; }"));

                Assert.That(this.RenderPocoImplementation(this.QueryProperty("ProjectMember", "owns")), Is.EqualTo("public List<IOwnership> Owns { get; set; } = [];"));

                Assert.That(this.RenderPocoImplementation(this.QueryProperty("ProjectMember", "isOutsideCollaborator")),
                    Is.EqualTo("public bool IsOutsideCollaborator => this.ComputeIsOutsideCollaborator();"));

                Assert.That(this.RenderPocoImplementation(this.QueryProperty("FunctionalProject", "sharedPreferences")),
                    Is.EqualTo("public Dictionary<string,string> SharedPreferences { get; set; } = [];"));

                Assert.That(this.RenderPocoImplementation(this.QueryProperty("Thing", "id")), Is.EqualTo("public Guid Id { get; set; }"));
            }
        }

        [Test]
        public void Verify_that_Poco_interface_declarations_match_the_FunctionalData_contract()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(this.RenderPocoInterface(this.QueryProperty("ProjectMember", "activeOwnership")), Is.EqualTo("IOwnership ActiveOwnership { get; set; }"));

                Assert.That(this.RenderPocoInterface(this.QueryProperty("ProjectMember", "owns")), Is.EqualTo("List<IOwnership> Owns { get; set; }"));

                Assert.That(this.RenderPocoInterface(this.QueryProperty("ProjectMember", "role")), Is.EqualTo("ProjectMemberRole Role { get; set; }"));

                Assert.That(this.RenderPocoInterface(this.QueryProperty("ProjectMember", "isOutsideCollaborator")), Is.EqualTo("bool IsOutsideCollaborator { get; }"));

                Assert.That(this.RenderPocoInterface(this.QueryProperty("Thing", "id")), Is.EqualTo("Guid Id { get; set; }"));
            }
        }

        [Test]
        public void Verify_that_Poco_property_helpers_reject_multiple_arguments()
        {
            var template = this.pocoHandlebars.Compile("{{ #Property.WritePocoInterfaceDeclaration this this }}");

            Assert.That(() => template(new object()), Throws.TypeOf<HandlebarsException>()
                .With.Message.EqualTo("{{Property.WritePocoInterfaceDeclaration}} requires exactly one argument."));
        }

        [Test]
        public void Verify_that_Poco_property_helpers_require_an_IProperty_argument()
        {
            var template = this.pocoHandlebars.Compile("{{ #Property.WritePocoInterfaceDeclaration this }}");

            Assert.That(() => template(new object()), Throws.TypeOf<HandlebarsException>()
                .With.Message.EqualTo("{{Property.WritePocoInterfaceDeclaration}} requires an IProperty argument."));
        }

        [Test]
        public void Verify_that_RegisterJsonSerializerPropertyHelper_rejects_a_null_environment()
        {
            IHandlebars handlebars = null;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => handlebars.RegisterJsonSerializerPropertyHelper(), Throws.ArgumentNullException);

                Assert.That(() => handlebars.RegisterSafeContextHelper(), Throws.ArgumentNullException);
            }
        }

        [Test]
        public void Verify_that_RegisterPocoPropertyHelper_rejects_a_null_environment()
        {
            IHandlebars handlebars = null;

            Assert.That(() => handlebars.RegisterPocoPropertyHelper(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_safe_context_exposes_the_property_and_owning_class()
        {
            var classContext = this.classes.Single(umlClass => umlClass.Name == "ProjectMember");

            var property = this.QueryProperty("ProjectMember", "role");

            var template = this.jsonSerializerHandlebars.Compile(
                "{{#withPropertyClassContext property classContext}}{{property.Name}}:{{classContext.Name}}{{/withPropertyClassContext}}");

            var renderedContext = template(new { property, classContext });

            Assert.That(renderedContext, Is.EqualTo("role:ProjectMember"));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Verify_that_uri_scalars_delegate_after_property_specific_null_handling(bool optional)
        {
            var property = CreateUriProperty(optional);
            var expectedSerialization = optional
                ? """
                  if (dto.Location == null)
                  {
                      writer.WriteNil();
                  }
                  else
                  {
                      UriMessagePackFormatter.Instance.Serialize(ref writer, dto.Location, options);
                  }
                  """
                : """
                  if (dto.Location == null)
                  {
                      throw new MessagePackSerializationException("URI value 'Location' may not be null.");
                  }
                  else
                  {
                      UriMessagePackFormatter.Instance.Serialize(ref writer, dto.Location, options);
                  }
                  """;
            var expectedDeserialization = optional
                ? """
                  if (reader.TryReadNil())
                  {
                      dto.Location = null;
                  }
                  else
                  {
                      dto.Location = UriMessagePackFormatter.Instance.Deserialize(ref reader, options);
                  }
                  """
                : """
                  if (reader.TryReadNil())
                  {
                      throw new MessagePackSerializationException("URI value 'Location' may not be nil.");
                  }
                  else
                  {
                      dto.Location = UriMessagePackFormatter.Instance.Deserialize(ref reader, options);
                  }
                  """;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(Render(property, "Serialization"), Is.EqualTo(expectedSerialization.Replace("<blank>", "    ", StringComparison.Ordinal) + Environment.NewLine));
                Assert.That(Render(property, "Deserialization"), Is.EqualTo(expectedDeserialization.Replace("<blank>", "    ", StringComparison.Ordinal) + Environment.NewLine));
            }
        }

        [Test]
        public void Verify_that_uri_lists_retain_framing_and_delegate_non_null_items()
        {
            var property = CreateUriProperty(false);
            property.UpperValue.Clear();
            property.UpperValue.Add(new LiteralUnlimitedNatural { Value = "*" });

            var expectedSerialization = """
                if (dto.Location == null)
                {
                    writer.WriteNil();
                }
                else
                {
                    writer.WriteArrayHeader(dto.Location.Count);
                <blank>
                    for (var i = 0; i < dto.Location.Count; i++)
                    {
                        if (dto.Location[i] == null)
                        {
                            throw new MessagePackSerializationException("URI value 'Location item' may not be null.");
                        }
                        else
                        {
                            UriMessagePackFormatter.Instance.Serialize(ref writer, dto.Location[i], options);
                        }
                    }
                }
                """;
            var expectedDeserialization = """
                if (reader.TryReadNil())
                {
                    dto.Location = null;
                }
                else
                {
                    var messagePackLocationCount = reader.ReadArrayHeader();
                    dto.Location.Clear();
                <blank>
                    if (dto.Location.Capacity < messagePackLocationCount)
                    {
                        dto.Location.Capacity = messagePackLocationCount;
                    }
                <blank>
                    for (var i = 0; i < messagePackLocationCount; i++)
                    {
                        if (reader.TryReadNil())
                        {
                            throw new MessagePackSerializationException("URI value 'Location item' may not be nil.");
                        }
                        else
                        {
                            dto.Location.Add(UriMessagePackFormatter.Instance.Deserialize(ref reader, options));
                        }
                    }
                }
                """;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(Render(property, "Serialization"), Is.EqualTo(expectedSerialization.Replace("<blank>", "    ", StringComparison.Ordinal) + Environment.NewLine));
                Assert.That(Render(property, "Deserialization"), Is.EqualTo(expectedDeserialization.Replace("<blank>", "    ", StringComparison.Ordinal) + Environment.NewLine));
            }
        }

        [Test]
        public void Verify_that_nullable_DateTime_MessagePack_code_handles_null_and_non_null_values()
        {
            var property = new Property
            {
                XmiId = "optional-date-time",
                Name = "optionalDateTime",
                Type = new DataType { XmiId = "date-time-type", Name = "DateTime" }
            };

            property.LowerValue.Add(new LiteralInteger { Value = 0 });
            property.UpperValue.Add(new LiteralUnlimitedNatural { Value = "1" });

            var expectedSerialization = """
                if (dto.OptionalDateTime.HasValue)
                {
                    WriteRoundTripDateTime(ref writer, dto.OptionalDateTime.Value);
                }
                else
                {
                    writer.WriteNil();
                }
                """;
            var expectedDeserialization = """
                if (reader.TryReadNil())
                {
                    dto.OptionalDateTime = null;
                }
                else
                {
                    dto.OptionalDateTime = ReadRoundTripDateTime(ref reader, "OptionalDateTime");
                }
                """;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(Render(property, "Serialization"), Is.EqualTo(expectedSerialization + Environment.NewLine));

                Assert.That(Render(property, "Deserialization"), Is.EqualTo(expectedDeserialization + Environment.NewLine));
            }
        }

        [Test]
        public void Verify_that_nullable_enumeration_MessagePack_code_handles_null_and_delegates_non_null_values()
        {
            var property = new Property
            {
                XmiId = "optional-enumeration",
                Name = "optionalEnumeration",
                Type = new Enumeration { XmiId = "enumeration-type", Name = "OptionalKind" }
            };

            property.LowerValue.Add(new LiteralInteger { Value = 0 });
            property.UpperValue.Add(new LiteralUnlimitedNatural { Value = "1" });

            var expectedSerialization = """
                if (dto.OptionalEnumeration.HasValue)
                {
                    OptionalKindMessagePackFormatter.Instance.Serialize(ref writer, dto.OptionalEnumeration.Value, options);
                }
                else
                {
                    writer.WriteNil();
                }
                """;
            var expectedDeserialization = """
                if (reader.TryReadNil())
                {
                    dto.OptionalEnumeration = null;
                }
                else
                {
                    dto.OptionalEnumeration = OptionalKindMessagePackFormatter.Instance.Deserialize(ref reader, options);
                }
                """;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(Render(property, "Serialization"), Is.EqualTo(expectedSerialization + Environment.NewLine));

                Assert.That(Render(property, "Deserialization"), Is.EqualTo(expectedDeserialization + Environment.NewLine));
            }
        }

        [Test]
        public void VerifyThatNullableUuidDataTypesDelegateNonNullValuesToTheGuidFormatter()
        {
            var property = new Property
            {
                XmiId = "optional-uuid",
                Name = "optionalUuid",
                Type = new DataType { XmiId = "uuid-type", Name = "UUID" }
            };

            property.LowerValue.Add(new LiteralInteger { Value = 0 });
            property.UpperValue.Add(new LiteralUnlimitedNatural { Value = "1" });

            var expectedSerialization = """
                if (dto.OptionalUuid.HasValue)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.OptionalUuid.Value, options);
                }
                else
                {
                    writer.WriteNil();
                }
                """;
            var expectedDeserialization = """
                if (reader.TryReadNil())
                {
                    dto.OptionalUuid = null;
                }
                else
                {
                    dto.OptionalUuid = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                }
                """;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(Render(property, "Serialization"), Is.EqualTo(expectedSerialization + Environment.NewLine));
                Assert.That(Render(property, "Deserialization"), Is.EqualTo(expectedDeserialization + Environment.NewLine));
            }
        }

        private static Property CreateUriProperty(bool optional)
        {
            var property = new Property
            {
                XmiId = "uri-property",
                Name = "location",
                Type = new DataType { XmiId = "uri-type", Name = "URI" }
            };

            property.LowerValue.Add(new LiteralInteger { Value = optional ? 0 : 1 });
            property.UpperValue.Add(new LiteralUnlimitedNatural { Value = "1" });

            return property;
        }

        private static string Render(Property property, string operation)
        {
            var handlebars = Handlebars.CreateSharedEnvironment();
            handlebars.RegisterMessagePackFormatterPropertyHelper();

            var template = handlebars.Compile("{{ #Property.WriteMessagePack" + operation + " this }}");

            return template(property);
        }

        private string RenderJsonSerializerProperty(string templateText, IProperty property)
        {
            var template = this.jsonSerializerHandlebars.Compile(templateText);

            return template(property);
        }

        private string RenderPocoInterface(IProperty property)
        {
            var template = this.pocoHandlebars.Compile("{{ #Property.WritePocoInterfaceDeclaration this }}");

            return template(property);
        }

        private string RenderPocoImplementation(IProperty property)
        {
            var template = this.pocoHandlebars.Compile("{{ #Property.WritePocoImplementationDeclaration this }}");

            return template(property);
        }

        private IProperty QueryProperty(string className, string propertyName)
        {
            return this.classes
                .Single(umlClass => umlClass.Name == className).OwnedAttribute
                .Single(property => property.Name == propertyName);
        }
    }
}
