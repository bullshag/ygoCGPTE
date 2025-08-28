
using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp2.Multiplayer;

namespace WinFormsApp2
{
    public class HireMultiplayerPartyWindow : Form
    {
        private readonly int _accountId;

        private readonly ListBox _partyList = new();
        private readonly ListBox _memberList = new();
        private readonly Label _costLabel = new();
        private readonly Label _statsLabel = new();
        private readonly TabControl _tabs = new();
        private readonly ListBox _myPartyList = new();
        private readonly NumericUpDown _costInput = new();


        public HireMultiplayerPartyWindow(int accountId, bool showHireOut = false)
        {
            _accountId = accountId;
            Text = "Multiplayer Tavern";
            Width = 520;
            Height = 360;

            _tabs.Dock = DockStyle.Fill;
            var hireTab = new TabPage("Hire a Party");
            var hireOutTab = new TabPage("Party for Hire");
            _tabs.TabPages.Add(hireTab);
            _tabs.TabPages.Add(hireOutTab);
            _tabs.SelectedIndex = showHireOut ? 1 : 0;
            Controls.Add(_tabs);

            // Hire tab controls
            _partyList.SetBounds(10, 10, 150, 240);
            _partyList.SelectedIndexChanged += PartySelected;
            hireTab.Controls.Add(_partyList);

            _memberList.SetBounds(170, 10, 150, 240);
            _memberList.SelectedIndexChanged += MemberSelected;
            hireTab.Controls.Add(_memberList);

            _statsLabel.SetBounds(330, 10, 150, 100);
            hireTab.Controls.Add(_statsLabel);

            _costLabel.SetBounds(170, 260, 150, 30);
            _costLabel.Text = "Cost: -";
            hireTab.Controls.Add(_costLabel);

            var hireButton = new Button { Text = "Hire", Left = 330, Top = 260, Width = 100 };
            hireButton.Click += (s, e) => HireSelectedParty();
            hireTab.Controls.Add(hireButton);

            // Hire-out tab controls
            _myPartyList.SetBounds(10, 10, 200, 240);
            hireOutTab.Controls.Add(_myPartyList);

            _costInput.SetBounds(220, 10, 80, 23);
            _costInput.Minimum = 1;
            _costInput.Maximum = 10000;
            _costInput.Value = 100;
            hireOutTab.Controls.Add(_costInput);

            var depositBtn = new Button { Text = "Deposit", Left = 220, Top = 40, Width = 100 };
            depositBtn.Click += (s, e) => DepositParty();
            hireOutTab.Controls.Add(depositBtn);

            var withdrawBtn = new Button { Text = "Withdraw", Left = 220, Top = 80, Width = 100 };
            withdrawBtn.Click += (s, e) => WithdrawParty();
            hireOutTab.Controls.Add(withdrawBtn);

            RefreshLists();
        }

        private void RefreshLists()
        {
            _partyList.Items.Clear();
            foreach (var party in PartyHireService.GetAvailableParties())
                _partyList.Items.Add(party);

            _myPartyList.Items.Clear();
            foreach (var party in PartyHireService.GetOwnerParties(_accountId))
            {
                string status = party.OnMission ? "(Hired)" : "(Idle)";
                _myPartyList.Items.Add($"{party.Name} {status} - Earned {party.GoldEarned}g");

            }
        }

        private void PartySelected(object? sender, EventArgs e)
        {
            _memberList.Items.Clear();
            _statsLabel.Text = string.Empty;
            if (_partyList.SelectedItem is HireableParty party)
            {
                _costLabel.Text = $"Cost: {party.Cost}g";
                foreach (var m in party.Members)
                    _memberList.Items.Add(m);

            }
            else
            {
                _costLabel.Text = "Cost: -";
            }
        }

        private void MemberSelected(object? sender, EventArgs e)
        {
            if (_memberList.SelectedItem is HireableMember m)
            {
                _statsLabel.Text = $"STR: {m.Strength}\nDEX: {m.Dexterity}\nINT: {m.Intelligence}\nHP: {m.MaxHp}";
            }
            else
            {
                _statsLabel.Text = string.Empty;
            }
        }

        private void HireSelectedParty()
        {
            if (_partyList.SelectedItem is HireableParty party)
            {
                if (PartyHireService.HireParty(_accountId, party))
                {
                    MessageBox.Show($"Hired {party.Name} for {party.Cost}g.");
                    RefreshLists();
                }
                else
                {
                    MessageBox.Show("Unable to hire party.");
                }
            }
        }

        private void DepositParty()
        {
            int cost = (int)_costInput.Value;
            if (PartyHireService.DepositAccountParty(_accountId, cost))
            {
                MessageBox.Show("Party deposited for hire.");
                RefreshLists();
            }
            else
            {
                MessageBox.Show("No available party to deposit.");
            }
        }

        private void WithdrawParty()
        {
            int index = _myPartyList.SelectedIndex;
            if (index < 0) return;
            var parties = PartyHireService.GetOwnerParties(_accountId);
            if (index >= parties.Count) return;
            var party = parties[index];
            if (party.OnMission)
            {
                MessageBox.Show("Party is currently hired and cannot be withdrawn.");
                return;
            }
            int gold = PartyHireService.RetrieveParty(_accountId, party);
            MessageBox.Show(gold > 0 ? $"Party retrieved. Earned {gold}g." : "Party retrieved.");
            RefreshLists();
        }
    }
}

