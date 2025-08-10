using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class HeroViewForm : Form
    {
        private readonly RecruitCandidate _candidate;
        private readonly int _userId;
        private readonly int _searchCost;
        private readonly int _hireCost;

        public HeroViewForm(int userId, RecruitCandidate candidate, int searchCost)
        {
            _userId = userId;
            _candidate = candidate;
            _searchCost = searchCost;
            _hireCost = 10 + (int)Math.Ceiling(searchCost * 0.1);
            InitializeComponent();
            txtName.Text = candidate.Name;
            lblStr.Text = $"STR: {candidate.Strength}";
            lblDex.Text = $"DEX: {candidate.Dexterity}";
            lblInt.Text = $"INT: {candidate.Intelligence}";
            btnHire.Text = $"Hire Hero ({_hireCost} gold)";
        }

        private void StatsChanged(object? sender, EventArgs e)
        {
            int spent = (int)numStr.Value + (int)numDex.Value + (int)numInt.Value;
            int remaining = 10 - spent;
            if (remaining < 0)
            {
                var control = (NumericUpDown)sender!;
                control.Value += remaining;
                spent = (int)numStr.Value + (int)numDex.Value + (int)numInt.Value;
                remaining = 10 - spent;
            }
            numStr.Maximum = (int)numStr.Value + remaining;
            numDex.Maximum = (int)numDex.Value + remaining;
            numInt.Maximum = (int)numInt.Value + remaining;
            lblPoints.Text = $"Points left: {remaining}";
        }

        private void btnHire_Click(object? sender, EventArgs e)
        {
            int finalStr = _candidate.Strength + (int)numStr.Value;
            int finalDex = _candidate.Dexterity + (int)numDex.Value;
            int finalInt = _candidate.Intelligence + (int)numInt.Value;
            int hp = 10 + 5 * finalStr;
            int mana = 10 + 5 * finalInt;

            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn);
            goldCmd.Parameters.AddWithValue("@id", _userId);
            int gold = Convert.ToInt32(goldCmd.ExecuteScalar());
            if (gold < _hireCost)
            {
                MessageBox.Show("Not enough gold to hire this hero.");
                return;
            }
            using MySqlCommand updateGold = new MySqlCommand("UPDATE users SET gold=gold-@cost WHERE id=@id", conn);
            updateGold.Parameters.AddWithValue("@cost", _hireCost);
            updateGold.Parameters.AddWithValue("@id", _userId);
            updateGold.ExecuteNonQuery();

            using MySqlCommand insert = new MySqlCommand("INSERT INTO characters(account_id, name, current_hp, max_hp, mana, experience_points, action_speed, strength, dex, intelligence, melee_defense, magic_defense, level, skill_points) VALUES(@acc,@name,@hp,@maxHp,@mana,0,@speed,@str,@dex,@int,0,0,1,0)", conn);
            insert.Parameters.AddWithValue("@acc", _userId);
            string name = txtName.Text.Trim();
            if (name.Contains(' ') || name.Length < 3 || name.Length > 12)
            {
                MessageBox.Show("Name must be 3-12 characters with no spaces");
                return;
            }
            insert.Parameters.AddWithValue("@name", name);
            insert.Parameters.AddWithValue("@hp", hp);
            insert.Parameters.AddWithValue("@maxHp", hp);
            insert.Parameters.AddWithValue("@mana", mana);
            insert.Parameters.AddWithValue("@speed", _candidate.ActionSpeed);
            insert.Parameters.AddWithValue("@str", finalStr);
            insert.Parameters.AddWithValue("@dex", finalDex);
            insert.Parameters.AddWithValue("@int", finalInt);
            insert.ExecuteNonQuery();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
