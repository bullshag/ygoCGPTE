USE accounts;
INSERT INTO abilities (name, description, cost, cooldown) VALUES
('Ice Lance', 'Launch a shard of ice dealing 6 + 110% of your INT damage to a single enemy.', 40, 0),
('Lightning Bolt', 'Call down lightning dealing 4 + 120% of your INT damage to a single foe.', 45, 0),
('Shield Bash', 'Bash a foe for 2 + 50% of your STR damage and stun briefly.', 30, 0),
('Rejuvenate', 'Heal an ally over time for 1 + 60% of your INT every 2s for 6s.', 35, 0),
('Stone Skin', 'Increase an ally\'s defense by 20% for 5s.', 25, 0),
('Arcane Blast', 'Unleash arcane energy dealing 8 + 90% of your INT damage to all enemies.', 60, 0),
('Poison Arrow', 'Fire a toxic shot dealing 2 + 40% of your DEX damage and poisoning for 6s.', 30, 0),
('Cleanse', 'Remove negative effects from an ally.', 20, 0),
('Berserk', 'Increase own damage by 40% for 8s.', 50, 0),
('Drain Life', 'Steal 3 + 70% of your INT HP from an enemy.', 55, 0),
('Guardian Ward', 'Grant a shield to yourself and nearby allies absorbing 4 + 120% of your INT damage for 15s.', 55, 25),
('Divine Aegis', 'Place a large barrier on an ally absorbing 8 + 250% of your INT damage for 15s.', 45, 20);
