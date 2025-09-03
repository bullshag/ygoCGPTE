-- REST login query
SELECT id, nickname FROM accounts WHERE username = @username AND password_hash = @passwordHash;
