using System;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    /// <summary>
    /// Persists travel logs for later review.
    /// </summary>
    public static class TravelLogService
    {
        private static readonly Random _rng = new Random();

        public static string GetDepartureFlavor(string fromNode, string toNode)
        {
            var from = WorldMapService.GetNode(fromNode).Name;
            var to = WorldMapService.GetNode(toNode).Name;
            string[] templates =
            {
                "The party leaves {0} and heads toward {1}.",
                "You depart {0}, bound for {1}.",
                "Setting out from {0}, the road stretches toward {1}."
            };
            return string.Format(templates[_rng.Next(templates.Length)], from, to);
        }

        public static string GetArrivalFlavor(string nodeId)
        {
            var name = WorldMapService.GetNode(nodeId).Name;
            string[] templates =
            {
                "You arrive at {0}, a welcome sight.",
                "The town of {0} greets the weary travelers.",
                "At last, {0} comes into view."
            };
            return string.Format(templates[_rng.Next(templates.Length)], name);
        }

        public static void LogJourney(int accountId, string fromNode, string toNode, int originalDays, int finalDays, int cost, bool fasterTravel)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO travel_logs(account_id,from_node,to_node,start_time,end_time,original_days,travel_days,cost_gold,faster_travel_applied)"+
                " VALUES(@a,@f,@t,@s,@e,@o,@d,@c,@fast)", conn);
            cmd.Parameters.AddWithValue("@a", accountId);
            cmd.Parameters.AddWithValue("@f", fromNode);
            cmd.Parameters.AddWithValue("@t", toNode);
            cmd.Parameters.AddWithValue("@s", DateTime.UtcNow.AddSeconds(-finalDays * 60));
            cmd.Parameters.AddWithValue("@e", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@o", originalDays);
            cmd.Parameters.AddWithValue("@d", finalDays);
            cmd.Parameters.AddWithValue("@c", cost);
            cmd.Parameters.AddWithValue("@fast", fasterTravel);
            cmd.ExecuteNonQuery();
        }
    }
}
