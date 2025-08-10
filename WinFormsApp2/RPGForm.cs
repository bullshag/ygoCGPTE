using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class RPGForm : Form
    {
        private readonly int _userId;
        private int _searchCost;
        private int _playerGold;
        private readonly System.Windows.Forms.Timer _regenTimer = new System.Windows.Forms.Timer();

        public RPGForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
        }

        private void RPGForm_Load(object? sender, EventArgs e)
        {
            LoadPartyData();
            _regenTimer.Interval = 10000;
            _regenTimer.Tick += (s, e2) => Regenerate();
            _regenTimer.Start();
        }

        private void LoadPartyData()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using MySqlCommand cmd = new MySqlCommand("SELECT name, experience_points, level FROM characters WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            using MySqlDataReader reader = cmd.ExecuteReader();
            lstParty.Items.Clear();
            int totalExp = 0;
            int totalLevel = 0;
            while (reader.Read())
            {
                string name = reader.GetString("name");
                int exp = reader.GetInt32("experience_points");
                int level = reader.GetInt32("level");
                int nextExp = ExperienceHelper.GetNextLevelRequirement(level);
                lstParty.Items.Add($"{name} - LVL {level} EXP {exp}/{nextExp}");
                totalExp += exp;
                totalLevel += level;
            }
            reader.Close();

            lblTotalExp.Text = $"Party EXP: {totalExp}";

            _searchCost = 100 + totalLevel * 10 + lstParty.Items.Count * 20;
            if (lstParty.Items.Count == 0)
            {
                _searchCost = 0;
            }

            using MySqlCommand goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn);
            goldCmd.Parameters.AddWithValue("@id", _userId);
            object? goldResult = goldCmd.ExecuteScalar();
            _playerGold = goldResult == null ? 0 : Convert.ToInt32(goldResult);
            lblGold.Text = $"Gold: {_playerGold}";

            btnHire.Text = _searchCost > 0
                ? $"Search for new recruits ({_searchCost} gold)"
                : "Search for new recruits (free)";
            btnHire.Enabled = lstParty.Items.Count < 5 && _playerGold >= _searchCost;
            btnInspect.Enabled = false;
            btnInspect.Text = "Inspect";
            btnBattle.Enabled = lstParty.Items.Count > 0;
        }

        private void btnHire_Click(object? sender, EventArgs e)
        {
            if (_playerGold < _searchCost)
            {
                MessageBox.Show("Not enough gold to search for recruits.");
                return;
            }

            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand payCmd = new MySqlCommand("UPDATE users SET gold = gold - @cost WHERE id=@id", conn);
            payCmd.Parameters.AddWithValue("@cost", _searchCost);
            payCmd.Parameters.AddWithValue("@id", _userId);
            payCmd.ExecuteNonQuery();

            LoadPartyData();

            var rng = new Random();
            var candidates = new System.Collections.Generic.List<RecruitCandidate>();
            for (int i = 0; i < 3; i++)
            {
                candidates.Add(RecruitCandidate.Generate(rng, i));
            }
            using var recruitForm = new RecruitForm(_userId, candidates, _searchCost, LoadPartyData);
            recruitForm.ShowDialog(this);
        }

        private void lstParty_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstParty.SelectedItem == null)
            {
                btnInspect.Enabled = false;
                btnInspect.Text = "Inspect";
            }
            else
            {
                string item = lstParty.SelectedItem.ToString() ?? string.Empty;
                string name = item.Split(" - ")[0];
                btnInspect.Enabled = true;
                btnInspect.Text = $"Inspect {name}";
            }
        }

        private void btnInspect_Click(object? sender, EventArgs e)
        {
            if (lstParty.SelectedItem == null) return;
            string item = lstParty.SelectedItem.ToString() ?? string.Empty;
            string name = item.Split(" - ")[0];

            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT id FROM characters WHERE account_id=@id AND name=@name", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            cmd.Parameters.AddWithValue("@name", name);
            object? result = cmd.ExecuteScalar();
            if (result != null)
            {
                int charId = Convert.ToInt32(result);
                using var inspect = new HeroInspectForm(_userId, charId);
                inspect.ShowDialog(this);
            }
        }

        private void btnBattle_Click(object? sender, EventArgs e)
        {
            using var battle = new BattleForm(_userId);
            battle.ShowDialog(this);
        }

        private void btnLogs_Click(object? sender, EventArgs e)
        {
            using var logs = new BattleLogForm();
            logs.ShowDialog(this);
        }

        private void btnShop_Click(object? sender, EventArgs e)
        {
            using var shop = new ShopForm(_userId);
            shop.ShowDialog(this);
            LoadPartyData();
        }

        private void btnInventory_Click(object? sender, EventArgs e)
        {
            using var inv = new InventoryForm(_userId);
            inv.ShowDialog(this);
        }

        private void Regenerate()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET current_hp = LEAST(max_hp, current_hp + GREATEST(10, CEILING(max_hp*0.05))) WHERE account_id=@id AND current_hp>0 AND current_hp<max_hp", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            cmd.ExecuteNonQuery();

            int selectedIndex = lstParty.SelectedIndex;
            LoadPartyData();
            if (selectedIndex >= 0 && selectedIndex < lstParty.Items.Count)
            {
                lstParty.SelectedIndex = selectedIndex;
            }
        }
    }
}
