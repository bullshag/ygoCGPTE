-- Adds a power column to the npcs table for manual power assignment.
USE accounts;

ALTER TABLE npcs
    ADD COLUMN power INT;

DELIMITER $$
CREATE TRIGGER before_insert_npcs_power
BEFORE INSERT ON npcs
FOR EACH ROW
BEGIN
    IF NEW.power IS NULL THEN
        SET NEW.power = 75 * NEW.level;
    END IF;
END$$
DELIMITER ;

UPDATE npcs
SET power = 75 * level
WHERE power IS NULL;
