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

        public HeroInspectForm(int userId, int characterId)
        {
            _userId = userId;
            _characterId = characterId;
            Text = "Hero Details";
            Width = 300;
            Height = 250;

            lblStats.Left = 10;
            lblStats.Top = 10;
            lblStats.Width = 260;
            lblStats.Height = 150;
            Controls.Add(lblStats);

            btnLevelUp.Text = "Level Up";
            btnLevelUp.Left = 10;
            btnLevelUp.Top = 170;
            btnLevelUp.Click += BtnLevelUp_Click;
            Controls.Add(btnLevelUp);

            Load += HeroInspectForm_Load;
        }

        private void HeroInspectForm_Load(object? sender, EventArgs e)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT name, level, experience_points, strength, dex, intelligence, current_hp, max_hp FROM characters WHERE id=@cid AND account_id=@uid", conn);
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
            }
        }

        private void BtnLevelUp_Click(object? sender, EventArgs e)
        {
            using var form = new LevelUpForm(_userId, _characterId);
            form.ShowDialog(this);
            HeroInspectForm_Load(null, EventArgs.Empty);
        }
    }
}
