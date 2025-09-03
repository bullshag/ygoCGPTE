-- Inserts a new account for Unity RegisterManager
INSERT INTO accounts (username, nickname, password_hash, gold, last_seen)
VALUES (@username, @nickname, @passwordHash, 300, NOW());
