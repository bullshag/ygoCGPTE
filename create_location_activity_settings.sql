USE accounts;

CREATE TABLE IF NOT EXISTS location_activity_settings (
    id INT AUTO_INCREMENT PRIMARY KEY,
    location_id VARCHAR(50) NOT NULL,
    activity_key VARCHAR(32) NOT NULL,
    is_enabled TINYINT(1) NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY uq_location_activity (location_id, activity_key),
    CONSTRAINT fk_location_activity_node FOREIGN KEY (location_id) REFERENCES nodes(id),
    CONSTRAINT chk_location_activity_key CHECK (activity_key IN (
        'tavern',
        'shop',
        'temple',
        'academy',
        'graveyard',
        'arena',
        'search_for_enemies'
    ))
);

-- Use INSERT ... ON DUPLICATE KEY UPDATE to maintain availability flags, for example:
-- INSERT INTO location_activity_settings (location_id, activity_key, is_enabled)
-- VALUES ('nodeRiverVillage', 'tavern', 1)
-- ON DUPLICATE KEY UPDATE is_enabled = VALUES(is_enabled);
