using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object? sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using MySqlCommand check = new MySqlCommand("SELECT COUNT(1) FROM Users WHERE Username=@u", conn);
            check.Parameters.AddWithValue("@u", txtUsername.Text);
            int exists = Convert.ToInt32(check.ExecuteScalar());
            if (exists > 0)
            {
                MessageBox.Show("Username already exists");
                return;
            }

            using MySqlCommand insert = new MySqlCommand("INSERT INTO Users (Username, PasswordHash, Gold) VALUES (@u, @p, 300)", conn);
            insert.Parameters.AddWithValue("@u", txtUsername.Text);
            insert.Parameters.AddWithValue("@p", Form1.HashPassword(txtPassword.Text));
            insert.ExecuteNonQuery();

            MessageBox.Show("Account created");
            Close();
        }
    }
}
