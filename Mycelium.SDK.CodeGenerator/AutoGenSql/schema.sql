-- ------------------------------------------------------------------------------------------------
-- <copyright file="schema.sql" company="Starion Group S.A.">
--
--   Copyright 2026 Starion Group S.A.
--   SPDX-License-Identifier: Apache-2.0
--
-- </copyright>
-- ------------------------------------------------------------------------------------------------
-- THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!
-- Fabric #33: initial PostgreSQL 18 schema; the consumer owns the transaction.
-- Mycelium.Model.Fabric 0.2.0
-- Mycelium.Model.CommonPrimitives 0.4.0

CREATE SCHEMA IF NOT EXISTS "Fabric";

CREATE TABLE "Fabric"."AuditableThing" (
    "id" uuid NOT NULL,
    "createdBy" uuid NOT NULL,
    "updatedBy" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."BranchProtectionRule" (
    "id" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."BranchProtectionRule_defaultReviewers__User" (
    "sourceBranchProtectionRule" uuid NOT NULL,
    "targetUser" uuid NOT NULL,
    PRIMARY KEY ("sourceBranchProtectionRule", "targetUser")
);

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

CREATE TABLE "Fabric"."FunctionalProjectPolicy" (
    "id" uuid NOT NULL,
    PRIMARY KEY ("id")
);

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

CREATE TABLE "Fabric"."OrganizationMember" (
    "id" uuid NOT NULL,
    "organization" uuid NOT NULL,
    "user" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."OrganizationPolicy" (
    "id" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."Ownership" (
    "id" uuid NOT NULL,
    PRIMARY KEY ("id")
);

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

CREATE TABLE "Fabric"."Review" (
    "id" uuid NOT NULL,
    "author" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."Review_comments__Comment" (
    "sourceReview" uuid NOT NULL,
    "targetComment" uuid NOT NULL,
    PRIMARY KEY ("sourceReview", "targetComment")
);

CREATE TABLE "Fabric"."Review_reviewers__User" (
    "sourceReview" uuid NOT NULL,
    "targetUser" uuid NOT NULL,
    PRIMARY KEY ("sourceReview", "targetUser")
);

CREATE TABLE "Fabric"."Thing" (
    "id" uuid NOT NULL,
    "classKind" text,
    "data" jsonb,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."User" (
    "id" uuid NOT NULL,
    PRIMARY KEY ("id")
);

CREATE TABLE "Fabric"."User_isPartOfOrganizations__OrganizationMember" (
    "sourceUser" uuid NOT NULL,
    "targetOrganizationMember" uuid NOT NULL,
    PRIMARY KEY ("sourceUser", "targetOrganizationMember")
);

CREATE TABLE "Fabric"."User_isPartOfProjects__ProjectMember" (
    "sourceUser" uuid NOT NULL,
    "targetProjectMember" uuid NOT NULL,
    PRIMARY KEY ("sourceUser", "targetProjectMember")
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

ALTER TABLE "Fabric"."FunctionalProjectPolicy"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

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

ALTER TABLE "Fabric"."OrganizationMember"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."OrganizationMember"
    ADD FOREIGN KEY ("organization") REFERENCES "Fabric"."Organization" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."OrganizationMember" ("organization");

ALTER TABLE "Fabric"."OrganizationMember"
    ADD FOREIGN KEY ("user") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."OrganizationMember" ("user");

ALTER TABLE "Fabric"."OrganizationPolicy"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."Ownership"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

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

ALTER TABLE "Fabric"."Review"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."Review"
    ADD FOREIGN KEY ("author") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."Review" ("author");

ALTER TABLE "Fabric"."Review_comments__Comment"
    ADD FOREIGN KEY ("sourceReview") REFERENCES "Fabric"."Review" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."Review_comments__Comment"
    ADD FOREIGN KEY ("targetComment") REFERENCES "Fabric"."Comment" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."Review_comments__Comment" ("targetComment");

ALTER TABLE "Fabric"."Review_reviewers__User"
    ADD FOREIGN KEY ("sourceReview") REFERENCES "Fabric"."Review" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."Review_reviewers__User"
    ADD FOREIGN KEY ("targetUser") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."Review_reviewers__User" ("targetUser");

ALTER TABLE "Fabric"."User"
    ADD FOREIGN KEY ("id") REFERENCES "Fabric"."Thing" ("id")
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."User_isPartOfOrganizations__OrganizationMember"
    ADD FOREIGN KEY ("sourceUser") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."User_isPartOfOrganizations__OrganizationMember"
    ADD FOREIGN KEY ("targetOrganizationMember") REFERENCES "Fabric"."OrganizationMember" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."User_isPartOfOrganizations__OrganizationMember" ("targetOrganizationMember");

ALTER TABLE "Fabric"."User_isPartOfProjects__ProjectMember"
    ADD FOREIGN KEY ("sourceUser") REFERENCES "Fabric"."User" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE "Fabric"."User_isPartOfProjects__ProjectMember"
    ADD FOREIGN KEY ("targetProjectMember") REFERENCES "Fabric"."ProjectMember" ("id")
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ON "Fabric"."User_isPartOfProjects__ProjectMember" ("targetProjectMember");

CREATE FUNCTION "Fabric"."auditablething_delete"()
RETURNS trigger
LANGUAGE plpgsql
AS $mycelium$
BEGIN
    DELETE FROM "Fabric"."AuditableThing" WHERE "id" = OLD."id";
    RETURN OLD;
END;
$mycelium$;

CREATE FUNCTION "Fabric"."thing_delete"()
RETURNS trigger
LANGUAGE plpgsql
AS $mycelium$
BEGIN
    DELETE FROM "Fabric"."Thing" WHERE "id" = OLD."id";
    RETURN OLD;
END;
$mycelium$;

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."AuditableThing"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."BranchProtectionRule"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."BranchProtectionRule"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."Comment"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."Comment"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."FunctionalProject"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."FunctionalProject"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."FunctionalProjectPolicy"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."FunctionalProjectPolicy"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."Organization"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."Organization"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."OrganizationMember"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."OrganizationMember"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."OrganizationPolicy"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."OrganizationPolicy"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."Ownership"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."Ownership"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."ProjectMember"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."ProjectMember"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."Review"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."Review"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();

CREATE TRIGGER "trg_thing_delete"
    AFTER DELETE ON "Fabric"."User"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."thing_delete"();

CREATE TRIGGER "trg_auditablething_delete"
    AFTER DELETE ON "Fabric"."User"
    FOR EACH ROW EXECUTE FUNCTION "Fabric"."auditablething_delete"();
