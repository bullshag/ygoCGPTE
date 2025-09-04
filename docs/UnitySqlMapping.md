# Unity Inventory SQL Mapping

This document maps each Unity inventory SQL file to the WinForms `InventoryService` method it mirrors.

| Unity SQL file | Corresponding WinForms method |
|---------------|--------------------------------|
| `unity_inventory_load.sql` | `InventoryService.LoadAsync` (items and equipment retrieval) |
| `unity_inventory_add.sql` | `InventoryService.AddItem` |
| `unity_inventory_remove.sql` | `InventoryService.RemoveItem` |
| `unity_inventory_save_equipment.sql` | `InventoryService.SaveEquipment` |
