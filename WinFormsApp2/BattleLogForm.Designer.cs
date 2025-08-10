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
            lstBattles.ItemHeight = 25;
            lstBattles.Location = new Point(17, 20);
            lstBattles.Margin = new Padding(4, 5, 4, 5);
            lstBattles.Name = "lstBattles";
            lstBattles.Size = new Size(170, 329);
            lstBattles.TabIndex = 1;
            lstBattles.SelectedIndexChanged += lstBattles_SelectedIndexChanged;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(197, 20);
            txtLog.Margin = new Padding(4, 5, 4, 5);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(355, 329);
            txtLog.TabIndex = 0;
            // 
            // BattleLogForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(570, 364);
            Controls.Add(txtLog);
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
