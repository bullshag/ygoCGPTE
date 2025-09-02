-- Adds power tracking columns for Dark Spire progression and migrates data.
ALTER TABLE dark_spire_state
    ADD COLUMN current_min_power INT NOT NULL DEFAULT 1,
    ADD COLUMN current_max_power INT NOT NULL DEFAULT 5;

-- Migrate existing min/max level data to new power columns.
UPDATE dark_spire_state
    SET current_min_power = current_min,
        current_max_power = current_max;

-- Remove old level columns.
ALTER TABLE dark_spire_state
    DROP COLUMN current_min,
    DROP COLUMN current_max;
