using System;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public class ArenaForm : Form
    {
        private readonly int _userId;
        private int _wins;
        private readonly Label _lblStatus;
        private readonly Button _btnFight;

        public ArenaForm(int userId)
        {
            _userId = userId;
            Text = "Battle Arena";
            Width = 300;
            Height = 200;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;

            _lblStatus = new Label { Text = "Wins: 0", AutoSize = true, Left = 110, Top = 20 };
            _btnFight = new Button { Text = "Fight!", Width = 100, Left = 90, Top = 60 };
            _btnFight.Click += BtnFight_Click;

            Controls.Add(_lblStatus);
            Controls.Add(_btnFight);
        }

        private void BtnFight_Click(object? sender, EventArgs e)
        {
            using var battle = new BattleForm(_userId, arenaBattle: true);
            battle.ShowDialog(this);
            if (battle.Cancelled) return;
            if (battle.PlayersWin)
            {
                _wins++;
                _lblStatus.Text = $"Wins: {_wins}";
            }
            else
            {
                MessageBox.Show($"Defeated after {_wins} win(s).");
                Close();
            }
        }
    }
}
