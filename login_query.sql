-- Query used by LoginManager to authenticate users
SELECT id FROM accounts WHERE username = ? AND password_hash = ?;
