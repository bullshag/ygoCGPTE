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
        private RichTextBox rtbAbility;
        private Button btnBuy;
        private Button btnSave;
        private ListBox lstPassives;
        private Button btnBuyPassive;
        private NumericUpDown numStr;
        private NumericUpDown numDex;
        private NumericUpDown numInt;
        private NumericUpDown numHP;
        private NumericUpDown numMP;
        private Label labelStr;
        private Label labelDex;
        private Label labelInt;
        private Label labelHP;
        private Label labelMP;

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
            rtbAbility = new RichTextBox();
            lstPassives = new ListBox();
            btnBuyPassive = new Button();
            numHP = new NumericUpDown();
            numMP = new NumericUpDown();
            labelHP = new Label();
            labelMP = new Label();
            ((System.ComponentModel.ISupportInitialize)numStr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDex).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numInt).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMP).BeginInit();
            SuspendLayout();
            // 
            // lblPoints
            // 
            lblPoints.Location = new Point(10, 160);
            lblPoints.Name = "lblPoints";
            lblPoints.Size = new Size(200, 15);
            lblPoints.TabIndex = 4;
            // 
            // lblGold
            // 
            lblGold.Location = new Point(220, 10);
            lblGold.Name = "lblGold";
            lblGold.Size = new Size(150, 15);
            lblGold.TabIndex = 2;
            // 
            // lstAbilities
            // 
            lstAbilities.ItemHeight = 15;
            lstAbilities.Location = new Point(126, 8);
            lstAbilities.Name = "lstAbilities";
            lstAbilities.Size = new Size(150, 109);
            lstAbilities.TabIndex = 1;
            // 
            // btnBuy
            // 
            btnBuy.Location = new Point(125, 189);
            btnBuy.Name = "btnBuy";
            btnBuy.Size = new Size(150, 23);
            btnBuy.TabIndex = 0;
            btnBuy.Text = "Buy";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(12, 189);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 23);
            btnSave.TabIndex = 3;
            btnSave.Text = "Save";
            // 
            // numStr
            // 
            numStr.Location = new Point(60, 8);
            numStr.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numStr.Name = "numStr";
            numStr.Size = new Size(60, 23);
            numStr.TabIndex = 9;
            // 
            // numDex
            // 
            numDex.Location = new Point(60, 38);
            numDex.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numDex.Name = "numDex";
            numDex.Size = new Size(60, 23);
            numDex.TabIndex = 7;
            // 
            // numInt
            // 
            numInt.Location = new Point(60, 68);
            numInt.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            numInt.Name = "numInt";
            numInt.Size = new Size(60, 23);
            numInt.TabIndex = 5;
            // 
            // labelStr
            // 
            labelStr.Location = new Point(10, 10);
            labelStr.Name = "labelStr";
            labelStr.Size = new Size(40, 15);
            labelStr.TabIndex = 10;
            labelStr.Text = "STR";
            // 
            // labelDex
            // 
            labelDex.Location = new Point(10, 40);
            labelDex.Name = "labelDex";
            labelDex.Size = new Size(40, 15);
            labelDex.TabIndex = 8;
            labelDex.Text = "DEX";
            // 
            // labelInt
            // 
            labelInt.Location = new Point(10, 70);
            labelInt.Name = "labelInt";
            labelInt.Size = new Size(40, 15);
            labelInt.TabIndex = 6;
            labelInt.Text = "INT";
            // 
            // rtbAbility
            // 
            rtbAbility.Location = new Point(126, 118);
            rtbAbility.Name = "rtbAbility";
            rtbAbility.ReadOnly = true;
            rtbAbility.Size = new Size(150, 68);
            rtbAbility.TabIndex = 17;
            rtbAbility.Text = "";
            // 
            // lstPassives
            // 
            lstPassives.ItemHeight = 15;
            lstPassives.Location = new Point(282, 8);
            lstPassives.Name = "lstPassives";
            lstPassives.Size = new Size(150, 109);
            lstPassives.TabIndex = 11;
            // 
            // btnBuyPassive
            // 
            btnBuyPassive.Location = new Point(281, 118);
            btnBuyPassive.Name = "btnBuyPassive";
            btnBuyPassive.Size = new Size(150, 23);
            btnBuyPassive.TabIndex = 12;
            btnBuyPassive.Text = "Buy Passive";
            // 
            // numHP
            // 
            numHP.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numHP.Location = new Point(60, 98);
            numHP.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numHP.Name = "numHP";
            numHP.Size = new Size(60, 23);
            numHP.TabIndex = 13;
            // 
            // numMP
            // 
            numMP.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numMP.Location = new Point(60, 128);
            numMP.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numMP.Name = "numMP";
            numMP.Size = new Size(60, 23);
            numMP.TabIndex = 15;
            numMP.ValueChanged += numMP_ValueChanged;
            // 
            // labelHP
            // 
            labelHP.Location = new Point(10, 100);
            labelHP.Name = "labelHP";
            labelHP.Size = new Size(40, 15);
            labelHP.TabIndex = 14;
            labelHP.Text = "HP";
            // 
            // labelMP
            // 
            labelMP.Location = new Point(10, 130);
            labelMP.Name = "labelMP";
            labelMP.Size = new Size(40, 15);
            labelMP.TabIndex = 16;
            labelMP.Text = "MP";
            // 
            // LevelUpForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(441, 222);
            Controls.Add(btnBuyPassive);
            Controls.Add(lstPassives);
            Controls.Add(btnBuy);
            Controls.Add(rtbAbility);
            Controls.Add(lstAbilities);
            Controls.Add(lblGold);
            Controls.Add(btnSave);
            Controls.Add(lblPoints);
            Controls.Add(numMP);
            Controls.Add(labelMP);
            Controls.Add(numHP);
            Controls.Add(labelHP);
            Controls.Add(numInt);
            Controls.Add(labelInt);
            Controls.Add(numDex);
            Controls.Add(labelDex);
            Controls.Add(numStr);
            Controls.Add(labelStr);
            Name = "LevelUpForm";
            Text = "Level Up";
            ((System.ComponentModel.ISupportInitialize)numStr).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDex).EndInit();
            ((System.ComponentModel.ISupportInitialize)numInt).EndInit();
            ((System.ComponentModel.ISupportInitialize)numHP).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMP).EndInit();
            ResumeLayout(false);
        }
    }
}
