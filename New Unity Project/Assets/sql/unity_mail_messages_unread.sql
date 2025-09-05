SELECT id, sender_id, subject, body, sent_at
FROM mail_messages
WHERE recipient_id = @accountId AND is_read = 0
ORDER BY sent_at;
