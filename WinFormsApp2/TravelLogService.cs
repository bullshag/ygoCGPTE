using System;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    /// <summary>
    /// Persists travel logs for later review.
    /// </summary>
    public static class TravelLogService
    {
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
