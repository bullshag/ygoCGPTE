using System;
using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp2
{
    /// <summary>
    /// Provides a global loot pool that is shared between shops and enemy drops.
    /// Items are grouped into broad level brackets and shop stock is generated
    /// without overlap so each shop offers unique wares.
    /// </summary>
    public static class LootPool
    {
        private static readonly Random _rng = new();

        // Pools keyed by (minLevel,maxLevel)
        private static readonly Dictionary<(int Min, int Max), List<string>> _pools = new()
        {
            [(1,10)] = new List<string>
            {
                "Healing Potion",
                "Dagger",
                "Shortsword",
                "Leather Armor",
                "Leather Cap",
                "Leather Boots",
                "Cloth Robe"
            },
            [(11,20)] = new List<string>
            {
                "Bow",
                "Staff",
                "Wand",
                "Longsword",
                "Plate Armor",
                "Rod",
                "Mace"
            },
            [(21,40)] = new List<string>
            {
                "Greataxe",
                "Scythe",
                "Greatsword",
                "Greatmaul"
            }
        };

        private static readonly HashSet<string> _usedItems = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, List<Item>> _shopStocks = new();

        private static List<string> GetPool(int level)
        {
            foreach (var kv in _pools)
            {
                if (level >= kv.Key.Min && level <= kv.Key.Max)
                    return kv.Value;
            }
            return _pools.First().Value;
        }

        /// <summary>
        /// Returns the stock for the given shop node. The stock is generated
        /// once and cached so repeated visits show the same items. Items are
        /// drawn from the global pool and are unique across shops.
        /// </summary>
        public static List<Item> GetShopStock(string nodeId)
        {
            if (_shopStocks.TryGetValue(nodeId, out var stock))
                return stock;

            int level = nodeId switch
            {
                "nodeSmallVillage" => 5,
                "nodeMounttown" => 15,
                "nodeRiverVillage" => 25,
                _ => 5
            };

            var pool = GetPool(level);
            var items = new List<Item>
            {
                // every shop sells at least one potion
                new HealingPotion()
            };

            // Track item names added to this shop to avoid duplicates within the shop
            var localUsed = new HashSet<string>(items.Select(i => i.Name), StringComparer.OrdinalIgnoreCase);

            while (items.Count < 15)
            {
                var available = pool.Where(n => !_usedItems.Contains(n) && !localUsed.Contains(n)).ToList();
                if (available.Count == 0)
                    break; // no more unique items to add

                string name = available[_rng.Next(available.Count)];
                _usedItems.Add(name);
                localUsed.Add(name);
                Item? item = InventoryService.CreateItem(name);
                if (item != null)
                    items.Add(item);
            }

            _shopStocks[nodeId] = items;
            return items;
        }

        /// <summary>
        /// Returns a random item suitable for an enemy of the given level.
        /// This does not affect shop stock and can return items already used
        /// by shops.
        /// </summary>
        public static Item? GetEnemyLoot(int level)
        {
            var pool = GetPool(level);
            if (pool.Count == 0)
                return null;
            string name = pool[_rng.Next(pool.Count)];
            return InventoryService.CreateItem(name);
        }
    }
}
