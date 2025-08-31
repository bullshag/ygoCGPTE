using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class TempleForm
    {
        private System.ComponentModel.IContainer? components = null;
        private Button btnBless;

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
            btnBless = new Button();
            SuspendLayout();
            // 
            // btnBless
            // 
            btnBless.Location = new Point(40, 30);
            btnBless.Name = "btnBless";
            btnBless.Size = new Size(200, 23);
            btnBless.TabIndex = 0;
            btnBless.Text = "Buy Travel Blessing (100g)";
            btnBless.UseVisualStyleBackColor = true;
            btnBless.Click += btnBless_Click;
            // 
            // TempleForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(280, 150);
            Controls.Add(btnBless);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "TempleForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Temple";
            ResumeLayout(false);
        }
    }
}
