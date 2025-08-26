using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class RecruitForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstCandidates;
        private Button btnView;

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
            lstCandidates = new ListBox();
            btnView = new Button();
            SuspendLayout();
            // 
            // lstCandidates
            // 
            lstCandidates.FormattingEnabled = true;
            lstCandidates.ItemHeight = 15;
            lstCandidates.Location = new Point(12, 12);
            lstCandidates.Name = "lstCandidates";
            lstCandidates.Size = new Size(200, 94);
            lstCandidates.TabIndex = 1;
            // 
            // btnView
            // 
            btnView.Enabled = false;
            btnView.Location = new Point(12, 112);
            btnView.Name = "btnView";
            btnView.Size = new Size(200, 23);
            btnView.TabIndex = 0;
            btnView.Text = "View Hero";
            btnView.UseVisualStyleBackColor = true;
            btnView.Click += btnView_Click;
            // 
            // RecruitForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(224, 141);
            Controls.Add(btnView);
            Controls.Add(lstCandidates);
            Name = "RecruitForm";
            Text = "Recruit Candidates";
            ResumeLayout(false);
        }
    }
}
