using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class RecruitForm : Form
    {
        private readonly List<RecruitCandidate> _candidates;
        private readonly int _userId;
        private readonly int _searchCost;
        private readonly Action _onHire;

        public RecruitForm(int userId, List<RecruitCandidate> candidates, int searchCost, Action onHire)
        {
            _userId = userId;
            _candidates = candidates;
            _searchCost = searchCost;
            _onHire = onHire;
            InitializeComponent();
            foreach (var c in _candidates)
            {
                lstCandidates.Items.Add(c.Name);
            }
            lstCandidates.SelectedIndexChanged += (s, e) =>
            {
                btnView.Enabled = lstCandidates.SelectedIndex >= 0;
            };
        }

        private void btnView_Click(object? sender, EventArgs e)
        {
            if (lstCandidates.SelectedIndex < 0) return;
            var candidate = _candidates[lstCandidates.SelectedIndex];
            using var view = new HeroViewForm(_userId, candidate, _searchCost);
            if (view.ShowDialog(this) == DialogResult.OK)
            {
                int index = lstCandidates.SelectedIndex;
                _candidates.RemoveAt(index);
                lstCandidates.Items.RemoveAt(index);
                _onHire();
            }
        }
    }
}
