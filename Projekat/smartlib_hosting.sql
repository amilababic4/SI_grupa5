SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS `AuditLogs`;
DROP TABLE IF EXISTS `Clanarine`;
DROP TABLE IF EXISTS `Rezervacije`;
DROP TABLE IF EXISTS `Zaduzenja`;
DROP TABLE IF EXISTS `Primjerci`;
DROP TABLE IF EXISTS `Korisnici`;
DROP TABLE IF EXISTS `Knjige`;
DROP TABLE IF EXISTS `Uloge`;
DROP TABLE IF EXISTS `Kategorije`;

CREATE TABLE `Uloge` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(50) NOT NULL,
  `Opis` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Uloge_Naziv` (`Naziv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `Uloge` (`Id`, `Naziv`, `Opis`) VALUES
(1, 'Član', 'Član biblioteke'),
(2, 'Bibliotekar', 'Bibliotečko osoblje'),
(3, 'Administrator', 'Sistem administrator');

CREATE TABLE `Kategorije` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(100) NOT NULL,
  `Opis` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Kategorije_Naziv` (`Naziv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `Kategorije` (`Id`, `Naziv`, `Opis`) VALUES
(1, 'Beletristika', 'Romani, pripovijetke i ostala beletristika'),
(2, 'Naučna fantastika', 'SF i fantastična književnost'),
(3, 'Historija', 'Historijska literatura i memoari'),
(4, 'Nauka i tehnika', 'Naučno-stručna literatura'),
(5, 'Filozofija', 'Filozofska i društvena literatura'),
(6, 'Biografija', 'Biografije i autobiografije'),
(7, 'Dječija literatura', 'Knjige za djecu i mlade'),
(8, 'Udžbenici', 'Obrazovni udžbenici i priručnici'),
(9, 'Ostalo', 'Ostale kategorije');

CREATE TABLE `Korisnici` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Ime` varchar(100) NOT NULL,
  `Prezime` varchar(100) NOT NULL,
  `Email` varchar(200) NOT NULL,
  `LozinkaHash` longtext NOT NULL,
  `UlogaId` int NOT NULL,
  `Status` longtext NOT NULL,
  `DatumKreiranja` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Korisnici_Email` (`Email`),
  KEY `IX_Korisnici_UlogaId` (`UlogaId`),
  CONSTRAINT `FK_Korisnici_Uloge_UlogaId`
    FOREIGN KEY (`UlogaId`) REFERENCES `Uloge` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `Korisnici`
(`Id`, `Ime`, `Prezime`, `Email`, `LozinkaHash`, `UlogaId`, `Status`, `DatumKreiranja`) VALUES
(1, 'Admin', 'SmartLib', 'admin@smartlib.ba', 'R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=', 3, 'aktivan', '2026-04-25 09:18:44.000000'),
(2, 'Bibliotekar', 'SmartLib', 'bibliotekar@smartlib.ba', 'R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=', 2, 'aktivan', '2026-04-25 09:18:44.000000'),
(3, 'Test', 'Clan', 'clan@smartlib.ba', 'R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=', 1, 'aktivan', '2026-04-25 09:18:44.000000'),
(4, 'Amila', 'Babic', 'proba@smartlib', 'RXc8HNFmlRF+0H6XsJGvjg==:+Fi3Qst3q84y5t6TNlJd9zltFZgqyYHErKm+CbKRTXY=', 1, 'aktivan', '2026-04-25 20:47:00.000000'),
(5, 'Ime', 'Prezime', 'imeprezime@gmail.com', 'eyWukOsw1/s1sYE7WkYNVg==:fdqafu1WAN62dAwt9lyR/GSSq05y9h7PaTebwSsK4Tc=', 1, 'aktivan', '2026-04-26 15:04:48.000000'),
(6, 'Imran', 'Vlajcic', 'ivlajcic1@etf.unsa.ba', 'v1OSV7uzmOkIb4n89gxqVQ==:ghU7xePMJ4TLW/UIySZEozPekQ+BnAyfoUgUXzU3l0k=', 1, 'aktivan', '2026-04-26 21:54:08.000000');

CREATE TABLE `Knjige` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Naslov` varchar(300) NOT NULL,
  `Autor` varchar(200) NOT NULL,
  `Isbn` varchar(20) NOT NULL,
  `KategorijaId` int NOT NULL,
  `Izdavac` varchar(200) DEFAULT NULL,
  `GodinaIzdanja` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Knjige_Isbn` (`Isbn`),
  KEY `IX_Knjige_KategorijaId` (`KategorijaId`),
  CONSTRAINT `FK_Knjige_Kategorije_KategorijaId`
    FOREIGN KEY (`KategorijaId`) REFERENCES `Kategorije` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Primjerci` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KnjigaId` int NOT NULL,
  `InventarniBroj` varchar(50) NOT NULL,
  `Status` longtext NOT NULL,
  `DatumNabave` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Primjerci_InventarniBroj` (`InventarniBroj`),
  KEY `IX_Primjerci_KnjigaId` (`KnjigaId`),
  CONSTRAINT `FK_Primjerci_Knjige_KnjigaId`
    FOREIGN KEY (`KnjigaId`) REFERENCES `Knjige` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Clanarine` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KorisnikId` int NOT NULL,
  `Status` longtext NOT NULL,
  `DatumPocetka` datetime(6) NOT NULL,
  `DatumIsteka` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Clanarine_KorisnikId` (`KorisnikId`),
  CONSTRAINT `FK_Clanarine_Korisnici_KorisnikId`
    FOREIGN KEY (`KorisnikId`) REFERENCES `Korisnici` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Rezervacije` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KorisnikId` int NOT NULL,
  `KnjigaId` int NOT NULL,
  `DatumRezervacije` datetime(6) NOT NULL,
  `DatumIsteka` datetime(6) NOT NULL,
  `Status` longtext NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Rezervacije_KnjigaId` (`KnjigaId`),
  KEY `IX_Rezervacije_KorisnikId` (`KorisnikId`),
  CONSTRAINT `FK_Rezervacije_Knjige_KnjigaId`
    FOREIGN KEY (`KnjigaId`) REFERENCES `Knjige` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Rezervacije_Korisnici_KorisnikId`
    FOREIGN KEY (`KorisnikId`) REFERENCES `Korisnici` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `AuditLogs` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KorisnikId` int DEFAULT NULL,
  `Akcija` varchar(100) NOT NULL,
  `EntitetTip` varchar(100) NOT NULL,
  `EntitetId` int DEFAULT NULL,
  `VrijednostiPrije` longtext DEFAULT NULL,
  `VrijednostiNakon` longtext DEFAULT NULL,
  `DatumAkcije` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AuditLogs_KorisnikId` (`KorisnikId`),
  CONSTRAINT `FK_AuditLogs_Korisnici_KorisnikId`
    FOREIGN KEY (`KorisnikId`) REFERENCES `Korisnici` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Zaduzenja` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `KorisnikId` int NOT NULL,
  `PrimjerakId` int NOT NULL,
  `DatumZaduzivanja` datetime(6) NOT NULL,
  `DatumPlaniranogVracanja` datetime(6) NOT NULL,
  `DatumStvarnogVracanja` datetime(6) DEFAULT NULL,
  `Status` longtext NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Zaduzenja_KorisnikId` (`KorisnikId`),
  KEY `IX_Zaduzenja_PrimjerakId` (`PrimjerakId`),
  CONSTRAINT `FK_Zaduzenja_Korisnici_KorisnikId`
    FOREIGN KEY (`KorisnikId`) REFERENCES `Korisnici` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Zaduzenja_Primjerci_PrimjerakId`
    FOREIGN KEY (`PrimjerakId`) REFERENCES `Primjerci` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

SET FOREIGN_KEY_CHECKS = 1;
