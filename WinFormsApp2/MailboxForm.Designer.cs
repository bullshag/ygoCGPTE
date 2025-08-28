using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class MailboxForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstMail;
        private TextBox txtBody;
        private Button btnRefresh;

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
            lstMail = new ListBox();
            txtBody = new TextBox();
            btnRefresh = new Button();
            SuspendLayout();
            // 
            // lstMail
            // 
            lstMail.FormattingEnabled = true;
            lstMail.ItemHeight = 15;
            lstMail.Location = new Point(10, 10);
            lstMail.Name = "lstMail";
            lstMail.Size = new Size(200, 334);
            lstMail.TabIndex = 0;
            lstMail.SelectedIndexChanged += lstMail_SelectedIndexChanged;
            // 
            // txtBody
            // 
            txtBody.Location = new Point(220, 10);
            txtBody.Multiline = true;
            txtBody.ReadOnly = true;
            txtBody.Size = new Size(260, 330);
            txtBody.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(10, 350);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 25);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // MailboxForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 420);
            Controls.Add(btnRefresh);
            Controls.Add(txtBody);
            Controls.Add(lstMail);
            Name = "MailboxForm";
            Text = "Mailbox";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
