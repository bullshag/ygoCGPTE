-- Seed a baseline set of tavern recruits when none are marked as available
UPDATE characters
SET in_tavern = 1
WHERE in_tavern = 0
  AND (account_id IS NULL OR account_id = 0)
ORDER BY id
LIMIT @limit;
