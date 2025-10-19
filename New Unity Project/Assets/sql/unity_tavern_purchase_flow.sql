-- Deduct gold, assign the recruit, and flag the roster entry as purchased
UPDATE users
SET gold = gold - @cost
WHERE id = @userId
  AND gold >= @cost;

UPDATE characters
SET account_id = @userId,
    in_tavern = 0
WHERE id = @recruitId
  AND in_tavern = 1;

UPDATE unity_tavern_recruits
SET purchased_utc = UTC_TIMESTAMP(),
    purchased_account_id = @userId
WHERE node_id = @nodeId
  AND recruit_id = @recruitId
  AND purchased_utc IS NULL;
