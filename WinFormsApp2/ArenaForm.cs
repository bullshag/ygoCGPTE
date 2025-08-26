using System.Windows.Forms;

namespace WinFormsApp2
{
    public class ArenaForm : Form
    {
        public ArenaForm()
        {
            Text = "Battle Arena";
            Width = 250;
            Height = 150;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            var lbl = new Label { Text = "Arena battles coming soon", AutoSize = true, Left = 40, Top = 40 };
            Controls.Add(lbl);
        }
    }
}
