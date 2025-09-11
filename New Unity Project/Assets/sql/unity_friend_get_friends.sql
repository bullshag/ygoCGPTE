SELECT u.nickname
FROM friends f
JOIN users u ON f.friend_id = u.id
WHERE f.user_id = @id;
