using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    /// <summary>
    /// Displays active quests with ability to abandon entries.
    /// </summary>
    public partial class QuestLogForm : Form
    {
        public QuestLogForm()
        {
            InitializeComponent();
        }

        private void btnAbandon_Click(object? sender, System.EventArgs e)
        {
            if (lstQuests.SelectedIndex >= 0)
                lstQuests.Items.RemoveAt(lstQuests.SelectedIndex);
        }

        public void LoadQuests(IEnumerable<string> quests)
        {
            lstQuests.Items.Clear();
            foreach (var q in quests) lstQuests.Items.Add(q);
        }
    }
}
