-- MySQL script to create passive ability tables and seed data
USE accounts;

CREATE TABLE IF NOT EXISTS passives (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT
);

CREATE TABLE IF NOT EXISTS character_passives (
    character_id INT NOT NULL,
    passive_id INT NOT NULL,
    level INT NOT NULL DEFAULT 1,
    PRIMARY KEY(character_id, passive_id),
    FOREIGN KEY (character_id) REFERENCES characters(id),
    FOREIGN KEY (passive_id) REFERENCES passives(id)
);

INSERT INTO passives (name, description) VALUES
('Parry', '5% +1% per 30 STR or DEX chance to parry incoming attacks.'),
('Nimble', '1% per 10 DEX chance to dodge attacks.'),
('Flesh Rip', '5% +1% per 15 STR chance to inflict a bleed dealing 10% +1% per 15 STR of weapon damage.'),
('Deadly Strikes', 'Crit chance increased by 1% for every 10 DEX.'),
('Battle Mage', 'Leech mana equal to 15% of INT on weapon hit.'),
('Bloodlust', 'Take 75% more damage and deal +2% weapon damage. Missing HP reduces damage taken and increases bonus.'),
('Pacifist', 'Deal 50% less damage but heal 50% more.'),
('Cleaving Strikes', 'Basic attacks splash 20% damage to a nearby enemy.'),
('Iron Wall', 'Start battle with 100% reduced damage taken; each hit reduces this by 10%.'),
('Mana Conduit', 'Healing others restores their mana by 15% of the amount healed.'),
('Thornmail', 'When struck, deal 10% of damage back to the attacker.'),
('Vampiric Strikes', 'Heal for 10% of the damage you deal.'),
('Berserker', 'Deal 25% more damage but take 15% more.'),
('Bulwark', '+20% melee defense but 10% slower attacks.'),
('Arcane Mastery', '+20% spell damage.'),
('Fleet Footed', 'Attack 10% faster.'),
('Regenerative', 'Receive 10% more healing.'),
('Mana Efficiency', 'Abilities cost 20% less mana.'),
('Quick Recovery', 'Ability cooldowns reduced by 20%.'),
('Steadfast', 'Immune to critical hits.'),
('Guardian\'s Grace', 'Healing others also grants them a shield for 10% of the amount healed.'),
('Protective Barrier', 'Begin battle with a shield equal to 20% of max HP.'),
('Momentum', 'Each attack increases damage by 5%, up to 25%. Taking damage resets the bonus.'),
('Firebrand', 'Basic attacks deal +5 +20% INT additional fire damage.'),
('Poison Mastery', 'Poisons you apply last 50% longer.'),
('Mana Shield', '50% of incoming damage is absorbed by mana (1 mana per damage).'),
('Second Wind', 'The first time you drop below 30% HP each battle, heal 25% of max HP.'),
('Nature\'s Grace', 'Healing over time effects you cast tick 50% faster.'),
('Spell Deflection', '10% chance to completely avoid spell damage.'),
('Rejuvenating Healer', 'Healing others also heals you for 20% of the amount.'),
('Arcane Siphon', 'Dealing spell damage restores mana equal to 5% of the damage done.'),
('Plate Mastery', 'Increases plate armor effectiveness by 50%.'),
('Cloth Mastery', 'Increases cloth armor effectiveness by 50%.'),
('Leather Mastery', 'Increases leather armor effectiveness by 50%.');
