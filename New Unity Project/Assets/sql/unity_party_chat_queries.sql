-- Fetch party members for a player
SELECT c.name, c.hp, c.max_hp, c.mana, c.max_mana
FROM party_members pm
JOIN characters c ON pm.character_id = c.id
WHERE pm.user_id = ?;

-- Fetch gold for a player
SELECT gold
FROM users
WHERE id = ?;

-- Fetch recent chat messages visible to a player
SELECT s.nickname AS sender,
       r.nickname AS recipient,
       m.message,
       m.sent_at
FROM chat_messages m
JOIN users s ON m.sender_id = s.id
LEFT JOIN users r ON m.recipient_id = r.id
WHERE m.sent_at > ?
ORDER BY m.sent_at;
