-- MySQL script to create passive ability tables and seed data
USE accounts;

CREATE TABLE IF NOT EXISTS passives (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT
);

CREATE TABLE IF NOT EXISTS character_passives (
    character_id INT NOT NULL,
    passive_id INT NOT NULL,
    level INT NOT NULL DEFAULT 1,
    PRIMARY KEY(character_id, passive_id),
    FOREIGN KEY (character_id) REFERENCES characters(id),
    FOREIGN KEY (passive_id) REFERENCES passives(id)
);

INSERT INTO passives (name, description) VALUES
('Parry', '5% +1% per 30 STR or DEX chance to parry incoming attacks.'),
('Nimble', '1% per 10 DEX chance to dodge attacks.'),
('Flesh Rip', '5% +1% per 15 STR chance to inflict a bleed dealing 10% +1% per 15 STR of weapon damage.'),
('Deadly Strikes', 'Crit chance increased by 1% for every 10 DEX.'),
('Battle Mage', 'Leech mana equal to 15% of INT on weapon hit.'),
('Bloodlust', 'Take 75% more damage and deal +2% weapon damage. Missing HP reduces damage taken and increases bonus.');
