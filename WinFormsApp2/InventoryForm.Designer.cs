using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class InventoryForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstItems;
        private Label lblDescription;
        private Button btnUse;

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
            lstItems = new ListBox();
            lblDescription = new Label();
            btnUse = new Button();
            SuspendLayout();
            //
            // lstItems
            //
            lstItems.FormattingEnabled = true;
            lstItems.ItemHeight = 15;
            lstItems.Location = new Point(12, 12);
            lstItems.Size = new Size(150, 199);
            lstItems.SelectedIndexChanged += lstItems_SelectedIndexChanged;
            //
            // lblDescription
            //
            lblDescription.Location = new Point(168, 12);
            lblDescription.Size = new Size(200, 120);
            //
            // btnUse
            //
            btnUse.Location = new Point(168, 135);
            btnUse.Size = new Size(200, 23);
            btnUse.Text = "Use";
            btnUse.Enabled = false;
            btnUse.Click += btnUse_Click;
            //
            // InventoryForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(380, 223);
            Controls.Add(btnUse);
            Controls.Add(lblDescription);
            Controls.Add(lstItems);
            Text = "Inventory";
            Load += InventoryForm_Load;
            ResumeLayout(false);
        }
    }
}
