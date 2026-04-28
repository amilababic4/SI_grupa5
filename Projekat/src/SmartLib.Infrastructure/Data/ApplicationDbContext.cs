using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Models;

namespace SmartLib.Infrastructure.Data
{
    /// <summary>
    /// Placeholder za bazni data sloj.
    /// Projekat trenutno koristi jednostavan file-based storage preko SmartLibDataStore.
    /// </summary>
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Korisnik> Korisnici => Set<Korisnik>();
        public DbSet<Uloga> Uloge => Set<Uloga>();
        public DbSet<Knjiga> Knjige => Set<Knjiga>();
        public DbSet<Kategorija> Kategorije => Set<Kategorija>();
        public DbSet<Primjerak> Primjerci => Set<Primjerak>();
        public DbSet<Zaduzenje> Zaduzenja => Set<Zaduzenje>();
        public DbSet<Clanarina> Clanarine => Set<Clanarina>();
        public DbSet<Rezervacija> Rezervacije => Set<Rezervacija>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ─── ULOGA ───────────────────────────────────────────────────────────
            modelBuilder.Entity<Uloga>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.Naziv).IsRequired().HasMaxLength(50);
                e.Property(u => u.Opis).HasMaxLength(500);
                e.HasIndex(u => u.Naziv).IsUnique();

                e.HasData(
                    new Uloga { Id = 1, Naziv = "Član", Opis = "Član biblioteke" },
                    new Uloga { Id = 2, Naziv = "Bibliotekar", Opis = "Bibliotečko osoblje" },
                    new Uloga { Id = 3, Naziv = "Administrator", Opis = "Sistem administrator" }
                );
            });

            // ─── KORISNIK ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Korisnik>(e =>
            {
                e.HasKey(k => k.Id);
                e.Property(k => k.Ime).IsRequired().HasMaxLength(100);
                e.Property(k => k.Prezime).IsRequired().HasMaxLength(100);
                e.Property(k => k.Email).IsRequired().HasMaxLength(200);
                e.Property(k => k.LozinkaHash).IsRequired();
                e.Property(k => k.Status).IsRequired().HasDefaultValue("aktivan");
                e.Property(k => k.DatumKreiranja).IsRequired();
                e.HasIndex(k => k.Email).IsUnique();

                e.HasOne(k => k.Uloga)
                 .WithMany(u => u.Korisnici)
                 .HasForeignKey(k => k.UlogaId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasData(
                    new Korisnik
                    {
                        Id = 1,
                        Ime = "Admin",
                        Prezime = "SmartLib",
                        Email = "admin@smartlib.ba",
                        LozinkaHash = "R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=",
                        UlogaId = 3,
                        Status = "aktivan",
                        DatumKreiranja = new DateTime(2026, 4, 25, 9, 18, 44, DateTimeKind.Utc)
                    },
                    new Korisnik
                    {
                        Id = 2,
                        Ime = "Bibliotekar",
                        Prezime = "SmartLib",
                        Email = "bibliotekar@smartlib.ba",
                        LozinkaHash = "R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=",
                        UlogaId = 2,
                        Status = "aktivan",
                        DatumKreiranja = new DateTime(2026, 4, 25, 9, 18, 44, DateTimeKind.Utc)
                    },
                    new Korisnik
                    {
                        Id = 3,
                        Ime = "Test",
                        Prezime = "Clan",
                        Email = "clan@smartlib.ba",
                        LozinkaHash = "R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=",
                        UlogaId = 1,
                        Status = "aktivan",
                        DatumKreiranja = new DateTime(2026, 4, 25, 9, 18, 44, DateTimeKind.Utc)
                    },
                    new Korisnik
                    {
                        Id = 4,
                        Ime = "Amila",
                        Prezime = "Babic",
                        Email = "proba@smartlib",
                        LozinkaHash = "RXc8HNFmlRF+0H6XsJGvjg==:+Fi3Qst3q84y5t6TNlJd9zltFZgqyYHErKm+CbKRTXY=",
                        UlogaId = 1,
                        Status = "aktivan",
                        DatumKreiranja = new DateTime(2026, 4, 25, 20, 47, 0, DateTimeKind.Utc)
                    },
                    new Korisnik
                    {
                        Id = 5,
                        Ime = "Ime",
                        Prezime = "Prezime",
                        Email = "imeprezime@gmail.com",
                        LozinkaHash = "eyWukOsw1/s1sYE7WkYNVg==:fdqafu1WAN62dAwt9lyR/GSSq05y9h7PaTebwSsK4Tc=",
                        UlogaId = 1,
                        Status = "aktivan",
                        DatumKreiranja = new DateTime(2026, 4, 26, 15, 4, 48, DateTimeKind.Utc)
                    },
                    new Korisnik
                    {
                        Id = 6,
                        Ime = "Imran",
                        Prezime = "Vlajcic",
                        Email = "ivlajcic1@etf.unsa.ba",
                        LozinkaHash = "v1OSV7uzmOkIb4n89gxqVQ==:ghU7xePMJ4TLW/UIySZEozPekQ+BnAyfoUgUXzU3l0k=",
                        UlogaId = 1,
                        Status = "aktivan",
                        DatumKreiranja = new DateTime(2026, 4, 26, 21, 54, 8, DateTimeKind.Utc)
                    }
                );
            });

            // ─── KATEGORIJA ───────────────────────────────────────────────────────
            modelBuilder.Entity<Kategorija>(e =>
            {
                e.HasKey(k => k.Id);
                e.Property(k => k.Naziv).IsRequired().HasMaxLength(100);
                e.Property(k => k.Opis).HasMaxLength(500);
                e.HasIndex(k => k.Naziv).IsUnique();
            });

            // ─── KNJIGA ───────────────────────────────────────────────────────────
            modelBuilder.Entity<Knjiga>(e =>
            {
                e.HasKey(k => k.Id);
                e.Property(k => k.Naslov).IsRequired().HasMaxLength(300);
                e.Property(k => k.Autor).IsRequired().HasMaxLength(200);
                e.Property(k => k.Isbn).HasMaxLength(20);
                e.Property(k => k.Izdavac).HasMaxLength(200);
                e.Property(k => k.GodinaIzdanja).IsRequired();
                e.HasIndex(k => k.Isbn).IsUnique().HasFilter("\"Isbn\" IS NOT NULL");

                e.HasOne(k => k.Kategorija)
                 .WithMany(kat => kat.Knjige)
                 .HasForeignKey(k => k.KategorijaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── PRIMJERAK ────────────────────────────────────────────────────────
            modelBuilder.Entity<Primjerak>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.InventarniBroj).IsRequired().HasMaxLength(50);
                e.Property(p => p.Status).IsRequired().HasDefaultValue("dostupan");
                e.Property(p => p.DatumNabave);
                e.HasIndex(p => p.InventarniBroj).IsUnique();

                e.HasOne(p => p.Knjiga)
                 .WithMany(k => k.Primjerci)
                 .HasForeignKey(p => p.KnjigaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── ZADUZENJE ────────────────────────────────────────────────────────
            modelBuilder.Entity<Zaduzenje>(e =>
            {
                e.HasKey(z => z.Id);
                e.Property(z => z.DatumZaduzivanja).IsRequired();
                e.Property(z => z.DatumPlaniranogVracanja).IsRequired();
                e.Property(z => z.DatumStvarnogVracanja);
                e.Property(z => z.Status).IsRequired().HasDefaultValue("aktivno");

                e.HasOne(z => z.Korisnik)
                 .WithMany(k => k.Zaduzenja)
                 .HasForeignKey(z => z.KorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(z => z.Primjerak)
                 .WithMany(p => p.Zaduzenja)
                 .HasForeignKey(z => z.PrimjerakId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── CLANARINA ────────────────────────────────────────────────────────
            modelBuilder.Entity<Clanarina>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.DatumPocetka).IsRequired();
                e.Property(c => c.DatumIsteka).IsRequired();
                e.Property(c => c.Status).IsRequired().HasDefaultValue("aktivna");

                e.HasOne(c => c.Korisnik)
                 .WithMany(k => k.Clanarine)
                 .HasForeignKey(c => c.KorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── REZERVACIJA ──────────────────────────────────────────────────────
            modelBuilder.Entity<Rezervacija>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.DatumRezervacije).IsRequired();
                e.Property(r => r.DatumIsteka).IsRequired();
                e.Property(r => r.Status).IsRequired().HasDefaultValue("aktivna");

                e.HasOne(r => r.Korisnik)
                 .WithMany(k => k.Rezervacije)
                 .HasForeignKey(r => r.KorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Knjiga)
                 .WithMany(k => k.Rezervacije)
                 .HasForeignKey(r => r.KnjigaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── AUDIT LOG ────────────────────────────────────────────────────────
            modelBuilder.Entity<AuditLog>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.Akcija).IsRequired().HasMaxLength(100);
                e.Property(a => a.EntitetTip).IsRequired().HasMaxLength(100);
                e.Property(a => a.DatumAkcije).IsRequired();
                // JSONB kolone za PostgreSQL
                e.Property(a => a.VrijednostiPrije).HasColumnType("jsonb");
                e.Property(a => a.VrijednostiNakon).HasColumnType("jsonb");

                // KorisnikId je opcionalan (nullable FK)
                e.HasOne(a => a.Korisnik)
                 .WithMany()
                 .HasForeignKey(a => a.KorisnikId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
