using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class ArenaForm : Form
    {
        private readonly int _userId;
        private int _wins;
        private bool _deposited;

        public ArenaForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
            Load += ArenaForm_Load;
        }

        private void ArenaForm_Load(object? sender, EventArgs e)
        {
            RefreshStatus();
            RefreshTeams();
        }

        private void RefreshStatus()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT wins FROM arena_teams WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            object? res = cmd.ExecuteScalar();
            if (res == null)
            {
                _deposited = false;
                _lblStatus.Text = "Your team is not in the arena.";
                _btnDeposit.Text = "Deposit";
                _wins = 0;
            }
            else
            {
                _deposited = true;
                _wins = Convert.ToInt32(res);
                _lblStatus.Text = $"Your team waiting - Wins: {_wins}";
                _btnDeposit.Text = "Withdraw";
            }
        }

        private void RefreshTeams()
        {
            _lstTeams.Items.Clear();
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT at.account_id, at.wins, u.nickname FROM arena_teams at JOIN users u ON at.account_id=u.id WHERE at.account_id!=@id", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var team = new ArenaTeam
                {
                    AccountId = r.GetInt32("account_id"),
                    Wins = r.GetInt32("wins"),
                    Nickname = r.GetString("nickname")
                };
                team.Power = GetTeamPower(team.AccountId);
                _lstTeams.Items.Add(team);
            }
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            _btnChallenge.Enabled = _lstTeams.SelectedItem != null && _lstTeams.Items.Count > 0;
        }

        private void LstTeams_SelectedIndexChanged(object? sender, EventArgs e) => UpdateButtons();

        private void BtnDeposit_Click(object? sender, EventArgs e)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            if (_deposited)
            {
                using var partyCountCmd = new MySqlCommand("SELECT COUNT(*) FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
                partyCountCmd.Parameters.AddWithValue("@id", _userId);
                int partyCount = Convert.ToInt32(partyCountCmd.ExecuteScalar());
                using var arenaCountCmd = new MySqlCommand("SELECT COUNT(*) FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=1", conn);
                arenaCountCmd.Parameters.AddWithValue("@id", _userId);
                int arenaCount = Convert.ToInt32(arenaCountCmd.ExecuteScalar());
                if (partyCount + arenaCount > 10)
                {
                    MessageBox.Show("You need to make room to withdraw your party members.");
                    return;
                }
                using var del = new MySqlCommand("DELETE FROM arena_teams WHERE account_id=@id", conn);
                del.Parameters.AddWithValue("@id", _userId);
                del.ExecuteNonQuery();
                using var updChars = new MySqlCommand("UPDATE characters SET in_arena=0 WHERE account_id=@id AND is_dead=0 AND in_arena=1", conn);
                updChars.Parameters.AddWithValue("@id", _userId);
                updChars.ExecuteNonQuery();
                _deposited = false;
            }
            else
            {
                using var chk = new MySqlCommand("SELECT COUNT(*) FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
                chk.Parameters.AddWithValue("@id", _userId);
                if (Convert.ToInt32(chk.ExecuteScalar()) == 0)
                {
                    MessageBox.Show("No living characters to deposit.");
                    return;
                }
                using var ins = new MySqlCommand("INSERT INTO arena_teams(account_id,wins) VALUES(@id,0) ON DUPLICATE KEY UPDATE wins=wins", conn);
                ins.Parameters.AddWithValue("@id", _userId);
                ins.ExecuteNonQuery();
                using var upd = new MySqlCommand("UPDATE characters SET in_arena=1 WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
                upd.Parameters.AddWithValue("@id", _userId);
                upd.ExecuteNonQuery();
                _deposited = true;
            }
            RefreshStatus();
            RefreshTeams();
        }

        private int GetTeamPower(int accountId)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            int totalLevel;
            using (var lvlCmd = new MySqlCommand("SELECT IFNULL(SUM(level),0) FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=1", conn))
            {
                lvlCmd.Parameters.AddWithValue("@id", accountId);
                totalLevel = Convert.ToInt32(lvlCmd.ExecuteScalar());
            }
            int equipCost = 0;
            using (var eqCmd = new MySqlCommand("SELECT item_name FROM character_equipment WHERE account_id=@id", conn))
            {
                eqCmd.Parameters.AddWithValue("@id", accountId);
                using var er = eqCmd.ExecuteReader();
                while (er.Read())
                {
                    string itemName = er.GetString("item_name");
                    var item = InventoryService.CreateItem(itemName);
                    if (item != null)
                        equipCost += item.Price;
                }
            }
            int skillCount;
            using (var sCmd = new MySqlCommand("SELECT COUNT(*) FROM character_abilities ca JOIN characters c ON ca.character_id=c.id WHERE c.account_id=@id AND c.is_dead=0 AND in_arena=1", conn))
            {
                sCmd.Parameters.AddWithValue("@id", accountId);
                skillCount = Convert.ToInt32(sCmd.ExecuteScalar() ?? 0);
            }
            return (int)Math.Ceiling((totalLevel + equipCost + 3 * skillCount) * 0.15);
        }

        private void LstTeams_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = _lstTeams.IndexFromPoint(e.Location);
            if (index >= 0 && _lstTeams.Items[index] is ArenaTeam team)
            {
                _tip.Show($"Power: {team.Power}", _lstTeams, e.Location + new Size(15, 15));
            }
            else
            {
                _tip.Hide(_lstTeams);
            }
        }

        private void BtnChallenge_Click(object? sender, EventArgs e)
        {
            if (_lstTeams.SelectedItem is not ArenaTeam team) return;
            int playerPower = GetTeamPower(_userId);
            int oppPower = GetTeamPower(team.AccountId);
            var battle = new BattleForm(_userId, arenaBattle: true, arenaOpponentId: team.AccountId);
            battle.FormClosed += (_, __) =>
            {
                if (battle.Cancelled)
                {
                    battle.Dispose();
                    return;
                }
                using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                if (battle.PlayersWin)
                {
                    if (oppPower > playerPower)
                    {
                        InventoryService.AddItem(new ArenaCoin());
                        MessageBox.Show("You earned an Arena Coin!");
                    }
                    using var del = new MySqlCommand("DELETE FROM arena_teams WHERE account_id=@id", conn);
                    del.Parameters.AddWithValue("@id", team.AccountId);
                    del.ExecuteNonQuery();
                    using var kill = new MySqlCommand("UPDATE characters SET is_dead=1, in_graveyard=1, in_arena=0, in_tavern=0, current_hp=0, death_time=NOW(), cause_of_death='Arena defeat' WHERE account_id=@id AND is_dead=0", conn);
                    kill.Parameters.AddWithValue("@id", team.AccountId);
                    kill.ExecuteNonQuery();
                    using var winCmd = new MySqlCommand("UPDATE arena_teams SET wins=wins+1 WHERE account_id=@id", conn);
                    winCmd.Parameters.AddWithValue("@id", _userId);
                    winCmd.ExecuteNonQuery();
                    MessageBox.Show("Victory!");
                }
                else
                {
                    MessageBox.Show("Defeat!");
                }
                var logs = string.Join("\n", BattleLogService.GetLogs());
                string challenger;
                using (var nameCmd = new MySqlCommand("SELECT nickname FROM users WHERE id=@id", conn))
                {
                    nameCmd.Parameters.AddWithValue("@id", _userId);
                    challenger = nameCmd.ExecuteScalar()?.ToString() ?? "Unknown";
                }
                string resultText = battle.PlayersWin ? $"was defeated by {challenger}" : $"defeated {challenger}";
                MailService.SendMail(null, team.AccountId, "Arena Battle Result", $"Your arena team {resultText}.\n\nBattle Log:\n{logs}");
                RefreshStatus();
                RefreshTeams();
                battle.Dispose();
            };
            battle.Show(this);
        }

        private class ArenaTeam
        {
            public int AccountId { get; set; }
            public int Wins { get; set; }
            public int Power { get; set; }
            public string Nickname { get; set; } = string.Empty;
            public override string ToString() => $"{Nickname}'s team (Power {Power}, {Wins} wins)";
        }
    }
}
