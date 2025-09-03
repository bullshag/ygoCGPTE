-- Queries used by the Unity MailboxManager

-- Retrieve unread mail for a user
SELECT id, subject, body FROM mailbox WHERE user_id=@userId AND is_read=0 ORDER BY created DESC;

-- Mark a message as read
UPDATE mailbox SET is_read=1 WHERE id=@messageId;
