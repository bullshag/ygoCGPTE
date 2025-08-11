using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class LevelUpForm : Form
    {
        private readonly int _userId;
        private readonly int _characterId;
        private int _baseStr;
        private int _baseDex;
        private int _baseInt;
        private int _availablePoints;
        private int _playerGold;
        private List<Ability> _abilities = new List<Ability>();
        private int _maxMana;

        public LevelUpForm(int userId, int characterId)
        {
            _userId = userId;
            _characterId = characterId;
            InitializeComponent();
            numStr.ValueChanged += StatsChanged;
            numDex.ValueChanged += StatsChanged;
            numInt.ValueChanged += StatsChanged;
            btnBuy.Click += BtnBuy_Click;
            btnSave.Click += BtnSave_Click;
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

            using (MySqlCommand cmd = new MySqlCommand("SELECT strength, dex, intelligence, skill_points, mana FROM characters WHERE id=@cid", conn))
            {
                cmd.Parameters.AddWithValue("@cid", _characterId);
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    _baseStr = reader.GetInt32("strength");
                    _baseDex = reader.GetInt32("dex");
                    _baseInt = reader.GetInt32("intelligence");
                    _availablePoints = reader.GetInt32("skill_points");
                    _maxMana = 10 + 5 * _baseInt;
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
            _maxMana = 10 + 5 * (int)numInt.Value;
        }

        private void BtnBuy_Click(object? sender, EventArgs e)
        {
            if (lstAbilities.SelectedIndex < 0) return;
            var ability = _abilities[lstAbilities.SelectedIndex];
            if (_maxMana < ability.Cost)
            {
                MessageBox.Show("Not enough mana.");
                return;
            }
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            AbilityService.PurchaseAbility(_characterId, ability.Id, conn);
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
            using (MySqlCommand cmd = new MySqlCommand(
                "UPDATE characters SET strength=@s, dex=@d, intelligence=@i, skill_points=@sp WHERE id=@cid",
                conn))
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
