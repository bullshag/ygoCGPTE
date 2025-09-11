-- Fetch active non-mercenary party members for Unity CharacterService
SELECT name, current_hp AS hp, max_hp, mana, max_mana
FROM characters
WHERE account_id = @id AND is_dead = 0 AND in_arena = 0 AND in_tavern = 0 AND is_mercenary = 0;
