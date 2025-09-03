-- Loads user inventory items
SELECT item_name, quantity FROM user_items WHERE account_id=@id;
