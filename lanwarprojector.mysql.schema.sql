SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

CREATE SCHEMA IF NOT EXISTS `lanwarprojector` DEFAULT CHARACTER SET utf8 ;
USE `lanwarprojector` ;

-- -----------------------------------------------------
-- Table `lanwarprojector`.`connectioninfo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `lanwarprojector`.`connectioninfo` (
  `Id` VARCHAR(40) NOT NULL,
  `Name` VARCHAR(255) NULL DEFAULT NULL,
  `Path` VARCHAR(255) NULL DEFAULT NULL,
  `Status` VARCHAR(255) NULL DEFAULT NULL,
  `LastindexTime` DATETIME NULL DEFAULT NULL,
  `Password` LONGBLOB NULL DEFAULT NULL,
  `Username` VARCHAR(255) NULL DEFAULT NULL,
  `Statustime` DATETIME NULL DEFAULT NULL,
  `Queueable` BIT(1) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `lanwarprojector`.`mediaindex`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `lanwarprojector`.`mediaindex` (
  `Id` VARCHAR(40) NOT NULL,
  `ConnectionId` VARCHAR(40) NOT NULL,
  `Path` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `ConnectionId` (`ConnectionId` ASC),
  CONSTRAINT `FK908D88B2C4699909`
    FOREIGN KEY (`ConnectionId`)
    REFERENCES `lanwarprojector`.`connectioninfo` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `lanwarprojector`.`videos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `lanwarprojector`.`videos` (
  `Id` VARCHAR(40) NOT NULL,
  `Title` VARCHAR(255) NULL DEFAULT NULL,
  `Rank` SMALLINT(6) NULL DEFAULT NULL,
  `Path` VARCHAR(255) NULL DEFAULT NULL,
  `WatchTime` DATETIME NULL DEFAULT NULL,
  `Url` VARCHAR(255) NULL DEFAULT NULL,
  `AddTime` DATETIME NULL DEFAULT NULL,
  `Message` VARCHAR(255) NULL DEFAULT NULL,
  `Status` INT(11) NULL DEFAULT NULL,
  `VideoType` INT(11) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `lanwarprojector`.`votes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `lanwarprojector`.`votes` (
  `Id` VARCHAR(40) NOT NULL,
  `VideoId` VARCHAR(40) NOT NULL,
  `IpAddress` VARCHAR(255) NULL DEFAULT NULL,
  `VoteValue` SMALLINT(6) NULL DEFAULT NULL,
  `VoteTime` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `VideoId` (`VideoId` ASC),
  CONSTRAINT `FK5F7A353FF5131651`
    FOREIGN KEY (`VideoId`)
    REFERENCES `lanwarprojector`.`videos` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
