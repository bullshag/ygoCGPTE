using System.Collections.Generic;

namespace WinFormsApp2
{
    /// <summary>
    /// Represents a node on the world map. Nodes contain connections to other
    /// nodes with associated travel time in days and a list of activities
    /// available at that location.
    /// </summary>
    public class WorldMapNode
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public Dictionary<string, int> Connections { get; } = new();
        public List<string> Activities { get; } = new();
        /// <summary>
        /// Minimum power of enemies encountered at this node, if any.
        /// </summary>
        public int? MinEnemyPower { get; set; }
        /// <summary>
        /// Maximum power of enemies encountered at this node, if any.
        /// </summary>
        public int? MaxEnemyPower { get; set; }

        public WorldMapNode(string id, string name, string description = "")
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
