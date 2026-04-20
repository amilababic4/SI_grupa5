# Tehnički Setup — SmartLib

## 1. Tech Stack

### 1.1 Programski jezik

| Komponenta | Jezik | Obrazloženje |
|---|---|---|
| **Backend** | C# 12 | Moderan, strogo tipiziran, bogat ekosistem |
| **Frontend** | C# (Razor Views) | Isti jezik za cijeli stack — konzistentan razvoj |

### 1.2 Frameworks

| Komponenta | Framework | Verzija | Obrazloženje |
|---|---|---|---|
| **Web Framework** | ASP.NET Core | 8.0 (LTS) | Zreli, performantni framework sa ugrađenim MVC patternom, dependency injection, middleware, autentifikacijom. LTS verzija garantuje dugoročnu podršku. |
| **Frontend (UI)** | ASP.NET Core MVC + Razor Views | 8.0 | Server-side renderovanje UI-a u C#. Razor sintaksa omogućava kombinaciju C# logike i HTML markup-a u `.cshtml` fajlovima. Tag Helpers olakšavaju generisanje formi, linkova i validacije. |
| **ORM** | Entity Framework Core | 8.x | Službeni ORM za .NET sa podrškom za Code-First migracije, LINQ upite i PostgreSQL provider (Npgsql). |
| **Autentifikacija** | ASP.NET Core Identity / Cookie Auth | 8.0 | Ugrađeni mehanizam za cookie-based autentifikaciju, pogodan za MVC aplikacije. |
| **Hashiranje lozinki** | BCrypt.Net | — | Industrijski standard za sigurno hashiranje lozinki |
| **Test Framework** | xUnit + Moq | — | Najpopularniji test framework za .NET, uz Moq za mockanje zavisnosti |

#### Zašto ASP.NET Core MVC?

| Opcija | Prednosti | Nedostaci |
|---|---|---|
| **ASP.NET Core MVC (Razor Views)** | C# za sve, server-side rendering, zreli framework, Tag Helpers, široka dokumentacija | Svaki klik = novi HTTP zahtjev |


**Obrazloženje:** ASP.NET Core MVC je izabran jer je najzreliji i najdokumentovaniji pristup za C# web razvoj. Tim koristi isti jezik (C#) za kompletni stack bez potrebe za učenjem JavaScript-a. Razor Views pružaju dovoljnu interaktivnost za bibliotečki sistem, a server-side rendering je jednostavniji za deployment i debugging.

### 1.3 Baza podataka

| Komponenta | Tehnologija | Verzija | Obrazloženje |
|---|---|---|---|
| **RDBMS** | PostgreSQL | 16.x | ACID transakcije, JSONB podrška (za audit log), referencijalni integritet. Besplatan i open-source. |
| **EF Core Provider** | Npgsql.EntityFrameworkCore.PostgreSQL | 8.x | Službeni PostgreSQL provider za EF Core |

#### Zašto PostgreSQL?

| Baza | Razlog odbijanja |
|---|---|
| **SQLite** | Nema podršku za konkurentne upise, nepogodna za produkciju |
| **MySQL** | Slabija JSONB podrška u odnosu na PostgreSQL (bitno za audit log) |
| **SQL Server** | Komercijalna licenca, veći resursni zahtjevi |
| **PostgreSQL** | Besplatan, JSONB sa GIN indeksima, zrele transakcije, široka zajednica |

### 1.4 Razvojni alati

| Alat | Svrha |
|---|---|
| Visual Studio 2022 / VS Code + C# Dev Kit | IDE za C# razvoj |
| pgAdmin 4 | GUI za PostgreSQL administraciju |
| Postman | Testiranje API endpoint-a |
| Git + GitHub | Verzioniranje koda i kolaboracija |
| .NET CLI (`dotnet`) | Build, run, migracije, testiranje |
| Docker Desktop | Kontejnerizacija za lokalni razvoj |

---

## 2. Branching strategija

Detaljno opisano u dokumentu [BranchingStrategy.md](BranchingStrategy.md).

**Ukratko:** Odabran je **GitHub Flow** — jednostavna strategija sa `main` granom i kratkoživućim feature granama. Svaka promjena ide kroz Pull Request sa obaveznim code review-om.

---

## 3. Deployment strategija

### 3.1 Odabrani pristup: Docker kontejneri na Linux VM

| Alternativa | Razlog odbijanja |
|---|---|
| **Fizički server** | Zahtijeva fizički hardver, skupo, nepraktično za studentski projekat |
| **Cloud (AWS/Azure/GCP)** | Troškovi, kompleksnost konfiguracije, overkill za mali broj korisnika |
| **Kubernetes** | Značajan overhead za orkestraciju — nepotreban za monolitnu aplikaciju |
| **Bare-metal VM bez Dockera** | Docker pruža konzistentnost okruženja i jednostavniji deployment |

**Zašto Docker na Linux VM:**
1. **Konzistentnost** — isti kontejner radi identično na dev i production mašini
2. **Izolacija** — aplikacija i baza su izolovani u zasebnim kontejnerima
3. **Jednostavan deployment** — `docker-compose up` pokreće cijeli sistem
4. **Cijena** — Linux VM je dostupna po niskoj cijeni ili besplatno (univerzitetski server)

### 3.2 DockerCompose

| Servis | Opis |
|---|---|
| **`smartlib-web`** | ASP.NET Core MVC aplikacija (Kestrel) |
| **`smartlib-db`** | PostgreSQL 16 baza podataka |
| **`nginx`** (opciono) | Reverse proxy, SSL termination |

---

## 4. Web Server i Application Server

### 4.1 Web Server: Nginx

| Aspekt | Detalj |
|---|---|
| **Uloga** | Reverse proxy, SSL termination, load balancing |
| **Port** | 80 (HTTP), 443 (HTTPS) |
| **Zašto** | Performantan, nizak memory footprint, široka zajednica |

### 4.2 Application Server: Kestrel

| Aspekt | Detalj |
|---|---|
| **Uloga** | Pokretanje ASP.NET Core MVC aplikacije |
| **Port** | 5000 (interni) |
| **Zašto** | Ugrađen u ASP.NET Core, ne zahtijeva zasebnu konfiguraciju |

**Tok zahtjeva:**
```
Korisnik → Nginx (:80/443) → Kestrel (:5000) → PostgreSQL (:5432)
```

---

## 5. Operativni sistem

| Aspekt | Detalj |
|---|---|
| **Server OS** | Ubuntu Server 22.04 LTS |
| **Zašto Linux** | Besplatan, stabilan, optimalan za Docker i Nginx |
| **Razvoj** | Windows 10/11 (sa Docker Desktop ili WSL2) |
| **Cross-platform** | .NET 8 i Docker rade na svim platformama |

---

## 6. Rezime tehničkih odluka

| Odluka | Odabrano | Obrazloženje |
|---|---|---|
| Programski jezik | C# 12 | Cijeli stack u jednom jeziku |
| Web Framework | ASP.NET Core 8 MVC | Zreo, dokumentovan, C# za sve |
| Frontend | Razor Views (.cshtml) | Server-side C#, Tag Helpers |
| Baza podataka | PostgreSQL 16 | JSONB, ACID, besplatan |
| ORM | Entity Framework Core 8 | Code-First, LINQ, migracije |
| Web Server | Nginx | Reverse proxy, SSL |
| App Server | Kestrel | Ugrađen u .NET |
| Deployment | Docker na Linux VM | Konzistentnost, izolacija |
| Server OS | Ubuntu 22.04 LTS | Stabilan, besplatan |
| Test Framework | xUnit + Moq | Standard za .NET |
| Branching | GitHub Flow | Jednostavno, PR-based |
