CREATE TABLE "Fabric"."FunctionalProjectPolicy" (
    "id" uuid NOT NULL,
    PRIMARY KEY ("id")
);

ALTER TABLE "Fabric"."FunctionalProjectPolicy"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."FunctionalProjectPolicy"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."FunctionalProjectPolicy"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();
