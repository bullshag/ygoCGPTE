-- Checks if username exists
SELECT COUNT(1) FROM Users WHERE Username=@username;
