# SmartLib — Bibliotečki Informacioni Sistem

## Opis projekta
SmartLib je web aplikacija za upravljanje bibliotečkim fondom. Sistem omogućava bibliotečkom osoblju centralizovano upravljanje knjigama, članovima i zaduženjima, dok članovima pruža uvid u dostupnost literature i status članarine.

## Tehnologije
- **Jezik:** C# 12 (cijeli stack)
- **Web Framework:** ASP.NET Core 8 MVC
- **Frontend:** Razor Views (.cshtml) + Tag Helpers
- **ORM:** Entity Framework Core 8
- **Baza podataka:** PostgreSQL 16
- **Autentifikacija:** Cookie-based (ASP.NET Core Identity)

## Struktura projekta
```
Projekat/
├── src/
│   ├── SmartLib.Web/            # ASP.NET Core MVC (Controllers + Razor Views)
│   ├── SmartLib.API/            # REST API kontroleri (opciono)
│   ├── SmartLib.Core/           # Domain modeli, interfejsi, DTO-ovi
│   └── SmartLib.Infrastructure/ # EF Core DbContext, repozitoriji, migracije
├── tests/
│   └── SmartLib.Tests/          # xUnit testovi
├── docker-compose.yml
└── SmartLib.sln
```

## Pokretanje

### Pristupni link
- https://smartlib-web.onrender.com/

### Preduvjeti
- .NET 8 SDK
- PostgreSQL 16
- Git

### Lokalno pokretanje
```bash
cd src/SmartLib.Web
dotnet ef database update --project ../SmartLib.Infrastructure
dotnet run
```

### Docker
```bash
docker-compose up --build
```

## Branching strategija
Koristimo **GitHub Flow**. Detalji u `Sprint4/BranchingStrategy.md`.

## Tim
SI Grupa 5 — ETF Sarajevo
