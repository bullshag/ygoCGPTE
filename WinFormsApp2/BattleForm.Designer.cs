using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class BattleForm
    {
        private System.ComponentModel.IContainer? components = null;
        private FlowLayoutPanel pnlPlayers;
        private FlowLayoutPanel pnlEnemies;
        private RichTextBox rtbLog;

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
            rtbLog = new RichTextBox();
            SuspendLayout();
            // 
            // pnlPlayers
            // 
            pnlPlayers.Location = new Point(13, 14);
            pnlPlayers.Margin = new Padding(4, 5, 4, 5);
            pnlPlayers.Name = "pnlPlayers";
            pnlPlayers.Size = new Size(252, 398);
            pnlPlayers.TabIndex = 2;
            // 
            // pnlEnemies
            // 
            pnlEnemies.Location = new Point(587, 14);
            pnlEnemies.Margin = new Padding(4, 5, 4, 5);
            pnlEnemies.Name = "pnlEnemies";
            pnlEnemies.Size = new Size(264, 398);
            pnlEnemies.TabIndex = 1;
            // 
            // rtbLog
            //
            rtbLog.Location = new Point(13, 422);
            rtbLog.Margin = new Padding(4, 5, 4, 5);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbLog.Size = new Size(838, 254);
            rtbLog.TabIndex = 0;
            rtbLog.Text = "";
            // 
            // BattleForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(864, 690);
            Controls.Add(rtbLog);
            Controls.Add(pnlEnemies);
            Controls.Add(pnlPlayers);
            Margin = new Padding(4, 5, 4, 5);
            Name = "BattleForm";
            Text = "Battle";
            Load += BattleForm_Load;
            ResumeLayout(false);
        }
    }
}
