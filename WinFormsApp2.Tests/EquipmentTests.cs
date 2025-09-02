using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using WinFormsApp2;
using Xunit;

namespace WinFormsApp2.Tests;

public class EquipmentTests
{
    private static void ResetInventoryService()
    {
        var itemsField = typeof(InventoryService).GetField("_items", BindingFlags.NonPublic | BindingFlags.Static);
        ((List<InventoryItem>)itemsField!.GetValue(null)!).Clear();
        var equipField = typeof(InventoryService).GetField("_equipment", BindingFlags.NonPublic | BindingFlags.Static);
        ((Dictionary<string, Dictionary<EquipmentSlot, Item?>>)equipField!.GetValue(null)!).Clear();
        var loadedField = typeof(InventoryService).GetField("_loaded", BindingFlags.NonPublic | BindingFlags.Static);
        loadedField!.SetValue(null, false);
    }

    [Fact]
    public void EquipItemRemovesFromInventory()
    {
        ResetInventoryService();
        var sword = new Weapon { Name = "Sword", Slot = EquipmentSlot.RightHand, Stackable = false };
        InventoryService.AddItem(sword);
        InventoryService.Equip("Hero", EquipmentSlot.RightHand, sword);
        Assert.Empty(InventoryService.Items);
        Assert.Equal(sword, InventoryService.GetEquippedItem("Hero", EquipmentSlot.RightHand));
    }

    [Fact]
    public void EquipReplacesExistingGear()
    {
        ResetInventoryService();
        var oldSword = new Weapon { Name = "OldSword", Slot = EquipmentSlot.RightHand, Stackable = false };
        var newSword = new Weapon { Name = "NewSword", Slot = EquipmentSlot.RightHand, Stackable = false };
        InventoryService.AddItem(oldSword);
        InventoryService.Equip("Hero", EquipmentSlot.RightHand, oldSword);
        InventoryService.AddItem(newSword);
        InventoryService.Equip("Hero", EquipmentSlot.RightHand, newSword);
        Assert.Single(InventoryService.Items);
        Assert.Equal("OldSword", InventoryService.Items[0].Item.Name);
        Assert.Equal(newSword, InventoryService.GetEquippedItem("Hero", EquipmentSlot.RightHand));
    }

    private class TestInventoryForm : InventoryForm
    {
        public string? LastMessage;
        public TestInventoryForm() : base(0) {}
        protected override void ShowMessage(string text) => LastMessage = text;
        public void TestSelectAndEquip(Item item, string target)
        {
            InventoryService.AddItem(item);
            var refresh = typeof(InventoryForm).GetMethod("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance);
            refresh!.Invoke(this, null);
            var listBox = (ListBox)typeof(InventoryForm).GetField("lstItems", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(this)!;
            listBox.SelectedIndex = 0;
            typeof(InventoryForm).GetField("_selectedTarget", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(this, target);
            var use = typeof(InventoryForm).GetMethod("btnUse_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            use!.Invoke(this, new object?[] { null, EventArgs.Empty });
        }
    }

    [Fact]
    public void EquipShowsMessage()
    {
        ResetInventoryService();
        var form = new TestInventoryForm();
        var sword = new Weapon { Name = "Sword", Slot = EquipmentSlot.RightHand, Stackable = false };
        form.TestSelectAndEquip(sword, "Hero");
        Assert.Equal("Equipped Sword to Hero", form.LastMessage);
    }
}
