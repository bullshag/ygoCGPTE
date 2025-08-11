using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class GraveyardForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstDead;
        private Label lblInfo;
        private Button btnResurrect;

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
            lstDead = new ListBox();
            lblInfo = new Label();
            btnResurrect = new Button();
            SuspendLayout();
            //
            // lstDead
            //
            lstDead.Location = new Point(12, 12);
            lstDead.Size = new Size(260, 199);
            lstDead.SelectedIndexChanged += lstDead_SelectedIndexChanged;
            //
            // lblInfo
            //
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(12, 214);
            lblInfo.Size = new Size(0, 15);
            //
            // btnResurrect
            //
            btnResurrect.Enabled = false;
            btnResurrect.Location = new Point(12, 240);
            btnResurrect.Size = new Size(260, 23);
            btnResurrect.Text = "Resurrect";
            btnResurrect.UseVisualStyleBackColor = true;
            btnResurrect.Click += btnResurrect_Click;
            //
            // GraveyardForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(284, 275);
            Controls.Add(btnResurrect);
            Controls.Add(lblInfo);
            Controls.Add(lstDead);
            Name = "GraveyardForm";
            Text = "Graveyard";
            Load += GraveyardForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
