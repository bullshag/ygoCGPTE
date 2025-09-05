SELECT id, nickname FROM users WHERE username = @username AND password_hash = @passwordHash;
