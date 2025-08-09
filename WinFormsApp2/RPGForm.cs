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

        public RPGForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
        }

        private void RPGForm_Load(object? sender, EventArgs e)
        {
            LoadPartyData();
        }

        private void LoadPartyData()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using MySqlCommand cmd = new MySqlCommand("SELECT name, experience_points FROM characters WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            using MySqlDataReader reader = cmd.ExecuteReader();
            lstParty.Items.Clear();
            int totalExp = 0;
            while (reader.Read())
            {
                string name = reader.GetString("name");
                int exp = reader.GetInt32("experience_points");
                lstParty.Items.Add($"{name} - EXP {exp}");
                totalExp += exp;
            }
            reader.Close();

            lblTotalExp.Text = $"Party EXP: {totalExp}";

            int level = totalExp / 100 + 1;
            _searchCost = 100 + level * 10 + lstParty.Items.Count * 20;

            using MySqlCommand goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn);
            goldCmd.Parameters.AddWithValue("@id", _userId);
            object? goldResult = goldCmd.ExecuteScalar();
            _playerGold = goldResult == null ? 0 : Convert.ToInt32(goldResult);
            lblGold.Text = $"Gold: {_playerGold}";

            btnHire.Text = $"Search for new recruits ({_searchCost} gold)";
            btnHire.Enabled = lstParty.Items.Count < 5 && _playerGold >= _searchCost;
            btnInspect.Enabled = false;
            btnInspect.Text = "Inspect";
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
            using MySqlCommand cmd = new MySqlCommand("SELECT current_hp, max_hp, mana, strength, dex, intelligence, action_speed FROM characters WHERE account_id=@id AND name=@name", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            cmd.Parameters.AddWithValue("@name", name);
            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int hp = reader.GetInt32("current_hp");
                int maxHp = reader.GetInt32("max_hp");
                int mana = reader.GetInt32("mana");
                int str = reader.GetInt32("strength");
                int dex = reader.GetInt32("dex");
                int intel = reader.GetInt32("intelligence");
                int speed = reader.GetInt32("action_speed");
                MessageBox.Show($"{name}\nHP: {hp}/{maxHp}\nMana: {mana}\nSTR: {str}\nDEX: {dex}\nINT: {intel}\nSpeed: {speed}", $"Inspect {name}");
            }
        }
    }
}
