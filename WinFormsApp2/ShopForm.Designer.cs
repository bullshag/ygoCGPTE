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
            components = new System.ComponentModel.Container();
            _lstShop = new ListBox();
            _lstInventory = new ListBox();
            _btnBuy = new Button();
            _btnSell = new Button();
            _lblGold = new Label();
            SuspendLayout();
            // 
            // _lstShop
            // 
            _lstShop.Location = new Point(10, 10);
            _lstShop.Size = new Size(250, 300);
            // 
            // _lstInventory
            // 
            _lstInventory.Location = new Point(330, 10);
            _lstInventory.Size = new Size(250, 300);
            // 
            // _btnBuy
            // 
            _btnBuy.Location = new Point(10, 320);
            _btnBuy.Size = new Size(100, 23);
            _btnBuy.Text = "Buy";
            // 
            // _btnSell
            // 
            _btnSell.Location = new Point(330, 320);
            _btnSell.Size = new Size(100, 23);
            _btnSell.Text = "Sell";
            // 
            // _lblGold
            // 
            _lblGold.AutoSize = true;
            _lblGold.Location = new Point(10, 350);
            _lblGold.Text = "Gold:";
            // 
            // ShopForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(650, 420);
            Controls.Add(_lblGold);
            Controls.Add(_btnSell);
            Controls.Add(_btnBuy);
            Controls.Add(_lstInventory);
            Controls.Add(_lstShop);
            Name = "ShopForm";
            Text = "Shop";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
