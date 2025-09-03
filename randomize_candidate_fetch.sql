-- Randomized candidate fetch query for BattleForm
SELECT n.name, n.power, n.level
FROM npcs n
LEFT JOIN npc_locations l ON n.id = l.npc_id
WHERE n.level BETWEEN @min AND @max
  AND (@area IS NULL OR l.node_id = @area)
ORDER BY RAND();
