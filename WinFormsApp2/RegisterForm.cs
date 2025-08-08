using System.Data.SqlClient;
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

            using SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using SqlCommand check = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Username=@u", conn);
            check.Parameters.AddWithValue("@u", txtUsername.Text);
            int exists = (int)check.ExecuteScalar();
            if (exists > 0)
            {
                MessageBox.Show("Username already exists");
                return;
            }

            using SqlCommand insert = new SqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@u, @p)", conn);
            insert.Parameters.AddWithValue("@u", txtUsername.Text);
            insert.Parameters.AddWithValue("@p", Form1.HashPassword(txtPassword.Text));
            insert.ExecuteNonQuery();

            MessageBox.Show("Account created");
            Close();
        }
    }
}
