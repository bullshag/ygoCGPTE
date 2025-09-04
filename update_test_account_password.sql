-- Updates the stored password hash for the default test account.
UPDATE accounts
SET password_hash = 'n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg='
WHERE username = 'test';
