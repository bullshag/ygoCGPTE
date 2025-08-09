using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public class LevelUpForm : Form
    {
        private readonly int _userId;
        private readonly int _characterId;
        private NumericUpDown numStr = new NumericUpDown();
        private NumericUpDown numDex = new NumericUpDown();
        private NumericUpDown numInt = new NumericUpDown();
        private Label lblPoints = new Label();
        private Label lblGold = new Label();
        private ListBox lstAbilities = new ListBox();
        private Button btnBuy = new Button();
        private Button btnSave = new Button();

        private int _baseStr;
        private int _baseDex;
        private int _baseInt;
        private int _availablePoints;
        private int _playerGold;
        private List<Ability> _abilities = new List<Ability>();

        public LevelUpForm(int userId, int characterId)
        {
            _userId = userId;
            _characterId = characterId;
            Text = "Level Up";
            Width = 400;
            Height = 300;

            var lblStr = new Label { Text = "STR", Left = 10, Top = 10, Width = 40 };
            numStr.Left = 60; numStr.Top = 8; numStr.Width = 60;
            numStr.Minimum = 0; numStr.Maximum = 999;
            numStr.ValueChanged += StatsChanged;

            var lblDex = new Label { Text = "DEX", Left = 10, Top = 40, Width = 40 };
            numDex.Left = 60; numDex.Top = 38; numDex.Width = 60;
            numDex.Minimum = 0; numDex.Maximum = 999;
            numDex.ValueChanged += StatsChanged;

            var lblInt = new Label { Text = "INT", Left = 10, Top = 70, Width = 40 };
            numInt.Left = 60; numInt.Top = 68; numInt.Width = 60;
            numInt.Minimum = 0; numInt.Maximum = 999;
            numInt.ValueChanged += StatsChanged;

            lblPoints.Left = 10; lblPoints.Top = 100; lblPoints.Width = 200;
            lblGold.Left = 220; lblGold.Top = 10; lblGold.Width = 150;
            lstAbilities.Left = 220; lstAbilities.Top = 40; lstAbilities.Width = 150; lstAbilities.Height = 120;
            btnBuy.Text = "Buy"; btnBuy.Left = 220; btnBuy.Top = 170; btnBuy.Click += BtnBuy_Click;
            btnSave.Text = "Save"; btnSave.Left = 10; btnSave.Top = 130; btnSave.Click += BtnSave_Click;

            Controls.AddRange(new Control[] { lblStr, numStr, lblDex, numDex, lblInt, numInt, lblPoints, lblGold, lstAbilities, btnBuy, btnSave });
            Load += LevelUpForm_Load;
        }

        private void LevelUpForm_Load(object? sender, EventArgs e)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using (MySqlCommand update = new MySqlCommand("UPDATE characters SET level=level+1, skill_points=skill_points+5 WHERE id=@cid", conn))
            {
                update.Parameters.AddWithValue("@cid", _characterId);
                update.ExecuteNonQuery();
            }

            using (MySqlCommand cmd = new MySqlCommand("SELECT strength, dex, intelligence, skill_points FROM characters WHERE id=@cid", conn))
            {
                cmd.Parameters.AddWithValue("@cid", _characterId);
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    _baseStr = reader.GetInt32("strength");
                    _baseDex = reader.GetInt32("dex");
                    _baseInt = reader.GetInt32("intelligence");
                    _availablePoints = reader.GetInt32("skill_points");
                    numStr.Value = _baseStr;
                    numDex.Value = _baseDex;
                    numInt.Value = _baseInt;
                    lblPoints.Text = $"Points: {_availablePoints}";
                }
            }

            using (MySqlCommand goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn))
            {
                goldCmd.Parameters.AddWithValue("@id", _userId);
                _playerGold = Convert.ToInt32(goldCmd.ExecuteScalar());
                lblGold.Text = $"Gold: {_playerGold}";
            }

            _abilities = AbilityService.GetShopAbilities(_characterId, conn);
            lstAbilities.Items.Clear();
            foreach (var a in _abilities)
            {
                lstAbilities.Items.Add($"{a.Name} ({a.Cost})");
            }
        }

        private void StatsChanged(object? sender, EventArgs e)
        {
            int spent = (int)((numStr.Value - _baseStr) + (numDex.Value - _baseDex) + (numInt.Value - _baseInt));
            if (spent > _availablePoints)
            {
                var control = (NumericUpDown)sender!;
                control.Value -= spent - _availablePoints;
                spent = _availablePoints;
            }
            lblPoints.Text = $"Points: {_availablePoints - spent}";
        }

        private void BtnBuy_Click(object? sender, EventArgs e)
        {
            if (lstAbilities.SelectedIndex < 0) return;
            var ability = _abilities[lstAbilities.SelectedIndex];
            if (_playerGold < ability.Cost)
            {
                MessageBox.Show("Not enough gold.");
                return;
            }
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using (MySqlCommand goldCmd = new MySqlCommand("UPDATE users SET gold=gold-@cost WHERE id=@id", conn))
            {
                goldCmd.Parameters.AddWithValue("@cost", ability.Cost);
                goldCmd.Parameters.AddWithValue("@id", _userId);
                goldCmd.ExecuteNonQuery();
            }
            AbilityService.PurchaseAbility(_characterId, ability.Id, conn);
            _playerGold -= ability.Cost;
            lblGold.Text = $"Gold: {_playerGold}";
            _abilities = AbilityService.GetShopAbilities(_characterId, conn);
            lstAbilities.Items.Clear();
            foreach (var a in _abilities)
            {
                lstAbilities.Items.Add($"{a.Name} ({a.Cost})");
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            int newStr = (int)numStr.Value;
            int newDex = (int)numDex.Value;
            int newInt = (int)numInt.Value;
            int spent = (newStr - _baseStr) + (newDex - _baseDex) + (newInt - _baseInt);
            int remaining = _availablePoints - spent;
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET strength=@s, dex=@d, intelligence=@i, skill_points=@sp WHERE id=@cid", conn)
            {
                cmd.Parameters.AddWithValue("@s", newStr);
                cmd.Parameters.AddWithValue("@d", newDex);
                cmd.Parameters.AddWithValue("@i", newInt);
                cmd.Parameters.AddWithValue("@sp", remaining);
                cmd.Parameters.AddWithValue("@cid", _characterId);
                cmd.ExecuteNonQuery();
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
