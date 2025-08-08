-- MySQL script to create the 'accounts' database and users table
-- Connect with: mysql -u root -p'' -h localhost -P 3306

CREATE DATABASE IF NOT EXISTS accounts;
USE accounts;

CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    gold INT NOT NULL DEFAULT 0
);
