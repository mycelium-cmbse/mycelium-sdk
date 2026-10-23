CREATE TABLE "Fabric"."FunctionalProject" (
    "id" uuid NOT NULL,
    "belongsTo" uuid NOT NULL,
    "policy" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."FunctionalProject_branchRules__BranchProtectionRule" (
    "sourceFunctionalProject" uuid NOT NULL,
    "targetBranchProtectionRule" uuid NOT NULL,
    PRIMARY KEY ("sourceFunctionalProject", "targetBranchProtectionRule")
);

CREATE TABLE "Fabric"."FunctionalProject_defines__Ownership" (
    "sourceFunctionalProject" uuid NOT NULL,
    "targetOwnership" uuid NOT NULL,
    PRIMARY KEY ("sourceFunctionalProject", "targetOwnership")
);

CREATE TABLE "Fabric"."FunctionalProject_involves__ProjectMember" (
    "sourceFunctionalProject" uuid NOT NULL,
    "targetProjectMember" uuid NOT NULL,
    PRIMARY KEY ("sourceFunctionalProject", "targetProjectMember")
);

CREATE TABLE "Fabric"."FunctionalProject_reviews__Review" (
    "sourceFunctionalProject" uuid NOT NULL,
    "targetReview" uuid NOT NULL,
    PRIMARY KEY ("sourceFunctionalProject", "targetReview")
);

ALTER TABLE "Fabric"."FunctionalProject"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."FunctionalProject"
    ADD FOREIGN KEY ("belongsTo") REFERENCES "Fabric"."Organization" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."FunctionalProject" ("belongsTo");

ALTER TABLE "Fabric"."FunctionalProject_branchRules__BranchProtectionRule"
    ADD FOREIGN KEY ("sourceFunctionalProject") REFERENCES "Fabric"."FunctionalProject" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."FunctionalProject_branchRules__BranchProtectionRule"
    ADD FOREIGN KEY ("targetBranchProtectionRule") REFERENCES "Fabric"."BranchProtectionRule" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."FunctionalProject_branchRules__BranchProtectionRule" ("targetBranchProtectionRule");

ALTER TABLE "Fabric"."FunctionalProject_defines__Ownership"
    ADD FOREIGN KEY ("sourceFunctionalProject") REFERENCES "Fabric"."FunctionalProject" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."FunctionalProject_defines__Ownership"
    ADD FOREIGN KEY ("targetOwnership") REFERENCES "Fabric"."Ownership" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."FunctionalProject_defines__Ownership" ("targetOwnership");

ALTER TABLE "Fabric"."FunctionalProject_involves__ProjectMember"
    ADD FOREIGN KEY ("sourceFunctionalProject") REFERENCES "Fabric"."FunctionalProject" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."FunctionalProject_involves__ProjectMember"
    ADD FOREIGN KEY ("targetProjectMember") REFERENCES "Fabric"."ProjectMember" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."FunctionalProject_involves__ProjectMember" ("targetProjectMember");

ALTER TABLE "Fabric"."FunctionalProject"
    ADD FOREIGN KEY ("policy") REFERENCES "Fabric"."FunctionalProjectPolicy" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."FunctionalProject" ("policy");

ALTER TABLE "Fabric"."FunctionalProject_reviews__Review"
    ADD FOREIGN KEY ("sourceFunctionalProject") REFERENCES "Fabric"."FunctionalProject" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."FunctionalProject_reviews__Review"
    ADD FOREIGN KEY ("targetReview") REFERENCES "Fabric"."Review" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."FunctionalProject_reviews__Review" ("targetReview");

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."FunctionalProject"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."FunctionalProject"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();
