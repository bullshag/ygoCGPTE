-- MySQL migration to add max_mana to characters table and backfill data
USE accounts;

ALTER TABLE characters
    ADD COLUMN max_mana INT NOT NULL DEFAULT 0 AFTER mana;

UPDATE characters
SET max_mana = GREATEST(mana, 0);
