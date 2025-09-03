-- Initializes travel_state for a newly created account
REPLACE INTO travel_state(account_id,current_node,destination_node,start_time,arrival_time,progress_seconds,faster_travel,travel_cost)
VALUES (?, ?, ?, NULL, NULL, 0, 0, 0);
