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
