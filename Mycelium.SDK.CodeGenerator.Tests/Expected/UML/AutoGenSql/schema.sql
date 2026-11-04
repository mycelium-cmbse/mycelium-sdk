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
-- Mycelium.Model.Fabric 0.3.0
-- Mycelium.Model.CommonPrimitives 0.4.0

CREATE SCHEMA IF NOT EXISTS fabric;

CREATE TABLE fabric.auditable_thing (
    id uuid NOT NULL,
    created_by uuid NOT NULL,
    updated_by uuid NOT NULL,
    CONSTRAINT pk_auditable_thing_id_66ca3296260efbf7 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.branch_protection_rule (
    id uuid NOT NULL,
    CONSTRAINT pk_branch_protection_rule_id_d242ae8c51645501 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.branch_protection_rule_default_reviewers_user (
    source_branch_protection_rule uuid NOT NULL,
    target_user uuid NOT NULL,
    CONSTRAINT pk_branch_protection_rule_default_reviewers_us_ff66b8269acbe382 PRIMARY KEY (source_branch_protection_rule, target_user)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.comment (
    id uuid NOT NULL,
    CONSTRAINT pk_comment_id_669e58f0a8642c17 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.comment_replies_reply (
    source_comment uuid NOT NULL,
    target_reply uuid NOT NULL,
    CONSTRAINT pk_comment_replies_reply_source_comment_target_939e4b8264998800 PRIMARY KEY (source_comment, target_reply)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.functional_project (
    id uuid NOT NULL,
    belongs_to uuid NOT NULL,
    policy uuid NOT NULL,
    CONSTRAINT pk_functional_project_id_20fcc0dccc5138c3 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.functional_project_branch_rules_branch_protection_rule (
    source_functional_project uuid NOT NULL,
    target_branch_protection_rule uuid NOT NULL,
    CONSTRAINT pk_functional_project_branch_rules_branch_prot_50a64003b2111785 PRIMARY KEY (source_functional_project, target_branch_protection_rule)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.functional_project_defines_ownership (
    source_functional_project uuid NOT NULL,
    target_ownership uuid NOT NULL,
    CONSTRAINT pk_functional_project_defines_ownership_source_b17e242735075122 PRIMARY KEY (source_functional_project, target_ownership)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.functional_project_reviews_review (
    source_functional_project uuid NOT NULL,
    target_review uuid NOT NULL,
    CONSTRAINT pk_functional_project_reviews_review_source_fu_d6ea2c891a96c8b5 PRIMARY KEY (source_functional_project, target_review)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.functional_project_policy (
    id uuid NOT NULL,
    CONSTRAINT pk_functional_project_policy_id_8a2af00df0dd8092 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.message (
    id uuid NOT NULL,
    quotes uuid,
    CONSTRAINT pk_message_id_8d82859e352a261a PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.organization (
    id uuid NOT NULL,
    policy uuid NOT NULL,
    CONSTRAINT pk_organization_id_2b07e26c759bc143 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.organization_member (
    id uuid NOT NULL,
    organization uuid NOT NULL,
    model_user uuid NOT NULL,
    CONSTRAINT pk_organization_member_id_7b0f8b2a37fc414c PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.organization_policy (
    id uuid NOT NULL,
    CONSTRAINT pk_organization_policy_id_db4f4f0aed613a80 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.ownership (
    id uuid NOT NULL,
    CONSTRAINT pk_ownership_id_e676b4b73574efdd PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.project_member (
    id uuid NOT NULL,
    active_ownership uuid,
    is_part_of uuid NOT NULL,
    model_user uuid NOT NULL,
    CONSTRAINT pk_project_member_id_2f2f5651e107fbd9 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.project_member_owns_ownership (
    source_project_member uuid NOT NULL,
    target_ownership uuid NOT NULL,
    CONSTRAINT pk_project_member_owns_ownership_source_projec_1b4d0a7cf683b9f2 PRIMARY KEY (source_project_member, target_ownership)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.reply (
    id uuid NOT NULL,
    CONSTRAINT pk_reply_id_2967daf2494819a0 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.review (
    id uuid NOT NULL,
    CONSTRAINT pk_review_id_898c2f8b172ba2b8 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.review_comments_comment (
    source_review uuid NOT NULL,
    target_comment uuid NOT NULL,
    CONSTRAINT pk_review_comments_comment_source_review_targe_e10bf666381ddfb6 PRIMARY KEY (source_review, target_comment)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.review_reviewers_user (
    source_review uuid NOT NULL,
    target_user uuid NOT NULL,
    CONSTRAINT pk_review_reviewers_user_source_review_target__288467e2f54d2572 PRIMARY KEY (source_review, target_user)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.thing (
    id uuid NOT NULL,
    class_kind text NOT NULL,
    data jsonb NOT NULL,
    CONSTRAINT ck_thing_class_kind_a5e3c76a43ee76df CHECK (class_kind IN ('BranchProtectionRule', 'Comment', 'FunctionalProject', 'FunctionalProjectPolicy', 'Organization', 'OrganizationMember', 'OrganizationPolicy', 'Ownership', 'ProjectMember', 'Reply', 'Review', 'User')),
    CONSTRAINT ck_thing_data_metadata_b7364e19b70378a6 CHECK ((
        jsonb_typeof(data) = 'object'
        AND jsonb_typeof(data -> '@id') = 'string'
        AND jsonb_typeof(data -> '@type') = 'string'
        AND (data ->> '@id')::uuid = id
        AND data ->> '@type' = class_kind
    ) IS TRUE),
    CONSTRAINT pk_thing_id_d4b927f80093cf60 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.model_user (
    id uuid NOT NULL,
    CONSTRAINT pk_model_user_id_1e4cd347ffaea3a5 PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

ALTER TABLE fabric.auditable_thing
    ADD CONSTRAINT fk_auditable_thing_id_thing_1f9e0d452a4e6fe8 FOREIGN KEY (id) REFERENCES fabric.thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.auditable_thing
    ADD CONSTRAINT fk_auditable_thing_created_by_model_user_06d1ba260c71cede FOREIGN KEY (created_by) REFERENCES fabric.model_user (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_auditable_thing_created_by_181d7a82eafadc2f ON fabric.auditable_thing (created_by);

ALTER TABLE fabric.auditable_thing
    ADD CONSTRAINT fk_auditable_thing_updated_by_model_user_b26aae25254c211b FOREIGN KEY (updated_by) REFERENCES fabric.model_user (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_auditable_thing_updated_by_81ae399ef82bdf47 ON fabric.auditable_thing (updated_by);

ALTER TABLE fabric.branch_protection_rule
    ADD CONSTRAINT fk_branch_protection_rule_id_auditable_thing_2181d8456f67e8d1 FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.branch_protection_rule_default_reviewers_user
    ADD CONSTRAINT fk_branch_protection_rule_default_reviewers_us_878bb6f0a64b3f14 FOREIGN KEY (source_branch_protection_rule) REFERENCES fabric.branch_protection_rule (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.branch_protection_rule_default_reviewers_user
    ADD CONSTRAINT fk_branch_protection_rule_default_reviewers_us_c201ef7079e374fa FOREIGN KEY (target_user) REFERENCES fabric.model_user (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_branch_protection_rule_default_reviewers_us_69d81bcb9adc03ac ON fabric.branch_protection_rule_default_reviewers_user (target_user);

ALTER TABLE fabric.comment
    ADD CONSTRAINT fk_comment_id_message_f7e2195dd6e624a0 FOREIGN KEY (id) REFERENCES fabric.message (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.comment_replies_reply
    ADD CONSTRAINT fk_comment_replies_reply_source_comment_commen_56ff440013756b61 FOREIGN KEY (source_comment) REFERENCES fabric.comment (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.comment_replies_reply
    ADD CONSTRAINT fk_comment_replies_reply_target_reply_reply_c6ac9760579eab36 FOREIGN KEY (target_reply) REFERENCES fabric.reply (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.comment_replies_reply
    ADD CONSTRAINT uq_comment_replies_reply_target_reply_25c4943af7db9897 UNIQUE (target_reply) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project
    ADD CONSTRAINT fk_functional_project_id_auditable_thing_71fac0c45ca8c932 FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project
    ADD CONSTRAINT fk_functional_project_belongs_to_organization_080403d7c7ed427a FOREIGN KEY (belongs_to) REFERENCES fabric.organization (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_functional_project_belongs_to_b26b71a4bde8a1a4 ON fabric.functional_project (belongs_to);

ALTER TABLE fabric.functional_project
    ADD CONSTRAINT fk_functional_project_policy_functional_projec_2367343fd3c9090e FOREIGN KEY (policy) REFERENCES fabric.functional_project_policy (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project
    ADD CONSTRAINT uq_functional_project_policy_6b41245d01e6369b UNIQUE (policy) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project_branch_rules_branch_protection_rule
    ADD CONSTRAINT fk_functional_project_branch_rules_branch_prot_eb952a2165a2912b FOREIGN KEY (source_functional_project) REFERENCES fabric.functional_project (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_branch_rules_branch_protection_rule
    ADD CONSTRAINT fk_functional_project_branch_rules_branch_prot_e7de2a33423a4d98 FOREIGN KEY (target_branch_protection_rule) REFERENCES fabric.branch_protection_rule (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_branch_rules_branch_protection_rule
    ADD CONSTRAINT uq_functional_project_branch_rules_branch_prot_157aff4638de9312 UNIQUE (target_branch_protection_rule) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project_defines_ownership
    ADD CONSTRAINT fk_functional_project_defines_ownership_source_99f717cb8849f380 FOREIGN KEY (source_functional_project) REFERENCES fabric.functional_project (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_defines_ownership
    ADD CONSTRAINT fk_functional_project_defines_ownership_target_2e9a576e6946f1eb FOREIGN KEY (target_ownership) REFERENCES fabric.ownership (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_defines_ownership
    ADD CONSTRAINT uq_functional_project_defines_ownership_target_43f32fb6eeb4ed99 UNIQUE (target_ownership) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project_reviews_review
    ADD CONSTRAINT fk_functional_project_reviews_review_source_fu_85ba2c06d155c663 FOREIGN KEY (source_functional_project) REFERENCES fabric.functional_project (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_reviews_review
    ADD CONSTRAINT fk_functional_project_reviews_review_target_re_cc0f782f02ca211e FOREIGN KEY (target_review) REFERENCES fabric.review (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_reviews_review
    ADD CONSTRAINT uq_functional_project_reviews_review_target_re_da1d08a899225c58 UNIQUE (target_review) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project_policy
    ADD CONSTRAINT fk_functional_project_policy_id_auditable_thin_f189954e3ec8edfe FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.message
    ADD CONSTRAINT fk_message_id_auditable_thing_2bd55495957f786c FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.message
    ADD CONSTRAINT fk_message_quotes_message_7d2044c5079685cc FOREIGN KEY (quotes) REFERENCES fabric.message (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_message_quotes_d767aaa5275900dc ON fabric.message (quotes);

ALTER TABLE fabric.organization
    ADD CONSTRAINT fk_organization_id_auditable_thing_0d90cd2cc694afc4 FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.organization
    ADD CONSTRAINT fk_organization_policy_organization_policy_205ac3eabe305c07 FOREIGN KEY (policy) REFERENCES fabric.organization_policy (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.organization
    ADD CONSTRAINT uq_organization_policy_b9b4fce7677e6c84 UNIQUE (policy) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.organization_member
    ADD CONSTRAINT fk_organization_member_id_auditable_thing_c306b474ff6144a4 FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.organization_member
    ADD CONSTRAINT fk_organization_member_organization_organizati_4cde24eeb121dc4f FOREIGN KEY (organization) REFERENCES fabric.organization (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_organization_member_organization_16c5bd788927b549 ON fabric.organization_member (organization);

ALTER TABLE fabric.organization_member
    ADD CONSTRAINT fk_organization_member_model_user_model_user_5c92915e42d06b71 FOREIGN KEY (model_user) REFERENCES fabric.model_user (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_organization_member_model_user_96daed42a0232088 ON fabric.organization_member (model_user);

ALTER TABLE fabric.organization_policy
    ADD CONSTRAINT fk_organization_policy_id_auditable_thing_947c7898a939fca4 FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.ownership
    ADD CONSTRAINT fk_ownership_id_auditable_thing_812730ab7d731cc1 FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.project_member
    ADD CONSTRAINT fk_project_member_id_auditable_thing_37495760ecf1b90e FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.project_member
    ADD CONSTRAINT fk_project_member_active_ownership_ownership_20a754fe05935501 FOREIGN KEY (active_ownership) REFERENCES fabric.ownership (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_project_member_active_ownership_729b4c638a122ab6 ON fabric.project_member (active_ownership);

ALTER TABLE fabric.project_member
    ADD CONSTRAINT fk_project_member_is_part_of_functional_projec_2f6496acc5186982 FOREIGN KEY (is_part_of) REFERENCES fabric.functional_project (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_project_member_is_part_of_3b6bc2b5d06b8164 ON fabric.project_member (is_part_of);

ALTER TABLE fabric.project_member
    ADD CONSTRAINT fk_project_member_model_user_model_user_8fdb93398c120c65 FOREIGN KEY (model_user) REFERENCES fabric.model_user (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_project_member_model_user_0e9cdbde4fcdd984 ON fabric.project_member (model_user);

ALTER TABLE fabric.project_member_owns_ownership
    ADD CONSTRAINT fk_project_member_owns_ownership_source_projec_13d3c8916265cc57 FOREIGN KEY (source_project_member) REFERENCES fabric.project_member (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.project_member_owns_ownership
    ADD CONSTRAINT fk_project_member_owns_ownership_target_owners_2781318ae10511ad FOREIGN KEY (target_ownership) REFERENCES fabric.ownership (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_project_member_owns_ownership_target_owners_ba02b03cf35b7ce1 ON fabric.project_member_owns_ownership (target_ownership);

ALTER TABLE fabric.reply
    ADD CONSTRAINT fk_reply_id_message_d770fac9efdda8cb FOREIGN KEY (id) REFERENCES fabric.message (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review
    ADD CONSTRAINT fk_review_id_auditable_thing_4cebf791cb87737f FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review_comments_comment
    ADD CONSTRAINT fk_review_comments_comment_source_review_revie_cd506c00523bdf2c FOREIGN KEY (source_review) REFERENCES fabric.review (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review_comments_comment
    ADD CONSTRAINT fk_review_comments_comment_target_comment_comm_1d46752f8eb1c0b0 FOREIGN KEY (target_comment) REFERENCES fabric.comment (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review_comments_comment
    ADD CONSTRAINT uq_review_comments_comment_target_comment_05d0faab1e6708a5 UNIQUE (target_comment) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.review_reviewers_user
    ADD CONSTRAINT fk_review_reviewers_user_source_review_review_44322f12bcf1f655 FOREIGN KEY (source_review) REFERENCES fabric.review (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review_reviewers_user
    ADD CONSTRAINT fk_review_reviewers_user_target_user_model_use_12f0caddc785154e FOREIGN KEY (target_user) REFERENCES fabric.model_user (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX ix_review_reviewers_user_target_user_c5abab6dc142cb75 ON fabric.review_reviewers_user (target_user);

ALTER TABLE fabric.model_user
    ADD CONSTRAINT fk_model_user_id_auditable_thing_8b096f2acff04a88 FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

-- Updating a non-key root column serializes writers for this object and forces
-- a snapshot-conflict failure at repeatable-read isolation instead of a stale check.
-- It preserves the payload value and avoids a database-wide graph lock.
CREATE FUNCTION fabric.lock_object(p_id uuid)
RETURNS void
LANGUAGE plpgsql
AS $mycelium$
BEGIN
    UPDATE fabric.thing SET data = data WHERE id = p_id;
END;
$mycelium$;

CREATE FUNCTION fabric.thing_delete()
RETURNS trigger
LANGUAGE plpgsql
AS $mycelium$
BEGIN
    -- A root already removed by the current cascade matches no row.
    DELETE FROM fabric.thing WHERE id = OLD.id;

    RETURN OLD;
END;
$mycelium$;

CREATE FUNCTION fabric.serialize_object()
RETURNS trigger
LANGUAGE plpgsql
AS $mycelium$
BEGIN
    IF TG_OP <> 'INSERT' THEN
        PERFORM fabric.lock_object(OLD.id);
    END IF;

    IF TG_OP = 'INSERT' OR (TG_OP = 'UPDATE' AND NEW.id IS DISTINCT FROM OLD.id) THEN
        PERFORM fabric.lock_object(NEW.id);
    END IF;

    RETURN NULL;
END;
$mycelium$;

-- All composition stores participate in the same ownership check. Multiple
-- occurrences for one owner remain legal where the model permits duplicates.
CREATE FUNCTION fabric.owners(p_part uuid)
RETURNS TABLE (owner_id uuid)
LANGUAGE sql
AS $mycelium$
    SELECT NULL::uuid WHERE false
    UNION ALL
    SELECT relationship.source_comment
    FROM fabric.comment_replies_reply AS relationship
    WHERE relationship.target_reply = p_part AND relationship.source_comment IS NOT NULL
    UNION ALL
    SELECT relationship.belongs_to
    FROM fabric.functional_project AS relationship
    WHERE relationship.id = p_part AND relationship.belongs_to IS NOT NULL
    UNION ALL
    SELECT relationship.source_functional_project
    FROM fabric.functional_project_branch_rules_branch_protection_rule AS relationship
    WHERE relationship.target_branch_protection_rule = p_part AND relationship.source_functional_project IS NOT NULL
    UNION ALL
    SELECT relationship.source_functional_project
    FROM fabric.functional_project_defines_ownership AS relationship
    WHERE relationship.target_ownership = p_part AND relationship.source_functional_project IS NOT NULL
    UNION ALL
    SELECT relationship.id
    FROM fabric.functional_project AS relationship
    WHERE relationship.policy = p_part AND relationship.id IS NOT NULL
    UNION ALL
    SELECT relationship.source_functional_project
    FROM fabric.functional_project_reviews_review AS relationship
    WHERE relationship.target_review = p_part AND relationship.source_functional_project IS NOT NULL
    UNION ALL
    SELECT relationship.id
    FROM fabric.organization AS relationship
    WHERE relationship.policy = p_part AND relationship.id IS NOT NULL
    UNION ALL
    SELECT relationship.is_part_of
    FROM fabric.project_member AS relationship
    WHERE relationship.id = p_part AND relationship.is_part_of IS NOT NULL
    UNION ALL
    SELECT relationship.source_review
    FROM fabric.review_comments_comment AS relationship
    WHERE relationship.target_comment = p_part AND relationship.source_review IS NOT NULL
$mycelium$;

-- Capture detached identities immediately; delete them only after reparenting
-- can finish. The private queue is transaction-local, including savepoint rollback.
CREATE FUNCTION fabric.track_ownership()
RETURNS trigger
LANGUAGE plpgsql
AS $mycelium$
DECLARE
    old_part uuid;
    new_part uuid;
    old_owner uuid;
    new_owner uuid;
    part_id uuid;
    pending jsonb;
BEGIN
    IF TG_OP <> 'INSERT' THEN
        old_part := (to_jsonb(OLD) ->> TG_ARGV[0])::uuid;
        old_owner := (to_jsonb(OLD) ->> TG_ARGV[1])::uuid;
    END IF;

    IF TG_OP <> 'DELETE' THEN
        new_part := (to_jsonb(NEW) ->> TG_ARGV[0])::uuid;
        new_owner := (to_jsonb(NEW) ->> TG_ARGV[1])::uuid;
    END IF;

    IF TG_OP = 'UPDATE' AND old_part IS NOT DISTINCT FROM new_part AND old_owner IS NOT DISTINCT FROM new_owner THEN
        RETURN NULL;
    END IF;

    FOR part_id IN
        SELECT DISTINCT candidate
        FROM unnest(ARRAY[old_part, new_part]) AS affected(candidate)
        WHERE candidate IS NOT NULL
        ORDER BY candidate
    LOOP
        PERFORM fabric.lock_object(part_id);
    END LOOP;

    IF old_part IS NOT NULL AND old_owner IS NOT NULL THEN
        pending := COALESCE(NULLIF(current_setting('mycelium.fabric_detached_parts', true), ''), '[]')::jsonb;
        PERFORM set_config('mycelium.fabric_detached_parts', (pending || jsonb_build_array(old_part))::text, true);
    END IF;

    RETURN NULL;
END;
$mycelium$;

CREATE FUNCTION fabric.remove_orphans()
RETURNS void
LANGUAGE plpgsql
AS $mycelium$
DECLARE
    pending jsonb;
    part_id uuid;
BEGIN
    LOOP
        pending := COALESCE(NULLIF(current_setting('mycelium.fabric_detached_parts', true), ''), '[]')::jsonb;
        EXIT WHEN pending = '[]'::jsonb;
        PERFORM set_config('mycelium.fabric_detached_parts', '[]', true);

        FOR part_id IN
            SELECT DISTINCT value::uuid FROM jsonb_array_elements_text(pending) ORDER BY 1
        LOOP
            PERFORM fabric.lock_object(part_id);

            IF NOT EXISTS (SELECT 1 FROM fabric.owners(part_id)) THEN
                DELETE FROM fabric.thing WHERE id = part_id;
            END IF;
        END LOOP;
        -- Cascaded deletion may have queued further contained objects.
    END LOOP;
END;
$mycelium$;

CREATE FUNCTION fabric.check_ownership(p_part uuid)
RETURNS void
LANGUAGE plpgsql
AS $mycelium$
BEGIN
    IF p_part IS NULL THEN
        RETURN;
    END IF;

    -- Reacquire protection when references were inserted before their deferred root.
    PERFORM fabric.lock_object(p_part);

    IF (SELECT count(DISTINCT owner_id) FROM fabric.owners(p_part)) > 1 THEN
        RAISE EXCEPTION 'Part % has multiple composite owners', p_part USING ERRCODE = '23505';
    END IF;
END;
$mycelium$;

CREATE FUNCTION fabric.validate_ownership()
RETURNS trigger
LANGUAGE plpgsql
AS $mycelium$
DECLARE
    old_part uuid;
    new_part uuid;
    old_owner uuid;
    new_owner uuid;
BEGIN
    IF TG_OP <> 'INSERT' THEN
        old_part := (to_jsonb(OLD) ->> TG_ARGV[0])::uuid;
        old_owner := (to_jsonb(OLD) ->> TG_ARGV[1])::uuid;
    END IF;

    IF TG_OP <> 'DELETE' THEN
        new_part := (to_jsonb(NEW) ->> TG_ARGV[0])::uuid;
        new_owner := (to_jsonb(NEW) ->> TG_ARGV[1])::uuid;
    END IF;

    IF TG_OP = 'UPDATE' AND old_part IS NOT DISTINCT FROM new_part AND old_owner IS NOT DISTINCT FROM new_owner THEN
        RETURN NULL;
    END IF;

    PERFORM fabric.remove_orphans();
    PERFORM fabric.check_ownership(old_part);

    IF new_part IS DISTINCT FROM old_part THEN
        PERFORM fabric.check_ownership(new_part);
    END IF;

    RETURN NULL;
END;
$mycelium$;

CREATE TRIGGER guard_comment_replies_reply_source_comment_tar_be6de4d8eb8987c0
    AFTER INSERT OR UPDATE OR DELETE ON fabric.comment_replies_reply
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_reply', 'source_comment');

CREATE CONSTRAINT TRIGGER ownership_comment_replies_reply_source_comment_bc2c64a9dfbca732
    AFTER INSERT OR UPDATE OR DELETE ON fabric.comment_replies_reply
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_reply', 'source_comment');

CREATE TRIGGER guard_functional_project_belongs_to_id_eaid_ds_0193c30ef0610a35
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('id', 'belongs_to');

CREATE CONSTRAINT TRIGGER ownership_functional_project_belongs_to_id_eai_c12f9a53079a328a
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('id', 'belongs_to');

CREATE TRIGGER guard_functional_project_branch_rules_branch_p_c505788b8488fc1f
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_branch_rules_branch_protection_rule
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_branch_protection_rule', 'source_functional_project');

CREATE CONSTRAINT TRIGGER ownership_functional_project_branch_rules_bran_c1d2f61b8d0a483d
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_branch_rules_branch_protection_rule
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_branch_protection_rule', 'source_functional_project');

CREATE TRIGGER guard_functional_project_defines_ownership_sou_dc4c38d3026ccaa6
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_defines_ownership
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_ownership', 'source_functional_project');

CREATE CONSTRAINT TRIGGER ownership_functional_project_defines_ownership_b0024635c6fbe7f2
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_defines_ownership
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_ownership', 'source_functional_project');

CREATE TRIGGER guard_functional_project_id_policy_eaid_src334_b841731cc2e7b907
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('policy', 'id');

CREATE CONSTRAINT TRIGGER ownership_functional_project_id_policy_eaid_sr_2a0ab346af1f6e0d
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('policy', 'id');

CREATE TRIGGER guard_functional_project_reviews_review_source_49b3d22307c1392c
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_reviews_review
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_review', 'source_functional_project');

CREATE CONSTRAINT TRIGGER ownership_functional_project_reviews_review_so_7f970c2dda60210f
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_reviews_review
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_review', 'source_functional_project');

CREATE TRIGGER guard_organization_id_policy_eaid_src55_fb64_e_f46be3af09d90ce0
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('policy', 'id');

CREATE CONSTRAINT TRIGGER ownership_organization_id_policy_eaid_src55_fb_db13af43380301b8
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('policy', 'id');

CREATE TRIGGER guard_project_member_is_part_of_id_eaid_dst128_7f96b59b4b4df852
    AFTER INSERT OR UPDATE OR DELETE ON fabric.project_member
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('id', 'is_part_of');

CREATE CONSTRAINT TRIGGER ownership_project_member_is_part_of_id_eaid_ds_d4211e11e6f524a5
    AFTER INSERT OR UPDATE OR DELETE ON fabric.project_member
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('id', 'is_part_of');

CREATE TRIGGER guard_review_comments_comment_source_review_ta_5eed82b8408bbd1f
    AFTER INSERT OR UPDATE OR DELETE ON fabric.review_comments_comment
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_comment', 'source_review');

CREATE CONSTRAINT TRIGGER ownership_review_comments_comment_source_revie_6755434285537ca2
    AFTER INSERT OR UPDATE OR DELETE ON fabric.review_comments_comment
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_comment', 'source_review');

CREATE FUNCTION fabric.validate_object(p_id uuid)
RETURNS void
LANGUAGE plpgsql
AS $mycelium$
DECLARE
    kind text;
BEGIN
    -- Process all recorded removals first, regardless of deferred-event ordering.
    PERFORM fabric.remove_orphans();
    SELECT class_kind INTO kind FROM fabric.thing WHERE id = p_id;

    IF NOT FOUND THEN
        RETURN;
    END IF;

    IF ((kind IN ('BranchProtectionRule', 'Comment', 'FunctionalProject', 'FunctionalProjectPolicy', 'Organization', 'OrganizationMember', 'OrganizationPolicy', 'Ownership', 'ProjectMember', 'Reply', 'Review', 'User')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.auditable_thing WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('BranchProtectionRule')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.branch_protection_rule WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('Comment')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.comment WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('FunctionalProject')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.functional_project WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('FunctionalProjectPolicy')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.functional_project_policy WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('Comment', 'Reply')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.message WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('Organization')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.organization WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('OrganizationMember')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.organization_member WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('OrganizationPolicy')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.organization_policy WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('Ownership')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.ownership WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('ProjectMember')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.project_member WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('Reply')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.reply WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('Review')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.review WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF ((kind IN ('User')) IS TRUE)
        IS DISTINCT FROM (EXISTS (SELECT 1 FROM fabric.model_user WHERE id = p_id)) THEN
        RAISE EXCEPTION 'Inheritance rows do not match type % for %', kind, p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('Reply')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.comment_replies_reply AS relationship
            WHERE relationship.target_reply = p_id AND relationship.source_comment IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('FunctionalProject')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.functional_project AS relationship
            WHERE relationship.id = p_id AND relationship.belongs_to IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('BranchProtectionRule')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.functional_project_branch_rules_branch_protection_rule AS relationship
            WHERE relationship.target_branch_protection_rule = p_id AND relationship.source_functional_project IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('Ownership')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.functional_project_defines_ownership AS relationship
            WHERE relationship.target_ownership = p_id AND relationship.source_functional_project IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('FunctionalProjectPolicy')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.functional_project AS relationship
            WHERE relationship.policy = p_id AND relationship.id IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('Review')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.functional_project_reviews_review AS relationship
            WHERE relationship.target_review = p_id AND relationship.source_functional_project IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('OrganizationPolicy')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.organization AS relationship
            WHERE relationship.policy = p_id AND relationship.id IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('ProjectMember')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.project_member AS relationship
            WHERE relationship.id = p_id AND relationship.is_part_of IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('Comment')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.review_comments_comment AS relationship
            WHERE relationship.target_comment = p_id AND relationship.source_review IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

END;
$mycelium$;

CREATE FUNCTION fabric.validate_inheritance()
RETURNS trigger
LANGUAGE plpgsql
AS $mycelium$
BEGIN
    IF TG_OP <> 'INSERT' THEN
        PERFORM fabric.validate_object(OLD.id);
    END IF;

    IF TG_OP = 'INSERT' OR (TG_OP = 'UPDATE' AND NEW.id IS DISTINCT FROM OLD.id) THEN
        PERFORM fabric.validate_object(NEW.id);
    END IF;

    RETURN NULL;
END;
$mycelium$;

CREATE TRIGGER trg_auditable_thing_delete_root_1783ec8ffbe01551
    AFTER DELETE ON fabric.auditable_thing
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_auditable_thing_serialize_object_f961699f9f32eb8d
    AFTER INSERT OR UPDATE OR DELETE ON fabric.auditable_thing
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_auditable_thing_validate_inheritance_c36fa3a3d886a449
    AFTER INSERT OR UPDATE OR DELETE ON fabric.auditable_thing
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_branch_protection_rule_delete_root_8c0680d706cae5ba
    AFTER DELETE ON fabric.branch_protection_rule
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_branch_protection_rule_serialize_object_638bb70dac30cf0a
    AFTER INSERT OR UPDATE OR DELETE ON fabric.branch_protection_rule
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_branch_protection_rule_validate_inheritanc_ce97a50dbeb2f018
    AFTER INSERT OR UPDATE OR DELETE ON fabric.branch_protection_rule
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_comment_delete_root_e80394fbfb3ae14c
    AFTER DELETE ON fabric.comment
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_comment_serialize_object_9968bf7110aa188d
    AFTER INSERT OR UPDATE OR DELETE ON fabric.comment
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_comment_validate_inheritance_f932950c963a4468
    AFTER INSERT OR UPDATE OR DELETE ON fabric.comment
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_functional_project_delete_root_29320a60165d19f6
    AFTER DELETE ON fabric.functional_project
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_functional_project_serialize_object_deb4f6354fd01568
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_functional_project_validate_inheritance_571b0ac6fe2c2ee7
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_functional_project_policy_delete_root_de4ec0a7937c1ba4
    AFTER DELETE ON fabric.functional_project_policy
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_functional_project_policy_serialize_object_8fc3137657b2b987
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_policy
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_functional_project_policy_validate_inherit_d7be4fe2cf0c5a8e
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_policy
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_message_delete_root_4c157095e4d3fb4a
    AFTER DELETE ON fabric.message
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_message_serialize_object_db0eed979eab9caf
    AFTER INSERT OR UPDATE OR DELETE ON fabric.message
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_message_validate_inheritance_202709dc217a3fe7
    AFTER INSERT OR UPDATE OR DELETE ON fabric.message
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_organization_delete_root_01ec6d09bcc20638
    AFTER DELETE ON fabric.organization
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_organization_serialize_object_142237bba5fd29dd
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_organization_validate_inheritance_b4b30dbb32362cdc
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_organization_member_delete_root_7db397c27e1b4425
    AFTER DELETE ON fabric.organization_member
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_organization_member_serialize_object_cc97f91e3463e335
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization_member
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_organization_member_validate_inheritance_3a28339dd1a0912f
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization_member
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_organization_policy_delete_root_85c13ddf3a02333a
    AFTER DELETE ON fabric.organization_policy
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_organization_policy_serialize_object_38bd1ca68d7639b3
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization_policy
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_organization_policy_validate_inheritance_2e5ea0f57a8eb5c0
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization_policy
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_ownership_delete_root_5b9c2ca42bd07d62
    AFTER DELETE ON fabric.ownership
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_ownership_serialize_object_f83f6e5b155b2b32
    AFTER INSERT OR UPDATE OR DELETE ON fabric.ownership
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_ownership_validate_inheritance_a57f26b10622e8ee
    AFTER INSERT OR UPDATE OR DELETE ON fabric.ownership
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_project_member_delete_root_e5add233212bf381
    AFTER DELETE ON fabric.project_member
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_project_member_serialize_object_54fd84e2cc795a20
    AFTER INSERT OR UPDATE OR DELETE ON fabric.project_member
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_project_member_validate_inheritance_a8fe6effccfee550
    AFTER INSERT OR UPDATE OR DELETE ON fabric.project_member
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_reply_delete_root_f43adc3d2ebd4254
    AFTER DELETE ON fabric.reply
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_reply_serialize_object_a6ec5af1bc8310bb
    AFTER INSERT OR UPDATE OR DELETE ON fabric.reply
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_reply_validate_inheritance_c169f2cbfcf3963d
    AFTER INSERT OR UPDATE OR DELETE ON fabric.reply
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_review_delete_root_3d99ff7b5597e214
    AFTER DELETE ON fabric.review
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_review_serialize_object_6553520f65c4992d
    AFTER INSERT OR UPDATE OR DELETE ON fabric.review
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_review_validate_inheritance_41358cc7f80a4239
    AFTER INSERT OR UPDATE OR DELETE ON fabric.review
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE CONSTRAINT TRIGGER trg_thing_validate_inheritance_44b900cbf1426f05
    AFTER INSERT OR UPDATE OR DELETE ON fabric.thing
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER trg_model_user_delete_root_161f79f2bbe3f2f9
    AFTER DELETE ON fabric.model_user
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER trg_model_user_serialize_object_4927eeb83083bd59
    AFTER INSERT OR UPDATE OR DELETE ON fabric.model_user
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER trg_model_user_validate_inheritance_c4e31c7bb952d83c
    AFTER INSERT OR UPDATE OR DELETE ON fabric.model_user
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();
