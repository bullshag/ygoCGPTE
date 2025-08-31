-- Combined SQL script to rebuild database

-- File: accounts.sql
-- MySQL script to create the 'accounts' database and users table
-- Connect with: mysql -u root -p'' -h localhost -P 3306

CREATE DATABASE IF NOT EXISTS accounts;
USE accounts;

CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) NOT NULL UNIQUE,
    nickname VARCHAR(255) NOT NULL UNIQUE,
    passwordhash VARCHAR(255) NOT NULL,
    gold INT NOT NULL DEFAULT 300,
    last_seen DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS chat_messages (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sender_id INT NOT NULL,
    recipient_id INT NULL,
    message TEXT NOT NULL,
    sent_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (sender_id) REFERENCES users(id),
    FOREIGN KEY (recipient_id) REFERENCES users(id)
);

CREATE TABLE IF NOT EXISTS friend_requests (
    id INT AUTO_INCREMENT PRIMARY KEY,
    requester_id INT NOT NULL,
    receiver_id INT NOT NULL,
    status ENUM('pending','accepted','declined') NOT NULL DEFAULT 'pending',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (requester_id) REFERENCES users(id),
    FOREIGN KEY (receiver_id) REFERENCES users(id)
);

CREATE TABLE IF NOT EXISTS friends (
    user_id INT NOT NULL,
    friend_id INT NOT NULL,
    PRIMARY KEY (user_id, friend_id),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (friend_id) REFERENCES users(id)
);

CREATE TABLE IF NOT EXISTS mail_messages (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sender_id INT NULL,
    recipient_id INT NOT NULL,
    subject VARCHAR(255) NOT NULL,
    body TEXT NOT NULL,
    sent_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_read TINYINT(1) NOT NULL DEFAULT 0,
    FOREIGN KEY (sender_id) REFERENCES users(id),
    FOREIGN KEY (recipient_id) REFERENCES users(id)
);

-- File: characters.sql
-- MySQL script to create the 'characters' table for RPG data
-- Assumes the 'accounts' database already exists

USE accounts;

CREATE TABLE IF NOT EXISTS characters (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id INT,
    name VARCHAR(255) NOT NULL,
    current_hp INT NOT NULL,
    max_hp INT NOT NULL,
    mana INT NOT NULL,
    experience_points INT NOT NULL DEFAULT 0,
    action_speed INT NOT NULL,
    strength INT NOT NULL,
    dex INT NOT NULL,
    intelligence INT NOT NULL,
    melee_defense INT NOT NULL,
    magic_defense INT NOT NULL,
    level INT NOT NULL DEFAULT 1,
    skill_points INT NOT NULL DEFAULT 0,
    in_tavern TINYINT NOT NULL DEFAULT 0,
    FOREIGN KEY (account_id) REFERENCES users(id)
);

-- File: abilities.sql
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

-- File: add_more_spells.sql
USE accounts;
INSERT INTO abilities (name, description, cost, cooldown) VALUES
('Ice Lance', 'Launch a shard of ice dealing 6 + 110% of your INT damage to a single enemy.', 40, 0),
('Lightning Bolt', 'Call down lightning dealing 4 + 120% of your INT damage to a single foe.', 45, 0),
('Shield Bash', 'Bash a foe for 2 + 50% of your STR damage and stun briefly.', 30, 0),
('Rejuvenate', 'Heal an ally over time for 1 + 60% of your INT every 2s for 6s.', 35, 0),
('Stone Skin', 'Increase an ally\'s defense by 20% for 5s.', 25, 0),
('Arcane Blast', 'Unleash arcane energy dealing 8 + 90% of your INT damage to all enemies.', 60, 0),
('Poison Arrow', 'Fire a toxic shot dealing 2 + 40% of your DEX damage and poisoning for 6s.', 30, 0),
('Cleanse', 'Remove negative effects from an ally.', 20, 0),
('Berserk', 'Increase own damage by 40% for 8s.', 50, 0),
('Drain Life', 'Steal 3 + 70% of your INT HP from an enemy.', 55, 0),
('Guardian Ward', 'Grant a shield to yourself and nearby allies absorbing 4 + 120% of your INT damage for 15s.', 55, 25),
('Divine Aegis', 'Place a large barrier on an ally absorbing 8 + 250% of your INT damage for 15s.', 45, 20);

-- File: additional_skills.sql
USE accounts;
INSERT INTO abilities (name, description, cost, cooldown) VALUES
('Smite', 'Smite an enemy for 6 + 110% of your STR holy damage.', 35, 0),
('Shadow Bolt', 'Hurl a shadowy bolt dealing 5 + 115% of your INT shadow damage.', 40, 0),
('Frost Nova', 'Explode in frost dealing 3 + 100% of your INT ice damage to all enemies.', 50, 10),
('Piercing Shot', 'Fire a shot dealing 4 + 90% of your DEX damage that ignores armor.', 30, 0),
('Shockwave', 'Release a shockwave for 7 + 80% of your STR damage to all foes.', 45, 5),
('Quick Strike', 'Strike swiftly for 2 + 75% of your DEX damage.', 20, 0),
('Blizzard', 'Summon icy winds dealing 2 + 60% of your INT damage each second for 5s.', 60, 15),
('Searing Light', 'Burn undead with 8 + 120% of your INT holy damage.', 40, 5),
('Venom Cloud', 'Create a cloud dealing 1 + 40% of your DEX damage each second for 6s.', 35, 12),
('Earthquake', 'Shake the ground for 10 + 100% of your STR damage to all enemies.', 70, 20),
('Wind Slash', 'Cut with wind for 3 + 85% of your DEX damage.', 25, 0),
('Mind Spike', 'Lance the mind for 7 + 105% of your INT psychic damage.', 40, 0),
('Holy Blessing', 'Increase an ally\'s damage by 2 + 50% of your INT for 10s.', 30, 20),
('Arcane Shield', 'Conjure a barrier absorbing 5 + 150% of your INT damage for 15s.', 45, 15),
('Battle Cry', 'Increase all allies\' STR by 5 + 30% of your STR for 10s.', 35, 20),
('Crippling Blow', 'Smash an enemy for 5 + 95% of your STR damage and slow them.', 30, 8),
('Shadowstep', 'Blink behind a foe dealing 5 + 95% of your DEX damage.', 25, 5),
('Meteor', 'Call a meteor for 12 + 130% of your INT fire damage to all enemies.', 80, 30),
('Chain Lightning', 'Zap a foe for 6 + 110% of your INT damage, jumping to others.', 55, 8),
('Blood Pact', 'Deal 8 + 120% of your STR damage but take 2 + 50% of your STR recoil.', 40, 10),
('Enrage', 'Increase your damage by 5 + 40% of your STR for 8s.', 30, 25),
('Healing Wave', 'Heal the party for 4 + 100% of your INT HP.', 60, 20),
('Fortify', 'Grant a shield of 3 + 60% of your INT damage for 8s.', 35, 15),
('Flame Strike', 'Smash the ground for 7 + 110% of your INT fire damage to all enemies.', 65, 12),
('Silencing Shot', 'Deal 4 + 80% of your DEX damage and silence for 3s.', 30, 8),
('Rend Armor', 'Rip armor dealing 3 + 70% of your STR damage and reducing defense.', 30, 10),
('Stunning Fist', 'Punch for 4 + 80% of your STR damage and stun briefly.', 25, 6),
('Smokescreen', 'Obscure the area increasing dodge by 5 + 40% of your DEX for 6s.', 30, 18),
('Magic Missile', 'Launch a missile dealing 3 + 80% of your INT arcane damage.', 20, 0),
('Thunderclap', 'Clap thunder for 6 + 100% of your STR damage to nearby foes.', 45, 12),
('Chain Heal', 'Heal an ally for 5 + 100% of your INT then bounce to another ally for 50% of that amount.', 50, 12),
('Prayer of Healing', 'Heal all allies for 6 + 80% of your INT.', 55, 15),
('Holy Light', 'Heal a single ally for 8 + 150% of your INT.', 45, 8),
('Guardian Ward', 'Grant a shield to yourself and nearby allies absorbing 4 + 120% of your INT damage for 15s.', 55, 25),
('Divine Aegis', 'Place a large barrier on an ally absorbing 8 + 250% of your INT damage for 15s.', 45, 20);

-- File: passives.sql
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

-- File: inventory_tables.sql
-- MySQL script to create tables for storing user inventory and equipment
USE accounts;

CREATE TABLE IF NOT EXISTS user_items (
    account_id INT NOT NULL,
    item_name VARCHAR(255) NOT NULL,
    quantity INT NOT NULL DEFAULT 0,
    PRIMARY KEY (account_id, item_name),
    FOREIGN KEY (account_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS character_equipment (
    account_id INT NOT NULL,
    character_name VARCHAR(255) NOT NULL,
    slot VARCHAR(50) NOT NULL,
    item_name VARCHAR(255) NOT NULL,
    PRIMARY KEY (account_id, character_name, slot),
    FOREIGN KEY (account_id) REFERENCES users(id) ON DELETE CASCADE
);

-- File: arena_tables.sql
-- Tables for the battle arena feature
USE accounts;

CREATE TABLE IF NOT EXISTS arena_teams (
    account_id INT PRIMARY KEY,
    wins INT NOT NULL DEFAULT 0,
    FOREIGN KEY (account_id) REFERENCES users(id)
);

CREATE TABLE IF NOT EXISTS arena_battle_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    attacker_id INT NOT NULL,
    defender_id INT NOT NULL,
    log TEXT NOT NULL,
    battle_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (attacker_id) REFERENCES users(id),
    FOREIGN KEY (defender_id) REFERENCES users(id)
);

-- File: world_map.sql
CREATE TABLE IF NOT EXISTS nodes (
    id VARCHAR(50) PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS node_connections (
    from_node VARCHAR(50) NOT NULL,
    to_node VARCHAR(50) NOT NULL,
    travel_time_days INT NOT NULL,
    PRIMARY KEY (from_node, to_node),
    FOREIGN KEY (from_node) REFERENCES nodes(id),
    FOREIGN KEY (to_node) REFERENCES nodes(id)
);

CREATE TABLE IF NOT EXISTS activities (
    id INT AUTO_INCREMENT PRIMARY KEY,
    node_id VARCHAR(50) NOT NULL,
    activity_type VARCHAR(50),
    description VARCHAR(255),
    duration_seconds INT,
    FOREIGN KEY (node_id) REFERENCES nodes(id)
);

CREATE TABLE IF NOT EXISTS travel_state (
    account_id INT PRIMARY KEY,
    current_node VARCHAR(50) NOT NULL,
    destination_node VARCHAR(50) NOT NULL,
    start_time DATETIME,
    arrival_time DATETIME,
    progress_seconds INT DEFAULT 0,
    faster_travel TINYINT(1) DEFAULT 0,
    travel_cost INT DEFAULT 0,
    FOREIGN KEY (current_node) REFERENCES nodes(id),
    FOREIGN KEY (destination_node) REFERENCES nodes(id)
);

CREATE TABLE IF NOT EXISTS travel_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id INT,
    from_node VARCHAR(50),
    to_node VARCHAR(50),
    start_time DATETIME,
    end_time DATETIME,
    original_days INT,
    travel_days INT,
    cost_gold INT,
    faster_travel_applied TINYINT(1),
    FOREIGN KEY (from_node) REFERENCES nodes(id),
    FOREIGN KEY (to_node) REFERENCES nodes(id)
);

CREATE TABLE IF NOT EXISTS notifications (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id INT,
    created_at DATETIME,
    message TEXT
);

CREATE TABLE IF NOT EXISTS quests (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id INT,
    description TEXT,
    progress INT DEFAULT 0,
    target INT DEFAULT 0,
    completed TINYINT(1) DEFAULT 0
);

CREATE TABLE IF NOT EXISTS dark_spire_state (
    account_id INT PRIMARY KEY,
    current_min INT NOT NULL,
    current_max INT NOT NULL,
    FOREIGN KEY (account_id) REFERENCES users(id)
);

-- File: npcs.sql
-- MySQL script to create the 'npcs' table and a procedure to generate random NPCs
-- Assumes the 'accounts' database already exists

USE accounts;

CREATE TABLE IF NOT EXISTS npcs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    level INT NOT NULL,
    current_hp INT NOT NULL,
    max_hp INT NOT NULL,
    mana INT NOT NULL,
    action_speed INT NOT NULL,
    strength INT NOT NULL,
    dex INT NOT NULL,
    intelligence INT NOT NULL,
    melee_defense INT NOT NULL,
    magic_defense INT NOT NULL
);

DELIMITER $$
CREATE PROCEDURE GenerateNPC(IN npcName VARCHAR(255), IN npcLevel INT)
BEGIN
    INSERT INTO npcs (name, level, current_hp, max_hp, mana, action_speed, strength, dex, intelligence, melee_defense, magic_defense)
    SELECT npcName,
           npcLevel,
           base_hp + npcLevel * 5,
           base_hp + npcLevel * 5,
           base_mana + npcLevel * 5,
           1,
           base_str + npcLevel,
           base_dex + npcLevel,
           base_int + npcLevel,
           base_melee_def + npcLevel,
           base_magic_def + npcLevel
    FROM (
        SELECT
            FLOOR(RAND() * 20) + 30 AS base_hp,
            FLOOR(RAND() * 20) + 30 AS base_mana,
            FLOOR(RAND() * 5) + 5 AS base_str,
            FLOOR(RAND() * 5) + 5 AS base_dex,
            FLOOR(RAND() * 5) + 5 AS base_int,
            FLOOR(RAND() * 5) + 5 AS base_melee_def,
            FLOOR(RAND() * 5) + 5 AS base_magic_def
    ) AS t;
END$$
DELIMITER ;

-- File: seed_npcs.sql
USE accounts;
CALL GenerateNPC('Goblin Grunt',1);
CALL GenerateNPC('Forest Wolf',2);
CALL GenerateNPC('Bandit Scout',3);
CALL GenerateNPC('Skeleton Warrior',4);
CALL GenerateNPC('Orc Brute',5);
CALL GenerateNPC('Dark Mage',6);
CALL GenerateNPC('Troll Berserker',7);
CALL GenerateNPC('Vampire Knight',8);
CALL GenerateNPC('Dragon Whelp',9);
CALL GenerateNPC('Ancient Dragon',10);
CALL GenerateNPC('Highwayman',13);
CALL GenerateNPC('Forest Goblin',14);
CALL GenerateNPC('Cave Spider',2);
CALL GenerateNPC('Desert Bandit',9);
CALL GenerateNPC('Swamp Troll',17);
CALL GenerateNPC('Hill Giant',16);
CALL GenerateNPC('Night Stalker',13);
CALL GenerateNPC('Frost Mage',10);
CALL GenerateNPC('Fire Elemental',16);
CALL GenerateNPC('Water Sprite',12);
CALL GenerateNPC('Stone Golem',19);
CALL GenerateNPC('Shadow Assassin',7);
CALL GenerateNPC('Storm Caller',17);
CALL GenerateNPC('Earth Shaman',5);
CALL GenerateNPC('Skeleton Archer',10);
CALL GenerateNPC('Zombie Brute',5);
CALL GenerateNPC('Pirate Captain',4);
CALL GenerateNPC('Sea Serpent',20);
CALL GenerateNPC('Mountain Yeti',9);
CALL GenerateNPC('Dark Priest',18);
CALL GenerateNPC('Forest Dryad',20);
CALL GenerateNPC('Lava Beast',5);
CALL GenerateNPC('Ice Witch',10);
CALL GenerateNPC('Thunder Drake',4);
CALL GenerateNPC('Cursed Knight',3);
CALL GenerateNPC('Wild Boar',11);
CALL GenerateNPC('Bandit Leader',16);
CALL GenerateNPC('Feral Wolf',18);
CALL GenerateNPC('Savage Orc',4);
CALL GenerateNPC('Ancient Lich',12);
CALL GenerateNPC('Warlock Adept',14);
CALL GenerateNPC('Giant Scorpion',11);
CALL GenerateNPC('Plains Centaur',20);
CALL GenerateNPC('Marsh Hag',7);
CALL GenerateNPC('Ghostly Warrior',18);
CALL GenerateNPC('Crystal Gargoyle',16);
CALL GenerateNPC('Blood Bat',15);
CALL GenerateNPC('Desert Nomad',17);
CALL GenerateNPC('Rogue Mage',9);
CALL GenerateNPC('Temple Guardian',2);
CALL GenerateNPC('Feral Cat',18);
CALL GenerateNPC('Cave Bat',1);
CALL GenerateNPC('Goblin Shaman',3);
CALL GenerateNPC('Toxic Slime',13);
CALL GenerateNPC('Cursed Archer',1);
CALL GenerateNPC('Orc Berserker',20);
CALL GenerateNPC('Nightmare Steed',16);
CALL GenerateNPC('Dire Bear',11);
CALL GenerateNPC('Ghoul',8);
CALL GenerateNPC('Rogue Knight',11);
CALL GenerateNPC('Frost Giant',3);
CALL GenerateNPC('Fire Drake',7);
CALL GenerateNPC('Forest Nymph',19);
CALL GenerateNPC('Dark Ranger',8);
CALL GenerateNPC('Mutant Rat',8);
CALL GenerateNPC('Swamp Beast',5);
CALL GenerateNPC('Hill Raider',18);
CALL GenerateNPC('Storm Mage',15);
CALL GenerateNPC('Stone Titan',3);
CALL GenerateNPC('Shadow Stalker',3);
CALL GenerateNPC('Earth Elemental',11);
CALL GenerateNPC('Skeletal Mage',17);
CALL GenerateNPC('Zombie Hunter',16);
CALL GenerateNPC('Pirate Sailor',4);
CALL GenerateNPC('Sea Witch',10);
CALL GenerateNPC('Mountain Troll',18);
CALL GenerateNPC('Dark Acolyte',10);
CALL GenerateNPC('Forest Sprite',4);
CALL GenerateNPC('Lava Elemental',18);
CALL GenerateNPC('Ice Spirit',11);
CALL GenerateNPC('Thunder Roc',18);
CALL GenerateNPC('Cursed Paladin',7);
CALL GenerateNPC('Wild Stag',20);
CALL GenerateNPC('Bandit Rogue',18);
CALL GenerateNPC('Feral Boar',19);
CALL GenerateNPC('Savage Troll',10);
CALL GenerateNPC('Ancient Dragonling',15);
CALL GenerateNPC('Warlock Master',3);
CALL GenerateNPC('Giant Beetle',20);
CALL GenerateNPC('Plains Raider',13);
CALL GenerateNPC('Marsh Zombie',11);
CALL GenerateNPC('Ghostly Mage',19);
CALL GenerateNPC('Crystal Spirit',8);
CALL GenerateNPC('Blood Wolf',10);
CALL GenerateNPC('Desert Warrior',6);
CALL GenerateNPC('Rogue Assassin',7);
CALL GenerateNPC('Temple Monk',6);
CALL GenerateNPC('Feral Hawk',2);
CALL GenerateNPC('Cave Lizard',20);
CALL GenerateNPC('Goblin Raider',9);
CALL GenerateNPC('Toxic Spider',16);
CALL GenerateNPC('Cursed Warrior',3);
CALL GenerateNPC('Orc Shaman',3);
CALL GenerateNPC('Nightmare Beast',5);
CALL GenerateNPC('Dire Wolf',5);
CALL GenerateNPC('Ghastly Monk',2);
CALL GenerateNPC('Rogue Archer',3);
CALL GenerateNPC('Frost Wyrm',18);
CALL GenerateNPC('Fire Spirit',13);
CALL GenerateNPC('Forest Guardian',17);

-- File: npc_abilities.sql
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

INSERT INTO npc_abilities (npc_name, ability_id, slot, priority) VALUES
('Skeleton Warrior', 3, 1, 1),
('Orc Brute', 6, 1, 1),
('Troll Berserker', 16, 1, 1),
('Vampire Knight', 17, 1, 1),
('Dragon Whelp', 8, 1, 1),
('Ancient Dragon', 35, 1, 1),
('Highwayman', 3, 1, 1),
('Forest Goblin', 4, 1, 1),
('Cave Spider', 4, 1, 1),
('Desert Bandit', 3, 1, 1),
('Swamp Troll', 16, 1, 1),
('Hill Giant', 22, 1, 1),
('Night Stalker', 34, 1, 1),
('Frost Mage', 20, 1, 1),
('Fire Elemental', 41, 1, 1),
('Water Sprite', 24, 1, 1),
('Stone Golem', 27, 1, 1),
('Shadow Assassin', 34, 1, 1),
('Storm Caller', 36, 1, 1),
('Earth Shaman', 27, 1, 1),
('Skeleton Archer', 14, 1, 1),
('Zombie Brute', 3, 1, 1),
('Pirate Captain', 32, 1, 1),
('Sea Serpent', 36, 1, 1),
('Mountain Yeti', 22, 1, 1),
('Dark Priest', 37, 1, 1),
('Forest Dryad', 39, 1, 1),
('Lava Beast', 41, 1, 1),
('Ice Witch', 24, 1, 1),
('Thunder Drake', 36, 1, 1),
('Cursed Knight', 43, 1, 1),
('Wild Boar', 33, 1, 1),
('Bandit Leader', 32, 1, 1),
('Feral Wolf', 4, 1, 1),
('Savage Orc', 33, 1, 1),
('Ancient Lich', 19, 1, 1),
('Warlock Adept', 19, 1, 1),
('Giant Scorpion', 26, 1, 1),
('Plains Centaur', 28, 1, 1),
('Marsh Hag', 26, 1, 1),
('Ghostly Warrior', 29, 1, 1),
('Crystal Gargoyle', 27, 1, 1),
('Blood Bat', 26, 1, 1),
('Desert Nomad', 21, 1, 1),
('Rogue Mage', 19, 1, 1),
('Temple Guardian', 18, 1, 1),
('Feral Cat', 23, 1, 1),
('Cave Bat', 23, 1, 1),
('Goblin Shaman', 31, 1, 1),
('Toxic Slime', 26, 1, 1),
('Cursed Archer', 42, 1, 1),
('Orc Berserker', 16, 1, 1),
('Nightmare Steed', 34, 1, 1),
('Dire Bear', 33, 1, 1),
('Ghoul', 19, 1, 1),
('Rogue Knight', 43, 1, 1),
('Frost Giant', 20, 1, 1),
('Fire Drake', 41, 1, 1),
('Forest Nymph', 39, 1, 1),
('Dark Ranger', 14, 1, 1),
('Mutant Rat', 26, 1, 1),
('Swamp Beast', 33, 1, 1),
('Hill Raider', 22, 1, 1),
('Storm Mage', 36, 1, 1),
('Stone Titan', 27, 1, 1),
('Shadow Stalker', 34, 1, 1),
('Earth Elemental', 27, 1, 1),
('Skeletal Mage', 19, 1, 1),
('Zombie Hunter', 33, 1, 1),
('Pirate Sailor', 21, 1, 1),
('Sea Witch', 24, 1, 1),
('Mountain Troll', 22, 1, 1),
('Dark Acolyte', 19, 1, 1),
('Forest Sprite', 39, 1, 1),
('Lava Elemental', 41, 1, 1),
('Ice Spirit', 24, 1, 1),
('Thunder Roc', 36, 1, 1),
('Cursed Paladin', 18, 1, 1),
('Wild Stag', 33, 1, 1),
('Bandit Rogue', 34, 1, 1),
('Feral Boar', 33, 1, 1),
('Savage Troll', 22, 1, 1),
('Ancient Dragonling', 35, 1, 1),
('Warlock Master', 19, 1, 1),
('Giant Beetle', 33, 1, 1),
('Plains Raider', 28, 1, 1),
('Marsh Zombie', 19, 1, 1),
('Ghostly Mage', 29, 1, 1),
('Crystal Spirit', 40, 1, 1),
('Blood Wolf', 26, 1, 1),
('Desert Warrior', 33, 1, 1),
('Rogue Assassin', 34, 1, 1),
('Temple Monk', 18, 1, 1),
('Feral Hawk', 21, 1, 1),
('Cave Lizard', 26, 1, 1),
('Goblin Raider', 21, 1, 1),
('Toxic Spider', 26, 1, 1),
('Cursed Warrior', 33, 1, 1),
('Orc Shaman', 31, 1, 1),
('Nightmare Beast', 34, 1, 1),
('Dire Wolf', 4, 1, 1),
('Ghastly Monk', 18, 1, 1),
('Rogue Archer', 21, 1, 1),
('Frost Wyrm', 20, 1, 1),
('Fire Spirit', 41, 1, 1),
('Forest Guardian', 40, 1, 1);

-- File: npc_equipment.sql
USE accounts;

CREATE TABLE IF NOT EXISTS npc_equipment (
    npc_name VARCHAR(255) NOT NULL,
    slot VARCHAR(50) NOT NULL,
    item_name VARCHAR(255) NOT NULL
);

INSERT INTO npc_equipment (npc_name, slot, item_name)
SELECT name, 'LeftHand',
       CASE
           WHEN level <= 5 THEN 'Dagger'
           WHEN level <= 10 THEN 'Shortsword'
           WHEN level <= 15 THEN 'Longsword'
           WHEN level <= 20 THEN 'Greatsword'
           ELSE 'Greatsword'
       END
FROM npcs;

INSERT INTO npc_equipment (npc_name, slot, item_name)
SELECT name, 'Body',
       CASE
           WHEN level <= 5 THEN 'Cloth Robe'
           WHEN level <= 10 THEN 'Leather Armor'
           ELSE 'Plate Armor'
       END
FROM npcs;

-- File: npc_loot_tables.sql
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
('Goblin Grunt', 'Leather Cap', 0.1, 1, 1),
('Goblin Grunt', 'Healing Potion', 0.15, 1, 1),

('Forest Wolf', 'gold', 1.0, 8, 15),
('Forest Wolf', 'Dagger', 0.1, 1, 1),
('Forest Wolf', 'Healing Potion', 0.05, 1, 1),

('Bandit Scout', 'gold', 1.0, 10, 20),
('Bandit Scout', 'Shortsword', 0.15, 1, 1),
('Bandit Scout', 'Bow', 0.2, 1, 1),
('Bandit Scout', 'Leather Armor', 0.15, 1, 1),
('Bandit Scout', 'Healing Potion', 0.15, 1, 1),

('Skeleton Warrior', 'gold', 1.0, 15, 25),
('Skeleton Warrior', 'Longsword', 0.15, 1, 1),
('Skeleton Warrior', 'Greataxe', 0.1, 1, 1),
('Skeleton Warrior', 'Leather Boots', 0.15, 1, 1),
('Skeleton Warrior', 'Healing Potion', 0.2, 1, 1),

('Orc Brute', 'gold', 1.0, 20, 30),
('Orc Brute', 'Greataxe', 0.2, 1, 1),
('Orc Brute', 'Mace', 0.1, 1, 1),
('Orc Brute', 'Healing Potion', 0.25, 1, 1),

('Dark Mage', 'gold', 1.0, 25, 40),
('Dark Mage', 'Staff', 0.2, 1, 1),
('Dark Mage', 'Wand', 0.2, 1, 1),
('Dark Mage', 'Cloth Robe', 0.25, 1, 1),
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

-- File: update_characters_arena.sql
USE accounts;

ALTER TABLE characters
    ADD COLUMN in_arena TINYINT NOT NULL DEFAULT 0;

-- File: update_characters_graveyard.sql
USE accounts;

ALTER TABLE characters
    ADD COLUMN is_dead TINYINT NOT NULL DEFAULT 0,
    ADD COLUMN in_graveyard TINYINT NOT NULL DEFAULT 0,
    ADD COLUMN cause_of_death VARCHAR(255),
    ADD COLUMN death_time DATETIME;

-- File: update_characters_mercenary.sql
USE accounts;

ALTER TABLE characters
    ADD COLUMN is_mercenary TINYINT NOT NULL DEFAULT 0;

-- File: update_characters_targeting.sql
USE accounts;
ALTER TABLE characters ADD COLUMN role VARCHAR(20) NOT NULL DEFAULT 'DPS';
ALTER TABLE characters ADD COLUMN targeting_style VARCHAR(50) NOT NULL DEFAULT 'no priorities';

-- File: update_npcs_targeting.sql
USE accounts;
ALTER TABLE npcs ADD COLUMN role VARCHAR(20) NOT NULL DEFAULT 'DPS';
ALTER TABLE npcs ADD COLUMN targeting_style VARCHAR(50) NOT NULL DEFAULT 'no priorities';

-- File: update_poison.sql
UPDATE abilities SET description = 'Poison an enemy for 6s, dealing 1 + 35% of your DEX each second. "Watch them wither away."' WHERE name = 'Poison';

