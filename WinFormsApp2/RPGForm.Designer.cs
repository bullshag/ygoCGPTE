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
        private Button btnBattle;
        private Button btnInventory;
        private Button btnLogs;
        private Button btnNavigate;
        private Label lblGold;
        private Label lblTotalExp;
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
            btnBattle = new Button();
            btnInventory = new Button();
            btnLogs = new Button();
            btnNavigate = new Button();
            lblGold = new Label();
            lblTotalExp = new Label();
            tabSocial = new TabControl();
            tabChat = new TabPage();
            tabFriends = new TabPage();
            txtChatDisplay = new TextBox();
            txtChatInput = new TextBox();
            btnChatSend = new Button();
            lstOnline = new ListBox();
            lstFriends = new ListBox();
            lstFriendRequests = new ListBox();
            txtFriendNick = new TextBox();
            btnAddFriend = new Button();
            btnAcceptFriend = new Button();
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
            lstParty.SelectedIndexChanged += lstParty_SelectedIndexChanged;
            lstParty.Visible = false;
            //
            // pnlParty
            //
            pnlParty.Location = new Point(12, 12);
            pnlParty.Name = "pnlParty";
            pnlParty.Size = new Size(260, 154);
            pnlParty.FlowDirection = FlowDirection.TopDown;
            pnlParty.WrapContents = false;
            // 
            // btnInspect
            //
            btnInspect.Enabled = false;
            btnInspect.Location = new Point(12, 172);
            btnInspect.Name = "btnInspect";
            btnInspect.Size = new Size(260, 23);
            btnInspect.Text = "Inspect";
            btnInspect.UseVisualStyleBackColor = true;
            btnInspect.Click += btnInspect_Click;
            //
            // btnFire
            //
            btnFire.Enabled = false;
            btnFire.Location = new Point(12, 201);
            btnFire.Name = "btnFire";
            btnFire.Size = new Size(260, 23);
            btnFire.Text = "Fire";
            btnFire.UseVisualStyleBackColor = true;
            btnFire.Click += btnFire_Click;
            //
            // btnBattle
            //
            btnBattle.Location = new Point(12, 230);
            btnBattle.Name = "btnBattle";
            btnBattle.Size = new Size(260, 23);
            btnBattle.Text = "Find Battle";
            btnBattle.UseVisualStyleBackColor = true;
            btnBattle.Click += btnBattle_Click;
            //
            // btnInventory
            //
            btnInventory.Location = new Point(12, 259);
            btnInventory.Name = "btnInventory";
            btnInventory.Size = new Size(260, 23);
            btnInventory.Text = "Inventory";
            btnInventory.UseVisualStyleBackColor = true;
            btnInventory.Click += btnInventory_Click;
            //
            // btnLogs
            //
            btnLogs.Location = new Point(12, 288);
            btnLogs.Name = "btnLogs";
            btnLogs.Size = new Size(260, 23);
            btnLogs.Text = "Battle Logs";
            btnLogs.UseVisualStyleBackColor = true;
            btnLogs.Click += btnLogs_Click;
            //
            // btnNavigate
            //
            btnNavigate.Location = new Point(12, 317);
            btnNavigate.Name = "btnNavigate";
            btnNavigate.Size = new Size(260, 23);
            btnNavigate.Text = "World Map";
            btnNavigate.UseVisualStyleBackColor = true;
            btnNavigate.Click += btnNavigate_Click;
            //
            // lblGold
            //
            lblGold.AutoSize = true;
            lblGold.Location = new Point(12, 346);
            lblGold.Name = "lblGold";
            lblGold.Size = new Size(35, 15);
            lblGold.Text = "Gold:";
            //
            // lblTotalExp
            //
            lblTotalExp.AutoSize = true;
            lblTotalExp.Location = new Point(12, 371);
            lblTotalExp.Name = "lblTotalExp";
            lblTotalExp.Size = new Size(69, 15);
            lblTotalExp.Text = "Party EXP:";
            //
            // tabSocial
            //
            tabSocial.Controls.Add(tabChat);
            tabSocial.Controls.Add(tabFriends);
            tabSocial.Location = new Point(278, 12);
            tabSocial.Name = "tabSocial";
            tabSocial.SelectedIndex = 0;
            tabSocial.Size = new Size(394, 377);
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
            tabChat.Size = new Size(386, 349);
            tabChat.Text = "Chat";
            tabChat.UseVisualStyleBackColor = true;
            //
            // txtChatDisplay
            //
            txtChatDisplay.Location = new Point(6, 6);
            txtChatDisplay.Multiline = true;
            txtChatDisplay.ReadOnly = true;
            txtChatDisplay.Size = new Size(252, 300);
            //
            // txtChatInput
            //
            txtChatInput.Location = new Point(6, 312);
            txtChatInput.Size = new Size(252, 23);
            //
            // btnChatSend
            //
            btnChatSend.Location = new Point(264, 312);
            btnChatSend.Size = new Size(116, 23);
            btnChatSend.Text = "Send";
            btnChatSend.UseVisualStyleBackColor = true;
            btnChatSend.Click += btnChatSend_Click;
            //
            // lstOnline
            //
            lstOnline.Location = new Point(264, 6);
            lstOnline.Size = new Size(116, 300);
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
            tabFriends.Size = new Size(386, 349);
            tabFriends.Text = "Friends";
            tabFriends.UseVisualStyleBackColor = true;
            //
            // lstFriends
            //
            lstFriends.Location = new Point(6, 6);
            lstFriends.Size = new Size(180, 264);
            //
            // lstFriendRequests
            //
            lstFriendRequests.Location = new Point(198, 6);
            lstFriendRequests.Size = new Size(180, 264);
            //
            // txtFriendNick
            //
            txtFriendNick.Location = new Point(6, 276);
            txtFriendNick.Size = new Size(180, 23);
            //
            // btnAddFriend
            //
            btnAddFriend.Location = new Point(6, 305);
            btnAddFriend.Size = new Size(180, 23);
            btnAddFriend.Text = "Add Friend";
            btnAddFriend.UseVisualStyleBackColor = true;
            btnAddFriend.Click += btnAddFriend_Click;
            //
            // btnAcceptFriend
            //
            btnAcceptFriend.Location = new Point(198, 276);
            btnAcceptFriend.Size = new Size(180, 23);
            btnAcceptFriend.Text = "Accept";
            btnAcceptFriend.UseVisualStyleBackColor = true;
            btnAcceptFriend.Click += btnAcceptFriend_Click;
            //
            // RPGForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 426);
            Controls.Add(tabSocial);
            Controls.Add(lblTotalExp);
            Controls.Add(lblGold);
            Controls.Add(btnLogs);
            Controls.Add(btnInventory);
            Controls.Add(btnBattle);
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
