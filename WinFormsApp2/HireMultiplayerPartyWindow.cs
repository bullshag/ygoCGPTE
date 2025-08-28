
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public class HireableMember
    {
        public string Name { get; set; } = string.Empty;
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int MaxHp => 10 + Strength * 5;
        public override string ToString() => Name;
    }

    public class HireableParty
    {
        public string Name { get; set; } = string.Empty;
        public int Cost { get; set; }
        public List<HireableMember> Members { get; set; } = new();
        public int OwnerId { get; set; }
        public override string ToString() => Name;
    }

    public partial class HireMultiplayerPartyWindow : Form
    {
        private readonly ListBox _partyList = new();
        private readonly ListBox _memberList = new();
        private readonly Label _costLabel = new();
        private readonly Label _statsLabel = new();
        private readonly TabControl _tabs = new();
        private readonly List<HireableParty> _availableParties = new();
        private readonly int _accountId;

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

            // Hire out tab placeholder
            hireOutTab.Controls.Add(new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Depositing parties not implemented"
            });

            LoadParties();
            foreach (var party in _availableParties)
                _partyList.Items.Add(party);
        }

        private void LoadParties()
        {
            // Placeholder data until backend is implemented
            var rng = new Random();
            for (int p = 0; p < 3; p++)
            {
                var party = new HireableParty { Name = $"Party {p + 1}", Cost = 100 * (p + 1), OwnerId = p + 1 };
                for (int m = 0; m < 3; m++)
                {
                    party.Members.Add(new HireableMember
                    {
                        Name = $"Member {p + 1}-{m + 1}",
                        Strength = rng.Next(5, 11),
                        Dexterity = rng.Next(5, 11),
                        Intelligence = rng.Next(5, 11)
                    });
                }
                _availableParties.Add(party);
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
                {
                    _memberList.Items.Add(m);
                }
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
                MessageBox.Show($"Hired {party.Name} for {party.Cost}g. Gameplay not implemented.");
                TavernService.NotifyPartyHired(party.OwnerId);
                TavernService.NotifyPartyReturned(party.OwnerId);
            }

        }
    }
}
