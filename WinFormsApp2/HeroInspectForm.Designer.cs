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
            ((System.ComponentModel.ISupportInitialize)numPriority1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPriority2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPriority3).BeginInit();
            SuspendLayout();
            // 
            // lblStats
            // 
            lblStats.Location = new Point(14, 17);
            lblStats.Margin = new Padding(4, 0, 4, 0);
            lblStats.Name = "lblStats";
            lblStats.Size = new Size(371, 250);
            lblStats.TabIndex = 15;
            // 
            // cmbRole
            // 
            cmbRole.Location = new Point(14, 283);
            cmbRole.Margin = new Padding(4, 5, 4, 5);
            cmbRole.Name = "cmbRole";
            cmbRole.Size = new Size(170, 33);
            cmbRole.TabIndex = 14;
            // 
            // cmbTarget
            // 
            cmbTarget.Location = new Point(214, 283);
            cmbTarget.Margin = new Padding(4, 5, 4, 5);
            cmbTarget.Name = "cmbTarget";
            cmbTarget.Size = new Size(393, 33);
            cmbTarget.TabIndex = 13;
            // 
            // cmbAbility1
            // 
            cmbAbility1.Location = new Point(14, 333);
            cmbAbility1.Margin = new Padding(4, 5, 4, 5);
            cmbAbility1.Name = "cmbAbility1";
            cmbAbility1.Size = new Size(170, 33);
            cmbAbility1.TabIndex = 12;
            // 
            // cmbAbility2
            // 
            cmbAbility2.Location = new Point(14, 383);
            cmbAbility2.Margin = new Padding(4, 5, 4, 5);
            cmbAbility2.Name = "cmbAbility2";
            cmbAbility2.Size = new Size(170, 33);
            cmbAbility2.TabIndex = 11;
            // 
            // cmbAbility3
            // 
            cmbAbility3.Location = new Point(14, 433);
            cmbAbility3.Margin = new Padding(4, 5, 4, 5);
            cmbAbility3.Name = "cmbAbility3";
            cmbAbility3.Size = new Size(170, 33);
            cmbAbility3.TabIndex = 10;
            // 
            // numPriority1
            // 
            numPriority1.Location = new Point(214, 333);
            numPriority1.Margin = new Padding(4, 5, 4, 5);
            numPriority1.Name = "numPriority1";
            numPriority1.Size = new Size(57, 31);
            numPriority1.TabIndex = 9;
            // 
            // numPriority2
            // 
            numPriority2.Location = new Point(214, 383);
            numPriority2.Margin = new Padding(4, 5, 4, 5);
            numPriority2.Name = "numPriority2";
            numPriority2.Size = new Size(57, 31);
            numPriority2.TabIndex = 8;
            // 
            // numPriority3
            // 
            numPriority3.Location = new Point(214, 433);
            numPriority3.Margin = new Padding(4, 5, 4, 5);
            numPriority3.Name = "numPriority3";
            numPriority3.Size = new Size(57, 31);
            numPriority3.TabIndex = 7;
            // 
            // cmbLeft
            // 
            cmbLeft.Location = new Point(14, 533);
            cmbLeft.Margin = new Padding(4, 5, 4, 5);
            cmbLeft.Name = "cmbLeft";
            cmbLeft.Size = new Size(267, 33);
            cmbLeft.TabIndex = 6;
            cmbLeft.SelectedIndexChanged += cmbLeft_SelectedIndexChanged;
            // 
            // cmbRight
            // 
            cmbRight.Location = new Point(348, 533);
            cmbRight.Margin = new Padding(4, 5, 4, 5);
            cmbRight.Name = "cmbRight";
            cmbRight.Size = new Size(259, 33);
            cmbRight.TabIndex = 5;
            // 
            // cmbBody
            // 
            cmbBody.Location = new Point(14, 583);
            cmbBody.Margin = new Padding(4, 5, 4, 5);
            cmbBody.Name = "cmbBody";
            cmbBody.Size = new Size(267, 33);
            cmbBody.TabIndex = 4;
            // 
            // cmbLegs
            // 
            cmbLegs.Location = new Point(348, 583);
            cmbLegs.Margin = new Padding(4, 5, 4, 5);
            cmbLegs.Name = "cmbLegs";
            cmbLegs.Size = new Size(259, 33);
            cmbLegs.TabIndex = 3;
            // 
            // cmbHead
            // 
            cmbHead.Location = new Point(14, 633);
            cmbHead.Margin = new Padding(4, 5, 4, 5);
            cmbHead.Name = "cmbHead";
            cmbHead.Size = new Size(267, 33);
            cmbHead.TabIndex = 2;
            // 
            // cmbTrinket
            // 
            cmbTrinket.Location = new Point(348, 633);
            cmbTrinket.Margin = new Padding(4, 5, 4, 5);
            cmbTrinket.Name = "cmbTrinket";
            cmbTrinket.Size = new Size(259, 33);
            cmbTrinket.TabIndex = 1;
            // 
            // btnLevelUp
            // 
            btnLevelUp.Location = new Point(14, 697);
            btnLevelUp.Margin = new Padding(4, 5, 4, 5);
            btnLevelUp.Name = "btnLevelUp";
            btnLevelUp.Size = new Size(171, 38);
            btnLevelUp.TabIndex = 0;
            btnLevelUp.Text = "Level Up";
            // 
            // HeroInspectForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(621, 781);
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
            Controls.Add(cmbAbility3);
            Controls.Add(cmbAbility2);
            Controls.Add(cmbAbility1);
            Controls.Add(cmbTarget);
            Controls.Add(cmbRole);
            Controls.Add(lblStats);
            Margin = new Padding(4, 5, 4, 5);
            Name = "HeroInspectForm";
            Text = "Hero Details";
            ((System.ComponentModel.ISupportInitialize)numPriority1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPriority2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPriority3).EndInit();
            ResumeLayout(false);
        }
    }
}
