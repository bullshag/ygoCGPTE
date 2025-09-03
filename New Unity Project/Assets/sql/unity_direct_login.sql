-- Query used by Unity LoginManager for direct database authentication
SELECT id, nickname FROM accounts WHERE username = @username AND password_hash = @passwordHash;
