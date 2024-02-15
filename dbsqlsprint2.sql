CREATE DATABASE  IF NOT EXISTS `quizdb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `quizdb`;
-- MySQL dump 10.13  Distrib 8.0.33, for Win64 (x86_64)
--
-- Host: localhost    Database: quizdb
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
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `category` (
  `category_id` int NOT NULL AUTO_INCREMENT,
  `category_name` varchar(255) NOT NULL,
  `category_is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`category_id`),
  UNIQUE KEY `category_name` (`category_name`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `category_quiz`
--

DROP TABLE IF EXISTS `category_quiz`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `category_quiz` (
  `category_quiz_id` int NOT NULL AUTO_INCREMENT,
  `category_id` int DEFAULT NULL,
  `quiz_id` int DEFAULT NULL,
  PRIMARY KEY (`category_quiz_id`),
  KEY `category_id` (`category_id`),
  KEY `quiz_id` (`quiz_id`),
  CONSTRAINT `category_quiz_ibfk_1` FOREIGN KEY (`category_id`) REFERENCES `category` (`category_id`),
  CONSTRAINT `category_quiz_ibfk_2` FOREIGN KEY (`quiz_id`) REFERENCES `quiz` (`quiz_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `option`
--

DROP TABLE IF EXISTS `option`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `option` (
  `option_id` int NOT NULL AUTO_INCREMENT,
  `option_text` text NOT NULL,
  `option_is_correct` tinyint(1) NOT NULL,
  PRIMARY KEY (`option_id`)
) ENGINE=InnoDB AUTO_INCREMENT=110 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `question`
--

DROP TABLE IF EXISTS `question`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `question` (
  `question_id` int NOT NULL AUTO_INCREMENT,
  `question_text` text NOT NULL,
  `question_feedback` text,
  `question_level` smallint NOT NULL,
  `question_is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`question_id`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `question_option`
--

DROP TABLE IF EXISTS `question_option`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `question_option` (
  `question_option_id` int NOT NULL AUTO_INCREMENT,
  `question_id` int DEFAULT NULL,
  `option_id` int DEFAULT NULL,
  PRIMARY KEY (`question_option_id`),
  KEY `idx_question_id` (`question_id`),
  KEY `idx_option_id` (`option_id`),
  CONSTRAINT `question_option_ibfk_1` FOREIGN KEY (`question_id`) REFERENCES `question` (`question_id`),
  CONSTRAINT `question_option_ibfk_2` FOREIGN KEY (`option_id`) REFERENCES `option` (`option_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1288 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quiz`
--

DROP TABLE IF EXISTS `quiz`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `quiz` (
  `quiz_id` int NOT NULL AUTO_INCREMENT,
  `quiz_name` varchar(255) NOT NULL,
  `quiz_is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`quiz_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

DROP TABLE IF EXISTS `quiz_img`;
CREATE TABLE `quiz_img` (
  `img_id` int NOT NULL AUTO_INCREMENT,
  `quiz_id` int NOT NULL,
  `img` varchar(255) NOT NULL,
  PRIMARY KEY (`img_id`),
  KEY `fk_quiz_id` (`quiz_id`),
  CONSTRAINT `fk_quiz` FOREIGN KEY (`quiz_id`) REFERENCES `quiz` (`quiz_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


--
-- Table structure for table `quiz_issue`
--

DROP TABLE IF EXISTS `quiz_issue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `quiz_issue` (
  `quiz_issue_id` int NOT NULL AUTO_INCREMENT,
  `quiz_issue_description` text NOT NULL,
  `quiz_issue_date_reported` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `quiz_issue_is_fixed` tinyint(1) NOT NULL,
  PRIMARY KEY (`quiz_issue_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quiz_question`
--

DROP TABLE IF EXISTS `quiz_question`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `quiz_question` (
  `quiz_question_id` int NOT NULL AUTO_INCREMENT,
  `quiz_id` int DEFAULT NULL,
  `question_id` int DEFAULT NULL,
  PRIMARY KEY (`quiz_question_id`),
  KEY `idx_quiz_id` (`quiz_id`),
  KEY `idx_question_id` (`question_id`),
  CONSTRAINT `quiz_question_ibfk_1` FOREIGN KEY (`quiz_id`) REFERENCES `quiz` (`quiz_id`),
  CONSTRAINT `quiz_question_ibfk_2` FOREIGN KEY (`question_id`) REFERENCES `question` (`question_id`)
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quiz_session`
--

DROP TABLE IF EXISTS `quiz_session`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `quiz_session` (
  `quiz_session_id` int NOT NULL AUTO_INCREMENT,
  `quiz_session_score` int NOT NULL,
  `quiz_session_time` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`quiz_session_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quiz_tracker`
--

DROP TABLE IF EXISTS `quiz_tracker`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `quiz_tracker` (
  `quiz_tracker_id` int NOT NULL AUTO_INCREMENT,
  `user_quiz_id` int DEFAULT NULL,
  `quiz_session_id` int DEFAULT NULL,
  `quiz_id` int DEFAULT NULL,
  PRIMARY KEY (`quiz_tracker_id`),
  KEY `idx_user_quiz_id` (`user_quiz_id`),
  KEY `idx_quiz_session_id` (`quiz_session_id`),
  KEY `idx_quiz_id` (`quiz_id`),
  CONSTRAINT `quiz_tracker_ibfk_1` FOREIGN KEY (`user_quiz_id`) REFERENCES `user_quiz` (`user_quiz_id`),
  CONSTRAINT `quiz_tracker_ibfk_2` FOREIGN KEY (`quiz_session_id`) REFERENCES `quiz_session` (`quiz_session_id`),
  CONSTRAINT `quiz_tracker_ibfk_3` FOREIGN KEY (`quiz_id`) REFERENCES `quiz` (`quiz_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_issue`
--

DROP TABLE IF EXISTS `user_issue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_issue` (
  `user_issue_id` int NOT NULL AUTO_INCREMENT,
  `quiz_issue_id` int DEFAULT NULL,
  `user_quiz_id` int DEFAULT NULL,
  PRIMARY KEY (`user_issue_id`),
  KEY `quiz_issue_id` (`quiz_issue_id`),
  KEY `user_quiz_id` (`user_quiz_id`),
  CONSTRAINT `user_issue_ibfk_1` FOREIGN KEY (`quiz_issue_id`) REFERENCES `quiz_issue` (`quiz_issue_id`),
  CONSTRAINT `user_issue_ibfk_2` FOREIGN KEY (`user_quiz_id`) REFERENCES `user_quiz` (`user_quiz_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_quiz`
--

DROP TABLE IF EXISTS `user_quiz`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_quiz` (
  `user_quiz_id` int NOT NULL AUTO_INCREMENT,
  `user_quiz_username` varchar(255) NOT NULL,
  `user_quiz_password` varchar(64) NOT NULL,
  `user_quiz_email` varchar(255) NOT NULL,
  `user_quiz_is_admin` tinyint(1) NOT NULL,
  `user_quiz_is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`user_quiz_id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

INSERT INTO `user_quiz` 
(`user_quiz_username`, `user_quiz_password`, `user_quiz_email`, `user_quiz_is_admin`, `user_quiz_is_active`) 
VALUES 
('user', '04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb', 'user@user.com', 0, 1);

INSERT INTO `user_quiz` 
(`user_quiz_username`, `user_quiz_password`, `user_quiz_email`, `user_quiz_is_admin`, `user_quiz_is_active`) 
VALUES 
('admin', '04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb', 'admin@admin.com', 1, 1);

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-02-09  8:36:55
