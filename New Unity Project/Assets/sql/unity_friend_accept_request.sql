UPDATE friend_requests SET status='accepted' WHERE requester_id=@r AND receiver_id=@u;
INSERT IGNORE INTO friends (user_id, friend_id) VALUES (@u, @r);
INSERT IGNORE INTO friends (user_id, friend_id) VALUES (@r, @u);
