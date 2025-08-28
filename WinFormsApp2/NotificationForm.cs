using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    /// <summary>
    /// Simple expandable notification list. Entries can be removed individually.
    /// </summary>
    public partial class NotificationForm : Form
    {
        public NotificationForm()
        {
            InitializeComponent();
        }

        private void btnRemove_Click(object? sender, System.EventArgs e)
        {
            if (lstNotifications.SelectedIndex >= 0)
                lstNotifications.Items.RemoveAt(lstNotifications.SelectedIndex);
        }

        public void LoadNotifications(IEnumerable<string> notes)
        {
            lstNotifications.Items.Clear();
            foreach (var n in notes) lstNotifications.Items.Add(n);
        }
    }
}
