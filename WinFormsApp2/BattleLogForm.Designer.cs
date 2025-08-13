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
            lstBattles.ItemHeight = 25;
            lstBattles.Location = new Point(17, 20);
            lstBattles.Margin = new Padding(4, 5, 4, 5);
            lstBattles.Name = "lstBattles";
            lstBattles.Size = new Size(170, 329);
            lstBattles.TabIndex = 1;
            lstBattles.SelectedIndexChanged += lstBattles_SelectedIndexChanged;
            // 
            // lstLog
            //
            lstLog.FormattingEnabled = true;
            lstLog.ItemHeight = 25;
            lstLog.Location = new Point(197, 20);
            lstLog.Margin = new Padding(4, 5, 4, 5);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(355, 329);
            lstLog.TabIndex = 0;
            // 
            // BattleLogForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(570, 364);
            Controls.Add(lstLog);
            Controls.Add(lstBattles);
            Margin = new Padding(4, 5, 4, 5);
            Name = "BattleLogForm";
            Text = "Battle Logs";
            Load += BattleLogForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
