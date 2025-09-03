-- Fetch items for sale at a node
SELECT s.item_id AS id, i.name, s.price
FROM shop_stock s
JOIN items i ON s.item_id = i.id
WHERE s.node_id = @nodeId;

