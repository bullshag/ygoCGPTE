using System;
using System.Collections.Generic;
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

        private async void btnLogin_Click(object? sender, EventArgs e)
        {
            DatabaseConfig.DebugMode = chkDebugMode.Checked;
            DatabaseConfig.UseKimServer = kimCheckbox.Checked;
            try
            {
                var rows = await DatabaseClient.QueryAsync(
                    "SELECT id, nickname FROM Users WHERE Username=@u AND PasswordHash=@p",
                    new Dictionary<string, object?>
                    {
                        ["@u"] = txtUsername.Text,
                        ["@p"] = HashPassword(txtPassword.Text)
                    });
                if (rows.Count > 0)
                {
                    int userId = Convert.ToInt32(rows[0]["id"]);
                    string nickname = Convert.ToString(rows[0]["nickname"]) ?? string.Empty;
                    await DatabaseClient.ExecuteAsync("UPDATE users SET last_seen=NOW() WHERE id=@id",
                        new Dictionary<string, object?> { ["@id"] = userId });
                    await InventoryService.LoadAsync(userId);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}");
            }
        }

        private void btnCreateAccount_Click(object? sender, EventArgs e)
        {
            var register = new RegisterForm();
            register.FormClosed += (_, __) => register.Dispose();
            register.Show(this);
        }

        internal static string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private void chkDebugMode_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void kimCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            DatabaseConfig.UseKimServer = kimCheckbox.Checked;
        }
    }
}
