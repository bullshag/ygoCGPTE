SELECT account_id
FROM arena_teams
WHERE account_id <> @id
ORDER BY RAND()
LIMIT 1;
