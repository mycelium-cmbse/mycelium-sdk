CREATE TABLE "Fabric"."BranchProtectionRule" (
    "id" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."BranchProtectionRule_defaultReviewers__User" (
    "sourceBranchProtectionRule" uuid NOT NULL,
    "targetUser" uuid NOT NULL,
    PRIMARY KEY ("sourceBranchProtectionRule", "targetUser")
);

ALTER TABLE "Fabric"."BranchProtectionRule"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."BranchProtectionRule_defaultReviewers__User"
    ADD FOREIGN KEY ("sourceBranchProtectionRule") REFERENCES "Fabric"."BranchProtectionRule" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."BranchProtectionRule_defaultReviewers__User"
    ADD FOREIGN KEY ("targetUser") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."BranchProtectionRule_defaultReviewers__User" ("targetUser");

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."BranchProtectionRule"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."BranchProtectionRule"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();
