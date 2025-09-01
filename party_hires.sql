-- MySQL script to create the party_hires table for shared hireable parties
USE accounts;

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
