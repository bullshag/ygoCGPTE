-- Insert template tavern recruits when no neutral characters are available for seeding
INSERT INTO characters (
    account_id,
    name,
    current_hp,
    max_hp,
    mana,
    max_mana,
    experience_points,
    action_speed,
    strength,
    dex,
    intelligence,
    melee_defense,
    magic_defense,
    level,
    skill_points,
    in_tavern
)
SELECT
    NULL,
    fallback.name,
    fallback.max_hp,
    fallback.max_hp,
    fallback.max_mp,
    fallback.max_mp,
    0,
    fallback.action_speed,
    fallback.strength,
    fallback.dex,
    fallback.intelligence,
    fallback.melee_defense,
    fallback.magic_defense,
    fallback.level,
    0,
    1
FROM (
    SELECT 'Aspirant Mara' AS name, 38 AS max_hp, 30 AS max_mp, 10 AS action_speed, 6 AS strength, 5 AS dex, 4 AS intelligence, 1 AS melee_defense, 0 AS magic_defense, 1 AS level
    UNION ALL
    SELECT 'Scout Bronn', 34, 28, 11, 5, 7, 4, 0, 1, 1
    UNION ALL
    SELECT 'Acolyte Nia', 30, 36, 10, 4, 4, 7, 0, 1, 1
    UNION ALL
    SELECT 'Sentry Kellan', 42, 24, 9, 7, 4, 3, 2, 0, 1
    UNION ALL
    SELECT 'Channeler Ise', 28, 40, 10, 3, 4, 8, 0, 2, 1
    UNION ALL
    SELECT 'Veteran Orik', 40, 26, 11, 6, 5, 4, 2, 1, 1
) AS fallback
WHERE NOT EXISTS (
    SELECT 1
    FROM characters c
    WHERE c.name = fallback.name
);
