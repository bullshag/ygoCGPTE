using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class RecruitForm : Form
    {
        private readonly List<RecruitCandidate> _candidates;
        private readonly int _userId;
        private readonly Func<int> _getSearchCost;
        private readonly Action _onHire;

        public RecruitForm(int userId, List<RecruitCandidate> candidates, Func<int> getSearchCost, Action onHire)
        {
            _userId = userId;
            _candidates = candidates;
            _getSearchCost = getSearchCost;
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
            int index = lstCandidates.SelectedIndex;
            var view = new HeroViewForm(_userId, candidate, _getSearchCost());
            view.FormClosed += (_, __) =>
            {
                if (view.DialogResult == DialogResult.OK)
                {
                    _candidates.RemoveAt(index);
                    lstCandidates.Items.RemoveAt(index);
                    _onHire();
                }
                view.Dispose();
            };
            view.Show(this);
        }
    }
}
