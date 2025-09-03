-- Checks if nickname exists
SELECT COUNT(1) FROM Users WHERE Nickname=@nickname;
