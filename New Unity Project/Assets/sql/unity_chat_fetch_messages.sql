SELECT c.sender_id, u.nickname AS sender, c.message, c.sent_at, r.nickname AS recipient
FROM chat_messages c
JOIN users u ON c.sender_id = u.id
LEFT JOIN users r ON c.recipient_id = r.id
WHERE c.sent_at > @since AND (c.recipient_id IS NULL OR c.sender_id = @uid OR c.recipient_id = @uid)
ORDER BY c.sent_at;
