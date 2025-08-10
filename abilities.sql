-- MySQL script to create ability tables and seed data
USE accounts;

CREATE TABLE IF NOT EXISTS abilities (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    cost INT NOT NULL
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

INSERT INTO abilities (name, description, cost) VALUES
('Fireball', 'Deal fire damage to a single enemy.', 50),
('Heal', 'Restore a small amount of HP to an ally.', 30);
