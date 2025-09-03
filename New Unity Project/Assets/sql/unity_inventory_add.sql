-- Adds quantity to an item in inventory
INSERT INTO user_items(account_id, item_name, quantity) VALUES(@id, @name, @qty)
ON DUPLICATE KEY UPDATE quantity = quantity + @qty;
