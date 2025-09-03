-- Additional shop transaction queries for Unity client

-- Record sale of an item and update player inventory and gold
UPDATE users SET gold = gold + @earnings WHERE id=@userId;
UPDATE inventory SET quantity = quantity - @qty WHERE user_id=@userId AND item_id=@itemId;
DELETE FROM inventory WHERE user_id=@userId AND item_id=@itemId AND quantity<=0;

