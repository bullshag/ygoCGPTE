-- Refund gold when a tavern hire fails after deducting funds
UPDATE users
SET gold = gold + @cost
WHERE id = @userId;
