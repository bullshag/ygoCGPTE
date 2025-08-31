using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class BattleSummaryForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox _list;
        private Button _btnContinue;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _list = new ListBox();
            _btnContinue = new Button();
            SuspendLayout();
            // 
            // _list
            // 
            _list.Dock = DockStyle.Top;
            _list.Height = 230;
            // 
            // _btnContinue
            // 
            _btnContinue.Text = "Continue";
            _btnContinue.Dock = DockStyle.Bottom;
            _btnContinue.Click += (s, e) => Close();
            // 
            // BattleSummaryForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(450, 300);
            Controls.Add(_btnContinue);
            Controls.Add(_list);
            Name = "BattleSummaryForm";
            Text = "Battle Summary";
            ResumeLayout(false);
        }
    }
}
