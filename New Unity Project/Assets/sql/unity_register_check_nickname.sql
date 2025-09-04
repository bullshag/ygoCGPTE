-- Checks if nickname exists for Unity RegisterManager
SELECT COUNT(1) AS cnt FROM accounts WHERE nickname=@nickname;
