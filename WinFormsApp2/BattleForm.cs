using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class BattleForm : Form
    {
        private readonly Creature _player = new();
        private readonly Creature _npc = new();
        private readonly Timer _playerTimer = new();
        private readonly Timer _npcTimer = new();
        private readonly int _userId;

        public BattleForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using var cmd = new MySqlCommand("SELECT name, current_hp, max_hp, strength, dex, action_speed, melee_defense FROM characters WHERE account_id=@id LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            using (var r = cmd.ExecuteReader())
            {
                if (r.Read())
                {
                    _player.Name = r.GetString("name");
                    _player.CurrentHp = r.GetInt32("current_hp");
                    _player.MaxHp = r.GetInt32("max_hp");
                    _player.Strength = r.GetInt32("strength");
                    _player.Dex = r.GetInt32("dex");
                    _player.ActionSpeed = r.GetInt32("action_speed");
                    _player.MeleeDefense = r.GetInt32("melee_defense");
                }
            }

            using var npcCmd = new MySqlCommand("SELECT name, current_hp, max_hp, strength, dex, action_speed, melee_defense FROM npcs ORDER BY RAND() LIMIT 1", conn);
            using (var r2 = npcCmd.ExecuteReader())
            {
                if (r2.Read())
                {
                    _npc.Name = r2.GetString("name");
                    _npc.CurrentHp = r2.GetInt32("current_hp");
                    _npc.MaxHp = r2.GetInt32("max_hp");
                    _npc.Strength = r2.GetInt32("strength");
                    _npc.Dex = r2.GetInt32("dex");
                    _npc.ActionSpeed = r2.GetInt32("action_speed");
                    _npc.MeleeDefense = r2.GetInt32("melee_defense");
                }
            }
        }

        private void BattleForm_Load(object? sender, EventArgs e)
        {
            UpdateLabels();
            StartTimers();
        }

        private void UpdateLabels()
        {
            lblPlayer.Text = $"{_player.Name}: {_player.CurrentHp}/{_player.MaxHp} HP";
            lblNpc.Text = $"{_npc.Name}: {_npc.CurrentHp}/{_npc.MaxHp} HP";
        }

        private void StartTimers()
        {
            _playerTimer.Interval = (int)(3000 / (_player.ActionSpeed + _player.Dex / 25.0));
            _npcTimer.Interval = (int)(3000 / (_npc.ActionSpeed + _npc.Dex / 25.0));
            _playerTimer.Tick += PlayerAction;
            _npcTimer.Tick += NpcAction;
            _playerTimer.Start();
            _npcTimer.Start();
        }

        private void PlayerAction(object? sender, EventArgs e)
        {
            int dmg = Math.Max(1, _player.Strength - _npc.MeleeDefense);
            _npc.CurrentHp -= dmg;
            lstLog.Items.Add($"{_player.Name} hits {_npc.Name} for {dmg} damage!");
            CheckEnd();
        }

        private void NpcAction(object? sender, EventArgs e)
        {
            int dmg = Math.Max(1, _npc.Strength - _player.MeleeDefense);
            _player.CurrentHp -= dmg;
            lstLog.Items.Add($"{_npc.Name} hits {_player.Name} for {dmg} damage!");
            CheckEnd();
        }

        private void CheckEnd()
        {
            UpdateLabels();
            if (_player.CurrentHp <= 0 || _npc.CurrentHp <= 0)
            {
                _playerTimer.Stop();
                _npcTimer.Stop();
                string winner = _player.CurrentHp > 0 ? _player.Name : _npc.Name;
                lstLog.Items.Add($"{winner} wins!");
            }
        }

        private class Creature
        {
            public string Name { get; set; } = string.Empty;
            public int MaxHp { get; set; }
            public int CurrentHp { get; set; }
            public int Strength { get; set; }
            public int Dex { get; set; }
            public int ActionSpeed { get; set; }
            public int MeleeDefense { get; set; }
        }
    }
}
