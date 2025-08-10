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
            components = new System.ComponentModel.Container();
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
            lblStats.Location = new Point(10, 10);
            lblStats.Size = new Size(260, 150);
            // 
            // cmbRole
            // 
            cmbRole.Location = new Point(10, 170);
            cmbRole.Size = new Size(120, 23);
            // 
            // cmbTarget
            // 
            cmbTarget.Location = new Point(150, 170);
            cmbTarget.Size = new Size(140, 23);
            // 
            // cmbAbility1
            // 
            cmbAbility1.Location = new Point(10, 200);
            cmbAbility1.Size = new Size(120, 23);
            // 
            // cmbAbility2
            // 
            cmbAbility2.Location = new Point(10, 230);
            cmbAbility2.Size = new Size(120, 23);
            // 
            // cmbAbility3
            // 
            cmbAbility3.Location = new Point(10, 260);
            cmbAbility3.Size = new Size(120, 23);
            // 
            // numPriority1
            // 
            numPriority1.Location = new Point(150, 200);
            numPriority1.Size = new Size(40, 23);
            // 
            // numPriority2
            // 
            numPriority2.Location = new Point(150, 230);
            numPriority2.Size = new Size(40, 23);
            // 
            // numPriority3
            // 
            numPriority3.Location = new Point(150, 260);
            numPriority3.Size = new Size(40, 23);
            // 
            // cmbLeft
            // 
            cmbLeft.Location = new Point(10, 320);
            cmbLeft.Size = new Size(120, 23);
            // 
            // cmbRight
            // 
            cmbRight.Location = new Point(150, 320);
            cmbRight.Size = new Size(120, 23);
            // 
            // cmbBody
            // 
            cmbBody.Location = new Point(10, 350);
            cmbBody.Size = new Size(120, 23);
            // 
            // cmbLegs
            // 
            cmbLegs.Location = new Point(150, 350);
            cmbLegs.Size = new Size(120, 23);
            // 
            // cmbHead
            // 
            cmbHead.Location = new Point(10, 380);
            cmbHead.Size = new Size(120, 23);
            // 
            // cmbTrinket
            // 
            cmbTrinket.Location = new Point(150, 380);
            cmbTrinket.Size = new Size(120, 23);
            // 
            // btnLevelUp
            // 
            btnLevelUp.Location = new Point(10, 450);
            btnLevelUp.Size = new Size(120, 23);
            btnLevelUp.Text = "Level Up";
            // 
            // HeroInspectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(330, 520);
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
            Name = "HeroInspectForm";
            Text = "Hero Details";
            ((System.ComponentModel.ISupportInitialize)numPriority1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPriority2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPriority3).EndInit();
            ResumeLayout(false);
        }
    }
}
