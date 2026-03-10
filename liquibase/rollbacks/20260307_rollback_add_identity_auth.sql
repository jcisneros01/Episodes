DROP TABLE IF EXISTS episodes.user_tokens;
DROP TABLE IF EXISTS episodes.user_roles;
DROP TABLE IF EXISTS episodes.user_logins;
DROP TABLE IF EXISTS episodes.user_claims;
DROP TABLE IF EXISTS episodes.role_claims;
DROP TABLE IF EXISTS episodes.roles;

DROP INDEX IF EXISTS episodes.users_normalized_email_key;
DROP INDEX IF EXISTS episodes.users_normalized_user_name_key;

ALTER TABLE episodes.users
    DROP COLUMN IF EXISTS access_failed_count,
    DROP COLUMN IF EXISTS lockout_enabled,
    DROP COLUMN IF EXISTS lockout_end,
    DROP COLUMN IF EXISTS two_factor_enabled,
    DROP COLUMN IF EXISTS phone_number_confirmed,
    DROP COLUMN IF EXISTS phone_number,
    DROP COLUMN IF EXISTS concurrency_stamp,
    DROP COLUMN IF EXISTS security_stamp,
    DROP COLUMN IF EXISTS email_confirmed,
    DROP COLUMN IF EXISTS normalized_email,
    DROP COLUMN IF EXISTS normalized_user_name,
    DROP COLUMN IF EXISTS user_name;

CREATE UNIQUE INDEX users_email_lower_idx
    ON episodes.users (LOWER(email));
