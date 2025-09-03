USE accounts;

CREATE TABLE IF NOT EXISTS npc_locations (
    npc_name VARCHAR(255) NOT NULL,
    node_id VARCHAR(50) NOT NULL,
    PRIMARY KEY (npc_name, node_id),
    FOREIGN KEY (npc_name) REFERENCES npcs(name),
    FOREIGN KEY (node_id) REFERENCES nodes(id)
);
