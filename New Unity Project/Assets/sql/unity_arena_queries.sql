-- Queries used by the Unity ArenaManager

-- Mark characters as entering the arena
UPDATE characters SET in_arena=1 WHERE id=@charId;

-- Record battle result and free characters
INSERT INTO arena_battles(user_id, opponent_id, victory, reward_gold)
VALUES(@userId, @opponentId, @victory, @gold);
UPDATE characters SET in_arena=0 WHERE id=@charId;
