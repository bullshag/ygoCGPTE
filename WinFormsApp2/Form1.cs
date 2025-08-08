using System.Data.SqlClient;
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
            using SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Username=@u AND PasswordHash=@p", conn);
            cmd.Parameters.AddWithValue("@u", txtUsername.Text);
            cmd.Parameters.AddWithValue("@p", HashPassword(txtPassword.Text));
            int count = (int)cmd.ExecuteScalar();
            if (count == 1)
            {
                MessageBox.Show("Login successful");
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
