USE accounts;

ALTER TABLE characters
    ADD COLUMN in_tavern TINYINT NOT NULL DEFAULT 0;
