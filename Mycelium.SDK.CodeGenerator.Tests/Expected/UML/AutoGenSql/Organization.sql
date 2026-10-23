CREATE TABLE "Fabric"."Organization" (
    "id" uuid NOT NULL,
    "policy" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."Organization_involvedUser__OrganizationMember" (
    "sourceOrganization" uuid NOT NULL,
    "targetOrganizationMember" uuid NOT NULL,
    PRIMARY KEY ("sourceOrganization", "targetOrganizationMember")
);

CREATE TABLE "Fabric"."Organization_projects__FunctionalProject" (
    "sourceOrganization" uuid NOT NULL,
    "targetFunctionalProject" uuid NOT NULL,
    PRIMARY KEY ("sourceOrganization", "targetFunctionalProject")
);

ALTER TABLE "Fabric"."Organization"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."Organization_involvedUser__OrganizationMember"
    ADD FOREIGN KEY ("sourceOrganization") REFERENCES "Fabric"."Organization" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."Organization_involvedUser__OrganizationMember"
    ADD FOREIGN KEY ("targetOrganizationMember") REFERENCES "Fabric"."OrganizationMember" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."Organization_involvedUser__OrganizationMember" ("targetOrganizationMember");

ALTER TABLE "Fabric"."Organization"
    ADD FOREIGN KEY ("policy") REFERENCES "Fabric"."OrganizationPolicy" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."Organization" ("policy");

ALTER TABLE "Fabric"."Organization_projects__FunctionalProject"
    ADD FOREIGN KEY ("sourceOrganization") REFERENCES "Fabric"."Organization" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."Organization_projects__FunctionalProject"
    ADD FOREIGN KEY ("targetFunctionalProject") REFERENCES "Fabric"."FunctionalProject" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."Organization_projects__FunctionalProject" ("targetFunctionalProject");

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."Organization"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."Organization"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();
