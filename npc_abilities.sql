USE accounts;

CREATE TABLE IF NOT EXISTS npc_abilities (
    npc_name VARCHAR(255) NOT NULL,
    ability_id INT NOT NULL,
    slot TINYINT NOT NULL DEFAULT 1,
    priority INT NOT NULL DEFAULT 1,
    FOREIGN KEY (ability_id) REFERENCES abilities(id)
);

INSERT INTO npc_abilities (npc_name, ability_id, slot, priority) VALUES
('Goblin Grunt', 3, 1, 1), -- Bleed
('Forest Wolf', 4, 1, 1), -- Poison
('Bandit Scout', 3, 1, 1),
('Dark Mage', 1, 1, 1), -- Fireball
('Dark Mage', 2, 2, 2);
