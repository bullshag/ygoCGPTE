using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public class ChatMessage
    {
        public string Sender { get; set; } = string.Empty;
        public string? Recipient { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public static class ChatService
    {
        public static List<ChatMessage> GetMessages()
        {
            var list = new List<ChatMessage>();
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            string sql = File.ReadAllText("fetch_all_chat_messages.sql");
            using MySqlCommand cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ChatMessage
                {
                    Sender = reader.GetString("sender"),
                    Recipient = reader["recipient"] as string,
                    Message = reader.GetString("message"),
                    SentAt = reader.GetDateTime("sent_at")
                });
            }
            return list;
        }

        public static void SendMessage(int senderId, int? recipientId, string message)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("INSERT INTO chat_messages (sender_id, recipient_id, message) VALUES (@s, @r, @m)", conn);
            cmd.Parameters.AddWithValue("@s", senderId);
            cmd.Parameters.AddWithValue("@r", recipientId.HasValue ? recipientId.Value : (object?)DBNull.Value);
            cmd.Parameters.AddWithValue("@m", message);
            cmd.ExecuteNonQuery();
        }

        public static List<string> GetOnlinePlayers()
        {
            var list = new List<string>();
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT nickname FROM users WHERE last_seen > DATE_SUB(NOW(), INTERVAL 5 SECOND)", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetString("nickname"));
            }
            return list;
        }

        public static void UpdateLastSeen(int userId)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("UPDATE users SET last_seen=NOW() WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
        }

        public static int? GetUserIdByNickname(string nickname)
        {
            using MySqlConnection conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT id FROM users WHERE nickname=@n", conn);
            cmd.Parameters.AddWithValue("@n", nickname);
            object? result = cmd.ExecuteScalar();
            if (result == null) return null;
            return Convert.ToInt32(result);
        }
    }
}

