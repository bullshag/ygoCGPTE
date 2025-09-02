USE accounts;

INSERT INTO npc_loot (npc_name, item_name, drop_chance, min_quantity, max_quantity)
SELECT n.name, 'gold', 0.5, 5, 10
FROM npcs n
LEFT JOIN npc_loot l ON n.name = l.npc_name
WHERE l.npc_name IS NULL;
