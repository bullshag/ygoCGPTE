using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class NavigationWindow : Form
    {
        private readonly int _accountId;
        private int _partySize;
        private bool _hasBlessing;
        private string _currentNode = "nodeMounttown";
        private readonly TravelManager _travelManager;
        private readonly Action _refresh;

        private class ConnectionItem
        {
            public string Id { get; }
            private readonly string _display;
            public ConnectionItem(string id, string display)
            {
                Id = id;
                _display = display;
            }
            public override string ToString() => _display;
        }

        private class EnemyListItem
        {
            public EnemyInfo Info { get; }
            private readonly string _display;
            public EnemyListItem(EnemyInfo info, int kills)
            {
                Info = info;
                _display = kills >= 25 ? $"{info.Name} {Math.Min(kills,25)}/25" : $"?????? {Math.Min(kills,25)}/25";
            }
            public override string ToString() => _display;
        }

        public NavigationWindow(int accountId, int partySize, bool hasBlessing, Action refresh)
        {
            _accountId = accountId;
            _partySize = partySize;
            _hasBlessing = hasBlessing;
            _refresh = refresh;
            InitializeComponent();
            _travelManager = new TravelManager(_accountId);
            _travelManager.ProgressChanged += TravelManager_ProgressChanged;
            _travelManager.TravelCompleted += TravelManager_TravelCompleted;
            _travelManager.AmbushEncounter += TravelManager_AmbushEncounter;
            _currentNode = GetCurrentNode();
            LoadNode(_currentNode);
            lstConnections.SelectedIndexChanged += LstConnections_SelectedIndexChanged;
            knownEnemyList.SelectedIndexChanged += KnownEnemyList_SelectedIndexChanged;
            btnShop.Click += BtnShop_Click;
            btnGraveyard.Click += BtnGraveyard_Click;
            btnTavern.Click += BtnTavern_Click;
            btnFindEnemies.Click += BtnFindEnemies_Click;
            btnArena.Click += BtnArena_Click;
            btnTemple.Click += BtnTemple_Click;
            toolTip1.SetToolTip(btnShop, "Buy and sell items");
            toolTip1.SetToolTip(btnGraveyard, "View and resurrect fallen heroes");
            toolTip1.SetToolTip(btnTavern, "Visit the tavern");
            toolTip1.SetToolTip(btnFindEnemies, "Search the area for enemies scaled to your party");
            toolTip1.SetToolTip(btnArena, "Enter the battle arena");
            toolTip1.SetToolTip(btnTemple, "Receive divine blessings");
            _travelManager.Resume();
            btnBeginTravel.Enabled = !_travelManager.IsTraveling;
            if (_travelManager.IsTraveling)
            {
                lblTravelInfo.Text = "Traveling...";
            }
        }

        public NavigationWindow() : this(0, 0, false, () => { }) { }

        private string GetCurrentNode()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT current_node FROM travel_state WHERE account_id=@a", conn);
            cmd.Parameters.AddWithValue("@a", _accountId);
            object? result = cmd.ExecuteScalar();
            return result?.ToString() ?? "nodeRiverVillage";
        }

        private void LoadNode(string id)
        {
            _currentNode = id;
            foreach (var rb in Controls.OfType<RadioButton>())
            {
                rb.Checked = rb.Name == id;
            }
            var node = WorldMapService.GetNode(id);
            locationLabel.Text = $"Current Location: {node.Name}";
            var activities = node.Activities;
            btnShop.Enabled = activities.Any(a => a.StartsWith("Shop"));
            btnGraveyard.Enabled = activities.Any(a => a.StartsWith("Graveyard"));
            btnTavern.Enabled = activities.Any(a => a.Contains("Tavern"));
            btnFindEnemies.Enabled = node.MinEnemyLevel.HasValue;
            btnFindEnemies.Text = "Search for Enemies";
            if (id == "nodeDarkSpire")
            {
                var (dsMin, _) = GetDarkSpireBracket();
                int floor = (dsMin - 1) / 5 + 1;
                btnFindEnemies.Text = $"Search for Enemies (Floor {floor})";
            }
            btnArena.Enabled = activities.Any(a => a.Contains("Battle Arena"));
            btnTemple.Enabled = activities.Any(a => a.Contains("Temple"));
            lstConnections.Items.Clear();
            foreach (var (dest, days) in WorldMapService.GetConnections(id))
            {
                lstConnections.Items.Add(new ConnectionItem(dest.Id, $"{dest.Name} - {days} day(s)"));
            }
            travelProgressBar.Value = 0;
            lblTravelInfo.Text = string.Empty;
            rtbNodeDescription.Text = node.Description;
            btnBeginTravel.Text = "Begin Travel";
            PopulateKnownEnemies();
        }

        private void PopulateKnownEnemies()
        {
            knownEnemyList.Items.Clear();
            enemyInfo.Clear();
            var node = WorldMapService.GetNode(_currentNode);
            if (!node.MinEnemyLevel.HasValue) return;
            int min = node.MinEnemyLevel.Value;
            int max = node.MaxEnemyLevel ?? int.MaxValue;
            if (_currentNode == "nodeDarkSpire")
            {
                (min, max) = GetDarkSpireBracket();
            }
            foreach (var info in EnemyKnowledgeService.GetEnemiesForArea(min, max))
            {
                int kills = EnemyKnowledgeService.GetKillCount(_accountId, info.Name);
                knownEnemyList.Items.Add(new EnemyListItem(info, kills));
            }
        }

        private void KnownEnemyList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (knownEnemyList.SelectedItem is EnemyListItem item)
            {
                int kills = EnemyKnowledgeService.GetKillCount(_accountId, item.Info.Name);
                if (kills >= 25)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"Name: {item.Info.Name}");
                    sb.AppendLine($"Power: {item.Info.Power}");
                    sb.AppendLine(item.Info.Description);
                    if (item.Info.Skills.Count > 0)
                    {
                        sb.AppendLine("Skills:");
                        foreach (var abil in item.Info.Skills)
                        {
                            sb.AppendLine($"- {abil.Name}: {abil.Description}");
                        }
                    }
                    enemyInfo.Text = sb.ToString();
                }
                else
                {
                    enemyInfo.Text = "Unknown enemy.";
                }
            }
        }

        private void btnBeginTravel_Click(object? sender, EventArgs e)
        {
            if (lstConnections.SelectedItem is ConnectionItem item)
            {
                _travelManager.StartTravel(_currentNode, item.Id, _partySize, _hasBlessing);
                lblTravelInfo.Text = TravelLogService.GetDepartureFlavor(_currentNode, item.Id);
                btnBeginTravel.Enabled = false;
            }
        }

        private void TravelManager_ProgressChanged(int percent)
        {
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;
            travelProgressBar.Value = percent;
        }

        private void TravelManager_TravelCompleted(string nodeId)
        {
            if (_currentNode == "nodeDarkSpire" && nodeId != "nodeDarkSpire")
            {
                ResetDarkSpireBracket();
            }
            LoadNode(nodeId);
            lblTravelInfo.Text = TravelLogService.GetArrivalFlavor(nodeId);
            btnBeginTravel.Enabled = true;
        }

        private void LstConnections_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstConnections.SelectedItem is ConnectionItem item)
            {
                var dest = WorldMapService.GetNode(item.Id);
                rtbNodeDescription.Text = dest.Description;
                int days = WorldMapService.GetNode(_currentNode).Connections[item.Id];
                if (_hasBlessing && days > 1) days -= 1;
                int cost = days * _partySize * 5;
                btnBeginTravel.Text = $"Travel ({cost}g)";
            }
            else
            {
                rtbNodeDescription.Text = WorldMapService.GetNode(_currentNode).Description;
                btnBeginTravel.Text = "Begin Travel";
            }
        }

        private void BtnShop_Click(object? sender, EventArgs e)
        {
            var shop = new ShopForm(_accountId, _currentNode);
            if (sender is Button btn) btn.Enabled = false;
            shop.FormClosed += (_, __) =>
            {
                _refresh();
                UpdatePartySize();
                if (sender is Button b) b.Enabled = true;
                shop.Dispose();
            };
            shop.Show(this);
        }

        private void BtnGraveyard_Click(object? sender, EventArgs e)
        {
            var grave = new GraveyardForm(_accountId, () => { _refresh(); UpdatePartySize(); });
            if (sender is Button btn) btn.Enabled = false;
            grave.FormClosed += (_, __) =>
            {
                if (sender is Button b) b.Enabled = true;
                grave.Dispose();
            };
            grave.Show(this);
        }

        private void BtnTavern_Click(object? sender, EventArgs e)
        {
            var tavern = new TavernForm(_accountId, () => { _refresh(); UpdatePartySize(); });
            if (sender is Button btn) btn.Enabled = false;
            tavern.FormClosed += (_, __) =>
            {
                if (sender is Button b) b.Enabled = true;
                tavern.Dispose();
            };
            tavern.Show(this);
        }

        private void BtnFindEnemies_Click(object? sender, EventArgs e)
        {
            var node = WorldMapService.GetNode(_currentNode);
            int? min = node.MinEnemyLevel;
            int? max = node.MaxEnemyLevel;
            bool darkSpire = _currentNode == "nodeDarkSpire";
            if (darkSpire)
            {
                (min, max) = GetDarkSpireBracket();
            }
            var battle = new BattleForm(_accountId, areaMinLevel: min, areaMaxLevel: max, darkSpireBattle: darkSpire, areaId: _currentNode);
            if (sender is Button btn) btn.Enabled = false;
            battle.FormClosed += (_, __) =>
            {
                _refresh();
                UpdatePartySize();
                LoadNode(_currentNode);
                if (sender is Button b) b.Enabled = true;
                battle.Dispose();
            };
            battle.Show(this);
        }

        private void BtnArena_Click(object? sender, EventArgs e)
        {
            var arena = new ArenaForm(_accountId);
            if (sender is Button btn) btn.Enabled = false;
            arena.FormClosed += (_, __) =>
            {
                if (sender is Button b) b.Enabled = true;
                arena.Dispose();
            };
            arena.Show(this);
        }

        private void BtnTemple_Click(object? sender, EventArgs e)
        {
            var temple = new TempleForm(_accountId, RefreshBlessing);
            if (sender is Button btn) btn.Enabled = false;
            temple.FormClosed += (_, __) =>
            {
                if (sender is Button b) b.Enabled = true;
                temple.Dispose();
            };
            temple.Show(this);
        }

        private void RefreshBlessing()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT faster_travel FROM travel_state WHERE account_id=@a", conn);
            cmd.Parameters.AddWithValue("@a", _accountId);
            _hasBlessing = Convert.ToBoolean(cmd.ExecuteScalar() ?? 0);
        }

        private void UpdatePartySize()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
            cmd.Parameters.AddWithValue("@id", _accountId);
            _partySize = Convert.ToInt32(cmd.ExecuteScalar());
        }

        private (int min, int max) GetDarkSpireBracket()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT current_min, current_max FROM dark_spire_state WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _accountId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int min = reader.GetInt32("current_min");
                int max = reader.GetInt32("current_max");
                return (min, max);
            }
            reader.Close();
            using var ins = new MySqlCommand("INSERT INTO dark_spire_state(account_id, current_min, current_max) VALUES (@id, 1, 5)", conn);
            ins.Parameters.AddWithValue("@id", _accountId);
            ins.ExecuteNonQuery();
            return (1, 5);
        }

        private void ResetDarkSpireBracket()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("UPDATE dark_spire_state SET current_min=1, current_max=5 WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _accountId);
            cmd.ExecuteNonQuery();
        }

        private void TravelManager_AmbushEncounter()
        {
            lblTravelInfo.Text = "Ambushed by wild enemies!";
            var battle = new BattleForm(_accountId, true, areaId: _currentNode);
            battle.FormClosed += (_, __) =>
            {
                _refresh();
                UpdatePartySize();
                LoadNode(_currentNode);
                _travelManager.ResumeAfterEncounter();
                battle.Dispose();
            };
            // If the navigation window has already been disposed (for example
            // when the user closes the form while travel continues), showing
            // a dialog with this form as the owner will throw an
            // ObjectDisposedException.  Guard against that by only using the
            // form as the owner when it's still alive.
            if (!IsDisposed && IsHandleCreated)
            {
                battle.Show(this);
            }
            else
            {
                battle.Show();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Unsubscribe from travel manager events to avoid callbacks after
            // the form has been disposed.
            _travelManager.ProgressChanged -= TravelManager_ProgressChanged;
            _travelManager.TravelCompleted -= TravelManager_TravelCompleted;
            _travelManager.AmbushEncounter -= TravelManager_AmbushEncounter;
            base.OnFormClosed(e);
        }
    }
}
