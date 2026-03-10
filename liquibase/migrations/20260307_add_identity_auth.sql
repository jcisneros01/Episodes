ALTER TABLE episodes.users
    ADD COLUMN user_name VARCHAR(254),
    ADD COLUMN normalized_user_name VARCHAR(254),
    ADD COLUMN normalized_email VARCHAR(254),
    ADD COLUMN email_confirmed BOOLEAN NOT NULL DEFAULT FALSE,
    ADD COLUMN security_stamp VARCHAR(255),
    ADD COLUMN concurrency_stamp VARCHAR(255),
    ADD COLUMN phone_number VARCHAR(32),
    ADD COLUMN phone_number_confirmed BOOLEAN NOT NULL DEFAULT FALSE,
    ADD COLUMN two_factor_enabled BOOLEAN NOT NULL DEFAULT FALSE,
    ADD COLUMN lockout_end TIMESTAMPTZ NULL,
    ADD COLUMN lockout_enabled BOOLEAN NOT NULL DEFAULT FALSE,
    ADD COLUMN access_failed_count INT NOT NULL DEFAULT 0;

UPDATE episodes.users
SET user_name = email,
    normalized_user_name = UPPER(email),
    normalized_email = UPPER(email),
    security_stamp = md5(id::text || clock_timestamp()::text),
    concurrency_stamp = md5(random()::text || clock_timestamp()::text)
WHERE user_name IS NULL;

ALTER TABLE episodes.users
    ALTER COLUMN user_name SET NOT NULL,
    ALTER COLUMN normalized_user_name SET NOT NULL,
    ALTER COLUMN normalized_email SET NOT NULL,
    ALTER COLUMN security_stamp SET NOT NULL,
    ALTER COLUMN concurrency_stamp SET NOT NULL;

DROP INDEX IF EXISTS episodes.users_email_lower_idx;

CREATE UNIQUE INDEX users_normalized_user_name_key
    ON episodes.users (normalized_user_name);

CREATE UNIQUE INDEX users_normalized_email_key
    ON episodes.users (normalized_email);

CREATE TABLE episodes.roles (
    id                SERIAL PRIMARY KEY,
    name              VARCHAR(256) NULL,
    normalized_name   VARCHAR(256) NULL,
    concurrency_stamp VARCHAR(255) NULL
);

CREATE UNIQUE INDEX roles_normalized_name_key
    ON episodes.roles (normalized_name);

CREATE TABLE episodes.role_claims (
    id          SERIAL PRIMARY KEY,
    role_id     INT NOT NULL,
    claim_type  TEXT NULL,
    claim_value TEXT NULL,

    CONSTRAINT role_claims_role_id_fkey
        FOREIGN KEY (role_id)
        REFERENCES episodes.roles(id)
        ON DELETE CASCADE
);

CREATE INDEX role_claims_role_id_idx
    ON episodes.role_claims (role_id);

CREATE TABLE episodes.user_claims (
    id          SERIAL PRIMARY KEY,
    user_id     INT NOT NULL,
    claim_type  TEXT NULL,
    claim_value TEXT NULL,

    CONSTRAINT user_claims_user_id_fkey
        FOREIGN KEY (user_id)
        REFERENCES episodes.users(id)
        ON DELETE CASCADE
);

CREATE INDEX user_claims_user_id_idx
    ON episodes.user_claims (user_id);

CREATE TABLE episodes.user_logins (
    login_provider       VARCHAR(128) NOT NULL,
    provider_key         VARCHAR(128) NOT NULL,
    provider_display_name TEXT NULL,
    user_id              INT NOT NULL,

    PRIMARY KEY (login_provider, provider_key),

    CONSTRAINT user_logins_user_id_fkey
        FOREIGN KEY (user_id)
        REFERENCES episodes.users(id)
        ON DELETE CASCADE
);

CREATE INDEX user_logins_user_id_idx
    ON episodes.user_logins (user_id);

CREATE TABLE episodes.user_roles (
    user_id INT NOT NULL,
    role_id INT NOT NULL,

    PRIMARY KEY (user_id, role_id),

    CONSTRAINT user_roles_user_id_fkey
        FOREIGN KEY (user_id)
        REFERENCES episodes.users(id)
        ON DELETE CASCADE,

    CONSTRAINT user_roles_role_id_fkey
        FOREIGN KEY (role_id)
        REFERENCES episodes.roles(id)
        ON DELETE CASCADE
);

CREATE INDEX user_roles_role_id_idx
    ON episodes.user_roles (role_id);

CREATE TABLE episodes.user_tokens (
    user_id        INT NOT NULL,
    login_provider VARCHAR(128) NOT NULL,
    name           VARCHAR(128) NOT NULL,
    value          TEXT NULL,

    PRIMARY KEY (user_id, login_provider, name),

    CONSTRAINT user_tokens_user_id_fkey
        FOREIGN KEY (user_id)
        REFERENCES episodes.users(id)
        ON DELETE CASCADE
);
