using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class HeroViewForm
    {
        private System.ComponentModel.IContainer? components = null;
        private Label lblName;
        private TextBox txtName;
        private Label lblStr;
        private Label lblDex;
        private Label lblInt;
        private Label lblAbility;
        private Label lblPassive;
        private NumericUpDown numStr;
        private NumericUpDown numDex;
        private NumericUpDown numInt;
        private Label lblPoints;
        private Button btnHire;

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
            lblName = new Label();
            txtName = new TextBox();
            lblStr = new Label();
            lblDex = new Label();
            lblInt = new Label();
            lblAbility = new Label();
            lblPassive = new Label();
            numStr = new NumericUpDown();
            numDex = new NumericUpDown();
            numInt = new NumericUpDown();
            lblPoints = new Label();
            btnHire = new Button();
            ((System.ComponentModel.ISupportInitialize)numStr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDex).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numInt).BeginInit();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(12, 9);
            lblName.Name = "lblName";
            lblName.Size = new Size(42, 15);
            lblName.TabIndex = 9;
            lblName.Text = "Name:";
            // 
            // txtName
            // 
            txtName.Location = new Point(70, 6);
            txtName.Name = "txtName";
            txtName.Size = new Size(120, 23);
            txtName.TabIndex = 8;
            // 
            // lblStr
            // 
            lblStr.AutoSize = true;
            lblStr.Location = new Point(12, 37);
            lblStr.Name = "lblStr";
            lblStr.Size = new Size(29, 15);
            lblStr.TabIndex = 7;
            lblStr.Text = "STR:";
            // 
            // lblDex
            // 
            lblDex.AutoSize = true;
            lblDex.Location = new Point(12, 66);
            lblDex.Name = "lblDex";
            lblDex.Size = new Size(31, 15);
            lblDex.TabIndex = 6;
            lblDex.Text = "DEX:";
            // 
            // lblInt
            // 
            lblInt.AutoSize = true;
            lblInt.Location = new Point(12, 95);
            lblInt.Name = "lblInt";
            lblInt.Size = new Size(28, 15);
            lblInt.TabIndex = 5;
            lblInt.Text = "INT:";
            // 
            // lblAbility
            // 
            lblAbility.AutoSize = true;
            lblAbility.Location = new Point(12, 124);
            lblAbility.Name = "lblAbility";
            lblAbility.Size = new Size(44, 15);
            lblAbility.TabIndex = 12;
            lblAbility.Text = "Ability:";
            // 
            // lblPassive
            // 
            lblPassive.AutoSize = true;
            lblPassive.Location = new Point(12, 148);
            lblPassive.Name = "lblPassive";
            lblPassive.Size = new Size(48, 15);
            lblPassive.TabIndex = 13;
            lblPassive.Text = "Passive:";
            // 
            // numStr
            // 
            numStr.Location = new Point(70, 35);
            numStr.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numStr.Name = "numStr";
            numStr.Size = new Size(50, 23);
            numStr.TabIndex = 4;
            numStr.ValueChanged += StatsChanged;
            // 
            // numDex
            // 
            numDex.Location = new Point(70, 64);
            numDex.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numDex.Name = "numDex";
            numDex.Size = new Size(50, 23);
            numDex.TabIndex = 3;
            numDex.ValueChanged += StatsChanged;
            // 
            // numInt
            // 
            numInt.Location = new Point(70, 93);
            numInt.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numInt.Name = "numInt";
            numInt.Size = new Size(50, 23);
            numInt.TabIndex = 2;
            numInt.ValueChanged += StatsChanged;
            // 
            // lblPoints
            // 
            lblPoints.AutoSize = true;
            lblPoints.Location = new Point(70, 204);
            lblPoints.Name = "lblPoints";
            lblPoints.Size = new Size(78, 15);
            lblPoints.TabIndex = 1;
            lblPoints.Text = "Points left: 10";
            // 
            // btnHire
            // 
            btnHire.Location = new Point(8, 222);
            btnHire.Name = "btnHire";
            btnHire.Size = new Size(200, 23);
            btnHire.TabIndex = 0;
            btnHire.Text = "Hire Hero";
            btnHire.UseVisualStyleBackColor = true;
            btnHire.Click += btnHire_Click;
            // 
            // HeroViewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(220, 257);
            Controls.Add(lblPassive);
            Controls.Add(lblAbility);
            Controls.Add(btnHire);
            Controls.Add(lblPoints);
            Controls.Add(numInt);
            Controls.Add(numDex);
            Controls.Add(numStr);
            Controls.Add(lblInt);
            Controls.Add(lblDex);
            Controls.Add(lblStr);
            Controls.Add(txtName);
            Controls.Add(lblName);
            Name = "HeroViewForm";
            Text = "Hero";
            ((System.ComponentModel.ISupportInitialize)numStr).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDex).EndInit();
            ((System.ComponentModel.ISupportInitialize)numInt).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
