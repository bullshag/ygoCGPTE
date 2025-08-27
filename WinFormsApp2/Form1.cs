using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            DatabaseConfig.DebugMode = chkDebugMode.Checked;
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT id, nickname FROM Users WHERE Username=@u AND PasswordHash=@p", conn);
            cmd.Parameters.AddWithValue("@u", txtUsername.Text);
            cmd.Parameters.AddWithValue("@p", HashPassword(txtPassword.Text));
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int userId = reader.GetInt32("id");
                string nickname = reader.GetString("nickname");
                reader.Close();
                using MySqlCommand seen = new MySqlCommand("UPDATE users SET last_seen=NOW() WHERE id=@id", conn);
                seen.Parameters.AddWithValue("@id", userId);
                seen.ExecuteNonQuery();
                InventoryService.Load(userId);
                RPGForm rpg = new RPGForm(userId, nickname);
                rpg.FormClosed += (s, args) => this.Close();
                rpg.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }
        }

        private void btnCreateAccount_Click(object? sender, EventArgs e)
        {
            using RegisterForm register = new RegisterForm();
            register.ShowDialog();
        }

        internal static string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
