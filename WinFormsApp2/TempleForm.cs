using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public class TempleForm : Form
    {
        private readonly int _accountId;
        private readonly Action _onBlessing;

        public TempleForm(int accountId, Action onBlessing)
        {
            _accountId = accountId;
            _onBlessing = onBlessing;
            Text = "Temple";
            Width = 280;
            Height = 150;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;

            var btnBless = new Button { Text = "Buy Travel Blessing (100g)", Left = 40, Top = 30, Width = 200 };
            btnBless.Click += BtnBless_Click;
            Controls.Add(btnBless);
        }

        private void BtnBless_Click(object? sender, EventArgs e)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn);
            goldCmd.Parameters.AddWithValue("@id", _accountId);
            int gold = Convert.ToInt32(goldCmd.ExecuteScalar() ?? 0);
            if (gold < 100)
            {
                MessageBox.Show("Not enough gold.");
                return;
            }
            using (var pay = new MySqlCommand("UPDATE users SET gold = gold - 100 WHERE id=@id", conn))
            {
                pay.Parameters.AddWithValue("@id", _accountId);
                pay.ExecuteNonQuery();
            }
            using (var bless = new MySqlCommand("UPDATE travel_state SET faster_travel=1 WHERE account_id=@a", conn))
            {
                bless.Parameters.AddWithValue("@a", _accountId);
                bless.ExecuteNonQuery();
            }
            _onBlessing();
            MessageBox.Show("The temple grants you swiftness for your next journey.");
            Close();
        }
    }
}
