using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class RPGForm : Form
    {
        private readonly int _userId;

        public RPGForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
        }

        private void RPGForm_Load(object? sender, EventArgs e)
        {
            LoadPartyData();
        }

        private void LoadPartyData()
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using MySqlCommand cmd = new MySqlCommand("SELECT name, experience_points FROM characters WHERE account_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            using MySqlDataReader reader = cmd.ExecuteReader();
            lstParty.Items.Clear();
            int totalExp = 0;
            while (reader.Read())
            {
                string name = reader.GetString("name");
                int exp = reader.GetInt32("experience_points");
                lstParty.Items.Add($"{name} - EXP {exp}");
                totalExp += exp;
            }
            reader.Close();

            lblTotalExp.Text = $"Party EXP: {totalExp}";
            btnHire.Enabled = lstParty.Items.Count < 5;

            using MySqlCommand goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn);
            goldCmd.Parameters.AddWithValue("@id", _userId);
            object? goldResult = goldCmd.ExecuteScalar();
            int gold = goldResult == null ? 0 : Convert.ToInt32(goldResult);
            lblGold.Text = $"Gold: {gold}";
        }

        private void btnHire_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Hire Party Member window coming soon.");
        }
    }
}
