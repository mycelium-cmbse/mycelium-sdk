CREATE TABLE "Fabric"."Comment" (
    "id" uuid NOT NULL,
    "author" uuid NOT NULL,
    "quotes" uuid,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."Comment_replies__Comment" (
    "sourceComment" uuid NOT NULL,
    "targetComment" uuid NOT NULL,
    PRIMARY KEY ("sourceComment", "targetComment")
);

ALTER TABLE "Fabric"."Comment"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."Comment"
    ADD FOREIGN KEY ("author") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."Comment" ("author");

ALTER TABLE "Fabric"."Comment"
    ADD FOREIGN KEY ("quotes") REFERENCES "Fabric"."Comment" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."Comment" ("quotes");

ALTER TABLE "Fabric"."Comment_replies__Comment"
    ADD FOREIGN KEY ("sourceComment") REFERENCES "Fabric"."Comment" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."Comment_replies__Comment"
    ADD FOREIGN KEY ("targetComment") REFERENCES "Fabric"."Comment" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."Comment_replies__Comment" ("targetComment");

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."Comment"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."Comment"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();
