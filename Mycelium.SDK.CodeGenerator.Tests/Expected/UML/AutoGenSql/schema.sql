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
    CONSTRAINT auditable_thing_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.branch_protection_rule (
    id uuid NOT NULL,
    CONSTRAINT branch_protection_rule_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.branch_protection_rule_default_reviewers (
    source_id uuid NOT NULL,
    target_id uuid NOT NULL,
    CONSTRAINT branch_protection_rule_default_reviewers_pkey PRIMARY KEY (source_id, target_id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.comment (
    id uuid NOT NULL,
    CONSTRAINT comment_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.comment_replies (
    source_id uuid NOT NULL,
    target_id uuid NOT NULL,
    CONSTRAINT comment_replies_pkey PRIMARY KEY (source_id, target_id)
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
    CONSTRAINT functional_project_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.functional_project_branch_rules (
    source_id uuid NOT NULL,
    target_id uuid NOT NULL,
    CONSTRAINT functional_project_branch_rules_pkey PRIMARY KEY (source_id, target_id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.functional_project_defines (
    source_id uuid NOT NULL,
    target_id uuid NOT NULL,
    CONSTRAINT functional_project_defines_pkey PRIMARY KEY (source_id, target_id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.functional_project_reviews (
    source_id uuid NOT NULL,
    target_id uuid NOT NULL,
    CONSTRAINT functional_project_reviews_pkey PRIMARY KEY (source_id, target_id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.functional_project_policy (
    id uuid NOT NULL,
    CONSTRAINT functional_project_policy_pkey PRIMARY KEY (id)
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
    CONSTRAINT message_pkey PRIMARY KEY (id)
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
    CONSTRAINT organization_pkey PRIMARY KEY (id)
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
    CONSTRAINT organization_member_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.organization_policy (
    id uuid NOT NULL,
    CONSTRAINT organization_policy_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.ownership (
    id uuid NOT NULL,
    CONSTRAINT ownership_pkey PRIMARY KEY (id)
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
    CONSTRAINT project_member_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.project_member_owns (
    source_id uuid NOT NULL,
    target_id uuid NOT NULL,
    CONSTRAINT project_member_owns_pkey PRIMARY KEY (source_id, target_id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.reply (
    id uuid NOT NULL,
    CONSTRAINT reply_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.review (
    id uuid NOT NULL,
    CONSTRAINT review_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.review_comments (
    source_id uuid NOT NULL,
    target_id uuid NOT NULL,
    CONSTRAINT review_comments_pkey PRIMARY KEY (source_id, target_id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.review_reviewers (
    source_id uuid NOT NULL,
    target_id uuid NOT NULL,
    CONSTRAINT review_reviewers_pkey PRIMARY KEY (source_id, target_id)
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
    CONSTRAINT thing_class_kind_check CHECK (class_kind IN ('BranchProtectionRule', 'Comment', 'FunctionalProject', 'FunctionalProjectPolicy', 'Organization', 'OrganizationMember', 'OrganizationPolicy', 'Ownership', 'ProjectMember', 'Reply', 'Review', 'User')),
    CONSTRAINT thing_data_check CHECK ((
        jsonb_typeof(data) = 'object'
        AND jsonb_typeof(data -> '@id') = 'string'
        AND jsonb_typeof(data -> '@type') = 'string'
        AND (data ->> '@id')::uuid = id
        AND data ->> '@type' = class_kind
    ) IS TRUE),
    CONSTRAINT thing_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

CREATE TABLE fabric.model_user (
    id uuid NOT NULL,
    CONSTRAINT model_user_pkey PRIMARY KEY (id)
)
WITH (
    autovacuum_vacuum_scale_factor = 0.0,
    autovacuum_vacuum_threshold = 2500,
    autovacuum_analyze_scale_factor = 0.0,
    autovacuum_analyze_threshold = 2500
);

ALTER TABLE fabric.auditable_thing
    ADD CONSTRAINT auditable_thing_id_fkey FOREIGN KEY (id) REFERENCES fabric.thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.auditable_thing
    ADD CONSTRAINT auditable_thing_created_by_fkey FOREIGN KEY (created_by) REFERENCES fabric.model_user (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX auditable_thing_created_by_idx ON fabric.auditable_thing (created_by);

ALTER TABLE fabric.auditable_thing
    ADD CONSTRAINT auditable_thing_updated_by_fkey FOREIGN KEY (updated_by) REFERENCES fabric.model_user (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX auditable_thing_updated_by_idx ON fabric.auditable_thing (updated_by);

ALTER TABLE fabric.branch_protection_rule
    ADD CONSTRAINT branch_protection_rule_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.branch_protection_rule_default_reviewers
    ADD CONSTRAINT branch_protection_rule_default_reviewers_source_id_fkey FOREIGN KEY (source_id) REFERENCES fabric.branch_protection_rule (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.branch_protection_rule_default_reviewers
    ADD CONSTRAINT branch_protection_rule_default_reviewers_target_id_fkey FOREIGN KEY (target_id) REFERENCES fabric.model_user (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX branch_protection_rule_default_reviewers_target_id_idx ON fabric.branch_protection_rule_default_reviewers (target_id);

ALTER TABLE fabric.comment
    ADD CONSTRAINT comment_id_fkey FOREIGN KEY (id) REFERENCES fabric.message (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.comment_replies
    ADD CONSTRAINT comment_replies_source_id_fkey FOREIGN KEY (source_id) REFERENCES fabric.comment (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.comment_replies
    ADD CONSTRAINT comment_replies_target_id_fkey FOREIGN KEY (target_id) REFERENCES fabric.reply (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.comment_replies
    ADD CONSTRAINT comment_replies_target_id_key UNIQUE (target_id) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project
    ADD CONSTRAINT functional_project_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project
    ADD CONSTRAINT functional_project_belongs_to_fkey FOREIGN KEY (belongs_to) REFERENCES fabric.organization (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX functional_project_belongs_to_idx ON fabric.functional_project (belongs_to);

ALTER TABLE fabric.functional_project
    ADD CONSTRAINT functional_project_policy_fkey FOREIGN KEY (policy) REFERENCES fabric.functional_project_policy (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project
    ADD CONSTRAINT functional_project_policy_key UNIQUE (policy) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project_branch_rules
    ADD CONSTRAINT functional_project_branch_rules_source_id_fkey FOREIGN KEY (source_id) REFERENCES fabric.functional_project (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_branch_rules
    ADD CONSTRAINT functional_project_branch_rules_target_id_fkey FOREIGN KEY (target_id) REFERENCES fabric.branch_protection_rule (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_branch_rules
    ADD CONSTRAINT functional_project_branch_rules_target_id_key UNIQUE (target_id) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project_defines
    ADD CONSTRAINT functional_project_defines_source_id_fkey FOREIGN KEY (source_id) REFERENCES fabric.functional_project (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_defines
    ADD CONSTRAINT functional_project_defines_target_id_fkey FOREIGN KEY (target_id) REFERENCES fabric.ownership (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_defines
    ADD CONSTRAINT functional_project_defines_target_id_key UNIQUE (target_id) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project_reviews
    ADD CONSTRAINT functional_project_reviews_source_id_fkey FOREIGN KEY (source_id) REFERENCES fabric.functional_project (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_reviews
    ADD CONSTRAINT functional_project_reviews_target_id_fkey FOREIGN KEY (target_id) REFERENCES fabric.review (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.functional_project_reviews
    ADD CONSTRAINT functional_project_reviews_target_id_key UNIQUE (target_id) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.functional_project_policy
    ADD CONSTRAINT functional_project_policy_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.message
    ADD CONSTRAINT message_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.message
    ADD CONSTRAINT message_quotes_fkey FOREIGN KEY (quotes) REFERENCES fabric.message (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX message_quotes_idx ON fabric.message (quotes);

ALTER TABLE fabric.organization
    ADD CONSTRAINT organization_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.organization
    ADD CONSTRAINT organization_policy_fkey FOREIGN KEY (policy) REFERENCES fabric.organization_policy (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.organization
    ADD CONSTRAINT organization_policy_key UNIQUE (policy) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.organization_member
    ADD CONSTRAINT organization_member_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.organization_member
    ADD CONSTRAINT organization_member_organization_fkey FOREIGN KEY (organization) REFERENCES fabric.organization (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX organization_member_organization_idx ON fabric.organization_member (organization);

ALTER TABLE fabric.organization_member
    ADD CONSTRAINT organization_member_model_user_fkey FOREIGN KEY (model_user) REFERENCES fabric.model_user (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX organization_member_model_user_idx ON fabric.organization_member (model_user);

ALTER TABLE fabric.organization_policy
    ADD CONSTRAINT organization_policy_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.ownership
    ADD CONSTRAINT ownership_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.project_member
    ADD CONSTRAINT project_member_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.project_member
    ADD CONSTRAINT project_member_active_ownership_fkey FOREIGN KEY (active_ownership) REFERENCES fabric.ownership (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX project_member_active_ownership_idx ON fabric.project_member (active_ownership);

ALTER TABLE fabric.project_member
    ADD CONSTRAINT project_member_is_part_of_fkey FOREIGN KEY (is_part_of) REFERENCES fabric.functional_project (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX project_member_is_part_of_idx ON fabric.project_member (is_part_of);

ALTER TABLE fabric.project_member
    ADD CONSTRAINT project_member_model_user_fkey FOREIGN KEY (model_user) REFERENCES fabric.model_user (id)
    ON DELETE NO ACTION DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX project_member_model_user_idx ON fabric.project_member (model_user);

ALTER TABLE fabric.project_member_owns
    ADD CONSTRAINT project_member_owns_source_id_fkey FOREIGN KEY (source_id) REFERENCES fabric.project_member (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.project_member_owns
    ADD CONSTRAINT project_member_owns_target_id_fkey FOREIGN KEY (target_id) REFERENCES fabric.ownership (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX project_member_owns_target_id_idx ON fabric.project_member_owns (target_id);

ALTER TABLE fabric.reply
    ADD CONSTRAINT reply_id_fkey FOREIGN KEY (id) REFERENCES fabric.message (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review
    ADD CONSTRAINT review_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review_comments
    ADD CONSTRAINT review_comments_source_id_fkey FOREIGN KEY (source_id) REFERENCES fabric.review (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review_comments
    ADD CONSTRAINT review_comments_target_id_fkey FOREIGN KEY (target_id) REFERENCES fabric.comment (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review_comments
    ADD CONSTRAINT review_comments_target_id_key UNIQUE (target_id) DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE fabric.review_reviewers
    ADD CONSTRAINT review_reviewers_source_id_fkey FOREIGN KEY (source_id) REFERENCES fabric.review (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

ALTER TABLE fabric.review_reviewers
    ADD CONSTRAINT review_reviewers_target_id_fkey FOREIGN KEY (target_id) REFERENCES fabric.model_user (id)
    ON DELETE CASCADE DEFERRABLE INITIALLY IMMEDIATE;

CREATE INDEX review_reviewers_target_id_idx ON fabric.review_reviewers (target_id);

ALTER TABLE fabric.model_user
    ADD CONSTRAINT model_user_id_fkey FOREIGN KEY (id) REFERENCES fabric.auditable_thing (id)
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
    SELECT relationship.source_id
    FROM fabric.comment_replies AS relationship
    WHERE relationship.target_id = p_part AND relationship.source_id IS NOT NULL
    UNION ALL
    SELECT relationship.belongs_to
    FROM fabric.functional_project AS relationship
    WHERE relationship.id = p_part AND relationship.belongs_to IS NOT NULL
    UNION ALL
    SELECT relationship.source_id
    FROM fabric.functional_project_branch_rules AS relationship
    WHERE relationship.target_id = p_part AND relationship.source_id IS NOT NULL
    UNION ALL
    SELECT relationship.source_id
    FROM fabric.functional_project_defines AS relationship
    WHERE relationship.target_id = p_part AND relationship.source_id IS NOT NULL
    UNION ALL
    SELECT relationship.id
    FROM fabric.functional_project AS relationship
    WHERE relationship.policy = p_part AND relationship.id IS NOT NULL
    UNION ALL
    SELECT relationship.source_id
    FROM fabric.functional_project_reviews AS relationship
    WHERE relationship.target_id = p_part AND relationship.source_id IS NOT NULL
    UNION ALL
    SELECT relationship.id
    FROM fabric.organization AS relationship
    WHERE relationship.policy = p_part AND relationship.id IS NOT NULL
    UNION ALL
    SELECT relationship.is_part_of
    FROM fabric.project_member AS relationship
    WHERE relationship.id = p_part AND relationship.is_part_of IS NOT NULL
    UNION ALL
    SELECT relationship.source_id
    FROM fabric.review_comments AS relationship
    WHERE relationship.target_id = p_part AND relationship.source_id IS NOT NULL
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

CREATE TRIGGER t10_track_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.comment_replies
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_id', 'source_id');

CREATE CONSTRAINT TRIGGER t80_validate_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.comment_replies
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_id', 'source_id');

CREATE TRIGGER t10_track_ownership_belongs_to
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('id', 'belongs_to');

CREATE CONSTRAINT TRIGGER t80_validate_ownership_belongs_to
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('id', 'belongs_to');

CREATE TRIGGER t10_track_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_branch_rules
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_id', 'source_id');

CREATE CONSTRAINT TRIGGER t80_validate_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_branch_rules
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_id', 'source_id');

CREATE TRIGGER t10_track_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_defines
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_id', 'source_id');

CREATE CONSTRAINT TRIGGER t80_validate_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_defines
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_id', 'source_id');

CREATE TRIGGER t10_track_ownership_policy
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('policy', 'id');

CREATE CONSTRAINT TRIGGER t80_validate_ownership_policy
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('policy', 'id');

CREATE TRIGGER t10_track_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_reviews
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_id', 'source_id');

CREATE CONSTRAINT TRIGGER t80_validate_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_reviews
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_id', 'source_id');

CREATE TRIGGER t10_track_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('policy', 'id');

CREATE CONSTRAINT TRIGGER t80_validate_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('policy', 'id');

CREATE TRIGGER t10_track_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.project_member
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('id', 'is_part_of');

CREATE CONSTRAINT TRIGGER t80_validate_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.project_member
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('id', 'is_part_of');

CREATE TRIGGER t10_track_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.review_comments
    FOR EACH ROW EXECUTE FUNCTION fabric.track_ownership('target_id', 'source_id');

CREATE CONSTRAINT TRIGGER t80_validate_ownership
    AFTER INSERT OR UPDATE OR DELETE ON fabric.review_comments
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_ownership('target_id', 'source_id');

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
            SELECT 1 FROM fabric.comment_replies AS relationship
            WHERE relationship.target_id = p_id AND relationship.source_id IS NOT NULL
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
            SELECT 1 FROM fabric.functional_project_branch_rules AS relationship
            WHERE relationship.target_id = p_id AND relationship.source_id IS NOT NULL
        ) THEN
        RAISE EXCEPTION 'Part % is missing a mandatory composite owner', p_id USING ERRCODE = '23514';
    END IF;

    IF (kind IN ('Ownership')) IS TRUE
        AND NOT EXISTS (
            SELECT 1 FROM fabric.functional_project_defines AS relationship
            WHERE relationship.target_id = p_id AND relationship.source_id IS NOT NULL
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
            SELECT 1 FROM fabric.functional_project_reviews AS relationship
            WHERE relationship.target_id = p_id AND relationship.source_id IS NOT NULL
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
            SELECT 1 FROM fabric.review_comments AS relationship
            WHERE relationship.target_id = p_id AND relationship.source_id IS NOT NULL
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

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.auditable_thing
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.auditable_thing
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.auditable_thing
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.branch_protection_rule
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.branch_protection_rule
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.branch_protection_rule
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.comment
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.comment
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.comment
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.functional_project
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.functional_project_policy
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_policy
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.functional_project_policy
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.message
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.message
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.message
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.organization
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.organization_member
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization_member
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization_member
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.organization_policy
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization_policy
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.organization_policy
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.ownership
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.ownership
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.ownership
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.project_member
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.project_member
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.project_member
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.reply
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.reply
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.reply
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.review
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.review
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.review
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.thing
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();

CREATE TRIGGER t20_delete_root
    AFTER DELETE ON fabric.model_user
    FOR EACH ROW EXECUTE FUNCTION fabric.thing_delete();

CREATE TRIGGER t30_serialize_object
    AFTER INSERT OR UPDATE OR DELETE ON fabric.model_user
    FOR EACH ROW EXECUTE FUNCTION fabric.serialize_object();

CREATE CONSTRAINT TRIGGER t90_validate_inheritance
    AFTER INSERT OR UPDATE OR DELETE ON fabric.model_user
    DEFERRABLE INITIALLY DEFERRED
    FOR EACH ROW EXECUTE FUNCTION fabric.validate_inheritance();
