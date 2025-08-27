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
        private FlowLayoutPanel pnlParty;
        private Button btnInspect;
        private Button btnFire;
        private Button btnInventory;
        private Button btnLogs;
        private Button btnNavigate;
        private Label lblGold;
        private Label lblTotalExp;
        private Label partyPowerLabel;
        private TabControl tabSocial;
        private TabPage tabChat;
        private TabPage tabFriends;
        private TextBox txtChatDisplay;
        private TextBox txtChatInput;
        private Button btnChatSend;
        private ListBox lstOnline;
        private ListBox lstFriends;
        private ListBox lstFriendRequests;
        private TextBox txtFriendNick;
        private Button btnAddFriend;
        private Button btnAcceptFriend;

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
            pnlParty = new FlowLayoutPanel();
            btnInspect = new Button();
            btnFire = new Button();
            btnInventory = new Button();
            btnLogs = new Button();
            btnNavigate = new Button();
            lblGold = new Label();
            lblTotalExp = new Label();
            partyPowerLabel = new Label();
            tabSocial = new TabControl();
            tabChat = new TabPage();
            txtChatDisplay = new TextBox();
            txtChatInput = new TextBox();
            btnChatSend = new Button();
            lstOnline = new ListBox();
            tabFriends = new TabPage();
            lstFriends = new ListBox();
            lstFriendRequests = new ListBox();
            txtFriendNick = new TextBox();
            btnAddFriend = new Button();
            btnAcceptFriend = new Button();
            partyPowerLabel = new Label();
            tabSocial.SuspendLayout();
            tabChat.SuspendLayout();
            tabFriends.SuspendLayout();
            SuspendLayout();
            // 
            // lstParty
            // 
            lstParty.FormattingEnabled = true;
            lstParty.ItemHeight = 15;
            lstParty.Location = new Point(12, 12);
            lstParty.Name = "lstParty";
            lstParty.Size = new Size(260, 154);
            lstParty.TabIndex = 9;
            lstParty.Visible = false;
            lstParty.SelectedIndexChanged += lstParty_SelectedIndexChanged;
            // 
            // pnlParty
            // 
            pnlParty.AutoScroll = true;
            pnlParty.FlowDirection = FlowDirection.TopDown;
            pnlParty.Location = new Point(12, 12);
            pnlParty.Name = "pnlParty";
            pnlParty.Size = new Size(273, 598);
            pnlParty.TabIndex = 8;
            pnlParty.WrapContents = false;
            // 
            // btnInspect
            // 
            btnInspect.Enabled = false;
            btnInspect.Location = new Point(291, 12);
            btnInspect.Name = "btnInspect";
            btnInspect.Size = new Size(260, 23);
            btnInspect.TabIndex = 6;
            btnInspect.Text = "Inspect";
            btnInspect.UseVisualStyleBackColor = true;
            btnInspect.Click += btnInspect_Click;
            // 
            // btnFire
            // 
            btnFire.Enabled = false;
            btnFire.Location = new Point(291, 41);
            btnFire.Name = "btnFire";
            btnFire.Size = new Size(260, 23);
            btnFire.TabIndex = 5;
            btnFire.Text = "Fire";
            btnFire.UseVisualStyleBackColor = true;
            btnFire.Click += btnFire_Click;
            // 
            // btnInventory
            // 
            btnInventory.Location = new Point(291, 70);
            btnInventory.Name = "btnInventory";
            btnInventory.Size = new Size(260, 23);
            btnInventory.TabIndex = 4;
            btnInventory.Text = "Inventory";
            btnInventory.UseVisualStyleBackColor = true;
            btnInventory.Click += btnInventory_Click;
            // 
            // btnLogs
            // 
            btnLogs.Location = new Point(291, 99);
            btnLogs.Name = "btnLogs";
            btnLogs.Size = new Size(260, 23);
            btnLogs.TabIndex = 3;
            btnLogs.Text = "Battle Logs";
            btnLogs.UseVisualStyleBackColor = true;
            btnLogs.Click += btnLogs_Click;
            // 
            // btnNavigate
            // 
            btnNavigate.Location = new Point(291, 128);
            btnNavigate.Name = "btnNavigate";
            btnNavigate.Size = new Size(260, 23);
            btnNavigate.TabIndex = 7;
            btnNavigate.Text = "World Map";
            btnNavigate.UseVisualStyleBackColor = true;
            btnNavigate.Click += btnNavigate_Click;
            // 
            // lblGold
            // 
            lblGold.AutoSize = true;
            lblGold.Location = new Point(567, 16);
            lblGold.Name = "lblGold";
            lblGold.Size = new Size(35, 15);
            lblGold.TabIndex = 2;
            lblGold.Text = "Gold:";
            // 
            // lblTotalExp
            //
            lblTotalExp.AutoSize = true;
            lblTotalExp.Location = new Point(567, 45);
            lblTotalExp.Name = "lblTotalExp";
            lblTotalExp.Size = new Size(60, 15);
            lblTotalExp.TabIndex = 1;
            lblTotalExp.Text = "Party EXP:";
            //
            // partyPowerLabel
            //
            partyPowerLabel.AutoSize = true;
            partyPowerLabel.Location = new Point(567, 74);
            partyPowerLabel.Name = "partyPowerLabel";
            partyPowerLabel.Size = new Size(82, 15);
            partyPowerLabel.TabIndex = 10;
            partyPowerLabel.Text = "Party Power:";
            // 
            // tabSocial
            // 
            tabSocial.Controls.Add(tabChat);
            tabSocial.Controls.Add(tabFriends);
            tabSocial.Location = new Point(287, 157);
            tabSocial.Name = "tabSocial";
            tabSocial.SelectedIndex = 0;
            tabSocial.Size = new Size(394, 457);
            tabSocial.TabIndex = 0;
            // 
            // tabChat
            // 
            tabChat.Controls.Add(txtChatDisplay);
            tabChat.Controls.Add(txtChatInput);
            tabChat.Controls.Add(btnChatSend);
            tabChat.Controls.Add(lstOnline);
            tabChat.Location = new Point(4, 24);
            tabChat.Name = "tabChat";
            tabChat.Padding = new Padding(3);
            tabChat.Size = new Size(386, 429);
            tabChat.TabIndex = 0;
            tabChat.Text = "Chat";
            tabChat.UseVisualStyleBackColor = true;
            // 
            // txtChatDisplay
            // 
            txtChatDisplay.Location = new Point(6, 6);
            txtChatDisplay.Multiline = true;
            txtChatDisplay.Name = "txtChatDisplay";
            txtChatDisplay.ReadOnly = true;
            txtChatDisplay.Size = new Size(252, 300);
            txtChatDisplay.TabIndex = 0;
            // 
            // txtChatInput
            // 
            txtChatInput.Location = new Point(6, 312);
            txtChatInput.Name = "txtChatInput";
            txtChatInput.Size = new Size(252, 23);
            txtChatInput.TabIndex = 1;
            // 
            // btnChatSend
            // 
            btnChatSend.Location = new Point(264, 312);
            btnChatSend.Name = "btnChatSend";
            btnChatSend.Size = new Size(116, 23);
            btnChatSend.TabIndex = 2;
            btnChatSend.Text = "Send";
            btnChatSend.UseVisualStyleBackColor = true;
            btnChatSend.Click += btnChatSend_Click;
            // 
            // lstOnline
            // 
            lstOnline.ItemHeight = 15;
            lstOnline.Location = new Point(264, 6);
            lstOnline.Name = "lstOnline";
            lstOnline.Size = new Size(116, 289);
            lstOnline.TabIndex = 3;
            // 
            // tabFriends
            // 
            tabFriends.Controls.Add(lstFriends);
            tabFriends.Controls.Add(lstFriendRequests);
            tabFriends.Controls.Add(txtFriendNick);
            tabFriends.Controls.Add(btnAddFriend);
            tabFriends.Controls.Add(btnAcceptFriend);
            tabFriends.Location = new Point(4, 24);
            tabFriends.Name = "tabFriends";
            tabFriends.Padding = new Padding(3);
            tabFriends.Size = new Size(386, 429);
            tabFriends.TabIndex = 1;
            tabFriends.Text = "Friends";
            tabFriends.UseVisualStyleBackColor = true;
            // 
            // lstFriends
            // 
            lstFriends.ItemHeight = 15;
            lstFriends.Location = new Point(6, 6);
            lstFriends.Name = "lstFriends";
            lstFriends.Size = new Size(180, 259);
            lstFriends.TabIndex = 0;
            // 
            // lstFriendRequests
            // 
            lstFriendRequests.ItemHeight = 15;
            lstFriendRequests.Location = new Point(198, 6);
            lstFriendRequests.Name = "lstFriendRequests";
            lstFriendRequests.Size = new Size(180, 259);
            lstFriendRequests.TabIndex = 1;
            // 
            // txtFriendNick
            // 
            txtFriendNick.Location = new Point(6, 276);
            txtFriendNick.Name = "txtFriendNick";
            txtFriendNick.Size = new Size(180, 23);
            txtFriendNick.TabIndex = 2;
            // 
            // btnAddFriend
            // 
            btnAddFriend.Location = new Point(6, 305);
            btnAddFriend.Name = "btnAddFriend";
            btnAddFriend.Size = new Size(180, 23);
            btnAddFriend.TabIndex = 3;
            btnAddFriend.Text = "Add Friend";
            btnAddFriend.UseVisualStyleBackColor = true;
            btnAddFriend.Click += btnAddFriend_Click;
            // 
            // btnAcceptFriend
            // 
            btnAcceptFriend.Location = new Point(198, 276);
            btnAcceptFriend.Name = "btnAcceptFriend";
            btnAcceptFriend.Size = new Size(180, 23);
            btnAcceptFriend.TabIndex = 4;
            btnAcceptFriend.Text = "Accept";
            btnAcceptFriend.UseVisualStyleBackColor = true;
            btnAcceptFriend.Click += btnAcceptFriend_Click;
            // 
            // partyPowerLabel
            // 
            partyPowerLabel.AutoSize = true;
            partyPowerLabel.Location = new Point(571, 72);
            partyPowerLabel.Name = "partyPowerLabel";
            partyPowerLabel.Size = new Size(38, 15);
            partyPowerLabel.TabIndex = 10;
            partyPowerLabel.Text = "label1";
            // 
            // RPGForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 626);
            Controls.Add(partyPowerLabel);
            Controls.Add(tabSocial);
            Controls.Add(lblTotalExp);
            Controls.Add(partyPowerLabel);
            Controls.Add(lblGold);
            Controls.Add(btnLogs);
            Controls.Add(btnInventory);
            Controls.Add(btnFire);
            Controls.Add(btnInspect);
            Controls.Add(btnNavigate);
            Controls.Add(pnlParty);
            Controls.Add(lstParty);
            Name = "RPGForm";
            Text = "Party";
            Load += RPGForm_Load;
            tabSocial.ResumeLayout(false);
            tabChat.ResumeLayout(false);
            tabChat.PerformLayout();
            tabFriends.ResumeLayout(false);
            tabFriends.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

    }
}
