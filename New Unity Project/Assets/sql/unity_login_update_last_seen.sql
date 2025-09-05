UPDATE users SET last_seen = NOW() WHERE id = @id;
