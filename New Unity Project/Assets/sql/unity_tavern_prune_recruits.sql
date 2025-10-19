-- Remove stale or purchased tavern recruits for a node
DELETE FROM unity_tavern_recruits
WHERE node_id = @nodeId
  AND (
        purchased_utc IS NOT NULL
        OR created_utc < (UTC_TIMESTAMP() - INTERVAL 24 HOUR)
        OR @forceReset = 1
      );
