-- Mark a mailbox message as read
UPDATE mailbox SET is_read=1 WHERE id=@messageId;

