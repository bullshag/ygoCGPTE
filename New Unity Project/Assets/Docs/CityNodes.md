# City Nodes

City nodes define interactive areas for cities on the world map.

## Requirements
- GameObject is tagged `CityNode`.
- `CityNodeData` component attached with a radius and city ID.
- `SphereCollider` set as trigger with radius matching `CityNodeData`.
- When the player enters the radius, `CityInteraction` UI panel enables; it disables on exit.
