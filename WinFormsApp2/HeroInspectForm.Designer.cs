using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class HeroInspectForm
    {
        private System.ComponentModel.IContainer? components = null;
        private Label lblStats;
        private ComboBox cmbRole;
        private ComboBox cmbTarget;
        private ComboBox cmbAbility1;
        private ComboBox cmbAbility2;
        private ComboBox cmbAbility3;
        private NumericUpDown numPriority1;
        private NumericUpDown numPriority2;
        private NumericUpDown numPriority3;
        private ListBox lstPassives;
        private ComboBox cmbLeft;
        private ComboBox cmbRight;
        private ComboBox cmbBody;
        private ComboBox cmbLegs;
        private ComboBox cmbHead;
        private ComboBox cmbTrinket;
        private Button btnLevelUp;

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
            lblStats = new Label();
            cmbRole = new ComboBox();
            cmbTarget = new ComboBox();
            cmbAbility1 = new ComboBox();
            cmbAbility2 = new ComboBox();
            cmbAbility3 = new ComboBox();
            numPriority1 = new NumericUpDown();
            numPriority2 = new NumericUpDown();
            numPriority3 = new NumericUpDown();
            cmbLeft = new ComboBox();
            cmbRight = new ComboBox();
            cmbBody = new ComboBox();
            cmbLegs = new ComboBox();
            cmbHead = new ComboBox();
            cmbTrinket = new ComboBox();
            btnLevelUp = new Button();
            lstPassives = new ListBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            ((System.ComponentModel.ISupportInitialize)numPriority1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPriority2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPriority3).BeginInit();
            SuspendLayout();
            // 
            // lblStats
            // 
            lblStats.Location = new Point(10, 10);
            lblStats.Name = "lblStats";
            lblStats.Size = new Size(260, 150);
            lblStats.TabIndex = 15;
            // 
            // cmbRole
            // 
            cmbRole.Location = new Point(90, 39);
            cmbRole.Name = "cmbRole";
            cmbRole.Size = new Size(120, 23);
            cmbRole.TabIndex = 14;
            // 
            // cmbTarget
            // 
            cmbTarget.Location = new Point(90, 10);
            cmbTarget.Name = "cmbTarget";
            cmbTarget.Size = new Size(333, 23);
            cmbTarget.TabIndex = 13;
            // 
            // cmbAbility1
            // 
            cmbAbility1.Location = new Point(214, 255);
            cmbAbility1.Name = "cmbAbility1";
            cmbAbility1.Size = new Size(149, 23);
            cmbAbility1.TabIndex = 12;
            // 
            // cmbAbility2
            // 
            cmbAbility2.Location = new Point(214, 285);
            cmbAbility2.Name = "cmbAbility2";
            cmbAbility2.Size = new Size(149, 23);
            cmbAbility2.TabIndex = 11;
            // 
            // cmbAbility3
            // 
            cmbAbility3.Location = new Point(214, 315);
            cmbAbility3.Name = "cmbAbility3";
            cmbAbility3.Size = new Size(149, 23);
            cmbAbility3.TabIndex = 10;
            // 
            // numPriority1
            // 
            numPriority1.Location = new Point(383, 255);
            numPriority1.Name = "numPriority1";
            numPriority1.Size = new Size(40, 23);
            numPriority1.TabIndex = 9;
            // 
            // numPriority2
            // 
            numPriority2.Location = new Point(383, 285);
            numPriority2.Name = "numPriority2";
            numPriority2.Size = new Size(40, 23);
            numPriority2.TabIndex = 8;
            // 
            // numPriority3
            // 
            numPriority3.Location = new Point(383, 315);
            numPriority3.Name = "numPriority3";
            numPriority3.Size = new Size(40, 23);
            numPriority3.TabIndex = 7;
            // 
            // cmbLeft
            // 
            cmbLeft.Location = new Point(90, 70);
            cmbLeft.Name = "cmbLeft";
            cmbLeft.Size = new Size(119, 23);
            cmbLeft.TabIndex = 6;
            cmbLeft.SelectedIndexChanged += cmbLeft_SelectedIndexChanged;
            // 
            // cmbRight
            // 
            cmbRight.Location = new Point(88, 99);
            cmbRight.Name = "cmbRight";
            cmbRight.Size = new Size(121, 23);
            cmbRight.TabIndex = 5;
            // 
            // cmbBody
            // 
            cmbBody.Location = new Point(88, 128);
            cmbBody.Name = "cmbBody";
            cmbBody.Size = new Size(121, 23);
            cmbBody.TabIndex = 4;
            // 
            // cmbLegs
            // 
            cmbLegs.Location = new Point(88, 157);
            cmbLegs.Name = "cmbLegs";
            cmbLegs.Size = new Size(121, 23);
            cmbLegs.TabIndex = 3;
            // 
            // cmbHead
            // 
            cmbHead.Location = new Point(88, 215);
            cmbHead.Name = "cmbHead";
            cmbHead.Size = new Size(121, 23);
            cmbHead.TabIndex = 2;
            // 
            // cmbTrinket
            // 
            cmbTrinket.Location = new Point(88, 186);
            cmbTrinket.Name = "cmbTrinket";
            cmbTrinket.Size = new Size(121, 23);
            cmbTrinket.TabIndex = 1;
            // 
            // btnLevelUp
            // 
            btnLevelUp.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLevelUp.Location = new Point(10, 258);
            btnLevelUp.Name = "btnLevelUp";
            btnLevelUp.Size = new Size(108, 80);
            btnLevelUp.TabIndex = 0;
            btnLevelUp.Text = "Level Up";
            // 
            // lstPassives
            // 
            lstPassives.ItemHeight = 15;
            lstPassives.Location = new Point(215, 100);
            lstPassives.Name = "lstPassives";
            lstPassives.Size = new Size(208, 139);
            lstPassives.TabIndex = 16;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 73);
            label1.Name = "label1";
            label1.Size = new Size(72, 14);
            label1.TabIndex = 17;
            label1.Text = "Main Hand";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(20, 102);
            label2.Name = "label2";
            label2.Size = new Size(62, 14);
            label2.TabIndex = 18;
            label2.Text = "Off Hand";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(40, 131);
            label3.Name = "label3";
            label3.Size = new Size(42, 14);
            label3.TabIndex = 19;
            label3.Text = "Chest";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(41, 160);
            label4.Name = "label4";
            label4.Size = new Size(43, 14);
            label4.TabIndex = 20;
            label4.Text = "Boots";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(38, 189);
            label5.Name = "label5";
            label5.Size = new Size(46, 14);
            label5.TabIndex = 21;
            label5.Text = "Gloves";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(34, 218);
            label6.Name = "label6";
            label6.Size = new Size(50, 14);
            label6.TabIndex = 22;
            label6.Text = "Trinket";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(124, 258);
            label7.Name = "label7";
            label7.Size = new Size(85, 14);
            label7.TabIndex = 23;
            label7.Text = "Active Skill 1";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(123, 288);
            label8.Name = "label8";
            label8.Size = new Size(85, 14);
            label8.TabIndex = 24;
            label8.Text = "Active Skill 2";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.Location = new Point(123, 319);
            label9.Name = "label9";
            label9.Size = new Size(85, 14);
            label9.TabIndex = 25;
            label9.Text = "Active Skill 3";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.Location = new Point(258, 79);
            label10.Name = "label10";
            label10.Size = new Size(105, 14);
            label10.TabIndex = 26;
            label10.Text = "Passive Skill List";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.Location = new Point(48, 42);
            label11.Name = "label11";
            label11.Size = new Size(34, 14);
            label11.TabIndex = 27;
            label11.Text = "Role";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Tahoma", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label12.Location = new Point(24, 13);
            label12.Name = "label12";
            label12.Size = new Size(60, 14);
            label12.TabIndex = 28;
            label12.Text = "Behavior";
            // 
            // HeroInspectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 353);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnLevelUp);
            Controls.Add(cmbTrinket);
            Controls.Add(cmbHead);
            Controls.Add(cmbLegs);
            Controls.Add(cmbBody);
            Controls.Add(cmbRight);
            Controls.Add(cmbLeft);
            Controls.Add(numPriority3);
            Controls.Add(numPriority2);
            Controls.Add(numPriority1);
            Controls.Add(lstPassives);
            Controls.Add(cmbAbility3);
            Controls.Add(cmbAbility2);
            Controls.Add(cmbAbility1);
            Controls.Add(cmbTarget);
            Controls.Add(cmbRole);
            Controls.Add(lblStats);
            Name = "HeroInspectForm";
            Text = "Hero Details";
            ((System.ComponentModel.ISupportInitialize)numPriority1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPriority2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPriority3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
    }
}
