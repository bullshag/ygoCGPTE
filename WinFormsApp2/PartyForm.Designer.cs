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
        private ListBox lstParty = null!;
        private Label lblGold = null!;
        private Label lblPartyExp = null!;
        private Button btnHire = null!;

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
            lstParty = new ListBox();
            lblGold = new Label();
            lblPartyExp = new Label();
            btnHire = new Button();
            SuspendLayout();
            //
            // lstParty
            //
            lstParty.Location = new Point(70, 90);
            lstParty.Size = new Size(220, 199);
            lstParty.Name = "lstParty";
            //
            // lblGold
            //
            lblGold.AutoSize = true;
            lblGold.Location = new Point(70, 530);
            lblGold.Name = "lblGold";
            lblGold.Size = new Size(52, 15);
            lblGold.Text = "Gold: 0";
            //
            // lblPartyExp
            //
            lblPartyExp.AutoSize = true;
            lblPartyExp.Location = new Point(220, 530);
            lblPartyExp.Name = "lblPartyExp";
            lblPartyExp.Size = new Size(86, 15);
            lblPartyExp.Text = "Party EXP: 0";
            //
            // btnHire
            //
            btnHire.Location = new Point(70, 300);
            btnHire.Name = "btnHire";
            btnHire.Size = new Size(120, 23);
            btnHire.Text = "Hire Party Member";
            btnHire.UseVisualStyleBackColor = true;
            btnHire.Click += btnHire_Click;
            //
            // PartyForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkSlateGray;
            ClientSize = new Size(800, 600);
            Controls.Add(btnHire);
            Controls.Add(lblPartyExp);
            Controls.Add(lblGold);
            Controls.Add(lstParty);
            Name = "PartyForm";
            Text = "Party";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
