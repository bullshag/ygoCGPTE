-- Saves equipment for a character and slot
REPLACE INTO character_equipment(account_id,character_name,slot,item_name)
VALUES(@id,@character,@slot,@name);
