using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class PartyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;
        private GroupBox grpParty = null!;
        private ListBox lstParty = null!;
        private Button btnHire = null!;
        private GroupBox grpStatus = null!;
        private Label lblGold = null!;
        private Label lblPartyExp = null!;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            grpParty = new GroupBox();
            lstParty = new ListBox();
            btnHire = new Button();
            grpStatus = new GroupBox();
            lblGold = new Label();
            lblPartyExp = new Label();
            SuspendLayout();
            //
            // grpParty
            //
            grpParty.Controls.Add(btnHire);
            grpParty.Controls.Add(lstParty);
            grpParty.Location = new Point(40, 40);
            grpParty.Name = "grpParty";
            grpParty.Size = new Size(300, 300);
            grpParty.Text = "Party Members";
            //
            // lstParty
            //
            lstParty.Location = new Point(10, 22);
            lstParty.Size = new Size(280, 200);
            lstParty.Name = "lstParty";
            //
            // lblGold
            //
            lblGold.AutoSize = true;
            lblGold.Location = new Point(10, 25);
            lblGold.Name = "lblGold";
            lblGold.Size = new Size(52, 15);
            lblGold.Text = "Gold: 0";
            //
            // lblPartyExp
            //
            lblPartyExp.AutoSize = true;
            lblPartyExp.Location = new Point(10, 50);
            lblPartyExp.Name = "lblPartyExp";
            lblPartyExp.Size = new Size(86, 15);
            lblPartyExp.Text = "Party EXP: 0";
            //
            // btnHire
            //
            btnHire.Location = new Point(10, 230);
            btnHire.Name = "btnHire";
            btnHire.Size = new Size(150, 23);
            btnHire.Text = "Hire Party Member";
            btnHire.UseVisualStyleBackColor = true;
            btnHire.Click += btnHire_Click;
            //
            // grpStatus
            //
            grpStatus.Controls.Add(lblPartyExp);
            grpStatus.Controls.Add(lblGold);
            grpStatus.Location = new Point(40, 360);
            grpStatus.Name = "grpStatus";
            grpStatus.Size = new Size(300, 80);
            grpStatus.Text = "Status";
            //
            // PartyForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 600);
            Controls.Add(grpStatus);
            Controls.Add(grpParty);
            Name = "PartyForm";
            Text = "Party";
            ResumeLayout(false);
        }

        #endregion
    }
}
