using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartLib.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategorije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategorije", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uloge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Opis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uloge", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Knjige",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naslov = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Autor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Isbn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    KategorijaId = table.Column<int>(type: "integer", nullable: false),
                    Izdavac = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GodinaIzdanja = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Knjige", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Knjige_Kategorije_KategorijaId",
                        column: x => x.KategorijaId,
                        principalTable: "Kategorije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Prezime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LozinkaHash = table.Column<string>(type: "text", nullable: false),
                    UlogaId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "aktivan"),
                    DatumKreiranja = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Korisnici_Uloge_UlogaId",
                        column: x => x.UlogaId,
                        principalTable: "Uloge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Primjerci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KnjigaId = table.Column<int>(type: "integer", nullable: false),
                    InventarniBroj = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "dostupan"),
                    DatumNabave = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Primjerci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Primjerci_Knjige_KnjigaId",
                        column: x => x.KnjigaId,
                        principalTable: "Knjige",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KorisnikId = table.Column<int>(type: "integer", nullable: true),
                    Akcija = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntitetTip = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntitetId = table.Column<int>(type: "integer", nullable: true),
                    VrijednostiPrije = table.Column<string>(type: "jsonb", nullable: true),
                    VrijednostiNakon = table.Column<string>(type: "jsonb", nullable: true),
                    DatumAkcije = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Clanarine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KorisnikId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "aktivna"),
                    DatumPocetka = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DatumIsteka = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clanarine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clanarine_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rezervacije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KorisnikId = table.Column<int>(type: "integer", nullable: false),
                    KnjigaId = table.Column<int>(type: "integer", nullable: false),
                    DatumRezervacije = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DatumIsteka = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "aktivna")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Knjige_KnjigaId",
                        column: x => x.KnjigaId,
                        principalTable: "Knjige",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zaduzenja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KorisnikId = table.Column<int>(type: "integer", nullable: false),
                    PrimjerakId = table.Column<int>(type: "integer", nullable: false),
                    DatumZaduzivanja = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DatumPlaniranogVracanja = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DatumStvarnogVracanja = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "aktivno")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zaduzenja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zaduzenja_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zaduzenja_Primjerci_PrimjerakId",
                        column: x => x.PrimjerakId,
                        principalTable: "Primjerci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Uloge",
                columns: new[] { "Id", "Naziv", "Opis" },
                values: new object[,]
                {
                    { 1, "Član", "Član biblioteke" },
                    { 2, "Bibliotekar", "Bibliotečko osoblje" },
                    { 3, "Administrator", "Sistem administrator" }
                });

            migrationBuilder.InsertData(
                table: "Korisnici",
                columns: new[] { "Id", "DatumKreiranja", "Email", "Ime", "LozinkaHash", "Prezime", "Status", "UlogaId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 25, 9, 18, 44, 0, DateTimeKind.Utc), "admin@smartlib.ba", "Admin", "R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=", "SmartLib", "aktivan", 3 },
                    { 2, new DateTime(2026, 4, 25, 9, 18, 44, 0, DateTimeKind.Utc), "bibliotekar@smartlib.ba", "Bibliotekar", "R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=", "SmartLib", "aktivan", 2 },
                    { 3, new DateTime(2026, 4, 25, 9, 18, 44, 0, DateTimeKind.Utc), "clan@smartlib.ba", "Test", "R+QQe1m/6nmEN3ZEN4gXFw==:X5nJN7Js0COVp4hLI8OT/IXLK1GT8EBHa9QqNQNalh0=", "Clan", "aktivan", 1 },
                    { 4, new DateTime(2026, 4, 25, 20, 47, 0, 0, DateTimeKind.Utc), "proba@smartlib", "Amila", "RXc8HNFmlRF+0H6XsJGvjg==:+Fi3Qst3q84y5t6TNlJd9zltFZgqyYHErKm+CbKRTXY=", "Babic", "aktivan", 1 },
                    { 5, new DateTime(2026, 4, 26, 15, 4, 48, 0, DateTimeKind.Utc), "imeprezime@gmail.com", "Ime", "eyWukOsw1/s1sYE7WkYNVg==:fdqafu1WAN62dAwt9lyR/GSSq05y9h7PaTebwSsK4Tc=", "Prezime", "aktivan", 1 },
                    { 6, new DateTime(2026, 4, 26, 21, 54, 8, 0, DateTimeKind.Utc), "ivlajcic1@etf.unsa.ba", "Imran", "v1OSV7uzmOkIb4n89gxqVQ==:ghU7xePMJ4TLW/UIySZEozPekQ+BnAyfoUgUXzU3l0k=", "Vlajcic", "aktivan", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_KorisnikId",
                table: "AuditLogs",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Clanarine_KorisnikId",
                table: "Clanarine",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Kategorije_Naziv",
                table: "Kategorije",
                column: "Naziv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Knjige_Isbn",
                table: "Knjige",
                column: "Isbn",
                unique: true,
                filter: "\"Isbn\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Knjige_KategorijaId",
                table: "Knjige",
                column: "KategorijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_Email",
                table: "Korisnici",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_UlogaId",
                table: "Korisnici",
                column: "UlogaId");

            migrationBuilder.CreateIndex(
                name: "IX_Primjerci_InventarniBroj",
                table: "Primjerci",
                column: "InventarniBroj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Primjerci_KnjigaId",
                table: "Primjerci",
                column: "KnjigaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_KnjigaId",
                table: "Rezervacije",
                column: "KnjigaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_KorisnikId",
                table: "Rezervacije",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Uloge_Naziv",
                table: "Uloge",
                column: "Naziv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zaduzenja_KorisnikId",
                table: "Zaduzenja",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Zaduzenja_PrimjerakId",
                table: "Zaduzenja",
                column: "PrimjerakId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Clanarine");

            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.DropTable(
                name: "Zaduzenja");

            migrationBuilder.DropTable(
                name: "Korisnici");

            migrationBuilder.DropTable(
                name: "Primjerci");

            migrationBuilder.DropTable(
                name: "Uloge");

            migrationBuilder.DropTable(
                name: "Knjige");

            migrationBuilder.DropTable(
                name: "Kategorije");
        }
    }
}
