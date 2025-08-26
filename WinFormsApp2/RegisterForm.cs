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
            string nick = txtNickname.Text;
            string pass = txtPassword.Text;
            if (user.Contains(' ') || pass.Contains(' ') || nick.Contains(' '))
            {
                MessageBox.Show("No spaces allowed in username, nickname or password");
                return;
            }
            if (user.Length < 3 || user.Length > 12 || pass.Length < 3 || pass.Length > 12 || nick.Length < 3 || nick.Length > 12)
            {
                MessageBox.Show("Username, nickname and password must be 3-12 characters");
                return;
            }
            if (pass != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            using MySqlCommand checkUser = new MySqlCommand("SELECT COUNT(1) FROM Users WHERE Username=@u", conn);
            checkUser.Parameters.AddWithValue("@u", user);
            int existsUser = Convert.ToInt32(checkUser.ExecuteScalar());
            if (existsUser > 0)
            {
                MessageBox.Show("Username already exists");
                return;
            }

            using MySqlCommand checkNick = new MySqlCommand("SELECT COUNT(1) FROM Users WHERE Nickname=@n", conn);
            checkNick.Parameters.AddWithValue("@n", nick);
            int existsNick = Convert.ToInt32(checkNick.ExecuteScalar());
            if (existsNick > 0)
            {
                MessageBox.Show("Nickname already exists");
                return;
            }

            using MySqlCommand insert = new MySqlCommand("INSERT INTO Users (Username, Nickname, PasswordHash, Gold, last_seen) VALUES (@u, @n, @p, 300, NOW())", conn);
            insert.Parameters.AddWithValue("@u", user);
            insert.Parameters.AddWithValue("@n", nick);
            insert.Parameters.AddWithValue("@p", Form1.HashPassword(pass));
            insert.ExecuteNonQuery();
            long newId = insert.LastInsertedId;

            using MySqlCommand initTravel = new MySqlCommand("REPLACE INTO travel_state(account_id,current_node,destination_node,start_time,arrival_time,progress_seconds,faster_travel,travel_cost) VALUES (@a,@node,@node,NULL,NULL,0,0,0)", conn);
            initTravel.Parameters.AddWithValue("@a", newId);
            initTravel.Parameters.AddWithValue("@node", "nodeRiverVillage");
            initTravel.ExecuteNonQuery();

            MessageBox.Show("Account created");
            Close();
        }
    }
}
