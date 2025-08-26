using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class NavigationWindow : Form
    {
        private readonly int _accountId;
        private readonly int _partySize;
        private bool _hasBlessing;
        private string _currentNode = "nodeMounttown";
        private readonly TravelManager _travelManager;

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

        public NavigationWindow(int accountId, int partySize, bool hasBlessing)
        {
            _accountId = accountId;
            _partySize = partySize;
            _hasBlessing = hasBlessing;
            InitializeComponent();
            _travelManager = new TravelManager(_accountId);
            _travelManager.ProgressChanged += TravelManager_ProgressChanged;
            _travelManager.TravelCompleted += TravelManager_TravelCompleted;
            LoadNode(_currentNode);
            _travelManager.Resume();
        }

        public NavigationWindow() : this(0, 0, false) { }

        private void LoadNode(string id)
        {
            _currentNode = id;
            foreach (var rb in Controls.OfType<RadioButton>())
            {
                rb.Checked = rb.Name == id;
            }
            var node = WorldMapService.GetNode(id);
            lstActivities.Items.Clear();
            foreach (var act in node.Activities)
            {
                lstActivities.Items.Add(act);
            }
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
    }
}
