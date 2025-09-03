-- Grant an ability to a character from a tome
INSERT IGNORE INTO character_abilities(character_id, ability_id)
SELECT id, @aid FROM characters WHERE account_id=@uid AND name=@name;
