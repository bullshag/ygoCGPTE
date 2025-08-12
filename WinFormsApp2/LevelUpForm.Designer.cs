using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class LevelUpForm
    {
        private System.ComponentModel.IContainer? components = null;
        private Label lblPoints;
        private Label lblGold;
        private ListBox lstAbilities;
        private Button btnBuy;
        private Button btnSave;
        private ListBox lstPassives;
        private Button btnBuyPassive;
        private NumericUpDown numStr;
        private NumericUpDown numDex;
        private NumericUpDown numInt;
        private Label labelStr;
        private Label labelDex;
        private Label labelInt;

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
            lblPoints = new Label();
            lblGold = new Label();
            lstAbilities = new ListBox();
            btnBuy = new Button();
            btnSave = new Button();
            numStr = new NumericUpDown();
            numDex = new NumericUpDown();
            numInt = new NumericUpDown();
            labelStr = new Label();
            labelDex = new Label();
            labelInt = new Label();
            ((System.ComponentModel.ISupportInitialize)numStr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDex).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numInt).BeginInit();
            SuspendLayout();
            // 
            // lblPoints
            // 
            lblPoints.Location = new Point(14, 167);
            lblPoints.Margin = new Padding(4, 0, 4, 0);
            lblPoints.Name = "lblPoints";
            lblPoints.Size = new Size(286, 25);
            lblPoints.TabIndex = 4;
            // 
            // lblGold
            // 
            lblGold.Location = new Point(314, 17);
            lblGold.Margin = new Padding(4, 0, 4, 0);
            lblGold.Name = "lblGold";
            lblGold.Size = new Size(214, 25);
            lblGold.TabIndex = 2;
            // 
            // lstAbilities
            // 
            lstAbilities.ItemHeight = 25;
            lstAbilities.Location = new Point(180, 13);
            lstAbilities.Margin = new Padding(4, 5, 4, 5);
            lstAbilities.Name = "lstAbilities";
            lstAbilities.Size = new Size(213, 179);
            lstAbilities.TabIndex = 1;
            //
            // lstPassives
            //
            lstPassives = new ListBox();
            lstPassives.ItemHeight = 25;
            lstPassives.Location = new Point(403, 13);
            lstPassives.Margin = new Padding(4, 5, 4, 5);
            lstPassives.Name = "lstPassives";
            lstPassives.Size = new Size(213, 179);
            lstPassives.TabIndex = 11;
            // 
            // btnBuy
            // 
            btnBuy.Location = new Point(179, 197);
            btnBuy.Margin = new Padding(4, 5, 4, 5);
            btnBuy.Name = "btnBuy";
            btnBuy.Size = new Size(214, 38);
            btnBuy.TabIndex = 0;
            btnBuy.Text = "Buy";
            //
            // btnBuyPassive
            //
            btnBuyPassive = new Button();
            btnBuyPassive.Location = new Point(402, 197);
            btnBuyPassive.Margin = new Padding(4, 5, 4, 5);
            btnBuyPassive.Name = "btnBuyPassive";
            btnBuyPassive.Size = new Size(214, 38);
            btnBuyPassive.TabIndex = 12;
            btnBuyPassive.Text = "Buy Passive";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(29, 154);
            btnSave.Margin = new Padding(4, 5, 4, 5);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(143, 38);
            btnSave.TabIndex = 3;
            btnSave.Text = "Save";
            // 
            // numStr
            // 
            numStr.Location = new Point(86, 13);
            numStr.Margin = new Padding(4, 5, 4, 5);
            numStr.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numStr.Name = "numStr";
            numStr.Size = new Size(86, 31);
            numStr.TabIndex = 9;
            // 
            // numDex
            // 
            numDex.Location = new Point(86, 63);
            numDex.Margin = new Padding(4, 5, 4, 5);
            numDex.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numDex.Name = "numDex";
            numDex.Size = new Size(86, 31);
            numDex.TabIndex = 7;
            // 
            // numInt
            // 
            numInt.Location = new Point(86, 113);
            numInt.Margin = new Padding(4, 5, 4, 5);
            numInt.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numInt.Name = "numInt";
            numInt.Size = new Size(86, 31);
            numInt.TabIndex = 5;
            // 
            // labelStr
            // 
            labelStr.Location = new Point(14, 17);
            labelStr.Margin = new Padding(4, 0, 4, 0);
            labelStr.Name = "labelStr";
            labelStr.Size = new Size(57, 25);
            labelStr.TabIndex = 10;
            labelStr.Text = "STR";
            // 
            // labelDex
            // 
            labelDex.Location = new Point(14, 67);
            labelDex.Margin = new Padding(4, 0, 4, 0);
            labelDex.Name = "labelDex";
            labelDex.Size = new Size(57, 25);
            labelDex.TabIndex = 8;
            labelDex.Text = "DEX";
            // 
            // labelInt
            // 
            labelInt.Location = new Point(14, 117);
            labelInt.Margin = new Padding(4, 0, 4, 0);
            labelInt.Name = "labelInt";
            labelInt.Size = new Size(57, 25);
            labelInt.TabIndex = 6;
            labelInt.Text = "INT";
            // 
            // LevelUpForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(630, 250);
            Controls.Add(btnBuyPassive);
            Controls.Add(lstPassives);
            Controls.Add(btnBuy);
            Controls.Add(lstAbilities);
            Controls.Add(lblGold);
            Controls.Add(btnSave);
            Controls.Add(lblPoints);
            Controls.Add(numInt);
            Controls.Add(labelInt);
            Controls.Add(numDex);
            Controls.Add(labelDex);
            Controls.Add(numStr);
            Controls.Add(labelStr);
            Margin = new Padding(4, 5, 4, 5);
            Name = "LevelUpForm";
            Text = "Level Up";
            ((System.ComponentModel.ISupportInitialize)numStr).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDex).EndInit();
            ((System.ComponentModel.ISupportInitialize)numInt).EndInit();
            ResumeLayout(false);
        }
    }
}
