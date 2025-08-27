using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public partial class GraveyardForm : Form
    {
        private readonly int _userId;
        private readonly Action _refresh;
        private readonly List<DeadChar> _dead = new();

        private class DeadChar
        {
            public int Id;
            public string Name = string.Empty;
            public int Level;
            public string Cause = string.Empty;
        }

        public GraveyardForm(int userId, Action refresh)
        {
            _userId = userId;
            _refresh = refresh;
            InitializeComponent();
        }

        private void GraveyardForm_Load(object? sender, EventArgs e)
        {
            LoadDead();
        }

        private void LoadDead()
        {
            lstDead.Items.Clear();
            _dead.Clear();
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT id, name, level, cause_of_death FROM characters WHERE account_id=@id AND in_graveyard=1", conn);
            cmd.Parameters.AddWithValue("@id", _userId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var d = new DeadChar
                {
                    Id = r.GetInt32("id"),
                    Name = r.GetString("name"),
                    Level = r.GetInt32("level"),
                    Cause = r.IsDBNull(r.GetOrdinal("cause_of_death")) ? string.Empty : r.GetString("cause_of_death")
                };
                _dead.Add(d);
                int cost = CalculateResCost(d.Level);
                lstDead.Items.Add($"{d.Name} - LVL {d.Level} (Cost: {cost} gold)");
            }
        }

        private void lstDead_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstDead.SelectedIndex >= 0 && lstDead.SelectedIndex < _dead.Count)
            {
                var d = _dead[lstDead.SelectedIndex];
                lblInfo.Text = $"Killed by: {d.Cause}";
                btnResurrect.Enabled = true;
            }
            else
            {
                lblInfo.Text = string.Empty;
                btnResurrect.Enabled = false;
            }
        }

        private void btnResurrect_Click(object? sender, EventArgs e)
        {
            if (lstDead.SelectedIndex < 0) return;
            var d = _dead[lstDead.SelectedIndex];
            int cost = CalculateResCost(d.Level);
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var goldCmd = new MySqlCommand("SELECT gold FROM users WHERE id=@id", conn);
            goldCmd.Parameters.AddWithValue("@id", _userId);
            int gold = Convert.ToInt32(goldCmd.ExecuteScalar());
            if (gold < cost)
            {
                MessageBox.Show($"Need {cost} gold to resurrect.");
                return;
            }
            using var pay = new MySqlCommand("UPDATE users SET gold=gold-@c WHERE id=@id", conn);
            pay.Parameters.AddWithValue("@c", cost);
            pay.Parameters.AddWithValue("@id", _userId);
            pay.ExecuteNonQuery();
            using var res = new MySqlCommand("UPDATE characters SET is_dead=0, in_graveyard=0, in_arena=0, current_hp=max_hp, cause_of_death=NULL, death_time=NULL WHERE id=@cid", conn);
            res.Parameters.AddWithValue("@cid", d.Id);
            res.ExecuteNonQuery();
            MessageBox.Show($"{d.Name} has been resurrected!");
            LoadDead();
            _refresh();
        }

        private int CalculateResCost(int level) => 2 * (10 + level * 10);
    }
}
