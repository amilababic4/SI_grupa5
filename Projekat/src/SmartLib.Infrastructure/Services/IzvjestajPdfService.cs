using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SmartLib.Core.DTOs;

using IContainer = QuestPDF.Infrastructure.IContainer;
using LicenseType = QuestPDF.Infrastructure.LicenseType;

namespace SmartLib.Infrastructure.Services
{
    public static class IzvjestajPdfService
    {
        // ─── BOJE ───────────────────────────────────────────────────────────────
        private static readonly Color BojaTamna = Color.FromHex("07111f");
        private static readonly Color BojaPrimarna = Color.FromHex("173b63");
        private static readonly Color BojaGold = Color.FromHex("c8974a");
        private static readonly Color BojaMuted = Color.FromHex("64748b");
        private static readonly Color BojaBorder = Color.FromHex("dbeafe");
        private static readonly Color BojaSuccess = Color.FromHex("027a48");
        private static readonly Color BojaDanger = Color.FromHex("b42318");
        private static readonly Color BojaWarning = Color.FromHex("92400e");

        // ════════════════════════════════════════════════════════════════════════
        // ZADUŽENJA
        // ════════════════════════════════════════════════════════════════════════

        public static byte[] GenerirajMjesecnaZaduzenjaPdf(MjesecniZaduzenjaIzvjestajDto data)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(28);
                    page.DefaultTextStyle(x =>
                        x.FontFamily("Arial").FontSize(9).FontColor(BojaTamna));
                    page.Header().Element(c => IzgradiHeader(c, data));
                    page.Content().Element(c => IzgradiSadrzaj(c, data));
                    page.Footer().Element(c => IzgradiFooter(c, data));
                });
            }).GeneratePdf();
        }

        private static void IzgradiHeader(IContainer container, MjesecniZaduzenjaIzvjestajDto data)
        {
            container.Background(BojaPrimarna).Padding(20).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("SmartLib").FontSize(22).Bold().FontColor(Color.FromHex("ffffff"));
                    col.Item().Text("Bibliotečki informacioni sistem").FontSize(9).FontColor(Color.FromHex("A6FFFFFF"));
                });
                row.RelativeItem(2).AlignCenter().Column(col =>
                {
                    col.Item().Text("Izvještaj o zaduživanjima knjiga")
                        .FontSize(14).Bold().FontColor(Color.FromHex("ffffff")).AlignCenter();
                    col.Item().Height(4);
                    col.Item().Background(BojaGold).PaddingVertical(4).PaddingHorizontal(6).AlignCenter()
                        .Text($"{data.NazivMjeseca} {data.Godina}").FontSize(11).Bold().FontColor(Color.FromHex("ffffff"));
                });
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"Generisano: {data.GenerisanoU:dd.MM.yyyy}").FontSize(8).FontColor(Color.FromHex("B3FFFFFF")).AlignRight();
                    col.Item().Text($"u {data.GenerisanoU:HH:mm}").FontSize(8).FontColor(Color.FromHex("80FFFFFF")).AlignRight();
                });
            });
        }

        private static void IzgradiSadrzaj(IContainer container, MjesecniZaduzenjaIzvjestajDto data)
        {
            container.Column(col =>
            {
                col.Item().Height(16);
                col.Item().Element(c => IzgradiStatistike(c, data));
                col.Item().Height(16);
                if (data.Stavke.Count == 0)
                    col.Item().Element(c => IzgradiPraznaPoruka(c, data));
                else
                    col.Item().Element(c => IzgradiTabela(c, data));
            });
        }

        private static void IzgradiStatistike(IContainer container, MjesecniZaduzenjaIzvjestajDto data)
        {
            container.Row(row =>
            {
                StatKartica(row.RelativeItem(), "UKUPNO ZADUŽENJA", data.UkupnoZaduzenja.ToString(), BojaPrimarna);
                row.ConstantItem(8);
                StatKartica(row.RelativeItem(), "AKTIVNA", data.AktivnaZaduzenja.ToString(), BojaSuccess);
                row.ConstantItem(8);
                StatKartica(row.RelativeItem(), "ZATVORENA", data.ZatvorenaZaduzenja.ToString(), BojaMuted);
                row.ConstantItem(8);
                StatKartica(row.RelativeItem(), "ZAKAŠNJELA", data.ZakasnjelaZaduzenja.ToString(), BojaDanger);
            });
        }

        private static void StatKartica(IContainer container, string label, string vrijednost, Color boja)
        {
            container.Border(1).BorderColor(BojaBorder).Padding(14).Column(col =>
            {
                col.Item().Text(label).FontSize(7).Bold().FontColor(BojaMuted);
                col.Item().Height(6);
                col.Item().Text(vrijednost).FontSize(28).Bold().FontColor(boja);
            });
        }

        private static void IzgradiTabela(IContainer container, MjesecniZaduzenjaIzvjestajDto data)
        {
            container.Column(col =>
            {
                col.Item().PaddingBottom(8)
                    .Text($"Lista zaduženja — {data.NazivMjeseca} {data.Godina} ({data.UkupnoZaduzenja})")
                    .FontSize(10).Bold().FontColor(BojaPrimarna);

                col.Item().Table(table =>
                {
                    // 9 kolona: # | Član | Knjiga | Autor | Inv.broj | Zaduženo | Rok | Vraćeno | Status
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(24);
                        cols.RelativeColumn(2.2f);
                        cols.RelativeColumn(2.8f);
                        cols.RelativeColumn(1.5f);
                        cols.RelativeColumn(1.2f);
                        cols.RelativeColumn(1.3f);
                        cols.RelativeColumn(1.3f);
                        cols.RelativeColumn(1.3f);
                        cols.ConstantColumn(70);
                    });

                    table.Header(header =>
                    {
                        void CellHeader(IContainer c, string text) =>
                            c.Background(Color.FromHex("1e3a5f")).Padding(6)
                             .Text(text).FontSize(7.5f).Bold().FontColor(Color.FromHex("ffffff"));

                        header.Cell().Element(c => CellHeader(c, "#"));
                        header.Cell().Element(c => CellHeader(c, "ČLAN"));
                        header.Cell().Element(c => CellHeader(c, "KNJIGA"));
                        header.Cell().Element(c => CellHeader(c, "AUTOR"));
                        header.Cell().Element(c => CellHeader(c, "INV. BROJ"));
                        header.Cell().Element(c => CellHeader(c, "ZADUŽENO"));
                        header.Cell().Element(c => CellHeader(c, "ROK"));
                        header.Cell().Element(c => CellHeader(c, "VRAĆENO"));
                        header.Cell().Element(c => CellHeader(c, "STATUS"));
                    });

                    foreach (var s in data.Stavke)
                    {
                        var bg = s.RedniBroj % 2 == 0 ? Color.FromHex("f8faff") : Color.FromHex("ffffff");
                        IContainer Cell(IContainer c) =>
                            c.Background(bg).BorderBottom(0.5f).BorderColor(BojaBorder).Padding(6);

                        table.Cell().Element(Cell).Text(s.RedniBroj.ToString()).FontColor(BojaMuted);
                        table.Cell().Element(Cell).Text(s.ClanImePrezime);
                        table.Cell().Element(Cell).Text(s.NaslovKnjige);
                        table.Cell().Element(Cell).Text(s.Autor);
                        table.Cell().Element(Cell).Text(s.InventarniBroj);
                        table.Cell().Element(Cell).Text(s.DatumZaduzivanja.ToString("dd.MM.yyyy"));
                        table.Cell().Element(Cell).Text(s.DatumPlaniranogVracanja?.ToString("dd.MM.yyyy") ?? "—");
                        table.Cell().Element(Cell).Text(
                            s.DatumStvarnogVracanja.HasValue
                                ? s.DatumStvarnogVracanja.Value.ToString("dd.MM.yyyy")
                                : "—");

                        var status = s.Status switch
                        {
                            "aktivno" => ("Aktivno", BojaSuccess),
                            "zatvoreno" => ("Zatvoreno", BojaMuted),
                            "zakašnjelo" => ("Zakašnjelo", BojaDanger),
                            _ => (s.Status, BojaMuted)
                        };
                        table.Cell().Element(Cell).AlignCenter().Text(status.Item1).FontColor(status.Item2);
                    }
                });
            });
        }

        private static void IzgradiPraznaPoruka(IContainer container, MjesecniZaduzenjaIzvjestajDto data)
        {
            container.Border(1).BorderColor(BojaBorder).Padding(40).AlignCenter().Column(col =>
            {
                col.Item().Text("Nema podataka").FontSize(16).Bold().FontColor(BojaMuted);
                col.Item().Height(8);
                col.Item().Text($"Nema zaduženja za {data.NazivMjeseca} {data.Godina}.").FontSize(10).FontColor(BojaMuted);
            });
        }

        private static void IzgradiFooter(IContainer container, MjesecniZaduzenjaIzvjestajDto data)
        {
            container.PaddingTop(10).BorderTop(0.5f).BorderColor(BojaBorder).Row(row =>
            {
                row.RelativeItem().Text($"SmartLib © {DateTime.Now.Year}").FontSize(7.5f).FontColor(BojaMuted);
                row.RelativeItem().AlignCenter().Text($"Period: {data.NazivMjeseca} {data.Godina}").FontSize(7.5f).FontColor(BojaMuted);
                row.RelativeItem().AlignRight().Text(t =>
                {
                    t.Span("Stranica ").FontSize(7.5f);
                    t.CurrentPageNumber().FontSize(7.5f).Bold().FontColor(BojaPrimarna);
                    t.Span(" / ");
                    t.TotalPages().FontSize(7.5f).Bold().FontColor(BojaPrimarna);
                });
            });
        }

        // ════════════════════════════════════════════════════════════════════════
        // REZERVACIJE
        // ════════════════════════════════════════════════════════════════════════

        public static byte[] GenerirajMjesecneRezervacijePdf(MjesecneRezervacijeIzvjestajDto data)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(28);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9).FontColor(BojaTamna));
                    page.Header().Element(c => RezHeader(c, data));
                    page.Content().Element(c => RezSadrzaj(c, data));
                    page.Footer().Element(c =>
                        c.PaddingTop(10).BorderTop(0.5f).BorderColor(BojaBorder).Row(row =>
                        {
                            row.RelativeItem().Text($"SmartLib © {DateTime.Now.Year}").FontSize(7.5f).FontColor(BojaMuted);
                            row.RelativeItem().AlignCenter().Text($"Period: {data.NazivMjeseca} {data.Godina}").FontSize(7.5f).FontColor(BojaMuted);
                            row.RelativeItem().AlignRight().Text(t =>
                            {
                                t.Span("Stranica ").FontSize(7.5f);
                                t.CurrentPageNumber().FontSize(7.5f).Bold().FontColor(BojaPrimarna);
                                t.Span(" / ");
                                t.TotalPages().FontSize(7.5f).Bold().FontColor(BojaPrimarna);
                            });
                        }));
                });
            }).GeneratePdf();
        }

        private static void RezHeader(IContainer container, MjesecneRezervacijeIzvjestajDto data)
        {
            container.Background(BojaPrimarna).Padding(20).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("SmartLib").FontSize(22).Bold().FontColor(Color.FromHex("ffffff"));
                    col.Item().Text("Bibliotečki informacioni sistem").FontSize(9).FontColor(Color.FromHex("A6FFFFFF"));
                });
                row.RelativeItem(2).AlignCenter().Column(col =>
                {
                    col.Item().Text("Izvještaj o rezervacijama knjiga")
                        .FontSize(14).Bold().FontColor(Color.FromHex("ffffff")).AlignCenter();
                    col.Item().Height(4);
                    col.Item().Background(BojaGold).PaddingVertical(4).PaddingHorizontal(6).AlignCenter()
                        .Text($"{data.NazivMjeseca} {data.Godina}").FontSize(11).Bold().FontColor(Color.FromHex("ffffff"));
                });
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"Generisano: {data.GenerisanoU:dd.MM.yyyy}").FontSize(8).FontColor(Color.FromHex("B3FFFFFF")).AlignRight();
                    col.Item().Text($"u {data.GenerisanoU:HH:mm}").FontSize(8).FontColor(Color.FromHex("80FFFFFF")).AlignRight();
                });
            });
        }

        private static void RezSadrzaj(IContainer container, MjesecneRezervacijeIzvjestajDto data)
        {
            container.Column(col =>
            {
                col.Item().Height(16);
                col.Item().Row(row =>
                {
                    StatKartica(row.RelativeItem(), "UKUPNO REZERVACIJA", data.UkupnoRezervacija.ToString(), BojaPrimarna);
                    row.ConstantItem(8);
                    StatKartica(row.RelativeItem(), "AKTIVNE", data.AktivneRezervacije.ToString(), BojaSuccess);
                    row.ConstantItem(8);
                    StatKartica(row.RelativeItem(), "ZAVRŠENE", data.ZavrseneRezervacije.ToString(), BojaMuted);
                    row.ConstantItem(8);
                    StatKartica(row.RelativeItem(), "OTKAZANE", data.OtkazaneRezervacije.ToString(), BojaDanger);
                });
                col.Item().Height(16);

                if (data.Stavke.Count == 0)
                {
                    container.Border(1).BorderColor(BojaBorder).Padding(40).AlignCenter().Column(c =>
                    {
                        c.Item().Text("Nema podataka").FontSize(16).Bold().FontColor(BojaMuted);
                        c.Item().Height(8);
                        c.Item().Text($"Nema rezervacija za {data.NazivMjeseca} {data.Godina}.").FontSize(10).FontColor(BojaMuted);
                    });
                    return;
                }

                col.Item().PaddingBottom(8)
                    .Text($"Lista rezervacija — {data.NazivMjeseca} {data.Godina} ({data.UkupnoRezervacija})")
                    .FontSize(10).Bold().FontColor(BojaPrimarna);

                col.Item().Table(table =>
                {
                    // 7 kolona: # | Član | Knjiga | Autor | Rezervisano | Ističe | Status
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(24);
                        cols.RelativeColumn(2.2f);
                        cols.RelativeColumn(2.8f);
                        cols.RelativeColumn(1.8f);
                        cols.RelativeColumn(1.4f);
                        cols.RelativeColumn(1.4f);
                        cols.ConstantColumn(70);
                    });

                    table.Header(header =>
                    {
                        void H(IContainer c, string t) =>
                            c.Background(Color.FromHex("1e3a5f")).Padding(6)
                             .Text(t).FontSize(7.5f).Bold().FontColor(Color.FromHex("ffffff"));

                        header.Cell().Element(c => H(c, "#"));
                        header.Cell().Element(c => H(c, "ČLAN"));
                        header.Cell().Element(c => H(c, "KNJIGA"));
                        header.Cell().Element(c => H(c, "AUTOR"));
                        header.Cell().Element(c => H(c, "REZERVISANO"));
                        header.Cell().Element(c => H(c, "ISTIČE"));
                        header.Cell().Element(c => H(c, "STATUS"));
                    });

                    foreach (var s in data.Stavke)
                    {
                        var bg = s.RedniBroj % 2 == 0 ? Color.FromHex("f8faff") : Color.FromHex("ffffff");
                        IContainer Cell(IContainer c) =>
                            c.Background(bg).BorderBottom(0.5f).BorderColor(BojaBorder).Padding(6);

                        var status = s.Status switch
                        {
                            "aktivna" => ("Aktivna", BojaSuccess),
                            "završena" => ("Završena", BojaMuted),
                            "otkazana" => ("Otkazana", BojaDanger),
                            _ => (s.Status, BojaMuted)
                        };

                        table.Cell().Element(Cell).Text(s.RedniBroj.ToString()).FontColor(BojaMuted);
                        table.Cell().Element(Cell).Text(s.ClanImePrezime);
                        table.Cell().Element(Cell).Text(s.NaslovKnjige);
                        table.Cell().Element(Cell).Text(s.Autor);
                        table.Cell().Element(Cell).Text(s.DatumRezervacije.ToString("dd.MM.yyyy"));
                        table.Cell().Element(Cell).Text(s.DatumIsteka?.ToString("dd.MM.yyyy") ?? "—");
                        table.Cell().Element(Cell).AlignCenter().Text(status.Item1).FontColor(status.Item2);
                    }
                });
            });
        }

        // ════════════════════════════════════════════════════════════════════════
        // ČLANOVI
        // ════════════════════════════════════════════════════════════════════════

        public static byte[] GenerirajMjesecneClanovePdf(MjesecniClanoviIzvjestajDto data)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(28);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9).FontColor(BojaTamna));
                    page.Header().Element(c => ClanHeader(c, data));
                    page.Content().Element(c => ClanSadrzaj(c, data));
                    page.Footer().Element(c =>
                        c.PaddingTop(10).BorderTop(0.5f).BorderColor(BojaBorder).Row(row =>
                        {
                            row.RelativeItem().Text($"SmartLib © {DateTime.Now.Year}").FontSize(7.5f).FontColor(BojaMuted);
                            row.RelativeItem().AlignCenter().Text($"Period: {data.NazivMjeseca} {data.Godina}").FontSize(7.5f).FontColor(BojaMuted);
                            row.RelativeItem().AlignRight().Text(t =>
                            {
                                t.Span("Stranica ").FontSize(7.5f);
                                t.CurrentPageNumber().FontSize(7.5f).Bold().FontColor(BojaPrimarna);
                                t.Span(" / ");
                                t.TotalPages().FontSize(7.5f).Bold().FontColor(BojaPrimarna);
                            });
                        }));
                });
            }).GeneratePdf();
        }

        private static void ClanHeader(IContainer container, MjesecniClanoviIzvjestajDto data)
        {
            container.Background(BojaPrimarna).Padding(20).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("SmartLib").FontSize(22).Bold().FontColor(Color.FromHex("ffffff"));
                    col.Item().Text("Bibliotečki informacioni sistem").FontSize(9).FontColor(Color.FromHex("A6FFFFFF"));
                });
                row.RelativeItem(2).AlignCenter().Column(col =>
                {
                    col.Item().Text("Izvještaj o članovima")
                        .FontSize(14).Bold().FontColor(Color.FromHex("ffffff")).AlignCenter();
                    col.Item().Height(4);
                    col.Item().Background(BojaGold).PaddingVertical(4).PaddingHorizontal(6).AlignCenter()
                        .Text($"{data.NazivMjeseca} {data.Godina}").FontSize(11).Bold().FontColor(Color.FromHex("ffffff"));
                });
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"Generisano: {data.GenerisanoU:dd.MM.yyyy}").FontSize(8).FontColor(Color.FromHex("B3FFFFFF")).AlignRight();
                    col.Item().Text($"u {data.GenerisanoU:HH:mm}").FontSize(8).FontColor(Color.FromHex("80FFFFFF")).AlignRight();
                });
            });
        }

        private static void ClanSadrzaj(IContainer container, MjesecniClanoviIzvjestajDto data)
        {
            container.Column(col =>
            {
                col.Item().Height(16);
                col.Item().Row(row =>
                {
                    StatKartica(row.RelativeItem(), "UKUPNO AKTIVNIH", data.UkupnoAktivnihClanova.ToString(), BojaPrimarna);
                    row.ConstantItem(8);
                    StatKartica(row.RelativeItem(), "NOVIH U PERIODU", data.NovihClanova.ToString(), BojaSuccess);
                    row.ConstantItem(8);
                    StatKartica(row.RelativeItem(), "AKTIVNA ČLAN.", data.ClanovaAktivnaClanarina.ToString(), BojaWarning);
                    row.ConstantItem(8);
                    StatKartica(row.RelativeItem(), "ISTEKLA ČLAN.", data.ClanovaIsteklaClanarina.ToString(), BojaDanger);
                });
                col.Item().Height(16);

                if (data.Stavke.Count == 0)
                {
                    container.Border(1).BorderColor(BojaBorder).Padding(40).AlignCenter().Column(c =>
                    {
                        c.Item().Text("Nema podataka").FontSize(16).Bold().FontColor(BojaMuted);
                        c.Item().Height(8);
                        c.Item().Text($"Nema članova za {data.NazivMjeseca} {data.Godina}.").FontSize(10).FontColor(BojaMuted);
                    });
                    return;
                }

                col.Item().PaddingBottom(8)
                    .Text($"Lista članova — {data.NazivMjeseca} {data.Godina} ({data.UkupnoAktivnihClanova})")
                    .FontSize(10).Bold().FontColor(BojaPrimarna);

                col.Item().Table(table =>
                {
                    // 8 kolona: # | Ime i prezime | Email | Registrovan | Čl.status | Čl.važi do | Zad. | Rez.
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(24);
                        cols.RelativeColumn(2f);
                        cols.RelativeColumn(2.5f);
                        cols.RelativeColumn(1.4f);
                        cols.ConstantColumn(65);
                        cols.RelativeColumn(1.4f);
                        cols.ConstantColumn(50);
                        cols.ConstantColumn(50);
                    });

                    table.Header(header =>
                    {
                        void H(IContainer c, string t) =>
                            c.Background(Color.FromHex("1e3a5f")).Padding(6)
                             .Text(t).FontSize(7.5f).Bold().FontColor(Color.FromHex("ffffff"));

                        header.Cell().Element(c => H(c, "#"));
                        header.Cell().Element(c => H(c, "IME I PREZIME"));
                        header.Cell().Element(c => H(c, "EMAIL"));
                        header.Cell().Element(c => H(c, "REGISTROVAN"));
                        header.Cell().Element(c => H(c, "ČLAN. STATUS"));
                        header.Cell().Element(c => H(c, "ČLAN. VAŽI DO"));
                        header.Cell().Element(c => H(c, "ZADUŽENIH"));
                        header.Cell().Element(c => H(c, "REZERVISANIH"));
                    });

                    foreach (var s in data.Stavke)
                    {
                        var bg = s.RedniBroj % 2 == 0 ? Color.FromHex("f8faff") : Color.FromHex("ffffff");
                        IContainer Cell(IContainer c) =>
                            c.Background(bg).BorderBottom(0.5f).BorderColor(BojaBorder).Padding(6);

                        var statusBoja = s.StatusClanarine == "Aktivna" ? BojaSuccess : BojaDanger;

                        table.Cell().Element(Cell).Text(s.RedniBroj.ToString()).FontColor(BojaMuted);
                        table.Cell().Element(Cell).Text(s.ImePrezime);
                        table.Cell().Element(Cell).Text(s.Email);
                        table.Cell().Element(Cell).Text(s.DatumRegistracije.ToString("dd.MM.yyyy"));
                        table.Cell().Element(Cell).AlignCenter().Text(s.StatusClanarine).FontColor(statusBoja);
                        table.Cell().Element(Cell).Text(s.ClanarinaVaziDo.HasValue ? s.ClanarinaVaziDo.Value.ToString("dd.MM.yyyy") : "—");
                        table.Cell().Element(Cell).AlignCenter().Text(s.BrojZaduzenja.ToString());
                        table.Cell().Element(Cell).AlignCenter().Text(s.BrojRezervacija.ToString());
                    }
                });
            });
        }
    }
}