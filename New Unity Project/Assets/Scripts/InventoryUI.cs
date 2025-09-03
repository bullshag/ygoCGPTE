using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WinFormsApp2;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemListContent = null!;
    [SerializeField] private GameObject itemEntryPrefab = null!;
    [SerializeField] private Text descriptionText = null!;
    [SerializeField] private Dropdown targetDropdown = null!;
    [SerializeField] private Button useButton = null!;
    [SerializeField] private Text tooltipText = null!;
    [SerializeField] private int userId;

    private InventoryItem? selectedItem;

    private void Start()
    {
        InventoryService.Load(userId);
        PopulateItems();
        LoadTargets();
        useButton.onClick.AddListener(OnUseClicked);
        targetDropdown.onValueChanged.AddListener(_ => OnTargetChanged());
    }

    private void PopulateItems()
    {
        foreach (Transform child in itemListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (var inv in InventoryService.Items)
        {
            var go = Instantiate(itemEntryPrefab, itemListContent);
            var label = go.GetComponentInChildren<Text>();
            string suffix = inv.Item.Stackable ? $" x{inv.Quantity}" : string.Empty;
            label.text = inv.Item.Name + suffix;
            var button = go.GetComponent<Button>();
            var current = inv;
            button.onClick.AddListener(() => OnItemSelected(current));

            var trigger = go.GetComponent<EventTrigger>() ?? go.AddComponent<EventTrigger>();
            var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener(_ => tooltipText.text = DescribeItem(current.Item));
            trigger.triggers.Add(enter);
            var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener(_ => tooltipText.text = string.Empty);
            trigger.triggers.Add(exit);
        }
        OnItemSelected(null);
    }

    private void OnItemSelected(InventoryItem? inv)
    {
        selectedItem = inv;
        if (inv == null)
        {
            descriptionText.text = string.Empty;
            useButton.interactable = false;
            useButton.GetComponentInChildren<Text>().text = "Use";
        }
        else
        {
            descriptionText.text = DescribeItem(inv.Item);
            bool isEquipment = inv.Item is Weapon || inv.Item is Armor || inv.Item is Trinket;
            useButton.GetComponentInChildren<Text>().text = isEquipment ? "Equip" : "Use";
            useButton.interactable = targetDropdown.options.Count > 0 && targetDropdown.value >= 0 &&
                                    (isEquipment || inv.Item is HealingPotion || inv.Item is AbilityTome);
        }
    }

    private void LoadTargets()
    {
        targetDropdown.ClearOptions();
        var options = new List<string> { "Hero", "Mage" };
        targetDropdown.AddOptions(options);
        OnTargetChanged();
    }

    private void OnTargetChanged()
    {
        var item = selectedItem?.Item;
        bool isEquipment = item is Weapon || item is Armor || item is Trinket;
        useButton.GetComponentInChildren<Text>().text = isEquipment ? "Equip" : "Use";
        useButton.interactable = targetDropdown.options.Count > 0 && targetDropdown.value >= 0 &&
                                (isEquipment || item is HealingPotion || item is AbilityTome);
    }

    private void OnUseClicked()
    {
        if (selectedItem == null || targetDropdown.value < 0) return;
        string target = targetDropdown.options[targetDropdown.value].text;
        Item item = selectedItem.Item;
        if (item is HealingPotion)
        {
            InventoryService.RemoveItem(item);
            PopulateItems();
        }
        else if (item is AbilityTome)
        {
            InventoryService.RemoveItem(item);
            PopulateItems();
        }
        else if (item is Weapon || item is Armor || item is Trinket)
        {
            var slot = item.Slot ?? EquipmentSlot.Weapon;
            InventoryService.Equip(target, slot, item);
            InventoryService.RemoveItem(item);
            PopulateItems();
        }
    }

    private string DescribeItem(Item item)
    {
        var sb = new StringBuilder();
        sb.AppendLine(item.Description);
        if (item is Weapon w)
        {
            sb.AppendLine($"Scaling - STR {w.StrScaling:P0}, DEX {w.DexScaling:P0}, INT {w.IntScaling:P0}");
            sb.AppendLine($"Damage {w.MinMultiplier:0.##}x-{w.MaxMultiplier:0.##}x");
            if (w.CritChanceBonus != 0) sb.AppendLine($"Crit Chance +{w.CritChanceBonus:P0}");
            if (w.CritDamageBonus != 0) sb.AppendLine($"Crit Damage +{w.CritDamageBonus:P0}");
        }
        foreach (var kv in item.FlatBonuses)
        {
            sb.AppendLine($"+{kv.Value} {kv.Key}");
        }
        foreach (var kv in item.PercentBonuses)
        {
            sb.AppendLine($"+{kv.Value}% {kv.Key}");
        }
        return sb.ToString().Trim();
    }
}

