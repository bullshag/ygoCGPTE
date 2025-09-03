-- Ensures starting node exists
INSERT IGNORE INTO nodes (id, name) VALUES (@node, @name);
