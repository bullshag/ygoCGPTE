using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using WinFormsApp2;

using UnityClient;
using MySqlConnector;

public class InventoryUI : MonoBehaviour
{
    public List<GameObject> itemEntrySlots = new();
    public TextMeshProUGUI descriptionText = null!;
    public TMP_Dropdown targetDropdown = null!;
    public Button useButton = null!;
    public TextMeshProUGUI tooltipText = null!;
    public int userId;

    private InventoryItem? selectedItem;

    private void Start()
    {
        InventoryServiceUnity.Load(userId);
        PopulateItems();
        LoadTargets();
        useButton.onClick.AddListener(OnUseClicked);
        targetDropdown.onValueChanged.AddListener(_ => OnTargetChanged());
    }

    private void PopulateItems()
    {
        for (int i = 0; i < itemEntrySlots.Count; i++)
        {
            var go = itemEntrySlots[i];
            if (i < InventoryServiceUnity.Items.Count)
            {
                go.SetActive(true);
                var inv = InventoryServiceUnity.Items[i];
                var label = go.GetComponentInChildren<TextMeshProUGUI>();
                string suffix = inv.Item.Stackable ? $" x{inv.Quantity}" : string.Empty;
                label.text = inv.Item.Name + suffix;
                var button = go.GetComponent<Button>();
                var current = inv;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnItemSelected(current));

                var trigger = go.GetComponent<EventTrigger>() ?? go.AddComponent<EventTrigger>();
                trigger.triggers.Clear();
                var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                enter.callback.AddListener(_ => tooltipText.text = DescribeItem(current.Item));
                trigger.triggers.Add(enter);
                var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                exit.callback.AddListener(_ => tooltipText.text = string.Empty);
                trigger.triggers.Add(exit);
            }
            else
            {
                go.SetActive(false);
            }
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
            useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
        }
        else
        {
            descriptionText.text = DescribeItem(inv.Item);
            bool isEquipment = inv.Item is Weapon || inv.Item is Armor || inv.Item is Trinket;
            useButton.GetComponentInChildren<TextMeshProUGUI>().text = isEquipment ? "Equip" : "Use";
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
        useButton.GetComponentInChildren<TextMeshProUGUI>().text = isEquipment ? "Equip" : "Use";
        useButton.interactable = targetDropdown.options.Count > 0 && targetDropdown.value >= 0 &&
                                (isEquipment || item is HealingPotion || item is AbilityTome);
    }

    private void OnUseClicked()
    {
        if (selectedItem == null || targetDropdown.value < 0) return;
        string target = targetDropdown.options[targetDropdown.value].text;
        Item item = selectedItem.Item;
        Debug.Log($"Using {item.Name} on {target}");
        if (item is HealingPotion potion)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfigUnity.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(
                "UPDATE characters SET current_hp = LEAST(max_hp, current_hp + @heal) " +
                "WHERE account_id=@uid AND name=@name AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
            cmd.Parameters.AddWithValue("@heal", potion.HealAmount);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@name", target);
            int result = cmd.ExecuteNonQuery();
            Debug.Log($"SQL rows affected: {result} for {item.Name} on {target}");
            InventoryServiceUnity.RemoveItem(item);
            PopulateItems();
        }
        else if (item is AbilityTome tome)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfigUnity.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(
                "INSERT IGNORE INTO character_abilities(character_id, ability_id) " +
                "SELECT id, @aid FROM characters WHERE account_id=@uid AND name=@name", conn);
            cmd.Parameters.AddWithValue("@aid", tome.AbilityId);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@name", target);
            int result = cmd.ExecuteNonQuery();
            Debug.Log($"SQL rows affected: {result} for {item.Name} on {target}");
            InventoryServiceUnity.RemoveItem(item);
            PopulateItems();
        }
        else if (item is Weapon || item is Armor || item is Trinket)
        {
            var slot = item.Slot ?? EquipmentSlot.Weapon;
            InventoryServiceUnity.Equip(target, slot, item);
            InventoryServiceUnity.RemoveItem(item);
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

