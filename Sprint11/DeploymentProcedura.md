# SmartLib — Deployment Procedura

## Sadržaj

1. [Naziv aplikacije i opis arhitekture](#1-naziv-aplikacije-i-opis-arhitekture)
2. [Tehnologije koje se koriste](#2-tehnologije-koje-se-koriste)
3. [Potrebni alati i verzije](#3-potrebni-alati-i-verzije)
4. [Environment varijable](#4-environment-varijable)
5. [Upute za lokalno pokretanje backend-a](#5-upute-za-lokalno-pokretanje-backend-a)
6. [Upute za lokalno pokretanje frontend-a](#6-upute-za-lokalno-pokretanje-frontend-a)
7. [Upute za pokretanje baze podataka](#7-upute-za-pokretanje-baze-podataka)
8. [Migracije i seed podaci](#8-migracije-i-seed-podaci)
9. [Pokretanje testova](#9-pokretanje-testova)
10. [Produkcijski / Cloud deployment](#10-produkcijski--cloud-deployment)
11. [Link na deployment](#11-link-na-deployment)
12. [Poznata ograničenja deploymenta](#12-poznata-ograničenja-deploymenta)
13. [Najčešći problemi i rješenja](#13-najčešći-problemi-i-rješenja)

---

## 1. Naziv aplikacije i opis arhitekture

**Naziv:** SmartLib — Bibliotečki Informacioni Sistem

**Kratak opis:** SmartLib je web aplikacija za upravljanje bibliotečkim fondom. Sistem omogućava bibliotečkom osoblju centralizovano upravljanje knjigama, članovima i zaduženjima, dok članovima pruža uvid u dostupnost literature i status članarine. Aplikacija podržava forum, recenzije, notifikacije, nabavku knjiga, listu želja, kolekcije i generisanje PDF izvještaja.

### Arhitektura

Aplikacija je izgrađena po **Clean Architecture** principu sa jasnim razdvajanjem odgovornosti u četiri sloja:

```
Projekat/
├── src/
│   ├── SmartLib.Web/              # Prezentacijski sloj (ASP.NET Core MVC + Razor Views)
│   ├── SmartLib.API/              # REST API sloj (JWT autentifikacija)
│   ├── SmartLib.Core/             # Domenski sloj (modeli, interfejsi, DTO-ovi)
│   └── SmartLib.Infrastructure/   # Infrastrukturni sloj (EF Core, repozitoriji, servisi)
├── tests/
│   └── SmartLib.Tests/            # Unit, integracioni, UI i sigurnosni testovi
├── docker-compose.yml             # Docker orkestacija (MySQL + Web)
├── .env                           # Environment varijable (lokalni razvoj)
└── SmartLib.sln                   # Visual Studio solution
```

**Dijagram arhitekture:**

```
┌─────────────────────────────────────────────────────────┐
│                     Klijent (Browser)                    │
└──────────────────────────┬──────────────────────────────┘
                           │ HTTP/HTTPS
┌──────────────────────────▼──────────────────────────────┐
│               SmartLib.Web (MVC + Razor)                 │
│         Port: 5000 (Docker) / 5xxx (lokalno)             │
│     Cookie autentifikacija, Session management           │
├──────────────────────────────────────────────────────────┤
│               SmartLib.API (REST, opciono)                │
│              JWT Bearer autentifikacija                   │
├──────────────────────────────────────────────────────────┤
│             SmartLib.Infrastructure                       │
│      EF Core, Repozitoriji, Brevo Email, Redis cache     │
├──────────────────────────────────────────────────────────┤
│                SmartLib.Core                              │
│          Modeli, Interfejsi, DTO-ovi                     │
└──────────────────────────┬──────────────────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        ▼                  ▼                  ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────────┐
│  MySQL 8.0   │  │ Upstash Redis│  │ Brevo API        │
│  (TiDB Cloud)│  │ (Cache)      │  │ (Email servis)   │
└──────────────┘  └──────────────┘  └──────────────────┘
```

### Kako email servis funkcioniše

`EmailService.cs` koristi **tri strategije** za slanje emailova, s prioritetom:

1. **Brevo HTTP API** (primarno, koristi se u produkciji na Render-u) — šalje putem `https://api.brevo.com/v3/smtp/email`, ne zavisi od SMTP portova
2. **SMTP** (fallback za lokalni razvoj) — koristi Gmail SMTP ili Mailtrap, radi lokalno ali **ne radi na Render-u** jer Render blokira odlazne SMTP portove
3. **Log fallback** (debug) — ako nijedna metoda ne uspije, email sadržaj se ispisuje u logove

---

## 2. Tehnologije koje se koriste

| Kategorija              | Tehnologija                                | Verzija       |
|-------------------------|--------------------------------------------|---------------|
| **Programski jezik**     | C#                                         | 12            |
| **Framework**            | ASP.NET Core MVC                           | 8.0           |
| **Target Framework**     | .NET                                       | 8.0           |
| **Frontend**             | Razor Views (.cshtml) + Tag Helpers        | -             |
| **ORM**                  | Entity Framework Core                      | 8.0.*         |
| **DB Provider**          | Pomelo.EntityFrameworkCore.MySql            | 8.0.*         |
| **Baza podataka**        | MySQL (TiDB Cloud u produkciji)            | 8.0           |
| **Cache (produkcija)**   | Upstash Redis (StackExchange.Redis)        | -             |
| **Cache (lokalno)**      | In-Memory Distributed Cache (fallback)     | -             |
| **Autentifikacija (Web)**| Cookie-based (ASP.NET Core Identity)       | -             |
| **Autentifikacija (API)**| JWT Bearer Token                           | -             |
| **Email servis**         | Brevo API (HTTP) — primarno               | v3            |
| **Email fallback**       | SMTP (Gmail/Mailtrap) — lokalno            | -             |
| **PDF generisanje**      | QuestPDF                                   | 2024.10.4     |
| **Markdown parsing**     | Markdig                                    | 0.37.0        |
| **JSON**                 | Newtonsoft.Json                             | 13.0.3        |
| **Kontejnerizacija**     | Docker (multi-stage build)                 | -             |
| **Orkestracija**         | Docker Compose                             | 3.8           |
| **CI/CD**                | GitHub Actions                             | -             |
| **Cloud hosting**        | Render (Web Service)                       | -             |
| **Container Registry**   | Docker Hub                                 | -             |
| **Test framework**       | xUnit + NUnit + Playwright                 | Razne         |
| **Mocking**              | Moq                                        | 4.20.72       |
| **Assertions**           | FluentAssertions                           | 8.9.0         |
| **Code coverage**        | Coverlet                                   | 10.0.0        |

---

## 3. Potrebni alati i verzije

### Za lokalni razvoj

| Alat                     | Minimalna verzija | Napomena                                  |
|--------------------------|-------------------|-------------------------------------------|
| **.NET SDK**              | 8.0               | Obavezno. Preuzeti sa https://dotnet.microsoft.com/download/dotnet/8.0 |
| **Git**                   | 2.40+             | Za kloniranje repozitorija                |
| **MySQL Server**          | 8.0               | Lokalna baza podataka. Alternativno koristiti Docker |
| **Docker Desktop**        | 24.0+             | Opciono — za pokretanje putem `docker-compose` |
| **Docker Compose**        | 2.20+             | Dolazi sa Docker Desktop                  |
| **Visual Studio 2022**    | 17.0+             | Opciono — preporučeni IDE                 |
| **Visual Studio Code**    | Najnovija          | Opciono — alternativni editor sa C# ekstenzijom |
| **Node.js**               | -                 | Nije potreban (nema JS frontend-a)        |

### Za produkcijski deployment

| Alat/Servis              | Opis                                       |
|--------------------------|--------------------------------------------|
| **Docker Hub nalog**      | Za push Docker image-a                    |
| **Render nalog**          | Cloud platforma za hosting                |
| **GitHub nalog**          | Za CI/CD workflow                         |
| **Upstash nalog**         | Za Redis cache u produkciji (opciono)     |
| **Brevo nalog**           | Za email slanje putem Brevo HTTP API      |
| **TiDB Cloud nalog**      | Za managed MySQL bazu u produkciji        |

### Instalacija .NET 8 SDK

**Windows:**
```powershell
# Preuzeti instalacijski fajl sa:
# https://dotnet.microsoft.com/download/dotnet/8.0
# ili putem winget:
winget install Microsoft.DotNet.SDK.8
```

**Linux (Ubuntu/Debian):**
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

**macOS:**
```bash
brew install dotnet-sdk@8
```

**Provjera instalacije:**
```bash
dotnet --version
# Očekivani output: 8.0.xxx
```

---

## 4. Environment varijable

Aplikacija koristi `.env` fajl u root-u `Projekat/` direktorija za lokalni razvoj. Taj fajl se automatski parsira pri pokretanju aplikacije u `Program.cs`. U produkciji se environment varijable postavljaju direktno na Render dashboardu.

### Kompletna lista environment varijabli

#### Obavezne varijable

| Varijabla | Opis | Primjer vrijednosti |
|-----------|------|---------------------|
| `ConnectionStrings__DefaultConnection` | MySQL connection string | `Server=localhost;Port=3306;Database=smartlib;User=smartlib_user;Password=smartlib_pass;` |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core okruženje | `Development` / `Production` |

#### Email servis (Brevo API — produkcija)

| Varijabla | Opis | Primjer vrijednosti |
|-----------|------|---------------------|
| `EmailSettings__BrevoApiKey` | Brevo API ključ za slanje emailova putem HTTP API-ja | `xkeysib-xxxxxxxx...` |
| `EmailSettings__SenderEmail` | Email adresa pošiljaoca (mora biti verifikovana na Brevo) | `theofficialsmartlibrary@gmail.com` |
| `EmailSettings__SenderName` | Ime pošiljaoca koje se prikazuje primaocu | `SmartLib` |

> **Napomena:** `EmailSettings__BrevoApiKey` je obavezna u produkciji. Bez nje, aplikacija neće moći slati emailove na Render-u jer Render blokira odlazne SMTP portove (25, 465, 587).

#### Email servis (SMTP — lokalni fallback)

| Varijabla | Opis | Primjer vrijednosti |
|-----------|------|---------------------|
| `EmailSettings__SmtpServer` | SMTP server (koristi se samo ako Brevo API nije dostupan) | `smtp.gmail.com` |
| `EmailSettings__SmtpPort` | SMTP port | `587` |
| `EmailSettings__Username` | SMTP korisničko ime | `theofficialsmartlibrary@gmail.com` |
| `EmailSettings__Password` | SMTP lozinka (Gmail App Password) | `usndeejiwujdqyot` |

#### Google Books API

| Varijabla | Opis | Primjer vrijednosti |
|-----------|------|---------------------|
| `GOOGLE_BOOKS_API_KEY` | API ključ za pretragu knjiga putem Google Books API | `AIzaSyBAO54pQlK7WKNctuxmuwPtNFyogwKm178` |

#### Upstash Redis (opciono — cache)

| Varijabla | Opis | Primjer vrijednosti |
|-----------|------|---------------------|
| `UPSTASH_REDIS_REST_URL` | Upstash Redis REST endpoint | `https://growing-eagle-130927.upstash.io` |
| `UPSTASH_REDIS_REST_TOKEN` | Upstash Redis REST token | `gQAAAAAAAf9vAAIgcDFl...` |
| `UPSTASH_REDIS_URL` | Upstash Redis TCP URL (za StackExchange.Redis klijent) | `rediss://default:TOKEN@host:6379` |
| `UPSTASH_REDIS_PASSWORD` | Upstash Redis lozinka | `gQAAAAAAAf9vAAIgcDFl...` |

> **Napomena o Redis varijablama:** Aplikacija koristi `StackExchange.Redis` paket koji komunicira sa Redisom putem standardnog **TCP protokola** (koristeći `UPSTASH_REDIS_URL` i `UPSTASH_REDIS_PASSWORD`). Upstash dashboard automatski generiše i REST API kredencijale (`REST_URL` i `REST_TOKEN`) koji su dokumentovani i postavljeni na Render-u za slučaj da se u budućnosti pređe na serverless arhitekturu, ali se trenutno u kodu ne koriste. Ako nijedna Redis varijabla nije postavljena, aplikacija automatski koristi In-Memory Cache kao fallback.

### Kompletni `.env` fajl za lokalni razvoj

Fajl se nalazi na putanji: `Projekat/.env`

```env
ConnectionStrings__DefaultConnection=Server=localhost;Port=3306;Database=smartlib;User=smartlib_user;Password=smartlib_pass;

# Brevo Email API (primarni način slanja emailova — koristi se u produkciji na Render-u)
EmailSettings__BrevoApiKey=UNESITE_BREVO_API_KEY_OVDJE
EmailSettings__SenderEmail=theofficialsmartlibrary@gmail.com
EmailSettings__SenderName=SmartLib

# SMTP Email Setup (fallback za lokalni razvoj, koristi se ako Brevo API nije dostupan)
EmailSettings__SmtpServer=smtp.gmail.com
EmailSettings__SmtpPort=587
EmailSettings__Username=theofficialsmartlibrary@gmail.com
EmailSettings__Password=usndeejiwujdqyot

GOOGLE_BOOKS_API_KEY=AIzaSyBAO54pQlK7WKNctuxmuwPtNFyogwKm178

UPSTASH_REDIS_REST_URL="https://growing-eagle-130927.upstash.io"
UPSTASH_REDIS_REST_TOKEN="gQAAAAAAAf9vAAIgcDFlMDA5YmJiN2ZlOWE0ODMwODE3YmEyYzcyMDIwM2I0Yg"

# Upstash Redis TCP (StackExchange.Redis)
UPSTASH_REDIS_URL="rediss://default:gQAAAAAAAf9vAAIgcDFlMDA5YmJiN2ZlOWE0ODMwODE3YmEyYzcyMDIwM2I0Yg@growing-eagle-130927.upstash.io:6379"
UPSTASH_REDIS_PASSWORD="gQAAAAAAAf9vAAIgcDFlMDA5YmJiN2ZlOWE0ODMwODE3YmEyYzcyMDIwM2I0Yg"
```

> **VAŽNO:** `.env` fajl je dodan u `.gitignore` i **ne smije** biti commitovan u repozitorij jer sadrži osjetljive podatke (lozinke, API ključeve). Ovaj fajl se već nalazi u `Projekat/.env` — samo je potrebno unijeti Brevo API ključ.

### Environment varijable na Render-u (produkcija)

Na Render dashboardu (Environment tab) postavljene su sljedeće varijable:

| Varijabla | Opis |
|-----------|------|
| `ConnectionStrings__DefaultConnection` | Connection string za TiDB Cloud MySQL bazu |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `EmailSettings__BrevoApiKey` | Brevo API ključ (primarni email servis) |
| `EmailSettings__SenderEmail` | `theofficialsmartlibrary@gmail.com` |
| `EmailSettings__SenderName` | `SmartLib` |
| `GOOGLE_BOOKS_API_KEY` | Google Books API ključ |
| `UPSTASH_REDIS_URL` | Upstash Redis TCP connection URL |
| `UPSTASH_REDIS_PASSWORD` | Upstash Redis lozinka |
| `UPSTASH_REDIS_REST_URL` | Upstash Redis REST endpoint |
| `UPSTASH_REDIS_REST_TOKEN` | Upstash Redis REST token |

---

## 5. Upute za lokalno pokretanje backend-a

### Preduvjeti
- Instaliran .NET 8 SDK
- Pokrenuta MySQL baza podataka (vidi [Sekcija 7](#7-upute-za-pokretanje-baze-podataka))
- Kreiran `.env` fajl u `Projekat/` direktoriju (vidi [Sekcija 4](#4-environment-varijable))

### Koraci

**1. Klonirati repozitorij:**
```bash
git clone https://github.com/amilababic4/SI_grupa5.git
cd SI_grupa5/Projekat
```

**2. Provjeriti `.env` fajl:**

`.env` fajl se već nalazi u `Projekat/` direktoriju. Provjeriti da connection string odgovara vašoj MySQL konfiguraciji. Ako želite koristiti Brevo API za emailove lokalno, zamijeniti `UNESITE_BREVO_API_KEY_OVDJE` sa stvarnim ključem.

**3. Restore NuGet paketa:**
```bash
dotnet restore SmartLib.sln
```

**4. Build projekta:**
```bash
dotnet build SmartLib.sln
```

**5. Pokrenuti Web aplikaciju:**
```bash
cd src/SmartLib.Web
dotnet run
```

**6. Pristupiti aplikaciji u browseru:**
```
https://localhost:5001   (HTTPS)
http://localhost:5000    (HTTP)
```

> **Napomena:** Pri prvom pokretanju, aplikacija automatski kreira bazu, sve tabele i seed podatke. Proces može trajati 10-30 sekundi.

### Pokretanje API projekta (opciono)

API projekat (`SmartLib.API`) je opcionalni REST API sloj sa JWT autentifikacijom. Pokreće se zasebno:

```bash
cd src/SmartLib.API
dotnet run
```

> **Napomena:** API projekat koristi zasebne `appsettings.json` postavke i JWT konfiguraciju. Potrebno je konfigurirati `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` u `src/SmartLib.API/appsettings.json`.

---

## 6. Upute za lokalno pokretanje frontend-a

SmartLib koristi **server-side rendering** putem Razor Views (.cshtml). **Nema zasebnog frontend projekta** — frontend je integrisan u `SmartLib.Web` projekat.

- **View engine:** Razor (.cshtml fajlovi)
- **Stilizacija:** Vanilla CSS + statički fajlovi u `wwwroot/`
- **JavaScript:** Vanilla JS u `wwwroot/`
- **Tag Helpers:** ASP.NET Core Tag Helpers za forme i linkove

### Struktura frontend-a

```
src/SmartLib.Web/
├── Views/                  # Razor Views (.cshtml)
│   ├── Home/               # Početna stranica
│   ├── Auth/               # Login/Register
│   ├── Knjiga/             # Upravljanje knjigama
│   ├── Shared/             # Layout, _ViewStart, _ViewImports
│   └── ...
├── wwwroot/                # Statički fajlovi
│   ├── css/                # CSS stilovi
│   ├── js/                 # JavaScript fajlovi
│   └── images/             # Slike
└── Models/                 # View modeli
```

### Pokretanje

Frontend se automatski pokreće zajedno sa backend-om — **nema potrebe za zasebnim pokretanjem**:

```bash
cd src/SmartLib.Web
dotnet run
```

Nema potrebe za `npm install`, `npm run dev`, ili sličnim komandama jer ne postoji zaseban JavaScript frontend.

> **Napomena:** Statički fajlovi se serviraju putem `app.UseStaticFiles()` middleware-a. Promjene u `.cshtml` fajlovima se automatski reflektuju pri refreshu stranice u Development modu.

---

## 7. Upute za pokretanje baze podataka

SmartLib koristi **MySQL 8.0** kao primarnu bazu podataka. U produkciji se koristi **TiDB Cloud** (MySQL-kompatibilna managed baza).

### Opcija A: MySQL putem Docker-a (preporučeno za lokalni razvoj)

**1. Pokrenuti samo MySQL kontejner:**
```bash
cd Projekat
docker-compose up -d smartlib-db
```

Ovo pokreće MySQL kontejner sa sljedećim parametrima:
- **Host:** `localhost`
- **Port:** `3306`
- **Database:** `smartlib`
- **User:** `smartlib_user`
- **Password:** `smartlib_pass`
- **Root password:** `root_pass`

**2. Provjera da je MySQL pokrenut:**
```bash
docker ps
# Trebalo bi pokazati kontejner 'smartlib-db' sa statusom 'healthy'
```

**3. Povezivanje na bazu (opciono, za provjeru):**
```bash
docker exec -it smartlib-db mysql -u smartlib_user -psmartlib_pass smartlib
```

### Opcija B: Lokalna MySQL instalacija

**1. Instalirati MySQL 8.0:**
- Windows: https://dev.mysql.com/downloads/installer/
- Linux: `sudo apt-get install mysql-server-8.0`
- macOS: `brew install mysql@8.0`

**2. Kreirati bazu i korisnika:**
```sql
CREATE DATABASE smartlib CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'smartlib_user'@'localhost' IDENTIFIED BY 'smartlib_pass';
GRANT ALL PRIVILEGES ON smartlib.* TO 'smartlib_user'@'localhost';
FLUSH PRIVILEGES;
```

**3. Provjera connection stringa:**

Provjeriti da `ConnectionStrings__DefaultConnection` u `.env` fajlu odgovara vašoj konfiguraciji:

```
Server=localhost;Port=3306;Database=smartlib;User=smartlib_user;Password=smartlib_pass;
```

### Opcija C: Kompletni Docker stack (MySQL + Web)

```bash
cd Projekat
docker-compose up --build
```

Ovo pokreće:
- `smartlib-db` — MySQL 8.0 kontejner na portu `3306`
- `smartlib-web` — ASP.NET Core Web aplikaciju na portu `5000`

Web aplikacija automatski čeka da baza bude zdrava (`service_healthy`) prije pokretanja.

### Produkcijska baza (TiDB Cloud)

U produkciji se koristi **TiDB Cloud** — managed MySQL-kompatibilna baza. Connection string se postavlja kao environment varijabla na Render-u (`ConnectionStrings__DefaultConnection`) i pokazuje na TiDB Cloud instancu.

---

## 8. Migracije i seed podaci

### EF Core migracije

Projekat sadrži EF Core migracije u `src/SmartLib.Infrastructure/Migrations/`:

| Migracija | Opis |
|-----------|------|
| `20260515174049_AddPasswordResetFields` | Dodaje polja za reset lozinke (ResetToken, ResetTokenExpiry) |
| `20260526094943_AddBanSystem` | Dodaje sistem zabrana (BanSystem, DatumDeaktivacije, BrojUklonjenihSadrzaja) |

### Primjena migracija

**Automatski pri pokretanju (produkcijski pristup):**

Aplikacija koristi `EnsureCreated()` u `Program.cs` koji automatski kreira bazu i sve tabele pri prvom pokretanju. Dodatno, aplikacija izvršava niz `ALTER TABLE` i `CREATE TABLE IF NOT EXISTS` naredbi za dodavanje novih kolona i tabela koje su dodane nakon inicijalne sheme.

> **VAŽNO:** Zbog korištenja `EnsureCreated()`, **standardne EF Core migracije (`dotnet ef database update`) neće raditi** na već kreiranoj bazi. Aplikacija sama upravlja shemom pri svakom pokretanju.

**Ručno putem SQL skripte (alternativa):**

U root-u `Projekat/` direktorija nalaze se SQL skripte za inicijalizaciju baze:

| Fajl | Opis |
|------|------|
| `smartlib.sql` | Kompletna shema + podaci (25 KB) |
| `smartlib_utf8.sql` | Verzija sa UTF-8 encodingom |
| `smartlib_mariadb.sql` | Kompatibilna verzija za MariaDB |
| `smartlib_hosting.sql` | Verzija za hosting okruženje |

```bash
# Import putem MySQL CLI:
mysql -u smartlib_user -psmartlib_pass smartlib < smartlib.sql
```

### Automatski seed podaci

Aplikacija pri svakom pokretanju (`Program.cs`) automatski seed-uje sljedeće podatke:

1. **Kategorije knjiga** (9 kategorija): Beletristika, Naučna fantastika, Historija, Nauka i tehnika, Filozofija, Biografija, Dječija literatura, Udžbenici, Ostalo
2. **Demo knjige** (11 knjiga): Sa ISBN-ovima, autorima, izdavačima i slikama
3. **Primjerci knjiga**: Po jedan primjerak za svaku seed knjiga (inventarni brojevi formata `SEED-ISBN-01`)
4. **Opisi i slike knjiga**: Hardkodirani opisi i URL-ovi slika za poznate naslove
5. **Aplikacijske postavke**: Default distributer email (`distributersmartlib@gmail.com`) u `AppPostavke` tabeli
6. **Demo primjerak**: Ako nema dostupnih primjeraka, kreira se demo knjiga sa primjerkom

> **Napomena:** Seed se izvršava samo ako podaci već ne postoje (koristi `IF NOT EXISTS` i provjere u kodu). Sigurno je pokretati aplikaciju više puta — neće duplicirati podatke.

### Tabele koje aplikacija automatski kreira

Aplikacija putem `Program.cs` automatski kreira sljedeće tabele (pored onih iz EF Core modela):

- `ForumObjave`, `ForumKomentari`, `ForumKomentarPrijave`, `ForumObjavaPrijave`, `ForumReakcije`
- `Recenzije`, `RecenzijaPrijave`
- `Notifikacije`
- `ListaZeljaStavke`, `ListaKolekcije`, `ListaKolekcijaStavke`
- `Vijesti`, `Dogadjaji`
- `ZahtjeviProduzenja`
- `NabavkaZahtjevi`
- `AppPostavke`

---

## 9. Pokretanje testova

### Struktura testova

```
tests/SmartLib.Tests/
├── Unit/              # Unit testovi (xUnit)
├── Integration/       # Integracioni testovi
├── UI/                # UI/Playwright testovi (NUnit)
├── Security/          # Sigurnosni testovi
└── TestData/          # Test podaci i helperi
```

### Test frameworkovi i alati

| Alat | Verzija | Namjena |
|------|---------|---------|
| xUnit | 2.9.3 | Unit testovi |
| NUnit | 4.6.0 | UI testovi (Playwright) |
| Moq | 4.20.72 | Mockanje zavisnosti |
| FluentAssertions | 8.9.0 | Čitljive tvrdnje |
| Microsoft.Playwright | 1.59.0 | End-to-end UI testovi |
| Microsoft.EntityFrameworkCore.InMemory | 8.0.0 | In-memory baza za testove |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.0 | Integracioni testovi |
| Coverlet | 10.0.0 | Code coverage |

### Pokretanje svih testova

```bash
cd Projekat
dotnet test SmartLib.sln
```

### Pokretanje specifičnih kategorija testova

```bash
# Samo unit testovi
dotnet test tests/SmartLib.Tests --filter "FullyQualifiedName~Unit"

# Samo integracioni testovi
dotnet test tests/SmartLib.Tests --filter "FullyQualifiedName~Integration"

# Samo sigurnosni testovi
dotnet test tests/SmartLib.Tests --filter "FullyQualifiedName~Security"

# Samo UI testovi
dotnet test tests/SmartLib.Tests --filter "FullyQualifiedName~UI"
```

### Pokretanje sa code coverage izvještajem

```bash
dotnet test SmartLib.sln --collect:"XPlat Code Coverage"
```

Coverage izvještaj se generira u `tests/SmartLib.Tests/TestResults/` direktoriju.

### Preduvjeti za UI (Playwright) testove

```bash
# Instalirati Playwright browsere (samo jednom)
cd tests/SmartLib.Tests
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

> **Napomena:** Playwright testovi zahtijevaju pokrenutu aplikaciju na očekivanom URL-u. Provjeriti konfiguraciju u test fajlovima.

### Testovi u CI/CD pipeline-u

Testovi se automatski pokreću u GitHub Actions pipeline-u **prije svakog deploymenta** kao quality gate. Ako bilo koji test padne, deployment se **ne izvršava**.

```bash
# Komanda koja se koristi u pipeline-u (isključuje UI testove):
dotnet test Projekat/SmartLib.sln --no-build --configuration Release --filter "FullyQualifiedName!~UiTests"
```

Pipeline pokreće sljedeće kategorije testova:

| Kategorija | Broj fajlova | Zahtijeva MySQL? | Uključena u pipeline? |
|------------|-------------|------------------|----------------------|
| Unit testovi (API) | 6 | ❌ (InMemory) | ✅ Da |
| Unit testovi (Web) | 10 | ❌ (InMemory) | ✅ Da |
| Integracioni testovi | 10 | ❌ (InMemory) | ✅ Da |
| Sigurnosni testovi | 1 | ❌ (InMemory) | ✅ Da |
| UI/Playwright testovi | 13 | ✅ Da + Browser | ❌ Ne (zahtijevaju live aplikaciju) |

> **Napomena:** UI/Playwright testovi su isključeni iz pipeline-a jer zahtijevaju pokrenut browser i live aplikaciju. Pokreću se samo lokalno.

---

## 10. Produkcijski / Cloud deployment

### Pregled CI/CD pipeline-a

SmartLib koristi **GitHub Actions** za automatski deployment na **Render** putem Docker Hub-a. Pipeline uključuje **automatsko testiranje** kao quality gate — deployment se ne izvršava ako testovi padnu.

```
┌────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│  Push na   │────▶│ GitHub       │────▶│  Testovi     │────▶│  Docker Hub  │────▶│    Render    │
│  main      │     │ Actions      │     │  (Quality    │     │  (Image)     │     │  (Deploy)    │
│  branch    │     │ Setup+Build  │     │   Gate) ✅/❌ │     │              │     │              │
└────────────┘     └──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
```

> **Napomena:** Ako bilo koji test padne (unit, integracioni ili sigurnosni), pipeline se zaustavlja i Docker build/push/deploy se **ne izvršavaju**. Ovo osigurava da samo testiran i provjeren kod dospije u produkciju.

### Trigger-i za deployment

Deployment se automatski pokreće kada:
1. **Push na `main` branch** — direktni push
2. **Merge pull requesta u `main`** — zatvoren i merged PR

### GitHub Actions workflow

Lokacija: `.github/workflows/deploy.yml`

Workflow izvršava sljedeće korake:

**Faza 1: Testiranje (Quality Gate)**
1. **Checkout** — klonira repozitorij
2. **Setup .NET 8 SDK** — instalira .NET 8 SDK na GitHub Actions runner
3. **Restore NuGet packages** — preuzima sve zavisnosti (`dotnet restore`)
4. **Build solution** — kompajlira cijeli projekat u Release modu (`dotnet build`)
5. **Run tests** — pokreće unit, integracione i sigurnosne testove (`dotnet test`). UI/Playwright testovi su isključeni jer zahtijevaju pokrenut browser i live aplikaciju.

**Faza 2: Docker Build & Push**
6. **Docker Hub Login** — prijava na Docker Hub koristeći GitHub Secrets
7. **Build image** — gradi Docker image iz `Projekat/src/SmartLib.Web/Dockerfile`
8. **Tag image** — tagira image sa `latest` tagom
9. **Push image** — pushuje image na Docker Hub

**Faza 3: Deployment**
10. **Trigger Render deploy** — šalje POST request na Render Deploy Hook URL

### Potrebni GitHub Secrets

Postaviti u **Settings → Secrets and variables → Actions** na GitHub repozitoriju:

| Secret | Opis |
|--------|------|
| `DOCKERHUB_USERNAME` | Docker Hub korisničko ime |
| `DOCKERHUB_TOKEN` | Docker Hub Access Token (ne lozinka!) |
| `RENDER_DEPLOY_HOOK` | Render Deploy Hook URL (generisan na Render dashboardu) |

### Dockerfile (Multi-stage build)

Lokacija: `Projekat/src/SmartLib.Web/Dockerfile`

```dockerfile
# Build stage — koristi .NET SDK za kompajliranje
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/SmartLib.Core/SmartLib.Core.csproj src/SmartLib.Core/
COPY src/SmartLib.Infrastructure/SmartLib.Infrastructure.csproj src/SmartLib.Infrastructure/
COPY src/SmartLib.Web/SmartLib.Web.csproj src/SmartLib.Web/
RUN dotnet restore src/SmartLib.Web/SmartLib.Web.csproj
COPY src/ src/
RUN dotnet publish src/SmartLib.Web/SmartLib.Web.csproj -c Release -o /app/publish

# Runtime stage — koristi manji ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SmartLib.Web.dll"]
```

### Ručni deployment na Render

**1. Kreiranje Web Service-a na Render-u:**
- Prijaviti se na https://render.com
- Kliknuti **New → Web Service**
- Odabrati **Deploy an existing image from a registry**
- Unijeti Docker image URL: `docker.io/VAŠE_IME/smartlib-web:latest`

**2. Konfiguracija Render servisa:**
- **Name:** `smartlib-web`
- **Region:** Frankfurt (EU) ili po izboru
- **Instance Type:** Free ili Starter
- **Docker Command:** Ostaviti prazno (koristi ENTRYPOINT iz Dockerfile-a)
- **Port:** `8080`

**3. Postaviti environment varijable na Render-u:**

Navigirati na **Environment** tab i dodati sve varijable iz [Sekcije 4](#environment-varijable-na-render-u-produkcija).

**4. Kreirati Deploy Hook:**
- Na Render dashboardu → Settings → **Deploy Hook**
- Kopirati generirani URL
- Dodati ga kao `RENDER_DEPLOY_HOOK` GitHub Secret

**5. Pokrenuti prvi deployment:**
```bash
# Opcija A: Push na main branch (automatski putem GitHub Actions)
git push origin main

# Opcija B: Ručni build i push
cd Projekat
docker build -f src/SmartLib.Web/Dockerfile -t smartlib-web:latest .
docker tag smartlib-web:latest docker.io/VAŠE_IME/smartlib-web:latest
docker push docker.io/VAŠE_IME/smartlib-web:latest

# Triggerovati Render deploy ručno
curl -X POST "https://api.render.com/deploy/srv-xxxxx?key=xxxxx"
```

### Baza podataka u produkciji (TiDB Cloud)

Produkcijska MySQL baza je hostovana na **TiDB Cloud** — MySQL-kompatibilnoj managed bazi podataka. Connection string u `ConnectionStrings__DefaultConnection` na Render-u pokazuje na TiDB Cloud instancu.

TiDB Cloud je odabran jer:
- Pruža MySQL 8.0 kompatibilnost
- Nudi besplatni Serverless Tier
- Render ne nudi nativni MySQL hosting

---

## 11. Link na deployment

| Okruženje | URL |
|-----------|-----|
| **Produkcija (Render)** | https://smartlib-web.onrender.com/ |
| **Redis Health Check** | https://smartlib-web.onrender.com/health/redis |

> **Napomena:** Render free tier servisi ulaze u sleep nakon 15 minuta neaktivnosti. Prvo učitavanje može trajati 30-60 sekundi dok se servis "budi".

---

## 12. Poznata ograničenja deploymenta

### Render Free Tier ograničenja
1. **Cold start:** Servis se gasi nakon ~15 minuta neaktivnosti. Prvo otvaranje stranice nakon toga traje 30-60 sekundi
2. **Ograničen RAM:** Free tier ima 512 MB RAM-a
3. **Nema persistentnog storage-a:** Uploadovani fajlovi se gube pri redeploymentu
4. **Bandwidth limit:** 100 GB/mjesec na free tier-u
5. **Build timeout:** Maksimalno 30 minuta za build
6. **SMTP blokiran:** Render blokira odlazne SMTP portove — zato se koristi Brevo HTTP API za emailove

### Baza podataka
7. **Eksterna baza:** MySQL baza se hostuje na TiDB Cloud jer Render ne podržava MySQL nativno
8. **EnsureCreated() vs Migracije:** Koristi se `EnsureCreated()` umjesto standardnih EF Core migracija, što znači da se promjene sheme dodaju putem raw SQL `ALTER TABLE` naredbi u `Program.cs`
9. **Seed podaci:** Seed knjiga koristi hardkodirane Amazon image URL-ove koji mogu prestati raditi

### Redis/Cache
10. **Redis opcioni:** Ako Upstash Redis nije konfigurisan, koristi se In-Memory cache koji se gubi pri svakom restartu servisa
11. **Upstash Free Tier:** Ograničen na 10.000 komandi dnevno i 256 MB podataka

### Email (Brevo)
12. **Brevo Free Tier:** Ograničen na 300 emailova dnevno
13. **Verifikacija sender-a:** Email adresa pošiljaoca mora biti verifikovana na Brevo nalogu

### Docker
14. **Docker Hub rate limiting:** Anonimni korisnici imaju limit od 100 pull-ova na 6 sati
15. **Image veličina:** Multi-stage build koristi .NET 8 runtime (~220 MB), ukupni image je ~300-350 MB

### Općenito
16. **Nema HTTPS certifikata lokalno:** Lokalni razvoj koristi self-signed certifikat
17. **Nema horizontalnog skaliranja:** Jedna instanca servisa na Render-u
18. **Nema backup strategije:** Nema automatskog backup-a baze podataka u free tier-u

---

## 13. Najčešći problemi i rješenja

### Problem 1: `Connection refused` pri pokretanju aplikacije

**Simptom:** Greška `MySql.Data.MySqlClient.MySqlException: Unable to connect to any of the specified MySQL hosts`

**Uzrok:** MySQL server nije pokrenut ili connection string nije ispravan.

**Rješenje:**
```bash
# Provjera da li MySQL radi (Docker):
docker ps | grep smartlib-db

# Ako nije pokrenut:
docker-compose up -d smartlib-db

# Ako koristite lokalni MySQL:
# Windows:
net start MySQL80
# Linux:
sudo systemctl start mysql

# Provjeriti connection string u .env fajlu:
# Server=localhost;Port=3306;Database=smartlib;User=smartlib_user;Password=smartlib_pass;
```

---

### Problem 2: `.env` fajl se ne čita

**Simptom:** Aplikacija koristi defaultne appsettings.json vrijednosti umjesto environment varijabli.

**Uzrok:** `.env` fajl nije na očekivanoj putanji. Aplikacija traži `.env` na sljedećim lokacijama (redom):
1. `{CWD}/.env`
2. `{CWD}/Projekat/.env`
3. `{CWD}/../../.env`
4. `{CWD}/../.env`

**Rješenje:**
```bash
# Pokrenuti aplikaciju iz pravog direktorija:
cd Projekat/src/SmartLib.Web
dotnet run

# ILI kopirati .env bliže radnom direktoriju
```

---

### Problem 3: Port 3306 već zauzet

**Simptom:** `docker-compose up` ne uspije jer je port 3306 zauzet.

**Uzrok:** Lokalni MySQL servis ili drugi kontejner koristi port 3306.

**Rješenje:**
```bash
# Windows — Provjera šta koristi port:
netstat -ano | findstr :3306

# Zaustaviti lokalni MySQL:
net stop MySQL80

# ILI promijeniti port u docker-compose.yml:
# ports:
#   - "3307:3306"
# I ažurirati connection string u .env:
# Server=localhost;Port=3307;...
```

---

### Problem 4: `dotnet restore` ne uspijeva

**Simptom:** Greška pri restore-u NuGet paketa.

**Rješenje:**
```bash
# Očistiti NuGet cache:
dotnet nuget locals all --clear

# Pokušati ponovo:
dotnet restore SmartLib.sln

# Ako ne radi, provjera .NET verzije:
dotnet --version
# Mora biti 8.0.xxx
```

---

### Problem 5: Docker build ne uspijeva

**Simptom:** `docker build` javlja grešku pri kopiranju fajlova ili restore-u.

**Uzrok:** Docker context nije ispravan. Dockerfile očekuje da se build pokrene iz `Projekat/` direktorija.

**Rješenje:**
```bash
# Ispravna komanda (iz Projekat/ direktorija):
docker build -f src/SmartLib.Web/Dockerfile -t smartlib-web:latest .

# ILI koristiti docker-compose:
docker-compose up --build
```

---

### Problem 6: GitHub Actions workflow ne pokreće deployment

**Simptom:** Push na `main` ne triggeruje deployment na Render.

**Uzrok:** GitHub Secrets nisu postavljeni ili Render Deploy Hook URL je nevažeći.

**Rješenje:**
1. Provjeriti GitHub Secrets (Settings → Secrets):
   - `DOCKERHUB_USERNAME` — mora biti tačno korisničko ime
   - `DOCKERHUB_TOKEN` — mora biti Access Token, ne lozinka
   - `RENDER_DEPLOY_HOOK` — mora biti važeći URL
2. Provjeriti Actions tab za error logove
3. Provjeriti da workflow fajl postoji na `.github/workflows/deploy.yml`

---

### Problem 7: Render servis se ne pokreće (Health Check fails)

**Simptom:** Render prikazuje "Deploy failed" ili servis stalno restartuje.

**Uzrok:** Aplikacija ne može uspostaviti konekciju na bazu ili nedostaju environment varijable.

**Rješenje:**
1. Provjeriti Render logove: Dashboard → Service → Logs
2. Provjeriti da su sve obavezne environment varijable postavljene
3. Provjeriti da je connection string ispravan i da je TiDB Cloud baza dostupna
4. Port mora biti `8080` (Dockerfile EXPOSE)

---

### Problem 8: Emailovi se ne šalju u produkciji

**Simptom:** Registracija ili reset lozinke ne šalje email na Render-u.

**Uzrok:** `EmailSettings__BrevoApiKey` nije postavljen na Render-u, ili Brevo API ključ je nevažeći.

**Rješenje:**
1. Provjeriti da je `EmailSettings__BrevoApiKey` postavljen na Render Environment tab-u
2. Provjeriti da je Brevo API ključ validan na https://app.brevo.com → SMTP & API → API Keys
3. Provjeriti da je sender email (`theofficialsmartlibrary@gmail.com`) verifikovan na Brevo nalogu
4. Provjeriti Render logove za poruke `Brevo API returned {StatusCode}`
5. **Ne koristiti SMTP na Render-u** — Render blokira odlazne SMTP portove (25, 465, 587)

---

### Problem 9: Emailovi se ne šalju lokalno

**Simptom:** Email se ne šalje pri lokalnom razvoju.

**Uzrok:** SMTP kredencijali nisu ispravni ili Brevo API ključ nije postavljen u `.env`.

**Rješenje:**
- **Opcija A:** Dodati Brevo API ključ u `.env` fajl (`EmailSettings__BrevoApiKey=xkeysib-...`)
- **Opcija B:** Koristiti Gmail SMTP lokalno — kreirati Gmail App Password:
  1. Google Account → Security → 2-Factor Authentication → uključiti
  2. App Passwords → kreirati novu → kopirati 16-znakovnu lozinku
  3. Postaviti u `.env`: `EmailSettings__Password=vaša_app_lozinka`
- **Opcija C:** Koristiti Mailtrap za testno okruženje
- **Fallback:** Ako nijedna metoda ne radi, email sadržaj se ispisuje u konzolu (logove)

---

### Problem 10: Redis konekcija ne radi

**Simptom:** `[Redis] Using in-memory distributed cache (missing Redis TCP credentials)` u logovima.

**Uzrok:** Upstash Redis environment varijable nisu postavljene ili su neispravne.

**Rješenje:**
```bash
# Provjera Redis health check-a:
curl https://smartlib-web.onrender.com/health/redis

# Provjeriti da su postavljene:
# UPSTASH_REDIS_URL
# UPSTASH_REDIS_PASSWORD

# Format UPSTASH_REDIS_URL mora biti:
# rediss://default:PASSWORD@HOST:6379
```

> **Napomena:** Aplikacija radi i bez Redis-a — koristi in-memory cache kao fallback.

---

### Problem 11: Playwright testovi ne rade

**Simptom:** UI testovi padaju sa "Browser not found" greškom.

**Rješenje:**
```bash
# Instalirati Playwright browsere:
cd tests/SmartLib.Tests
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install

# Na Linux-u:
dotnet tool install --global Microsoft.Playwright.CLI
playwright install --with-deps
```

---

### Problem 12: `EnsureCreated()` konflikt sa migracijama

**Simptom:** `dotnet ef database update` javlja grešku jer tabele već postoje.

**Uzrok:** `EnsureCreated()` je kreirao tabele, a EF Core migracije pokušavaju kreirati iste tabele.

**Rješenje:**
- **Ne koristiti** `dotnet ef database update` na bazi koja je kreirana putem `EnsureCreated()`
- Pustiti aplikaciju da sama upravlja shemom pri pokretanju
- Ako trebate "čistu" bazu: obrisati bazu i pustiti aplikaciju da je ponovo kreira

```sql
-- Brisanje baze (OPREZ: briše sve podatke!)
DROP DATABASE smartlib;
CREATE DATABASE smartlib CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

---

### Problem 13: Slike knjiga se ne prikazuju

**Simptom:** Slike knjiga su prazne ili pokazuju broken image ikonu.

**Uzrok:** Slike se učitavaju sa eksternih URL-ova (Amazon, itd.) koji mogu biti nedostupni ili blokirani.

**Rješenje:**
- Ovo je poznato ograničenje — slike zavise od eksternih servisa
- Seed podaci u `Program.cs` sadrže hardkodirane URL-ove slika
- Za lokalni razvoj, slike će raditi dokle god imate internet konekciju

---

*Dokument pripremila: SI Grupa 5, ETF Sarajevo*
*Datum: Juni 2026.*
*Verzija: 1.1*
