-- Loads user inventory items
SELECT item_name, quantity FROM user_items WHERE account_id=@id;

-- Loads user equipment
SELECT character_name, slot, item_name FROM character_equipment WHERE account_id=@id;
