using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public static class FriendService
    {
        public static List<string> GetFriends(int userId)
        {
            var list = new List<string>();
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(@"SELECT u.nickname FROM friends f JOIN users u ON f.friend_id=u.id WHERE f.user_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetString("nickname"));
            }
            return list;
        }

        public static List<string> GetFriendRequests(int userId)
        {
            var list = new List<string>();
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(@"SELECT u.nickname FROM friend_requests fr JOIN users u ON fr.requester_id=u.id WHERE fr.receiver_id=@id AND fr.status='pending'", conn);
            cmd.Parameters.AddWithValue("@id", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetString("nickname"));
            }
            return list;
        }

        public static void SendFriendRequest(int requesterId, int targetId)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("INSERT INTO friend_requests (requester_id, receiver_id) VALUES (@r, @t)", conn);
            cmd.Parameters.AddWithValue("@r", requesterId);
            cmd.Parameters.AddWithValue("@t", targetId);
            cmd.ExecuteNonQuery();
        }

        public static void AcceptFriendRequest(int userId, int requesterId)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE friend_requests SET status='accepted' WHERE requester_id=@r AND receiver_id=@u", conn);
            cmd.Parameters.AddWithValue("@r", requesterId);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.ExecuteNonQuery();
            using MySqlCommand add1 = new MySqlCommand("INSERT IGNORE INTO friends (user_id, friend_id) VALUES (@u,@r)", conn);
            add1.Parameters.AddWithValue("@u", userId);
            add1.Parameters.AddWithValue("@r", requesterId);
            add1.ExecuteNonQuery();
            using MySqlCommand add2 = new MySqlCommand("INSERT IGNORE INTO friends (user_id, friend_id) VALUES (@r,@u)", conn);
            add2.Parameters.AddWithValue("@u", userId);
            add2.Parameters.AddWithValue("@r", requesterId);
            add2.ExecuteNonQuery();
        }
    }
}

