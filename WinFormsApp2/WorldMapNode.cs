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
        public Dictionary<string, int> Connections { get; } = new();
        public List<string> Activities { get; } = new();

        public WorldMapNode(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
