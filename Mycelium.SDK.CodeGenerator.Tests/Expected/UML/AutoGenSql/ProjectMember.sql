CREATE TABLE "Fabric"."ProjectMember" (
    "id" uuid NOT NULL,
    "activeOwnership" uuid,
    "isPartOf" uuid NOT NULL,
    "user" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."ProjectMember_owns__Ownership" (
    "sourceProjectMember" uuid NOT NULL,
    "targetOwnership" uuid NOT NULL,
    PRIMARY KEY ("sourceProjectMember", "targetOwnership")
);

ALTER TABLE "Fabric"."ProjectMember"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."ProjectMember"
    ADD FOREIGN KEY ("activeOwnership") REFERENCES "Fabric"."Ownership" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."ProjectMember" ("activeOwnership");

ALTER TABLE "Fabric"."ProjectMember"
    ADD FOREIGN KEY ("isPartOf") REFERENCES "Fabric"."FunctionalProject" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."ProjectMember" ("isPartOf");

ALTER TABLE "Fabric"."ProjectMember_owns__Ownership"
    ADD FOREIGN KEY ("sourceProjectMember") REFERENCES "Fabric"."ProjectMember" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."ProjectMember_owns__Ownership"
    ADD FOREIGN KEY ("targetOwnership") REFERENCES "Fabric"."Ownership" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."ProjectMember_owns__Ownership" ("targetOwnership");

ALTER TABLE "Fabric"."ProjectMember"
    ADD FOREIGN KEY ("user") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."ProjectMember" ("user");

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."ProjectMember"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."ProjectMember"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();
