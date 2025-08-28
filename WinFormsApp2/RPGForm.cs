using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using MySql.Data.MySqlClient;
using WinFormsApp2.Multiplayer;

namespace WinFormsApp2
{
    public partial class RPGForm : Form
    {
        private readonly int _userId;
        private readonly string _nickname;
        private int _playerGold;
        private readonly System.Windows.Forms.Timer _chatTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer _regenTimer = new System.Windows.Forms.Timer();
        private DateTime _lastMessage = DateTime.MinValue;
        private HashSet<string> _hiredMembers = new();

        public RPGForm(int userId, string nickname)
        {
            _userId = userId;
            _nickname = nickname;
            InitializeComponent();
        }

        private void RPGForm_Load(object? sender, EventArgs e)
        {
            InventoryService.Load(_userId);
            LoadPartyData();
            _chatTimer.Interval = 1000;
            _chatTimer.Tick += ChatTimer_Tick;
            _chatTimer.Start();
            _regenTimer.Interval = 3000;
            _regenTimer.Tick += RegenTimer_Tick;
            _regenTimer.Start();
        }

        private void LoadPartyData()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            _hiredMembers = PartyHireService.GetHiredMemberNames(_userId);

            using MySqlCommand cmd = new MySqlCommand("SELECT name, experience_points, level, current_hp, max_hp, mana, intelligence, in_tavern FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0", conn);

            cmd.Parameters.AddWithValue("@id", _userId);
            using MySqlDataReader reader = cmd.ExecuteReader();
            lstParty.Items.Clear();
            pnlParty.Controls.Clear();
            int totalExp = 0;
            int totalLevel = 0;
            int totalEquipCost = 0;
            int index = 0;
            while (reader.Read())
            {
                string name = reader.GetString("name");
                bool inTavern = reader.GetBoolean("in_tavern");
                if (inTavern && !_hiredMembers.Contains(name))
                    continue;
                int exp = reader.GetInt32("experience_points");
                int level = reader.GetInt32("level");
                int hp = reader.GetInt32("current_hp");
                int maxHp = reader.GetInt32("max_hp");
                int mana = reader.GetInt32("mana");
                int intel = reader.GetInt32("intelligence");
                int maxMana = 10 + 5 * intel;
                int nextExp = ExperienceHelper.GetNextLevelRequirement(level);
                string display = $"{name} - LVL {level} EXP {exp}/{nextExp}";
                if (_hiredMembers.Contains(name))
                    display += " (Hired Out)";
                lstParty.Items.Add(display);

                var panel = new Panel { Width = 180, Height = maxMana > 0 ? 60 : 40, Margin = new Padding(3) };
                var lbl = new Label { Text = name, AutoSize = true };
                var hpBar = new ProgressBar { Maximum = maxHp, Value = Math.Min(hp, maxHp), Width = 170, Location = new Point(0, 15), ForeColor = Color.Red, Style = ProgressBarStyle.Continuous };
                panel.Controls.Add(lbl);
                panel.Controls.Add(hpBar);
                if (maxMana > 0)
                {
                    var manaBar = new ProgressBar { Maximum = maxMana, Value = Math.Min(mana, maxMana), Width = 170, Location = new Point(0, 35), ForeColor = Color.Blue, Style = ProgressBarStyle.Continuous };
                    panel.Controls.Add(manaBar);
                }
                int current = index;
                panel.Click += (s, e) => { lstParty.SelectedIndex = current; };
                foreach (Control c in panel.Controls) c.Click += (s, e) => { lstParty.SelectedIndex = current; };
                pnlParty.Controls.Add(panel);

                totalExp += exp;
                totalLevel += level;
                foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
                {
                    var eqItem = InventoryService.GetEquippedItem(name, slot);
                    if (eqItem != null)
                        totalEquipCost += eqItem.Price;
                }
                index++;
            }
            reader.Close();

            lblTotalExp.Text = $"Party EXP: {totalExp}";
            int totalSkills = 0;
            using (var skillCmd = new MySqlCommand("SELECT COUNT(*) FROM character_abilities ca JOIN characters c ON ca.character_id=c.id WHERE c.account_id=@id AND c.is_dead=0 AND in_arena=0 AND in_tavern=0", conn))
            {
                skillCmd.Parameters.AddWithValue("@id", _userId);
                object? sres = skillCmd.ExecuteScalar();
                totalSkills = sres == null || sres == DBNull.Value ? 0 : Convert.ToInt32(sres);
            }
            int partyPower = (int)Math.Ceiling((totalLevel + totalEquipCost + 3 * totalSkills) * 0.15);
            partyPowerLabel.Text = $"Party Power: {partyPower}";

            using MySqlCommand goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn);
            goldCmd.Parameters.AddWithValue("@id", _userId);
            object? goldResult = goldCmd.ExecuteScalar();
            _playerGold = goldResult == null ? 0 : Convert.ToInt32(goldResult);
            lblGold.Text = $"Gold: {_playerGold}";
            btnInspect.Enabled = false;
            btnInspect.Text = "Inspect";
        }

        private void lstParty_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstParty.SelectedItem == null)
            {
                btnInspect.Enabled = false;
                btnInspect.Text = "Inspect";
                btnFire.Enabled = false;
                btnFire.Text = "Fire";
            }
            else
            {
                string item = lstParty.SelectedItem.ToString() ?? string.Empty;
                string name = item.Split(" - ")[0];
                btnInspect.Enabled = true;
                btnInspect.Text = $"Inspect {name}";
                bool hired = _hiredMembers.Contains(name);
                btnFire.Enabled = !hired;
                btnFire.Text = hired ? "Hired Out" : $"Fire {name}";
            }
        }

        private void btnInspect_Click(object? sender, EventArgs e)
        {
            if (lstParty.SelectedItem == null) return;
            string item = lstParty.SelectedItem.ToString() ?? string.Empty;
            string name = item.Split(" - ")[0];

            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT id FROM characters WHERE account_id=@id AND name=@name AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            cmd.Parameters.AddWithValue("@name", name);
            object? result = cmd.ExecuteScalar();
                if (result != null)
                {
                    int charId = Convert.ToInt32(result);
                    var inspect = new HeroInspectForm(_userId, charId, _hiredMembers.Contains(name));
                    inspect.FormClosed += (_, __) =>
                    {
                        LoadPartyData();
                        inspect.Dispose();
                    };
                    inspect.Show(this);
                }
        }

        private void ConfirmFire(string name, Action onConfirm)
        {
            var confirm = new Form
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Confirm Fire",
                StartPosition = FormStartPosition.CenterParent
            };

            var lbl = new Label { Left = 10, Top = 10, Width = 260, Text = $"Type '{name}' to confirm firing:" };
            var txt = new TextBox { Left = 10, Top = 40, Width = 260 };
            var btn = new Button { Text = "Confirm", Left = 10, Top = 70, Width = 100, Enabled = false };
            btn.Click += (s, e) => { confirm.Close(); onConfirm(); };
            txt.TextChanged += (s, e) => btn.Enabled = txt.Text == name;

            confirm.Controls.Add(lbl);
            confirm.Controls.Add(txt);
            confirm.Controls.Add(btn);
            confirm.AcceptButton = btn;
            confirm.FormClosed += (_, __) => confirm.Dispose();
            confirm.Show(this);
        }

        private void btnFire_Click(object? sender, EventArgs e)
        {
            if (lstParty.SelectedItem == null) return;
            string item = lstParty.SelectedItem.ToString() ?? string.Empty;
            string name = item.Split(" - ")[0];

            if (_hiredMembers.Contains(name))
            {
                MessageBox.Show("This hero is currently hired and cannot be fired.");
                return;
            }

            ConfirmFire(name, () => FireHero(name));
        }

        private void FireHero(string name)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT id, level FROM characters WHERE account_id=@id AND name=@name AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            cmd.Parameters.AddWithValue("@name", name);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return;
            int charId = reader.GetInt32("id");
            int level = reader.GetInt32("level");
            reader.Close();

            int hireCost = 10 + level * 10;
            int refund = (int)(hireCost * 0.4);

            InventoryService.Load(_userId);
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                var eqItem = InventoryService.GetEquippedItem(name, slot);
                if (eqItem != null)
                {
                    InventoryService.AddItem(eqItem);
                    InventoryService.ConsumeEquipped(name, slot);
                }
            }

            using (var delAb = new MySqlCommand("DELETE FROM character_abilities WHERE character_id=@cid", conn))
            {
                delAb.Parameters.AddWithValue("@cid", charId);
                delAb.ExecuteNonQuery();
            }
            using (var delSlots = new MySqlCommand("DELETE FROM character_ability_slots WHERE character_id=@cid", conn))
            {
                delSlots.Parameters.AddWithValue("@cid", charId);
                delSlots.ExecuteNonQuery();
            }
            using (var delPass = new MySqlCommand("DELETE FROM character_passives WHERE character_id=@cid", conn))
            {
                delPass.Parameters.AddWithValue("@cid", charId);
                delPass.ExecuteNonQuery();
            }
            using (var delChar = new MySqlCommand("DELETE FROM characters WHERE id=@cid", conn))
            {
                delChar.Parameters.AddWithValue("@cid", charId);
                delChar.ExecuteNonQuery();
            }

            using (var goldCmd = new MySqlCommand("UPDATE users SET gold = gold + @g WHERE id=@id", conn))
            {
                goldCmd.Parameters.AddWithValue("@g", refund);
                goldCmd.Parameters.AddWithValue("@id", _userId);
                goldCmd.ExecuteNonQuery();
            }

            MessageBox.Show($"{name} has been dismissed. You receive {refund} gold.");
            LoadPartyData();
        }

        private void btnLogs_Click(object? sender, EventArgs e)
        {
            var logs = new BattleLogForm();
            logs.FormClosed += (_, __) => logs.Dispose();
            logs.Show(this);
        }

        private void btnNavigate_Click(object? sender, EventArgs e)
        {
            bool hasBlessing = false;
            using (var conn = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                using var cmd = new MySqlCommand("SELECT faster_travel FROM travel_state WHERE account_id=@a", conn);
                cmd.Parameters.AddWithValue("@a", _userId);
                hasBlessing = Convert.ToBoolean(cmd.ExecuteScalar() ?? 0);
            }
            var nav = new NavigationWindow(_userId, lstParty.Items.Count, hasBlessing, LoadPartyData);
            nav.FormClosed += (_, __) =>
            {
                LoadPartyData();
                nav.Dispose();
            };
            nav.Show(this);
        }

        private void btnMail_Click(object? sender, EventArgs e)
        {
            var box = new MailboxForm(_userId);
            box.FormClosed += (_, __) => box.Dispose();
            box.Show(this);
        }

        private void btnInventory_Click(object? sender, EventArgs e)
        {
            var inv = new InventoryForm(_userId);
            inv.FormClosed += (_, __) =>
            {
                LoadPartyData();
                inv.Dispose();
            };
            inv.Show(this);
        }

        private void Regenerate()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(
                "UPDATE characters SET " +
                "current_hp = LEAST(max_hp, current_hp + 5 + CEILING(max_hp*0.05)), " +
                "mana = LEAST(10 + 5*intelligence, mana + 5 + CEILING((10 + 5*intelligence)*0.05)) " +
                "WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0 AND current_hp>0 AND (current_hp < max_hp OR mana < (10 + 5*intelligence))",
                conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            cmd.ExecuteNonQuery();

            int selectedIndex = lstParty.SelectedIndex;
            LoadPartyData();
            if (selectedIndex >= 0 && selectedIndex < lstParty.Items.Count)
            {
                lstParty.SelectedIndex = selectedIndex;
            }
        }

        private void RegenTimer_Tick(object? sender, EventArgs e)
        {
            Regenerate();
        }

        private void ChatTimer_Tick(object? sender, EventArgs e)
        {
            ChatService.UpdateLastSeen(_userId);
            var messages = ChatService.GetMessages(_lastMessage, _userId);
            foreach (var m in messages)
            {
                string prefix = m.Recipient == null ? string.Empty : $"[PM to {m.Recipient}] ";
                txtChatDisplay.AppendText($"[{m.SentAt:HH:mm:ss}] {m.Sender}: {prefix}{m.Message}\r\n");
                _lastMessage = m.SentAt;
            }
            lstOnline.DataSource = ChatService.GetOnlinePlayers();
            lstFriends.DataSource = FriendService.GetFriends(_userId);
            lstFriendRequests.DataSource = FriendService.GetFriendRequests(_userId);
        }

        private void btnChatSend_Click(object? sender, EventArgs e)
        {
            string msg = txtChatInput.Text.Trim();
            if (msg.Length == 0) return;
            int? recipient = null;
            if (lstOnline.SelectedItem != null && lstOnline.SelectedItem.ToString() != _nickname)
            {
                recipient = ChatService.GetUserIdByNickname(lstOnline.SelectedItem.ToString() ?? string.Empty);
            }
            ChatService.SendMessage(_userId, recipient, msg);
            txtChatInput.Clear();
        }

        private void btnAddFriend_Click(object? sender, EventArgs e)
        {
            string nick = txtFriendNick.Text.Trim();
            int? target = ChatService.GetUserIdByNickname(nick);
            if (target.HasValue)
            {
                FriendService.SendFriendRequest(_userId, target.Value);
                txtFriendNick.Clear();
            }
        }

        private void btnAcceptFriend_Click(object? sender, EventArgs e)
        {
            if (lstFriendRequests.SelectedItem == null) return;
            string nick = lstFriendRequests.SelectedItem.ToString() ?? string.Empty;
            int? requester = ChatService.GetUserIdByNickname(nick);
            if (requester.HasValue)
            {
                FriendService.AcceptFriendRequest(_userId, requester.Value);
            }
        }
    }
}
