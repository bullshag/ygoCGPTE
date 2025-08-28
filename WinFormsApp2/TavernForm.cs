using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class TavernForm : Form
    {
        private readonly int _accountId;
        private readonly Action _onUpdate;
        private int _searchCost;

        public TavernForm(int accountId, Action onUpdate)
        {
            _accountId = accountId;
            _onUpdate = onUpdate;
            InitializeComponent();
            RefreshSearchCost();
        }

        private void btnRecruit_Click(object? sender, EventArgs e)
        {
            _searchCost = CalculateSearchCost(out int partyCount);
            if (partyCount >= 10)
            {
                MessageBox.Show("Party is full. Release a member before recruiting.");
                RefreshSearchCost();
                return;
            }

            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            int playerGold;
            using (var goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn))
            {
                goldCmd.Parameters.AddWithValue("@id", _accountId);
                playerGold = Convert.ToInt32(goldCmd.ExecuteScalar() ?? 0);
            }

            if (playerGold < _searchCost)
            {
                MessageBox.Show("Not enough gold to search for recruits.");
                return;
            }

            using (var payCmd = new MySqlCommand("UPDATE users SET gold = GREATEST(gold - @cost, 0) WHERE id=@id", conn))
            {
                payCmd.Parameters.AddWithValue("@cost", _searchCost);
                payCmd.Parameters.AddWithValue("@id", _accountId);
                payCmd.ExecuteNonQuery();
            }

            var rng = new Random();
            var candidates = new List<RecruitCandidate>();
            for (int i = 0; i < 3; i++)
            {
                candidates.Add(RecruitCandidate.Generate(rng, i));
            }

            using var recruitForm = new RecruitForm(_accountId, candidates, () => _searchCost, OnHire);
            recruitForm.ShowDialog(this);
        }

        private void btnJoin_Click(object? sender, EventArgs e)
        {
            using var window = new HireMultiplayerPartyWindow(_accountId, OnHire);
            window.ShowDialog(this);
        }

        private void btnHireOut_Click(object? sender, EventArgs e)
        {
            using var window = new HireMultiplayerPartyWindow(_accountId, OnHire, showHireOut: true);
            window.ShowDialog(this);
        }

        private void OnHire()
        {
            _onUpdate();
            RefreshSearchCost();
        }

        private void RefreshSearchCost()
        {
            _searchCost = CalculateSearchCost(out int partyCount);
            if (partyCount >= 10)
            {
                btnRecruit.Text = "Party Full";
                btnRecruit.Enabled = false;
            }
            else
            {
                btnRecruit.Text = $"Find Recruits ({_searchCost}g)";
                btnRecruit.Enabled = true;
            }
        }

        private int CalculateSearchCost(out int partyCount)
        {
            partyCount = 0;
            int totalLevel = 0;
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT level FROM characters WHERE account_id=@id AND is_dead=0 AND in_arena=0 AND in_tavern=0", conn);
            cmd.Parameters.AddWithValue("@id", _accountId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                partyCount++;
                totalLevel += reader.GetInt32("level");
            }
            return partyCount == 0 ? 0 : 100 + totalLevel * 10 + partyCount * 20;
        }
    }
}
