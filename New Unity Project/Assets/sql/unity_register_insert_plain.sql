-- Inserts a new account without hashing the password
INSERT INTO Users (Username, Nickname, PasswordHash, Gold, last_seen)
VALUES (@username, @nickname, @password, 300, NOW());
