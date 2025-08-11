USE accounts;

ALTER TABLE characters
    ADD COLUMN is_dead TINYINT NOT NULL DEFAULT 0,
    ADD COLUMN in_graveyard TINYINT NOT NULL DEFAULT 0,
    ADD COLUMN cause_of_death VARCHAR(255),
    ADD COLUMN death_time DATETIME;
