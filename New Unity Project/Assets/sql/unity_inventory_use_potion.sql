-- Heal a character with a potion
UPDATE characters SET current_hp = LEAST(max_hp, current_hp + @heal)
WHERE account_id=@uid AND name=@name AND is_dead=0 AND in_arena=0 AND in_tavern=0;
