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
        public DbSet<ForumObjava> ForumObjave => Set<ForumObjava>();
        public DbSet<ForumKomentar> ForumKomentari => Set<ForumKomentar>();
        public DbSet<ForumReakcija> ForumReakcije => Set<ForumReakcija>();
        public DbSet<ForumKomentarPrijava> ForumKomentarPrijave => Set<ForumKomentarPrijava>();
        public DbSet<ForumObjavaPrijava> ForumObjavaPrijave => Set<ForumObjavaPrijava>();
        public DbSet<Recenzija> Recenzije => Set<Recenzija>();
        public DbSet<RecenzijaPrijava> RecenzijaPrijave => Set<RecenzijaPrijava>();
        public DbSet<Notifikacija> Notifikacije => Set<Notifikacija>();
        public DbSet<Vijest> Vijesti => Set<Vijest>();
        public DbSet<Dogadjaj> Dogadjaji => Set<Dogadjaj>();
        public DbSet<ListaZeljaStavka> ListaZeljaStavke => Set<ListaZeljaStavka>();
        public DbSet<ListaKolekcija> ListaKolekcije => Set<ListaKolekcija>();
        public DbSet<ListaKolekcijaStavka> ListaKolekcijaStavke => Set<ListaKolekcijaStavka>();
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
                e.Property(k => k.ResetToken).HasMaxLength(256);
                e.Property(k => k.ResetTokenExpiry);
                e.Property(k => k.Status).IsRequired().HasDefaultValue("aktivan");
                e.Property(k => k.ListaZeljaJavna).IsRequired().HasDefaultValue(false);
                e.Property(k => k.DatumKreiranja).IsRequired();
                e.Property(k => k.DatumDeaktivacije);
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
                e.HasIndex(k => k.Isbn).IsUnique();

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

            // ─── LISTA ZELJA ───────────────────────────────────────────────────
            modelBuilder.Entity<ListaZeljaStavka>(e =>
            {
                e.HasKey(l => l.Id);
                e.Property(l => l.DatumDodavanja).IsRequired();
                e.HasIndex(l => new { l.KorisnikId, l.KnjigaId }).IsUnique();

                e.HasOne(l => l.Korisnik)
                 .WithMany(k => k.ListaZelja)
                 .HasForeignKey(l => l.KorisnikId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(l => l.Knjiga)
                 .WithMany(k => k.ListaZeljaStavke)
                 .HasForeignKey(l => l.KnjigaId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── LISTA KOLEKCIJA ───────────────────────────────────────────────
            modelBuilder.Entity<ListaKolekcija>(e =>
            {
                e.HasKey(l => l.Id);
                e.Property(l => l.Naziv).IsRequired().HasMaxLength(200);
                e.Property(l => l.Opis).HasMaxLength(1000);
                e.Property(l => l.Javna).IsRequired().HasDefaultValue(false);
                e.Property(l => l.DatumKreiranja).IsRequired();
                e.Property(l => l.DatumAzuriranja);
                e.HasIndex(l => new { l.KorisnikId, l.Naziv }).IsUnique();

                e.HasOne(l => l.Korisnik)
                 .WithMany()
                 .HasForeignKey(l => l.KorisnikId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ListaKolekcijaStavka>(e =>
            {
                e.HasKey(s => s.Id);
                e.Property(s => s.DatumDodavanja).IsRequired();
                e.HasIndex(s => new { s.ListaKolekcijaId, s.KnjigaId }).IsUnique();

                e.HasOne(s => s.ListaKolekcija)
                 .WithMany(l => l.Stavke)
                 .HasForeignKey(s => s.ListaKolekcijaId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(s => s.Knjiga)
                 .WithMany()
                 .HasForeignKey(s => s.KnjigaId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── AUDIT LOG ────────────────────────────────────────────────────────
            modelBuilder.Entity<AuditLog>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.Akcija).IsRequired().HasMaxLength(100);
                e.Property(a => a.EntitetTip).IsRequired().HasMaxLength(100);
                e.Property(a => a.DatumAkcije).IsRequired();
                e.Property(a => a.VrijednostiPrije).HasColumnType("longtext");
                e.Property(a => a.VrijednostiNakon).HasColumnType("longtext");

                // KorisnikId je opcionalan (nullable FK)
                e.HasOne(a => a.Korisnik)
                 .WithMany()
                 .HasForeignKey(a => a.KorisnikId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ─── FORUM OBJAVA ─────────────────────────────────────────────────────
            modelBuilder.Entity<ForumObjava>(e =>
            {
                e.HasKey(o => o.Id);
                e.Property(o => o.Naslov).IsRequired().HasMaxLength(200);
                e.Property(o => o.Sadrzaj).IsRequired().HasMaxLength(5000);
                e.Property(o => o.Kategorija).IsRequired().HasMaxLength(100);
                e.Property(o => o.DatumKreiranja).IsRequired();

                e.HasOne(o => o.Korisnik)
                 .WithMany()
                 .HasForeignKey(o => o.KorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── FORUM KOMENTAR ───────────────────────────────────────────────────
            modelBuilder.Entity<ForumKomentar>(e =>
            {
                e.HasKey(k => k.Id);
                e.Property(k => k.Sadrzaj).IsRequired().HasMaxLength(2000);
                e.Property(k => k.DatumKreiranja).IsRequired();

                e.HasOne(k => k.Objava)
                 .WithMany(o => o.Komentari)
                 .HasForeignKey(k => k.ObjavaId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(k => k.Korisnik)
                 .WithMany()
                 .HasForeignKey(k => k.KorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── FORUM REAKCIJA ───────────────────────────────────────────────────
            modelBuilder.Entity<ForumReakcija>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Tip).IsRequired().HasMaxLength(50);
                e.Property(r => r.DatumKreiranja).IsRequired();

                e.HasOne(r => r.Objava)
                 .WithMany(o => o.Reakcije)
                 .HasForeignKey(r => r.ObjavaId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(r => r.Korisnik)
                 .WithMany()
                 .HasForeignKey(r => r.KorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── FORUM KOMENTAR PRIJAVA ────────────────────────────────────────
            modelBuilder.Entity<ForumKomentarPrijava>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Razlog).HasMaxLength(500);
                e.Property(p => p.Status).IsRequired().HasMaxLength(30).HasDefaultValue("otvorena");
                e.Property(p => p.DatumKreiranja).IsRequired();
                e.Property(p => p.DatumRazrjesenja);

                e.HasOne(p => p.Komentar)
                 .WithMany()
                 .HasForeignKey(p => p.KomentarId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(p => p.PrijavioKorisnik)
                 .WithMany()
                 .HasForeignKey(p => p.PrijavioKorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.RazrijesioKorisnik)
                 .WithMany()
                 .HasForeignKey(p => p.RazrijesioKorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(p => new { p.KomentarId, p.PrijavioKorisnikId }).IsUnique();
            });

            // ─── FORUM OBJAVA PRIJAVA ─────────────────────────────────────────
            modelBuilder.Entity<ForumObjavaPrijava>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Razlog).HasMaxLength(500);
                e.Property(p => p.Status).IsRequired().HasMaxLength(30).HasDefaultValue("otvorena");
                e.Property(p => p.DatumKreiranja).IsRequired();
                e.Property(p => p.DatumRazrjesenja);

                e.HasOne(p => p.Objava)
                 .WithMany()
                 .HasForeignKey(p => p.ObjavaId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(p => p.PrijavioKorisnik)
                 .WithMany()
                 .HasForeignKey(p => p.PrijavioKorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.RazrijesioKorisnik)
                 .WithMany()
                 .HasForeignKey(p => p.RazrijesioKorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(p => new { p.ObjavaId, p.PrijavioKorisnikId }).IsUnique();
            });

            // ─── RECENZIJA ────────────────────────────────────────────────────────
            modelBuilder.Entity<Recenzija>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Ocjena).IsRequired();
                e.Property(r => r.Komentar).HasMaxLength(2000);
                e.Property(r => r.DatumKreiranja).IsRequired();

                e.HasOne(r => r.Knjiga)
                 .WithMany(k => k.Recenzije)
                 .HasForeignKey(r => r.KnjigaId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(r => r.Korisnik)
                 .WithMany()
                 .HasForeignKey(r => r.KorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── RECENZIJA PRIJAVA ─────────────────────────────────────────────
            modelBuilder.Entity<RecenzijaPrijava>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Razlog).HasMaxLength(500);
                e.Property(p => p.Status).IsRequired().HasMaxLength(30).HasDefaultValue("otvorena");
                e.Property(p => p.DatumKreiranja).IsRequired();
                e.Property(p => p.DatumRazrjesenja);

                e.HasOne(p => p.Recenzija)
                 .WithMany()
                 .HasForeignKey(p => p.RecenzijaId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(p => p.PrijavioKorisnik)
                 .WithMany()
                 .HasForeignKey(p => p.PrijavioKorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.RazrijesioKorisnik)
                 .WithMany()
                 .HasForeignKey(p => p.RazrijesioKorisnikId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(p => new { p.RecenzijaId, p.PrijavioKorisnikId }).IsUnique();
            });

            // ─── NOTIFIKACIJA ──────────────────────────────────────────────────
            modelBuilder.Entity<Notifikacija>(e =>
            {
                e.HasKey(n => n.Id);
                e.Property(n => n.Naslov).IsRequired().HasMaxLength(200);
                e.Property(n => n.Poruka).IsRequired().HasColumnType("longtext");
                e.Property(n => n.Tip).IsRequired().HasMaxLength(50).HasDefaultValue("Sistem");
                e.Property(n => n.LinkUrl).HasMaxLength(512);
                e.Property(n => n.Procitano).IsRequired().HasDefaultValue(false);
                e.Property(n => n.DatumKreiranja).IsRequired();

                e.HasOne(n => n.Korisnik)
                 .WithMany()
                 .HasForeignKey(n => n.KorisnikId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(n => new { n.KorisnikId, n.Procitano });
            });

            // ─── VIJEST ───────────────────────────────────────────────────────────
            modelBuilder.Entity<Vijest>(e =>
            {
                e.HasKey(v => v.Id);
                e.Property(v => v.Naslov).IsRequired().HasMaxLength(300);
                e.Property(v => v.Sadrzaj).IsRequired().HasColumnType("longtext");
                e.Property(v => v.Kategorija).IsRequired().HasMaxLength(100);
                e.Property(v => v.SlikaUrl).HasMaxLength(512);
                e.Property(v => v.DatumObjave).IsRequired();

                e.HasOne(v => v.Autor)
                 .WithMany()
                 .HasForeignKey(v => v.AutorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── DOGADJAJ ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Dogadjaj>(e =>
            {
                e.HasKey(d => d.Id);
                e.Property(d => d.Naslov).IsRequired().HasMaxLength(300);
                e.Property(d => d.Opis).HasMaxLength(2000);
                e.Property(d => d.Datum).IsRequired();
                e.Property(d => d.Sat).HasMaxLength(20);
                e.Property(d => d.Lokacija).HasMaxLength(200);
                e.Property(d => d.Kategorija).IsRequired().HasMaxLength(100);

                e.HasOne(d => d.Autor)
                 .WithMany()
                 .HasForeignKey(d => d.AutorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
