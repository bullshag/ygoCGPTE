using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class ShopForm
    {
        private System.ComponentModel.IContainer? components = null;
        private ListBox _lstShop;
        private ListBox _lstInventory;
        private Button _btnBuy;
        private Button _btnSell;
        private Label _lblGold;

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
            _lstShop = new ListBox();
            _lstInventory = new ListBox();
            _btnBuy = new Button();
            _btnSell = new Button();
            _lblGold = new Label();
            label1 = new Label();
            label2 = new Label();
            shopDescBox = new RichTextBox();
            SuspendLayout();
            // 
            // _lstShop
            // 
            _lstShop.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lstShop.ItemHeight = 15;
            _lstShop.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            _lstShop.Location = new Point(10, 25);
            _lstShop.Name = "_lstShop";
            _lstShop.Size = new Size(250, 139);
            _lstShop.TabIndex = 4;
            // 
            // _lstInventory
            // 
            _lstInventory.ItemHeight = 15;
            _lstInventory.Location = new Point(330, 25);
            _lstInventory.Name = "_lstInventory";
            _lstInventory.Size = new Size(250, 139);
            _lstInventory.TabIndex = 3;
            // 
            // _btnBuy
            // 
            _btnBuy.Location = new Point(72, 224);
            _btnBuy.Name = "_btnBuy";
            _btnBuy.Size = new Size(100, 23);
            _btnBuy.TabIndex = 2;
            _btnBuy.Text = "Buy";
            // 
            // _btnSell
            // 
            _btnSell.Location = new Point(406, 224);
            _btnSell.Name = "_btnSell";
            _btnSell.Size = new Size(100, 23);
            _btnSell.TabIndex = 1;
            _btnSell.Text = "Sell";
            // 
            // _lblGold
            // 
            _lblGold.AutoSize = true;
            _lblGold.Location = new Point(238, 228);
            _lblGold.Name = "_lblGold";
            _lblGold.Size = new Size(35, 15);
            _lblGold.TabIndex = 0;
            _lblGold.Text = "Gold:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(73, 1);
            label1.Name = "label1";
            label1.Size = new Size(99, 21);
            label1.TabIndex = 6;
            label1.Text = "Shop Wares";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(406, 1);
            label2.Name = "label2";
            label2.Size = new Size(85, 21);
            label2.TabIndex = 7;
            label2.Text = "Inventory";
            // 
            // shopDescBox
            // 
            shopDescBox.BackColor = SystemColors.Window;
            shopDescBox.Location = new Point(10, 170);
            shopDescBox.Name = "shopDescBox";
            shopDescBox.ReadOnly = true;
            shopDescBox.Size = new Size(570, 48);
            shopDescBox.TabIndex = 8;
            shopDescBox.Text = "";
            // 
            // ShopForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(613, 258);
            Controls.Add(shopDescBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(_lblGold);
            Controls.Add(_btnSell);
            Controls.Add(_btnBuy);
            Controls.Add(_lstInventory);
            Controls.Add(_lstShop);
            Name = "ShopForm";
            Text = "Shop";
            Load += ShopForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private Label label1;
        private Label label2;
        private RichTextBox shopDescBox;
    }
}
