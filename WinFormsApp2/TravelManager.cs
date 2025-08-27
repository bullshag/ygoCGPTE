using System;
using System.Timers;
using System.Threading;
using MySql.Data.MySqlClient;
using Timer = System.Timers.Timer;

namespace WinFormsApp2
{
    /// <summary>
    /// Handles travel timing, cost calculations and persistence. The manager
    /// stores progress in the database so travel can resume if the application
    /// closes.
    /// </summary>
    public class TravelManager
    {
        private readonly int _accountId;
        private readonly Timer _timer = new Timer(1000);
        private int _totalSeconds;
        private int _elapsedSeconds;
        private int _originalDays;
        private int _travelCost;
        private string? _fromNode;
        private string? _toNode;
        private readonly Random _rng = new Random();
        private bool _fasterTravelApplied;
        private readonly SynchronizationContext _syncContext;
        private int _partySize;
        private bool _pausedForEncounter;

        public event Action<int>? ProgressChanged;
        public event Action<string>? TravelCompleted;
        public event Action? AmbushEncounter;
        public bool IsTraveling => _timer.Enabled;

        public TravelManager(int accountId)
        {
            _accountId = accountId;
            _timer.Elapsed += Timer_Elapsed;
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        public void StartTravel(string fromNode, string toNode, int partySize, bool hasFasterTravel)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _partySize = partySize;
            EnsureNodeExists(fromNode);
            EnsureNodeExists(toNode);
            _originalDays = WorldMapService.GetNode(fromNode).Connections[toNode];
            int days = _originalDays;
            _fasterTravelApplied = hasFasterTravel && days > 1;
            if (_fasterTravelApplied) days -= 1;
            _totalSeconds = days * 60;
            _elapsedSeconds = 0;
            _timer.Start();
            _travelCost = days * partySize * 5;
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("UPDATE users SET gold = gold - @c WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@c", _travelCost);
            cmd.Parameters.AddWithValue("@id", _accountId);
            cmd.ExecuteNonQuery();
            SaveState();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _elapsedSeconds++;
            int percent = _elapsedSeconds * 100 / Math.Max(1, _totalSeconds);
            _syncContext.Post(state => ProgressChanged?.Invoke((int)state!), percent);
            if (_partySize > 0 && _elapsedSeconds % 30 == 0)
            {
                if (_rng.NextDouble() < 0.15)
                {
                    _timer.Stop();
                    _pausedForEncounter = true;
                    SaveState();
                    _syncContext.Post(_ => AmbushEncounter?.Invoke(), null);
                    return;
                }
            }
            if (_elapsedSeconds >= _totalSeconds)
            {
                _timer.Stop();
                CompleteTravel();
            }
            else
            {
                SaveState();
            }
        }

        public void ResumeAfterEncounter()
        {
            if (_pausedForEncounter)
            {
                _pausedForEncounter = false;
                _timer.Start();
            }
        }

        private void CompleteTravel()
        {
            if (_fromNode == null || _toNode == null) return;
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using (var cmd = new MySqlCommand("REPLACE INTO travel_state(account_id,current_node,destination_node,start_time,arrival_time,progress_seconds,faster_travel,travel_cost) VALUES (@a,@curr,@dest,NULL,NULL,0,0,0)", conn))
            {
                cmd.Parameters.AddWithValue("@a", _accountId);
                cmd.Parameters.AddWithValue("@curr", _toNode);
                cmd.Parameters.AddWithValue("@dest", _toNode);
                cmd.ExecuteNonQuery();
            }
            int finalDays = _totalSeconds / 60;
            TravelLogService.LogJourney(_accountId, _fromNode, _toNode, _originalDays, finalDays, _travelCost, _fasterTravelApplied);
            string nodeId = _toNode;
            _syncContext.Post(state => TravelCompleted?.Invoke((string)state!), nodeId);
        }

        public void SaveState()
        {
            if (_fromNode == null || _toNode == null) return;
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("REPLACE INTO travel_state(account_id,current_node,destination_node,start_time,arrival_time,progress_seconds,faster_travel,travel_cost) VALUES (@a,@curr,@dest,@start,@arr,@prog,@fast,@cost)", conn);
            cmd.Parameters.AddWithValue("@a", _accountId);
            cmd.Parameters.AddWithValue("@curr", _fromNode);
            cmd.Parameters.AddWithValue("@dest", _toNode);
            cmd.Parameters.AddWithValue("@start", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@arr", DateTime.UtcNow.AddSeconds(_totalSeconds - _elapsedSeconds));
            cmd.Parameters.AddWithValue("@prog", _elapsedSeconds);
            cmd.Parameters.AddWithValue("@fast", _fasterTravelApplied);
            cmd.Parameters.AddWithValue("@cost", _travelCost);
            cmd.ExecuteNonQuery();
        }

        public void Resume()
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT current_node,destination_node,progress_seconds,arrival_time,faster_travel,travel_cost FROM travel_state WHERE account_id=@a", conn);
            cmd.Parameters.AddWithValue("@a", _accountId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                _fromNode = reader.GetString("current_node");
                _toNode = reader.GetString("destination_node");
                _elapsedSeconds = reader.GetInt32("progress_seconds");
                _fasterTravelApplied = reader.GetBoolean("faster_travel");
                _travelCost = reader.GetInt32("travel_cost");
                if (_fromNode == _toNode)
                {
                    // Not currently traveling; nothing to resume
                    _originalDays = 0;
                    _totalSeconds = 0;
                    _timer.Stop();
                }
                else
                {
                    _originalDays = WorldMapService.GetNode(_fromNode).Connections[_toNode];
                    int days = _originalDays;
                    if (_fasterTravelApplied && days > 1) days -= 1;
                    _totalSeconds = days * 60;
                    _timer.Start();
                }
            }
        }

        private static void EnsureNodeExists(string nodeId)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("INSERT IGNORE INTO nodes (id) VALUES (@id)", conn);
            cmd.Parameters.AddWithValue("@id", nodeId);
            cmd.ExecuteNonQuery();
        }
    }
}
