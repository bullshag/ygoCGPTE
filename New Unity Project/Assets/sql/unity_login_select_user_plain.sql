-- Query used by Unity LoginManager to authenticate users without hashing
SELECT id, nickname FROM Users WHERE Username = @username AND PasswordHash = @password;
