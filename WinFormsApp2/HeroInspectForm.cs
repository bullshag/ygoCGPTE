using System;
using System.Windows.Forms;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Drawing;

namespace WinFormsApp2
{
    public partial class HeroInspectForm : Form
    {
        private readonly int _userId;
        private readonly int _characterId;
        private readonly bool _readOnly;
        private ComboBox[] _abilityCombos = new ComboBox[3];
        private NumericUpDown[] _priorityNums = new NumericUpDown[3];
        private System.Collections.Generic.List<Ability> _knownAbilities = new();
        private bool _loadingAbilities;
        private string _characterName = string.Empty;
        private bool _loadingEquipment;
        private System.Collections.Generic.List<Passive> _passives = new();
        private readonly ToolTip _tip = new();
        private int _baseStr, _baseDex, _baseInt, _baseHp, _baseMaxHp, _baseMana, _level, _exp;

        public HeroInspectForm(int userId, int characterId, bool readOnly = false)
        {
            _userId = userId;
            _characterId = characterId;
            _readOnly = readOnly;
            InitializeComponent();
            _abilityCombos = new[] { cmbAbility1, cmbAbility2, cmbAbility3 };
            _priorityNums = new[] { numPriority1, numPriority2, numPriority3 };
            cmbRole.Items.AddRange(new[] { "Tank", "Healer", "DPS" });
            cmbRole.SelectedIndexChanged += CmbRole_SelectedIndexChanged;
            cmbTarget.SelectedIndexChanged += CmbTarget_SelectedIndexChanged;
            cmbLeft.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.LeftHand, cmbLeft);
            cmbRight.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.RightHand, cmbRight);
            cmbBody.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.Body, cmbBody);
            cmbLegs.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.Legs, cmbLegs);
            cmbHead.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.Head, cmbHead);
            cmbTrinket.SelectedIndexChanged += (s, e) => EquipSlot(EquipmentSlot.Trinket, cmbTrinket);
            foreach (var combo in new[] { cmbLeft, cmbRight, cmbBody, cmbLegs, cmbHead, cmbTrinket })
            {
                combo.DrawMode = DrawMode.OwnerDrawFixed;
                combo.DrawItem += Combo_DrawItem;
            }
            foreach (var cmb in _abilityCombos)
            {
                cmb.SelectedIndexChanged += AbilityComboChanged;
                cmb.MouseMove += AbilityCombo_MouseMove;
            }
            foreach (var num in _priorityNums)
            {
                num.Minimum = 1;
                num.Maximum = 99;
                num.ValueChanged += PriorityChanged;
            }
            btnLevelUp.Click += BtnLevelUp_Click;
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
                _level = reader.GetInt32("level");
                _exp = reader.GetInt32("experience_points");
                _baseStr = reader.GetInt32("strength");
                _baseDex = reader.GetInt32("dex");
                _baseInt = reader.GetInt32("intelligence");
                _baseHp = reader.GetInt32("current_hp");
                _baseMaxHp = reader.GetInt32("max_hp");
                _baseMana = 10 + 5 * _baseInt;
                DisplayStats();
                btnLevelUp.Enabled = !_readOnly && _exp >= ExperienceHelper.GetNextLevelRequirement(_level);

                string role = reader.GetString("role");
                string targeting = reader.GetString("targeting_style");
                cmbRole.SelectedItem = role;
                LoadTargetOptions(role);
                cmbTarget.SelectedItem = targeting;
                LoadEquipment();
                LoadAbilities();
                LoadPassives();

                if (_readOnly)
                {
                    cmbRole.Enabled = false;
                    cmbTarget.Enabled = false;
                    btnLevelUp.Enabled = false;
                    foreach (var cmb in _abilityCombos) cmb.Enabled = false;
                    foreach (var num in _priorityNums) num.Enabled = false;
                    cmbLeft.Enabled = false;
                    cmbRight.Enabled = false;
                    cmbBody.Enabled = false;
                    cmbLegs.Enabled = false;
                    cmbHead.Enabled = false;
                    cmbTrinket.Enabled = false;
                }
            }
        }

        private void DisplayStats()
        {
            int str = _baseStr, dex = _baseDex, intel = _baseInt, hp = _baseHp, maxHp = _baseMaxHp, mana = _baseMana, maxMana = _baseMana;
            InventoryService.ApplyEquipmentBonuses(_characterName, ref str, ref dex, ref intel, ref hp, ref maxHp, ref mana, ref maxMana);
            int nextExp = ExperienceHelper.GetNextLevelRequirement(_level);
            lblStats.Text = $"{_characterName}\nLevel: {_level}\nEXP: {_exp}/{nextExp}\nHP: {hp}/{maxHp}\nSTR: {str}\nDEX: {dex}\nINT: {intel}";
            ShowPredictions(str, dex, intel);
        }

        private void ShowPredictions(int str, int dex, int intel)
        {
            var left = InventoryService.GetEquippedItem(_characterName, EquipmentSlot.LeftHand) as Weapon;
            var right = InventoryService.GetEquippedItem(_characterName, EquipmentSlot.RightHand) as Weapon;
            var weapon = left ?? right;
            double statTotal = str;
            double min = 0.8, max = 1.2, critBonus = 0, speed = 1 + dex / 25.0;
            if (weapon != null)
            {
                statTotal = str * weapon.StrScaling + dex * weapon.DexScaling + intel * weapon.IntScaling;
                min = weapon.MinMultiplier;
                max = weapon.MaxMultiplier;
                critBonus = weapon.CritChanceBonus;
            }
            if (left != null) speed *= (1 + left.AttackSpeedMod);
            if (right != null) speed *= (1 + right.AttackSpeedMod);
            if (left != null && right != null && left.Name != right.Name) speed *= 1.5;
            double minD = Math.Max(1, statTotal * min);
            double maxD = Math.Max(1, statTotal * max);
            double crit = Math.Min(1.0, 0.05 + dex / 5 * 0.01 + critBonus);
            double atkRate = speed / 3.0;
            lblStats.Text += $"\nDamage: {minD:F1}-{maxD:F1}\nCrit Chance: {crit:P0}\nAttack Rate: {atkRate:F2}/s";
        }

        private void BtnLevelUp_Click(object? sender, EventArgs e)
        {
            if (_readOnly) return;
            var form = new LevelUpForm(_userId, _characterId);
            form.FormClosed += (_, __) =>
            {
                HeroInspectForm_Load(null, EventArgs.Empty);
                form.Dispose();
            };
            form.Show(this);
        }

        private void CmbRole_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_readOnly) return;
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
            if (_readOnly) return;
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

        private void LoadPassives()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            _passives = PassiveService.GetOwnedPassives(_characterId, conn);
            lstPassives.Items.Clear();
            foreach (var p in _passives)
            {
                lstPassives.Items.Add($"{p.Name} (Lv {p.Level})");
            }
            lstPassives.MouseMove += LstPassives_MouseMove;
        }

        private void LstPassives_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = lstPassives.IndexFromPoint(e.Location);
            if (index >= 0 && index < _passives.Count)
                _tip.Show(_passives[index].Description, lstPassives, e.Location + new Size(15, 15));
            else
                _tip.Hide(lstPassives);
        }

        private void AbilityCombo_MouseMove(object? sender, MouseEventArgs e)
        {
            var cmb = (ComboBox)sender!;
            int index = cmb.SelectedIndex;
            if (index > 0 && index - 1 < _knownAbilities.Count)
                _tip.Show(_knownAbilities[index - 1].Description, cmb, e.Location + new Size(15, 15));
            else
                _tip.Hide(cmb);
        }

        private void LoadSlot(ComboBox combo, EquipmentSlot slot)
        {
            combo.Items.Clear();
            combo.Items.Add("-empty-");
            var items = InventoryService.GetEquippableItems(slot, _characterName);
            foreach (var i in items)
            {
                combo.Items.Add(new ComboItem(i.Name, i.NameColor));
            }
            var equipped = InventoryService.GetEquippedItem(_characterName, slot);
            if (equipped != null && !combo.Items.Cast<object>().Any(o => o.ToString() == equipped.Name))
            {
                combo.Items.Add(new ComboItem(equipped.Name, equipped.NameColor));
            }
            combo.SelectedItem = combo.Items.Cast<object>().FirstOrDefault(o => o.ToString() == equipped?.Name) ?? "-empty-";
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

        private void Combo_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var combo = (ComboBox)sender!;
            e.DrawBackground();
            object item = combo.Items[e.Index];
            string text = item.ToString() ?? string.Empty;
            Color color = item is ComboItem ci ? ci.Color : Color.Black;
            using var brush = new SolidBrush(color);
            e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
        }

        private class ComboItem
        {
            public string Name { get; }
            public Color Color { get; }
            public ComboItem(string name, Color color) { Name = name; Color = color; }
            public override string ToString() => Name;
        }

        private void EquipSlot(EquipmentSlot slot, ComboBox combo)
        {
            if (_loadingEquipment || _readOnly) return;
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
            DisplayStats();
        }

        private void LoadAbilities()
        {
            _loadingAbilities = true;
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            _knownAbilities = AbilityService.GetCharacterAbilities(_characterId, conn);
            var equipped = new Dictionary<int, Ability>();
            foreach (var a in AbilityService.GetEquippedAbilities(_characterId, conn))
            {
                equipped[a.Slot] = a;
            }
            bool hasAbilities = _knownAbilities.Any();
            for (int i = 0; i < 3; i++)
            {
                var combo = _abilityCombos[i];
                combo.Items.Clear();
                combo.Items.Add("-basic attack-");
                foreach (var a in _knownAbilities)
                {
                    combo.Items.Add(a.Name);
                }
                if (equipped.TryGetValue(i + 1, out var abil))
                {
                    combo.SelectedItem = combo.Items.Contains(abil.Name) ? abil.Name : "-basic attack-";
                    _priorityNums[i].Value = abil.Priority;
                }
                else
                {
                    combo.SelectedItem = "-basic attack-";
                    _priorityNums[i].Value = 1;
                }
                combo.Enabled = hasAbilities;
                _priorityNums[i].Enabled = hasAbilities;
            }
            _loadingAbilities = false;
            UpdateAbilityOptions();
        }

        private void UpdateAbilityOptions()
        {
            if (_loadingAbilities || _readOnly) return;
            _loadingAbilities = true;
            var selected = new HashSet<string>(_abilityCombos.Select(c => c.SelectedItem?.ToString() ?? ""));
            for (int i = 0; i < 3; i++)
            {
                var combo = _abilityCombos[i];
                var current = combo.SelectedItem?.ToString() ?? "-basic attack-";
                combo.Items.Clear();
                combo.Items.Add("-basic attack-");
                foreach (var a in _knownAbilities)
                {
                    if (!selected.Contains(a.Name) || a.Name == current)
                    {
                        combo.Items.Add(a.Name);
                    }
                }
                combo.SelectedItem = current;
            }
            _loadingAbilities = false;
        }

        private void AbilityComboChanged(object? sender, EventArgs e)
        {
            if (_loadingAbilities || _readOnly) return;
            UpdateAbilityOptions();
            var combo = (ComboBox)sender!;
            int index = Array.IndexOf(_abilityCombos, combo);
            SaveAbility(index);
        }

        private void PriorityChanged(object? sender, EventArgs e)
        {
            if (_loadingAbilities || _readOnly) return;
            var num = (NumericUpDown)sender!;
            int index = Array.IndexOf(_priorityNums, num);
            SaveAbility(index);
        }

        private void SaveAbility(int index)
        {
            if (_readOnly) return;
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            string name = _abilityCombos[index].SelectedItem?.ToString() ?? "-basic attack-";
            int? abilityId = _knownAbilities.FirstOrDefault(a => a.Name == name)?.Id;
            int priority = (int)_priorityNums[index].Value;
            AbilityService.SetAbilitySlot(_characterId, index + 1, abilityId, priority, conn);
        }
    }
}
