using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class QuestLogForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstQuests;
        private Button btnAbandon;

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
            lstQuests = new ListBox();
            btnAbandon = new Button();
            SuspendLayout();
            // 
            // lstQuests
            // 
            lstQuests.Dock = DockStyle.Fill;
            lstQuests.FormattingEnabled = true;
            lstQuests.ItemHeight = 15;
            lstQuests.Name = "lstQuests";
            lstQuests.TabIndex = 0;
            // 
            // btnAbandon
            // 
            btnAbandon.Dock = DockStyle.Bottom;
            btnAbandon.Name = "btnAbandon";
            btnAbandon.Size = new Size(300, 23);
            btnAbandon.TabIndex = 1;
            btnAbandon.Text = "Abandon";
            btnAbandon.UseVisualStyleBackColor = true;
            btnAbandon.Click += btnAbandon_Click;
            // 
            // QuestLogForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(300, 400);
            Controls.Add(lstQuests);
            Controls.Add(btnAbandon);
            Name = "QuestLogForm";
            Text = "Quest Log";
            ResumeLayout(false);
        }
    }
}
