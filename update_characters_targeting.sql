USE accounts;
ALTER TABLE characters ADD COLUMN role VARCHAR(20) NOT NULL DEFAULT 'DPS';
ALTER TABLE characters ADD COLUMN targeting_style VARCHAR(50) NOT NULL DEFAULT 'no priorities';
