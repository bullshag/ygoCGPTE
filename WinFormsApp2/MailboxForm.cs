using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public class MailboxForm : Form
    {
        private readonly int _accountId;
        private readonly ListBox _lst = new ListBox();
        private readonly TextBox _body = new TextBox();
        private readonly Button _refresh = new Button();

        public MailboxForm(int accountId)
        {
            _accountId = accountId;
            Text = "Mailbox";
            Width = 500;
            Height = 420;

            _lst.SetBounds(10, 10, 200, 330);
            _lst.SelectedIndexChanged += (s, e) =>
            {
                if (_lst.SelectedItem is MailItem mail)
                    _body.Text = mail.Body;
            };

            _body.SetBounds(220, 10, 260, 330);
            _body.Multiline = true;
            _body.ReadOnly = true;

            _refresh.Text = "Refresh";
            _refresh.SetBounds(10, 350, 100, 25);
            _refresh.Click += (s, e) => LoadMail();

            Controls.AddRange(new Control[] { _lst, _body, _refresh });

            LoadMail();
        }

        private void LoadMail()
        {
            List<MailItem> mails = MailService.GetUnread(_accountId);
            _lst.Items.Clear();
            foreach (var m in mails)
                _lst.Items.Add(m);
            _body.Clear();
        }
    }
}
