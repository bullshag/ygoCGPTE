using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public class HeroInspectForm : Form
    {
        private readonly int _userId;
        private readonly int _characterId;
        private Label lblStats = new Label();
        private Button btnLevelUp = new Button();
        private ComboBox cmbRole = new ComboBox();
        private ComboBox cmbTarget = new ComboBox();

        public HeroInspectForm(int userId, int characterId)
        {
            _userId = userId;
            _characterId = characterId;
            Text = "Hero Details";
            Width = 300;
            Height = 260;

            lblStats.Left = 10;
            lblStats.Top = 10;
            lblStats.Width = 260;
            lblStats.Height = 150;
            Controls.Add(lblStats);

            cmbRole.Left = 10;
            cmbRole.Top = 170;
            cmbRole.Width = 120;
            cmbRole.Items.AddRange(new[] { "Tank", "Healer", "DPS" });
            cmbRole.SelectedIndexChanged += CmbRole_SelectedIndexChanged;
            Controls.Add(cmbRole);

            cmbTarget.Left = 150;
            cmbTarget.Top = 170;
            cmbTarget.Width = 140;
            cmbTarget.SelectedIndexChanged += CmbTarget_SelectedIndexChanged;
            Controls.Add(cmbTarget);

            btnLevelUp.Text = "Level Up";
            btnLevelUp.Left = 10;
            btnLevelUp.Top = 200;
            btnLevelUp.Click += BtnLevelUp_Click;
            Controls.Add(btnLevelUp);

            Load += HeroInspectForm_Load;
        }

        private void HeroInspectForm_Load(object? sender, EventArgs e)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT name, level, experience_points, strength, dex, intelligence, current_hp, max_hp, role, targeting_style FROM characters WHERE id=@cid AND account_id=@uid", conn);
            cmd.Parameters.AddWithValue("@cid", _characterId);
            cmd.Parameters.AddWithValue("@uid", _userId);
            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string name = reader.GetString("name");
                int level = reader.GetInt32("level");
                int exp = reader.GetInt32("experience_points");
                int str = reader.GetInt32("strength");
                int dex = reader.GetInt32("dex");
                int intel = reader.GetInt32("intelligence");
                int hp = reader.GetInt32("current_hp");
                int maxHp = reader.GetInt32("max_hp");
                int nextExp = ExperienceHelper.GetNextLevelRequirement(level);
                lblStats.Text = $"{name}\nLevel: {level}\nEXP: {exp}/{nextExp}\nHP: {hp}/{maxHp}\nSTR: {str}\nDEX: {dex}\nINT: {intel}";
                btnLevelUp.Enabled = exp >= nextExp;

                string role = reader.GetString("role");
                string targeting = reader.GetString("targeting_style");
                cmbRole.SelectedItem = role;
                LoadTargetOptions(role);
                cmbTarget.SelectedItem = targeting;
            }
        }

        private void BtnLevelUp_Click(object? sender, EventArgs e)
        {
            using var form = new LevelUpForm(_userId, _characterId);
            form.ShowDialog(this);
            HeroInspectForm_Load(null, EventArgs.Empty);
        }

        private void CmbRole_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string role = cmbRole.SelectedItem?.ToString() ?? "DPS";
            LoadTargetOptions(role);
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET role=@r WHERE id=@cid", conn);
            cmd.Parameters.AddWithValue("@r", role);
            cmd.Parameters.AddWithValue("@cid", _characterId);
            cmd.ExecuteNonQuery();
        }

        private void CmbTarget_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string targeting = cmbTarget.SelectedItem?.ToString() ?? "no priorities";
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE characters SET targeting_style=@t WHERE id=@cid", conn);
            cmd.Parameters.AddWithValue("@t", targeting);
            cmd.Parameters.AddWithValue("@cid", _characterId);
            cmd.ExecuteNonQuery();
        }

        private void LoadTargetOptions(string role)
        {
            cmbTarget.Items.Clear();
            switch (role)
            {
                case "Healer":
                    cmbTarget.Items.AddRange(new[] { "prioritize lowest health ally", "prioritize different allies each turn", "prioritize self", "no priorities" });
                    break;
                case "Tank":
                    cmbTarget.Items.AddRange(new[] { "prioritize strongest foe", "prioritize weakest foe", "prioritize targets that arent attack you", "no priorities" });
                    break;
                default: // DPS
                    cmbTarget.Items.AddRange(new[] { "prioritize target of the strongest tank", "prioritize targets attacking you", "prioritize targets that attack non-tanks", "no priorities" });
                    break;
            }
            if (cmbTarget.Items.Count > 0)
            {
                cmbTarget.SelectedIndex = 0;
            }
        }
    }
}
