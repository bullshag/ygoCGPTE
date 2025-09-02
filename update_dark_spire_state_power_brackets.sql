ALTER TABLE dark_spire_state
    CHANGE COLUMN current_min current_min_power INT NOT NULL,
    CHANGE COLUMN current_max current_max_power INT NOT NULL;
