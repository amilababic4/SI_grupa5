-- MySQL dump 10.13  Distrib 8.0.46, for Linux (x86_64)
--
-- Host: localhost    Database: smartlib
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `AuditLogs`
--

DROP TABLE IF EXISTS `AuditLogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AuditLogs` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KorisnikId` int DEFAULT NULL,
  `Akcija` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `EntitetTip` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `EntitetId` int DEFAULT NULL,
  `VrijednostiPrije` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `VrijednostiNakon` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `DatumAkcije` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AuditLogs_KorisnikId` (`KorisnikId`),
  CONSTRAINT `FK_AuditLogs_Korisnici_KorisnikId` FOREIGN KEY (`KorisnikId`) REFERENCES `Korisnici` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `AuditLogs`
--

LOCK TABLES `AuditLogs` WRITE;
/*!40000 ALTER TABLE `AuditLogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `AuditLogs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Clanarine`
--

DROP TABLE IF EXISTS `Clanarine`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Clanarine` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KorisnikId` int NOT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT (_utf8mb4'aktivna'),
  `DatumPocetka` datetime(6) NOT NULL,
  `DatumIsteka` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Clanarine_KorisnikId` (`KorisnikId`),
  CONSTRAINT `FK_Clanarine_Korisnici_KorisnikId` FOREIGN KEY (`KorisnikId`) REFERENCES `Korisnici` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Clanarine`
--

LOCK TABLES `Clanarine` WRITE;
/*!40000 ALTER TABLE `Clanarine` DISABLE KEYS */;
/*!40000 ALTER TABLE `Clanarine` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Kategorije`
--

DROP TABLE IF EXISTS `Kategorije`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Kategorije` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Opis` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Kategorije_Naziv` (`Naziv`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Kategorije`
--

LOCK TABLES `Kategorije` WRITE;
/*!40000 ALTER TABLE `Kategorije` DISABLE KEYS */;
INSERT INTO `Kategorije` VALUES (1,'Beletristika','Romani, pripovijetke i ostala beletristika'),(2,'Naučna fantastika','SF i fantastična književnost'),(3,'Historija','Historijska literatura i memoari'),(4,'Nauka i tehnika','Naučno-stručna literatura'),(5,'Filozofija','Filozofska i društvena literatura'),(6,'Biografija','Biografije i autobiografije'),(7,'Dječija literatura','Knjige za djecu i mlade'),(8,'Udžbenici','Obrazovni udžbenici i priručnici'),(9,'Ostalo','Ostale kategorije');
/*!40000 ALTER TABLE `Kategorije` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Knjige`
--

DROP TABLE IF EXISTS `Knjige`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Knjige` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Naslov` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Autor` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Isbn` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `KategorijaId` int NOT NULL,
  `Izdavac` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GodinaIzdanja` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Knjige_Isbn` (`Isbn`),
  KEY `IX_Knjige_KategorijaId` (`KategorijaId`),
  CONSTRAINT `FK_Knjige_Kategorije_KategorijaId` FOREIGN KEY (`KategorijaId`) REFERENCES `Kategorije` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Knjige`
--

LOCK TABLES `Knjige` WRITE;
/*!40000 ALTER TABLE `Knjige` DISABLE KEYS */;
/*!40000 ALTER TABLE `Knjige` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Korisnici`
--

DROP TABLE IF EXISTS `Korisnici`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Korisnici` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Ime` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Prezime` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Email` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `LozinkaHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `UlogaId` int NOT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT (_utf8mb4'aktivan'),
  `DatumKreiranja` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Korisnici_Email` (`Email`),
  KEY `IX_Korisnici_UlogaId` (`UlogaId`),
  CONSTRAINT `FK_Korisnici_Uloge_UlogaId` FOREIGN KEY (`UlogaId`) REFERENCES `Uloge` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Korisnici`
--

LOCK TABLES `Korisnici` WRITE;
/*!40000 ALTER TABLE `Korisnici` DISABLE KEYS */;
INSERT INTO `Korisnici` VALUES (1,'Admin','SmartLib','admin@smartlib.ba','R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=',3,'aktivan','2026-04-25 09:18:44.000000'),(2,'Bibliotekar','SmartLib','bibliotekar@smartlib.ba','R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=',2,'aktivan','2026-04-25 09:18:44.000000'),(3,'Test','Clan','clan@smartlib.ba','R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=',1,'aktivan','2026-04-25 09:18:44.000000'),(4,'Amila','Babic','proba@smartlib','RXc8HNFmlRF+0H6XsJGvjg==:+Fi3Qst3q84y5t6TNlJd9zltFZgqyYHErKm+CbKRTXY=',1,'aktivan','2026-04-25 20:47:00.000000'),(5,'Ime','Prezime','imeprezime@gmail.com','eyWukOsw1/s1sYE7WkYNVg==:fdqafu1WAN62dAwt9lyR/GSSq05y9h7PaTebwSsK4Tc=',1,'aktivan','2026-04-26 15:04:48.000000'),(6,'Imran','Vlajcic','ivlajcic1@etf.unsa.ba','v1OSV7uzmOkIb4n89gxqVQ==:ghU7xePMJ4TLW/UIySZEozPekQ+BnAyfoUgUXzU3l0k=',1,'aktivan','2026-04-26 21:54:08.000000');
/*!40000 ALTER TABLE `Korisnici` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Primjerci`
--

DROP TABLE IF EXISTS `Primjerci`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Primjerci` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KnjigaId` int NOT NULL,
  `InventarniBroj` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT (_utf8mb4'dostupan'),
  `DatumNabave` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Primjerci_InventarniBroj` (`InventarniBroj`),
  KEY `IX_Primjerci_KnjigaId` (`KnjigaId`),
  CONSTRAINT `FK_Primjerci_Knjige_KnjigaId` FOREIGN KEY (`KnjigaId`) REFERENCES `Knjige` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Primjerci`
--

LOCK TABLES `Primjerci` WRITE;
/*!40000 ALTER TABLE `Primjerci` DISABLE KEYS */;
/*!40000 ALTER TABLE `Primjerci` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Rezervacije`
--

DROP TABLE IF EXISTS `Rezervacije`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Rezervacije` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KorisnikId` int NOT NULL,
  `KnjigaId` int NOT NULL,
  `DatumRezervacije` datetime(6) NOT NULL,
  `DatumIsteka` datetime(6) NOT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT (_utf8mb4'aktivna'),
  PRIMARY KEY (`Id`),
  KEY `IX_Rezervacije_KnjigaId` (`KnjigaId`),
  KEY `IX_Rezervacije_KorisnikId` (`KorisnikId`),
  CONSTRAINT `FK_Rezervacije_Knjige_KnjigaId` FOREIGN KEY (`KnjigaId`) REFERENCES `Knjige` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Rezervacije_Korisnici_KorisnikId` FOREIGN KEY (`KorisnikId`) REFERENCES `Korisnici` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Rezervacije`
--

LOCK TABLES `Rezervacije` WRITE;
/*!40000 ALTER TABLE `Rezervacije` DISABLE KEYS */;
/*!40000 ALTER TABLE `Rezervacije` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Uloge`
--

DROP TABLE IF EXISTS `Uloge`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Uloge` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Opis` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Uloge_Naziv` (`Naziv`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Uloge`
--

LOCK TABLES `Uloge` WRITE;
/*!40000 ALTER TABLE `Uloge` DISABLE KEYS */;
INSERT INTO `Uloge` VALUES (1,'Član','Član biblioteke'),(2,'Bibliotekar','Bibliotečko osoblje'),(3,'Administrator','Sistem administrator');
/*!40000 ALTER TABLE `Uloge` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Zaduzenja`
--

DROP TABLE IF EXISTS `Zaduzenja`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Zaduzenja` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KorisnikId` int NOT NULL,
  `PrimjerakId` int NOT NULL,
  `DatumZaduzivanja` datetime(6) NOT NULL,
  `DatumPlaniranogVracanja` datetime(6) NOT NULL,
  `DatumStvarnogVracanja` datetime(6) DEFAULT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT (_utf8mb4'aktivno'),
  PRIMARY KEY (`Id`),
  KEY `IX_Zaduzenja_KorisnikId` (`KorisnikId`),
  KEY `IX_Zaduzenja_PrimjerakId` (`PrimjerakId`),
  CONSTRAINT `FK_Zaduzenja_Korisnici_KorisnikId` FOREIGN KEY (`KorisnikId`) REFERENCES `Korisnici` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Zaduzenja_Primjerci_PrimjerakId` FOREIGN KEY (`PrimjerakId`) REFERENCES `Primjerci` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Zaduzenja`
--

LOCK TABLES `Zaduzenja` WRITE;
/*!40000 ALTER TABLE `Zaduzenja` DISABLE KEYS */;
/*!40000 ALTER TABLE `Zaduzenja` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-30 10:25:29
