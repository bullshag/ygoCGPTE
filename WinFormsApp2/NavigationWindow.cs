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
            btnShop.Click += BtnShop_Click;
            btnGraveyard.Click += BtnGraveyard_Click;
            btnTavern.Click += BtnTavern_Click;
            btnFindEnemies.Click += BtnFindEnemies_Click;
            btnArena.Click += BtnArena_Click;
            btnTemple.Click += BtnTemple_Click;
            toolTip1.SetToolTip(btnShop, "Buy and sell items");
            toolTip1.SetToolTip(btnGraveyard, "View and resurrect fallen heroes");
            toolTip1.SetToolTip(btnTavern, "Visit the tavern");
            toolTip1.SetToolTip(btnFindEnemies, "Search the area for trouble");
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
            btnFindEnemies.Enabled = activities.Any(a => a.StartsWith("Search"));
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
            using var shop = new ShopForm(_accountId);
            shop.ShowDialog(this);
            _refresh();
            UpdatePartySize();
        }

        private void BtnGraveyard_Click(object? sender, EventArgs e)
        {
            using var grave = new GraveyardForm(_accountId, () => { _refresh(); UpdatePartySize(); });
            grave.ShowDialog(this);
        }

        private void BtnTavern_Click(object? sender, EventArgs e)
        {
            using var tavern = new TavernForm(_accountId, () => { _refresh(); UpdatePartySize(); });
            tavern.ShowDialog(this);
        }

        private void BtnFindEnemies_Click(object? sender, EventArgs e)
        {
            using var battle = new BattleForm(_accountId);
            battle.ShowDialog(this);
            _refresh();
            UpdatePartySize();
        }

        private void BtnArena_Click(object? sender, EventArgs e)
        {
            using var arena = new ArenaForm(_accountId);
            arena.ShowDialog(this);
        }

        private void BtnTemple_Click(object? sender, EventArgs e)
        {
            using var temple = new TempleForm(_accountId, RefreshBlessing);
            temple.ShowDialog(this);
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
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM characters WHERE account_id=@id AND is_dead=0", conn);
            cmd.Parameters.AddWithValue("@id", _accountId);
            _partySize = Convert.ToInt32(cmd.ExecuteScalar());
        }

        private void TravelManager_AmbushEncounter()
        {
            lblTravelInfo.Text = "Ambushed by wild enemies!";
            using var battle = new BattleForm(_accountId, true);
            battle.ShowDialog(this);
            _refresh();
            UpdatePartySize();
            _travelManager.ResumeAfterEncounter();
        }
    }
}
