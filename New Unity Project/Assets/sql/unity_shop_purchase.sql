-- Purchase item and update inventory
UPDATE users SET gold = gold - @price
WHERE id = @userId AND gold >= @price;
INSERT INTO inventory(user_id, item_id, quantity)
VALUES(@userId, @itemId, 1)
ON DUPLICATE KEY UPDATE quantity = quantity + 1;

