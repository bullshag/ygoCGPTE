-- Ensures starting node exists for Unity RegisterManager
INSERT IGNORE INTO nodes (id, name) VALUES (@node, @name);
