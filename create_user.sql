-- Query used by RegisterManager to create a new user
INSERT INTO users (Username, Nickname, PasswordHash, Gold, last_seen)
VALUES (?, ?, ?, 300, NOW());
