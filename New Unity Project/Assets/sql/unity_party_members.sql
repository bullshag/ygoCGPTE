SELECT name, experience_points, level, current_hp, max_hp, mana, strength, dex, intelligence, in_tavern, is_mercenary
FROM characters
WHERE account_id = @id AND is_dead = 0 AND in_arena = 0;
