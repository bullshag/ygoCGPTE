using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FriendServiceUnity
{
    public static async Task<List<string>> GetFriendsAsync(int userId)
    {
        try
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_friend_get_friends.sql");
            var rows = await DatabaseClientUnity.QueryAsync(File.ReadAllText(sqlPath), new Dictionary<string, object?> { ["@id"] = userId });
            var list = new List<string>();
            foreach (var row in rows)
            {
                list.Add(Convert.ToString(row["nickname"]) ?? string.Empty);
            }
            return list;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch friends: {ex.Message}");
            return new List<string>();
        }
    }

    public static async Task<List<string>> GetFriendRequestsAsync(int userId)
    {
        try
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_friend_get_requests.sql");
            var rows = await DatabaseClientUnity.QueryAsync(File.ReadAllText(sqlPath), new Dictionary<string, object?> { ["@id"] = userId });
            var list = new List<string>();
            foreach (var row in rows)
            {
                list.Add(Convert.ToString(row["nickname"]) ?? string.Empty);
            }
            return list;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch friend requests: {ex.Message}");
            return new List<string>();
        }
    }

    public static async Task SendFriendRequestAsync(int requesterId, int targetId)
    {
        try
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_friend_send_request.sql");
            var parameters = new Dictionary<string, object?> { ["@r"] = requesterId, ["@t"] = targetId };
            await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(sqlPath), parameters);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to send friend request: {ex.Message}");
        }
    }

    public static async Task SendFriendRequestAsync(int requesterId, string targetNickname)
    {
        int? targetId = await ChatService.GetUserIdByNicknameAsync(targetNickname);
        if (targetId.HasValue)
        {
            await SendFriendRequestAsync(requesterId, targetId.Value);
        }
    }

    public static async Task AcceptFriendRequestAsync(int userId, int requesterId)
    {
        try
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_friend_accept_request.sql");
            var statements = File.ReadAllText(sqlPath).Split(';', StringSplitOptions.RemoveEmptyEntries);
            var parameters = new Dictionary<string, object?> { ["@u"] = userId, ["@r"] = requesterId };
            foreach (var stmt in statements)
            {
                var sql = stmt.Trim();
                if (string.IsNullOrWhiteSpace(sql)) continue;
                await DatabaseClientUnity.ExecuteAsync(sql, parameters);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to accept friend request: {ex.Message}");
        }
    }
}
