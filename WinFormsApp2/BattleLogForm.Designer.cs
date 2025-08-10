using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class BattleLogForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstBattles;
        private TextBox txtLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lstBattles = new ListBox();
            txtLog = new TextBox();
            SuspendLayout();
            //
            // lstBattles
            //
            lstBattles.FormattingEnabled = true;
            lstBattles.ItemHeight = 15;
            lstBattles.Location = new Point(12, 12);
            lstBattles.Size = new Size(120, 199);
            lstBattles.SelectedIndexChanged += lstBattles_SelectedIndexChanged;
            //
            // txtLog
            //
            txtLog.Location = new Point(138, 12);
            txtLog.Multiline = true;
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(250, 199);
            //
            // BattleLogForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 223);
            Controls.Add(txtLog);
            Controls.Add(lstBattles);
            Text = "Battle Logs";
            Load += BattleLogForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
