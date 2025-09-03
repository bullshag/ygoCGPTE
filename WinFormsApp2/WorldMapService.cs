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

            var nodeMountain = new WorldMapNode("nodeMountain", "Mountain", "Towering peaks home to dangerous beasts. Enemies around level 10-20.")
            {
                MinEnemyLevel = 10,
                MaxEnemyLevel = 20
            };

            nodeMountain.Connections["nodeMounttown"] = 2;
            nodeMountain.Activities.Add("Search for enemies (Level 10-20)");

            Nodes[nodeMountain.Id] = nodeMountain;

            var nodeMounttown = new WorldMapNode("nodeMounttown", "Mounttown", "A bustling town carved into the mountainside. Generally safe from wild enemies.")
            {
                MinEnemyLevel = 5,
                MaxEnemyLevel = 15
            };

            nodeMounttown.Connections["nodeMountain"] = 2;
            nodeMounttown.Connections["nodeDarkSpire"] = 1;
            nodeMounttown.Connections["nodeRiverVillage"] = 3;
            nodeMounttown.Activities.Add("Shop");
            nodeMounttown.Activities.Add("Temple (30 min +10% HP buff)");
            nodeMounttown.Activities.Add("Graveyard (resurrect screen)");
            nodeMounttown.Activities.Add("Tavern (recruit party power 5 adventurers w/2 random passives)");
            nodeMounttown.Activities.Add("Search for enemies (Level 5-15)");
            Nodes[nodeMounttown.Id] = nodeMounttown;

            var nodeDarkSpire = new WorldMapNode("nodeDarkSpire", "Dark Spire", "An ominous tower shrouded in eternal twilight. Foes start around level 1-5 and grow stronger each floor.")
            {
                MinEnemyLevel = 1,
                MaxEnemyLevel = 999
            };

            nodeDarkSpire.Connections["nodeMounttown"] = 1;
            nodeDarkSpire.Connections["nodeRiverVillage"] = 3;
            nodeDarkSpire.Connections["nodeForestValley"] = 3;
            nodeDarkSpire.Activities.Add("Search for enemies (Level 1-5, +5 Power per win)");
            nodeDarkSpire.Activities.Add("Track floors cleared and reward bonus (15-20% for party power 15-20 floor)");
            Nodes[nodeDarkSpire.Id] = nodeDarkSpire;

            var nodeNorthernIsland = new WorldMapNode("nodeNorthernIsland", "Northern Island", "A remote island swept by cold winds. Home to formidable level 25-35 foes.")
            {
                MinEnemyLevel = 25,
                MaxEnemyLevel = 35
            };

            nodeNorthernIsland.Connections["nodeDarkSpire"] = 3;
            nodeNorthernIsland.Connections["nodeForestValley"] = 4;
            nodeNorthernIsland.Activities.Add("Ancient Stone of Regret (reset stats to 5 for 150% hire value cost)");
            nodeNorthernIsland.Activities.Add("Search for enemies (Level 25-35)");
            Nodes[nodeNorthernIsland.Id] = nodeNorthernIsland;

            var nodeSouthernIsland = new WorldMapNode("nodeSouthernIsland", "Southern Island", "A tropical island dotted with fishing huts. Dangerous foes roam at level 45-50.")
            {
                MinEnemyLevel = 45,
                MaxEnemyLevel = 50
            };

            nodeSouthernIsland.Connections["nodeSmallVillage"] = 10;
            nodeSouthernIsland.Activities.Add("Fisherman work: assign party member for N minutes → earns 5 gp/min");
            nodeSouthernIsland.Activities.Add("Tavern: hire hostile NPC mercenaries (no exp/power/equipment/resurrection)");
            nodeSouthernIsland.Activities.Add("Temple: blessing that reduces travel ≥2 days by 1 day");
            nodeSouthernIsland.Activities.Add("Search for enemies (Level 45-50)");
            Nodes[nodeSouthernIsland.Id] = nodeSouthernIsland;

            var nodeRiverVillage = new WorldMapNode("nodeRiverVillage", "River Village", "A prosperous settlement along winding rivers. Nearby foes span a wide range of levels.");

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

            var nodeSmallVillage = new WorldMapNode("nodeSmallVillage", "Small Village", "A quaint village surrounded by whispering woods. Local enemies range from level 1-10.")
            {
                MinEnemyLevel = 1,
                MaxEnemyLevel = 10
            };

            nodeSmallVillage.Connections["nodeSouthernIsland"] = 10;
            nodeSmallVillage.Connections["nodeRiverVillage"] = 1;
            nodeSmallVillage.Activities.Add("Shop");
            nodeSmallVillage.Activities.Add("Tavern (recruit DEX specialists)");
            nodeSmallVillage.Activities.Add("Search for enemies (Level 1-10)");

            Nodes[nodeSmallVillage.Id] = nodeSmallVillage;

            var nodeDesert = new WorldMapNode("nodeDesert", "Desert", "An endless expanse of scorching sands. Enemies range around level 20-45.")
            {
                MinEnemyLevel = 20,
                MaxEnemyLevel = 45
            };

            nodeDesert.Connections["nodeForestValley"] = 4;
            nodeDesert.Connections["nodeFarCliffs"] = 5;
            nodeDesert.Connections["nodeForestPlains"] = 5;
            nodeDesert.Activities.Add("Wander the desert: spend 1 day, chance to encounter power 45 giant worm raid boss");
            nodeDesert.Activities.Add("Search for enemies (Level 20-45)");

            Nodes[nodeDesert.Id] = nodeDesert;

            var nodeForestValley = new WorldMapNode("nodeForestValley", "Forest Valley", "A lush valley teeming with hidden wildlife. Expect enemies around level 5-15.")
            {
                MinEnemyLevel = 5,
                MaxEnemyLevel = 15
            };

            nodeForestValley.Connections["nodeDarkSpire"] = 3;
            nodeForestValley.Connections["nodeRiverVillage"] = 4;
            nodeForestValley.Connections["nodeForestPlains"] = 3;
            nodeForestValley.Connections["nodeDesert"] = 4;
            nodeForestValley.Activities.Add("Search for enemies (Level 5-15)");

            Nodes[nodeForestValley.Id] = nodeForestValley;

            var nodeForestPlains = new WorldMapNode("nodeForestPlains", "Forest Plains", "Open plains where the forest meets the sky. Enemies generally level 15-25.")
            {
                MinEnemyLevel = 15,
                MaxEnemyLevel = 25
            };

            nodeForestPlains.Connections["nodeFarCliffs"] = 1;
            nodeForestPlains.Connections["nodeDesert"] = 5;
            nodeForestPlains.Connections["nodeForestValley"] = 3;
            nodeForestPlains.Activities.Add("Commune with nature (receive raid-boss quest)");
            nodeForestPlains.Activities.Add("Search for enemies (Level 15-25)");
            Nodes[nodeForestPlains.Id] = nodeForestPlains;

            var nodeFarCliffs = new WorldMapNode("nodeFarCliffs", "Far Cliffs", "Sheer cliffs that overlook the restless sea. Local foes range around level 30-40.")
            {
                MinEnemyLevel = 30,
                MaxEnemyLevel = 40
            };

            nodeFarCliffs.Connections["nodeForestPlains"] = 1;
            nodeFarCliffs.Connections["nodeDesert"] = 5;
            nodeFarCliffs.Activities.Add("Ancient Altar: does nothing unless holding 'Orb of Unknowable Evil'");
            nodeFarCliffs.Activities.Add("Search for enemies (Level 30-40)");
            Nodes[nodeFarCliffs.Id] = nodeFarCliffs;

            PopulateEnemyLevels();
        }

        private static void PopulateEnemyLevels()
        {
            try
            {
                using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                foreach (var node in Nodes.Values)
                {
                    int minLevel = int.MaxValue;
                    int maxLevel = 0;
                    using (var cmd = new MySqlCommand("SELECT n.level FROM npcs n JOIN npc_locations l ON n.name=l.npc_name WHERE l.node_id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", node.Id);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int level = reader.GetInt32("level");
                            if (level < minLevel) minLevel = level;
                            if (level > maxLevel) maxLevel = level;
                        }
                    }

                    if (maxLevel > 0)
                    {
                        node.MinEnemyLevel = minLevel;
                        node.MaxEnemyLevel = maxLevel;
                        node.Description = $"{node.Description} Levels {node.MinEnemyLevel}-{node.MaxEnemyLevel}.";
                        node.Activities.Add($"Search for enemies (Level {node.MinEnemyLevel}-{node.MaxEnemyLevel})");
                    }
                }
            }
            catch (Exception)
            {
                // If the database is unavailable, nodes simply lack enemy level data.
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
