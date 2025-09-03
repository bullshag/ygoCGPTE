-- Query used by Unity LoginManager to authenticate users
SELECT id, nickname FROM accounts WHERE username = @username AND password_hash = @passwordHash;
