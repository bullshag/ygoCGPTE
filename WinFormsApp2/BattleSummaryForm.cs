using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class BattleSummaryForm : Form
    {
        public BattleSummaryForm(IEnumerable<CombatantSummary> players, IEnumerable<CombatantSummary> enemies, bool playersWin, string loot)
        {
            InitializeComponent();
            _list.Items.Add(playersWin ? "Victory" : "Defeat");
            if (!string.IsNullOrWhiteSpace(loot)) _list.Items.Add("Loot: " + loot);
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
