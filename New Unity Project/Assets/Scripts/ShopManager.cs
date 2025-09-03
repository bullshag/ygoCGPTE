using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityClient;

/// <summary>
/// Handles server-backed shop interactions for the Unity client.
/// Logic mirrors the WinForms ShopForm queries and inventory updates.
/// </summary>
public class ShopManager : MonoBehaviour
{
    private string BaseUrl => DatabaseConfigUnity.ApiBaseUrl;

    /// <summary>
    /// Fetch the list of items for sale at a given node.
    /// </summary>
    public async Task<List<ShopItem>> GetStockAsync(string nodeId)
    {
        using var req = UnityWebRequest.Get($"{BaseUrl}/shop/stock?nodeId={nodeId}");
        await req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
            return new List<ShopItem>();
        return JsonUtility.FromJson<ShopItemList>(req.downloadHandler.text).items;
    }

    /// <summary>
    /// Purchase an item for the given user.
    /// </summary>
    public async Task<bool> PurchaseAsync(int userId, int itemId)
    {
        var form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("itemId", itemId);
        using var req = UnityWebRequest.Post($"{BaseUrl}/shop/purchase", form);
        await req.SendWebRequest();
        return req.result == UnityWebRequest.Result.Success;
    }

    /// <summary>
    /// Sell an item from the player's inventory.
    /// </summary>
    public async Task<bool> SellAsync(int userId, int itemId, int quantity)
    {
        var form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("itemId", itemId);
        form.AddField("quantity", quantity);
        using var req = UnityWebRequest.Post($"{BaseUrl}/shop/sell", form);
        await req.SendWebRequest();
        return req.result == UnityWebRequest.Result.Success;
    }

    [System.Serializable]
    public class ShopItem
    {
        public int id;
        public string name;
        public int price;
    }

    [System.Serializable]
    private class ShopItemList
    {
        public List<ShopItem> items;
    }
}
