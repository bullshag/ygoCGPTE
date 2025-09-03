using System;
using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp2
{
    public static class LootPool
    {
        private static readonly Random _rng = new();

        private static readonly Dictionary<string, List<string>> _areaPools = RegionData.NodeToDisplay.ToDictionary(
            kv => kv.Key,
            kv => CreateRegionalPool(kv.Value)
        );

        private static readonly List<string> _baseItems = new()
        {
            "Cloth Robe",
            "Leather Armor",
            "Leather Cap",
            "Leather Boots",
            "Plate Armor",
            "Heavy Shield",
            "Shortsword",
            "Dagger",
            "Bow",
            "Longsword",
            "Staff",
            "Wand",
            "Rod",
            "Greataxe",
            "Scythe",
            "Greatsword",
            "Mace",
            "Greatmaul"
        };

        private static List<string> CreateRegionalPool(string regionDisplay)
        {
            var items = new List<string>();
            for (int i = 1; i <= 3; i++)
            {
                items.Add($"{regionDisplay} Cloth +{i}");
                items.Add($"{regionDisplay} Leather +{i}");
                items.Add($"{regionDisplay} Plate +{i}");
                items.Add($"{regionDisplay} Sword +{i}");
            }
            return items;
        }

        private static readonly Dictionary<string, HashSet<string>> _usedItems = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, List<Item>> _shopStocks = new();

        private static List<string> GetPool(string nodeId)
        {
            if (_areaPools.TryGetValue(nodeId, out var pool))
                return pool;
            return _areaPools.Values.First();
        }

        public static List<Item> GetShopStock(string nodeId)
        {
            if (_shopStocks.TryGetValue(nodeId, out var stock))
                return stock;

            var pool = _baseItems;
            if (!_usedItems.TryGetValue(nodeId, out var used))
                _usedItems[nodeId] = used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var items = new List<Item>
            {
                new HealingPotion()
            };
            var localUsed = new HashSet<string>(items.Select(i => i.Name), StringComparer.OrdinalIgnoreCase);

            while (items.Count < 15)
            {
                var available = pool.Where(n => !used.Contains(n) && !localUsed.Contains(n)).ToList();
                if (available.Count == 0)
                    break;
                string name = available[_rng.Next(available.Count)];
                used.Add(name);
                localUsed.Add(name);
                Item? item = InventoryService.CreateItem(name);
                if (item != null)
                    items.Add(item);
            }

            _shopStocks[nodeId] = items;
            return items;
        }

        public static Item? GetEnemyLoot(string? nodeId)
        {
            var pool = GetPool(nodeId ?? RegionData.NodeToDisplay.Keys.First());
            if (pool.Count == 0)
                return null;
            string name = pool[_rng.Next(pool.Count)];
            return InventoryService.CreateItem(name);
        }
    }
}
