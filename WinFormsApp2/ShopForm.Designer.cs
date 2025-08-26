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
        private Label _lblDescription;

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
            _lblDescription = new Label();
            SuspendLayout();
            // 
            // _lstShop
            // 
            _lstShop.ItemHeight = 25;
            _lstShop.Location = new Point(14, 17);
            _lstShop.Margin = new Padding(4, 5, 4, 5);
            _lstShop.Name = "_lstShop";
            _lstShop.Size = new Size(355, 479);
            _lstShop.TabIndex = 4;
            // 
            // _lstInventory
            // 
            _lstInventory.ItemHeight = 25;
            _lstInventory.Location = new Point(471, 17);
            _lstInventory.Margin = new Padding(4, 5, 4, 5);
            _lstInventory.Name = "_lstInventory";
            _lstInventory.Size = new Size(355, 479);
            _lstInventory.TabIndex = 3;
            // 
            // _btnBuy
            // 
            _btnBuy.Location = new Point(14, 533);
            _btnBuy.Margin = new Padding(4, 5, 4, 5);
            _btnBuy.Name = "_btnBuy";
            _btnBuy.Size = new Size(143, 38);
            _btnBuy.TabIndex = 2;
            _btnBuy.Text = "Buy";
            // 
            // _btnSell
            // 
            _btnSell.Location = new Point(471, 533);
            _btnSell.Margin = new Padding(4, 5, 4, 5);
            _btnSell.Name = "_btnSell";
            _btnSell.Size = new Size(143, 38);
            _btnSell.TabIndex = 1;
            _btnSell.Text = "Sell";
            // 
            // _lblGold
            // 
            _lblGold.AutoSize = true;
            _lblGold.Location = new Point(14, 583);
            _lblGold.Margin = new Padding(4, 0, 4, 0);
            _lblGold.Name = "_lblGold";
            _lblGold.Size = new Size(54, 25);
            _lblGold.TabIndex = 0;
            _lblGold.Text = "Gold:";
            //
            // _lblDescription
            //
            _lblDescription.AutoSize = false;
            _lblDescription.Location = new Point(14, 621);
            _lblDescription.Margin = new Padding(4, 0, 4, 0);
            _lblDescription.Name = "_lblDescription";
            _lblDescription.Size = new Size(812, 66);
            _lblDescription.TabIndex = 5;
            // 
            // ShopForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(929, 700);
            Controls.Add(_lblDescription);
            Controls.Add(_lblGold);
            Controls.Add(_btnSell);
            Controls.Add(_btnBuy);
            Controls.Add(_lstInventory);
            Controls.Add(_lstShop);
            Margin = new Padding(4, 5, 4, 5);
            Name = "ShopForm";
            Text = "Shop";
            Load += ShopForm_Load_2;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
