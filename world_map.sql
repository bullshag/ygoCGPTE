CREATE TABLE IF NOT EXISTS nodes (
    id VARCHAR(50) PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS node_connections (
    from_node VARCHAR(50) NOT NULL,
    to_node VARCHAR(50) NOT NULL,
    travel_time_days INT NOT NULL,
    PRIMARY KEY (from_node, to_node),
    FOREIGN KEY (from_node) REFERENCES nodes(id),
    FOREIGN KEY (to_node) REFERENCES nodes(id)
);

CREATE TABLE IF NOT EXISTS activities (
    id INT AUTO_INCREMENT PRIMARY KEY,
    node_id VARCHAR(50) NOT NULL,
    activity_type VARCHAR(50),
    description VARCHAR(255),
    duration_seconds INT,
    FOREIGN KEY (node_id) REFERENCES nodes(id)
);

CREATE TABLE IF NOT EXISTS travel_state (
    account_id INT PRIMARY KEY,
    current_node VARCHAR(50) NOT NULL,
    destination_node VARCHAR(50) NOT NULL,
    start_time DATETIME,
    arrival_time DATETIME,
    progress_seconds INT DEFAULT 0,
    faster_travel TINYINT(1) DEFAULT 0,
    travel_cost INT DEFAULT 0,
    FOREIGN KEY (current_node) REFERENCES nodes(id),
    FOREIGN KEY (destination_node) REFERENCES nodes(id)
);

CREATE TABLE IF NOT EXISTS travel_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id INT,
    from_node VARCHAR(50),
    to_node VARCHAR(50),
    start_time DATETIME,
    end_time DATETIME,
    original_days INT,
    travel_days INT,
    cost_gold INT,
    faster_travel_applied TINYINT(1),
    FOREIGN KEY (from_node) REFERENCES nodes(id),
    FOREIGN KEY (to_node) REFERENCES nodes(id)
);

CREATE TABLE IF NOT EXISTS notifications (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id INT,
    created_at DATETIME,
    message TEXT
);

CREATE TABLE IF NOT EXISTS quests (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id INT,
    description TEXT,
    progress INT DEFAULT 0,
    target INT DEFAULT 0,
    completed TINYINT(1) DEFAULT 0
);

CREATE TABLE IF NOT EXISTS dark_spire_state (
    account_id INT PRIMARY KEY,
    current_min INT NOT NULL,
    current_max INT NOT NULL,
    FOREIGN KEY (account_id) REFERENCES users(id)
);
