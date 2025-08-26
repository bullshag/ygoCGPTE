using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    /// <summary>
    /// Displays active quests with ability to abandon entries.
    /// </summary>
    public class QuestLogForm : Form
    {
        private readonly ListBox _list = new ListBox();
        private readonly Button _abandon = new Button();

        public QuestLogForm()
        {
            Text = "Quest Log";
            Width = 300;
            Height = 400;
            _list.Dock = DockStyle.Fill;
            _abandon.Text = "Abandon";
            _abandon.Dock = DockStyle.Bottom;
            _abandon.Click += (s, e) => { if (_list.SelectedIndex >= 0) _list.Items.RemoveAt(_list.SelectedIndex); };
            Controls.Add(_list);
            Controls.Add(_abandon);
        }

        public void LoadQuests(IEnumerable<string> quests)
        {
            _list.Items.Clear();
            foreach (var q in quests) _list.Items.Add(q);
        }
    }
}
