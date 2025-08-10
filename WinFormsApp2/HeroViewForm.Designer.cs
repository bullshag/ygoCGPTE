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
            lblName.Location = new Point(17, 15);
            lblName.Margin = new Padding(4, 0, 4, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(63, 25);
            lblName.TabIndex = 9;
            lblName.Text = "Name:";
            // 
            // txtName
            // 
            txtName.Location = new Point(100, 10);
            txtName.Margin = new Padding(4, 5, 4, 5);
            txtName.Name = "txtName";
            txtName.Size = new Size(170, 31);
            txtName.TabIndex = 8;
            // 
            // lblStr
            // 
            lblStr.AutoSize = true;
            lblStr.Location = new Point(17, 62);
            lblStr.Margin = new Padding(4, 0, 4, 0);
            lblStr.Name = "lblStr";
            lblStr.Size = new Size(46, 25);
            lblStr.TabIndex = 7;
            lblStr.Text = "STR:";
            // 
            // lblDex
            // 
            lblDex.AutoSize = true;
            lblDex.Location = new Point(17, 110);
            lblDex.Margin = new Padding(4, 0, 4, 0);
            lblDex.Name = "lblDex";
            lblDex.Size = new Size(49, 25);
            lblDex.TabIndex = 6;
            lblDex.Text = "DEX:";
            // 
            // lblInt
            // 
            lblInt.AutoSize = true;
            lblInt.Location = new Point(17, 158);
            lblInt.Margin = new Padding(4, 0, 4, 0);
            lblInt.Name = "lblInt";
            lblInt.Size = new Size(43, 25);
            lblInt.TabIndex = 5;
            lblInt.Text = "INT:";
            // 
            // numStr
            // 
            numStr.Location = new Point(100, 58);
            numStr.Margin = new Padding(4, 5, 4, 5);
            numStr.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numStr.Name = "numStr";
            numStr.Size = new Size(71, 31);
            numStr.TabIndex = 4;
            numStr.ValueChanged += StatsChanged;
            // 
            // numDex
            // 
            numDex.Location = new Point(100, 107);
            numDex.Margin = new Padding(4, 5, 4, 5);
            numDex.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numDex.Name = "numDex";
            numDex.Size = new Size(71, 31);
            numDex.TabIndex = 3;
            numDex.ValueChanged += StatsChanged;
            // 
            // numInt
            // 
            numInt.Location = new Point(100, 155);
            numInt.Margin = new Padding(4, 5, 4, 5);
            numInt.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numInt.Name = "numInt";
            numInt.Size = new Size(71, 31);
            numInt.TabIndex = 2;
            numInt.ValueChanged += StatsChanged;
            // 
            // lblPoints
            // 
            lblPoints.AutoSize = true;
            lblPoints.Location = new Point(17, 208);
            lblPoints.Margin = new Padding(4, 0, 4, 0);
            lblPoints.Name = "lblPoints";
            lblPoints.Size = new Size(119, 25);
            lblPoints.TabIndex = 1;
            lblPoints.Text = "Points left: 10";
            // 
            // btnHire
            // 
            btnHire.Location = new Point(17, 255);
            btnHire.Margin = new Padding(4, 5, 4, 5);
            btnHire.Name = "btnHire";
            btnHire.Size = new Size(286, 38);
            btnHire.TabIndex = 0;
            btnHire.Text = "Hire Hero";
            btnHire.UseVisualStyleBackColor = true;
            btnHire.Click += btnHire_Click;
            // 
            // HeroViewForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(315, 311);
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
            Margin = new Padding(4, 5, 4, 5);
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
