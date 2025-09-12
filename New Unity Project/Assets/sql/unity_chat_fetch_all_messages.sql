SELECT c.sender_id, u.nickname AS sender, c.message, c.sent_at, r.nickname AS recipient
FROM chat_messages c
JOIN users u ON c.sender_id = u.id
LEFT JOIN users r ON c.recipient_id = r.id
ORDER BY c.sent_at;
