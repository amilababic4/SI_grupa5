using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLib.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBanSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrojUklonjenihSadrzaja",
                table: "Korisnici",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatumDeaktivacije",
                table: "Korisnici",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatumZabraneDo",
                table: "Korisnici",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Opis",
                table: "Knjige",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SlikaUrl",
                table: "Knjige",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dogadjaji",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Naslov = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Opis = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Datum = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Sat = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Lokacija = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Kategorija = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AutorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dogadjaji", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dogadjaji_Korisnici_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ForumObjave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Naslov = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sadrzaj = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Kategorija = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Zakljucana = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumObjave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumObjave_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notifikacije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    Naslov = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Poruka = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tip = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Sistem")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Procitano = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifikacije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifikacije_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Recenzije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KnjigaId = table.Column<int>(type: "int", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    Ocjena = table.Column<int>(type: "int", nullable: false),
                    Komentar = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recenzije_Knjige_KnjigaId",
                        column: x => x.KnjigaId,
                        principalTable: "Knjige",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recenzije_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vijesti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Naslov = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sadrzaj = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Kategorija = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SlikaUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumObjave = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vijesti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vijesti_Korisnici_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ForumKomentari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Sadrzaj = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ObjavaId = table.Column<int>(type: "int", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumKomentari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumKomentari_ForumObjave_ObjavaId",
                        column: x => x.ObjavaId,
                        principalTable: "ForumObjave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumKomentari_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ForumObjavaPrijave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ObjavaId = table.Column<int>(type: "int", nullable: false),
                    PrijavioKorisnikId = table.Column<int>(type: "int", nullable: false),
                    Razlog = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "otvorena")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RazrijesioKorisnikId = table.Column<int>(type: "int", nullable: true),
                    DatumRazrjesenja = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumObjavaPrijave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumObjavaPrijave_ForumObjave_ObjavaId",
                        column: x => x.ObjavaId,
                        principalTable: "ForumObjave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumObjavaPrijave_Korisnici_PrijavioKorisnikId",
                        column: x => x.PrijavioKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForumObjavaPrijave_Korisnici_RazrijesioKorisnikId",
                        column: x => x.RazrijesioKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ForumReakcije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tip = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ObjavaId = table.Column<int>(type: "int", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumReakcije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumReakcije_ForumObjave_ObjavaId",
                        column: x => x.ObjavaId,
                        principalTable: "ForumObjave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumReakcije_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RecenzijaPrijave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RecenzijaId = table.Column<int>(type: "int", nullable: false),
                    PrijavioKorisnikId = table.Column<int>(type: "int", nullable: false),
                    Razlog = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "otvorena")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RazrijesioKorisnikId = table.Column<int>(type: "int", nullable: true),
                    DatumRazrjesenja = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecenzijaPrijave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecenzijaPrijave_Korisnici_PrijavioKorisnikId",
                        column: x => x.PrijavioKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecenzijaPrijave_Korisnici_RazrijesioKorisnikId",
                        column: x => x.RazrijesioKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecenzijaPrijave_Recenzije_RecenzijaId",
                        column: x => x.RecenzijaId,
                        principalTable: "Recenzije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ForumKomentarPrijave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KomentarId = table.Column<int>(type: "int", nullable: false),
                    PrijavioKorisnikId = table.Column<int>(type: "int", nullable: false),
                    Razlog = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "otvorena")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RazrijesioKorisnikId = table.Column<int>(type: "int", nullable: true),
                    DatumRazrjesenja = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumKomentarPrijave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumKomentarPrijave_ForumKomentari_KomentarId",
                        column: x => x.KomentarId,
                        principalTable: "ForumKomentari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumKomentarPrijave_Korisnici_PrijavioKorisnikId",
                        column: x => x.PrijavioKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForumKomentarPrijave_Korisnici_RazrijesioKorisnikId",
                        column: x => x.RazrijesioKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Korisnici",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BrojUklonjenihSadrzaja", "DatumDeaktivacije", "DatumZabraneDo" },
                values: new object[] { 0, null, null });

            migrationBuilder.UpdateData(
                table: "Korisnici",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BrojUklonjenihSadrzaja", "DatumDeaktivacije", "DatumZabraneDo" },
                values: new object[] { 0, null, null });

            migrationBuilder.UpdateData(
                table: "Korisnici",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BrojUklonjenihSadrzaja", "DatumDeaktivacije", "DatumZabraneDo" },
                values: new object[] { 0, null, null });

            migrationBuilder.UpdateData(
                table: "Korisnici",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BrojUklonjenihSadrzaja", "DatumDeaktivacije", "DatumZabraneDo" },
                values: new object[] { 0, null, null });

            migrationBuilder.UpdateData(
                table: "Korisnici",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BrojUklonjenihSadrzaja", "DatumDeaktivacije", "DatumZabraneDo" },
                values: new object[] { 0, null, null });

            migrationBuilder.UpdateData(
                table: "Korisnici",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BrojUklonjenihSadrzaja", "DatumDeaktivacije", "DatumZabraneDo" },
                values: new object[] { 0, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Dogadjaji_AutorId",
                table: "Dogadjaji",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumKomentari_KorisnikId",
                table: "ForumKomentari",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumKomentari_ObjavaId",
                table: "ForumKomentari",
                column: "ObjavaId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumKomentarPrijave_KomentarId_PrijavioKorisnikId",
                table: "ForumKomentarPrijave",
                columns: new[] { "KomentarId", "PrijavioKorisnikId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForumKomentarPrijave_PrijavioKorisnikId",
                table: "ForumKomentarPrijave",
                column: "PrijavioKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumKomentarPrijave_RazrijesioKorisnikId",
                table: "ForumKomentarPrijave",
                column: "RazrijesioKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumObjavaPrijave_ObjavaId_PrijavioKorisnikId",
                table: "ForumObjavaPrijave",
                columns: new[] { "ObjavaId", "PrijavioKorisnikId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForumObjavaPrijave_PrijavioKorisnikId",
                table: "ForumObjavaPrijave",
                column: "PrijavioKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumObjavaPrijave_RazrijesioKorisnikId",
                table: "ForumObjavaPrijave",
                column: "RazrijesioKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumObjave_KorisnikId",
                table: "ForumObjave",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumReakcije_KorisnikId",
                table: "ForumReakcije",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumReakcije_ObjavaId",
                table: "ForumReakcije",
                column: "ObjavaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifikacije_KorisnikId_Procitano",
                table: "Notifikacije",
                columns: new[] { "KorisnikId", "Procitano" });

            migrationBuilder.CreateIndex(
                name: "IX_RecenzijaPrijave_PrijavioKorisnikId",
                table: "RecenzijaPrijave",
                column: "PrijavioKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_RecenzijaPrijave_RazrijesioKorisnikId",
                table: "RecenzijaPrijave",
                column: "RazrijesioKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_RecenzijaPrijave_RecenzijaId_PrijavioKorisnikId",
                table: "RecenzijaPrijave",
                columns: new[] { "RecenzijaId", "PrijavioKorisnikId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recenzije_KnjigaId",
                table: "Recenzije",
                column: "KnjigaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzije_KorisnikId",
                table: "Recenzije",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Vijesti_AutorId",
                table: "Vijesti",
                column: "AutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dogadjaji");

            migrationBuilder.DropTable(
                name: "ForumKomentarPrijave");

            migrationBuilder.DropTable(
                name: "ForumObjavaPrijave");

            migrationBuilder.DropTable(
                name: "ForumReakcije");

            migrationBuilder.DropTable(
                name: "Notifikacije");

            migrationBuilder.DropTable(
                name: "RecenzijaPrijave");

            migrationBuilder.DropTable(
                name: "Vijesti");

            migrationBuilder.DropTable(
                name: "ForumKomentari");

            migrationBuilder.DropTable(
                name: "Recenzije");

            migrationBuilder.DropTable(
                name: "ForumObjave");

            migrationBuilder.DropColumn(
                name: "BrojUklonjenihSadrzaja",
                table: "Korisnici");

            migrationBuilder.DropColumn(
                name: "DatumDeaktivacije",
                table: "Korisnici");

            migrationBuilder.DropColumn(
                name: "DatumZabraneDo",
                table: "Korisnici");

            migrationBuilder.DropColumn(
                name: "Opis",
                table: "Knjige");

            migrationBuilder.DropColumn(
                name: "SlikaUrl",
                table: "Knjige");
        }
    }
}
