-- Saves equipment for a character slot using REPLACE, matching WinForms InventoryService.SaveEquipment
DELETE FROM character_equipment
WHERE account_id=@id AND character_name=@c AND slot=@s AND @n IS NULL;
REPLACE INTO character_equipment(account_id, character_name, slot, item_name)
SELECT @id, @c, @s, @n FROM DUAL WHERE @n IS NOT NULL;
