# SmartLib — Continuous Deployment (CD) Pipeline

## Sadržaj

1. [Pregled CD pipeline-a](#1-pregled-cd-pipeline-a)
2. [Lokacija skripti i konfiguracija](#2-lokacija-skripti-i-konfiguracija)
3. [Preduvjeti za CD pipeline](#3-preduvjeti-za-cd-pipeline)
4. [GitHub Actions workflow — detaljna analiza](#4-github-actions-workflow--detaljna-analiza)
5. [Dockerfile — build aplikacije](#5-dockerfile--build-aplikacije)
6. [Docker Compose — lokalni deployment](#6-docker-compose--lokalni-deployment)
7. [Potrebne varijable i secrets](#7-potrebne-varijable-i-secrets)
8. [Šta se tačno deploya](#8-šta-se-tačno-deploya)
9. [Povezivanje servisa](#9-povezivanje-servisa)
10. [Kako se pokreće deployment](#10-kako-se-pokreće-deployment)
11. [Provjera uspješnosti deploymenta](#11-provjera-uspješnosti-deploymenta)
12. [Ručni koraci (opravdanje)](#12-ručni-koraci-opravdanje)
13. [Ponovljivost deploymenta](#13-ponovljivost-deploymenta)
14. [Produkcijski URL](#14-produkcijski-url)
15. [Poznata ograničenja deploymenta](#15-poznata-ograničenja-deploymenta)
16. [Rješavanje čestih problema](#16-rješavanje-čestih-problema)

---

## 1. Pregled CD pipeline-a

SmartLib koristi potpuno automatizovan **Continuous Deployment pipeline** baziran na **GitHub Actions**, **Docker Hub** i **Render** platformi. Pipeline uključuje **automatsko testiranje** kao quality gate — ako testovi padnu, deployment se ne izvršava.

### Tok deploymenta — od koda do produkcije

```
┌─────────────────┐         ┌───────────────────────────────────────────────────────────────────┐
│   Developer     │         │                    GitHub Actions (CI/CD)                          │
│   push na main  │────────▶│                                                                   │
│   ili merge PR  │         │  ┌─────────────┐  ┌────────────┐  ┌──────────────┐                │
└─────────────────┘         │  │  Checkout    │─▶│ Setup .NET │─▶│ Restore +    │                │
                            │  │  repozitorij │  │ 8 SDK      │  │ Build        │                │
                            │  └─────────────┘  └────────────┘  └──────┬───────┘                │
                            │                                          │                        │
                            │                                   ┌──────▼───────┐                │
                            │                                   │  Run Tests   │                │
                            │                                   │  (Unit,      │                │
                            │                                   │  Integration,│                │
                            │                                   │  Security)   │                │
                            │                                   └──────┬───────┘                │
                            │                                          │                        │
                            │                                    ✅ Pass? / ❌ Fail?             │
                            │                                     │           │                 │
                            │                                     │      STOP (no deploy)       │
                            │                                     ▼                             │
                            │  ┌──────────────┐  ┌────────────┐  ┌──────────────┐               │
                            │  │  Trigger     │◀─│ Push image │◀─│ Docker       │               │
                            │  │  Render      │  │ na Docker  │  │ Build + Tag  │               │
                            │  │  Deploy Hook │  │ Hub        │  │              │               │
                            │  └──────┬───────┘  └────────────┘  └──────────────┘               │
                            └─────────┼─────────────────────────────────────────────────────────┘
                                      │
                                      ▼
                            ┌─────────────────────────────────────────┐
                            │              Render.com                  │
                            │                                         │
                            │  1. Pull image sa Docker Hub-a          │
                            │  2. Pokretanje kontejnera               │
                            │  3. Inject environment varijabli        │
                            │  4. Aplikacija se pokreće na portu 8080 │
                            │  5. EnsureCreated() — auto-migracija    │
                            │  6. Seed podataka                       │
                            │  7. Health check prolazi                │
                            │  8. Servis je LIVE ✅                    │
                            └─────────────────────────────────────────┘
                                      │
                      ┌───────────────┼───────────────┐
                      ▼               ▼               ▼
              ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
              │  TiDB Cloud  │ │ Upstash Redis│ │  Brevo API   │
              │  (MySQL)     │ │ (Cache)      │ │  (Email)     │
              └──────────────┘ └──────────────┘ └──────────────┘
```

### Ključne karakteristike pipeline-a

- **Potpuno automatizovan:** Push na `main` → testovi → aplikacija je live bez ikakve ručne intervencije
- **Quality gate:** Testovi služe kao kontrolna tačka — samo testiran kod dospijeva u produkciju
- **Ponovljiv:** Svaki deployment prolazi identične korake
- **Idempotentan:** Aplikacija pri pokretanju sama kreira/ažurira bazu i seed podatke
- **Rollback:** Moguć putem Docker Hub image historije ili git revert + push

---

## 2. Lokacija skripti i konfiguracija

Sve skripte i konfiguracije za CD pipeline se nalaze u repozitoriju:

| Fajl | Putanja | Opis |
|------|---------|------|
| **GitHub Actions workflow** | `.github/workflows/deploy.yml` | Glavni CD pipeline — build, push, deploy |
| **Dockerfile** | `Projekat/src/SmartLib.Web/Dockerfile` | Multi-stage Docker build za Web aplikaciju |
| **Docker Compose** | `Projekat/docker-compose.yml` | Lokalni deployment stack (MySQL + Web) |
| **Environment varijable** | `Projekat/.env` | Lokalne env varijable (nije commitovan) |
| **Auto-migracija + Seed** | `Projekat/src/SmartLib.Web/Program.cs` | Automatska priprema baze pri pokretanju |

---

## 3. Preduvjeti za CD pipeline

### Servisi i nalozi

| Servis | Namjena | Kako se dobija |
|--------|---------|----------------|
| **GitHub** | Hosting repozitorija + Actions runner | https://github.com (besplatno) |
| **Docker Hub** | Container registry za Docker image | https://hub.docker.com (besplatno) |
| **Render** | Cloud hosting za Web Service | https://render.com (Free Tier) |
| **TiDB Cloud** | MySQL-kompatibilna managed baza | https://tidbcloud.com (Serverless, besplatno) |
| **Upstash** | Managed Redis cache | https://upstash.com (Free Tier, opciono) |
| **Brevo** | Email API servis | https://brevo.com (Free Tier, 300 email/dan) |

### Tehnički preduvjeti

- Git repozitorij na GitHub-u sa `main` branch-om
- Docker Hub nalog sa kreiranim Access Token-om
- Render Web Service konfigurisan sa Docker image-om
- Render Deploy Hook URL generisan
- TiDB Cloud cluster kreiran sa `smartlib` bazom
- GitHub Secrets postavljeni (vidi [Sekcija 7](#7-potrebne-varijable-i-secrets))

---

## 4. GitHub Actions workflow — detaljna analiza

### Lokacija fajla

```
.github/workflows/deploy.yml
```

### Kompletni sadržaj workflow fajla

```yaml
name: Build, Test, Push Docker & Deploy to Render

on:
  push:
    branches: ["main"]
  pull_request:
    types: [closed]
    branches: ["main"]

jobs:
  build-test-deploy:
    if: |
      github.event_name == 'push' ||
      (github.event_name == 'pull_request' && github.event.pull_request.merged == true)
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      # ── .NET Test Phase ──────────────────────────────────────────────
      - name: Setup .NET 8 SDK
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore NuGet packages
        run: dotnet restore Projekat/SmartLib.sln

      - name: Build solution
        run: dotnet build Projekat/SmartLib.sln --no-restore --configuration Release

      - name: Run tests (Unit, Integration, Security — without UI)
        run: dotnet test Projekat/SmartLib.sln --no-build --configuration Release --verbosity normal --filter "FullyQualifiedName!~UiTests"

      # ── Docker Build & Push Phase ────────────────────────────────────
      - name: Log in to Docker Hub
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKERHUB_USERNAME }}
          password: ${{ secrets.DOCKERHUB_TOKEN }}

      - name: Build image
        run: docker build -f Projekat/src/SmartLib.Web/Dockerfile -t smartlib-web:latest Projekat

      - name: Tag image
        run: docker tag smartlib-web:latest docker.io/${{ secrets.DOCKERHUB_USERNAME }}/smartlib-web:latest

      - name: Push image
        run: docker push docker.io/${{ secrets.DOCKERHUB_USERNAME }}/smartlib-web:latest

      # ── Deploy Phase ─────────────────────────────────────────────────
      - name: Trigger Render deploy
        run: curl -X POST "${{ secrets.RENDER_DEPLOY_HOOK }}"
```

### Korak-po-korak objašnjenje

#### Trigger (`on`)

```yaml
on:
  push:
    branches: ["main"]
  pull_request:
    types: [closed]
    branches: ["main"]
```

Pipeline se pokreće u dva slučaja:
1. **Direktni push na `main`** — kada developer pushuje direktno (npr. hotfix)
2. **Merge pull requesta u `main`** — standardni tok razvoja putem PR-ova

#### Guard uslov (`if`)

```yaml
if: |
  github.event_name == 'push' ||
  (github.event_name == 'pull_request' && github.event.pull_request.merged == true)
```

Osigurava da se deployment ne pokrene za **zatvorene ali ne-merged PR-ove**. Samo stvarno merged PR-ovi triggeruju deployment.

---

### Faza 1: Testiranje (Quality Gate)

#### Korak 1: Checkout repozitorija

```yaml
- name: Checkout
  uses: actions/checkout@v4
```

Klonira kompletni repozitorij u GitHub Actions runner (Ubuntu VM). Koristi službeni `actions/checkout` action v4.

#### Korak 2: Setup .NET 8 SDK

```yaml
- name: Setup .NET 8 SDK
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '8.0.x'
```

Instalira .NET 8 SDK na GitHub Actions runner. Koristi službeni `actions/setup-dotnet` action v4. Ovo je potrebno jer runner dolazi sa minimalnim alatima — SDK se mora eksplicitno instalirati.

#### Korak 3: Restore NuGet paketa

```yaml
- name: Restore NuGet packages
  run: dotnet restore Projekat/SmartLib.sln
```

Preuzima sve NuGet zavisnosti definisane u `.csproj` fajlovima. Ovo uključuje zavisnosti za sve projekte u solution-u: `SmartLib.Core`, `SmartLib.Infrastructure`, `SmartLib.Web`, `SmartLib.API` i `SmartLib.Tests`.

#### Korak 4: Build solution-a

```yaml
- name: Build solution
  run: dotnet build Projekat/SmartLib.sln --no-restore --configuration Release
```

Kompajlira cijeli solution u Release konfiguraciji. `--no-restore` preskače restore jer je već obavljen u prethodnom koraku. Ovo osigurava da se svi projekti uspješno kompajliraju prije pokretanja testova.

#### Korak 5: Pokretanje testova

```yaml
- name: Run tests (Unit, Integration, Security — without UI)
  run: dotnet test Projekat/SmartLib.sln --no-build --configuration Release --verbosity normal --filter "FullyQualifiedName!~UiTests"
```

Pokreće sve testove **osim UI/Playwright testova**. Ovo uključuje:

| Kategorija testova | Broj fajlova | Šta testiraju |
|--------------------|-------------|---------------|
| **Unit testovi (API)** | 6 fajlova | API kontroleri sa mockovanim zavisnostima |
| **Unit testovi (Web)** | 10 fajlova | Web kontroleri sa mockovanim zavisnostima |
| **Integracioni testovi** | 10 fajlova | End-to-end tokovi putem WebApplicationFactory + InMemory baze |
| **Sigurnosni testovi** | 1 fajl | SQL injection, XSS, JWT manipulacija, autorizacija |

Parametri:
- `--no-build` — preskače build jer je već obavljen
- `--configuration Release` — koristi Release build
- `--verbosity normal` — prikazuje nazive testova i rezultate
- `--filter "FullyQualifiedName!~UiTests"` — **isključuje UI/Playwright testove** jer oni zahtijevaju pokrenut browser i live aplikaciju, što nije dostupno u GitHub Actions okruženju bez dodatne konfiguracije

**Zašto su UI testovi isključeni?** Playwright testovi (`SmartLib.UiTests` namespace) zahtijevaju:
1. Instaliran Chromium/Firefox/WebKit browser na runneru
2. Pokrenutu SmartLib aplikaciju na `http://localhost:5000`
3. Pokrenutu MySQL bazu sa seed podacima

Ovo bi značajno zakomplikovalo pipeline, a unit, integracioni i sigurnosni testovi već pružaju dovoljnu pokrivenost za quality gate.

**Ako bilo koji test padne**, `dotnet test` vraća exit code ≠ 0, što automatski zaustavlja GitHub Actions workflow. Nijedan naredni korak (Docker build, push, deploy) se **ne izvršava**. Ovo je ključna funkcija quality gate-a.

> **Napomena:** Svi integracioni i sigurnosni testovi koriste **InMemory bazu podataka** (EF Core InMemory provider), tako da ne zahtijevaju MySQL server u pipeline-u. Ovo pojednostavljuje konfiguraciju i ubrzava izvršavanje.

---

### Faza 2: Docker Build & Push

#### Korak 6: Docker Hub prijava

```yaml
- name: Log in to Docker Hub
  uses: docker/login-action@v3
  with:
    username: ${{ secrets.DOCKERHUB_USERNAME }}
    password: ${{ secrets.DOCKERHUB_TOKEN }}
```

Autentificira se na Docker Hub koristeći **GitHub Secrets**:
- `DOCKERHUB_USERNAME` — korisničko ime na Docker Hub-u
- `DOCKERHUB_TOKEN` — Access Token (ne lozinka!) generisan na Docker Hub → Account Settings → Security → Access Tokens

#### Korak 7: Build Docker image-a

```yaml
- name: Build image
  run: docker build -f Projekat/src/SmartLib.Web/Dockerfile -t smartlib-web:latest Projekat
```

Gradi Docker image koristeći **multi-stage Dockerfile**. Ovaj korak obuhvata:
- **Build backend aplikacije:** `dotnet restore` + `dotnet publish` unutar Docker build stage-a
- **Build frontend-a:** Frontend (Razor Views, CSS, JS) se automatski uključuje u publish output jer je dio `SmartLib.Web` projekta
- **Optimizacija:** Multi-stage build koristi `sdk:8.0` za kompajliranje, a `aspnet:8.0` za runtime (manji image)

Parametri:
- `-f Projekat/src/SmartLib.Web/Dockerfile` — putanja do Dockerfile-a
- `-t smartlib-web:latest` — lokalni tag za image
- `Projekat` — build context (direktorij iz kojeg se kopiraju fajlovi)

#### Korak 8: Tag image-a

```yaml
- name: Tag image
  run: docker tag smartlib-web:latest docker.io/${{ secrets.DOCKERHUB_USERNAME }}/smartlib-web:latest
```

Tagira lokalno izgradeni image sa Docker Hub putanjom u formatu: `docker.io/{username}/smartlib-web:latest`

#### Korak 9: Push image-a na Docker Hub

```yaml
- name: Push image
  run: docker push docker.io/${{ secrets.DOCKERHUB_USERNAME }}/smartlib-web:latest
```

Upload-uje Docker image na Docker Hub registry. Ovo čini image dostupnim za Render da ga pull-uje.

---

### Faza 3: Deployment

#### Korak 10: Trigger Render deploy

```yaml
- name: Trigger Render deploy
  run: curl -X POST "${{ secrets.RENDER_DEPLOY_HOOK }}"
```

Šalje HTTP POST request na **Render Deploy Hook URL**. Render prima ovaj webhook i automatski:
1. Pull-uje najnoviji `smartlib-web:latest` image sa Docker Hub-a
2. Gasi stari kontejner
3. Pokreće novi kontejner sa svježim image-om
4. Inject-uje environment varijable konfigurisane na Render dashboardu
5. Čeka da aplikacija prođe health check

---

## 5. Dockerfile — build aplikacije

### Lokacija

```
Projekat/src/SmartLib.Web/Dockerfile
```

### Kompletni sadržaj

```dockerfile
# SmartLib.Web — Multi-stage Dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiranje .csproj fajlova i restore zavisnosti
COPY src/SmartLib.Core/SmartLib.Core.csproj src/SmartLib.Core/
COPY src/SmartLib.Infrastructure/SmartLib.Infrastructure.csproj src/SmartLib.Infrastructure/
COPY src/SmartLib.Web/SmartLib.Web.csproj src/SmartLib.Web/
RUN dotnet restore src/SmartLib.Web/SmartLib.Web.csproj

# Kopiranje kompletnog source koda i build
COPY src/ src/
RUN dotnet publish src/SmartLib.Web/SmartLib.Web.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "SmartLib.Web.dll"]
```

### Šta Dockerfile radi

| Faza | Opis | Rezultat |
|------|------|----------|
| **Build stage** | Koristi .NET 8 SDK image (~700 MB) | Kompajlira i publishuje aplikaciju |
| `dotnet restore` | Preuzima sve NuGet zavisnosti | Cached layer za brži re-build |
| `dotnet publish -c Release` | Kompajlira sve projekte (Core, Infrastructure, Web) | Optimizirani Release binaries u `/app/publish` |
| **Runtime stage** | Koristi .NET 8 ASP.NET runtime image (~220 MB) | Manji finalni image |
| `COPY --from=build` | Kopira samo publish output iz build stage-a | Finalni image bez SDK-a i source koda |
| `EXPOSE 8080` | Dokumentuje da aplikacija sluša na portu 8080 | Render koristi ovaj port |
| `ENTRYPOINT` | Definiše komandu za pokretanje aplikacije | `dotnet SmartLib.Web.dll` |

### Šta se build-a

Dockerfile build-a **cijeli backend + frontend** u jednom koraku jer je SmartLib monolitna MVC aplikacija:

- **SmartLib.Core** — domenski modeli, interfejsi, DTO-ovi
- **SmartLib.Infrastructure** — EF Core, repozitoriji, servisi (Email, Cache, PDF, itd.)
- **SmartLib.Web** — Controllers, Razor Views (.cshtml), statički fajlovi (CSS, JS, slike)

Frontend (Razor Views, wwwroot/) je dio `SmartLib.Web` projekta i automatski se uključuje u publish output.

---

## 6. Docker Compose — lokalni deployment

### Lokacija

```
Projekat/docker-compose.yml
```

### Kompletni sadržaj

```yaml
version: '3.8'

services:
  smartlib-db:
    image: mysql:8.0
    container_name: smartlib-db
    environment:
      MYSQL_DATABASE: smartlib
      MYSQL_USER: smartlib_user
      MYSQL_PASSWORD: smartlib_pass
      MYSQL_ROOT_PASSWORD: root_pass
    ports:
      - "3306:3306"
    volumes:
      - mysql_data:/var/lib/mysql
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost", "-u", "root", "-proot_pass"]
      interval: 5s
      timeout: 5s
      retries: 5

  smartlib-web:
    build:
      context: .
      dockerfile: src/SmartLib.Web/Dockerfile
    container_name: smartlib-web
    restart: always
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=smartlib-db;Port=3306;Database=smartlib;User=smartlib_user;Password=smartlib_pass
    ports:
      - "5000:8080"
    depends_on:
      smartlib-db:
        condition: service_healthy

volumes:
  mysql_data:
```

### Pokretanje

```bash
cd Projekat
docker-compose up --build
```

### Šta Docker Compose pokreće

| Servis | Image | Port | Opis |
|--------|-------|------|------|
| `smartlib-db` | `mysql:8.0` | `3306:3306` | MySQL baza sa healthcheck-om |
| `smartlib-web` | Custom build | `5000:8080` | Web aplikacija, zavisi od zdrave baze |

### Razlika: Docker Compose vs Produkcija

| Aspekt | Docker Compose (lokalno) | Render (produkcija) |
|--------|-------------------------|---------------------|
| **Baza** | MySQL kontejner u Docker-u | TiDB Cloud (eksterni) |
| **Cache** | In-memory (nema Redis-a) | Upstash Redis |
| **Email** | SMTP fallback ili log | Brevo HTTP API |
| **Port** | `5000` (mapiran na 8080) | `8080` (direktno) |
| **Env varijable** | Hardkodirane u compose | Postavljene na Render dashboardu |

---

## 7. Potrebne varijable i secrets

### GitHub Secrets (za CI/CD pipeline)

Postaviti u GitHub repozitoriju: **Settings → Secrets and variables → Actions → New repository secret**

| Secret | Opis | Kako se dobija |
|--------|------|----------------|
| `DOCKERHUB_USERNAME` | Docker Hub korisničko ime | Docker Hub → profil |
| `DOCKERHUB_TOKEN` | Docker Hub Access Token | Docker Hub → Account Settings → Security → New Access Token |
| `RENDER_DEPLOY_HOOK` | Render Deploy Hook URL | Render Dashboard → Service → Settings → Deploy Hook → Copy URL |

### Render Environment varijable (za produkcijsku aplikaciju)

Postaviti na Render dashboardu: **Service → Environment tab → Add Environment Variable**

| Varijabla | Opis | Potrebna za |
|-----------|------|-------------|
| `ConnectionStrings__DefaultConnection` | TiDB Cloud MySQL connection string | Konekcija na bazu |
| `ASPNETCORE_ENVIRONMENT` | Mora biti `Production` | ASP.NET Core konfiguracija |
| `EmailSettings__BrevoApiKey` | Brevo API ključ | Slanje emailova (primarni servis) |
| `EmailSettings__SenderEmail` | Email adresa pošiljaoca (`theofficialsmartlibrary@gmail.com`) | Brevo sender |
| `EmailSettings__SenderName` | Ime pošiljaoca (`SmartLib`) | Brevo sender name |
| `GOOGLE_BOOKS_API_KEY` | Google Books API ključ (`AIzaSyBAO54pQlK7WKNctuxmuwPtNFyogwKm178`) | Pretraga knjiga |
| `UPSTASH_REDIS_URL` | Redis TCP URL | Distributed cache (koristi se aktivno) |
| `UPSTASH_REDIS_PASSWORD` | Redis lozinka | Distributed cache (koristi se aktivno) |
| `UPSTASH_REDIS_REST_URL` | Redis REST URL | Upstash ga automatski generiše (trenutno se ne koristi u kodu) |
| `UPSTASH_REDIS_REST_TOKEN` | Redis REST token | Upstash ga automatski generiše (trenutno se ne koristi u kodu) |

> **Napomena o Redis varijablama:** Aplikacija koristi `StackExchange.Redis` paket koji komunicira sa Redisom putem standardnog **TCP protokola** (koristeći `UPSTASH_REDIS_URL` i `UPSTASH_REDIS_PASSWORD`). Upstash dashboard automatski generiše i REST API kredencijale (`REST_URL` i `REST_TOKEN`) koji su postavljeni na Render-u za kompletnost, ali se trenutno u kodu ne koriste. REST API se tipično koristi u serverless okruženjima (npr. Cloudflare Workers, Vercel Edge) gdje standardne TCP konekcije nisu dozvoljene.

### Lokalni `.env` fajl

Za lokalni razvoj, aplikacija čita `.env` fajl iz `Projekat/` direktorija. Fajl se već nalazi u repozitoriju ali je u `.gitignore`. Kompletni sadržaj sa svim varijablama:

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

> **Napomena:** `.env` fajl je u `.gitignore` — ne commituje se. Brevo API ključ se mora ručno dodati nakon kloniranja repozitorija.

---

## 8. Šta se tačno deploya

### Komponente sistema i gdje se hostuju

```
┌─────────────────────────────────────────────────────────────────┐
│                    SmartLib — Produkcijski sistem                │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────┐                            │
│  │   Render.com                    │                            │
│  │   ─────────────                │                            │
│  │   SmartLib.Web Docker kontejner │                            │
│  │   • ASP.NET Core 8 MVC         │                            │
│  │   • Razor Views (frontend)     │                            │
│  │   • Controllers (backend)      │                            │
│  │   • EF Core (ORM)             │                            │
│  │   • Port: 8080                 │                            │
│  │   • URL: smartlib-web.onrender.com                          │
│  └────────────┬──────┬───────┬────┘                            │
│               │      │       │                                  │
│       ┌───────▼──┐ ┌─▼────┐ ┌▼───────┐                         │
│       │ TiDB     │ │Redis │ │ Brevo  │                         │
│       │ Cloud    │ │Upstash│ │ API    │                         │
│       │ (MySQL)  │ │(Cache)│ │(Email) │                         │
│       └──────────┘ └──────┘ └────────┘                         │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Detaljni pregled deployanih komponenti

| Komponenta | Šta se deploya | Gdje | Kako |
|------------|----------------|------|------|
| **Backend + Frontend** | `SmartLib.Web` Docker image (sadrži kompajliran C# kod + Razor Views + CSS/JS) | Render.com | GitHub Actions → Docker Hub → Render |
| **Baza podataka** | MySQL shema + seed podaci | TiDB Cloud | Automatski pri pokretanju aplikacije (`EnsureCreated()` + `CREATE TABLE IF NOT EXISTS` + seed) |
| **Redis cache** | Konfiguracija putem env varijabli | Upstash | Manualno kreiran, aplikacija se automatski povezuje |
| **Email servis** | API ključ putem env varijable | Brevo | Manualno kreiran nalog, aplikacija koristi HTTP API |

### Šta GitHub Actions pipeline NE deploya (i zašto)

| Komponenta | Zašto se ne deploya u pipeline-u | Kako se postavlja |
|------------|----------------------------------|-------------------|
| **TiDB Cloud baza** | Managed servis — baza se kreira jednom i perzistira | Ručno: kreiran cluster na tidbcloud.com |
| **Upstash Redis** | Managed servis — Redis instanca se kreira jednom | Ručno: kreirana instanca na upstash.com |
| **Brevo email** | Eksterni SaaS — samo API ključ je potreban | Ručno: kreiran nalog na brevo.com |
| **Environment varijable** | Render čuva ih perzistentno — postavljaju se jednom | Ručno: postavljeno na Render dashboardu |

---

## 9. Povezivanje servisa

### Kako su servisi međusobno povezani

SmartLib je **monolitna MVC aplikacija** — frontend i backend su u istom projektu i deployaju se zajedno kao jedan Docker kontejner. Nema potrebe za odvojenim povezivanjem frontend-a sa backend API-jem jer su oni u istom procesu.

```
Browser ──HTTP──▶ Render (SmartLib.Web kontejner)
                      │
                      ├──TCP──▶ TiDB Cloud (MySQL, port 4000)
                      │         Connection string iz env varijable
                      │
                      ├──TCP──▶ Upstash Redis (port 6379, SSL)
                      │         Redis URL iz env varijable
                      │
                      └──HTTP──▶ Brevo API (https://api.brevo.com/v3/smtp/email)
                                 API ključ iz env varijable
```

### Detalji konekcija

#### 1. Web aplikacija ↔ MySQL baza (TiDB Cloud)

- **Protokol:** TCP (MySQL wire protocol)
- **Konfiguracija:** `ConnectionStrings__DefaultConnection` environment varijabla
- **Format:** `Server={host};Port={port};Database=smartlib;User={user};Password={pass};`
- **ORM:** Entity Framework Core sa Pomelo.EntityFrameworkCore.MySql providerom
- **Auto-konfiguracija:** Aplikacija pri pokretanju automatski kreira tabele i seed podatke

#### 2. Web aplikacija ↔ Upstash Redis

- **Protokol:** TCP sa SSL/TLS (`rediss://` shema)
- **Konfiguracija:** `UPSTASH_REDIS_URL` i `UPSTASH_REDIS_PASSWORD` env varijable
- **Klijent:** StackExchange.Redis putem `AddStackExchangeRedisCache()`
- **Fallback:** Ako Redis nije dostupan, koristi se In-Memory Distributed Cache
- **Health check:** Endpoint `/health/redis` za provjeru konekcije

#### 3. Web aplikacija ↔ Brevo Email API

- **Protokol:** HTTPS (REST API)
- **Endpoint:** `https://api.brevo.com/v3/smtp/email`
- **Konfiguracija:** `EmailSettings__BrevoApiKey` env varijabla
- **Autentifikacija:** API ključ u `api-key` HTTP header-u
- **Fallback:** Ako Brevo ne uspije → SMTP (lokalno) → Log (debug)

#### 4. Web aplikacija ↔ Google Books API

- **Protokol:** HTTPS (REST API)
- **Konfiguracija:** `GOOGLE_BOOKS_API_KEY` env varijabla
- **Namjena:** Pretraga i dohvat metapodataka o knjigama

---

## 10. Kako se pokreće deployment

### Automatski deployment (primarni tok)

**Korak 1:** Developer radi promjene na feature branch-u
```bash
git checkout -b feature/nova-funkcionalnost
# ... kod ...
git add .
git commit -m "Dodana nova funkcionalnost"
git push origin feature/nova-funkcionalnost
```

**Korak 2:** Developer otvara Pull Request prema `main` branch-u na GitHub-u

**Korak 3:** Nakon code review-a, PR se merge-uje u `main`

**Korak 4:** GitHub Actions automatski pokreće `deploy.yml` workflow:
1. ✅ Checkout repozitorija
2. ✅ Setup .NET 8 SDK
3. ✅ Restore NuGet paketa
4. ✅ Build solution-a
5. ✅ Pokretanje testova (unit, integracioni, sigurnosni) — **quality gate**
6. ✅ Login na Docker Hub
7. ✅ Build Docker image-a
8. ✅ Tag + Push na Docker Hub
9. ✅ Trigger Render Deploy Hook

> **Napomena:** Ako bilo koji test padne u koraku 5, workflow se zaustavlja i koraci 6-9 se ne izvršavaju.

**Korak 5:** Render automatski:
1. ✅ Pull-uje najnoviji image
2. ✅ Pokreće kontejner sa konfiguriranim env varijablama
3. ✅ Aplikacija se pokreće, kreira/ažurira bazu, seed-uje podatke
4. ✅ Servis je dostupan na https://smartlib-web.onrender.com/

### Ručni deployment (alternativa)

Ako je potrebno deployati bez GitHub Actions-a:

```bash
# 1. Build Docker image lokalno
cd Projekat
docker build -f src/SmartLib.Web/Dockerfile -t smartlib-web:latest .

# 2. Tag image za Docker Hub
docker tag smartlib-web:latest docker.io/VAŠE_DOCKERHUB_IME/smartlib-web:latest

# 3. Login na Docker Hub
docker login

# 4. Push image
docker push docker.io/VAŠE_DOCKERHUB_IME/smartlib-web:latest

# 5. Trigger Render deployment
curl -X POST "VAŠA_RENDER_DEPLOY_HOOK_URL"
```

### Lokalni deployment (Docker Compose)

Za pokretanje kompletnog sistema lokalno:

```bash
cd Projekat
docker-compose up --build
# Aplikacija dostupna na http://localhost:5000
```

---

## 11. Provjera uspješnosti deploymenta

### Automatske provjere

| Provjera | Kako | Očekivani rezultat |
|----------|------|--------------------|
| **GitHub Actions** | GitHub → Actions tab → najnoviji workflow run | Zeleni ✅ na svim koracima |
| **Docker Hub** | hub.docker.com → Repository → Tags | `latest` tag sa svježim datumom |
| **Render** | Render Dashboard → Service → Events | Status: "Deploy succeeded" |

### Ručne provjere nakon deploymenta

**1. Provjera da je aplikacija dostupna:**
```bash
curl -s -o /dev/null -w "%{http_code}" https://smartlib-web.onrender.com/
# Očekivani output: 200
```

**2. Provjera Redis konekcije:**
```bash
curl https://smartlib-web.onrender.com/health/redis
# Očekivani output: {"status":"Healthy","backend":"Upstash Redis","writeReadOk":true,...}
```

**3. Provjera stranice u browseru:**
- Otvoriti https://smartlib-web.onrender.com/
- Provjeriti da se početna stranica učitava
- Provjeriti da su knjige vidljive (seed podaci)
- Provjeriti da login forma radi

**4. Provjera logova na Render-u:**
- Render Dashboard → Service → Logs
- Tražiti:
  - `[Redis] Using Upstash Redis distributed cache` — Redis je povezan
  - `Email sent via Brevo to ...` — email servis radi (kada se šalje email)
  - Nema `Unhandled exception` poruka

### Indikatori neuspjelog deploymenta

| Simptom | Vjerovatni uzrok |
|---------|-----------------|
| GitHub Actions: ❌ crveni X na test koraku | Jedan ili više testova je pao — deployment je blokiran |
| GitHub Actions: ❌ crveni X na build/push koraku | Build error ili Docker Hub login failed |
| Render: "Deploy failed" | Aplikacija se crashuje pri pokretanju |
| 502 Bad Gateway | Kontejner radi ali aplikacija nije spremna |
| Stranica se učitava ali nema podataka | Baza nije dostupna ili connection string je pogrešan |

---

## 12. Ručni koraci (opravdanje)

Sljedeći koraci su ručni i izvršavaju se **samo jednom** pri inicijalnom postavljanju sistema. Nakon toga, svaki deployment je potpuno automatizovan.

### Jednokratni ručni koraci

| # | Korak | Obrazloženje | Kada se radi |
|---|-------|-------------|-------------|
| 1 | Kreiranje Docker Hub naloga i Access Token-a | Sigurnosni razlozi — tokeni se ne čuvaju u kodu | Jednom, pri postavljanju projekta |
| 2 | Kreiranje Render Web Service-a | Zahtijeva GUI interakciju sa Render platformom | Jednom, pri postavljanju projekta |
| 3 | Postavljanje GitHub Secrets | Sigurnosni razlozi — secrets se ne čuvaju u kodu | Jednom, pri postavljanju projekta |
| 4 | Kreiranje TiDB Cloud clustera | Zahtijeva GUI interakciju, kreiranje naloga | Jednom, pri postavljanju projekta |
| 5 | Kreiranje Upstash Redis instance | Zahtijeva GUI interakciju, kreiranje naloga | Jednom, pri postavljanju projekta |
| 6 | Kreiranje Brevo naloga i API ključa | Zahtijeva GUI interakciju, kreiranje naloga | Jednom, pri postavljanju projekta |
| 7 | Postavljanje Render environment varijabli | Sigurnosni razlozi — sadrže lozinke i API ključeve | Jednom, ili kada se mijenjaju kredencijali |

### Šta NIJE ručno

Sljedeće stvari su **potpuno automatizovane** i ne zahtijevaju ručnu intervenciju:

- ✅ Pokretanje automatskih testova (unit, integracioni, sigurnosni) kao quality gate
- ✅ Build aplikacije (backend + frontend)
- ✅ Kreiranje Docker image-a
- ✅ Push image-a na Docker Hub
- ✅ Deployment na Render
- ✅ Kreiranje/ažuriranje baze podataka (EnsureCreated + ALTER TABLE)
- ✅ Seed podataka (kategorije, knjige, primjerci)
- ✅ Povezivanje sa eksternim servisima (MySQL, Redis, Brevo)
- ✅ Health check i verifikacija

---

## 13. Ponovljivost deploymenta

### Kako demonstrirati ponovljiv deployment

**Scenarij 1: Novi deployment putem GitHub-a**

```bash
# 1. Napraviti bilo kakvu promjenu
echo "<!-- deploy test -->" >> Projekat/src/SmartLib.Web/Views/Shared/_Layout.cshtml

# 2. Commitovati i pushati
git add .
git commit -m "Test deployment"
git push origin main

# 3. Pratiti deployment:
#    - GitHub → Actions tab → workflow run
#    - Render Dashboard → Events
#    - https://smartlib-web.onrender.com/ → provjera
```

**Scenarij 2: Lokalni deployment od nule (Docker Compose)**

```bash
# 1. Klonirati repozitorij
git clone https://github.com/amilababic4/SI_grupa5.git
cd SI_grupa5/Projekat

# 2. Pokrenuti kompletni stack
docker-compose up --build

# 3. Otvoriti http://localhost:5000 — aplikacija je potpuno funkcionalna
#    Baza je kreirana, tabele postoje, podaci su seed-ovani
```

**Scenarij 3: Potpuna rekonstrukcija na novom Render nalogu**

1. Kreirati novi Render Web Service → Docker image: `docker.io/{username}/smartlib-web:latest`
2. Postaviti environment varijable (Sekcija 7)
3. Pokrenuti manual deploy ili pushati na `main`
4. Aplikacija automatski kreira bazu i seed-uje podatke

### Garancije ponovljivosti

| Garancija | Kako je ostvarena |
|-----------|-------------------|
| **Isti kod → isti build** | Dockerfile je deklarativan, `dotnet publish` je determinističan |
| **Isti image → isti runtime** | Docker image sadrži sve zavisnosti |
| **Ista baza pri svakom pokretanju** | `EnsureCreated()` + `CREATE TABLE IF NOT EXISTS` + idempotentni seed |
| **Iste konekcije** | Env varijable definišu sve eksterne konekcije |
| **Nema skrivenih zavisnosti** | Sve zavisnosti su u `.csproj` fajlovima (NuGet) i Dockerfile-u |

### Rollback procedura

Ako novi deployment ima probleme:

```bash
# Opcija A: Git revert + push (triggeruje novi deployment)
git revert HEAD
git push origin main

# Opcija B: Ručni rollback na Docker Hub-u
# Na Render dashboardu, promijeniti Docker image tag na prethodni
# (potrebno koristiti specifični tag umjesto :latest)

# Opcija C: Render Manual Deploy
# Render Dashboard → Service → Manual Deploy → odabrati prethodni commit
```

---

## 14. Produkcijski URL

Nakon uspješnog CD pipeline-a, aplikacija je dostupna na:

| Servis | URL | Opis |
|--------|-----|------|
| **Web aplikacija** | https://smartlib-web.onrender.com/ | Glavna stranica — login, katalog, upravljanje knjigama |
| **Redis Health Check** | https://smartlib-web.onrender.com/health/redis | Provjera konekcije sa Upstash Redis servisom |

---

## 15. Poznata ograničenja deploymenta

### Render Free Tier — spin down

Render Free Tier automatski gasi kontejner nakon **15 minuta neaktivnosti**. Prvi zahtjev nakon toga traje **30-60 sekundi** dok se kontejner ponovo pokrene (cold start). Ovo je ograničenje besplatnog plana i ne može se izbjeći bez nadogradnje na plaćeni plan.

**Posljedica:** Evaluator može doživjeti sporo učitavanje pri prvom pristupu. Svaki naredni zahtjev je normalne brzine.

### TiDB Cloud Serverless — connection pooling

TiDB Cloud Serverless ima ograničenje na broj istovremenih konekcija. U besplatnom planu, limit je dovoljan za razvojne svrhe ali nije za visok promet.

### Upstash Redis — mjesečni limit komandi

Upstash Free Tier dozvoljava 10.000 komandi dnevno / 500.000 mjesečno. Trenutna potrošnja (vidljivo na dashboardu: 472K/500K) je blizu limita. Ako se limit dostigne, cache operacije će failovati ali aplikacija nastavlja raditi koristeći In-Memory Cache fallback.

### Render — blokiran SMTP

Render blokira odlazne SMTP portove (25, 465, 587), zato se u produkciji koristi **Brevo HTTP API** umjesto SMTP-a za slanje emailova. SMTP radi samo u lokalnom razvoju.

### Docker image tag — samo `:latest`

Pipeline koristi samo `latest` tag za Docker image. Ovo znači da rollback na prethodni image zahtijeva git revert + novi deployment, jer se prethodni image prepiše. Za poboljšanje, mogao bi se koristiti commit SHA kao dodatni tag.

### Nema automatskog health check-a u pipeline-u

GitHub Actions workflow ne vrši eksplicitnu provjeru da li je aplikacija uspješno pokrenuta nakon Render Deploy Hook-a. Uspjeh deploymenta se provjerava ručno ili putem Render dashboarda.

---

## 16. Rješavanje čestih problema

### GitHub Actions workflow pada na test koraku

**Simptom:** ❌ crveni X na "Run tests" koraku.

**Rješenje:**
1. Otvoriti GitHub → Actions → kliknuti na failani workflow run
2. Proširiti "Run tests" korak i pročitati koji test je pao
3. Lokalno pokrenuti iste testove: `dotnet test Projekat/SmartLib.sln --filter "FullyQualifiedName!~UiTests"`
4. Ispraviti kod i ponovo pushati

### GitHub Actions workflow pada na Docker build koraku

**Simptom:** ❌ na "Build image" ili "Push image" koraku.

**Rješenje:**
- Provjeriti da su `DOCKERHUB_USERNAME` i `DOCKERHUB_TOKEN` ispravno postavljeni u GitHub Secrets
- Provjeriti da Docker Hub Access Token nije istekao
- Lokalno testirati build: `docker build -f Projekat/src/SmartLib.Web/Dockerfile -t test Projekat`

### Aplikacija se učitava ali nema podataka (prazne stranice)

**Simptom:** Stranica se otvara ali nema knjiga, kategorija, itd.

**Rješenje:**
- Provjeriti `ConnectionStrings__DefaultConnection` na Render dashboardu
- Provjeriti Render logove za database connection greške
- Provjeriti da TiDB Cloud cluster nije pauziran (Serverless se automatski pauzira nakon dužeg perioda neaktivnosti)

### 502 Bad Gateway na Render-u

**Simptom:** Stranica prikazuje Render 502 error.

**Rješenje:**
- Kontejner se pokreće ali aplikacija nije spremna — sačekati 30-60 sekundi (cold start)
- Ako se ne oporavi — provjeriti Render logove za crash poruke
- Provjeriti da environment varijable nisu pogrešno formatirane (posebno connection string — ne smije imati extra razmake ili navodnike)

### Redis health check vraća "Unhealthy"

**Simptom:** `/health/redis` vraća status "Unhealthy".

**Rješenje:**
- Provjeriti `UPSTASH_REDIS_URL` i `UPSTASH_REDIS_PASSWORD` na Render dashboardu
- Provjeriti da Upstash instanca nije obrisana ili pauzirana
- Aplikacija nastavlja raditi — koristi In-Memory Cache kao fallback

### Email se ne šalje u produkciji

**Simptom:** Registracija i ostale email akcije ne šalju email.

**Rješenje:**
- Provjeriti `EmailSettings__BrevoApiKey` na Render dashboardu
- Provjeriti Brevo dashboard za kvote i status naloga
- Provjeriti Render logove za "Brevo API returned" poruke sa error statusom

---

*Dokument pripremila: SI Grupa 5, ETF Sarajevo*
*Datum: Juni 2026.*
*Verzija: 2.0*
