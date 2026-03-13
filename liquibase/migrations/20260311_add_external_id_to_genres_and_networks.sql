ALTER TABLE episodes.genres
    ADD COLUMN external_id INT NOT NULL DEFAULT 0;

ALTER TABLE episodes.tv_networks
    ADD COLUMN external_id INT NOT NULL DEFAULT 0;
