-- Loads user equipment
SELECT character_name, slot, item_name FROM character_equipment WHERE account_id=@id;
