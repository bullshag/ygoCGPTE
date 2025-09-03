-- Assign recruit to the player and remove from tavern
UPDATE characters
SET account_id = @userId,
    in_tavern = 0
WHERE id = @recruitId
  AND in_tavern = 1;
