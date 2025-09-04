-- Query used by Unity LoginManager to authenticate users against the Users table
SELECT id, nickname FROM Users WHERE Username = @username AND PasswordHash = @passwordHash;
