# SmartLib —  Known Issues / Limitations

Ovaj dokument predstavlja listu poznatih problema i ograničenja.

---

## 1. Poznati bugovi

U toku razvoja sistema (Sprintovi 5–10) evidentirano je šest bugova. Svi su identificirani i riješeni tokom razvojnog procesa, tako da u evidenciji testiranja **nema otvorenih bugova**.

| ID    | Opis greške                                                                                                  | Sprint otkrivanja | Status   |
| ----- | ------------------------------------------------------------------------------------------------------------ | ----------------- | -------- |
| BG-01 | In-memory baza u testovima bila kreirana iznova za svaki request, uzrokujući pad login testova               | 5–6               | Riješeno |
| BG-02 | XSS ranjivost u `KategorijaController` i `KorisnikController` — dozvoljavano pohranjivanje `<script>` tagova | 5–6               | Riješeno |
| BG-03 | Lazy loading problem u `PrimjerakController` — polje `knjiga` (naslov) vraćalo `null` u API odgovoru         | 5–6               | Riješeno |
| BG-04 | Email normalizacija — korisnik registrovan sa velikim slovima mogao se prijaviti samo tačno tim emailom      | 5–6               | Riješeno |
| BG-05 | Forma za rezervaciju dozvoljavala kreiranje rezervacije i kada knjiga ima dostupnih primjeraka               | 9                 | Riješeno |
| BG-06 | Sistem nije prikazivao jasnu poruku kada je komentar na forumu odbijen zbog blokirane riječi                 | 9                 | Riješeno |

---

## 2. Tehnička ograničenja

### 2.1 JWT token — nema mehanizma za opoziv

Sistem koristi JWT tokene čije trajanje se konfigurira kroz `appsettings.json` (polje `ExpirationMinutes`, trenutna vrijednost: 30 minuta). Jednom izdat token ne može biti poništen ni u jednom scenariju. Konkretan rizik: ako bibliotekar ili administrator deaktivira korisnički nalog, vlasnik tog naloga može nastaviti koristiti sistem do 30 minuta dok mu aktivan token ne istekne. Sistem ispravno blokira novu prijavu deaktiviranog korisnika, ali ne može retroaktivno poništiti već izdati token. Ovaj rizik je svjesno dokumentovan u sigurnosnim testovima kao PT-06 — Arhitekturalni rizik, sa statusom **Dokumentovano**.

### 2.2 Uređivanje podataka o knjizi — nema namjenskog sučelja za pregled historije promjena

Kada bibliotekar promijeni podatke o knjizi, svaka izmjena se bilježi u Audit log (stanje prije i stanje nakon izmjene). Međutim, ne postoji namjenski prikaz historije izmjena za konkretnu knjigu — uvid u promjene dostupan je isključivo kroz opći Audit log pregled koji je rezervisan za administratora. Bibliotekar i član nemaju direktan uvid u to šta je i kada mijenjano na određenoj knjizi.


### 2.3 Rok zaduženja — fiksan, isti za sve članove

Sistem automatski postavlja rok vraćanja na **dva mjeseca** od dana zaduživanja ako bibliotekar ručno ne unese datum (`datumZaduzivanja.AddMonths(2)`). Sistem ne podržava individualne rokove po članu ili po kategoriji knjige — jedina fleksibilnost je ručni unos datuma od strane bibliotekara u trenutku zaduživanja.

### 2.4 Maksimalan broj primjeraka po zahtjevu: 50

Sistem ne dozvoljava kreiranje više od 50 primjeraka knjige u jednom zahtjevu. Svaki pokušaj koji premašuje ovu granicu vraća grešku (`Create_BrojNovihVeciOd50_VracaBadRequest`). Za biblioteke s velikim fondom ovo može biti ograničavajuće i zahtijevati višestruke zahtjeve.

### 2.5 Korice knjiga — zavisnost o eksternim servisima

Sistem preuzima korice knjiga sa eksternih servisa (Open Library i Google Books). Ako ti servisi nisu dostupni ili ne posjeduju koricu za određeni ISBN, prikazuje se SVG fallback plaćeholder. Ovo ponašanje je testirano i dokumentovano (`Korice_HttpClientVracaNeuspjesanStatusCode_VracaFallbackSvg`, `Korice_HttpClientBacaException_VracaFallbackSvg`).

### 2.6 Email servis

Email servis ima tri strategije slanja: Brevo API → SMTP → fallback log. Ako sve tri ne uspiju, greška se bilježi isključivo u serverski log i korisnik ne dobiva nikakvo obavještenje o neuspješnom slanju. Rezervacija, zaduženje ili drugi zahtjev koji je pokrenuo slanje emaila nastavlja se normalno. Ovo ponašanje je svjesno dizajnirano i dokumentovano testom `Create_EmailGreskaNeBlokiraRezervaciju`.

Ovo ograničenje se odnosi na sve email funkcionalnosti: podsjetnike o rokovima (PB-41), obavještenja o rezervacijama (PB-42) i narudžbe distributeru (PB-49).

### 2.7 Dostupnost knjige — rezervisani primjerci se ne označavaju zasebno

Kreiranje rezervacije ne mijenja status primjerka — primjerak ostaje sa statusom `dostupan` čak i kada na tu knjigu postoji aktivna rezervacija. `BrojDostupnih` se računa isključivo na osnovu statusa primjeraka (`Status == "dostupan"`), bez uzimanja u obzir aktivnih rezervacija.

### 2.8 Brisanje knjige — koristi se hard delete

Sistem koristi fizičko brisanje (hard delete) iz baze podataka. Kod u `KnjigaRepository.DeleteAsync` izvršava `_db.Knjige.Remove(knjiga)` i `_db.SaveChangesAsync()`, čime se zapis trajno uklanja. Prije brisanja sistem bilježi Audit log zapis, ali jednom obrisana knjiga ne može se povratiti iz baze.


### 2.9 Automatizovani testovi provedeni isključivo na in-memory bazi

Svi automatizovani testovi (unit, integracijski, sigurnosni) koriste in-memory bazu podataka, ne stvarnu MySQL bazu koja se koristi u produkciji. Navedeno je u zaglavlju svakog test izvještaja: *"Okruženje: Development / Test (In-Memory DB)"*. Potencijalne razlike u ponašanju između in-memory i MySQL baze nisu pokrivene automatizovanim testovima.

### 2.10 UI testovi provedeni isključivo u Chromium browseru

Svi UI (End-to-End) testovi provedeni su u Playwright + Chromium okruženju na lokalnoj mašini. Kompatibilnost sistema s ostalim browserima (Firefox, Safari, Edge) nije automatizovano testirana.

---

## 3. Sigurnosna ograničenja

### 3.1 Arhitekturalni rizik — stari JWT deaktiviranog korisnika (PT-06)

Detaljno opisano u sekciji 2.1. Timska odluka bila je da se ovaj rizik dokumentuje, a ne da se implementira mehanizam za opoziv tokena. U test izvještajima ovaj rizik je evidentiran pod oznakom PT-06 sa statusom **Dokumentovano**.

### 3.2 XSS zaštita — ograničen opseg eksplicitnih provjera

XSS zaštita implementirana je kroz privatnu metodu `SadrziHtml()` koja se poziva isključivo u `KategorijaController` (naziv i opis kategorije) i `KorisnikController` (ime i prezime pri registraciji), oba u SmartLib.API projektu. Ostala polja na ostalim formama nemaju eksplicitnu XSS provjeru kroz ovu metodu.

### 3.3 Nema privremenog zaključavanja naloga pri brute force napadima

Sistem vraća `401 Unauthorized` za svaki neuspješan pokušaj prijave, ali ne blokira nalog privremeno nakon određenog broja pokušaja. U kodu `AuthController`-a nema mehanizma za praćenje broja neuspješnih pokušaja, rate-limitinga ni zaključavanja naloga. Dokumentovano testom PT-03.

---

## 4. Nedovršene funkcionalnosti

### 4.1 "Kazne" za kasno vraćanje — samo blokiranje, bez obračuna finansijske kazne

Originalni Product Backlog opisuje PB-47 kao: "Evidencija i obračun kazni za prekoračenje roka vraćanja". U Set of User Stories dokumentu, US-93 i US-94 opisuju obračun iznosa kazne po danu kašnjenja i prikaz ukupnog duga člana.

Međutim, ono što je stvarno implementirano u Sprintu 10 je isključivo blokiranje novih zaduženja i rezervacija za članove koji imaju zakašnjela nevraćena zaduženja: `"Nije moguće kreirati zaduženje — odabrani član ima jedno ili više zakasnjelih zaduženja koja nisu vraćena."` — što je potvrđeno i u Sprint Review dokumentu Sprinta 10.

Finansijski obračun kazni (iznos po danu, ukupni dug, prikaz korisnikove obaveze) nije implementiran. US-93 i US-94 iz Set of User Stories dokumenta koji opisuju obračun iznosa kazni nisu realizovani.

### 4.2 Online produžetak članarine — bez integracije plaćanja

U Product Backlogu, uz PB-48 stoji napomena: "Zahtijeva integraciju sistema za online plaćanje." Ono što je implementirano je tok: član podnosi zahtjev → bibliotekar odobrava ili odbija kroz `ZahtjevProduzenjaRepository`. Sistem ne podržava nikakav oblik plaćanja — nije definirano ni da li je produžetak besplatan ili plaćen, što ostaje kao otvoreno pitanje iz originalne dokumentacije.

---

## 5. Pretpostavke koje sistem pravi

### 5.1 Dostupnost email servisa

Sistem pretpostavlja da je barem jedan od email kanala dostupan (Brevo API ili SMTP) za: podsjetnike i upozorenja o rokovima vraćanja (PB-41), obavještenja bibliotekaru o novim rezervacijama (PB-42), narudžbe knjiga distributeru (PB-49). Ako nijedan kanal nije dostupan, ove funkcionalnosti neće raditi, a sistem neće prikazati grešku korisniku — vidjeti sekciju 2.7.

### 5.2 Validne i aktivne email adrese korisnika

Sistem pretpostavlja da korisnici imaju evidentiranu i aktivnu email adresu. Eksplicitno je navedeno u Sprint 10 Review da se obavještenja šalju *"isključivo za aktivna zaduženja članova sa evidentiranom email adresom"*. Korisnici bez evidentiranog emaila neće primati nikakve automatske notifikacije.

### 5.3 Definisana email adresa distributera knjiga

Sistem pretpostavlja da je email adresa distributera knjiga unaprijed definisana u konfiguraciji sistema. U dokumentaciji za PB-49 navedeno je: *"Sistem ima definisanu email adresu distributera."* Ako ta adresa nije ispravno konfigurirana, narudžbe knjiga neće biti dostavljene.

### 5.4 Knjige imaju podatke o kategoriji, izdavaču i godini izdanja

Napredna pretraga i filteri (PB-44) pretpostavljaju da svaka knjiga ima popunjena polja za kategoriju, izdavača i godinu izdanja. Knjige bez ovih podataka neće se pojaviti u rezultatima filtriranja po tim kriterijima.

### 5.5 Aktivna članarina kao uslov za korištenje usluga

Sistem pretpostavlja da član mora imati aktivnu (neisteklu) članarinu da bi mogao zaduživati knjige, što je navedeno u dokumentaciji za PB-25: *"Aktivna članarina je uslov za korištenje usluga."*

### 5.6 Bibliotekar ili administrator registruje nove članove

U dokumentaciji za PB-18 (Kreiranje naloga člana) eksplicitno je navedena pretpostavka: *"Član fizički dolazi u biblioteku i daje svoje podatke osoblju. Bibliotekar ili administrator unosi podatke u sistem."* Sistem ne podržava samostalnu online registraciju članova.

---

## 6. Dijelovi sistema koje ne treba predstavljati kao potpuno završene

### 6.1 Sistem kazni za kasno vraćanje

PB-47 je implementiran samo kao blokiranje novih zaduženja i rezervacija, bez finansijskog obračuna kazni. Originalni User Stories (US-93: obračun kazne po danu kašnjenja, US-94: prikaz ukupnog duga člana) **nisu realizovani**. Sistemu nedostaje logika za obračun iznosa, prikaz duga i evidenciju financijskih obaveza člana. Vidjeti sekciju 4.1.

### 6.2 Online produžetak članarine bez plaćanja

PB-48 je implementiran kao tok zahtjev-odobravanje bez integracije plaćanja, iako Product Backlog napominje da funkcionalnost *"Zahtijeva integraciju sistema za online plaćanje."* Vidjeti sekciju 4.2.