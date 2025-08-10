using System;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class InventoryForm : Form
    {
        private readonly int _userId;
        private string? _selectedTarget;

        public InventoryForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
        }

        private void InventoryForm_Load(object? sender, EventArgs e)
        {
            RefreshItems();
            LoadTargets();
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
                btnUse.Enabled = item is HealingPotion && _selectedTarget != null;
            }
        }

        private void btnUse_Click(object? sender, EventArgs e)
        {
            var item = SelectedItem();
            if (item == null || _selectedTarget == null) return;
            if (item is HealingPotion)
            {
                using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET current_hp = LEAST(max_hp, current_hp + @heal) WHERE account_id=@uid AND name=@name", conn);
                cmd.Parameters.AddWithValue("@heal", ((HealingPotion)item).HealAmount);
                cmd.Parameters.AddWithValue("@uid", _userId);
                cmd.Parameters.AddWithValue("@name", _selectedTarget);
                cmd.ExecuteNonQuery();
                InventoryService.RemoveItem(item);
                RefreshItems();
            }
        }

        private void LoadTargets()
        {
            cmbTarget.Items.Clear();
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT name FROM characters WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                cmbTarget.Items.Add(r.GetString("name"));
            }
        }

        private void cmbTarget_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _selectedTarget = cmbTarget.SelectedItem?.ToString();
            var item = SelectedItem();
            btnUse.Enabled = item is HealingPotion && _selectedTarget != null;
        }
    }
}
