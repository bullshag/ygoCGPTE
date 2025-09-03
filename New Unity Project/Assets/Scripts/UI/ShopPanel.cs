using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using WinFormsApp2;

/// <summary>
/// Simple Unity UI layer that mirrors the WinForms ShopForm behaviour.
/// It relies on existing inventory and loot services to populate the shop
/// and perform buy/sell actions.
/// </summary>
public class ShopPanel : MonoBehaviour
{
    private List<Item> _shopItems = new();

    void Start()
    {
        // Load stock using the existing loot pool service
        _shopItems = LootPool.GetShopStock("default");
        InventoryService.Load(InventoryService.Items.Count); // ensure inventory ready
    }

    public void OnBuy(int index)
    {
        if (index < 0 || index >= _shopItems.Count) return;
        var proto = _shopItems[index];
        var item = InventoryService.CreateItem(proto.Name);
        if (item != null)
        {
            InventoryService.AddItem(item);
        }
    }

    public void OnSell(string itemName)
    {
        // Placeholder: full selling logic would mirror ShopForm.BtnSell_Click
        var inv = InventoryService.Items.FirstOrDefault(i => i.Item.Name == itemName);
        if (inv != null)
        {
            InventoryService.RemoveItem(inv.Item, 1);
        }
    }

    public void BackToMain() => SceneManager.LoadScene("MainRPG");
}
