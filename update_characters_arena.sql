USE accounts;

ALTER TABLE characters
    ADD COLUMN in_arena TINYINT NOT NULL DEFAULT 0;
