using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public class ShopForm : Form
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
            WeaponFactory.Create("wand")
        };

        private ListBox _lstShop = new ListBox();
        private ListBox _lstInventory = new ListBox();
        private Button _btnBuy = new Button();
        private Button _btnSell = new Button();
        private Label _lblGold = new Label();

        public ShopForm(int userId)
        {
            _userId = userId;
            Text = "Shop";
            Width = 600;
            Height = 400;

            _lstShop.Location = new Point(10, 10);
            _lstShop.Size = new Size(250, 300);
            Controls.Add(_lstShop);

            _lstInventory.Location = new Point(330, 10);
            _lstInventory.Size = new Size(250, 300);
            Controls.Add(_lstInventory);

            _btnBuy.Text = "Buy";
            _btnBuy.Location = new Point(10, 320);
            _btnBuy.Click += BtnBuy_Click;
            Controls.Add(_btnBuy);

            _btnSell.Text = "Sell";
            _btnSell.Location = new Point(330, 320);
            _btnSell.Click += BtnSell_Click;
            Controls.Add(_btnSell);

            _lblGold.Location = new Point(10, 350);
            _lblGold.AutoSize = true;
            Controls.Add(_lblGold);

            Load += ShopForm_Load;
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
                _lstShop.Items.Add($"{item.Name} - {item.Price}g");
            }
        }

        private void RefreshInventory()
        {
            _lstInventory.Items.Clear();
            foreach (var inv in InventoryService.Items)
            {
                int price = (int)(inv.Item.Price * 0.45);
                string name = inv.Item.Name;
                if (inv.Item.Stackable) name += $" x{inv.Quantity}";
                _lstInventory.Items.Add($"{name} ({price}g)");
            }
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

        private void InitializeComponent()
        {

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
    }
}
