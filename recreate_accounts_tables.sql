-- Recreate the accounts database schema with all gameplay tables.
-- Run with: mysql -u <user> -p -h <host> < recreate_accounts_tables.sql

CREATE DATABASE IF NOT EXISTS accounts;
USE accounts;

-- Ensure triggers and procedures can be replaced when rerunning the script.
DROP TRIGGER IF EXISTS before_insert_npcs_power;
DROP PROCEDURE IF EXISTS GenerateNPC;

-- Core account and social structures.
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

-- Character progression and combat data.
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
    in_tavern TINYINT(1) NOT NULL DEFAULT 0,
    in_arena TINYINT(1) NOT NULL DEFAULT 0,
    is_dead TINYINT(1) NOT NULL DEFAULT 0,
    in_graveyard TINYINT(1) NOT NULL DEFAULT 0,
    cause_of_death VARCHAR(255),
    death_time DATETIME,
    is_mercenary TINYINT(1) NOT NULL DEFAULT 0,
    role VARCHAR(20) NOT NULL DEFAULT 'DPS',
    targeting_style VARCHAR(50) NOT NULL DEFAULT 'no priorities',
    FOREIGN KEY (account_id) REFERENCES users(id)
);

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
    PRIMARY KEY (character_id, ability_id),
    FOREIGN KEY (character_id) REFERENCES characters(id),
    FOREIGN KEY (ability_id) REFERENCES abilities(id)
);

CREATE TABLE IF NOT EXISTS character_ability_slots (
    character_id INT NOT NULL,
    slot TINYINT NOT NULL,
    ability_id INT NULL,
    priority INT NOT NULL DEFAULT 1,
    PRIMARY KEY (character_id, slot),
    FOREIGN KEY (character_id) REFERENCES characters(id),
    FOREIGN KEY (ability_id) REFERENCES abilities(id)
);

CREATE TABLE IF NOT EXISTS passives (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT
);

CREATE TABLE IF NOT EXISTS character_passives (
    character_id INT NOT NULL,
    passive_id INT NOT NULL,
    level INT NOT NULL DEFAULT 1,
    PRIMARY KEY (character_id, passive_id),
    FOREIGN KEY (character_id) REFERENCES characters(id),
    FOREIGN KEY (passive_id) REFERENCES passives(id)
);

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

-- World map traversal and questing tables.
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
    current_min_power INT NOT NULL DEFAULT 1,
    current_max_power INT NOT NULL DEFAULT 5,
    FOREIGN KEY (account_id) REFERENCES users(id)
);

-- Enemy, NPC, and loot metadata.
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
    magic_defense INT NOT NULL,
    role VARCHAR(20) NOT NULL DEFAULT 'DPS',
    targeting_style VARCHAR(50) NOT NULL DEFAULT 'no priorities',
    power INT DEFAULT NULL
);

CREATE TABLE IF NOT EXISTS npc_locations (
    npc_id INT NOT NULL,
    node_id VARCHAR(50) NOT NULL,
    PRIMARY KEY (npc_id, node_id),
    FOREIGN KEY (npc_id) REFERENCES npcs(id),
    FOREIGN KEY (node_id) REFERENCES nodes(id)
);

CREATE TABLE IF NOT EXISTS npc_abilities (
    npc_name VARCHAR(255) NOT NULL,
    ability_id INT NOT NULL,
    slot TINYINT NOT NULL DEFAULT 1,
    priority INT NOT NULL DEFAULT 1,
    FOREIGN KEY (ability_id) REFERENCES abilities(id)
);

CREATE TABLE IF NOT EXISTS npc_equipment (
    npc_name VARCHAR(255) NOT NULL,
    slot VARCHAR(50) NOT NULL,
    item_name VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS npc_loot (
    npc_name VARCHAR(255) NOT NULL,
    item_name VARCHAR(255) NOT NULL,
    drop_chance DOUBLE NOT NULL,
    min_quantity INT NOT NULL DEFAULT 1,
    max_quantity INT NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS enemy_kills (
    account_id INT NOT NULL,
    enemy_name VARCHAR(255) NOT NULL,
    kill_count INT NOT NULL,
    PRIMARY KEY (account_id, enemy_name)
);

-- Miscellaneous gameplay tables.
CREATE TABLE IF NOT EXISTS party_hires (
    id VARCHAR(36) PRIMARY KEY,
    owner_id INT NOT NULL,
    name VARCHAR(255) NOT NULL,
    cost INT NOT NULL,
    members_json TEXT NOT NULL,
    on_mission TINYINT(1) NOT NULL DEFAULT 0,
    current_hirer INT NULL,
    hired_until DATETIME NULL,
    gold_earned INT NOT NULL DEFAULT 0,
    FOREIGN KEY (owner_id) REFERENCES users(id),
    FOREIGN KEY (current_hirer) REFERENCES users(id)
);

CREATE TABLE IF NOT EXISTS trinkets (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    description TEXT NOT NULL,
    effect_json JSON NOT NULL,
    drop_chance DECIMAL(4,3) NOT NULL
);

CREATE TABLE IF NOT EXISTS player_position (
    player_id INT NOT NULL PRIMARY KEY,
    current_pos VARCHAR(255) NOT NULL,
    is_traveling TINYINT(1) NOT NULL,
    next_waypoint VARCHAR(255) NULL,
    timestamp TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP
);

-- Stored procedure for generating NPC templates.
DELIMITER $$
CREATE PROCEDURE GenerateNPC(IN npcName VARCHAR(255), IN npcLevel INT)
BEGIN
    INSERT INTO npcs (
        name, level, current_hp, max_hp, mana, action_speed,
        strength, dex, intelligence, melee_defense, magic_defense, power
    )
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
           base_magic_def + npcLevel,
           75 * npcLevel
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

-- Trigger to backfill NPC power ratings.
DELIMITER $$
CREATE TRIGGER before_insert_npcs_power
BEFORE INSERT ON npcs
FOR EACH ROW
BEGIN
    IF NEW.power IS NULL THEN
        SET NEW.power = 75 * NEW.level;
    END IF;
END$$
DELIMITER ;
