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
        private int _baseHp;
        private int _baseMp;
        private int _availablePoints;
        private int _playerGold;
        private List<Ability> _abilities = new List<Ability>();
        private List<Passive> _passives = new List<Passive>();
        private int _ownedPassiveCount;
        private int _maxMana;
        private readonly ToolTip _tip = new();
        private bool _loading;

        public LevelUpForm(int userId, int characterId)
        {
            _userId = userId;
            _characterId = characterId;
            InitializeComponent();
            numStr.ValueChanged += StatsChanged;
            numDex.ValueChanged += StatsChanged;
            numInt.ValueChanged += StatsChanged;
            numHP.ValueChanged += StatsChanged;
            numMP.ValueChanged += StatsChanged;
            btnBuy.Click += BtnBuy_Click;
            btnBuyPassive.Click += BtnBuyPassive_Click;
            btnSave.Click += BtnSave_Click;
            lstAbilities.SelectedIndexChanged += LstAbilities_SelectedIndexChanged;
            lstAbilities.MouseMove += LstAbilities_MouseMove;
            lstPassives.MouseMove += LstPassives_MouseMove;
            lstPassives.SelectedIndexChanged += LstPassives_SelectedIndexChanged;
            Load += LevelUpForm_Load;
        }

        private void LevelUpForm_Load(object? sender, EventArgs e)
        {
            _loading = true;
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using (MySqlCommand update = new MySqlCommand("UPDATE characters SET level=level+1, skill_points=skill_points+5 WHERE id=@cid", conn))
            {
                update.Parameters.AddWithValue("@cid", _characterId);
                update.ExecuteNonQuery();
            }

            using (MySqlCommand cmd = new MySqlCommand("SELECT strength, dex, intelligence, skill_points, max_hp, mana FROM characters WHERE id=@cid", conn))
            {
                cmd.Parameters.AddWithValue("@cid", _characterId);
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    _baseStr = reader.GetInt32("strength");
                    _baseDex = reader.GetInt32("dex");
                    _baseInt = reader.GetInt32("intelligence");
                    _baseHp = reader.GetInt32("max_hp");
                    _baseMp = reader.GetInt32("mana");
                    _availablePoints = reader.GetInt32("skill_points");
                    _maxMana = _baseMp;
                    numStr.Minimum = _baseStr;
                    numDex.Minimum = _baseDex;
                    numInt.Minimum = _baseInt;
                    numHP.Minimum = _baseHp;
                    numMP.Minimum = _baseMp;
                    numStr.Value = _baseStr;
                    numDex.Value = _baseDex;
                    numInt.Value = _baseInt;
                    numHP.Value = _baseHp;
                    numMP.Value = _baseMp;
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
                lstAbilities.Items.Add(a.Name);
            }
            rtbAbility.Clear();
            btnBuy.Text = "Buy";
            btnBuy.Enabled = false;
            _passives = PassiveService.GetAvailablePassives(_characterId, conn);
            _ownedPassiveCount = PassiveService.GetOwnedPassives(_characterId, conn).Count;
            lstPassives.Items.Clear();
            foreach (var p in _passives)
            {
                lstPassives.Items.Add(p.Name);
            }
            UpdatePassiveCostButton();
            _loading = false;
        }

        private void StatsChanged(object? sender, EventArgs e)
        {
            if (_loading) return;
            int spent = (int)((numStr.Value - _baseStr) + (numDex.Value - _baseDex) + (numInt.Value - _baseInt)
                + ((numHP.Value - _baseHp) / 5) + ((numMP.Value - _baseMp) / 5));
            if (spent < 0)
            {
                var control = (NumericUpDown)sender!;
                int adjust = -spent;
                if (control == numHP || control == numMP)
                    control.Value += adjust * 5;
                else
                    control.Value += adjust;
                spent = 0;
            }
            if (spent > _availablePoints)
            {
                var control = (NumericUpDown)sender!;
                int adjust = spent - _availablePoints;
                if (control == numHP || control == numMP)
                    control.Value -= adjust * 5;
                else
                    control.Value -= adjust;
                spent = _availablePoints;
            }
            lblPoints.Text = $"Points: {_availablePoints - spent}";
            _maxMana = (int)numMP.Value;
        }

        private void UpdatePassiveCostButton()
        {
            int cost = 1 + _ownedPassiveCount;
            btnBuyPassive.Text = $"Buy Passive ({cost} pt{(cost > 1 ? "s" : string.Empty)})";
            btnBuyPassive.Enabled = lstPassives.SelectedIndex >= 0 && cost <= _availablePoints;
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
            if (_availablePoints < ability.PointCost)
            {
                MessageBox.Show("Not enough points.");
                return;
            }
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            AbilityService.PurchaseAbility(_characterId, ability.Id, ability.PointCost, conn);
            _availablePoints -= ability.PointCost;
            lblPoints.Text = $"Points: {_availablePoints}";
            _abilities = AbilityService.GetShopAbilities(_characterId, conn);
            lstAbilities.Items.Clear();
            foreach (var a in _abilities)
            {
                lstAbilities.Items.Add(a.Name);
            }
            rtbAbility.Clear();
            btnBuy.Text = "Buy";
            btnBuy.Enabled = false;
        }

        private void BtnBuyPassive_Click(object? sender, EventArgs e)
        {
            if (lstPassives.SelectedIndex < 0) return;
            var passive = _passives[lstPassives.SelectedIndex];
            int cost = 1 + _ownedPassiveCount;
            if (_availablePoints < cost)
            {
                MessageBox.Show("Not enough points.");
                return;
            }
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            PassiveService.PurchasePassive(_characterId, passive.Id, cost, conn);
            _availablePoints -= cost;
            lblPoints.Text = $"Points: {_availablePoints}";
            _ownedPassiveCount++;
            _passives = PassiveService.GetAvailablePassives(_characterId, conn);
            lstPassives.Items.Clear();
            foreach (var p in _passives)
            {
                lstPassives.Items.Add(p.Name);
            }
            UpdatePassiveCostButton();
        }

        private void LstPassives_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdatePassiveCostButton();
        }

        private void LstAbilities_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstAbilities.SelectedIndex < 0)
            {
                rtbAbility.Clear();
                btnBuy.Text = "Buy";
                btnBuy.Enabled = false;
                return;
            }
            var ability = _abilities[lstAbilities.SelectedIndex];
            rtbAbility.Text = $"{ability.Description}\nCooldown: {ability.Cooldown}s\nMana Cost: {ability.Cost}\nPoint Cost: {ability.PointCost}";
            btnBuy.Text = $"Buy ({ability.PointCost} pt{(ability.PointCost > 1 ? "s" : string.Empty)})";
            btnBuy.Enabled = ability.PointCost <= _availablePoints;
        }

        private void LstAbilities_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = lstAbilities.IndexFromPoint(e.Location);
            if (index >= 0 && index < _abilities.Count)
                _tip.Show(_abilities[index].Description, lstAbilities, e.Location + new Size(15, 15));
            else
                _tip.Hide(lstAbilities);
        }

        private void LstPassives_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = lstPassives.IndexFromPoint(e.Location);
            if (index >= 0 && index < _passives.Count)
                _tip.Show(_passives[index].Description, lstPassives, e.Location + new Size(15, 15));
            else
                _tip.Hide(lstPassives);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            int newStr = (int)numStr.Value;
            int newDex = (int)numDex.Value;
            int newInt = (int)numInt.Value;
            int newHp = (int)numHP.Value;
            int newMp = (int)numMP.Value;
            int spent = (newStr - _baseStr) + (newDex - _baseDex) + (newInt - _baseInt)
                + ((newHp - _baseHp) / 5) + ((newMp - _baseMp) / 5);
            int remaining = _availablePoints - spent;
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using (MySqlCommand cmd = new MySqlCommand(
                "UPDATE characters SET strength=@s, dex=@d, intelligence=@i, max_hp=@hp, mana=@mana, skill_points=@sp WHERE id=@cid",
                conn))
            {
                cmd.Parameters.AddWithValue("@s", newStr);
                cmd.Parameters.AddWithValue("@d", newDex);
                cmd.Parameters.AddWithValue("@i", newInt);
                cmd.Parameters.AddWithValue("@hp", newHp);
                cmd.Parameters.AddWithValue("@mana", newMp);
                cmd.Parameters.AddWithValue("@sp", remaining);
                cmd.Parameters.AddWithValue("@cid", _characterId);
                cmd.ExecuteNonQuery();
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void numMP_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
