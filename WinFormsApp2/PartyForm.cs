using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class PartyForm : Form
    {
        private readonly int _userId;

        public PartyForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
            LoadPartyData();
        }

        private void LoadPartyData()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using (MySqlCommand cmdGold = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn))
            {
                cmdGold.Parameters.AddWithValue("@id", _userId);
                object? result = cmdGold.ExecuteScalar();
                int gold = result != null ? Convert.ToInt32(result) : 0;
                lblGold.Text = $"Gold: {gold}";
            }

            using (MySqlCommand cmdParty = new MySqlCommand("SELECT name, experience_points FROM characters WHERE account_id=@id", conn))
            {
                cmdParty.Parameters.AddWithValue("@id", _userId);
                using MySqlDataReader reader = cmdParty.ExecuteReader();
                int totalExp = 0;
                lstParty.Items.Clear();
                while (reader.Read())
                {
                    string name = reader.GetString("name");
                    int exp = reader.GetInt32("experience_points");
                    totalExp += exp;
                    lstParty.Items.Add($"{name} (EXP: {exp})");
                }
                lblPartyExp.Text = $"Party EXP: {totalExp}";
                btnHire.Visible = lstParty.Items.Count < 5;
            }
        }

        private void btnHire_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Hire party member functionality coming soon.");
        }
    }
}
