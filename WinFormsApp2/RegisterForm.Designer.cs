using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer? components = null;
        private TextBox txtUsername = null!;
        private TextBox txtNickname = null!;
        private TextBox txtPassword = null!;
        private TextBox txtConfirmPassword = null!;
        private Button btnRegister = null!;
        private Label lblUsername = null!;
        private Label lblNickname = null!;
        private Label lblPassword = null!;
        private Label lblConfirm = null!;

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
            txtUsername = new TextBox();
            txtNickname = new TextBox();
            txtPassword = new TextBox();
            txtConfirmPassword = new TextBox();
            btnRegister = new Button();
            lblUsername = new Label();
            lblNickname = new Label();
            lblPassword = new Label();
            lblConfirm = new Label();
            chkDebugMode2 = new CheckBox();
            kimCheckbox = new CheckBox();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(30, 48);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(200, 23);
            txtUsername.TabIndex = 1;
            // 
            // txtNickname
            // 
            txtNickname.Location = new Point(30, 108);
            txtNickname.Name = "txtNickname";
            txtNickname.Size = new Size(200, 23);
            txtNickname.TabIndex = 3;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(30, 168);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(200, 23);
            txtPassword.TabIndex = 5;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Location = new Point(30, 228);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(200, 23);
            txtConfirmPassword.TabIndex = 7;
            txtConfirmPassword.UseSystemPasswordChar = true;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(30, 270);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(100, 23);
            btnRegister.TabIndex = 8;
            btnRegister.Text = "Create";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(30, 30);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(60, 15);
            lblUsername.TabIndex = 0;
            lblUsername.Text = "Username";
            // 
            // lblNickname
            // 
            lblNickname.AutoSize = true;
            lblNickname.Location = new Point(30, 90);
            lblNickname.Name = "lblNickname";
            lblNickname.Size = new Size(61, 15);
            lblNickname.TabIndex = 2;
            lblNickname.Text = "Nickname";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(30, 150);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(57, 15);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Password";
            // 
            // lblConfirm
            // 
            lblConfirm.AutoSize = true;
            lblConfirm.Location = new Point(30, 210);
            lblConfirm.Name = "lblConfirm";
            lblConfirm.Size = new Size(104, 15);
            lblConfirm.TabIndex = 6;
            lblConfirm.Text = "Confirm Password";
            // 
            // chkDebugMode2
            //
            chkDebugMode2.AutoSize = true;
            chkDebugMode2.Location = new Point(39, 301);
            chkDebugMode2.Name = "chkDebugMode2";
            chkDebugMode2.Size = new Size(95, 19);
            chkDebugMode2.TabIndex = 9;
            chkDebugMode2.Text = "Debug Mode";
            chkDebugMode2.UseVisualStyleBackColor = true;
            //
            // kimCheckbox
            //
            kimCheckbox.AutoSize = true;
            kimCheckbox.Location = new Point(39, 326);
            kimCheckbox.Name = "kimCheckbox";
            kimCheckbox.Size = new Size(123, 19);
            kimCheckbox.TabIndex = 10;
            kimCheckbox.Text = "Use Kim address";
            kimCheckbox.UseVisualStyleBackColor = true;
            kimCheckbox.CheckedChanged += kimCheckbox_CheckedChanged;
            //
            // RegisterForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(264, 356);
            Controls.Add(kimCheckbox);
            Controls.Add(chkDebugMode2);
            Controls.Add(btnRegister);
            Controls.Add(txtConfirmPassword);
            Controls.Add(lblConfirm);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtNickname);
            Controls.Add(lblNickname);
            Controls.Add(txtUsername);
            Controls.Add(lblUsername);
            Name = "RegisterForm";
            Text = "Create Account";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkDebugMode2;
        private CheckBox kimCheckbox;
    }
}
