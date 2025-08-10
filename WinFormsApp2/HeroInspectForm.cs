using System;
using System.Windows.Forms;
using System.Linq;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public class HeroInspectForm : Form
    {
        private readonly int _userId;
        private readonly int _characterId;
        private Label lblStats = new Label();
        private Button btnLevelUp = new Button();
        private ComboBox cmbRole = new ComboBox();
        private ComboBox cmbTarget = new ComboBox();
        private ComboBox cmbLeft = new ComboBox();
        private ComboBox cmbRight = new ComboBox();
        private ComboBox cmbBody = new ComboBox();
        private ComboBox cmbLegs = new ComboBox();
        private ComboBox cmbHead = new ComboBox();
        private ComboBox cmbTrinket = new ComboBox();
        private string _characterName = string.Empty;
        private bool _loadingEquipment;

        public HeroInspectForm(int userId, int characterId)
        {
            _userId = userId;
            _characterId = characterId;
            Text = "Hero Details";
            Width = 300;
            Height = 430;

            lblStats.Left = 10;
            lblStats.Top = 10;
            lblStats.Width = 260;
            lblStats.Height = 150;
            Controls.Add(lblStats);

            cmbRole.Left = 10;
            cmbRole.Top = 170;
            cmbRole.Width = 120;
            cmbRole.Items.AddRange(new[] { "Tank", "Healer", "DPS" });
            cmbRole.SelectedIndexChanged += CmbRole_SelectedIndexChanged;
            Controls.Add(cmbRole);

            cmbTarget.Left = 150;
            cmbTarget.Top = 170;
            cmbTarget.Width = 140;
            cmbTarget.SelectedIndexChanged += CmbTarget_SelectedIndexChanged;
            Controls.Add(cmbTarget);

            cmbLeft.Left = 10;
            cmbLeft.Top = 230;
            cmbLeft.Width = 120;
            cmbLeft.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.LeftHand, cmbLeft);
            Controls.Add(cmbLeft);

            cmbRight.Left = 150;
            cmbRight.Top = 230;
            cmbRight.Width = 120;
            cmbRight.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.RightHand, cmbRight);
            Controls.Add(cmbRight);

            cmbBody.Left = 10;
            cmbBody.Top = 260;
            cmbBody.Width = 120;
            cmbBody.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.Body, cmbBody);
            Controls.Add(cmbBody);

            cmbLegs.Left = 150;
            cmbLegs.Top = 260;
            cmbLegs.Width = 120;
            cmbLegs.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.Legs, cmbLegs);
            Controls.Add(cmbLegs);

            cmbHead.Left = 10;
            cmbHead.Top = 290;
            cmbHead.Width = 120;
            cmbHead.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.Head, cmbHead);
            Controls.Add(cmbHead);

            cmbTrinket.Left = 150;
            cmbTrinket.Top = 290;
            cmbTrinket.Width = 120;
            cmbTrinket.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.Trinket, cmbTrinket);
            Controls.Add(cmbTrinket);

            btnLevelUp.Text = "Level Up";
            btnLevelUp.Left = 10;
            btnLevelUp.Top = 360;
            btnLevelUp.Click += BtnLevelUp_Click;
            Controls.Add(btnLevelUp);

            Load += HeroInspectForm_Load;
        }

        private void HeroInspectForm_Load(object? sender, EventArgs e)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT name, level, experience_points, strength, dex, intelligence, current_hp, max_hp, role, targeting_style FROM characters WHERE id=@cid AND account_id=@uid", conn);
            cmd.Parameters.AddWithValue("@cid", _characterId);
            cmd.Parameters.AddWithValue("@uid", _userId);
            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string name = reader.GetString("name");
                _characterName = name;
                int level = reader.GetInt32("level");
                int exp = reader.GetInt32("experience_points");
                int str = reader.GetInt32("strength");
                int dex = reader.GetInt32("dex");
                int intel = reader.GetInt32("intelligence");
                int hp = reader.GetInt32("current_hp");
                int maxHp = reader.GetInt32("max_hp");
                int nextExp = ExperienceHelper.GetNextLevelRequirement(level);
                lblStats.Text = $"{name}\nLevel: {level}\nEXP: {exp}/{nextExp}\nHP: {hp}/{maxHp}\nSTR: {str}\nDEX: {dex}\nINT: {intel}";
                ShowPredictions(str, dex, intel);
                btnLevelUp.Enabled = exp >= nextExp;

                string role = reader.GetString("role");
                string targeting = reader.GetString("targeting_style");
                cmbRole.SelectedItem = role;
                LoadTargetOptions(role);
                cmbTarget.SelectedItem = targeting;
                LoadEquipment();
            }
        }

        private void ShowPredictions(int str, int dex, int intel)
        {
            var weapon = InventoryService.GetEquippedItem(_characterName, EquipmentSlot.LeftHand) as Weapon
                ?? InventoryService.GetEquippedItem(_characterName, EquipmentSlot.RightHand) as Weapon;
            double statTotal = str;
            double min = 0.8, max = 1.2, critBonus = 0, speed = 1 + dex / 25.0;
            if (weapon != null)
            {
                statTotal = str * weapon.StrScaling + dex * weapon.DexScaling + intel * weapon.IntScaling;
                min = weapon.MinMultiplier;
                max = weapon.MaxMultiplier;
                critBonus = weapon.CritChanceBonus;
                speed *= (1 + weapon.AttackSpeedMod);
            }
            double minD = Math.Max(1, statTotal * min);
            double maxD = Math.Max(1, statTotal * max);
            double crit = Math.Min(1.0, 0.05 + dex / 5 * 0.01 + critBonus);
            double atkRate = speed / 3.0;
            lblStats.Text += $"\nDamage: {minD:F1}-{maxD:F1}\nCrit Chance: {crit:P0}\nAttack Rate: {atkRate:F2}/s";
        }

        private void BtnLevelUp_Click(object? sender, EventArgs e)
        {
            using var form = new LevelUpForm(_userId, _characterId);
            form.ShowDialog(this);
            HeroInspectForm_Load(null, EventArgs.Empty);
        }

        private void CmbRole_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string role = cmbRole.SelectedItem?.ToString() ?? "DPS";
            LoadTargetOptions(role);
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET role=@r WHERE id=@cid", conn);
            cmd.Parameters.AddWithValue("@r", role);
            cmd.Parameters.AddWithValue("@cid", _characterId);
            cmd.ExecuteNonQuery();
        }

        private void CmbTarget_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string targeting = cmbTarget.SelectedItem?.ToString() ?? "no priorities";
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET targeting_style=@t WHERE id=@cid", conn);
            cmd.Parameters.AddWithValue("@t", targeting);
            cmd.Parameters.AddWithValue("@cid", _characterId);
            cmd.ExecuteNonQuery();
        }

        private void LoadTargetOptions(string role)
        {
            cmbTarget.Items.Clear();
            switch (role)
            {
                case "Healer":
                    cmbTarget.Items.AddRange(new[] { "prioritize lowest health ally", "prioritize different allies each turn", "prioritize self", "no priorities" });
                    break;
                case "Tank":
                    cmbTarget.Items.AddRange(new[] { "prioritize strongest foe", "prioritize weakest foe", "prioritize targets that arent attack you", "no priorities" });
                    break;
                default: // DPS
                    cmbTarget.Items.AddRange(new[] { "prioritize target of the strongest tank", "prioritize targets attacking you", "prioritize targets that attack non-tanks", "no priorities" });
                    break;
            }
            if (cmbTarget.Items.Count > 0)
            {
                cmbTarget.SelectedIndex = 0;
            }
        }

        private void LoadEquipment()
        {
            _loadingEquipment = true;
            LoadSlot(cmbLeft, EquipmentSlot.LeftHand);
            LoadSlot(cmbRight, EquipmentSlot.RightHand);
            LoadSlot(cmbBody, EquipmentSlot.Body);
            LoadSlot(cmbLegs, EquipmentSlot.Legs);
            LoadSlot(cmbHead, EquipmentSlot.Head);
            LoadSlot(cmbTrinket, EquipmentSlot.Trinket);
            _loadingEquipment = false;
        }

        private void LoadSlot(ComboBox combo, EquipmentSlot slot)
        {
            combo.Items.Clear();
            combo.Items.Add("-empty-");
            var items = InventoryService.GetEquippableItems(slot, _characterName);
            foreach (var i in items)
            {
                combo.Items.Add(i.Name);
            }
            var equipped = InventoryService.GetEquippedItem(_characterName, slot);
            if (equipped != null && !combo.Items.Contains(equipped.Name))
            {
                combo.Items.Add(equipped.Name);
            }
            combo.SelectedItem = equipped?.Name ?? "-empty-";
            if (equipped is Weapon w && w.TwoHanded)
            {
                if (slot == EquipmentSlot.LeftHand)
                {
                    cmbRight.Enabled = false;
                }
                else if (slot == EquipmentSlot.RightHand)
                {
                    cmbLeft.Enabled = false;
                }
            }
            else
            {
                cmbLeft.Enabled = true;
                cmbRight.Enabled = true;
            }
        }

        private void EquipSlot(EquipmentSlot slot, ComboBox combo)
        {
            if (_loadingEquipment) return;
            string selected = combo.SelectedItem?.ToString() ?? "-empty-";
            if (selected == "-empty-")
            {
                var current = InventoryService.GetEquippedItem(_characterName, slot);
                if (current != null)
                {
                    InventoryService.Equip(_characterName, slot, null);
                }
            }
            else
            {
                var item = InventoryService.GetEquippableItems(slot, _characterName).FirstOrDefault(i => i.Name == selected);
                if (item != null)
                {
                    InventoryService.Equip(_characterName, slot, item);
                }
            }
            LoadEquipment();
        }
    }
}
