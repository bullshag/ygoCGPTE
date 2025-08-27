using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class InventoryForm : Form
    {
        private readonly int _userId;
        private string? _selectedTarget;
        private readonly ToolTip _tip = new();

        public InventoryForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
            lstItems.DrawMode = DrawMode.OwnerDrawFixed;
            lstItems.DrawItem += LstItems_DrawItem;
            lstItems.MouseMove += LstItems_MouseMove;
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
                lstItems.Items.Add(inv);
            }
        }

        private Item? SelectedItem()
        {
            int index = lstItems.SelectedIndex;
            if (index < 0 || index >= InventoryService.Items.Count) return null;
            return InventoryService.Items[index].Item;
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
                lblDescription.Text = DescribeItem(item);
                btnUse.Enabled = item is HealingPotion && _selectedTarget != null;
            }
        }

        private void LstItems_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = lstItems.IndexFromPoint(e.Location);
            if (index >= 0 && index < InventoryService.Items.Count)
                _tip.Show(DescribeItem(InventoryService.Items[index].Item), lstItems, e.Location + new Size(15, 15));
            else
                _tip.Hide(lstItems);
        }

        private void btnUse_Click(object? sender, EventArgs e)
        {
            var item = SelectedItem();
            if (item == null || _selectedTarget == null) return;
            if (item is HealingPotion)
            {
                using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET current_hp = LEAST(max_hp, current_hp + @heal) WHERE account_id=@uid AND name=@name AND is_dead=0 AND in_arena=0", conn);
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
            using MySqlCommand cmd = new MySqlCommand("SELECT name FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0", conn);
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

        private void LstItems_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var inv = (InventoryItem)lstItems.Items[e.Index];
            var item = inv.Item;
            e.DrawBackground();
            string suffix = item.Stackable ? $" x{inv.Quantity}" : string.Empty;
            if (item.RainbowColors != null)
            {
                int x = e.Bounds.Left;
                var colors = item.RainbowColors;
                for (int i = 0; i < item.Name.Length; i++)
                {
                    var brush = new SolidBrush(colors[i % colors.Count]);
                    string ch = item.Name[i].ToString();
                    e.Graphics.DrawString(ch, e.Font, brush, x, e.Bounds.Top);
                    x += (int)e.Graphics.MeasureString(ch, e.Font).Width;
                }
                e.Graphics.DrawString(suffix, e.Font, Brushes.Black, x, e.Bounds.Top);
            }
            else
            {
                using var brush = new SolidBrush(item.NameColor);
                e.Graphics.DrawString(item.Name + suffix, e.Font, brush, e.Bounds);
            }
            e.DrawFocusRectangle();
        }
    }
}
