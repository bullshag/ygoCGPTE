-- Create the per-node tavern recruit cache used by the Unity client
CREATE TABLE IF NOT EXISTS unity_tavern_recruits (
    node_id VARCHAR(64) NOT NULL,
    recruit_id INT NOT NULL,
    cost INT NOT NULL,
    created_utc DATETIME NOT NULL,
    strength INT NOT NULL,
    dexterity INT NOT NULL,
    intelligence INT NOT NULL,
    max_hp INT NOT NULL,
    max_mp INT NOT NULL,
    action_speed DECIMAL(6,3) NOT NULL,
    physical_defense INT NOT NULL,
    magic_defense INT NOT NULL,
    rolled_points INT NOT NULL,
    purchased_utc DATETIME NULL,
    purchased_account_id INT NULL,
    PRIMARY KEY (node_id, recruit_id),
    INDEX IX_unity_tavern_recruits_created (created_utc),
    INDEX IX_unity_tavern_recruits_purchased (purchased_utc),
    CONSTRAINT FK_unity_tavern_recruits_character FOREIGN KEY (recruit_id) REFERENCES characters(id)
);
