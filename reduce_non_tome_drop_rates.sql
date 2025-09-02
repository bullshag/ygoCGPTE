USE accounts;

-- Halve drop chances for all items except tomes
UPDATE npc_loot
SET drop_chance = drop_chance * 0.5
WHERE item_name NOT LIKE '%Tome%';

UPDATE trinkets
SET drop_chance = drop_chance * 0.5
WHERE name NOT LIKE '%Tome%';
