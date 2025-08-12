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
        private ComboBox cmbTarget;

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
            cmbTarget = new ComboBox();
            SuspendLayout();
            // 
            // lstItems
            // 
            lstItems.FormattingEnabled = true;
            lstItems.IntegralHeight = false;
            lstItems.ItemHeight = 25;
            lstItems.Items.AddRange(new object[] { "xcxvcx\t", "jyfyfy]", "jfyjyfjyfjfy" });
            lstItems.Location = new Point(17, 20);
            lstItems.Margin = new Padding(4, 5, 4, 5);
            lstItems.Name = "lstItems";
            lstItems.Size = new Size(213, 479);
            lstItems.TabIndex = 3;
            lstItems.SelectedIndexChanged += lstItems_SelectedIndexChanged;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = false;
            lblDescription.Location = new Point(240, 20);
            lblDescription.Margin = new Padding(4, 0, 4, 0);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(286, 371);
            lblDescription.TabIndex = 2;
            // 
            // btnUse
            // 
            btnUse.Enabled = false;
            btnUse.Location = new Point(244, 461);
            btnUse.Margin = new Padding(4, 5, 4, 5);
            btnUse.Name = "btnUse";
            btnUse.Size = new Size(286, 38);
            btnUse.TabIndex = 1;
            btnUse.Text = "Use";
            btnUse.Click += btnUse_Click;
            // 
            // cmbTarget
            // 
            cmbTarget.Location = new Point(246, 418);
            cmbTarget.Margin = new Padding(4, 5, 4, 5);
            cmbTarget.Name = "cmbTarget";
            cmbTarget.Size = new Size(284, 33);
            cmbTarget.TabIndex = 0;
            cmbTarget.SelectedIndexChanged += cmbTarget_SelectedIndexChanged;
            // 
            // InventoryForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(543, 542);
            Controls.Add(cmbTarget);
            Controls.Add(btnUse);
            Controls.Add(lblDescription);
            Controls.Add(lstItems);
            Margin = new Padding(4, 5, 4, 5);
            Name = "InventoryForm";
            Text = "Inventory";
            Load += InventoryForm_Load;
            ResumeLayout(false);
        }
    }
}
