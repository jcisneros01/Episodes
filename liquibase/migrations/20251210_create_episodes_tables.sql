CREATE TABLE episodes.users (
    id            SERIAL PRIMARY KEY,
    email         VARCHAR(254) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX users_email_lower_idx
    ON episodes.users (LOWER(email));

CREATE TABLE episodes.tv_data_providers (
    id         SERIAL PRIMARY KEY,
    name       VARCHAR(100) NOT NULL UNIQUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE episodes.shows (
    id               SERIAL PRIMARY KEY,
    name             VARCHAR(255) NOT NULL,
    premiered_date   DATE NULL,
    ended_date       DATE NULL,
    status           TEXT NOT NULL,
    poster_img_link  TEXT NULL,
    external_id      INT NOT NULL,
    data_provider_id INT NOT NULL,
    created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT shows_external_id_data_provider_id_key
        UNIQUE (external_id, data_provider_id),

    CONSTRAINT shows_data_provider_id_fkey
        FOREIGN KEY (data_provider_id)
        REFERENCES episodes.tv_data_providers(id)
);

CREATE TABLE episodes.seasons (
    id               SERIAL PRIMARY KEY,
    show_id          INT NOT NULL,
    season_number    INT NOT NULL,
    name             VARCHAR(255) NULL,
    overview         TEXT NULL,
    air_date         DATE NULL,
    poster_img_link  TEXT NULL,
    external_id      INT NOT NULL,
    data_provider_id INT NOT NULL,
    created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT seasons_show_id_season_number_key
        UNIQUE (show_id, season_number),

    CONSTRAINT seasons_show_id_fkey
        FOREIGN KEY (show_id)
        REFERENCES episodes.shows(id)
        ON DELETE CASCADE,

    CONSTRAINT seasons_data_provider_id_fkey
        FOREIGN KEY (data_provider_id)
        REFERENCES episodes.tv_data_providers(id)
);

CREATE INDEX seasons_show_id_idx
    ON episodes.seasons (show_id);

CREATE TABLE episodes.episodes (
    id               SERIAL PRIMARY KEY,
    season_id        INT NOT NULL,
    episode_number   INT NOT NULL,
    name             VARCHAR(255) NOT NULL,
    overview         TEXT NULL,
    air_date         DATE NULL,
    external_id      INT NOT NULL,
    data_provider_id INT NOT NULL,
    created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT episodes_season_id_episode_number_key
        UNIQUE (season_id, episode_number),

    CONSTRAINT episodes_season_id_fkey
        FOREIGN KEY (season_id)
        REFERENCES episodes.seasons(id)
        ON DELETE CASCADE,

    CONSTRAINT episodes_data_provider_id_fkey
        FOREIGN KEY (data_provider_id)
        REFERENCES episodes.tv_data_providers(id)
);

CREATE INDEX episodes_season_id_idx
    ON episodes.episodes (season_id);

CREATE TABLE episodes.tv_networks (
    id            SERIAL PRIMARY KEY,
    name          VARCHAR(255) NOT NULL UNIQUE,
    logo_img_link TEXT NULL,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE episodes.network_shows (
    network_id INT NOT NULL,
    show_id    INT NOT NULL,

    PRIMARY KEY (network_id, show_id),

    CONSTRAINT network_shows_network_id_fkey
        FOREIGN KEY (network_id)
        REFERENCES episodes.tv_networks(id)
        ON DELETE CASCADE,

    CONSTRAINT network_shows_show_id_fkey
        FOREIGN KEY (show_id)
        REFERENCES episodes.shows(id)
        ON DELETE CASCADE
);

CREATE INDEX network_shows_show_id_idx
    ON episodes.network_shows (show_id);

CREATE TABLE episodes.genres (
    id         SERIAL PRIMARY KEY,
    name       VARCHAR(100) NOT NULL UNIQUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE episodes.genre_shows (
    genre_id INT NOT NULL,
    show_id  INT NOT NULL,

    PRIMARY KEY (genre_id, show_id),

    CONSTRAINT genre_shows_genre_id_fkey
        FOREIGN KEY (genre_id)
        REFERENCES episodes.genres(id)
        ON DELETE CASCADE,

    CONSTRAINT genre_shows_show_id_fkey
        FOREIGN KEY (show_id)
        REFERENCES episodes.shows(id)
        ON DELETE CASCADE
);

CREATE INDEX genre_shows_show_id_idx
    ON episodes.genre_shows (show_id);

CREATE TABLE episodes.user_shows (
    user_id    INT NOT NULL,
    show_id    INT NOT NULL,
    status     TEXT NOT NULL,     
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    PRIMARY KEY (user_id, show_id),

    CONSTRAINT user_shows_user_id_fkey
        FOREIGN KEY (user_id)
        REFERENCES episodes.users(id)
        ON DELETE CASCADE,

    CONSTRAINT user_shows_show_id_fkey
        FOREIGN KEY (show_id)
        REFERENCES episodes.shows(id)
        ON DELETE CASCADE
);

CREATE INDEX user_shows_show_id_idx
    ON episodes.user_shows (show_id);

CREATE TABLE episodes.watched_episodes (
    user_id    INT NOT NULL,
    episode_id INT NOT NULL,
    watched_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    PRIMARY KEY (user_id, episode_id),

    CONSTRAINT watched_episodes_user_id_fkey
        FOREIGN KEY (user_id)
        REFERENCES episodes.users(id)
        ON DELETE CASCADE,

    CONSTRAINT watched_episodes_episode_id_fkey
        FOREIGN KEY (episode_id)
        REFERENCES episodes.episodes(id)
        ON DELETE CASCADE
);

CREATE INDEX watched_episodes_episode_id_idx
    ON episodes.watched_episodes (episode_id);