using Xunit;

namespace SmartLib.Tests.UAT
{
    /// <summary>
    /// UAT (Prihvatno testiranje) — Scenariji za manuelnu verifikaciju
    ///
    /// Ovi testovi su DOKUMENTACIJSKI — opisuju korake koje QA/PO
    /// treba manuelno da provjeri u pregledaču ili Postmanu.
    /// Svaki [Fact] je jedan UAT scenarij.
    ///
    /// Sprint: Auth & Korisnik modul
    /// Tester: _______________  Datum: _______________
    /// </summary>
    public class UatScenarii
    {
        // ══════════════════════════════════════════════════════════════════════
        // US-01/02/03 — Kreiranje naloga člana
        // ══════════════════════════════════════════════════════════════════════

        [Fact(DisplayName = "UAT-01 | Bibliotekar kreira novi nalog člana — uspješan tok")]
        public void UAT_01_KreiranjeNalogaClana_UspjesanTok()
        {
            /*
             * PREDUVJET: Prijavljen kao Bibliotekar ili Administrator
             *
             * KORACI:
             * 1. Otvoriti stranicu /Korisnik/Create
             * 2. Upisati: Ime = "Azra", Prezime = "Kovač",
             *             Email = "azra.kovac@test.ba", Lozinka = "Lozinka1!"
             * 3. Kliknuti "Kreiraj"
             *
             * OČEKIVANO:
             * Preusmeravanje na /Korisnik/Index
             * Prikazuje se zelena poruka "Nalog člana je uspješno kreiran."
             * "Azra Kovač" se pojavljuje u listi korisnika
             * U bazi: UlogaId = 1 (Član), Status = "aktivan"
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        [Fact(DisplayName = "UAT-02 | Kreiranje naloga bez obaveznih polja — validacija forme")]
        public void UAT_02_KreiranjeNaloga_PraznaPolja_Validacija()
        {
            /*
             * KORACI:
             * 1. Otvoriti /Korisnik/Create
             * 2. Ostaviti SVA polja prazna
             * 3. Kliknuti "Kreiraj"
             *
             * OČEKIVANO:
             * Forma ostaje otvorena (nije redirect)
             * Prikazuju se greške: "Ime je obavezno.", "Prezime je obavezno.",
             *   "Email adresa je obavezna.", "Lozinka je obavezna."
             * Nijedan korisnik nije kreiran u bazi
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        [Fact(DisplayName = "UAT-03 | Kreiranje naloga sa već registrovanim emailom")]
        public void UAT_03_KreiranjeNaloga_DuplikatEmail()
        {
            /*
             * PREDUVJET: Korisnik "clan@smartlib.ba" već postoji
             *
             * KORACI:
             * 1. Otvoriti /Korisnik/Create
             * 2. Upisati email "clan@smartlib.ba", ostalo popuniti
             * 3. Kliknuti "Kreiraj"
             *
             * OČEKIVANO:
             * Greška: "Ta email adresa je već registrovana."
             * Forma ostaje otvorena, nije kreiran novi korisnik
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        [Fact(DisplayName = "UAT-04 | Kreiranje naloga sa lozinkom kraćom od 8 znakova")]
        public void UAT_04_KreiranjeNaloga_KratkaLozinka()
        {
            /*
             * KORACI:
             * 1. Popuniti formu, lozinka = "abc"
             * 2. Kliknuti "Kreiraj"
             *
             * OČEKIVANO:
             * Greška: "Lozinka mora imati najmanje 8 znakova."
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        // US-04/05 — Prijava i poruke o neuspjehu

        [Fact(DisplayName = "UAT-05 | Uspješna prijava Člana — redirect na Home")]
        public void UAT_05_PrijavaClana_UspjesanTok()
        {
            /*
             * KORACI:
             * 1. Otvoriti /Auth/Login
             * 2. Email = "clan@smartlib.ba", Lozinka = "Test123!"
             * 3. Kliknuti "Prijavi se"
             *
             * OČEKIVANO:
             * Redirect na /Home/Index
             * Prikazano ime korisnika u headeru
             * Bibliotekar/Admin meni NIJE vidljiv
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        [Fact(DisplayName = "UAT-06 | Uspješna prijava Bibliotekara — redirect na Korisnik/Index")]
        public void UAT_06_PrijavaBibliotekar_RedirectNaKorisnikIndex()
        {
            /*
             * KORACI:
             * 1. Email = "bibliotekar@smartlib.ba", Lozinka = "Test123!"
             *
             * OČEKIVANO:
             * Redirect na /Korisnik/Index
             * Lista članova je vidljiva
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        [Fact(DisplayName = "UAT-07 | Neispravna lozinka — generička poruka, bez detalja")]
        public void UAT_07_PogresnaLozinka_GenerickaPorukaGreske()
        {
            /*
             * KORACI:
             * 1. Email = "clan@smartlib.ba", Lozinka = "PogresnaLozinka"
             * 2. Kliknuti "Prijavi se"
             *
             * OČEKIVANO:
             * Poruka: "Prijava nije uspjela." (generička)
             * Poruka NE kaže "lozinka je pogrešna" ni "email ne postoji"
             * Forma je dostupna za ponovni pokušaj
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        [Fact(DisplayName = "UAT-08 | Neispravni email format — validacijska greška")]
        public void UAT_08_NeispravanEmailFormat()
        {
            /*
             * KORACI:
             * 1. Email = "nijeEmail", Lozinka = "Test123!"
             *
             * OČEKIVANO:
             * Greška: "Email adresa nije u ispravnom formatu."
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        // US-06/07 — Odjava i sesija

        [Fact(DisplayName = "UAT-09 | Odjava — redirect na Login, zaštićene stranice nedostupne")]
        public void UAT_09_Odjava_RedirectNaLogin()
        {
            /*
             * PREDUVJET: Prijavljen korisnik
             *
             * KORACI:
             * 1. Kliknuti "Odjavi se"
             * 2. Pokušati ručno upisati /Korisnik/Index u browser
             *
             * OČEKIVANO:
             * Nakon odjave: redirect na /Auth/Login
             * Pokušaj pristupa /Korisnik/Index vodi na Login (ne prikazuje sadržaj)
             * Kolačić sesije je obrisan (DevTools > Application > Cookies)
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        [Fact(DisplayName = "UAT-10 | Sesija traje pri kretanju između stranica")]
        public void UAT_10_SesijaAktivnaIzmejuStranica()
        {
            /*
             * PREDUVJET: Prijavljen korisnik
             *
             * KORACI:
             * 1. Otvoriti nekoliko stranica unutar aplikacije
             * 2. Provjeriti da li je korisnik i dalje prijavljen
             *
             * OČEKIVANO:
             * Korisnik ostaje prijavljen (ne traži se ponovna prijava)
             * Header prikazuje ime tokom cijele navigacije
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        // US-08/09 — RBAC i deaktivacija

        [Fact(DisplayName = "UAT-11 | Član ne može pristupiti Korisnik/Index direktnim URL-om")]
        public void UAT_11_Clan_DirectUrl_ZasticenaSekcija()
        {
            /*
             * PREDUVJET: Prijavljen kao Član
             *
             * KORACI:
             * 1. Ručno upisati /Korisnik/Index u browser
             *
             * OČEKIVANO:
             * Redirect na Login ILI prikaz "Forbidden" (403)
             * Lista članova NIJE prikazana
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        [Fact(DisplayName = "UAT-12 | Deaktiviran korisnik ne može da se prijavi")]
        public void UAT_12_DeaktiviranKorisnik_NeMozePrijava()
        {
            /*
             * PREDUVJET: Postoji korisnik sa Status = "deaktiviran"
             *
             * KORACI:
             * 1. Pokušati prijavu sa emailom deaktiviranog korisnika
             *
             * OČEKIVANO:
             * Prikazuje se generička poruka "Prijava nije uspjela."
             * NE otkriva da je nalog deaktiviran
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }

        [Fact(DisplayName = "UAT-13 | Admin deaktivira korisnika — potvrda u listi")]
        public void UAT_13_AdminDeaktivirajuciKorisnika()
        {
            /*
             * PREDUVJET: Prijavljen kao Bibliotekar ili Administrator
             *
             * KORACI:
             * 1. Otvoriti /Korisnik/Index
             * 2. Pronaći nekog člana, kliknuti "Deaktiviraj"
             * 3. Potvrditi akciju
             *
             * OČEKIVANO:
             * Poruka "Nalog člana je deaktiviran."
             * Status u listi se mijenja
             * Deaktivirani korisnik ne može se prijaviti
             */
            Assert.True(true, "Scenarij definisan — izvršiti manuelno");
        }
    }
}
