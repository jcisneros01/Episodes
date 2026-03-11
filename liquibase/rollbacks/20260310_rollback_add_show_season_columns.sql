ALTER TABLE episodes.seasons
    DROP COLUMN episode_count;

ALTER TABLE episodes.shows
    DROP COLUMN overview,
    DROP COLUMN in_production,
    DROP COLUMN number_of_seasons,
    DROP COLUMN number_of_episodes;
