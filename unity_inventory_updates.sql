-- Healing potion usage
UPDATE characters SET current_hp = LEAST(max_hp, current_hp + @heal)
WHERE account_id=@uid AND name=@name AND is_dead=0 AND in_arena=0 AND in_tavern=0;

-- Ability tome usage
INSERT IGNORE INTO character_abilities(character_id, ability_id)
SELECT id, @aid FROM characters WHERE account_id=@uid AND name=@name;

-- Equip item
REPLACE INTO character_equipment(account_id, character_name, slot, item_name)
VALUES (@uid, @character, @slot, @item);

-- Unequip item
DELETE FROM character_equipment WHERE account_id=@uid AND character_name=@character AND slot=@slot;
