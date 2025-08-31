using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class BattleLogForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstBattles;
        private ListBox lstLog;

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
            lstLog = new ListBox();
            SuspendLayout();
            // 
            // lstBattles
            // 
            lstBattles.FormattingEnabled = true;
            lstBattles.ItemHeight = 15;
            lstBattles.Location = new Point(12, 12);
            lstBattles.Name = "lstBattles";
            lstBattles.Size = new Size(120, 199);
            lstBattles.TabIndex = 1;
            lstBattles.SelectedIndexChanged += lstBattles_SelectedIndexChanged;
            // 
            // lstLog
            // 
            lstLog.FormattingEnabled = true;
            lstLog.ItemHeight = 15;
            lstLog.Location = new Point(138, 12);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(691, 199);
            lstLog.TabIndex = 0;
            // 
            // BattleLogForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(841, 218);
            Controls.Add(lstLog);
            Controls.Add(lstBattles);
            Name = "BattleLogForm";
            Text = "Battle Logs";
            Load += BattleLogForm_Load;
            ResumeLayout(false);
        }
    }
}
