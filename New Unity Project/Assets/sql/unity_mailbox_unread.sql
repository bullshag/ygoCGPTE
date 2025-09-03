-- Retrieve unread mail for a user
SELECT id, sender_id, subject, body FROM mailbox WHERE user_id=@accountId AND is_read=0 ORDER BY created DESC;

