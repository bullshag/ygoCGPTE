-- Checks if username exists for Unity RegisterManager
SELECT COUNT(1) AS cnt FROM accounts WHERE username=@username;
