-- Minimal account table for Wonderland Private Server in MySQL/MariaDB mode.
-- Adjust the table/column names if your Config.settings.wlo DB mapping uses different names.

CREATE DATABASE IF NOT EXISTS wonderland CHARACTER SET utf8 COLLATE utf8_general_ci;
USE wonderland;

CREATE TABLE IF NOT EXISTS `user` (
  `userID` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(14) NOT NULL,
  `password` VARCHAR(255) NOT NULL,
  `character1ID` INT UNSIGNED NOT NULL DEFAULT 0,
  `character2ID` INT UNSIGNED NOT NULL DEFAULT 0,
  `IM` INT NOT NULL DEFAULT 0,
  `char_delete_code` VARCHAR(14) NOT NULL DEFAULT '',
  PRIMARY KEY (`userID`),
  UNIQUE KEY `ux_user_username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Development-only test account. Change or delete this before exposing the server.
-- PassVerification=0 expects the password verifier in RCLibrary to accept this value.
INSERT INTO `user` (`username`, `password`)
VALUES ('testuser', 'testpass')
ON DUPLICATE KEY UPDATE `username` = VALUES(`username`);
