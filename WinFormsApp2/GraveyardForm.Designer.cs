using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class GraveyardForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstDead;
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
            btnResurrect = new Button();
            lblInfo = new RichTextBox();
            SuspendLayout();
            // 
            // lstDead
            // 
            lstDead.ItemHeight = 25;
            lstDead.Location = new Point(17, 20);
            lstDead.Margin = new Padding(4, 5, 4, 5);
            lstDead.Name = "lstDead";
            lstDead.Size = new Size(370, 329);
            lstDead.TabIndex = 2;
            lstDead.SelectedIndexChanged += lstDead_SelectedIndexChanged;
            // 
            // btnResurrect
            // 
            btnResurrect.Enabled = false;
            btnResurrect.Location = new Point(16, 424);
            btnResurrect.Margin = new Padding(4, 5, 4, 5);
            btnResurrect.Name = "btnResurrect";
            btnResurrect.Size = new Size(371, 38);
            btnResurrect.TabIndex = 0;
            btnResurrect.Text = "Resurrect";
            btnResurrect.UseVisualStyleBackColor = true;
            btnResurrect.Click += btnResurrect_Click;
            // 
            // lblInfo
            // 
            lblInfo.Location = new Point(17, 357);
            lblInfo.Name = "lblInfo";
            lblInfo.ReadOnly = true;
            lblInfo.Size = new Size(370, 59);
            lblInfo.TabIndex = 3;
            lblInfo.Text = "";
            // 
            // GraveyardForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(408, 476);
            Controls.Add(lblInfo);
            Controls.Add(btnResurrect);
            Controls.Add(lstDead);
            Margin = new Padding(4, 5, 4, 5);
            Name = "GraveyardForm";
            Text = "Graveyard";
            Load += GraveyardForm_Load;
            ResumeLayout(false);
        }

        private RichTextBox lblInfo;
    }
}
