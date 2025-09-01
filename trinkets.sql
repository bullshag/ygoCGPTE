-- SQL script adding trinkets and their drop chances
USE accounts;

CREATE TABLE IF NOT EXISTS trinkets (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    description TEXT NOT NULL,
    effect_json JSON NOT NULL,
    drop_chance DECIMAL(4,3) NOT NULL
);

INSERT INTO trinkets (name, description, effect_json, drop_chance) VALUES
('Bulwark Charm I','While equipped, max HP +10% and damage dealt -15%.',JSON_OBJECT('max_hp_pct',10,'damage_dealt_pct',-15),0.05),
('Bulwark Charm II','While equipped, max HP +20% and damage dealt -15%.',JSON_OBJECT('max_hp_pct',20,'damage_dealt_pct',-15),0.03),
('Bulwark Charm III','While equipped, max HP +30% and damage dealt -15%.',JSON_OBJECT('max_hp_pct',30,'damage_dealt_pct',-15),0.01),
('Mystic Reservoir I','While equipped, max mana +10% and damage dealt -15%.',JSON_OBJECT('max_mana_pct',10,'damage_dealt_pct',-15),0.05),
('Mystic Reservoir II','While equipped, max mana +20% and damage dealt -15%.',JSON_OBJECT('max_mana_pct',20,'damage_dealt_pct',-15),0.03),
('Mystic Reservoir III','While equipped, max mana +30% and damage dealt -15%.',JSON_OBJECT('max_mana_pct',30,'damage_dealt_pct',-15),0.01),
('Arcane Focus Charm I','Ability damage +1% per 5 points of lowest stat.',JSON_OBJECT('ability_damage_per5_lowest_stat_pct',1),0.05),
('Arcane Focus Charm II','Ability damage +2% per 5 points of lowest stat.',JSON_OBJECT('ability_damage_per5_lowest_stat_pct',2),0.03),
('Arcane Focus Charm III','Ability damage +3% per 5 points of lowest stat.',JSON_OBJECT('ability_damage_per5_lowest_stat_pct',3),0.01),
('Battleborn Charm I','Auto-attack damage +5% per 5 points of lowest stat.',JSON_OBJECT('auto_attack_damage_per5_lowest_stat_pct',5),0.05),
('Battleborn Charm II','Auto-attack damage +10% per 5 points of lowest stat.',JSON_OBJECT('auto_attack_damage_per5_lowest_stat_pct',10),0.03),
('Battleborn Charm III','Auto-attack damage +15% per 5 points of lowest stat.',JSON_OBJECT('auto_attack_damage_per5_lowest_stat_pct',15),0.01),
('Gilded Medal','All experience gained is converted to gold.',JSON_OBJECT('convert_exp_to_gold',true),0.05),
('Phoenix Feather','On fatal damage, heal to full HP and mana, then destroy this trinket.',JSON_OBJECT('cheat_death',true),0.01),
('Rejuvenating Idol','At combat start, restore 10% max HP and mana every 3s.',JSON_OBJECT('combat_regen_pct',10,'combat_regen_interval_sec',3),0.03),
('Mana Barrier Stone','While mana >75%, damage taken is subtracted from mana before HP.',JSON_OBJECT('mana_shield_threshold_pct',75),0.03),
('Thorned Vengeance Charm','Attackers using basic attacks take damage equal to your lowest stat.',JSON_OBJECT('retaliate_lowest_stat',true),0.05),
('Precision Matrix I','Crit chance +10% and crit damage +10%.',JSON_OBJECT('crit_chance_pct',10,'crit_damage_pct',10),0.05),
('Precision Matrix II','Crit chance +20% and crit damage +20%.',JSON_OBJECT('crit_chance_pct',20,'crit_damage_pct',20),0.03),
('Precision Matrix III','Crit chance +30% and crit damage +30%.',JSON_OBJECT('crit_chance_pct',30,'crit_damage_pct',30),0.01),
('Rolemaster\'s Crest','Bonuses depend on role: +10% health and defenses if tank, +10% damage if dps, +10% healing if healer.',JSON_OBJECT('tank_health_defense_pct',10,'dps_damage_pct',10,'healer_healing_pct',10),0.03),
('Mana Reaper Seal','Damage +50%, but hits drain enemy mana before health.',JSON_OBJECT('damage_pct',50,'drain_mana_first',true),0.03),
('Temporal Loop Charm I','Damage and healing -15%, but action speed +10%.',JSON_OBJECT('damage_healing_pct',-15,'action_speed_pct',10),0.05),
('Temporal Loop Charm II','Damage and healing -15%, but action speed +20%.',JSON_OBJECT('damage_healing_pct',-15,'action_speed_pct',20),0.03),
('Temporal Loop Charm III','Damage and healing -15%, but action speed +30%.',JSON_OBJECT('damage_healing_pct',-15,'action_speed_pct',30),0.01);
