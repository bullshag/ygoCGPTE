-- Queries used by the Unity ShopManager

-- Fetch items for sale at a node
SELECT item_id, price FROM shop_stock WHERE node_id = @nodeId;

-- Purchase item and update inventory
UPDATE users SET gold = GREATEST(gold - @cost, 0) WHERE id=@userId;
INSERT INTO inventory(user_id, item_id, quantity) VALUES(@userId, @itemId, 1)
    ON DUPLICATE KEY UPDATE quantity = quantity + 1;
