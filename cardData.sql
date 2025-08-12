-- SQL script to create cardData database and populate it with Yu-Gi-Oh! card information.
-- This script uses the YGOPRODeck API (https://db.ygoprodeck.com/api-guide/) to download
-- card metadata and images. It requires the `lib_mysqludf_sys` plugin so that the
-- `sys_exec` function can run shell commands, as well as the `jq` and `curl` utilities
-- to be installed on the MySQL server host.

-- Create the database if it doesn't already exist
CREATE DATABASE IF NOT EXISTS cardData;
USE cardData;

-- Ensure the cards table exists so we can upsert data
CREATE TABLE IF NOT EXISTS cards (
    id INT PRIMARY KEY,
    name VARCHAR(255),
    type VARCHAR(100),
    description TEXT,
    image LONGBLOB
);

DELIMITER $$

CREATE PROCEDURE populate_cards()
BEGIN
    -- Download card data from the API and format as tab-separated values
    CALL sys_exec('curl https://db.ygoprodeck.com/api/v7/cardinfo.php | jq -r ''.data[] | [(.id|tostring), .name, .type, .desc, .card_images[0].image_url] | @tsv'' > /tmp/cards.tsv');

    DROP TEMPORARY TABLE IF EXISTS tmp_cards;
    CREATE TEMPORARY TABLE tmp_cards (
        id INT,
        name VARCHAR(255),
        type VARCHAR(100),
        description TEXT,
        image_url VARCHAR(512)
    );

    LOAD DATA INFILE '/tmp/cards.tsv'
        INTO TABLE tmp_cards
        FIELDS TERMINATED BY '\t'
        LINES TERMINATED BY '\n';

    DECLARE done INT DEFAULT 0;
    DECLARE v_id INT;
    DECLARE v_name VARCHAR(255);
    DECLARE v_type VARCHAR(100);
    DECLARE v_desc TEXT;
    DECLARE v_image_url VARCHAR(512);

    DECLARE card_cursor CURSOR FOR SELECT id, name, type, description, image_url FROM tmp_cards;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

    OPEN card_cursor;
    read_loop: LOOP
        FETCH card_cursor INTO v_id, v_name, v_type, v_desc, v_image_url;
        IF done THEN
            LEAVE read_loop;
        END IF;
        -- Download each card image and insert the record
        CALL sys_exec(CONCAT('curl -L "', v_image_url, '" -o /tmp/', v_id, '.jpg'));
        INSERT INTO cards(id, name, type, description, image)
            VALUES (v_id, v_name, v_type, v_desc, LOAD_FILE(CONCAT('/tmp/', v_id, '.jpg')))
            ON DUPLICATE KEY UPDATE
                name = VALUES(name),
                type = VALUES(type),
                description = VALUES(description),
                image = VALUES(image);
    END LOOP;
    CLOSE card_cursor;
END$$

DELIMITER ;

CALL populate_cards();
