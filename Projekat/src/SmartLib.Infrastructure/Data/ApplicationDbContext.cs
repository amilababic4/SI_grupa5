// TODO: Dodati using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Models;

namespace SmartLib.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework Core DbContext za SmartLib.
    /// Konfiguriše sve entitete, relacije i ograničenja.
    /// </summary>
    public class ApplicationDbContext // TODO: : DbContext
    {
        // TODO: Dodati DbSet svojstva za sve entitete
        // public DbSet<Uloga> Uloge { get; set; }
        // public DbSet<Korisnik> Korisnici { get; set; }
        // public DbSet<Kategorija> Kategorije { get; set; }
        // public DbSet<Knjiga> Knjige { get; set; }
        // public DbSet<Primjerak> Primjerci { get; set; }
        // public DbSet<Clanarina> Clanarine { get; set; }
        // public DbSet<Zaduzenje> Zaduzenja { get; set; }
        // public DbSet<Rezervacija> Rezervacije { get; set; }
        // public DbSet<AuditLog> AuditLogovi { get; set; }

        // TODO: Konfiguracija u OnModelCreating
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     // Unique constraints
        //     // modelBuilder.Entity<Korisnik>().HasIndex(k => k.Email).IsUnique();
        //     // modelBuilder.Entity<Knjiga>().HasIndex(k => k.Isbn).IsUnique();
        //     // modelBuilder.Entity<Primjerak>().HasIndex(p => p.InventarniBroj).IsUnique();
        //     // modelBuilder.Entity<Uloga>().HasIndex(u => u.Naziv).IsUnique();
        //     // modelBuilder.Entity<Kategorija>().HasIndex(k => k.Naziv).IsUnique();
        //
        //     // JSONB kolone za AuditLog
        //     // modelBuilder.Entity<AuditLog>().Property(a => a.VrijednostiPrije).HasColumnType("jsonb");
        //     // modelBuilder.Entity<AuditLog>().Property(a => a.VrijednostiNakon).HasColumnType("jsonb");
        //
        //     // Seed data za uloge
        //     // modelBuilder.Entity<Uloga>().HasData(
        //     //     new Uloga { Id = 1, Naziv = "Član", Opis = "Član biblioteke" },
        //     //     new Uloga { Id = 2, Naziv = "Bibliotekar", Opis = "Bibliotečko osoblje" },
        //     //     new Uloga { Id = 3, Naziv = "Administrator", Opis = "Sistem administrator" }
        //     // );
        // }
    }
}
