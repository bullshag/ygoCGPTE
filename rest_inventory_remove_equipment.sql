-- Removes equipment from a character slot
DELETE FROM character_equipment WHERE account_id=@id AND character_name=@character AND slot=@slot;
