using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp2
{
    public static class RegionData
    {
        public static readonly Dictionary<string, string> NodeToDisplay = new()
        {
            ["nodeMountain"] = "Mountain",
            ["nodeMounttown"] = "Mounttown",
            ["nodeDarkSpire"] = "Dark Spire",
            ["nodeNorthernIsland"] = "Northern Island",
            ["nodeSouthernIsland"] = "Southern Island",
            ["nodeRiverVillage"] = "River Village",
            ["nodeSmallVillage"] = "Small Village",
            ["nodeDesert"] = "Desert",
            ["nodeForestValley"] = "Forest Valley",
            ["nodeForestPlains"] = "Forest Plains",
            ["nodeFarCliffs"] = "Far Cliffs"
        };

        public static readonly Dictionary<string, string> KeyToDisplay =
            NodeToDisplay.ToDictionary(kv => kv.Key.Substring(4).ToLower(), kv => kv.Value);

        public static IEnumerable<string> Keys => KeyToDisplay.Keys;
    }
}
