using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    partial class TavernForm
    {
        private Button _btnRecruit = null!;
        private Button btnJoin = null!;
        private Button btnHireOut = null!;

        private void InitializeComponent()
        {
            _btnRecruit = new Button();
            btnJoin = new Button();
            btnHireOut = new Button();
            SuspendLayout();
            // 
            // _btnRecruit
            // 
            _btnRecruit.Location = new Point(50, 20);
            _btnRecruit.Name = "_btnRecruit";
            _btnRecruit.Size = new Size(160, 23);
            _btnRecruit.TabIndex = 0;
            _btnRecruit.UseVisualStyleBackColor = true;
            _btnRecruit.Click += btnRecruit_Click;
            // 
            // btnJoin
            // 
            btnJoin.Location = new Point(50, 60);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(160, 23);
            btnJoin.TabIndex = 1;
            btnJoin.Text = "Join Another Party";
            btnJoin.UseVisualStyleBackColor = true;
            btnJoin.Click += btnJoin_Click;
            // 
            // btnHireOut
            // 
            btnHireOut.Location = new Point(50, 100);
            btnHireOut.Name = "btnHireOut";
            btnHireOut.Size = new Size(160, 23);
            btnHireOut.TabIndex = 2;
            btnHireOut.Text = "Leave Party for Hire";
            btnHireOut.UseVisualStyleBackColor = true;
            btnHireOut.Click += btnHireOut_Click;
            // 
            // TavernForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(280, 200);
            Controls.Add(btnHireOut);
            Controls.Add(btnJoin);
            Controls.Add(_btnRecruit);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "TavernForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Tavern";
            ResumeLayout(false);
        }
    }
}

