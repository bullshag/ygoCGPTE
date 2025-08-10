USE accounts;

CREATE TABLE IF NOT EXISTS npc_loot (
    npc_name VARCHAR(255) NOT NULL,
    item_name VARCHAR(255) NOT NULL,
    drop_chance DOUBLE NOT NULL,
    min_quantity INT NOT NULL DEFAULT 1,
    max_quantity INT NOT NULL DEFAULT 1
);

INSERT INTO npc_loot (npc_name, item_name, drop_chance, min_quantity, max_quantity) VALUES
('Goblin Grunt', 'gold', 1.0, 5, 10),
('Goblin Grunt', 'Dagger', 0.2, 1, 1),
('Goblin Grunt', 'Shortsword', 0.1, 1, 1),
('Goblin Grunt', 'Healing Potion', 0.15, 1, 1),

('Forest Wolf', 'gold', 1.0, 8, 15),
('Forest Wolf', 'Dagger', 0.1, 1, 1),
('Forest Wolf', 'Healing Potion', 0.05, 1, 1),

('Bandit Scout', 'gold', 1.0, 10, 20),
('Bandit Scout', 'Shortsword', 0.15, 1, 1),
('Bandit Scout', 'Bow', 0.2, 1, 1),
('Bandit Scout', 'Healing Potion', 0.15, 1, 1),

('Skeleton Warrior', 'gold', 1.0, 15, 25),
('Skeleton Warrior', 'Longsword', 0.15, 1, 1),
('Skeleton Warrior', 'Greataxe', 0.1, 1, 1),
('Skeleton Warrior', 'Healing Potion', 0.2, 1, 1),

('Orc Brute', 'gold', 1.0, 20, 30),
('Orc Brute', 'Greataxe', 0.2, 1, 1),
('Orc Brute', 'Mace', 0.1, 1, 1),
('Orc Brute', 'Healing Potion', 0.25, 1, 1),

('Dark Mage', 'gold', 1.0, 25, 40),
('Dark Mage', 'Staff', 0.2, 1, 1),
('Dark Mage', 'Wand', 0.2, 1, 1),
('Dark Mage', 'Healing Potion', 0.3, 1, 1),

('Troll Berserker', 'gold', 1.0, 30, 50),
('Troll Berserker', 'Greatmaul', 0.2, 1, 1),
('Troll Berserker', 'Greataxe', 0.15, 1, 1),
('Troll Berserker', 'Healing Potion', 0.35, 1, 1),

('Vampire Knight', 'gold', 1.0, 40, 60),
('Vampire Knight', 'Scythe', 0.2, 1, 1),
('Vampire Knight', 'Greatsword', 0.25, 1, 1),
('Vampire Knight', 'Healing Potion', 0.4, 1, 1),

('Dragon Whelp', 'gold', 1.0, 50, 80),
('Dragon Whelp', 'Longsword', 0.2, 1, 1),
('Dragon Whelp', 'Staff', 0.2, 1, 1),
('Dragon Whelp', 'Healing Potion', 0.35, 1, 1),

('Ancient Dragon', 'gold', 1.0, 100, 200),
('Ancient Dragon', 'Greatsword', 0.25, 1, 1),
('Ancient Dragon', 'Rod', 0.2, 1, 1),
('Ancient Dragon', 'Healing Potion', 0.5, 1, 2);
