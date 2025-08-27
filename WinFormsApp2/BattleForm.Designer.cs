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
        private ProgressBar hpTemplate;
        private ProgressBar manaTemplate;
        private ProgressBar attackTemplate;

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
            hpTemplate = new ProgressBar();
            manaTemplate = new ProgressBar();
            attackTemplate = new ProgressBar();
            SuspendLayout();
            // 
            // pnlPlayers
            // 
            pnlPlayers.Location = new Point(13, 14);
            pnlPlayers.Margin = new Padding(4, 5, 4, 5);
            pnlPlayers.Name = "pnlPlayers";
            pnlPlayers.Size = new Size(252, 398);
            pnlPlayers.TabIndex = 2;
            pnlPlayers.AutoScroll = true;
            pnlPlayers.FlowDirection = FlowDirection.TopDown;
            pnlPlayers.WrapContents = false;
            // 
            // pnlEnemies
            // 
            pnlEnemies.Location = new Point(587, 14);
            pnlEnemies.Margin = new Padding(4, 5, 4, 5);
            pnlEnemies.Name = "pnlEnemies";
            pnlEnemies.Size = new Size(264, 398);
            pnlEnemies.TabIndex = 1;
            pnlEnemies.AutoScroll = true;
            pnlEnemies.FlowDirection = FlowDirection.TopDown;
            pnlEnemies.WrapContents = false;
            // 
            // lstLog
            //
            lstLog.DrawMode = DrawMode.OwnerDrawFixed;
            lstLog.Location = new Point(13, 422);
            lstLog.Margin = new Padding(4, 5, 4, 5);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(838, 254);
            lstLog.TabIndex = 0;
            lstLog.DrawItem += lstLog_DrawItem;

            // hpTemplate
            //
            hpTemplate.Location = new Point(300, 14);
            hpTemplate.Name = "hpTemplate";
            hpTemplate.Size = new Size(170, 20);
            hpTemplate.Style = ProgressBarStyle.Continuous;
            hpTemplate.Visible = false;

            // manaTemplate
            //
            manaTemplate.Location = new Point(300, 44);
            manaTemplate.Name = "manaTemplate";
            manaTemplate.Size = new Size(170, 20);
            manaTemplate.Style = ProgressBarStyle.Continuous;
            manaTemplate.Visible = false;

            // attackTemplate
            //
            attackTemplate.Location = new Point(300, 74);
            attackTemplate.Name = "attackTemplate";
            attackTemplate.Size = new Size(170, 20);
            attackTemplate.Visible = false;
            // 
            // BattleForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(864, 690);
            Controls.Add(attackTemplate);
            Controls.Add(manaTemplate);
            Controls.Add(hpTemplate);
            Controls.Add(lstLog);
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
