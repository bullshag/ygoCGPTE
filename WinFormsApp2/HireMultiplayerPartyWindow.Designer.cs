using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp2
{
    partial class HireMultiplayerPartyWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            partyDescription = new RichTextBox();
            partiesForHireList = new ListBox();
            partyMembersList = new ListBox();
            hpLabel = new Label();
            manLabel = new Label();
            strLabel = new Label();
            label5 = new Label();
            intLabel = new Label();
            abilitiesLabel = new Label();
            label8 = new Label();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            costLabel = new Label();
            hirePartyBtn = new Button();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(19, 6);
            label1.Name = "label1";
            label1.Size = new Size(145, 15);
            label1.TabIndex = 0;
            label1.Text = "Available Parties for Hire";
            // 
            // partyDescription
            // 
            partyDescription.Location = new Point(6, 211);
            partyDescription.Name = "partyDescription";
            partyDescription.ReadOnly = true;
            partyDescription.Size = new Size(337, 66);
            partyDescription.TabIndex = 1;
            partyDescription.Text = "";
            // 
            // partiesForHireList
            // 
            partiesForHireList.FormattingEnabled = true;
            partiesForHireList.ItemHeight = 15;
            partiesForHireList.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            partiesForHireList.Location = new Point(6, 21);
            partiesForHireList.Name = "partiesForHireList";
            partiesForHireList.Size = new Size(174, 79);
            partiesForHireList.TabIndex = 2;
            // 
            // partyMembersList
            // 
            partyMembersList.FormattingEnabled = true;
            partyMembersList.ItemHeight = 15;
            partyMembersList.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            partyMembersList.Location = new Point(186, 21);
            partyMembersList.Name = "partyMembersList";
            partyMembersList.Size = new Size(157, 79);
            partyMembersList.TabIndex = 3;
            // 
            // hpLabel
            // 
            hpLabel.AutoSize = true;
            hpLabel.Location = new Point(6, 118);
            hpLabel.Name = "hpLabel";
            hpLabel.Size = new Size(29, 15);
            hpLabel.TabIndex = 4;
            hpLabel.Text = "HP: ";
            // 
            // manLabel
            // 
            manLabel.AutoSize = true;
            manLabel.Location = new Point(3, 133);
            manLabel.Name = "manLabel";
            manLabel.Size = new Size(28, 15);
            manLabel.TabIndex = 5;
            manLabel.Text = "MP:";
            // 
            // strLabel
            // 
            strLabel.AutoSize = true;
            strLabel.Location = new Point(3, 148);
            strLabel.Name = "strLabel";
            strLabel.Size = new Size(32, 15);
            strLabel.TabIndex = 6;
            strLabel.Text = "STR: ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(0, 163);
            label5.Name = "label5";
            label5.Size = new Size(31, 15);
            label5.TabIndex = 7;
            label5.Text = "DEX:";
            // 
            // intLabel
            // 
            intLabel.AutoSize = true;
            intLabel.Location = new Point(4, 178);
            intLabel.Name = "intLabel";
            intLabel.Size = new Size(31, 15);
            intLabel.TabIndex = 8;
            intLabel.Text = "INT: ";
            // 
            // abilitiesLabel
            // 
            abilitiesLabel.AutoSize = true;
            abilitiesLabel.Location = new Point(3, 193);
            abilitiesLabel.Name = "abilitiesLabel";
            abilitiesLabel.Size = new Size(194, 15);
            abilitiesLabel.TabIndex = 9;
            abilitiesLabel.Text = "Active Abilities: XXXX, YYYY, ZZZZZ";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(215, 6);
            label8.Name = "label8";
            label8.Size = new Size(92, 15);
            label8.TabIndex = 10;
            label8.Text = "Party Members";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 10);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(360, 350);
            tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(352, 322);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Party for Hire";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(hirePartyBtn);
            tabPage2.Controls.Add(costLabel);
            tabPage2.Controls.Add(partiesForHireList);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(label1);
            tabPage2.Controls.Add(abilitiesLabel);
            tabPage2.Controls.Add(partyDescription);
            tabPage2.Controls.Add(intLabel);
            tabPage2.Controls.Add(partyMembersList);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(hpLabel);
            tabPage2.Controls.Add(strLabel);
            tabPage2.Controls.Add(manLabel);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(352, 322);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Hire a Party";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // costLabel
            // 
            costLabel.AutoSize = true;
            costLabel.Location = new Point(6, 103);
            costLabel.Name = "costLabel";
            costLabel.Size = new Size(62, 15);
            costLabel.TabIndex = 11;
            costLabel.Text = "Gold Cost:";
            // 
            // hirePartyBtn
            // 
            hirePartyBtn.Location = new Point(6, 283);
            hirePartyBtn.Name = "hirePartyBtn";
            hirePartyBtn.Size = new Size(337, 33);
            hirePartyBtn.TabIndex = 12;
            hirePartyBtn.Text = "Hire Selected Party";
            hirePartyBtn.UseVisualStyleBackColor = true;
            // 
            // HireMultiplayerPartyWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(383, 365);
            Controls.Add(tabControl1);
            Name = "HireMultiplayerPartyWindow";
            Text = "HireMultiplayerPartyWindow";
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private RichTextBox partyDescription;
        private ListBox partiesForHireList;
        private ListBox partyMembersList;
        private Label hpLabel;
        private Label manLabel;
        private Label strLabel;
        private Label label5;
        private Label intLabel;
        private Label abilitiesLabel;
        private Label label8;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label costLabel;
        private Button hirePartyBtn;
    }
}