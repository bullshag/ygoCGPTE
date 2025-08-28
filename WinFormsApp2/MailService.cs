using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace WinFormsApp2
{
    public class MailItem
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public override string ToString() => $"{SentAt:g} - {Subject}";
    }

    public static class MailService
    {
        public static void SendMail(int? senderId, int recipientId, string subject, string body)
        {
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("INSERT INTO mail_messages(sender_id,recipient_id,subject,body) VALUES(@s,@r,@u,@b)", conn);
            cmd.Parameters.AddWithValue("@s", senderId.HasValue ? senderId.Value : (object?)DBNull.Value);
            cmd.Parameters.AddWithValue("@r", recipientId);
            cmd.Parameters.AddWithValue("@u", subject);
            cmd.Parameters.AddWithValue("@b", body);
            cmd.ExecuteNonQuery();
        }

        public static List<MailItem> GetUnread(int accountId)
        {
            var list = new List<MailItem>();
            using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT id,subject,body,sent_at FROM mail_messages WHERE recipient_id=@r AND is_read=0 ORDER BY sent_at", conn);
            cmd.Parameters.AddWithValue("@r", accountId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new MailItem
                {
                    Id = reader.GetInt32("id"),
                    Subject = reader.GetString("subject"),
                    Body = reader.GetString("body"),
                    SentAt = reader.GetDateTime("sent_at")
                });
            }
            reader.Close();
            foreach (var mail in list)
            {
                using var upd = new MySqlCommand("UPDATE mail_messages SET is_read=1 WHERE id=@i", conn);
                upd.Parameters.AddWithValue("@i", mail.Id);
                upd.ExecuteNonQuery();
            }
            return list;
        }
    }
}
