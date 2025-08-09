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
