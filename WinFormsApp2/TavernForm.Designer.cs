using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class TavernForm
    {
        private System.ComponentModel.IContainer? components = null;
        private Button btnRecruit;
        private Button btnJoin;
        private Button btnHireOut;

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
            btnRecruit = new Button();
            btnJoin = new Button();
            btnHireOut = new Button();
            SuspendLayout();
            // 
            // btnRecruit
            // 
            btnRecruit.Location = new Point(50, 20);
            btnRecruit.Name = "btnRecruit";
            btnRecruit.Size = new Size(160, 23);
            btnRecruit.TabIndex = 0;
            btnRecruit.Text = "Find Recruits";
            btnRecruit.UseVisualStyleBackColor = true;
            btnRecruit.Click += btnRecruit_Click;
            // 
            // btnJoin
            // 
            btnJoin.Location = new Point(50, 60);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(160, 23);
            btnJoin.TabIndex = 1;
            btnJoin.Text = "Join/Start Raiding Party";
            btnJoin.UseVisualStyleBackColor = true;
            btnJoin.Click += btnJoin_Click;
            // 
            // btnHireOut
            // 
            btnHireOut.Location = new Point(50, 100);
            btnHireOut.Name = "btnHireOut";
            btnHireOut.Size = new Size(160, 23);
            btnHireOut.TabIndex = 2;
            btnHireOut.Text = "Leave Party for Hire";
            btnHireOut.UseVisualStyleBackColor = true;
            btnHireOut.Click += btnHireOut_Click;
            // 
            // TavernForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(280, 200);
            Controls.Add(btnHireOut);
            Controls.Add(btnJoin);
            Controls.Add(btnRecruit);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "TavernForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Tavern";
            ResumeLayout(false);
        }
    }
}
