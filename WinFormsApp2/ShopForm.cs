using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class ShopForm : Form
    {
        private readonly int _userId;
        private int _playerGold;
        private readonly List<Item> _shopItems = new()
        {
            new HealingPotion(),
            WeaponFactory.Create("dagger"),
            WeaponFactory.Create("shortsword"),
            WeaponFactory.Create("bow"),
            WeaponFactory.Create("staff"),
            WeaponFactory.Create("wand"),
            ArmorFactory.Create("leatherarmor"),
            ArmorFactory.Create("leathercap"),
            ArmorFactory.Create("leatherboots"),
            ArmorFactory.Create("clothrobe"),
            ArmorFactory.Create("platearmor")
        };

        private readonly ToolTip _tip = new();

        public ShopForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
            _btnBuy.Click += BtnBuy_Click;
            _btnSell.Click += BtnSell_Click;
            Load += ShopForm_Load;
            _lstShop.DrawMode = DrawMode.OwnerDrawFixed;
            _lstShop.DrawItem += LstShop_DrawItem;
            _lstInventory.DrawMode = DrawMode.OwnerDrawFixed;
            _lstInventory.DrawItem += LstInventory_DrawItem;
            _lstShop.SelectedIndexChanged += (s, e) => ShowShopDesc();
            _lstInventory.SelectedIndexChanged += (s, e) => ShowInventoryDesc();
            _lstShop.MouseMove += LstShop_MouseMove;
            _lstInventory.MouseMove += LstInventory_MouseMove;
        }

        private void ShopForm_Load(object? sender, EventArgs e)
        {
            RefreshGold();
            RefreshShop();
            RefreshInventory();
        }

        private void RefreshGold()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            object? result = cmd.ExecuteScalar();
            _playerGold = result == null ? 0 : Convert.ToInt32(result);
            _lblGold.Text = $"Gold: {_playerGold}";
        }

        private void RefreshShop()
        {
            _lstShop.Items.Clear();
            foreach (var item in _shopItems)
            {
                _lstShop.Items.Add(item);
            }
        }

        private void RefreshInventory()
        {
            _lstInventory.Items.Clear();
            foreach (var inv in InventoryService.Items)
            {
                _lstInventory.Items.Add(inv);
            }
        }

        private void ShowShopDesc()
        {
            int index = _lstShop.SelectedIndex;
            if (index >= 0 && index < _shopItems.Count)
                _lblDescription.Text = DescribeItem(_shopItems[index]);
            else
                _lblDescription.Text = string.Empty;
        }

        private void ShowInventoryDesc()
        {
            int index = _lstInventory.SelectedIndex;
            if (index >= 0 && index < InventoryService.Items.Count)
                _lblDescription.Text = DescribeItem(InventoryService.Items[index].Item);
            else
                _lblDescription.Text = string.Empty;
        }

        private string DescribeItem(Item item)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(item.Description);
            if (item is Weapon w)
            {
                sb.AppendLine($"Scaling - STR {w.StrScaling:P0}, DEX {w.DexScaling:P0}, INT {w.IntScaling:P0}");
                sb.AppendLine($"Damage {w.MinMultiplier:0.##}x-{w.MaxMultiplier:0.##}x");
                if (w.CritChanceBonus != 0) sb.AppendLine($"Crit Chance +{w.CritChanceBonus:P0}");
                if (w.CritDamageBonus != 0) sb.AppendLine($"Crit Damage +{w.CritDamageBonus:P0}");
            }
            foreach (var kv in item.FlatBonuses)
                sb.AppendLine($"+{kv.Value} {kv.Key}");
            foreach (var kv in item.PercentBonuses)
                sb.AppendLine($"+{kv.Value}% {kv.Key}");
            return sb.ToString().Trim();
        }

        private void LstShop_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = _lstShop.IndexFromPoint(e.Location);
            if (index >= 0 && index < _shopItems.Count)
                _tip.Show(DescribeItem(_shopItems[index]), _lstShop, e.Location + new Size(15, 15));
            else
                _tip.Hide(_lstShop);
        }

        private void LstInventory_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = _lstInventory.IndexFromPoint(e.Location);
            if (index >= 0 && index < InventoryService.Items.Count)
                _tip.Show(DescribeItem(InventoryService.Items[index].Item), _lstInventory, e.Location + new Size(15, 15));
            else
                _tip.Hide(_lstInventory);
        }

        private void BtnBuy_Click(object? sender, EventArgs e)
        {
            int index = _lstShop.SelectedIndex;
            if (index < 0 || index >= _shopItems.Count) return;
            var proto = _shopItems[index];
            if (_playerGold < proto.Price)
            {
                MessageBox.Show("Not enough gold");
                return;
            }
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE users SET gold = gold - @cost WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@cost", proto.Price);
            cmd.Parameters.AddWithValue("@id", _userId);
            cmd.ExecuteNonQuery();
            var item = InventoryService.CreateItem(proto.Name);
            if (item != null) InventoryService.AddItem(item);
            RefreshGold();
            RefreshInventory();
        }



        private void BtnSell_Click(object? sender, EventArgs e)
        {
            int index = _lstInventory.SelectedIndex;
            if (index < 0 || index >= InventoryService.Items.Count) return;
            var inv = InventoryService.Items[index];
            int value = (int)(inv.Item.Price * 0.45);
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE users SET gold = gold + @amt WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@amt", value);
            cmd.Parameters.AddWithValue("@id", _userId);
            cmd.ExecuteNonQuery();
            InventoryService.RemoveItem(inv.Item);
            RefreshGold();
            RefreshInventory();
        }

        private void LstShop_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var item = (Item)_lstShop.Items[e.Index];
            e.DrawBackground();
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
                string price = $" - {item.Price}g";
                e.Graphics.DrawString(price, e.Font, Brushes.Black, x, e.Bounds.Top);
            }
            else
            {
                using var brush = new SolidBrush(item.NameColor);
                e.Graphics.DrawString($"{item.Name} - {item.Price}g", e.Font, brush, e.Bounds);
            }
            e.DrawFocusRectangle();
        }

        private void LstInventory_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var inv = (InventoryItem)_lstInventory.Items[e.Index];
            var item = inv.Item;
            e.DrawBackground();
            string suffix = inv.Item.Stackable ? $" x{inv.Quantity}" : string.Empty;
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

        private void ShopForm_Load_1(object sender, EventArgs e)
        {

        }

        private void ShopForm_Load_2(object sender, EventArgs e)
        {

        }
    }
}
