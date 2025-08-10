using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public class BattleSummaryForm : Form
    {
        private readonly ListBox _list = new ListBox();
        public BattleSummaryForm(IEnumerable<dynamic> players, IEnumerable<dynamic> enemies)
        {
            Text = "Battle Summary";
            Width = 400;
            Height = 300;
            _list.Dock = DockStyle.Fill;
            Controls.Add(_list);
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
