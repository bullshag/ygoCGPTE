USE accounts;

UPDATE npcs
SET max_hp = CASE
        WHEN level BETWEEN 1 AND 25 THEN CEIL(max_hp * 0.5)
        WHEN level > 25 THEN CEIL(max_hp * 0.8)
        ELSE max_hp
    END,
    current_hp = CASE
        WHEN level BETWEEN 1 AND 25 THEN CEIL(current_hp * 0.5)
        WHEN level > 25 THEN CEIL(current_hp * 0.8)
        ELSE current_hp
    END,
    strength = CEIL(strength * 1.3),
    dex = CEIL(dex * 1.3),
    intelligence = CEIL(intelligence * 1.3);

UPDATE characters
SET strength = CEIL(strength * 1.3),
    dex = CEIL(dex * 1.3),
    intelligence = CEIL(intelligence * 1.3);
