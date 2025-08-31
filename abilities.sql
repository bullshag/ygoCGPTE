-- MySQL script to create ability tables and seed data
USE accounts;

CREATE TABLE IF NOT EXISTS abilities (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    cost INT NOT NULL,
    cooldown INT NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS character_abilities (
    character_id INT NOT NULL,
    ability_id INT NOT NULL,
    PRIMARY KEY(character_id, ability_id),
    FOREIGN KEY (character_id) REFERENCES characters(id),
    FOREIGN KEY (ability_id) REFERENCES abilities(id)
);

CREATE TABLE IF NOT EXISTS character_ability_slots (
    character_id INT NOT NULL,
    slot TINYINT NOT NULL,
    ability_id INT NULL,
    priority INT NOT NULL DEFAULT 1,
    PRIMARY KEY(character_id, slot),
    FOREIGN KEY (character_id) REFERENCES characters(id),
    FOREIGN KEY (ability_id) REFERENCES abilities(id)
);

INSERT INTO abilities (name, description, cost, cooldown) VALUES
('Fireball', 'Hurl a blazing orb dealing 5 + 100% of your INT fire damage to a single enemy.', 50, 0),
('Heal', 'Restore 5 + 120% of your INT HP to an ally.', 30, 0),
('Bleed', 'Bleed an enemy for 6s, dealing 1 + 25% of your STR every 0.5s. "Their blood writes your victory."', 20, 10),
('Poison', 'Poison an enemy for 6s, dealing 1 + 35% of your DEX each second. "Watch them wither away."', 20, 10),
('Regenerate', 'Mend an ally for 6s, healing 1 + 80% of their INT every 3s. "Life blooms anew."', 25, 10),
('Taunting Blows', 'Taunt all enemies to attack you for 2s +1s per 30 STR. Cooldown 5s.', 0, 5),
('Vanish', 'Disappear for 5 seconds, avoiding all attacks but unable to act.', 0, 30),
('Guardian Ward', 'Grant a shield to yourself and nearby allies absorbing 4 + 120% of your INT damage for 15s.', 55, 25),
('Divine Aegis', 'Place a large barrier on an ally absorbing 8 + 250% of your INT damage for 15s.', 45, 20);
