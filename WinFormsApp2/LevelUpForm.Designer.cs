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
            components = new System.ComponentModel.Container();
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
            // labelStr
            // 
            labelStr.Text = "STR";
            labelStr.Location = new Point(10, 10);
            labelStr.Size = new Size(40, 15);
            // 
            // numStr
            // 
            numStr.Location = new Point(60, 8);
            numStr.Maximum = 999;
            numStr.Size = new Size(60, 23);
            // 
            // labelDex
            // 
            labelDex.Text = "DEX";
            labelDex.Location = new Point(10, 40);
            labelDex.Size = new Size(40, 15);
            // 
            // numDex
            // 
            numDex.Location = new Point(60, 38);
            numDex.Maximum = 999;
            numDex.Size = new Size(60, 23);
            // 
            // labelInt
            // 
            labelInt.Text = "INT";
            labelInt.Location = new Point(10, 70);
            labelInt.Size = new Size(40, 15);
            // 
            // numInt
            // 
            numInt.Location = new Point(60, 68);
            numInt.Maximum = 999;
            numInt.Size = new Size(60, 23);
            // 
            // lblPoints
            // 
            lblPoints.Location = new Point(10, 100);
            lblPoints.Size = new Size(200, 15);
            // 
            // btnSave
            // 
            btnSave.Location = new Point(10, 130);
            btnSave.Size = new Size(100, 23);
            btnSave.Text = "Save";
            // 
            // lblGold
            // 
            lblGold.Location = new Point(220, 10);
            lblGold.Size = new Size(150, 15);
            // 
            // lstAbilities
            // 
            lstAbilities.Location = new Point(220, 40);
            lstAbilities.Size = new Size(150, 120);
            // 
            // btnBuy
            // 
            btnBuy.Location = new Point(220, 170);
            btnBuy.Size = new Size(150, 23);
            btnBuy.Text = "Buy";
            // 
            // LevelUpForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(420, 240);
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
            Name = "LevelUpForm";
            Text = "Level Up";
            ((System.ComponentModel.ISupportInitialize)numStr).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDex).EndInit();
            ((System.ComponentModel.ISupportInitialize)numInt).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
