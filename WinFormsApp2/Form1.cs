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
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT id FROM Users WHERE Username=@u AND PasswordHash=@p", conn);
            cmd.Parameters.AddWithValue("@u", txtUsername.Text);
            cmd.Parameters.AddWithValue("@p", HashPassword(txtPassword.Text));
            object? result = cmd.ExecuteScalar();
            if (result != null)
            {
                int userId = Convert.ToInt32(result);
                RPGForm rpg = new RPGForm(userId);
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
