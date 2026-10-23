CREATE TABLE "Fabric"."AuditableThing" (
    "id" uuid NOT NULL,
    "createdBy" uuid NOT NULL,
    "updatedBy" uuid NOT NULL,
    PRIMARY KEY ("id")
);

ALTER TABLE "Fabric"."AuditableThing"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."AuditableThing"
    ADD FOREIGN KEY ("createdBy") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."AuditableThing" ("createdBy");

ALTER TABLE "Fabric"."AuditableThing"
    ADD FOREIGN KEY ("updatedBy") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."AuditableThing" ("updatedBy");

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."AuditableThing"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();
