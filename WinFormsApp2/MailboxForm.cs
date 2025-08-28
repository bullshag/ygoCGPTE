using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class MailboxForm : Form
    {
        private readonly int _accountId;

        public MailboxForm(int accountId)
        {
            _accountId = accountId;
            InitializeComponent();
            LoadMail();
        }

        private void lstMail_SelectedIndexChanged(object? sender, System.EventArgs e)
        {
            if (lstMail.SelectedItem is MailItem mail)
                txtBody.Text = mail.Body;
        }

        private void btnRefresh_Click(object? sender, System.EventArgs e)
        {
            LoadMail();
        }

        private void LoadMail()
        {
            List<MailItem> mails = MailService.GetUnread(_accountId);
            lstMail.Items.Clear();
            foreach (var m in mails)
                lstMail.Items.Add(m);
            txtBody.Clear();
        }
    }
}
