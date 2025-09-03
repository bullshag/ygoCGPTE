-- Updates arena state and logs battles initiated from ArenaManager
UPDATE characters SET in_arena=1 WHERE account_id=@accountId;
INSERT INTO arena_battle_logs(attacker_id, defender_id, log)
VALUES(@accountId, @opponentId, @log);
