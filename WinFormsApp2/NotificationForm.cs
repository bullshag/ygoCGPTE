using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    /// <summary>
    /// Simple expandable notification list. Entries can be removed
    /// individually.
    /// </summary>
    public class NotificationForm : Form
    {
        private readonly ListBox _list = new ListBox();
        private readonly Button _remove = new Button();

        public NotificationForm()
        {
            Text = "Notifications";
            Width = 300;
            Height = 400;
            _list.Dock = DockStyle.Fill;
            _remove.Text = "Remove";
            _remove.Dock = DockStyle.Bottom;
            _remove.Click += (s, e) => { if (_list.SelectedIndex >= 0) _list.Items.RemoveAt(_list.SelectedIndex); };
            Controls.Add(_list);
            Controls.Add(_remove);
        }

        public void LoadNotifications(IEnumerable<string> notes)
        {
            _list.Items.Clear();
            foreach (var n in notes) _list.Items.Add(n);
        }
    }
}
