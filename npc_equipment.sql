USE accounts;

CREATE TABLE IF NOT EXISTS npc_equipment (
    npc_name VARCHAR(255) NOT NULL,
    slot VARCHAR(50) NOT NULL,
    item_name VARCHAR(255) NOT NULL
);

INSERT INTO npc_equipment (npc_name, slot, item_name)
SELECT name, 'LeftHand',
       CASE
           WHEN level <= 5 THEN 'Dagger'
           WHEN level <= 10 THEN 'Shortsword'
           WHEN level <= 15 THEN 'Longsword'
           WHEN level <= 20 THEN 'Greatsword'
           ELSE 'Greatsword'
       END
FROM npcs;

INSERT INTO npc_equipment (npc_name, slot, item_name)
SELECT name, 'Body',
       CASE
           WHEN level <= 5 THEN 'Cloth Robe'
           WHEN level <= 10 THEN 'Leather Armor'
           ELSE 'Plate Armor'
       END
FROM npcs;
