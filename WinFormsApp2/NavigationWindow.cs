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
            _currentNode = GetCurrentNode();
            LoadNode(_currentNode);
            btnShop.Click += BtnShop_Click;
            btnGraveyard.Click += BtnGraveyard_Click;
            btnTavern.Click += BtnTavern_Click;
            toolTip1.SetToolTip(btnShop, "Buy and sell items");
            toolTip1.SetToolTip(btnGraveyard, "View and resurrect fallen heroes");
            toolTip1.SetToolTip(btnTavern, "Recruit new party members");
            _travelManager.Resume();
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
            var activities = node.Activities;
            btnShop.Enabled = activities.Any(a => a.StartsWith("Shop"));
            btnGraveyard.Enabled = activities.Any(a => a.StartsWith("Graveyard"));
            btnTavern.Enabled = activities.Any(a => a.Contains("Tavern"));
            lstConnections.Items.Clear();
            foreach (var (dest, days) in WorldMapService.GetConnections(id))
            {
                lstConnections.Items.Add(new ConnectionItem(dest.Id, $"{dest.Name} - {days} day(s)"));
            }
            travelProgressBar.Value = 0;
            lblTravelInfo.Text = string.Empty;
        }

        private void btnBeginTravel_Click(object? sender, EventArgs e)
        {
            if (lstConnections.SelectedItem is ConnectionItem item)
            {
                _travelManager.StartTravel(_currentNode, item.Id, _partySize, _hasBlessing);
                lblTravelInfo.Text = $"Traveling to {item.ToString()}";
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
            lblTravelInfo.Text = "Arrived.";
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
            HandleRecruit();
        }

        private void HandleRecruit()
        {
            int partyCount = 0;
            int totalLevel = 0;
            int playerGold;
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using (var cmd = new MySqlCommand("SELECT level FROM characters WHERE account_id=@id AND is_dead=0", conn))
            {
                cmd.Parameters.AddWithValue("@id", _accountId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    partyCount++;
                    totalLevel += reader.GetInt32("level");
                }
            }
            using (var goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn))
            {
                goldCmd.Parameters.AddWithValue("@id", _accountId);
                playerGold = Convert.ToInt32(goldCmd.ExecuteScalar() ?? 0);
            }

            int searchCost = partyCount == 0 ? 0 : 100 + totalLevel * 10 + partyCount * 20;
            if (playerGold < searchCost)
            {
                MessageBox.Show("Not enough gold to search for recruits.");
                return;
            }

            using (var payCmd = new MySqlCommand("UPDATE users SET gold = gold - @cost WHERE id=@id", conn))
            {
                payCmd.Parameters.AddWithValue("@cost", searchCost);
                payCmd.Parameters.AddWithValue("@id", _accountId);
                payCmd.ExecuteNonQuery();
            }

            var rng = new Random();
            var candidates = new List<RecruitCandidate>();
            for (int i = 0; i < 3; i++)
            {
                candidates.Add(RecruitCandidate.Generate(rng, i));
            }
            using var recruitForm = new RecruitForm(_accountId, candidates, searchCost, () => { _refresh(); UpdatePartySize(); });
            recruitForm.ShowDialog(this);
        }

        private void UpdatePartySize()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM characters WHERE account_id=@id AND is_dead=0", conn);
            cmd.Parameters.AddWithValue("@id", _accountId);
            _partySize = Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
