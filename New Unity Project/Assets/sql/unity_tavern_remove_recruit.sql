-- Remove a specific recruit from a node's candidate pool
DELETE FROM unity_tavern_recruits
WHERE node_id = @nodeId
  AND recruit_id = @recruitId;
