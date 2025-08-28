USE accounts;

ALTER TABLE characters
    ADD COLUMN is_mercenary TINYINT NOT NULL DEFAULT 0;
