using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WinFormsApp2;

/// <summary>
/// Simple Unity UI layer that mirrors the WinForms ShopForm behaviour.
/// It relies on existing inventory and loot services to populate the shop
/// and perform buy/sell actions.
/// </summary>
public class ShopPanel : MonoBehaviour
{
    [SerializeField] private int userId;
    [SerializeField] private string nodeId = "default";

    private List<ShopManager.ShopItem> _stock = new();
    private ShopManager? _shopManager;

    private async void Start()
    {
        _shopManager = FindObjectOfType<ShopManager>();
        InventoryServiceUnity.Load(userId);
        await Refresh();
    }

    public async Task Refresh()
    {
        if (_shopManager == null)
            _shopManager = FindObjectOfType<ShopManager>();

        _stock = await _shopManager.GetStockAsync(nodeId);
        RefreshUI();
    }

    private void RefreshUI()
    {
        // Update visual elements to reflect _stock contents as needed by the scene.
    }

    public async void OnBuy(int index)
    {
        if (index < 0 || index >= _stock.Count) return;
        var itemData = _stock[index];
        if (await _shopManager!.PurchaseAsync(userId, itemData.id))
        {
            var item = InventoryServiceUnity.CreateItem(itemData.name);
            if (item != null)
                InventoryServiceUnity.AddItem(item);
            await Refresh();
        }
    }

    public async void OnSell(string itemName, int quantity = 1)
    {
        var inv = InventoryServiceUnity.Items.FirstOrDefault(i => i.Item.Name == itemName);
        if (inv == null) return;

        int itemId = _stock.FirstOrDefault(s => s.name == itemName)?.id ?? 0;
        if (await _shopManager!.SellAsync(userId, itemId, quantity))
        {
            InventoryServiceUnity.RemoveItem(inv.Item, quantity);
            await Refresh();
        }
    }

    public void BackToMain() => MainRPGNavigation.OpenMain();
}
