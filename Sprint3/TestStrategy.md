# Test Strategy 

## 1. Cilj testiranja

Cilj testiranja Bibliotečkog informacionog sistema je osigurati da sve implementirane funkcionalnosti rade ispravno i u skladu sa prethodno definisanim zahtjevima i acceptance kriterijima.

Konkretno, testiranjem se nastoji:

- **Potvrditi ispravnost osnovnih funkcionalnosti** – registracija i prijava korisnika, upravljanje knjigama i primjercima, zaduživanje i vraćanje, upravljanje kategorijama, članarinama i rezervacijama.
- **Osigurati tačnost i konzistentnost podataka** – knjiga ne smije biti označena kao dostupna dok ima aktivno zaduženje; primjerak se ne smije deaktivirati dok je zadužen; brisanje kategorije nije dozvoljeno ako su s njom povezane knjige.
- **Provjeriti ispravnost kontrole pristupa** – svaka uloga (Član, Bibliotekar, Administrator) smije vidjeti i koristiti samo funkcionalnosti koje su joj dozvoljene.
- **Verificirati integraciju modula** – moduli moraju ispravno međusobno komunicirati.
- **Identificirati i evidentirati greške** prije puštanja sistema u upotrebu, kako bi se smanjio rizik od kvarova u produkciji.
- **Provjeriti usklađenost s nefunkcionalnim zahtjevima** – performanse, sigurnost, upotrebljivost, pouzdanost i internacionalizacija.

Krajnji cilj je stabilan, pouzdan i funkcionalan sistem spreman za korištenje od strane krajnjih korisnika – članova, bibliotekara i administratora.

---

## 2. Nivoi testiranja

### 2.1 Unit testiranje

Radi lakše preglednosti, unit testovi su grupisani prema međusobno povezanim User Stories i njihovim acceptance kriterijima.

---

#### Unit 1 – Registracija člana (US-01, US-02, US-03)

**Acceptance Criteria:** Kada email adresa nije u ispravnom formatu, tada se prikazuje poruka o grešci

| Test Case | Ulaz | Očekivani rezultat |
|-----------|------|-------------------|
| UT-01 | `ime@gmail.com` | validan |
| UT-02 | `imegmail.com` | greška: nedostaje @ |
| UT-03 | `ime@` | greška: nema domene |
| UT-04 | `""` (prazno) | greška: obavezno polje |
| UT-05 | `ime@gmail` | greška: nema TLD |

---

**Acceptance Criteria:** Kada unesena lozinka ima manje od 8 znakova, tada se prikazuje poruka da lozinka nije dovoljno duga

| Test Case | Ulaz | Očekivani rezultat |
|-----------|------|-------------------|
| UT-06 | `Lozinka1` | validan |
| UT-07 | `abc123` | greška: prekratka |
| UT-08 | `ab` | greška: prekratka |
| UT-09 | `""` (prazno) | greška: obavezno polje |
| UT-10 | `12345678` | validan |

---

**Acceptance Criteria:** Sistem ne smije dozvoliti nastavak registracije bez unosa obaveznih podataka

| Test Case | Ulaz | Očekivani rezultat |
|-----------|------|-------------------|
| UT-11 | Sva polja popunjena | validan |
| UT-12 | Ime prazno | greška: ime obavezno |
| UT-13 | Prezime prazno | greška: prezime obavezno |
| UT-14 | Email prazan | greška: email obavezan |
| UT-15 | Lozinka prazna | greška: lozinka obavezna |
| UT-16 | Sva polja prazna | greška: sva polja su obavezna |

---

#### Unit 2 – Prijava korisnika (US-04, US-05, US-09)

**Acceptance Criteria:** Kada korisnik unese pogrešan email ili lozinku, tada se prijava odbija; sistem ne prikazuje koja je informacija neispravna

| Test Case | Situacija | Očekivani rezultat |
|-----------|-----------|-------------------|
| UT-17 | Pogrešan email | "Email ili lozinka nisu ispravni" |
| UT-18 | Pogrešna lozinka | "Email ili lozinka nisu ispravni" |
| UT-19 | Oba pogrešna | "Email ili lozinka nisu ispravni" |
| UT-20 | Deaktivirani korisnik pokuša prijavu | generička poruka o neuspjeloj prijavi |

---

#### Unit 3 – Dodavanje i validacija knjige (US-12, US-13, US-14)

**Acceptance Criteria:** Kada ISBN nije u prihvatljivom formatu ili već postoji, sistem odbija unos

| Test Case | Ulaz | Očekivani rezultat |
|-----------|------|-------------------|
| UT-21 | `978-3-16-148410-0` | validan format |
| UT-22 | `12345` | greška: prekratak |
| UT-23 | `ABCDEFGHIJ` | greška: nije numerički |
| UT-24 | `""` (prazno) | greška: obavezno polje |
| UT-25 | ISBN koji već postoji u sistemu | greška: ISBN već postoji |

---

**Acceptance Criteria:** Kada obavezni podaci nedostaju, tada sistem prikazuje poruku o grešci

| Test Case | Ulaz | Očekivani rezultat |
|-----------|------|-------------------|
| UT-26 | Sva polja popunjena | validan |
| UT-27 | Naslov prazan | greška |
| UT-28 | Autor prazan | greška |
| UT-29 | Broj primjeraka = 0 | greška |
| UT-30 | Broj primjeraka negativan | greška |

---

#### Unit 4 – Upravljanje kategorijama (US-30, US-33)

**Acceptance Criteria:** Kada kategorija već postoji, tada sistem prikazuje poruku o grešci; kada je naziv prazan, tada sistem prikazuje grešku

| Test Case | Ulaz | Očekivani rezultat |
|-----------|------|-------------------|
| UT-31 | `"Roman"`, lista bez `"Roman"` | validan |
| UT-32 | `""` (prazno) | greška: naziv obavezan |
| UT-33 | `"Roman"`, lista sadrži `"Roman"` | greška: kategorija već postoji |
| UT-34 | `"roman"`, lista sadrži `"Roman"` | greška: case-insensitive provjera |

---

#### Unit 5 – Upravljanje članarinom (US-73, US-74)

**Acceptance Criteria:** Sistem mora validirati da datum isteka nije prije datuma početka

| Test Case | Ulaz | Očekivani rezultat |
|-----------|------|-------------------|
| UT-35 | start: 01.01.2025, end: 01.01.2026 | validan |
| UT-36 | start: 01.01.2025, end: 31.12.2024 | greška: kraj prije početka |
| UT-37 | isti datumi | greška |
| UT-38 | start: prazno | greška |
| UT-39 | end: prazno | greška |

---

**Acceptance Criteria:** Kada je članarina aktivna, tada se prikazuje status "Aktivna"

| Test Case | Ulaz | Očekivani rezultat |
|-----------|------|-------------------|
| UT-40 | end: 31.12.2026, today: 12.04.2026 | Aktivna |
| UT-41 | end: 01.01.2026, today: 12.04.2026 | Istekla |
| UT-42 | end = today | Aktivna |

---

#### Unit 6 – Rezervacija knjige (US-90, US-91)

**Acceptance Criteria:** Sistem ne smije dozvoliti rezervaciju knjige koja nema dostupnih primjeraka; član ne može napraviti više aktivnih rezervacija iste knjige

| Test Case | Ulaz | Očekivani rezultat |
|-----------|------|-------------------|
| UT-43 | Dostupni primjerci, nema aktivne rezervacije | dozvoljeno |
| UT-44 | Ne postoje dostupni primjerci | zabranjeno |
| UT-45 | Primjerci dostupni, ali rezervacija već aktivna | zabranjeno |

---

#### Unit 7 – Brisanje knjige (US-25, US-28)

**Acceptance Criteria:** Kada knjiga ima aktivno zaduženje, tada sistem ne dozvoljava brisanje

| Test Case | Situacija | Očekivani rezultat |
|-----------|-----------|-------------------|
| UT-46 | Svi primjerci slobodni | dozvoljeno |
| UT-47 | Jedan primjerak zadužen | zabranjeno |
| UT-48 | Knjiga nema primjeraka | dozvoljeno |

---

#### Unit 8 – Deaktivacija primjerka (US-24)

**Acceptance Criteria:** Sistem ne dozvoljava deaktivaciju primjerka koji je trenutno aktivno zadužen

| Test Case | Situacija | Očekivani rezultat |
|-----------|-----------|-------------------|
| UT-49 | Primjerak slobodan (status: dostupan) | deaktivacija dozvoljena |
| UT-50 | Primjerak aktivo zadužen | deaktivacija zabranjena |

---

#### Rizici unit testiranja

Iako unit testiranje omogućava rano otkrivanje grešaka i provjeru ispravnosti pojedinih komponenti, sa sobom nosi određena ograničenja. Glavni rizici uključuju nemogućnost otkrivanja problema u interakciji između različitih komponenti sistema, što može stvoriti lažan osjećaj sigurnosti jer svi unit testovi mogu proći iako sistem u cjelini ne funkcioniše ispravno. Tu je i potreba za stalnim održavanjem testova pri izmjenama koda, kao i moguća nedovoljna pokrivenost testovima ako nisu obuhvaćeni svi scenariji.

---

### 2.2 Integraciono testiranje

Integraciono testiranje provjerava ispravnost komunikacije i saradnje između različitih modula sistema. Za razliku od unit testiranja, koje analizira komponente u izolaciji, integraciono testiranje fokus stavlja na tokove podataka i pozive između modula koji su opisani u Architecture Overview dokumentu.

Testovi su organizovani prema ključnim međumodulskim tokovima podataka.

---

#### Integracija 1 – Auth modul + Korisnici modul (US-04 do US-09)

| Test Case | Scenarij | Očekivani rezultat |
|-----------|----------|-------------------|
| IT-01 | Uspješna prijava → kreiranje sesije/tokena | Token se ispravno kreira i vraća klijentu |
| IT-02 | Prijava deaktiviranog korisnika | Backend odbija pristup; sesija se ne kreira |
| IT-03 | Pristup zaštićenoj ruti bez tokena | Backend vraća 401; klijent preusmjerava na prijavu |
| IT-04 | Pristup rute s nevažećim tokenom | Backend vraća 401 |
| IT-05 | Odjava → brisanje sesije | Nakon odjave, stari token ne daje pristup |

---

#### Integracija 2 – Katalog modul + Inventar modul (US-12 do US-24)

| Test Case | Scenarij | Očekivani rezultat |
|-----------|----------|-------------------|
| IT-06 | Dodavanje knjige s brojem primjeraka → primjerci se kreiraju | Baza sadrži tačan broj primjeraka sa statusom "dostupan" |
| IT-07 | Brisanje knjige s aktivnim zaduženjem | Backend odbija; poruka o grešci |
| IT-08 | Deaktivacija primjerka → provjera statusa dostupnosti knjige | Broj dostupnih primjeraka se smanjuje |
| IT-09 | Uređivanje podataka knjige → refleksija u katalogu | Katalog prikazuje ažurirane podatke |

---

#### Integracija 3 – Zaduženja modul + Inventar modul + Audit/Log modul (US-57 do US-65)

| Test Case | Scenarij | Očekivani rezultat |
|-----------|----------|-------------------|
| IT-10 | Zaduživanje primjerka → promjena statusa primjerka | Status primjerka mijenja se u "zadužen" atomski |
| IT-11 | Zaduživanje → upis u audit log | Audit log bilježi: ko je zadužio, koji primjerak, kada |
| IT-12 | Vraćanje knjige → oslobađanje primjerka | Status primjerka vraća se na "dostupan" |
| IT-13 | Vraćanje → upis u audit log | Audit log bilježi: ko je vratio, datum vraćanja |
| IT-14 | Zaduživanje knjige koja nema dostupnih primjeraka | Backend odbija; poruka klijentu |
| IT-15 | Paralelni zahtjevi za zaduženje istog primjerka | Samo jedan zahtjev uspijeva; drugi dobija grešku |

---

#### Integracija 4 – Korisnici modul + Članarina modul (US-73 do US-78)

| Test Case | Scenarij | Očekivani rezultat |
|-----------|----------|-------------------|
| IT-16 | Kreiranje naloga člana → provjera statusa članarine | Novi član nema aktivnu članarinu dok se ne kreira |
| IT-17 | Pokušaj zaduživanja bez aktivne članarine | Backend odbija; prikazuje se poruka |
| IT-18 | Ažuriranje članarine → refleksija u profilu člana | Status se ažurira bez odlaganja |

---

#### Integracija 5 – RBAC provjere (NFR-5)

| Test Case | Scenarij | Očekivani rezultat |
|-----------|----------|-------------------|
| IT-19 | Član pokušava pristupiti admin panelu | Backend vraća 403; pristup odbijen |
| IT-20 | Bibliotekar pokušava upravljati korisničkim ulogama | Backend vraća 403 |
| IT-21 | Administrator pristupa svim sekcijama | Pristup odobren |
| IT-22 | Direktan API poziv s niskim privilegijama | Backend provjerava i odbija; nije samo UI zaštita |

---

#### Integracija 6 – Rezervacije modul + Katalog modul (US-90 do US-94)

| Test Case | Scenarij | Očekivani rezultat |
|-----------|----------|-------------------|
| IT-23 | Kreiranje rezervacije → ažuriranje dostupnosti | Sistem evidentira rezervaciju, prikaz se ažurira |
| IT-24 | Preuzimanje rezervisane knjige → pretvaranje u zaduženje | Rezervacija se zatvara; zaduženje kreira |
| IT-25 | Automatsko otkazivanje rezervacije po isteku roka | Rezervacija se otkazuje; primjerak postaje dostupan |

---

#### Rizici integracionog testiranja

Integraciono testiranje povećava kompleksnost testnog okruženja jer zahtijeva ispravno postavljenu bazu podataka i dostupne module. Greške mogu biti teže dijagnosticirati jer uključuju više komponenti istovremeno. Postoji i rizik od lažno pozitivnih rezultata ako testno okruženje ne odgovara produkcijskom.

---

### 2.3 Sistemsko testiranje

Sistemsko testiranje provjerava sistem kao cjelinu – sa svim integriranim komponentama, kroz realne korisničke scenarije. Fokus je na end-to-end tokovima koji odgovaraju stvarnoj upotrebi sistema, te na provjeri nefunkcionalnih zahtjeva.

---

#### Sistemski scenarij 1 – Kompletan tok zaduživanja

**Pokriva:** US-01 do US-03, US-04, US-12 do US-16, US-57 do US-60, NFR-7

| Korak | Akcija | Očekivani rezultat |
|-------|--------|--------------------|
| 1 | Bibliotekar kreira nalog novog člana | Nalog se kreira, dodjeljuje se uloga Član |
| 2 | Bibliotekar kreira/ažurira članarinu za člana | Članarina je aktivna s ispravnim datumima |
| 3 | Član se prijavljuje u sistem | Preusmjeren na Član dashboard |
| 4 | Član pretražuje katalog po naslovu | Lista rezultata prikazuje odgovarajuće knjige |
| 5 | Bibliotekar evidentira zaduženje primjerka | Status primjerka postaje "zadužen", audit log bilježi akciju |
| 6 | Bibliotekar evidentira vraćanje | Status primjerka postaje "dostupan", zaduženje se zatvara |

---

#### Sistemski scenarij 2 – Upravljanje fondom biblioteke

**Pokriva:** US-12 do US-34, NFR-3, NFR-7

| Korak | Akcija | Očekivani rezultat |
|-------|--------|--------------------|
| 1 | Bibliotekar dodaje novu kategoriju | Kategorija se pojavljuje u listi i u formi za knjigu |
| 2 | Bibliotekar dodaje novu knjigu s primjercima | Knjiga i primjerci su vidljivi u katalogu |
| 3 | Bibliotekar uređuje podatke knjige | Izmjene su odmah vidljive u katalogu |
| 4 | Bibliotekar pokušava obrisati knjigu s aktivnim zaduženjem | Sistem prikazuje potvrdu, a zatim grešku |
| 5 | Bibliotekar deaktivira oštećen primjerak | Primjerak više nije dostupan za zaduživanje |
| 6 | Bibliotekar briše knjiga bez aktivnih zaduženja | Knjiga se uklanja iz kataloga |

---

#### Sistemski scenarij 3 – Kontrola pristupa po ulogama

**Pokriva:** US-08, US-09, NFR-5

| Korak | Akcija | Očekivani rezultat |
|-------|--------|--------------------|
| 1 | Neprijavljeni korisnik pristupa zaštićenoj stranici | Preusmjerenje na prijavu |
| 2 | Direktan unos URL-a zaštićene stranice | Ne prikazuje sadržaj, preusmjerava na prijavu |
| 3 | Član pokušava pristupiti admin panelu | Pristup odbijen |
| 4 | Bibliotekar pokušava upravljati korisničkim ulogama | Pristup odbijen |
| 5 | Administrator pristupa svim sekcijama | Pristup odobren za sve sekcije |
| 6 | Administrator deaktivira korisnika | Korisnik se ne može prijaviti ni s aktivnom sesijom |

---

#### Sistemski scenarij 4 – Nefunkcionalni zahtjevi

| Test Case | NFR | Scenarij | Kriterij prolaza |
|-----------|-----|----------|-----------------|
| ST-NF-01 | NFR-1 (Performanse) | Učitavanje stranice kataloga s knjigama | ≤ 2 sekunde na stabilnoj konekciji |
| ST-NF-02 | NFR-2 (Upotrebljivost) | Unos neispravnih podataka u svaku formu | Poruke greške jasne, bez tehničkih detalja |
| ST-NF-03 | NFR-3 (Upotrebljivost) | Brisanje knjige ili korisnika | Dijalog potvrde prikazan prije svake destruktivne akcije |
| ST-NF-04 | NFR-4 (Upotrebljivost) | Unos neispravnih podataka u formu | Poruka greške prikazana uz odgovarajuće polje |
| ST-NF-05 | NFR-5 (Sigurnost) | API pozivi s nižim privilegijama | Svaki neovlašten poziv odbijen na backendu |
| ST-NF-06 | NFR-6 (Sigurnost) | Pregled baze podataka | Lozinke su hashirane, nečitljive |
| ST-NF-07 | NFR-7 (Pouzdanost) | Zaduživanje primjerka | Knjiga nikad označena kao dostupna dok postoji aktivno zaduženje |
| ST-NF-08 | NFR-8 (Internacionalizacija) | Pregled svih ekrana | Svi tekstovi prikazani na bosanskom jeziku |
| ST-NF-09 | NFR-11 (Audit) | Izmjena podataka korisnika ili knjige | Audit log sadrži evidenciju akcije |

---

#### Rizici sistemskog testiranja

Sistemsko testiranje je vremenski najzahtjevnije jer pokriva end-to-end tokove i zahtijeva kompleksno testno okruženje. Identificiranje uzroka grešaka je teže nego na nižim nivoima. Postoji rizik od nedovoljne pokrivenosti rubnih scenarija ako su test slučajevi preopćeniti.

---

### 2.4 Prihvatno testiranje (UAT)

Prihvatno testiranje provjerava da li sistem zadovoljava poslovne zahtjeve i potrebe krajnjih korisnika iz perspektive korisnika, a ne iz tehničke perspektive. Testiranje se vrši scenarijski, simulirajući stvarnu upotrebu sistema u bibliotečkom okruženju.

---

#### UAT scenarij 1 – Perspektiva Člana

| Test Case | Scenarij | Kriterij prihvatanja |
|-----------|----------|---------------------|
| UAT-01 | Prijava u sistem | Korisnik se uspješno prijavljuje i vidi odgovarajući dashboard |
| UAT-02 | Pretraga knjige | Korisnik pronalazi željenu knjigu pretragom po naslovu ili autoru |
| UAT-03 | Pregled dostupnosti | Korisnik vidi da li je knjiga dostupna i koliko ima primjeraka |
| UAT-04 | Pregled vlastitih zaduženja | Korisnik vidi koje knjige ima zadužene i rokove vraćanja |
| UAT-05 | Pregled statusa članarine | Korisnik vidi je li njegova članarina aktivna i kada ističe |
| UAT-06 | Rezervacija knjige | Korisnik uspješno rezerviše nedostupnu knjigu |

---

#### UAT scenarij 2 – Perspektiva Bibliotekara

| Test Case | Scenarij | Kriterij prihvatanja |
|-----------|----------|---------------------|
| UAT-07 | Kreiranje naloga novog člana | Bibliotekar uspješno evidentira novog člana |
| UAT-08 | Dodavanje knjige u katalog | Nova knjiga vidljiva svim korisnicima odmah |
| UAT-09 | Evidencija zaduživanja | Zaduženje evidentirano; status primjerka ažuriran |
| UAT-10 | Evidencija vraćanja | Vraćanje evidentirano; primjerak slobodan |
| UAT-11 | Pregled aktivnih zaduženja | Bibliotekar vidi ko šta ima zaduženo |
| UAT-12 | Upravljanje članarinom člana | Bibliotekar može ažurirati status i datume članarine |

---

#### UAT scenarij 3 – Perspektiva Administratora

| Test Case | Scenarij | Kriterij prihvatanja |
|-----------|----------|---------------------|
| UAT-13 | Upravljanje korisnicima | Administrator može pregledati, mijenjati ulogu ili deaktivirati korisnika |
| UAT-14 | Pregled audit loga | Administrator može pregledati evidenciju kritičnih akcija |
| UAT-15 | Upravljanje kategorijama | Administrator može dodavati, uređivati i brisati kategorije |

---

#### Rizici prihvatnog testiranja

Prihvatno testiranje zahtijeva angažman krajnjih korisnika koji možda nemaju tehničko predznanje, što može otežati precizno izvještavanje o greškama. Postoji rizik da se subjektivna procjena korisničkog iskustva razlikuje od tehničkih kriterija. Vremenski pritisak u završnim sprintovima može skratiti vrijeme dostupno za UAT.

---

## 3. Šta se testira u kojem nivou

| Funkcionalnost | Unit | Integracija | Sistemski | UAT |
|----------------|:----:|:-----------:|:---------:|:---:|
| Validacija email adrese | X | | | |
| Validacija lozinke | X | | | |
| Validacija obaveznih polja forme | X | | | |
| Validacija ISBN-a | X | | | |
| Provjera dostupnosti primjeraka | X | | | |
| Provjera statusa членarine | X | | | |
| Validacija datuma членarine | X | | | |
| Zabrana rezervacije duplikata | X | | | |
| Zabrana brisanja zadužene knjige | X | | | |
| Zabrana deaktivacije zaduženog primjerka | X | | | |
| Auth modul ↔ Korisnici modul | | X | | |
| Katalog modul ↔ Inventar modul | | X | | |
| Zaduženja modul ↔ Audit log | | X | | |
| Zaduživanje u jednoj transakciji | | X | | |
| RBAC provjere na API nivou | | X | | |
| Rezervacije ↔ Katalog | | X | | |
| End-to-end tok zaduživanja | | | X | |
| End-to-end upravljanje fondom | | | X | |
| Kontrola pristupa po ulogama | | | X | |
| Performanse učitavanja (≤2s) | | | X | |
| Sigurnost (hashiranje lozinki) | | | X | |
| Audit log – evidentiranje akcija | | | X | |
| Upotrebljivost forme i poruke grešaka | | | X | |
| Prijava, pretraga i pregled kataloga | | | | X |
| Evidencija zaduživanja i vraćanja | | | | X |
| Upravljanje članovima i korisnicima | | | | X |

---

## 4. Veza sa acceptance kriterijima

Svaki test slučaj direktno je vezan za jedan ili više acceptance kriterija definisanih u dokumentu *Set of User Stories*. Sljedeća tabela prikazuje ključne veze:

| Test slučaj(evi) | User Story | Acceptance Criteria |
|-----------------|-----------|---------------------|
| UT-01 do UT-05 | US-02 | Email mora biti u ispravnom formatu |
| UT-06 do UT-10 | US-02 | Lozinka mora imati najmanje 8 znakova |
| UT-11 do UT-16 | US-01 | Sva obavezna polja moraju biti popunjena |
| UT-17 do UT-20 | US-05, US-09 | Prijava se odbija za pogrešne ili deaktivirane kredencijale |
| UT-21 do UT-25 | US-13 | ISBN mora biti u ispravnom formatu i jedinstven |
| UT-26 do UT-30 | US-12, US-14 | Obavezna polja knjige; broj primjeraka ne smije biti 0 |
| UT-31 do UT-34 | US-30, US-33 | Kategorija mora biti jedinstvena i neprazna |
| UT-35 do UT-39 | US-73 | Datum isteka ne smije biti prije datuma početka |
| UT-40 do UT-42 | US-74 | Status aktivne/istekle членarine ispravno se prikazuje |
| UT-43 do UT-45 | US-90 | Rezervacija nije dozvoljena bez dostupnih primjeraka ili ako već postoji |
| UT-46 do UT-48 | US-28 | Brisanje knjige nije dozvoljeno uz aktivno zaduženje |
| UT-49 do UT-50 | US-24 | Deaktivacija primjerka nije dozvoljena dok je zadužen |
| IT-01 do IT-05 | US-04, US-06, US-07, US-08, US-09 | Sesija se kreira/briše ispravno; zaštićene rute su blokirane |
| IT-06 do IT-09 | US-12, US-14, US-25, US-28 | Katalog i primjerci su konzistentni |
| IT-10 do IT-15 | US-57, US-58, US-59 | Atomska transakcija zaduživanja; audit log evidentira akcije |
| IT-16 do IT-18 | US-73, US-74, US-77 | Članarina blokira zaduživanje ako nije aktivna |
| IT-19 do IT-22 | US-08, NFR-5 | RBAC kontrola pristupa na API nivou |
| ST-NF-01 | NFR-1 | Stranice se učitavaju unutar 2 sekunde |
| ST-NF-02, ST-NF-04 | NFR-2, NFR-4 | Jasne poruke grešaka uz odgovarajuća polja |
| ST-NF-03 | NFR-3 | Potvrda prije destruktivnih akcija |
| ST-NF-05, ST-NF-06 | NFR-5, NFR-6 | Sigurnost: RBAC i hashiranje lozinki |
| ST-NF-07 | NFR-7 | Konzistentnost statusa primjerka |
| ST-NF-09 | NFR-11 | Audit log evidentira administratorske akcije |

---

## 5. Način evidentiranja rezultata testiranja

### 5.1 Evidencija bug prijava

Svaka pronađena greška evidentira se kao bug izvještaj koji sadrži sljedeće informacije:

| Polje | Opis |
|-------|------|
| **Bug ID** | Jedinstveni identifikator (npr. BUG-001) |
| **Naziv** | Kratak opisni naslov greške |
| **Test Case** | ID test slučaja koji je otkrio grešku (npr. UT-07, IT-10) |
| **User Story** | Povezani US (npr. US-02) |
| **Opis** | Detaljan opis pronađene greške |
| **Koraci za reprodukciju** | Precizni koraci kojima se greška može reproducirati |
| **Očekivano ponašanje** | Šta bi sistem trebao uraditi |
| **Stvarno ponašanje** | Šta sistem zapravo radi |
| **Ozbiljnost** | Kritično / Visoko / Srednje / Nisko |
| **Prioritet popravke** | Visok / Srednji / Nizak |
| **Status** | Otvoreno / U rješavanju / Riješeno / Zatvoreno |
| **Datum prijave** | Datum kada je bug evidentiran |
| **Datum rješavanja** | Datum kada je bug ispravljen |

### 5.2 Evidencija rezultata test slučajeva

Rezultati testiranja bilježe se u tabelarnom formatu po završetku svake testne sesije, kao na primjer:

| Test Case ID | User Story | Naziv testa | Status | Datum izvođenja | Napomena |
|-------------|------------|-------------|--------|-----------------|----------|
| UT-01 | US-02 | Validacija ispravnog emaila | Prošao | 15.05.2025 | |
| UT-02 | US-02 | Email bez @ znaka | Prošao | 15.05.2025 | |
| UT-07 | US-02 | Kratka lozinka | Pao | 15.05.2025 | BUG-001 |

### 5.3 Skup statusa testiranja

| Status | Značenje |
|--------|----------|
| Prošao | Test slučaj izveden, sistem se ponašao prema očekivanju |
| Pao | Test slučaj izveden, sistem se nije ponašao prema očekivanju |
| Preskočen | Test slučaj nije izveden (npr. preduslov nije ispunjen) |
| Blokiran | Test nije moguće izvesti zbog prethodno pronađene greške |

### 5.4 Pohranjivanje rezultata

Svi rezultati testiranja, uključujući evidenciju bug prijava i tabele test slučajeva, pohranjuju se u projektnom repozitoriju.

---

## 6. Glavni rizici kvaliteta

| ID | Rizik | Vjerovatnoća | Utjecaj | Mjere ublažavanja |
|----|-------|:------------:|:-------:|-------------------|
| RQ-01 | **Nedovoljna pokrivenost testovima** – nisu obuhvaćeni svi rubni scenariji funkcionalnosti | Srednja | Visok | Redovni pregled test slučajeva pri svakom sprintu; review acceptance kriterija |
| RQ-02 | **Kašnjenje u otkrivanju grešaka** – greške otkrivene tek u sistemskom testiranju | Srednja | Visok | Obavezno unit testiranje pri implementaciji svake komponente |
| RQ-03 | **Nekonzistentnost podataka pri paralelnim zahtjevima** – dva korisnika istovremeno zadužuju isti primjerak | Niska | Kritičan | Transakcije i zaključavanje na nivou baze podataka (IT-15) |
| RQ-04 | **Propusti u kontroli pristupa** – korisnik s nižim privilegijama pristupa zabranjenim funkcijama | Niska | Kritičan | RBAC testiranje i na UI i na API nivou (IT-19 do IT-22); sigurnosni review koda |
| RQ-05 | **Nestabilno testno okruženje** – rezultati testiranja ne odgovaraju produkcijskom okruženju | Srednja | Srednji | Što je moguće više uskladiti konfiguraciju testnog i produkcijskog okruženja |
| RQ-06 | **Regresione greške** – ispravka jedne greške uzrokuje novu u drugom dijelu sistema | Srednja | Visok | Ponavljanje ključnih test slučajeva pri svakoj novoj verziji |
| RQ-07 | **Nedovoljna provjera nefunkcionalnih zahtjeva** – performanse i sigurnost se ne testiraju do kasnih sprintova | Visoka | Srednji | Uključiti NFR testove (ST-NF-01 do ST-NF-09) u sistemsko testiranje od Sprinta 8 |
| RQ-08 | **Tehnički dug koji otežava testiranje** – kod koji nije modularan teško je testirati izolovano | Srednja | Srednji | Modularna arhitektura (NFR-9); redovni code review |
| RQ-09 | **Neadekvatna dokumentacija grešaka** – bug nije dovoljno opisan za reprodukciju | Srednja | Srednji | Obavezan format bug prijave (Bug ID, koraci, očekivano/stvarno ponašanje) |
| RQ-10 | **Preskakanje prihvatnog testiranja** – sistem prolazi tehničke testove ali ne zadovoljava korisnička očekivanja | Niska | Visok | UAT scenariji planirani od Sprinta 11 uz uključivanje finalnih korisnika |

---

*Ovaj dokument predstavlja Test Strategy za Sprint 3. Detalji implementacije test slučajeva, kao i rezultati testiranja, bit će razrađeni i evidentirani u sklopu kasnijih sprintova (Sprint 8 – Sprint 11) kako sistem bude implementiran.*
