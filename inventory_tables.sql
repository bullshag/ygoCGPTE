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
