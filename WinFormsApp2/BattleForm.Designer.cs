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
        private ColoredProgressBar hpTemplate;
        private ColoredProgressBar manaTemplate;
        private ColoredProgressBar attackTemplate;

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
            hpTemplate = new ColoredProgressBar();
            manaTemplate = new ColoredProgressBar();
            attackTemplate = new ColoredProgressBar();
            SuspendLayout();
            // 
            // pnlPlayers
            // 
            pnlPlayers.AutoScroll = true;
            pnlPlayers.BackColor = Color.Silver;
            pnlPlayers.FlowDirection = FlowDirection.TopDown;
            pnlPlayers.Location = new Point(9, 8);
            pnlPlayers.Name = "pnlPlayers";
            pnlPlayers.Size = new Size(176, 457);
            pnlPlayers.TabIndex = 2;
            pnlPlayers.WrapContents = false;
            // 
            // pnlEnemies
            // 
            pnlEnemies.AutoScroll = true;
            pnlEnemies.BackColor = Color.FromArgb(224, 224, 224);
            pnlEnemies.FlowDirection = FlowDirection.TopDown;
            pnlEnemies.Location = new Point(734, 8);
            pnlEnemies.Name = "pnlEnemies";
            pnlEnemies.Size = new Size(185, 457);
            pnlEnemies.TabIndex = 1;
            pnlEnemies.WrapContents = false;
            // 
            // lstLog
            // 
            lstLog.DrawMode = DrawMode.OwnerDrawFixed;
            lstLog.Location = new Point(8, 471);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(911, 148);
            lstLog.TabIndex = 0;
            lstLog.DrawItem += lstLog_DrawItem;
            // 
            // hpTemplate
            // 
            hpTemplate.Location = new Point(210, 8);
            hpTemplate.Margin = new Padding(2);
            hpTemplate.Name = "hpTemplate";
            hpTemplate.ProgressColor = Color.Red;
            hpTemplate.Size = new Size(119, 12);
            hpTemplate.Style = ProgressBarStyle.Continuous;
            hpTemplate.TabIndex = 2;
            hpTemplate.Visible = false;
            // 
            // manaTemplate
            // 
            manaTemplate.Location = new Point(210, 26);
            manaTemplate.Margin = new Padding(2);
            manaTemplate.Name = "manaTemplate";
            manaTemplate.ProgressColor = Color.Blue;
            manaTemplate.Size = new Size(119, 12);
            manaTemplate.Style = ProgressBarStyle.Continuous;
            manaTemplate.TabIndex = 1;
            manaTemplate.Visible = false;
            // 
            // attackTemplate
            // 
            attackTemplate.Location = new Point(210, 44);
            attackTemplate.Margin = new Padding(2);
            attackTemplate.Name = "attackTemplate";
            attackTemplate.ProgressColor = Color.Yellow;
            attackTemplate.Size = new Size(119, 12);
            attackTemplate.Style = ProgressBarStyle.Continuous;
            attackTemplate.TabIndex = 0;
            attackTemplate.Visible = false;
            // 
            // BattleForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(931, 631);
            Controls.Add(attackTemplate);
            Controls.Add(manaTemplate);
            Controls.Add(hpTemplate);
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
