-- Query to fetch additional enemies within 10 levels of the area's minimum level
SELECT n.name, n.power, n.level
FROM npcs n
LEFT JOIN npc_locations l ON n.id = l.npc_id
WHERE n.level BETWEEN @min AND @min + 10
  AND (@area IS NULL OR l.node_id = @area)
ORDER BY RAND();
