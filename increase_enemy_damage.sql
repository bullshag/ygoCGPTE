UPDATE npcs
SET strength = CEIL(strength * 1.2),
    dex = CEIL(dex * 1.2),
    intelligence = CEIL(intelligence * 1.2);
