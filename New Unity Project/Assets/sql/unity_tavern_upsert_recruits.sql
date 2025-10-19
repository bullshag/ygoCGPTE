-- Persist or refresh a generated tavern recruit for a node
INSERT INTO unity_tavern_recruits (
    node_id,
    recruit_id,
    cost,
    created_utc,
    strength,
    dexterity,
    intelligence,
    max_hp,
    max_mp,
    action_speed,
    physical_defense,
    magic_defense,
    rolled_points,
    purchased_utc,
    purchased_account_id
)
VALUES (
    @nodeId,
    @recruitId,
    @cost,
    @createdUtc,
    @strength,
    @dexterity,
    @intelligence,
    @maxHp,
    @maxMp,
    @actionSpeed,
    @physicalDefense,
    @magicDefense,
    @rolledPoints,
    NULL,
    NULL
)
ON DUPLICATE KEY UPDATE
    cost = VALUES(cost),
    created_utc = VALUES(created_utc),
    strength = VALUES(strength),
    dexterity = VALUES(dexterity),
    intelligence = VALUES(intelligence),
    max_hp = VALUES(max_hp),
    max_mp = VALUES(max_mp),
    action_speed = VALUES(action_speed),
    physical_defense = VALUES(physical_defense),
    magic_defense = VALUES(magic_defense),
    rolled_points = VALUES(rolled_points),
    purchased_utc = NULL,
    purchased_account_id = NULL;
