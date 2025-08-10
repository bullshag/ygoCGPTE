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
            string user = txtUsername.Text;
            string pass = txtPassword.Text;
            if (user.Contains(' ') || pass.Contains(' '))
            {
                MessageBox.Show("No spaces allowed in username or password");
                return;
            }
            if (user.Length < 3 || user.Length > 12 || pass.Length < 3 || pass.Length > 12)
            {
                MessageBox.Show("Username and password must be 3-12 characters");
                return;
            }
            if (pass != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using MySqlCommand check = new MySqlCommand("SELECT COUNT(1) FROM Users WHERE Username=@u", conn);
            check.Parameters.AddWithValue("@u", user);
            int exists = Convert.ToInt32(check.ExecuteScalar());
            if (exists > 0)
            {
                MessageBox.Show("Username already exists");
                return;
            }

            using MySqlCommand insert = new MySqlCommand("INSERT INTO Users (Username, PasswordHash, Gold) VALUES (@u, @p, 300)", conn);
            insert.Parameters.AddWithValue("@u", user);
            insert.Parameters.AddWithValue("@p", Form1.HashPassword(pass));
            insert.ExecuteNonQuery();

            MessageBox.Show("Account created");
            Close();
        }
    }
}
