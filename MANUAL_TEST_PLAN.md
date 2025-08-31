# Battle Encounter Manual Test Plan

## 1. Player-initiated search scaling
1. Launch the game and create a party of known total level (e.g., 50).
2. Travel to an area with a matching level bracket (area min = 50).
3. Initiate a battle search.
4. Observe the enemy party level total from the battle log or debugger.
5. Verify the total enemy level lies between `ceil(totalLevel * 0.8)` and `ceil(totalLevel * 1.2)` (40–60 in this example).

## 2. Under-leveled party enforcement
1. Use a party with total level lower than the area's minimum (e.g., total 40, area min 50).
2. Initiate a battle search in that area.
3. Observe the enemy party level total.
4. Confirm the enemies' combined level is approximately `ceil(areaMin * 1.2)` (60 in this example) regardless of the party's level.
