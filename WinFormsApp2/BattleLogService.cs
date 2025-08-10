using System.Collections.Generic;

namespace WinFormsApp2
{
    public static class BattleLogService
    {
        private static readonly Queue<string> _logs = new();
        private const int MaxLogs = 10;

        public static void AddLog(string log)
        {
            if (_logs.Count >= MaxLogs)
            {
                _logs.Dequeue();
            }
            _logs.Enqueue(log);
        }

        public static IReadOnlyList<string> GetLogs()
        {
            return _logs.ToArray();
        }
    }
}
