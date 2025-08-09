using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class RPGForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;
        private ListBox lstParty;
        private Button btnHire;
        private Button btnInspect;
        private Label lblGold;
        private Label lblTotalExp;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            btnHire = new Button();
            btnInspect = new Button();
            lblGold = new Label();
            lblTotalExp = new Label();
            SuspendLayout();
            // 
            // lstParty
            // 
            lstParty.FormattingEnabled = true;
            lstParty.ItemHeight = 15;
            lstParty.Location = new Point(12, 12);
            lstParty.Name = "lstParty";
            lstParty.Size = new Size(260, 154);
            lstParty.SelectedIndexChanged += lstParty_SelectedIndexChanged;
            // 
            // btnHire
            // 
            btnHire.Location = new Point(12, 172);
            btnHire.Name = "btnHire";
            btnHire.Size = new Size(260, 23);
            btnHire.Text = "Search for new recruits";
            btnHire.UseVisualStyleBackColor = true;
            btnHire.Click += btnHire_Click;
            //
            // btnInspect
            //
            btnInspect.Enabled = false;
            btnInspect.Location = new Point(12, 201);
            btnInspect.Name = "btnInspect";
            btnInspect.Size = new Size(260, 23);
            btnInspect.Text = "Inspect";
            btnInspect.UseVisualStyleBackColor = true;
            btnInspect.Click += btnInspect_Click;
            //
            // lblGold
            //
            lblGold.AutoSize = true;
            lblGold.Location = new Point(12, 237);
            lblGold.Name = "lblGold";
            lblGold.Size = new Size(35, 15);
            lblGold.Text = "Gold:";
            //
            // lblTotalExp
            //
            lblTotalExp.AutoSize = true;
            lblTotalExp.Location = new Point(12, 262);
            lblTotalExp.Name = "lblTotalExp";
            lblTotalExp.Size = new Size(69, 15);
            lblTotalExp.Text = "Party EXP:";
            //
            // RPGForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(284, 291);
            Controls.Add(lblTotalExp);
            Controls.Add(lblGold);
            Controls.Add(btnInspect);
            Controls.Add(btnHire);
            Controls.Add(lstParty);
            Name = "RPGForm";
            Text = "Party";
            Load += RPGForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
