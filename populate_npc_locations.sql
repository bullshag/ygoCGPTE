USE accounts;

INSERT INTO npc_locations (npc_id, node_id)
SELECT id,
       CASE
           WHEN level BETWEEN 45 AND 50 THEN 'nodeSouthernIsland'
           WHEN level BETWEEN 41 AND 44 THEN 'nodeDesert'
           WHEN level BETWEEN 36 AND 40 THEN 'nodeFarCliffs'
           WHEN level BETWEEN 26 AND 35 THEN 'nodeNorthernIsland'
           WHEN level BETWEEN 21 AND 25 THEN 'nodeForestPlains'
           ELSE 'nodeMountain'
       END
FROM npcs;

INSERT INTO npc_locations (npc_id, node_id)
SELECT id, 'nodeDarkSpire' FROM npcs;
