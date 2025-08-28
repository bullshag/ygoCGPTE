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
