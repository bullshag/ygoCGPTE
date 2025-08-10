using System;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class InventoryForm : Form
    {
        public InventoryForm()
        {
            InitializeComponent();
        }

        private void InventoryForm_Load(object? sender, EventArgs e)
        {
            RefreshItems();
        }

        private void RefreshItems()
        {
            lstItems.Items.Clear();
            foreach (var inv in InventoryService.Items)
            {
                string name = inv.Item.Name;
                if (inv.Item.Stackable)
                {
                    name += $" x{inv.Quantity}";
                }
                lstItems.Items.Add(name);
            }
        }

        private Item? SelectedItem()
        {
            int index = lstItems.SelectedIndex;
            if (index < 0 || index >= InventoryService.Items.Count) return null;
            return InventoryService.Items[index].Item;
        }

        private void lstItems_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var item = SelectedItem();
            if (item == null)
            {
                lblDescription.Text = string.Empty;
                btnUse.Enabled = false;
            }
            else
            {
                lblDescription.Text = item.Description;
                btnUse.Enabled = item is HealingPotion;
            }
        }

        private void btnUse_Click(object? sender, EventArgs e)
        {
            var item = SelectedItem();
            if (item == null) return;
            if (item is HealingPotion)
            {
                // use potion on first party member? For simplicity, remove from inventory.
                InventoryService.RemoveItem(item);
                RefreshItems();
            }
        }
    }
}
