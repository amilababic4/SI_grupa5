using System.Text.Json;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Security;

namespace SmartLib.Infrastructure.Data
{
    internal sealed class SmartLibDataSnapshot
    {
        public int NextKorisnikId { get; set; } = 1;
        public List<StoredUloga> Uloge { get; set; } = new();
        public List<StoredKorisnik> Korisnici { get; set; } = new();
    }

    internal sealed class StoredUloga
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string? Opis { get; set; }
    }

    internal sealed class StoredKorisnik
    {
        public int Id { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LozinkaHash { get; set; } = string.Empty;
        public int UlogaId { get; set; }
        public string Status { get; set; } = "aktivan";
        public DateTime DatumKreiranja { get; set; }
    }

    internal static class SmartLibDataStore
    {
        private static readonly object SyncRoot = new();
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        private static SmartLibDataSnapshot? _snapshot;

        public static IReadOnlyList<Uloga> GetUloge()
        {
            lock (SyncRoot)
            {
                return LoadSnapshot().Uloge.Select(u => MapToModel(u)).ToList();
            }
        }

        public static IReadOnlyList<Korisnik> GetKorisnici()
        {
            lock (SyncRoot)
            {
                var snapshot = LoadSnapshot();
                return snapshot.Korisnici.Select(k => MapToModel(k, snapshot)).ToList();
            }
        }

        public static Korisnik? GetKorisnikById(int id)
        {
            lock (SyncRoot)
            {
                var snapshot = LoadSnapshot();
                return snapshot.Korisnici
                    .Where(k => k.Id == id)
                    .Select(k => MapToModel(k, snapshot))
                    .FirstOrDefault();
            }
        }

        public static Korisnik? GetKorisnikByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var normalizedEmail = NormalizeEmail(email);

            lock (SyncRoot)
            {
                var snapshot = LoadSnapshot();
                return snapshot.Korisnici
                    .Where(k => NormalizeEmail(k.Email) == normalizedEmail)
                    .Select(k => MapToModel(k, snapshot))
                    .FirstOrDefault();
            }
        }

        public static Korisnik AddKorisnik(Korisnik korisnik)
        {
            ArgumentNullException.ThrowIfNull(korisnik);

            lock (SyncRoot)
            {
                var snapshot = LoadSnapshot();
                var normalizedEmail = NormalizeEmail(korisnik.Email);

                if (snapshot.Korisnici.Any(k => NormalizeEmail(k.Email) == normalizedEmail))
                {
                    throw new InvalidOperationException("Korisnik sa tom email adresom već postoji.");
                }

                var storedKorisnik = new StoredKorisnik
                {
                    Id = snapshot.NextKorisnikId++,
                    Ime = korisnik.Ime.Trim(),
                    Prezime = korisnik.Prezime.Trim(),
                    Email = normalizedEmail,
                    LozinkaHash = korisnik.LozinkaHash,
                    UlogaId = korisnik.UlogaId,
                    Status = string.IsNullOrWhiteSpace(korisnik.Status) ? "aktivan" : korisnik.Status,
                    DatumKreiranja = korisnik.DatumKreiranja == default ? DateTime.UtcNow : korisnik.DatumKreiranja
                };

                snapshot.Korisnici.Add(storedKorisnik);
                SaveSnapshot(snapshot);

                return MapToModel(storedKorisnik, snapshot);
            }
        }

        public static void UpdateKorisnik(Korisnik korisnik)
        {
            ArgumentNullException.ThrowIfNull(korisnik);

            lock (SyncRoot)
            {
                var snapshot = LoadSnapshot();
                var existing = snapshot.Korisnici.FirstOrDefault(k => k.Id == korisnik.Id);

                if (existing is null)
                {
                    throw new KeyNotFoundException($"Korisnik s ID-jem {korisnik.Id} nije pronađen.");
                }

                existing.Ime = korisnik.Ime.Trim();
                existing.Prezime = korisnik.Prezime.Trim();
                existing.Email = NormalizeEmail(korisnik.Email);
                existing.LozinkaHash = korisnik.LozinkaHash;
                existing.UlogaId = korisnik.UlogaId;
                existing.Status = korisnik.Status;
                existing.DatumKreiranja = korisnik.DatumKreiranja;

                SaveSnapshot(snapshot);
            }
        }

        public static Uloga? GetUlogaByName(string naziv)
        {
            lock (SyncRoot)
            {
                return LoadSnapshot().Uloge
                    .FirstOrDefault(u => string.Equals(u.Naziv, naziv, StringComparison.OrdinalIgnoreCase))
                    is StoredUloga storedUloga
                        ? MapToModel(storedUloga)
                        : null;
            }
        }

        private static SmartLibDataSnapshot LoadSnapshot()
        {
            if (_snapshot is not null)
            {
                return _snapshot;
            }

            var dataFilePath = ResolveDataFilePath();
            if (File.Exists(dataFilePath))
            {
                var json = File.ReadAllText(dataFilePath);
                _snapshot = JsonSerializer.Deserialize<SmartLibDataSnapshot>(json, JsonOptions) ?? CreateSeedSnapshot();
            }
            else
            {
                _snapshot = CreateSeedSnapshot();
                SaveSnapshot(_snapshot);
            }

            EnsureSeedData(_snapshot);
            SaveSnapshot(_snapshot);
            return _snapshot;
        }

        private static void SaveSnapshot(SmartLibDataSnapshot snapshot)
        {
            var dataFilePath = ResolveDataFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(dataFilePath)!);
            var json = JsonSerializer.Serialize(snapshot, JsonOptions);
            File.WriteAllText(dataFilePath, json);
            _snapshot = snapshot;
        }

        private static SmartLibDataSnapshot CreateSeedSnapshot()
        {
            var adminPasswordHash = PasswordHasher.HashPassword("Password123!");

            return new SmartLibDataSnapshot
            {
                NextKorisnikId = 4,
                Uloge = new List<StoredUloga>
                {
                    new() { Id = 1, Naziv = RoleNames.Clan, Opis = "Član biblioteke" },
                    new() { Id = 2, Naziv = RoleNames.Bibliotekar, Opis = "Bibliotečko osoblje" },
                    new() { Id = 3, Naziv = RoleNames.Administrator, Opis = "Sistem administrator" }
                },
                Korisnici = new List<StoredKorisnik>
                {
                    new()
                    {
                        Id = 1,
                        Ime = "Admin",
                        Prezime = "SmartLib",
                        Email = "admin@smartlib.ba",
                        LozinkaHash = adminPasswordHash,
                        UlogaId = 3,
                        Status = "aktivan",
                        DatumKreiranja = DateTime.UtcNow
                    },
                    new()
                    {
                        Id = 2,
                        Ime = "Bibliotekar",
                        Prezime = "SmartLib",
                        Email = "bibliotekar@smartlib.ba",
                        LozinkaHash = adminPasswordHash,
                        UlogaId = 2,
                        Status = "aktivan",
                        DatumKreiranja = DateTime.UtcNow
                    },
                    new()
                    {
                        Id = 3,
                        Ime = "Test",
                        Prezime = "Clan",
                        Email = "clan@smartlib.ba",
                        LozinkaHash = adminPasswordHash,
                        UlogaId = 1,
                        Status = "aktivan",
                        DatumKreiranja = DateTime.UtcNow
                    }
                }
            };
        }

        private static void EnsureSeedData(SmartLibDataSnapshot snapshot)
        {
            if (!snapshot.Uloge.Any())
            {
                snapshot.Uloge = CreateSeedSnapshot().Uloge;
            }

            if (!snapshot.Korisnici.Any())
            {
                snapshot.Korisnici = CreateSeedSnapshot().Korisnici;
            }

            snapshot.NextKorisnikId = Math.Max(snapshot.NextKorisnikId, snapshot.Korisnici.Any() ? snapshot.Korisnici.Max(k => k.Id) + 1 : 1);
        }

        private static string ResolveDataFilePath()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "SmartLib.sln")))
                {
                    return Path.Combine(directory.FullName, "src", "SmartLib.Infrastructure", "Data", "smartlib-data.json");
                }

                directory = directory.Parent;
            }

            return Path.Combine(AppContext.BaseDirectory, "smartlib-data.json");
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        private static Uloga MapToModel(StoredUloga storedUloga)
        {
            return new Uloga
            {
                Id = storedUloga.Id,
                Naziv = storedUloga.Naziv,
                Opis = storedUloga.Opis
            };
        }

        private static Korisnik MapToModel(StoredKorisnik storedKorisnik, SmartLibDataSnapshot snapshot)
        {
            var uloga = snapshot.Uloge.FirstOrDefault(u => u.Id == storedKorisnik.UlogaId);

            return new Korisnik
            {
                Id = storedKorisnik.Id,
                Ime = storedKorisnik.Ime,
                Prezime = storedKorisnik.Prezime,
                Email = storedKorisnik.Email,
                LozinkaHash = storedKorisnik.LozinkaHash,
                UlogaId = storedKorisnik.UlogaId,
                Status = storedKorisnik.Status,
                DatumKreiranja = storedKorisnik.DatumKreiranja,
                Uloga = uloga is null ? null : MapToModel(uloga)
            };
        }
    }
}