SELECT u.nickname
FROM friend_requests fr
JOIN users u ON fr.requester_id = u.id
WHERE fr.receiver_id = @id AND fr.status = 'pending';
