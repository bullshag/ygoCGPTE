using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class BattleSummaryForm : Form
    {
        public BattleSummaryForm(IEnumerable<CombatantSummary> players, IEnumerable<CombatantSummary> enemies)
        {
            InitializeComponent();
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
