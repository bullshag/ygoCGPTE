-- Inserts a new user
INSERT INTO users (Username, Nickname, PasswordHash, Gold, last_seen)
VALUES (@username, @nickname, @passwordHash, 300, NOW());
