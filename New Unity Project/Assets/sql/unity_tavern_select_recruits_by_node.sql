-- Fetch persisted tavern recruits for a specific node
SELECT r.recruit_id,
       c.name,
       c.level,
       r.cost,
       r.created_utc,
       r.strength,
       r.dexterity,
       r.intelligence,
       r.max_hp,
       r.max_mp,
       r.action_speed,
       r.physical_defense,
       r.magic_defense,
       r.rolled_points
FROM unity_tavern_recruits AS r
JOIN characters AS c ON c.id = r.recruit_id
WHERE r.node_id = @nodeId
  AND r.purchased_utc IS NULL
ORDER BY r.created_utc ASC;
