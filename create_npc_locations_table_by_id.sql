USE accounts;

CREATE TABLE IF NOT EXISTS npc_locations (
    npc_id INT NOT NULL,
    node_id VARCHAR(50) NOT NULL,
    PRIMARY KEY (npc_id, node_id),
    FOREIGN KEY (npc_id) REFERENCES npcs(id),
    FOREIGN KEY (node_id) REFERENCES nodes(id)
);
