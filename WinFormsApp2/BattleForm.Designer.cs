using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class BattleForm
    {
        private System.ComponentModel.IContainer? components = null;
        private Label lblPlayer;
        private Label lblNpc;
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
            lblPlayer = new Label();
            lblNpc = new Label();
            lstLog = new ListBox();
            SuspendLayout();
            // lblPlayer
            lblPlayer.AutoSize = true;
            lblPlayer.Location = new Point(12, 9);
            lblPlayer.Name = "lblPlayer";
            lblPlayer.Size = new Size(38, 15);
            lblPlayer.Text = "Player";
            // lblNpc
            lblNpc.AutoSize = true;
            lblNpc.Location = new Point(12, 34);
            lblNpc.Name = "lblNpc";
            lblNpc.Size = new Size(31, 15);
            lblNpc.Text = "NPC";
            // lstLog
            lstLog.FormattingEnabled = true;
            lstLog.ItemHeight = 15;
            lstLog.Location = new Point(12, 60);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(260, 184);
            // BattleForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(284, 261);
            Controls.Add(lstLog);
            Controls.Add(lblNpc);
            Controls.Add(lblPlayer);
            Name = "BattleForm";
            Text = "Battle";
            Load += BattleForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
