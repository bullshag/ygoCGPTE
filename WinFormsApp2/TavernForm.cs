using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public class TavernForm : Form
    {
        private readonly int _accountId;
        private readonly Action _onUpdate;

        public TavernForm(int accountId, Action onUpdate)
        {
            _accountId = accountId;
            _onUpdate = onUpdate;
            Text = "Tavern";
            Width = 280;
            Height = 200;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;

            var btnRecruit = new Button { Text = "Find Recruits", Left = 50, Top = 20, Width = 160 };
            btnRecruit.Click += BtnRecruit_Click;
            Controls.Add(btnRecruit);

            var btnJoin = new Button { Text = "Join Another Party", Left = 50, Top = 60, Width = 160 };
            btnJoin.Click += (s, e) => MessageBox.Show("Joining parties not yet implemented.");
            Controls.Add(btnJoin);

            var btnHireOut = new Button { Text = "Leave Party for Hire", Left = 50, Top = 100, Width = 160 };
            btnHireOut.Click += (s, e) => MessageBox.Show("Hiring out your party not yet implemented.");
            Controls.Add(btnHireOut);
        }

        private void BtnRecruit_Click(object? sender, EventArgs e)
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
            using var recruitForm = new RecruitForm(_accountId, candidates, searchCost, _onUpdate);
            recruitForm.ShowDialog(this);
        }
    }
}
