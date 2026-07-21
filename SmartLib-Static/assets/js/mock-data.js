/*
 * SmartLib-Static — seed/mock data.
 * Populates the localStorage "database" (via db.js) on first load only.
 * All data is fictional demo content for the static cPanel build.
 */
(function () {
    "use strict";

    function seed() {
        if (DB.isSeeded()) return;

        // ── Kategorije ────────────────────────────────────────────────
        const kategorije = [
            { id: 1, naziv: "Roman", opis: "Klasični i savremeni romani" },
            { id: 2, naziv: "Naučna fantastika", opis: "Distopije i naučnofantastična djela" },
            { id: 3, naziv: "Poezija i drama", opis: "Poetska i dramska djela" },
            { id: 4, naziv: "Filozofija", opis: "Filozofska djela" },
            { id: 5, naziv: "Historija", opis: "Historijska djela" },
            { id: 6, naziv: "Nauka i tehnika", opis: "Popularna nauka" },
            { id: 7, naziv: "Domaći autori", opis: "Djela autora sa prostora Bosne i Hercegovine" },
        ];
        DB.saveAll("kategorije", kategorije);

        // ── Knjige ────────────────────────────────────────────────────
        const knjige = [
            { id: 1, naslov: "Na Drini ćuprija", autor: "Ivo Andrić", isbn: "978-9958-23-001-1", kategorijaId: 7, izdavac: "Svjetlost", godinaIzdanja: 1945, opis: "Epska hronika o mostu na Drini i generacijama koje su živjele uz njega, kroz vijekove osmanske i austrougarske vladavine." },
            { id: 2, naslov: "Derviš i smrt", autor: "Meša Selimović", isbn: "978-9958-23-002-8", kategorijaId: 7, izdavac: "Svjetlost", godinaIzdanja: 1966, opis: "Roman o šejhu Ahmedu Nurudinu i njegovoj potrazi za pravdom i smislom u sukobu s vlašću." },
            { id: 3, naslov: "Prokleta avlija", autor: "Ivo Andrić", isbn: "978-9958-23-003-5", kategorijaId: 7, izdavac: "Svjetlost", godinaIzdanja: 1954, opis: "Priča smještena u istanbulskom zatvoru, o sudbinama ljudi koji su se u njemu našli." },
            { id: 4, naslov: "Travnička hronika", autor: "Ivo Andrić", isbn: "978-9958-23-004-2", kategorijaId: 7, izdavac: "Svjetlost", godinaIzdanja: 1945, opis: "Hronika o životu stranih konzula u Travniku u doba Napoleonovih ratova." },
            { id: 5, naslov: "1984", autor: "George Orwell", isbn: "978-0-452-28423-4", kategorijaId: 2, izdavac: "Buybook", godinaIzdanja: 1949, opis: "Distopijski roman o totalitarnom nadzoru i gušenju individualne slobode." },
            { id: 6, naslov: "Vrli novi svijet", autor: "Aldous Huxley", isbn: "978-0-06-085052-4", kategorijaId: 2, izdavac: "Buybook", godinaIzdanja: 1932, opis: "Vizija budućnosti u kojoj je sreća proizvedena, a individualnost žrtvovana stabilnosti." },
            { id: 7, naslov: "451 stepen po Fahrenheitu", autor: "Ray Bradbury", isbn: "978-1-4516-7331-9", kategorijaId: 2, izdavac: "Buybook", godinaIzdanja: 1953, opis: "Svijet u kojem su knjige zabranjene i spaljuju se, a vatrogasci pale umjesto da gase." },
            { id: 8, naslov: "Zločin i kazna", autor: "Fjodor Dostojevski", isbn: "978-9958-23-005-9", kategorijaId: 1, izdavac: "Veselin Masleša", godinaIzdanja: 1866, opis: "Psihološki roman o Raskoljnikovu, ubistvu i moralnoj patnji koja slijedi." },
            { id: 9, naslov: "Rat i mir", autor: "Lav Tolstoj", isbn: "978-9958-23-006-6", kategorijaId: 1, izdavac: "Veselin Masleša", godinaIzdanja: 1869, opis: "Panorama ruskog društva tokom Napoleonovih ratova, kroz sudbine nekoliko porodica." },
            { id: 10, naslov: "Sto godina samoće", autor: "Gabriel García Márquez", isbn: "978-0-06-088328-7", kategorijaId: 1, izdavac: "Buybook", godinaIzdanja: 1967, opis: "Magično-realistička saga o porodici Buendía i gradu Macondo." },
            { id: 11, naslov: "Hamlet", autor: "William Shakespeare", isbn: "978-9958-23-007-3", kategorijaId: 3, izdavac: "Svjetlost", godinaIzdanja: 1603, opis: "Tragedija o danskom princu, osveti i propadanju." },
            { id: 12, naslov: "Gorski vijenac", autor: "Petar II Petrović Njegoš", isbn: "978-9958-23-008-0", kategorijaId: 3, izdavac: "Svjetlost", godinaIzdanja: 1847, opis: "Dramski spjev o borbi za slobodu i identitet na Cetinju." },
            { id: 13, naslov: "Tako je govorio Zaratustra", autor: "Friedrich Nietzsche", isbn: "978-0-14-195274-0", kategorijaId: 4, izdavac: "Buybook", godinaIzdanja: 1883, opis: "Filozofsko djelo o natčovjeku, vječnom povratku i smrti Boga." },
            { id: 14, naslov: "Kritika čistog uma", autor: "Immanuel Kant", isbn: "978-0-521-65729-7", kategorijaId: 4, izdavac: "Buybook", godinaIzdanja: 1781, opis: "Temeljno djelo moderne filozofije o granicama ljudskog saznanja." },
            { id: 15, naslov: "Historija Bosne i Hercegovine", autor: "Enver Imamović", isbn: "978-9958-23-009-7", kategorijaId: 5, izdavac: "Preporod", godinaIzdanja: 2007, opis: "Pregled historije Bosne i Hercegovine od antike do savremenog doba." },
            { id: 16, naslov: "Kratka istorija vremena", autor: "Stephen Hawking", isbn: "978-0-553-38016-3", kategorijaId: 6, izdavac: "Buybook", godinaIzdanja: 1988, opis: "Popularno-naučni pregled kosmologije, crnih rupa i porijekla svemira." },
        ];
        DB.saveAll("knjige", knjige);

        // ── Primjerci ─────────────────────────────────────────────────
        let primjerakId = 1;
        const primjerci = [];
        const primjerciPoKnjizi = { 1: 3, 2: 2, 3: 2, 4: 1, 5: 3, 6: 2, 7: 2, 8: 2, 9: 1, 10: 3, 11: 1, 12: 1, 13: 2, 14: 1, 15: 1, 16: 2 };
        Object.keys(primjerciPoKnjizi).forEach((knjigaId) => {
            const broj = primjerciPoKnjizi[knjigaId];
            for (let i = 1; i <= broj; i++) {
                primjerci.push({
                    id: primjerakId,
                    knjigaId: Number(knjigaId),
                    inventarniBroj: `INV-${knjigaId}-${i}`,
                    status: "dostupan",
                    datumNabave: "2024-09-01",
                });
                primjerakId++;
            }
        });
        // Zauzmi par primjeraka radi aktivnih zaduženja (postavlja se ispod kad kreiramo zaduzenja)
        DB.saveAll("primjerci", primjerci);

        // ── Korisnici ─────────────────────────────────────────────────
        const korisnici = [
            { id: 1, ime: "Amina", prezime: "Hodžić", email: "admin@smartlib.ba", lozinka: "admin123", uloga: "Administrator", status: "aktivan", datumKreiranja: "2024-01-10", brojUklonjenihSadrzaja: 0, datumZabraneDo: null, listaZeljaJavna: false },
            { id: 2, ime: "Emina", prezime: "Softić", email: "bibliotekar@smartlib.ba", lozinka: "biblio123", uloga: "Bibliotekar", status: "aktivan", datumKreiranja: "2024-02-01", brojUklonjenihSadrzaja: 0, datumZabraneDo: null, listaZeljaJavna: false },
            { id: 3, ime: "Haris", prezime: "Delić", email: "haris.delic@smartlib.ba", lozinka: "biblio123", uloga: "Bibliotekar", status: "aktivan", datumKreiranja: "2024-03-15", brojUklonjenihSadrzaja: 0, datumZabraneDo: null, listaZeljaJavna: false },
            { id: 4, ime: "Faruk", prezime: "Kovačević", email: "clan@smartlib.ba", lozinka: "clan123", uloga: "Član", status: "aktivan", datumKreiranja: "2026-01-15", brojUklonjenihSadrzaja: 0, datumZabraneDo: null, listaZeljaJavna: true },
            { id: 5, ime: "Lejla", prezime: "Musić", email: "lejla.music@example.com", lozinka: "clan123", uloga: "Član", status: "aktivan", datumKreiranja: "2025-06-01", brojUklonjenihSadrzaja: 0, datumZabraneDo: null, listaZeljaJavna: true },
            { id: 6, ime: "Tarik", prezime: "Bešić", email: "tarik.besic@example.com", lozinka: "clan123", uloga: "Član", status: "aktivan", datumKreiranja: "2026-05-01", brojUklonjenihSadrzaja: 1, datumZabraneDo: null, listaZeljaJavna: false },
            { id: 7, ime: "Amar", prezime: "Zukić", email: "amar.zukic@example.com", lozinka: "clan123", uloga: "Član", status: "deaktiviran", datumKreiranja: "2024-01-01", brojUklonjenihSadrzaja: 3, datumZabraneDo: null, listaZeljaJavna: false },
        ];
        DB.saveAll("korisnici", korisnici);

        // ── Članarine ─────────────────────────────────────────────────
        const clanarine = [
            { id: 1, korisnikId: 4, datumPocetka: "2026-01-15", datumIsteka: "2027-01-15" },
            { id: 2, korisnikId: 5, datumPocetka: "2025-06-01", datumIsteka: "2026-06-01" },
            { id: 3, korisnikId: 6, datumPocetka: "2026-05-01", datumIsteka: "2026-11-01" },
            { id: 4, korisnikId: 7, datumPocetka: "2024-01-01", datumIsteka: "2025-01-01" },
        ];
        DB.saveAll("clanarine", clanarine);

        // ── Zaduženja (loans) ─────────────────────────────────────────
        // Primjerak ids: knjiga1 -> 1,2,3 | knjiga2 -> 4,5 | knjiga3 -> 6,7 | knjiga4 -> 8
        // knjiga5 -> 9,10,11 | knjiga6 -> 12,13 | knjiga7 -> 14,15 | knjiga8 -> 16,17
        // knjiga9 -> 18 | knjiga10 -> 19,20,21 | knjiga11 -> 22 | knjiga12 -> 23
        // knjiga13 -> 24,25 | knjiga14 -> 26 | knjiga15 -> 27 | knjiga16 -> 28,29
        const zaduzenja = [
            { id: 1, korisnikId: 4, primjerakId: 1, knjigaId: 1, datumZaduzivanja: "2026-06-10", datumPlaniranogVracanja: "2026-08-10", datumStvarnogVracanja: null, status: "aktivno" },
            { id: 2, korisnikId: 4, primjerakId: 9, knjigaId: 5, datumZaduzivanja: "2026-05-01", datumPlaniranogVracanja: "2026-07-01", datumStvarnogVracanja: null, status: "zakašnjelo" },
            { id: 3, korisnikId: 5, primjerakId: 4, knjigaId: 2, datumZaduzivanja: "2026-07-15", datumPlaniranogVracanja: "2026-09-15", datumStvarnogVracanja: null, status: "aktivno" },
            { id: 4, korisnikId: 6, primjerakId: 12, knjigaId: 6, datumZaduzivanja: "2026-07-01", datumPlaniranogVracanja: "2026-07-25", datumStvarnogVracanja: null, status: "aktivno" },
            { id: 5, korisnikId: 4, primjerakId: 19, knjigaId: 10, datumZaduzivanja: "2026-02-01", datumPlaniranogVracanja: "2026-04-01", datumStvarnogVracanja: "2026-03-28", status: "zatvoreno" },
            { id: 6, korisnikId: 5, primjerakId: 24, knjigaId: 13, datumZaduzivanja: "2026-01-05", datumPlaniranogVracanja: "2026-03-05", datumStvarnogVracanja: "2026-03-01", status: "zatvoreno" },
            { id: 7, korisnikId: 6, primjerakId: 6, knjigaId: 3, datumZaduzivanja: "2025-12-01", datumPlaniranogVracanja: "2026-02-01", datumStvarnogVracanja: "2026-01-20", status: "zatvoreno" },
            { id: 8, korisnikId: 4, primjerakId: 27, knjigaId: 15, datumZaduzivanja: "2025-11-10", datumPlaniranogVracanja: "2026-01-10", datumStvarnogVracanja: "2026-01-05", status: "zatvoreno" },
            { id: 9, korisnikId: 5, primjerakId: 16, knjigaId: 8, datumZaduzivanja: "2026-03-01", datumPlaniranogVracanja: "2026-05-01", datumStvarnogVracanja: "2026-04-25", status: "zatvoreno" },
            { id: 10, korisnikId: 6, primjerakId: 28, knjigaId: 16, datumZaduzivanja: "2026-06-25", datumPlaniranogVracanja: "2026-07-23", datumStvarnogVracanja: null, status: "aktivno" },
        ];
        DB.saveAll("zaduzenja", zaduzenja);

        // Ažuriraj statuse primjeraka koji su trenutno zaduženi
        const zauzetiPrimjerci = zaduzenja.filter((z) => z.status !== "zatvoreno").map((z) => z.primjerakId);
        const primjerciAzurirano = primjerci.map((p) => zauzetiPrimjerci.includes(p.id) ? Object.assign({}, p, { status: "zadužen" }) : p);
        primjerciAzurirano[primjerciAzurirano.length - 1].status = "deaktiviran"; // jedan primjerak deaktiviran radi demonstracije
        DB.saveAll("primjerci", primjerciAzurirano);

        // ── Rezervacije ───────────────────────────────────────────────
        const rezervacije = [
            { id: 1, korisnikId: 5, knjigaId: 1, datumRezervacije: "2026-07-10", datumIsteka: "2026-07-17", status: "aktivna" },
            { id: 2, korisnikId: 6, knjigaId: 5, datumRezervacije: "2026-07-05", datumIsteka: "2026-07-12", status: "istekla" },
            { id: 3, korisnikId: 4, knjigaId: 9, datumRezervacije: "2026-06-01", datumIsteka: "2026-06-08", status: "realizovana" },
        ];
        DB.saveAll("rezervacije", rezervacije);

        // ── Recenzije ─────────────────────────────────────────────────
        const recenzije = [
            { id: 1, knjigaId: 1, korisnikId: 4, ocjena: 5, komentar: "Nezaobilazno djelo. Andrić dočarava vijekove kroz priču o mostu.", datumKreiranja: "2026-04-02" },
            { id: 2, knjigaId: 1, korisnikId: 5, ocjena: 4, komentar: "Sporo se čita ali se isplati.", datumKreiranja: "2026-05-10" },
            { id: 3, knjigaId: 5, korisnikId: 4, ocjena: 5, komentar: "Zastrašujuće aktuelno i danas.", datumKreiranja: "2026-05-20" },
            { id: 4, knjigaId: 8, korisnikId: 5, ocjena: 5, komentar: "Najbolji psihološki roman koji sam čitala.", datumKreiranja: "2026-04-28" },
            { id: 5, knjigaId: 10, korisnikId: 4, ocjena: 4, komentar: "Magično realistično i pomalo zbunjujuće, ali vrijedno.", datumKreiranja: "2026-04-05" },
            { id: 6, knjigaId: 15, korisnikId: 4, ocjena: 3, komentar: "Korisno štivo, malo suhoparno mjestimično.", datumKreiranja: "2026-01-12" },
        ];
        DB.saveAll("recenzije", recenzije);

        DB.saveAll("recenzijaPrijave", [
            { id: 1, recenzijaId: 4, prijavioKorisnikId: 6, razlog: "Neprikladan sadržaj", datumKreiranja: "2026-07-18", status: "otvorena", razrijesioKorisnikId: null, datumRazrjesenja: null },
        ]);

        // ── Forum ─────────────────────────────────────────────────────
        const forumObjave = [
            { id: 1, naslov: "Preporuke za ljetno čitanje?", sadrzaj: "Tražim laganije romane za plažu, imate li prijedloga?", kategorija: "Preporuke knjiga", datumKreiranja: "2026-07-05", zakljucana: false, korisnikId: 4 },
            { id: 2, naslov: "Da li je 'Derviš i smrt' teško za čitanje?", sadrzaj: "Razmišljam da uzmem ovu knjigu, kakva su vaša iskustva?", kategorija: "Pitanja", datumKreiranja: "2026-07-10", zakljucana: false, korisnikId: 5 },
            { id: 3, naslov: "Radno vrijeme biblioteke tokom ljeta", sadrzaj: "Da li se mijenja radno vrijeme preko ljeta?", kategorija: "Opšta diskusija", datumKreiranja: "2026-06-20", zakljucana: true, korisnikId: 2 },
            { id: 4, naslov: "Recenzija: 1984 me je potpuno oduševila", sadrzaj: "Pročitala sam je u tri dana, nisam mogla stati.", kategorija: "Recenzije", datumKreiranja: "2026-06-01", zakljucana: false, korisnikId: 5 },
        ];
        DB.saveAll("forumObjave", forumObjave);

        const forumKomentari = [
            { id: 1, sadrzaj: "Probaj 'Sto godina samoće', savršeno za ljeto!", datumKreiranja: "2026-07-05", objavaId: 1, korisnikId: 5 },
            { id: 2, sadrzaj: "Slažem se, ili nešto od Orwella.", datumKreiranja: "2026-07-06", objavaId: 1, korisnikId: 6 },
            { id: 3, sadrzaj: "Meni je bilo gusto na početku ali se isplati.", datumKreiranja: "2026-07-11", objavaId: 2, korisnikId: 4 },
            { id: 4, sadrzaj: "Radno vrijeme ostaje isto, 08-16h.", datumKreiranja: "2026-06-20", objavaId: 3, korisnikId: 2 },
        ];
        DB.saveAll("forumKomentari", forumKomentari);

        DB.saveAll("forumReakcije", [
            { id: 1, tip: "korisno", datumKreiranja: "2026-07-06", objavaId: 1, korisnikId: 6 },
            { id: 2, tip: "korisno", datumKreiranja: "2026-06-02", objavaId: 4, korisnikId: 4 },
        ]);

        DB.saveAll("forumObjavaPrijave", []);
        DB.saveAll("forumKomentarPrijave", [
            { id: 1, komentarId: 2, prijavioKorisnikId: 5, razlog: "Van teme", datumKreiranja: "2026-07-07", status: "otvorena", razrijesioKorisnikId: null, datumRazrjesenja: null },
        ]);

        // ── Vijesti ───────────────────────────────────────────────────
        const vijesti = [
            { id: 1, naslov: "SmartLib uveo novi sistem kolekcija", sadrzaj: "Članovi sada mogu kreirati i dijeliti svoje kolekcije knjiga sa drugima. Nova opcija se nalazi u meniju **Kolekcije**.", kategorija: "Obavještenje", slikaUrl: "../assets/images/booooks.png", datumObjave: "2026-07-01", autorId: 2 },
            { id: 2, naslov: "Ljetni program čitanja za mlade", sadrzaj: "Ovog ljeta organizujemo poseban program čitanja za mlađe članove uz nagrade za najaktivnije čitaoce.", kategorija: "Djeca", slikaUrl: "../assets/images/kidsBookEvent.png", datumObjave: "2026-06-15", autorId: 2 },
            { id: 3, naslov: "Produženo radno vrijeme čitaonice", sadrzaj: "Čitaonica sada radi do 20h radnim danima kako bi izašla u susret studentima u ispitnim rokovima.", kategorija: "Obavještenje", slikaUrl: "../assets/images/chillCornerBooks.jpg", datumObjave: "2026-05-20", autorId: 3 },
            { id: 4, naslov: "Nove knjige stigle u katalog", sadrzaj: "Ovaj mjesec smo nabavili nekoliko novih naslova iz naučne fantastike i domaće književnosti.", kategorija: "Obavještenje", slikaUrl: "../assets/images/shelvesBooks.jpg", datumObjave: "2026-05-02", autorId: 2 },
            { id: 5, naslov: "Nova tema na forumu izaziva pažnju čitalaca", sadrzaj: "Diskusija o preporukama za ljetno čitanje postala je jedna od najaktivnijih tema na forumu ovog mjeseca.", kategorija: "Forum", slikaUrl: null, datumObjave: "2026-07-08", autorId: 2 },
        ];
        DB.saveAll("vijesti", vijesti);

        // ── Kalendar (događaji) ───────────────────────────────────────
        const dogadjaji = [
            { id: 1, naslov: "Književna večer: Ivo Andrić", opis: "Diskusija o djelima Ive Andrića uz čitanje odlomaka.", datum: "2026-07-28", sat: "18:00", lokacija: "Čitaonica, sprat 1", kategorija: "Predavanje", autorId: 2 },
            { id: 2, naslov: "Radionica kreativnog pisanja", opis: "Radionica za mlade pisce, prijave na recepciji.", datum: "2026-08-05", sat: "17:00", lokacija: "Sala za sastanke", kategorija: "Edukacija", autorId: 2 },
            { id: 3, naslov: "Sajam polovnih knjiga", opis: "Razmjena i prodaja polovnih knjiga u dvorištu biblioteke.", datum: "2026-08-15", sat: "10:00", lokacija: "Dvorište biblioteke", kategorija: "Zajednica", autorId: 3 },
            { id: 4, naslov: "Klub čitalaca — jul", opis: "Mjesečni sastanak kluba čitalaca, tema: 'Derviš i smrt'.", datum: "2026-07-10", sat: "18:30", lokacija: "Čitaonica, sprat 1", kategorija: "Zajednica", autorId: 2 },
            { id: 5, naslov: "Predavanje: Historija štampe u BiH", opis: "Gostujuće predavanje o razvoju štamparstva u regiji.", datum: "2026-06-25", sat: "19:00", lokacija: "Velika sala", kategorija: "Predavanje", autorId: 3 },
            { id: 6, naslov: "Radionica za djecu: Čitanje uz slikovnice", opis: "Interaktivna radionica čitanja za najmlađe posjetioce.", datum: "2026-08-02", sat: "11:00", lokacija: "Dječiji kutak", kategorija: "Djeca", autorId: 2 },
        ];
        DB.saveAll("dogadjaji", dogadjaji);

        // ── Notifikacije ──────────────────────────────────────────────
        const notifikacije = [
            { id: 1, korisnikId: 4, naslov: "Zaduženje kasni", poruka: "Vaš rok za vraćanje knjige '1984' je istekao.", tip: "Zaduzenje", linkUrl: "../zaduzenje/moja.html", procitano: false, datumKreiranja: "2026-07-02" },
            { id: 2, korisnikId: 4, naslov: "Rezervacija realizovana", poruka: "Vaša rezervacija za 'Rat i mir' je realizovana zaduženjem.", tip: "Rezervacija", linkUrl: "../zaduzenje/moja.html", procitano: true, datumKreiranja: "2026-06-01" },
            { id: 3, korisnikId: 4, naslov: "Dobrodošli u SmartLib", poruka: "Hvala vam što ste se pridružili SmartLib biblioteci!", tip: "Sistem", linkUrl: null, procitano: true, datumKreiranja: "2026-01-15" },
            { id: 4, korisnikId: 5, naslov: "Knjiga koju ste rezervisali je dostupna", poruka: "'Na Drini ćuprija' je sada dostupna za preuzimanje.", tip: "Rezervacija", linkUrl: "../rezervacija/moje.html", procitano: false, datumKreiranja: "2026-07-17" },
        ];
        DB.saveAll("notifikacije", notifikacije);

        // ── Kolekcije ─────────────────────────────────────────────────
        const listaKolekcija = [
            { id: 1, korisnikId: 4, naziv: "Lista želja", opis: null, javna: true, datumKreiranja: "2026-01-15", datumAzuriranja: "2026-07-10", isWishlist: true },
            { id: 2, korisnikId: 4, naziv: "Omiljeni romani", opis: "Moji najdraži romani svih vremena", javna: true, datumKreiranja: "2026-02-01", datumAzuriranja: "2026-06-01", isWishlist: false },
            { id: 3, korisnikId: 5, naziv: "Lista želja", opis: null, javna: true, datumKreiranja: "2025-06-01", datumAzuriranja: "2026-05-01", isWishlist: true },
            { id: 4, korisnikId: 5, naziv: "Filozofska čitanja", opis: "Za razmišljanje uz kafu", javna: true, datumKreiranja: "2025-07-01", datumAzuriranja: "2026-04-01", isWishlist: false },
        ];
        DB.saveAll("listaKolekcija", listaKolekcija);

        const listaKolekcijaStavke = [
            { id: 1, listaKolekcijaId: 1, knjigaId: 9, redoslijed: 1, datumDodavanja: "2026-02-01" },
            { id: 2, listaKolekcijaId: 1, knjigaId: 16, redoslijed: 2, datumDodavanja: "2026-03-01" },
            { id: 3, listaKolekcijaId: 2, knjigaId: 1, redoslijed: 1, datumDodavanja: "2026-02-01" },
            { id: 4, listaKolekcijaId: 2, knjigaId: 8, redoslijed: 2, datumDodavanja: "2026-02-10" },
            { id: 5, listaKolekcijaId: 2, knjigaId: 10, redoslijed: 3, datumDodavanja: "2026-03-05" },
            { id: 6, listaKolekcijaId: 3, knjigaId: 2, redoslijed: 1, datumDodavanja: "2025-08-01" },
            { id: 7, listaKolekcijaId: 4, knjigaId: 13, redoslijed: 1, datumDodavanja: "2025-09-01" },
            { id: 8, listaKolekcijaId: 4, knjigaId: 14, redoslijed: 2, datumDodavanja: "2025-09-10" },
        ];
        DB.saveAll("listaKolekcijaStavke", listaKolekcijaStavke);

        // ── Nabavka ───────────────────────────────────────────────────
        DB.saveAll("nabavkaZahtjevi", [
            { id: 1, nazivKnjige: "Alhemičar", autor: "Paulo Coelho", izdavac: "Buybook", brojPrimjeraka: 3, napomena: "Traženo od strane više članova", vrijemePodnosenja: "2026-07-05", emailPoslan: true, podnosilacId: 2 },
            { id: 2, nazivKnjige: "Malena princeza", autor: "Antoine de Saint-Exupéry", izdavac: "Svjetlost", brojPrimjeraka: 2, napomena: "", vrijemePodnosenja: "2026-06-20", emailPoslan: true, podnosilacId: 3 },
            { id: 3, nazivKnjige: "Sapiens", autor: "Yuval Noah Harari", izdavac: "Buybook", brojPrimjeraka: 4, napomena: "Popularno traženo, provjeriti cijenu", vrijemePodnosenja: "2026-07-18", emailPoslan: false, podnosilacId: 2 },
        ]);

        DB.saveAll("appPostavke", [
            { id: 1, kljuc: "DistributerEmail", vrijednost: "nabavka@izdavac.ba" },
        ]);

        // ── Audit Log ─────────────────────────────────────────────────
        DB.saveAll("auditLog", [
            { id: 1, korisnikId: 2, akcija: "Kreirano", entitetTip: "Knjiga", entitetId: 16, vrijednostiPrije: null, vrijednostiNakon: JSON.stringify({ naslov: "Kratka istorija vremena" }), datumAkcije: "2026-05-02" },
            { id: 2, korisnikId: 1, akcija: "Izmijenjeno", entitetTip: "Korisnik", entitetId: 6, vrijednostiPrije: JSON.stringify({ status: "aktivan" }), vrijednostiNakon: JSON.stringify({ status: "aktivan", brojUklonjenihSadrzaja: 1 }), datumAkcije: "2026-07-07" },
            { id: 3, korisnikId: 2, akcija: "Odobreno", entitetTip: "ZahtjevProduzenja", entitetId: 2, vrijednostiPrije: JSON.stringify({ status: "na_cekanju" }), vrijednostiNakon: JSON.stringify({ status: "odobreno" }), datumAkcije: "2026-06-15" },
            { id: 4, korisnikId: 3, akcija: "Obrisano", entitetTip: "Recenzija", entitetId: 7, vrijednostiPrije: JSON.stringify({ komentar: "Neprikladan komentar" }), vrijednostiNakon: null, datumAkcije: "2026-06-10" },
            { id: 5, korisnikId: 1, akcija: "Kreirano", entitetTip: "Korisnik", entitetId: 3, vrijednostiPrije: null, vrijednostiNakon: JSON.stringify({ ime: "Haris", prezime: "Delić", uloga: "Bibliotekar" }), datumAkcije: "2024-03-15" },
            { id: 6, korisnikId: 2, akcija: "Izmijenjeno", entitetTip: "Primjerak", entitetId: 29, vrijednostiPrije: JSON.stringify({ status: "dostupan" }), vrijednostiNakon: JSON.stringify({ status: "deaktiviran" }), datumAkcije: "2026-07-01" },
        ]);

        // ── Zahtjevi za produženje članarine ──────────────────────────
        DB.saveAll("zahtjeviProduzenja", [
            { id: 1, korisnikId: 5, trajanjeMjeseci: 6, napomena: "Molim produženje za još jedan semestar.", status: "na_cekanju", datumPodnosenja: "2026-07-15", datumObrade: null, obradioKorisnikId: null, razlogOdbijanja: null, noviDatumIsteka: null },
            { id: 2, korisnikId: 6, trajanjeMjeseci: 3, napomena: "", status: "odobreno", datumPodnosenja: "2026-04-20", datumObrade: "2026-04-22", obradioKorisnikId: 2, razlogOdbijanja: null, noviDatumIsteka: "2026-11-01" },
        ]);

        DB.markSeeded();
    }

    window.SmartLibSeed = { seed };
    seed();
})();
