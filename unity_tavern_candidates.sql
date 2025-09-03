-- Fetch recruit candidates available in the tavern
SELECT id, name, level
FROM characters
WHERE in_tavern = 1
  AND (account_id IS NULL OR account_id = 0);
