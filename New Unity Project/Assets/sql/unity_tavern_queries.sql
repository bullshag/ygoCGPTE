-- Queries used by the Unity TavernManager

-- Check player gold
SELECT gold FROM users WHERE id=@userId;

-- Hire a recruit and add to party
UPDATE users SET gold = GREATEST(gold - @cost, 0) WHERE id=@userId;
UPDATE characters SET account_id=@userId, in_tavern=0 WHERE id=@recruitId;
