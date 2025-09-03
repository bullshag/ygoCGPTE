using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles server-backed shop interactions for the Unity client.
/// Logic mirrors the WinForms ShopForm queries and inventory updates.
/// </summary>
public class ShopManager : MonoBehaviour
{
    private readonly Dictionary<int, int> _itemPrices = new();

    /// <summary>
    /// Fetch the list of items for sale at a given node.
    /// </summary>
    public async Task<List<ShopItem>> GetStockAsync(string nodeId)
    {
        string sqlPath = Path.Combine(AppContext.BaseDirectory, "unity_shop_stock.sql");
        var rows = await DatabaseClientUnity.QueryAsync(
            File.ReadAllText(sqlPath),
            new Dictionary<string, object?> { ["@nodeId"] = nodeId });

        var items = new List<ShopItem>();
        _itemPrices.Clear();
        foreach (var row in rows)
        {
            int id = Convert.ToInt32(row["id"]);
            string name = Convert.ToString(row["name"]) ?? string.Empty;
            int price = Convert.ToInt32(row["price"]);
            Debug.Log($"Stock item {id} price {price}");
            items.Add(new ShopItem { id = id, name = name, price = price });
            _itemPrices[id] = price;
        }
        return items;
    }

    /// <summary>
    /// Purchase an item for the given user.
    /// </summary>
    public async Task<bool> PurchaseAsync(int userId, int itemId)
    {
        _itemPrices.TryGetValue(itemId, out var price);
        Debug.Log($"Attempting purchase of item {itemId} for {price}");

        string sqlPath = Path.Combine(AppContext.BaseDirectory, "unity_shop_purchase.sql");
        var affected = await DatabaseClientUnity.ExecuteAsync(
            File.ReadAllText(sqlPath),
            new Dictionary<string, object?>
            {
                ["@userId"] = userId,
                ["@itemId"] = itemId,
                ["@price"] = price
            });
        bool success = affected > 0;
        Debug.Log($"Purchase {(success ? "succeeded" : "failed")} for item {itemId} at {price}");
        return success;
    }

    /// <summary>
    /// Sell an item from the player's inventory.
    /// </summary>
    public async Task<bool> SellAsync(int userId, int itemId, int quantity)
    {
        _itemPrices.TryGetValue(itemId, out var price);
        Debug.Log($"Attempting sell of item {itemId} x{quantity} for {price}");

        string sqlPath = Path.Combine(AppContext.BaseDirectory, "unity_shop_sell.sql");
        var affected = await DatabaseClientUnity.ExecuteAsync(
            File.ReadAllText(sqlPath),
            new Dictionary<string, object?>
            {
                ["@userId"] = userId,
                ["@itemId"] = itemId,
                ["@price"] = price,
                ["@quantity"] = quantity
            });
        bool success = affected > 0;
        Debug.Log($"Sell {(success ? "succeeded" : "failed")} for item {itemId} at {price} x{quantity}");
        return success;
    }

    [System.Serializable]
    public class ShopItem
    {
        public int id;
        public string name;
        public int price;
    }

}
