ALTER TABLE episodes.shows
    ADD COLUMN overview            TEXT NULL,
    ADD COLUMN in_production       BOOLEAN NOT NULL DEFAULT FALSE,
    ADD COLUMN number_of_seasons   INT NOT NULL DEFAULT 0,
    ADD COLUMN number_of_episodes  INT NOT NULL DEFAULT 0;

ALTER TABLE episodes.seasons
    ADD COLUMN episode_count INT NOT NULL DEFAULT 0;
