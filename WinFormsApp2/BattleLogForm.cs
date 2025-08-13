using System;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class BattleLogForm : Form
    {
        public BattleLogForm()
        {
            InitializeComponent();
        }

        private void BattleLogForm_Load(object? sender, EventArgs e)
        {
            var logs = BattleLogService.GetLogs();
            lstBattles.Items.Clear();
            for (int i = 0; i < logs.Count; i++)
            {
                lstBattles.Items.Add($"Battle {i + 1}");
            }
            if (lstBattles.Items.Count > 0)
            {
                lstBattles.SelectedIndex = lstBattles.Items.Count - 1;
            }
        }

        private void lstBattles_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int idx = lstBattles.SelectedIndex;
            var logs = BattleLogService.GetLogs();
            if (idx >= 0 && idx < logs.Count)
            {
                lstLog.Items.Clear();
                foreach (var line in logs[idx].Split(Environment.NewLine))
                {
                    lstLog.Items.Add(line);
                }
                if (lstLog.Items.Count > 0) lstLog.SelectedIndex = 0;
            }
        }
    }
}
