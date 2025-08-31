using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class ArenaForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox _lstTeams;
        private Button _btnChallenge;
        private Button _btnDeposit;
        private Label _lblStatus;
        private ToolTip _tip;

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
            components = new System.ComponentModel.Container();
            _lblStatus = new Label();
            _lstTeams = new ListBox();
            _btnChallenge = new Button();
            _btnDeposit = new Button();
            _tip = new ToolTip(components);
            SuspendLayout();
            // 
            // _lblStatus
            // 
            _lblStatus.AutoSize = true;
            _lblStatus.Location = new Point(10, 10);
            // 
            // _lstTeams
            // 
            _lstTeams.Location = new Point(10, 40);
            _lstTeams.Size = new Size(260, 140);
            _lstTeams.SelectedIndexChanged += LstTeams_SelectedIndexChanged;
            _lstTeams.MouseMove += LstTeams_MouseMove;
            // 
            // _btnChallenge
            // 
            _btnChallenge.Location = new Point(30, 190);
            _btnChallenge.Size = new Size(100, 23);
            _btnChallenge.Text = "Challenge";
            _btnChallenge.Click += BtnChallenge_Click;
            // 
            // _btnDeposit
            // 
            _btnDeposit.Location = new Point(150, 190);
            _btnDeposit.Size = new Size(100, 23);
            _btnDeposit.Text = "Deposit";
            _btnDeposit.Click += BtnDeposit_Click;
            // 
            // ArenaForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(300, 260);
            Controls.Add(_lblStatus);
            Controls.Add(_lstTeams);
            Controls.Add(_btnChallenge);
            Controls.Add(_btnDeposit);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Battle Arena";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
