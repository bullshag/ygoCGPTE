using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    /// <summary>
    /// Holds the static definition of all world map nodes and their
    /// connections/activities. This allows the navigation UI and travel system
    /// to query available destinations and actions.
    /// </summary>
    public static class WorldMapService
    {
        public static readonly Dictionary<string, WorldMapNode> Nodes;

        static WorldMapService()
        {
            Nodes = new Dictionary<string, WorldMapNode>();

            var nodeMountain = new WorldMapNode("nodeMountain", "Mountain", "Towering peaks home to dangerous beasts.");
            nodeMountain.Connections["nodeMounttown"] = 2;
            Nodes[nodeMountain.Id] = nodeMountain;

            var nodeMounttown = new WorldMapNode("nodeMounttown", "Mounttown", "A bustling town carved into the mountainside. Generally safe from wild enemies.");
            nodeMounttown.Connections["nodeMountain"] = 2;
            nodeMounttown.Connections["nodeDarkSpire"] = 1;
            nodeMounttown.Connections["nodeRiverVillage"] = 3;
            nodeMounttown.Activities.Add("Shop");
            nodeMounttown.Activities.Add("Temple (30 min +10% HP buff)");
            nodeMounttown.Activities.Add("Graveyard (resurrect screen)");
            nodeMounttown.Activities.Add("Tavern (recruit Lv5 adventurers w/2 random passives)");
            Nodes[nodeMounttown.Id] = nodeMounttown;

            var nodeDarkSpire = new WorldMapNode("nodeDarkSpire", "Dark Spire", "An ominous tower shrouded in eternal twilight. Foes grow stronger each floor.");
            nodeDarkSpire.Connections["nodeMounttown"] = 1;
            nodeDarkSpire.Connections["nodeRiverVillage"] = 3;
            nodeDarkSpire.Connections["nodeForestValley"] = 3;
            nodeDarkSpire.Activities.Add("Track floors cleared and reward bonus (15-20% for Lv15-20 floor)");
            Nodes[nodeDarkSpire.Id] = nodeDarkSpire;

            var nodeNorthernIsland = new WorldMapNode("nodeNorthernIsland", "Northern Island", "A remote island swept by cold winds.");
            nodeNorthernIsland.Connections["nodeDarkSpire"] = 3;
            nodeNorthernIsland.Connections["nodeForestValley"] = 4;
            nodeNorthernIsland.Activities.Add("Ancient Stone of Regret (reset stats to 5 for 150% hire value cost)");
            Nodes[nodeNorthernIsland.Id] = nodeNorthernIsland;

            var nodeSouthernIsland = new WorldMapNode("nodeSouthernIsland", "Southern Island", "A tropical island dotted with fishing huts.");
            nodeSouthernIsland.Connections["nodeSmallVillage"] = 10;
            nodeSouthernIsland.Activities.Add("Fisherman work: assign party member for N minutes → earns 5 gp/min");
            nodeSouthernIsland.Activities.Add("Tavern: hire hostile NPC mercenaries (no exp/level/equipment/resurrection)");
            nodeSouthernIsland.Activities.Add("Temple: blessing that reduces travel ≥2 days by 1 day");
            Nodes[nodeSouthernIsland.Id] = nodeSouthernIsland;

            var nodeRiverVillage = new WorldMapNode("nodeRiverVillage", "River Village", "A prosperous settlement along winding rivers.");
            nodeRiverVillage.Connections["nodeSmallVillage"] = 1;
            nodeRiverVillage.Connections["nodeDarkSpire"] = 3;
            nodeRiverVillage.Connections["nodeMounttown"] = 3;
            nodeRiverVillage.Connections["nodeDesert"] = 4;
            nodeRiverVillage.Connections["nodeForestValley"] = 4;
            nodeRiverVillage.Activities.Add("Battle Arena: leave party to fight other teams");
            nodeRiverVillage.Activities.Add("Tavern: recruit magic specialists and take quests");
            nodeRiverVillage.Activities.Add("Shop: strong/expensive items");
            nodeRiverVillage.Activities.Add("Wizard Tower: teleport to any node for cost");
            Nodes[nodeRiverVillage.Id] = nodeRiverVillage;

            var nodeSmallVillage = new WorldMapNode("nodeSmallVillage", "Small Village", "A quaint village surrounded by whispering woods.");
            nodeSmallVillage.Connections["nodeSouthernIsland"] = 10;
            nodeSmallVillage.Connections["nodeRiverVillage"] = 1;
            nodeSmallVillage.Activities.Add("Shop");
            nodeSmallVillage.Activities.Add("Tavern (recruit DEX specialists)");
            Nodes[nodeSmallVillage.Id] = nodeSmallVillage;

            var nodeDesert = new WorldMapNode("nodeDesert", "Desert", "An endless expanse of scorching sands.");
            nodeDesert.Connections["nodeForestValley"] = 4;
            nodeDesert.Connections["nodeFarCliffs"] = 5;
            nodeDesert.Connections["nodeForestPlains"] = 5;
            nodeDesert.Activities.Add("Wander the desert: spend 1 day, chance to encounter Lv45 giant worm raid boss");
            Nodes[nodeDesert.Id] = nodeDesert;

            var nodeForestValley = new WorldMapNode("nodeForestValley", "Forest Valley", "A lush valley teeming with hidden wildlife.");
            nodeForestValley.Connections["nodeDarkSpire"] = 3;
            nodeForestValley.Connections["nodeRiverVillage"] = 4;
            nodeForestValley.Connections["nodeForestPlains"] = 3;
            nodeForestValley.Connections["nodeDesert"] = 4;
            Nodes[nodeForestValley.Id] = nodeForestValley;

            var nodeForestPlains = new WorldMapNode("nodeForestPlains", "Forest Plains", "Open plains where the forest meets the sky.");
            nodeForestPlains.Connections["nodeFarCliffs"] = 1;
            nodeForestPlains.Connections["nodeDesert"] = 5;
            nodeForestPlains.Connections["nodeForestValley"] = 3;
            nodeForestPlains.Activities.Add("Commune with nature (receive raid-boss quest)");
            Nodes[nodeForestPlains.Id] = nodeForestPlains;

            var nodeFarCliffs = new WorldMapNode("nodeFarCliffs", "Far Cliffs", "Sheer cliffs that overlook the restless sea.");
            nodeFarCliffs.Connections["nodeForestPlains"] = 1;
            nodeFarCliffs.Connections["nodeDesert"] = 5;
            nodeFarCliffs.Activities.Add("Ancient Altar: does nothing unless holding 'Orb of Unknowable Evil'");
            Nodes[nodeFarCliffs.Id] = nodeFarCliffs;

            PopulateEnemyPowers();
        }

        private static void PopulateEnemyPowers()
        {
            try
            {
                using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                foreach (var node in Nodes.Values)
                {
                    var npcs = new List<(string name, int level)>();
                    using (var cmd = new MySqlCommand("SELECT n.name, n.level FROM npcs n JOIN npc_locations l ON n.name=l.npc_name WHERE l.node_id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", node.Id);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            npcs.Add((reader.GetString("name"), reader.GetInt32("level")));
                        }
                    }

                    int strongest = 0;
                    foreach (var (name, level) in npcs)
                    {
                        int power = PowerCalculator.CalculateNpcPower(conn, name, level);
                        if (power > strongest) strongest = power;
                    }

                    if (strongest > 0)
                    {
                        node.MinEnemyPower = strongest;
                        node.MaxEnemyPower = strongest * 4;
                        node.Description = $"{node.Description} Party Power {node.MinEnemyPower}-{node.MaxEnemyPower}.";
                        node.Activities.Add($"Search for enemies (Party Power {node.MinEnemyPower}-{node.MaxEnemyPower})");
                    }
                }
            }
            catch (Exception)
            {
                // If the database is unavailable, nodes simply lack enemy power data.
            }
        }

        public static WorldMapNode GetNode(string id) => Nodes[id];

        public static IEnumerable<(WorldMapNode node, int days)> GetConnections(string id)
        {
            var source = GetNode(id);
            foreach (var kv in source.Connections)
            {
                yield return (Nodes[kv.Key], kv.Value);
            }
        }
    }
}
