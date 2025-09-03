-- Saves equipment for a character slot, removing existing entry and inserting if item provided
DELETE FROM character_equipment WHERE account_id=@id AND character_name=@character AND slot=@slot;
INSERT INTO character_equipment(account_id, character_name, slot, item_name)
SELECT @id, @character, @slot, @name FROM DUAL WHERE @name IS NOT NULL;
