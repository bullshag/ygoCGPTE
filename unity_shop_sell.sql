-- Sell item and update inventory and gold
UPDATE users SET gold = gold + (@price * @quantity) WHERE id=@userId;
UPDATE inventory SET quantity = quantity - @quantity
WHERE user_id=@userId AND item_id=@itemId;
DELETE FROM inventory
WHERE user_id=@userId AND item_id=@itemId AND quantity <= 0;

