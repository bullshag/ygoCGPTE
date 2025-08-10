using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class BattleForm
    {
        private System.ComponentModel.IContainer? components = null;
        private FlowLayoutPanel pnlPlayers;
        private FlowLayoutPanel pnlEnemies;
        private ListBox lstLog;

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
            pnlPlayers = new FlowLayoutPanel();
            pnlEnemies = new FlowLayoutPanel();
            lstLog = new ListBox();
            SuspendLayout();
            // pnlPlayers
            pnlPlayers.Location = new Point(12, 12);
            pnlPlayers.Size = new Size(200, 120);
            // pnlEnemies
            pnlEnemies.Location = new Point(276, 12);
            pnlEnemies.Size = new Size(200, 120);
            // lstLog
            lstLog.FormattingEnabled = true;
            lstLog.ItemHeight = 15;
            lstLog.Location = new Point(12, 138);
            lstLog.Size = new Size(464, 154);
            // BattleForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(488, 304);
            Controls.Add(lstLog);
            Controls.Add(pnlEnemies);
            Controls.Add(pnlPlayers);
            Name = "BattleForm";
            Text = "Battle";
            Load += BattleForm_Load;
            ResumeLayout(false);
        }
    }
}
