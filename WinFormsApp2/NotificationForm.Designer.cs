using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class NotificationForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstNotifications;
        private Button btnRemove;

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
            lstNotifications = new ListBox();
            btnRemove = new Button();
            SuspendLayout();
            // 
            // lstNotifications
            // 
            lstNotifications.Dock = DockStyle.Fill;
            lstNotifications.FormattingEnabled = true;
            lstNotifications.ItemHeight = 15;
            lstNotifications.Name = "lstNotifications";
            lstNotifications.TabIndex = 0;
            // 
            // btnRemove
            // 
            btnRemove.Dock = DockStyle.Bottom;
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(300, 23);
            btnRemove.TabIndex = 1;
            btnRemove.Text = "Remove";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // NotificationForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(300, 400);
            Controls.Add(lstNotifications);
            Controls.Add(btnRemove);
            Name = "NotificationForm";
            Text = "Notifications";
            ResumeLayout(false);
        }
    }
}
