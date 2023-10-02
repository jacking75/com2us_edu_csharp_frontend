CREATE DATABASE  IF NOT EXISTS `accountdb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `accountdb`;
-- MySQL dump 10.13  Distrib 8.0.33, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: accountdb
-- ------------------------------------------------------
-- Server version	8.0.33

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account`
--

DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account` (
  `AccountId` bigint NOT NULL COMMENT '계정번호',
  `Email` varchar(50) NOT NULL COMMENT '이메일',
  `SaltValue` varchar(100) NOT NULL COMMENT '암호화 값',
  `HashedPassword` varchar(100) NOT NULL COMMENT '해싱된 비밀번호',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '생성 날짜',
  PRIMARY KEY (`AccountId`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='계정 정보';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping routines for database 'accountdb'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-08-02 10:44:43
CREATE DATABASE  IF NOT EXISTS `gamedb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `gamedb`;
-- MySQL dump 10.13  Distrib 8.0.33, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: gamedb
-- ------------------------------------------------------
-- Server version	8.0.33

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `inappreceipt`
--

DROP TABLE IF EXISTS `inappreceipt`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inappreceipt` (
  `PurchaseId` bigint NOT NULL COMMENT '결제 항목 고유 ID',
  `UserId` bigint NOT NULL COMMENT '계정번호',
  `ProductCode` bigint NOT NULL COMMENT '상품 번호',
  `PurchasedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '결제 일시',
  PRIMARY KEY (`PurchaseId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='인앱 결제 정보';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mail_data`
--

DROP TABLE IF EXISTS `mail_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mail_data` (
  `MailId` bigint NOT NULL COMMENT '우편 고유 ID',
  `UserId` bigint NOT NULL COMMENT '계정번호',
  `SenderId` bigint NOT NULL COMMENT '발신자 ID',
  `Title` varchar(100) NOT NULL COMMENT '제목',
  `Content` varchar(2000) NOT NULL COMMENT '내용',
  `IsRead` tinyint(1) NOT NULL DEFAULT '0' COMMENT '읽음 여부',
  `hasItem` tinyint(1) NOT NULL COMMENT '아이템 포함 여부',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0' COMMENT '메일 삭제 여부',
  `ObtainedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '메일 수신 일시',
  `ExpiredAt` datetime NOT NULL COMMENT '메일 만료 일시',
  PRIMARY KEY (`MailId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='우편 정보';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mail_item`
--

DROP TABLE IF EXISTS `mail_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mail_item` (
  `ItemId` bigint NOT NULL COMMENT '아이템 고유 ID',
  `MailId` bigint NOT NULL COMMENT '우편 고유 ID',
  `ItemCode` bigint DEFAULT NULL COMMENT '아이템 코드',
  `ItemCount` int DEFAULT NULL COMMENT '아이템 개수',
  `IsReceived` tinyint(1) NOT NULL DEFAULT '0' COMMENT '아이템 수령 여부',
  PRIMARY KEY (`ItemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='우편 아이템 정보';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_attendance`
--

DROP TABLE IF EXISTS `user_attendance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_attendance` (
  `UserId` bigint NOT NULL COMMENT '유저번호',
  `AttendanceCount` tinyint NOT NULL DEFAULT '0' COMMENT '출석 횟수',
  `LastAttendance` datetime NOT NULL DEFAULT '0001-01-01 00:00:00' COMMENT '마지막 출석 일시',
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='유저 출석 정보';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_basicinformation`
--

DROP TABLE IF EXISTS `user_basicinformation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_basicinformation` (
  `AccountId` bigint NOT NULL COMMENT '계정번호',
  `UserId` bigint NOT NULL AUTO_INCREMENT COMMENT '유저번호',
  `Level` smallint NOT NULL DEFAULT '1' COMMENT '레벨',
  `Exp` bigint NOT NULL DEFAULT '0' COMMENT '경험치',
  `Money` bigint NOT NULL DEFAULT '0' COMMENT '보유 재화',
  `BestClearStage` int NOT NULL DEFAULT '0' COMMENT '클리어한 최고 스테이지',
  `LastLogin` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '마지막 로그인 일시',
  PRIMARY KEY (`AccountId`),
  UNIQUE KEY `UserId` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=1000 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='유저 기본 정보';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_clearstage`
--

DROP TABLE IF EXISTS `user_clearstage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_clearstage` (
  `UserId` bigint NOT NULL COMMENT '계정번호',
  `StageCode` int NOT NULL COMMENT '스테이지 번호',
  `ClearRank` tinyint NOT NULL COMMENT '클리어 랭크',
  `ClearTime` time(3) NOT NULL COMMENT '클리어타임',
  PRIMARY KEY (`UserId`,`StageCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='클리어한 스테이지 정보';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_item`
--

DROP TABLE IF EXISTS `user_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_item` (
  `ItemId` bigint NOT NULL COMMENT '아이템 고유 ID',
  `UserId` bigint NOT NULL COMMENT '계정번호',
  `ItemCode` bigint NOT NULL COMMENT '아이템 번호',
  `ItemCount` int NOT NULL COMMENT '아이템 개수',
  `Attack` int NOT NULL COMMENT '공격력',
  `Defence` int NOT NULL COMMENT '방어력',
  `Magic` int NOT NULL COMMENT '마력',
  `EnhanceCount` tinyint NOT NULL DEFAULT '0' COMMENT '강화 수치',
  `IsDestroyed` tinyint(1) NOT NULL DEFAULT '0',
  `ObtainedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '획득 일시',
  PRIMARY KEY (`ItemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='유저 아이템 정보';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping routines for database 'gamedb'
--
/*!50003 DROP PROCEDURE IF EXISTS `create_dummy_user_basicinformation` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `create_dummy_user_basicinformation`()
BEGIN
	DECLARE i INT DEFAULT 1;
    
    WHILE i < 1000 DO
		INSERT INTO gamedb.user_basicinformation(AccountId, UserId, Level, Exp, Money, BestClearStage, LastLogin)
			VALUES (i, i, FLOOR(RAND() * 100), FLOOR(RAND() * 1000), FLOOR(RAND() * 100000), FLOOR(RAND() * 10), CURRENT_TIMESTAMP - INTERVAL FLOOR(RAND() * 14 * 24 * 60 *60) SECOND);
		SET i = i + 1;
	END WHILE;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `create_dummy_user_item` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `create_dummy_user_item`()
BEGIN
	DECLARE i INT DEFAULT 1;
    
    WHILE i < 10000 DO
		INSERT INTO gamedb.user_item(ItemId, UserId, ItemCode, ItemCount, Attack, Defence, Magic, IsDestroyed)
			VALUES (i, FLOOR(RAND() * 1000), i % 5 + 1, FLOOR(RAND() * 10), FLOOR(RAND() * 100), FLOOR(RAND() * 100), FLOOR(RAND() * 100), false);
		SET i = i + 1;
	END WHILE;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-08-02 10:44:43
CREATE DATABASE  IF NOT EXISTS `masterdb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `masterdb`;
-- MySQL dump 10.13  Distrib 8.0.33, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: masterdb
-- ------------------------------------------------------
-- Server version	8.0.33

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `attendancereward`
--

DROP TABLE IF EXISTS `attendancereward`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `attendancereward` (
  `Code` tinyint NOT NULL COMMENT '날짜',
  `ItemCode` bigint NOT NULL COMMENT '아이템 번호',
  `Count` int NOT NULL COMMENT '개수',
  PRIMARY KEY (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='출석부 보상';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `exptable`
--

DROP TABLE IF EXISTS `exptable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `exptable` (
  `Level` int NOT NULL COMMENT '레벨',
  `RequireExp` bigint NOT NULL COMMENT '해당 레벨에서 다음 레벨까지 필요한 경험치'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='경험치 테이블';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `inappproduct`
--

DROP TABLE IF EXISTS `inappproduct`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inappproduct` (
  `Code` int NOT NULL COMMENT '상품번호',
  `ItemCode` bigint NOT NULL COMMENT '아이템 번호',
  `ItemName` varchar(50) NOT NULL COMMENT '아이템 이름',
  `ItemCount` int NOT NULL COMMENT '아이템 개수',
  PRIMARY KEY (`Code`,`ItemCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='인앱 상품';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `item`
--

DROP TABLE IF EXISTS `item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `item` (
  `Code` bigint NOT NULL COMMENT '아이템 번호',
  `Name` varchar(50) NOT NULL COMMENT '아이템 이름',
  `Attribute` int NOT NULL COMMENT '특성',
  `SellPrice` bigint NOT NULL COMMENT '판매 금액',
  `BuyPrice` bigint NOT NULL COMMENT '구입 금액',
  `UseLv` smallint NOT NULL COMMENT '사용가능 레벨',
  `Attack` int NOT NULL COMMENT '공격력',
  `Defence` int NOT NULL COMMENT '방어력',
  `Magic` int NOT NULL COMMENT '마법력',
  `EnhanceMaxCount` tinyint NOT NULL COMMENT '최대 강화 가능 횟수',
  PRIMARY KEY (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='아이템';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `itemattribute`
--

DROP TABLE IF EXISTS `itemattribute`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `itemattribute` (
  `Name` varchar(50) NOT NULL COMMENT '특성 이름',
  `Code` int NOT NULL COMMENT '코드',
  PRIMARY KEY (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='아이템 특성';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stageenemy`
--

DROP TABLE IF EXISTS `stageenemy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stageenemy` (
  `Code` int NOT NULL COMMENT '스테이지 단계',
  `NpcCode` int NOT NULL COMMENT '공격 Npc',
  `Count` int NOT NULL COMMENT 'Npc 수',
  `Exp` int NOT NULL COMMENT '경험치',
  PRIMARY KEY (`Code`,`NpcCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='스테이지 적';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stageitem`
--

DROP TABLE IF EXISTS `stageitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stageitem` (
  `Code` int NOT NULL COMMENT '스테이지 단계',
  `ItemCode` bigint NOT NULL COMMENT '파밍 가능 아이템 번호',
  `Count` bigint NOT NULL COMMENT '파밍 가능 최대 개수',
  PRIMARY KEY (`Code`,`ItemCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='스테이지 아이템';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `versiondata`
--

DROP TABLE IF EXISTS `versiondata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `versiondata` (
  `AppVersion` decimal(5,4) NOT NULL COMMENT '앱 버전',
  `MasterVersion` decimal(5,4) NOT NULL COMMENT '마스터 버전'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='게임 버전 데이터';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping routines for database 'masterdb'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-08-02 10:44:43
