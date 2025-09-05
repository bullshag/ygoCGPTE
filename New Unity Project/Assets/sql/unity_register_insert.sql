INSERT INTO users (username, nickname, password_hash, gold, last_seen)
VALUES (@username, @nickname, @passwordHash, 300, NOW());
