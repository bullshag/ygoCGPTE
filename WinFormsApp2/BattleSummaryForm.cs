using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public class BattleSummaryForm : Form
    {
        private readonly ListBox _list = new();
        private readonly Button _btnContinue = new();

        public BattleSummaryForm(IEnumerable<CombatantSummary> players, IEnumerable<CombatantSummary> enemies)
        {
            Text = "Battle Summary";
            Width = 400;
            Height = 300;
            _list.Dock = DockStyle.Top;
            _list.Height = 230;
            Controls.Add(_list);
            _btnContinue.Text = "Continue";
            _btnContinue.Dock = DockStyle.Bottom;
            _btnContinue.Click += (s, e) => Close();
            Controls.Add(_btnContinue);
            foreach (var p in players)
            {
                _list.Items.Add($"{p.Name} - Damage Done: {p.DamageDone}, Damage Taken: {p.DamageTaken}");
            }
            foreach (var e in enemies)
            {
                _list.Items.Add($"{e.Name} - Damage Done: {e.DamageDone}, Damage Taken: {e.DamageTaken}");
            }
        }
    }
}
