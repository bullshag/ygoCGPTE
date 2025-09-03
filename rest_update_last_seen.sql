-- Updates last_seen for a given account
UPDATE accounts SET last_seen = NOW() WHERE id = @id;
