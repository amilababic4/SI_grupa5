# SmartLib — Bibliotečki informacioni sistem
## Izvještaj o testiranju — Sprint 5 & 6

**Datum kreiranja izvještaja:** 06.05.2026.  
**Okruženje:** Development / Test (In-Memory DB), Chrome (za UI testove)  
**Alati:** xUnit, WebApplicationFactory, Browser DevTools, Playwright, Fine Code Coverage 

---

## 1. Pregled testiranja
Ovaj dokument predstavlja formalni izvještaj o testiranju provedenom u okviru Sprinta 5 i 6 projekta SmartLib. Testiranje je provedeno u skladu sa definiranom Test strategijom (Sprint 3) i obuhvata sve implementirane funkcionalnosti (Knjige, Kategorije, Primjerci, Korisnici, Autentifikacija).

| Ukupno testova | Prošlo | Preskočeno (Skip) | Greška |
| :--- | :--- | :--- | :--- |
| **371** | **371** | **0** | **0** |

---

## 2. Nivoi testiranja — pregled aktivnosti

### 2.1 Unit testiranje

**Alat:** xUnit + Moq (.NET)  
**Pristup:** Izolirani testovi sa mock repozitorijima — bez baze podataka. Svaki test provjerava jednu konkretnu poslovnu logiku ili HTTP odgovor kontrolera.

**Podjela:** Testovi su podijeljeni u dvije grupe:
- **API kontroleri** — testiraju JSON odgovore i HTTP status kodove (`200`, `201`, `400`, `404`, `409`)
- **Web kontroleri** — testiraju View rezultate, redirect logiku i TempData poruke (`SuccessMessage`, `ErrorMessage`)

**Analiza pokrivenosti:** Za mjerenje pokrivenosti testova korišten je alat **Fine Code Coverage**. Postignuti su sljedeći rezultati:
* **Line Coverage:** 100% (svaka linija koda u testiranim komponentama je izvršena).
* **Branch Coverage:** ~97% (gotovo svi logički putevi su validirani).

**Rezultati testiranja:**
Svi planirani unit testovi (ukupno 221) su uspješno izvršeni. 

![Rezultati unit testiranja](./images/rezultati-unit-testiranja.png)

[**Prikaži detaljan izvještaj svih unit testnih slučajeva**](#detaljni-izvjestaj-unit)

> **Napomena:**  
>Svi unit testovi su implementirani prateći dvije ključne prakse za osiguranje čitljivosti i održivosti:
>
> **AAA (Arrange-Act-Assert) obrazac:** Testovi su logički podijeljeni u tri prepoznatljiva dijela:
> * **Arrange:** Priprema okruženja, inicijalizacija objekata i konfiguracija mock-ova.
> * **Act:** Izvršavanje konkretne metode koja se testira.
> * **Assert:** Provjera da li je rezultat (povratna vrijednost ili stanje) u skladu sa očekivanjima.
>
> **Osherova konvencija imenovanja:** Testovi su imenovani prema metodologiji koju zagovara **Roy Osherove**, u formatu:
> `[NazivMetode]_[Scenario]_[OcekivanoPonasanje]`

---

### 2.2 Penetracijsko / Sigurnosno testiranje
 
**Alat:** xUnit + WebApplicationFactory (.NET)  
**Pristup:** Simuliran je puni HTTP pipeline sa in-memory bazom podataka i stvarnim JWT middleware-om. Testovi šalju prave HTTP zahtjeve prema API endpointima i validiraju odgovore na nivou status kodova i sadržaja tijela odgovora — bez mockovanja.  
**Podjela:** Testovi su podijeljeni u četiri grupe:
 
- **Autentifikacija i SQL Injection** — testiraju otpornost login endpointa na zlonamjerne unose i valjanost JWT mehanizma
- **XSS zaštita** — testiraju da li sistem odbija ili neutralizira skripte u korisničkim unosima
- **Granične vrijednosti** — testiraju ponašanje sistema na rubnim i nevažećim ulazima
> **Napomena:** Provjere 401 bez tokena i 403 za eskalaciju privilegija (RBAC) namjerno su izostavljene iz sigurnosnih testova jer su u potpunosti pokrivene integracijskim testovima (Auth, Korisnik, Kategorija, Knjiga, Primjerak).
 
**Rezultati testiranja:**  
Svi planirani sigurnosni testovi su uspješno izvršeni.
 
![Rezultati sigurnosnog testiranja](./images/rezultati-sec-testiranja.png)
 
[**Prikaži detaljan izvještaj svih penetracijskih / sigurnosnih testnih slučajeva**](#detaljni-izvjestaj-security)
 
> **Pokriveni vektori napada:**
> * **PT-01 / PT-02:** SQL Injection u email i lozinka polju (US-04, US-05)
> * **PT-03:** Brute Force napad na login (US-04)
> * **PT-04 / PT-05:** Lažni i modificirani JWT token (US-08)
> * **PT-06:** Arhitekturalni rizik — stari JWT deaktiviranog korisnika (US-09)
> * **PT-07 / PT-08:** XSS u registracijskom obrascu i nazivu kategorije (US-01, US-02, US-30)
> * **PT-09:** Path Traversal / Injection u ISBN polju (US-25)


---

### 2.3 Regresiono testiranje

**Cilj:** Potvrda stabilnosti implementiranog rješenja osiguravanjem da nove izmjene u kodu ne narušavaju rad postojećih, prethodno validiranih funkcionalnosti unutar tekuće razvojne faze.

**Proces:** Regresiono testiranje se provodilo periodično, nakon svake značajne promjene u kodu ili ispravke defekata. Proces je obuhvatao:
- Ručno pokretanje kompletnog seta unit i integracijskih testova unutar Test Explorer-a, kako bi se osiguralo da su svi testovi uspješno izvršeni (status *Passed*).
- Ponovni prolazak kroz kritične UAT scenarije, s ciljem potvrde da korisnički interfejs i osnovne funkcionalnosti sistema ostaju stabilne.

**Rezultati:** Izvršavanjem regresionih testova potvrđeno je, uz manje probleme da sistem zadržava stabilnost nakon uvedenih izmjena.

#### Značajnije stavke obuhvaćene regresionim testiranjem:

* **Integritet brisanja i zavisnosti:**
    Nakon implementacije pravila da se kategorija ne može obrisati ako sadrži knjige (Sprint 6), izvršena je regresija nad modulom Knjiga. Potvrđeno je da brisanje same knjige i dalje ispravno funkcioniše i da ne narušava stabilnost preostalih podataka u bazi niti integritet relacija.

* **Normalizacija ISBN unosa:**
    Nakon što je dodata logika koja automatski uklanja crtice iz ISBN-a, regresijom je potvrđeno da sistem ispravno pohranjuje očišćene podatke i da se oni konzistentno prikazuju u detaljima knjige. 

* **Konzistentnost UI poruka (TempData):**
    Regresiono je verificirano da sigurnosni filteri ne blokiraju standardne sistemske poruke (npr. *"Korisnik uspješno kreiran"*), čime je osiguran kontinuitet vizuelnog feedbacka prema korisniku.

* **Paginacija i filtriranje:**
    Nakon značajnog povećanja broja testnih podataka u bazi tokom Sprinta 6, ponovo je testirana paginacija u katalogu. Potvrđeno je da sistem ispravno raspoređuje knjige po stranicama i da navigacija funkcioniše bez gubitka sinhronizacije podataka u prikazu.

---

### 2.4 UAT (User Acceptance Testing) 

Sprovedeno je manuelno prihvatno testiranje (UAT) od strane svih članova tima, u skladu sa definisanim Acceptance Criteria iz Sprint Backloga.

Testiranje je obuhvatilo ključne funkcionalnosti:
- autentifikaciju i autorizaciju korisnika
- upravljanje knjigama
- upravljanje primjercima
- upravljanje kategorijama
- pregled kataloga

[**Prikaži detaljan izvještaj svih UAT scenarija**](#detaljni-izvjestaj-uat)

**Rezultati testiranja:**
Svi scenariji su testirani kroz UI (browser) i validirani očekivani ishodi.
Manuelnim UAT testiranjem potvrđeno je da implementirane funkcionalnosti ispunjavaju sve definisane Acceptance Criteria iz Sprint Backloga. 

---

## 3. Evidencija pronađenih grešaka

### **BG-01: Konfiguracija In-Memory baze (SecurityTests.cs)**
* **Opis:** Korištenje `Guid.NewGuid()` u imenu baze unutar `CreateClient()` uzrokovalo je da svaki request dobije novu, praznu bazu. Login testovi su padali jer "seeder" nije bio u istoj bazi.
* **Rješenje:** Ime baze fiksirano pomoću statičkog polja: `private static readonly string _dbName = "TestDb_" + Guid.NewGuid();`.
* **Status:** Riješeno

### **BG-02: XSS ranjivost u Controllerima**
* **Opis:** `KategorijaController` i `KorisnikController` su dozvoljavali pohranu `<script>` tagova.
* **Rješenje:** Implementirana `SadrziHtml()` pomoćna metoda i dodana validacija u `Create` i `Update` akcije.
* **Status:** Riješeno

### **BG-03: Problem sa "Lazy Loading" u API odgovorima (PrimjerakController.cs)**
* **Opis:** Prilikom poziva `/api/primjerak/{id}`, polje `knjiga` (naslov) je vraćalo `null`, iako je `KnjigaId` bio ispravan. Entity Framework Core ne učitava navigacijske entitete automatski, što je uzrokovalo prazne podatke u JSON odgovoru.
* **Rješenje:** U `PrimjerakController` klasi, unutar LINQ upita, dodana je metoda `.Include(p => p.Knjiga)` kako bi se osiguralo "Eager Loading" (prijevremeno učitavanje) povezanog entiteta knjige.
* **Status:** Riješeno

### **BG-04: Email normalizacija pri registraciji i prijavi**
* **Opis:** Korisnik registrovan sa emailom `Korisnik@SmartLib.ba` mogao se prijaviti samo tim emailom — unos `korisnik@smartlib.ba` vraćao je `401 Unauthorized` jer login logika nije normalizovala email prije pretrage u bazi.
* **Rješenje:** Dodan `.ToLower()` na email polje u register i login logici prije upita u bazu: `email = email.ToLower()`.
* **Status:** Riješeno

---


<br>

<a name="detaljni-prikaz"></a>
## 4. Detaljan prikaz svih testnih slučajeva po nivoima testiranja

<a name="detaljni-izvjestaj-unit"></a>
### 4.1 Unit testovi — Detaljna lista

#### 4.1.1 API Kontroleri — Unit testovi

##### Auth API (`AuthApiControllerTests`)

Ovi testovi validiraju kompletan proces autentifikacije, od sigurnosne validacije ulaza do ispravnosti generisanog JWT tokena i zaštite sistema od neovlaštenog pristupa.

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:-:|:---|
| 1 | Login_ValidanEmailILozinka_VracaOkSaTokenom | Provjera uspješne prijave korisnika | 1. Unijeti validan email i lozinku<br>2. Kliknuti na dugme za prijavu | HTTP 200 OK + JWT token u odgovoru | Token generisan i vraćen | US-04 | Prošao |
| 2 | Login_UspjesanLogin_ResponseSadrziIme | Provjera da li se ime korisnika vraća u response | 1. Validan login | Ime korisnika prisutno u odgovoru | Ime vraćeno | US-04 | Prošao |
| 3 | Login_UspjesanLogin_ResponseSadrziPrezime | Validacija prezimena u response objektu | 1. Validan login | Prezime prisutno u odgovoru | Prezime vraćeno | US-04 | Prošao |
| 4 | Login_UspjesanLogin_ResponseSadrziUlogu | Provjera uloge korisnika u response | 1. Validan login | Ispravna uloga vraćena | Uloga vraćena | US-04 | Prošao |
| 5 | Login_UspjesanLogin_TokenSadrziTacniEmail | Validacija email claim-a u JWT tokenu | 1. Login<br>2. Dekodiranje tokena | Email claim odgovara korisniku | Validan claim | US-04 | Prošao |
| 6 | Login_UspjesanLogin_TokenSadrziTacnuUlogu | Validacija role claim-a u JWT tokenu | 1. Login<br>2. Dekodiranje tokena | Role claim ispravan | Validan claim | US-04 | Prošao |
| 7 | Login_UspjesanLogin_TokenSadrziTacniKorisnikId | Validacija korisničkog ID-a u tokenu | 1. Login<br>2. Dekodiranje tokena | ID u NameIdentifier claim-u | ID = 1 | US-04 | Prošao |
| 8 | Login_UspjesanLogin_TokenJeTrenutnoValidan | Validacija JWT potpisa i strukture | Validacija tokena | Token je validan | Token validan | US-04 | Prošao |
| 9 | Login_NetacnaLozinka_VracaUnauthorized | Pogrešna lozinka blokira pristup | 1. Email validan<br>2. Pogrešna lozinka | HTTP 401 Unauthorized | 401 vraćen | US-05 | Prošao |
| 10 | Login_NepostojeciEmail_VracaUnauthorized | Nepostojeći korisnik | 1. Nevalidan email | HTTP 401 Unauthorized | 401 vraćen | US-05 | Prošao |
| 11 | Login_Neuspjeh_PorukaJeGenericka | Sigurnosna provjera poruke greške | 1. Neuspješan login | Generička poruka bez detalja | OK | US-05 | Prošao |
| 12 | Login_NetacnaLozinka_IDeaktiviran_PorukaIsta | Uniformna greška za sve slučajeve | Deaktiviran + nepostojeći korisnik | Ista poruka greške | Identicno | US-05 | Prošao |
| 13 | Logout_VracaOkSaPorukom | Logout endpoint | Pozvati Logout | HTTP 200 OK | OK | US-06 | Prošao |
| 14 | Logout_ResponseSadrziPoruku | Provjera poruke nakon odjave | Logout | Poruka potvrde prisutna | OK | US-06 | Prošao |
| 15 | Login_UspjesanLogin_TokenIsteceZa60Minuta | Validacija trajanja sesije | Login + provjera expiry | Token važi 60 min | OK | US-07 | Prošao |
| 16 | Login_DeaktiviranKorisnik_VracaUnauthorized | Blokada deaktiviranog korisnika | Login deaktiviranog korisnika | HTTP 401 | 401 | US-09 | Prošao |
| 17 | Login_DeaktiviranKorisnik_NeDobivaToken | Deaktiviran korisnik nema token | Login | Nema tokena | OK | US-09 | Prošao |
| 18 | Login_NeispravanModel_VracaBadRequest | Validacija model state | Prazna/nevalidna polja | HTTP 400 | 400 | US-04 | Prošao |
| 19 | Login_PrazanEmail/Lozinka_NePozivaBazu | Optimizacija prije DB poziva | Prazan input | Repository se ne poziva | OK | US-04 | Prošao |

---

##### Knjiga API (`KnjigaApiControllerTests`)

Ovi unit testovi pokrivaju CRUD operacije nad knjigama, validaciju ISBN formata, te ključnu poslovnu logiku koja sprječava brisanje knjiga koje su trenutno kod korisnika.

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:-:|:---|
| 1 | GetById_KnjigaPostoji_VracaOkIObjekt | Dohvatanje knjige po ID-u | 1. Pozvati GET /api/knjiga/{id} | HTTP 200 OK + KnjigaDto | DTO vraćen | US-13 | Prošao |
| 2 | GetById_KnjigaNePostoji_VracaNotFound | Nepostojeća knjiga | 1. Pozvati GET sa nevalidnim ID | HTTP 404 Not Found | 404 vraćen | US-13 | Prošao |
| 3 | Create_ValidnaKnjiga_Vraca201Created | Kreiranje knjige | 1. Poslati validan DTO | HTTP 201 Created | Knjiga kreirana | US-12 | Prošao |
| 4 | Create_NeispravanIsbn_VracaBadRequest | Validacija ISBN-a | 1. Poslati nevalidan ISBN | HTTP 400 BadRequest | Greška vraćena | US-13 | Prošao |
| 5 | Create_DupliIsbn_VracaConflict | Duplikat ISBN-a | 1. Poslati postojeći ISBN | HTTP 409 Conflict | Conflict vraćen | US-13 | Prošao |
| 6 | Update_PogresanId_VracaBadRequest | Neusklađen ID u URL i body | 1. Neusklađenost identifikatora | HTTP 400 BadRequest | Greška vraćena | US-17 | Prošao |
| 7 | Delete_KnjigaImaAktivnaZaduzenja_VracaBadRequest | Brisanje sa aktivnim zaduženjima | 1. Knjiga ima zaduženja<br>2. DELETE | HTTP 400 BadRequest | Brisanje blokirano | US-28 | Prošao |
| 8 | Delete_Uspjesno_VracaNoContent | Uspješno brisanje knjige | 1. Knjiga bez zaduženja<br>2. DELETE | HTTP 204 NoContent | Knjiga obrisana | US-27 | Prošao |

---

##### Kategorija API (`KategorijaApiControllerTests`)

Ovi unit testovi pokrivaju kompletan CRUD ciklus nad kategorijama knjiga, sa posebnim fokusom na integritet podataka (sprječavanje duplikata) i poslovnu logiku koja štiti relacije u bazi (zabrana brisanja kategorija koje imaju knjige).

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:-:|:---|
| 1 | GetAll_VracaOkSaListomKategorija | Dohvatanje svih kategorija | 1. Pozvati GET /api/kategorija | HTTP 200 OK + lista kategorija | Lista vraćena | US-31 | Prošao |
| 2 | GetAll_NemaKategorija_VracaOkSaPraznomListom | Prazna baza kategorija | 1. GET bez podataka | HTTP 200 OK + [] | Prazna lista | US-31 | Prošao |
| 3 | GetById_KategorijaPostoji_VracaOkIObjekt | Dohvatanje kategorije po ID-u | 1. GET /api/kategorija/{id} | HTTP 200 OK + objekt | Objekt vraćen | US-31 | Prošao |
| 4 | GetById_KategorijaNePostoji_VracaNotFound | Nepostojeća kategorija | 1. GET nevalidan ID | HTTP 404 Not Found | 404 vraćen | US-31 | Prošao |
| 5 | Create_ValidanRequest_Vraca201Created | Kreiranje kategorije | 1. Validan POST request | HTTP 201 Created | Kategorija kreirana | US-30 | Prošao |
| 6 | Create_ValidanRequest_SpremaSaIspravnimPodacima | Sanitizacija unosa (trim) | 1. Unijeti naziv sa razmacima | Naziv trimovan u bazi | Podaci ispravni | US-30 | Prošao |
| 7 | Create_PrazanNaziv_VracaBadRequest | Validacija obaveznog polja | 1. Prazan naziv | HTTP 400 BadRequest | Greška vraćena | US-30 | Prošao |
| 8 | Create_NazivVecPostoji_VracaConflict | Sprječavanje duplikata | 1. Postojeći naziv | HTTP 409 Conflict | Conflict vraćen | US-30 | Prošao |
| 9 | Create_NazivCaseInsensitive_VracaConflict | Case-insensitive provjera duplikata | 1. "nauka" vs "Nauka" | HTTP 409 Conflict | Conflict vraćen | US-30 | Prošao |
| 10 | Create_NazivVecPostoji_NePozivaSeSprema | Sigurnost repository poziva | 1. Duplikat naziv | CreateAsync se ne poziva | DB nije pozvan | US-30 | Prošao |
| 11 | Create_NeispravanModel_VracaBadRequest | Validacija ModelState | 1. Nevalidan model | HTTP 400 BadRequest | 400 vraćen | US-30 | Prošao |
| 12 | Update_PostojecaKategorija_AzuriraPodatkeIVracaOk | Ažuriranje kategorije | 1. Validan PUT | HTTP 200 OK + izmjene | Podaci ažurirani | US-33 | Prošao |
| 13 | Update_PrazanNaziv_VracaBadRequest | Validacija naziva pri update-u | 1. Prazan naziv | HTTP 400 BadRequest | Greška vraćena | US-33 | Prošao |
| 14 | Update_NepostojecaKategorija_VracaNotFound | Update nepostojeće kategorije | 1. Nevalidan ID | HTTP 404 Not Found | 404 vraćen | US-33 | Prošao |
| 15 | Update_NazivVecPostojiKodDrugeKategorije_VracaConflict | Konflikt naziva | 1. Naziv već postoji | HTTP 409 Conflict | Conflict vraćen | US-33 | Prošao |
| 16 | Update_IstaNazivIstaKategorija_DozvoljenoAzuriranje | Dozvoljen update bez promjene naziva | 1. Samo opis promijenjen | HTTP 200 OK | OK | US-33 | Prošao |
| 17 | Delete_KategorijaNemaKnjige_BriseIVracaOk | Brisanje prazne kategorije | 1. DELETE bez knjiga | HTTP 200 OK | Obrisano | US-34 | Prošao |
| 18 | Delete_KategorijaImaKnjige_VracaConflict | Zabrana brisanja kategorije sa knjigama | 1. Kategorija ima knjige | HTTP 409 Conflict | Brisanje blokirano | US-32 | Prošao |
| 19 | Delete_KategorijaImaKnjige_PorukaObjasnjavaRazlog | Jasna poruka o zabrani | 1. DELETE sa knjigama | Objašnjenje u response-u | Poruka vraćena | US-32 | Prošao |
| 20 | Delete_KategorijaNePostoji_VracaNotFound | Brisanje nepostojeće kategorije | 1. Nevalidan ID | HTTP 404 Not Found | 404 vraćen | US-34 | Prošao |

---

##### Korisnik API (`KorisnikApiControllerTests`)

Ovi testovi validiraju proces registracije novih članova, strogu validaciju ulaznih podataka (ime, prezime, email, lozinka), automatsko dodjeljivanje uloga, sigurnosno heširanje lozinki, te upravljanje statusom korisnika (deaktivacija).

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:---|:---|
| 1 | Create_ValidanModel_VracaCreated201 | Uspješna registracija korisnika | 1. Kreirati validan DTO<br>2. Pozvati Create endpoint | 201 Created | 201 Created | US-01 | Prošao |
| 2 | Create_NoviKorisnik_UlogaIdJe1Clan | Automatsko dodjeljivanje uloge i statusa | 1. Kreirati korisnika<br>2. Provjeriti snimljeni objekat | UlogaId = 1, Status = "aktivan" | UlogaId = 1, Status = "aktivan" | US-03 | Prošao |
| 3 | Create_DuplikatEmail_VracaValidationProblem | Sprječavanje duplog emaila | 1. Postaviti postojeći email<br>2. Pozvati Create | ValidationProblem | ValidationProblem | US-02 | Prošao |
| 4 | Create_LozinkaSeHashuje_NijeChuvanaKaoPlainText | Provjera sigurnosnog hashiranja lozinke | 1. Poslati lozinku<br>2. Provjeriti snimljeni objekat | Lozinka nije u plain textu | Lozinka hashirana | US-02 | Prošao |
| 5 | GetAll_VracaListuKorisnika | Dohvatanje liste korisnika | 1. Pozvati GetAll endpoint | Lista korisnika | Lista korisnika | US-49 | Prošao |
| 6 | GetById_NepostojeciId_VracaNotFound | Dohvatanje nepostojećeg korisnika | 1. Poslati nevalidan ID | 404 NotFound | 404 NotFound | US-49 | Prošao |
| 7 | Deactivate_PostojeciKorisnik_SetujujeStatusNaDeaktiviran | Deaktivacija korisnika | 1. Kreirati korisnika<br>2. Pozvati Deactivate | Status = "deaktiviran", 204 NoContent | Status = "deaktiviran", 204 NoContent | US-09 | Prošao |
| 8 | Deactivate_NepostojećiId_VracaNotFound | Deaktivacija nepostojećeg korisnika | 1. Poslati nevalidan ID | 404 NotFound | 404 NotFound | US-09 | Prošao |
| 9 | Validacija_PraznoIme_VracaGresku | Validacija obaveznog polja ime | 1. Prazno ime<br>2. Validate DTO | Greška validacije | Greška validacije | US-02 | Prošao |
| 10 | Validacija_PraznoPrezime_VracaGresku | Validacija prezimena | 1. Prazno prezime<br>2. Validate DTO | Greška validacije | Greška validacije | US-02 | Prošao |
| 11 | Validacija_PrazanEmail_VracaGresku | Validacija emaila | 1. Prazan email<br>2. Validate DTO | Greška validacije | Greška validacije | US-02 | Prošao |
| 12 | Validacija_PraznaLozinka_VracaGresku | Validacija lozinke | 1. Prazna lozinka<br>2. Validate DTO | Greška validacije | Greška validacije | US-02 | Prošao |
| 13 | Validacija_LozinkaKracaOd8Znakova_VracaGresku | Minimalna dužina lozinke | 1. Lozinka < 8 znakova<br>2. Validate DTO | Greška validacije | Greška validacije | US-02 | Prošao |
| 14 | Validacija_LozinkaSa1Znakom_VracaGresku | Granični slučaj lozinke | 1. Lozinka = "A"<br>2. Validate DTO | Greška validacije | Greška validacije | US-02 | Prošao |
| 15 | Validacija_LozinkaTacno8Znakova_JeValidna | Validna minimalna lozinka | 1. Lozinka = 8 znakova<br>2. Validate DTO | Validan model | Validan model | US-02 | Prošao |
| 16 | Validacija_EmailBezAtZnaka_VracaGresku | Validacija email formata | 1. Email bez @<br>2. Validate DTO | Greška validacije | Greška validacije | US-02 | Prošao |
| 17 | Validacija_EmailBezDomene_VracaGresku | Validacija domene emaila | 1. Email bez domene<br>2. Validate DTO | Greška validacije | Greška validacije | US-02 | Prošao |

---

##### Primjerak API (`PrimjerakApiControllerTests`)

Ovi unit testovi pokrivaju upravljanje fizičkim primjercima knjiga, uključujući masovno dodavanje novih primjeraka, automatsko generiranje inventarnih brojeva, te stroga pravila za deaktivaciju primjeraka koji su u upotrebi.

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:---|:---|
| 1 | GetByKnjiga_KnjigaPostoji_VracaOkSaListomPrimjeraka | Vraća listu svih primjeraka za postojeću knjigu | 1. Pozvati API sa validnim knjigaId<br>2. Mock vraća knjigu i listu primjeraka | 200 OK + lista primjeraka | 200 OK + lista vraćena | US-22, US-23 | Prošao |
| 2 | GetByKnjiga_KnjigaNePostoji_VracaNotFound | Knjiga ne postoji u sistemu | 1. Pozvati API sa nepostojećim ID | 404 Not Found | 404 Not Found | US-22 | Prošao |
| 3 | GetByKnjiga_KnjigaNemaaPrimjeraka_VracaOkSaPraznomListom | Knjiga postoji ali nema primjeraka | 1. Pozvati API<br>2. Lista prazna | 200 OK + [] | 200 OK + prazna lista | US-22 | Prošao |
| 4 | GetById_PrimjerakPostoji_VracaOkIObjekt | Vraća pojedinačni primjerak | 1. Pozvati API sa validnim ID | 200 OK + objekt primjerka | 200 OK | US-23 | Prošao |
| 5 | GetById_PrimjerakNePostoji_VracaNotFound | Traženi primjerak ne postoji | 1. Pozvati API sa nevalidnim ID | 404 Not Found | 404 Not Found | US-23 | Prošao |
| 6 | Create_KnjigaNePostoji_VracaBadRequest | Ne može se dodati primjerak bez knjige | 1. Pozvati Create bez postojeće knjige | 400 Bad Request | 400 Bad Request | US-21 | Prošao |
| 7 | Create_KnjigaNePostoji_NePozivaSeSprema | Sprečava upis ako knjiga ne postoji | 1. Pozvati Create<br>2. Provjeriti repo | CreateAsync se ne poziva | CreateAsync nije pozvan | US-21 | Prošao |
| 8 | Create_BrojNovihManjiOd1_VracaBadRequest | Validacija minimalnog broja primjeraka | 1. BrojNovih = 0 | 400 Bad Request | 400 Bad Request | US-21 | Prošao |
| 9 | Create_BrojNovihVeciOd50_VracaBadRequest | Limit max 50 primjeraka | 1. BrojNovih = 51 | 400 Bad Request | 400 Bad Request | US-21 | Prošao |
| 10 | Create_ValidanRequest_VracaCreatedAtAction | Uspješno kreiranje primjeraka | 1. Validan request | 201 Created | 201 Created | US-21 | Prošao |
| 11 | Create_BrojNovih3_PozivaSeSprema3Puta | Provjera masovnog kreiranja | 1. BrojNovih = 3<br>2. Poziv API | CreateAsync x3 | CreateAsync x3 | US-21 | Prošao |
| 12 | Create_NoviPrimjerak_StatusJeDostupanByDefault | Novi primjerci imaju status "dostupan" | 1. Kreirati primjerke<br>2. Provjeriti status | status = "dostupan" | status = "dostupan" | US-23 | Prošao |
| 13 | Create_NoviPrimjerak_InventarniBrojSadrziKnjigaId | Generisanje inventarnog broja | 1. Kreirati primjerak<br>2. Provjeriti format | INV-{knjigaId}-XXX | ispravan format | US-21 | Prošao |
| 14 | Create_PostojeciPrimjerci_RedniBrojSeNastavlja | Nastavljanje rednog broja | 1. Postoje 2 primjerka<br>2. Dodati novi | Broj se nastavlja (003) | 003 generisan | US-21 | Prošao |
| 15 | Deaktiviraj_PrimjerakNePostoji_VracaNotFound | Deaktivacija nepostojećeg primjerka | 1. Pozvati Deaktiviraj sa nevalidnim ID | 404 Not Found | 404 Not Found | US-24 | Prošao |
| 16 | Deaktiviraj_VecDeaktiviran_VracaConflict | Sprečava dvostruku deaktivaciju | 1. Status = deaktiviran<br>2. Poziv API | 409 Conflict | 409 Conflict | US-24 | Prošao |
| 17 | Deaktiviraj_ImaAktivnoZaduzenje_VracaConflict | Ne može se deaktivirati zadužen primjerak | 1. Active loan = true<br>2. Poziv API | 409 Conflict | 409 Conflict | US-24 | Prošao |
| 18 | Deaktiviraj_ImaAktivnoZaduzenje_NePozivaseDeactivate | Sprečava update ako postoji zaduženje | 1. Active loan = true | DeactivateAsync se ne poziva | nije pozvan | US-24 | Prošao |
| 19 | Deaktiviraj_Uspjesno_VracaOk | Uspješna deaktivacija | 1. Validan primjerak<br>2. Poziv API | 200 OK | 200 OK | US-24 | Prošao |
| 20 | Deaktiviraj_Uspjesno_PozivaseDeactivate | Provjera poziva repo metode | 1. Validan request | DeactivateAsync x1 | x1 pozvan | US-24 | Prošao |

---

#### 4.1.2 Web Kontroleri — Unit testovi

##### Auth Web (`AuthWebControllerTests`)

Ovi unit testovi validiraju proces prijave putem web forme (MVC), fokusirajući se na ispravno usmjeravanje korisnika (Redirect) prema njihovim ulogama, sigurnost sesije kroz Cookie autentifikaciju, te zaštitu od curenja informacija putem generičkih poruka o greškama.

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:---|:---|
| 1 | `Login_UspjesanClan_RedirectNaHomeIndex` | Provjera redirekcije korisnika sa ulogom Član | Unijeti validne kredencijale za člana i izvršiti login | Redirect na Home/Index stranicu | Kao očekivano | US-04 | Prošao |
| 2 | `Login_UspjesanBibliotekar_RedirectNaKorisnikIndex` | Provjera redirekcije bibliotekara nakon prijave | Unijeti validne kredencijale bibliotekara | Redirect na admin/dashboard (Korisnik/Index) | Kao očekivano | US-04 | Prošao |
| 3 | `Login_UspjesanAdministrator_RedirectNaKorisnikIndex` | Provjera redirekcije administratora | Unijeti validne kredencijale administratora | Redirect na korisnički/admin dashboard | Kao očekivano | US-04 | Prošao |
| 4 | `Login_ValjanReturnUrl_RedirectNaTajUrl` | Provjera povratnog URL-a nakon login-a | Poslati login sa validnim returnUrl parametrom | Redirect na proslijeđeni returnUrl | Kao očekivano | US-04 | Prošao |
| 5 | `Login_PogresnaLozinka_VracaViewSaGenerickomPorukom` | Sigurnost: ne otkrivaju se detalji greške | Unijeti validan username + pogrešna lozinka | View sa generičkom porukom o grešci | Kao očekivano | US-05 | Prošao |
| 6 | `Login_DeaktiviranKorisnik_VracaGenericku` | Blokada deaktiviranih korisnika | Pokušaj login-a deaktiviranog korisnika | Generička greška, bez detalja | Kao očekivano | US-05, US-09 | Prošao |
| 7 | `Login_NepostojeciKorisnik_VracaViewSaGreskom` | Sigurno rukovanje nepostojećim korisnikom | Unijeti nepostojeće kredencijale | Generička poruka o grešci | Kao očekivano | US-05 | Prošao |
| 8 | `Logout_UvijekRedirectNaLoginStranu` | Provjera logout funkcionalnosti | Izvršiti logout zahtjev | Sesija obrisana i redirect na login stranicu | Kao očekivano | US-06 | Prošao |

---

##### Knjiga Web (`KnjigaWebControllerTests`)

Ovi testovi validiraju funkcionalnosti bibliotečkog kataloga namijenjenog krajnjim korisnicima, kao i administrativni interfejs za upravljanje fondom knjiga. Fokus je na ispravnom prikazu podataka, navigaciji kroz katalog (paginacija) i integritetu podataka pri unosu.

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:---|:---|
| 1 | Index_VracaKatalogViewModel | Prikaz kataloga knjiga | Pozvati Index bez filtera | Vraća ViewResult sa KatalogViewModel i listom knjiga | Kao očekivano | US-12 | Prošao |
| 2 | Index_NemaKnjiga_VracaPrazanKatalog | Prazan katalog | Mock prazne liste i poziv Index | Lista knjiga prazna, bez greške | Kao očekivano | US-13 | Prošao |
| 3 | Index_PaginacijaMetadata_IspravnaVrijednost | Paginacija kataloga | Pozvati Index sa page=2 | Ispravno izračunate stranice i metadata | Kao očekivano | US-20 | Prošao |
| 4 | Index_BrojDostupnihIzPrimjeraka | Brojanje dostupnih primjeraka | Knjiga sa 2 primjerka (1 dostupan) | Broj dostupnih = 1 | Kao očekivano | US-22, US-23 | Prošao |
| 5 | Create_ValidanModel_SpremaKnjigu | Kreiranje knjige | Poslati validan DTO | Knjiga se snima i redirect na Index | Kao očekivano | US-12 | Prošao |
| 6 | Create_ValidanModel_KreiraPrimjerkePremaKolicini | Kreiranje primjeraka | BrojPrimjeraka = 3 | Kreiraju se 3 primjerka | Kao očekivano | US-21 | Prošao |
| 7 | Create_NulaKopija_NeKreiraPrimjerke | Nula primjeraka | BrojPrimjeraka = 0 | Ne kreiraju se primjerci | Kao očekivano | US-21 | Prošao |
| 8 | Create_NeispravanModel_VracaView | Validacija forme | Nevalidan ModelState | Ostaje na View sa greškama | Kao očekivano | US-12 | Prošao |
| 9 | Create_NevazanIsbn_DodajeGresku | Neispravan ISBN | ISBN = "123" | ModelState greška za ISBN | Kao očekivano | US-13 | Prošao |
| 10 | Create_DuplikatIsbn | Dupli ISBN | Postojeća knjiga sa istim ISBN | Greška u ModelState | Kao očekivano | US-13 | Prošao |
| 11 | Create_NevalidnaKategorija | Neispravna kategorija | KategorijaId ne postoji | ModelState greška | Kao očekivano | US-12 | Prošao |
| 12 | Create_IsbnSacrticama_NormalizujeSe | Normalizacija ISBN | ISBN sa crticama | ISBN se čuva bez crtica | Kao očekivano | US-13 | Prošao |
| 13 | Edit_Get_PostojecaKnjiga | Učitavanje edit forme | GET Edit sa validnim ID | View sa popunjenim podacima | Kao očekivano | US-17 | Prošao |
| 14 | Edit_Get_NepostojecaKnjiga | Nevalidan ID | GET Edit sa nepostojećim ID | 404 NotFound | Kao očekivano | US-17 | Prošao |
| 15 | Edit_Post_ValidanModel | Ažuriranje knjige | POST validan DTO | Update + redirect Index | Kao očekivano | US-17 | Prošao |
| 16 | Edit_Post_NeispravanModel | Nevalidni podaci | ModelState invalid | Ostaje na View | Kao očekivano | US-17 | Prošao |
| 17 | Edit_Post_NepostojecaKnjiga | Knjiga obrisana u međuvremenu | POST sa ID koji ne postoji | 404 NotFound | Kao očekivano | US-17 | Prošao |

---

##### Kategorija Web (`KategorijaWebControllerTests`)

Ovi testovi validiraju administrativni interfejs za upravljanje kategorijama. Fokus je na ispravnom prikazu povratnih informacija korisniku (Success/Error poruke) putem `TempData` objekta, ispravnoj navigaciji (Redirect) nakon akcija, te očuvanju integriteta podataka na nivou Web formi.

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:-:|:-:|
| 1 | Index_VracaViewSaListomKategorija | Prikaz liste svih kategorija | Pozvati `Index()` kada postoje kategorije | View sadrži listu kategorija | Prošao | US-31 | Prošao |
| 2 | Index_NemaKategorija_VracaPrazanView | Prazno stanje liste | Pozvati `Index()` kada nema kategorija | View sa praznom kolekcijom | Prošao | US-31 | Prošao |
| 3 | Create_ValidanNaziv_SpremaIRedirektuje | Dodavanje nove kategorije | Pozvati `Create()` sa validnim nazivom | Redirekcija na Index | Prošao | US-30 | Prošao |
| 4 | Create_ValidanNaziv_PrikazujePorukuUspjeha | Feedback nakon dodavanja | Pozvati `Create()` sa validnim podacima | TempData sadrži SuccessMessage | Prošao | US-30 | Prošao |
| 5 | Create_PrazanNaziv_PrikazujeGreskuIRedirektuje | Validacija praznog naziva | Pozvati `Create()` sa praznim nazivom | Greška i redirekcija | Prošao | US-30 | Prošao |
| 6 | Create_PrazanNaziv_NePozivaSeSprema | Sprječavanje upisa u bazu | Pozvati `Create()` sa praznim nazivom | CreateAsync se ne poziva | Prošao | US-30 | Prošao |
| 7 | Create_NazivVecPostoji_PrikazujeGreskuIRedirektuje | Duplikat naziv | Pozvati `Create()` sa postojećim nazivom | Greška i redirekcija | Prošao | US-30 | Prošao |
| 8 | Create_NazivVecPostoji_NePozivaSeSprema | Integritet baze | Pozvati `Create()` sa duplikatom | CreateAsync se ne poziva | Prošao | US-30 | Prošao |
| 9 | Create_NazivCaseInsensitive_PrikazujeGresku | Case-insensitive provjera | Pozvati `Create()` sa nazivom različitog case-a | Greška u TempData | Prošao | US-30 | Prošao |
| 10 | Edit_ValidanModel_AzuriraIRedirektuje | Ažuriranje kategorije | Pozvati `Edit()` sa validnim podacima | Redirekcija i izmjena podataka | Prošao | US-33 | Prošao |
| 11 | Edit_ValidanModel_PrikazujePorukuUspjeha | Feedback nakon izmjene | Pozvati `Edit()` | SuccessMessage u TempData | Prošao | US-33 | Prošao |
| 12 | Edit_PrazanNaziv_PrikazujeGreskuIRedirektuje | Validacija izmjene | Pozvati `Edit()` sa praznim nazivom | Greška i bez update-a | Prošao | US-33 | Prošao |
| 13 | Edit_NepostojecaKategorija_VracaNotFound | Nevalidan ID | Pozvati `Edit()` sa nepostojećim ID | NotFound rezultat | Prošao | US-33 | Prošao |
| 14 | Edit_NazivVecPostojiKodDrugeKategorije_PrikazujeGresku | Konflikt naziva | Pozvati `Edit()` sa duplikatom | Greška i bez update-a | Prošao | US-33 | Prošao |
| 15 | Edit_IstaNazivIstaKategorija_DozvoljenoAzuriranje | Dozvoljena izmjena opisa | Pozvati `Edit()` bez promjene naziva | Uspješna izmjena | Prošao | US-33 | Prošao |
| 16 | Delete_KategorijaNemaKnjige_BriseIRedirektuje | Brisanje kategorije | Pozvati `Delete()` bez povezanih knjiga | Redirekcija i brisanje | Prošao | US-34 | Prošao |
| 17 | Delete_KategorijaNemaKnjige_PrikazujePorukuUspjeha | Feedback brisanja | Pozvati `Delete()` | SuccessMessage | Prošao | US-34 | Prošao |
| 18 | Delete_KategorijaImaKnjige_PrikazujeGreskuINeBrise | Zaštita relacija | Pozvati `Delete()` sa povezanim knjigama | Greška, nema brisanja | Prošao | US-32 | Prošao |
| 19 | Delete_KategorijaImaKnjige_PorukaJasnaKorisniku | Jasna poruka | Pozvati `Delete()` | ErrorMessage nije prazan | Prošao | US-32 | Prošao |
| 20 | Delete_NepostojecaKategorija_RedirektujeSaGreskom | Nevalidan ID | Pozvati `Delete()` | Redirekcija sa greškom | Prošao | US-34 | Prošao |
| 21 | GetById_PostojecaKategorija_VracaJson | Dohvat kategorije | Pozvati `GetById()` sa validnim ID | JSON rezultat | Prošao | US-33 | Prošao |
| 22 | GetById_NepostojecaKategorija_VracaNotFound | Nevalidan ID za GET | Pozvati `GetById()` | NotFound | Prošao | US-33 | Prošao |
| 23 | Create_OpisSamoRazmaci_SpremaSeKaoNull | Normalizacija opisa | Pozvati `Create()` sa whitespace opisom | Opis = null | Prošao | US-30 | Prošao |
| 24 | Create_ExceptionPriSprema_PrikazujeGresku | Greška baze | Simulirati exception | ErrorMessage | Prošao | US-30 | Prošao |
| 25 | Edit_ExceptionPriAzuriranju_PrikazujeGresku | Greška pri update | Simulirati exception | ErrorMessage | Prošao | US-33 | Prošao |
| 26 | Delete_ExceptionPriBrisanju_PrikazujeGresku | Greška pri delete | Simulirati exception | ErrorMessage | Prošao | US-34 | Prošao |

---

##### Korisnik Web (`KorisnikWebControllerTests`)

Ovi unit testovi pokrivaju administrativni interfejs za upravljanje članovima biblioteke. Fokus je na ispravnom filtriranju uloga unutar liste, validaciji unikatnosti email adresa pri registraciji, te procesu deaktivacije naloga uz odgovarajući feedback korisniku.

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:-:|:-:|
| 1 | `Index_VracaViewSaListomClanova` | Prikazuje samo korisnike sa ulogom "Član". | Pozvati `Index()` sa listom korisnika (Član + Administrator). | View sadrži samo članove (Administrator filtriran). | Odgovara očekivanom. | US-49 | Prošao |
| 2 | `Index_KorisnikBezUloge_NijeUkljucenUListu` | Filtrira korisnike bez definisane uloge. | Pozvati `Index()` gdje korisnik ima `Uloga = null`. | Takav korisnik se ne prikazuje u listi. | Odgovara očekivanom. | US-49 | Prošao |
| 3 | `Index_SortiranjePoPrezimenu_IspravanRedoslijed` | Provjerava sortiranje po prezimenu pa imenu. | Pozvati `Index()` sa više članova različitih imena/prezimena. | Lista sortirana po Prezime → Ime. | Odgovara očekivanom. | US-49 | Prošao |
| 4 | `Index_NemaKorisnika_VracaPrazanModel` | Ispravno ponašanje kada nema korisnika. | Pozvati `Index()` sa praznom listom. | View vraća prazan model bez greške. | Odgovara očekivanom. | US-49 | Prošao |
| 5 | `Create_Get_VracaViewSaPraznimModelom` | Prikaz forme za kreiranje korisnika. | Pozvati `Create()` GET metodu. | View sa praznim `KorisnikCreateDto` modelom. | Odgovara očekivanom. | US-01 | Prošao |
| 6 | `Create_Post_ValidanModel_RedirectsToIndex` | Uspješna registracija korisnika. | Poslati validan `KorisnikCreateDto`. | Redirekcija na `Index` + poruka uspjeha. | Odgovara očekivanom. | US-03 | Prošao |
| 7 | `Create_Post_EmailVecPostoji_VracaViewSaGreskom` | Validacija unikatnosti emaila. | Poslati model sa postojećim emailom. | Vraća se View sa greškom na polju Email. | Odgovara očekivanom. | US-02 | Prošao |
| 8 | `Create_Post_NeispravanModel_VracaView` | Validacija obaveznih polja. | Postaviti `ModelState` kao neispravan. | Vraća se isti View bez poziva repozitorija. | Odgovara očekivanom. | US-02 | Prošao |
| 9 | `Deaktiviraj_PostojeciKorisnik_RedirectsSaPorukom` | Deaktivacija postojećeg korisnika. | Pozvati `Deaktiviraj(id)` za validnog korisnika. | Status = "deaktiviran", redirekcija + poruka. | Odgovara očekivanom. | US-09 | Prošao |
| 10 | `Deaktiviraj_NepostojeciKorisnik_VracaNotFound` | Obrada greške za nepostojećeg korisnika. | Pozvati `Deaktiviraj(id)` za nepostojeći ID. | Vraća `NotFound`. | Odgovara očekivanom. | US-09 | Prošao |

---

##### Primjerak Web (`PrimjerakWebControllerTests`)

Ovi unit testovi validiraju web interfejs za upravljanje fizičkim primjercima knjiga. Fokus je na masovnom dodavanju novih primjeraka (do 50 odjednom), automatskoj generaciji inventarnih brojeva, te sigurnosnim provjerama koje sprječavaju deaktivaciju zaduženih primjeraka direktno kroz web forme.

| ID | Naziv testa | Opis | Testni koraci | Očekivani rezultat | Stvarni rezultat | US | Status |
|:-:|:---|:---|:---|:---|:---|:-:|:-:|
| 1 | `Dodaj_Get_KnjigaPostoji_VracaView` | Prikaz forme za dodavanje primjeraka postojeće knjige | Pozvati `Dodaj(knjigaId)` sa validnim ID-em knjige | Vraća se `ViewResult` sa formom | Prošao | US-21 | Prošao |
| 2 | `Dodaj_Get_KnjigaNePostoji_VracaNotFound` | Rukovanje greškom kada knjiga ne postoji | Pozvati `Dodaj(knjigaId)` sa nepostojećim ID-em | Vraća `NotFoundResult` | Prošao | US-21 | Prošao |
| 3 | `Dodaj_Post_KnjigaNePostoji_RedirektujeSaGreskom` | Sprječava dodavanje primjeraka bez validne knjige | Pozvati POST `Dodaj` sa nepostojećim `knjigaId` | Redirect na `Knjiga/Index` uz `ErrorMessage` | Prošao | US-21 | Prošao |
| 4 | `Dodaj_Post_BrojNovihManjiOd1_RedirektujeSaGreskom` | Validacija minimalnog broja primjeraka | Pozvati POST `Dodaj` sa `brojNovih = 0` | Redirect na `Details` uz grešku | Prošao | US-21 | Prošao |
| 5 | `Dodaj_Post_BrojNovihVeciOd50_RedirektujeSaGreskom` | Validacija maksimalnog broja primjeraka | Pozvati POST `Dodaj` sa `brojNovih = 51` | Redirect uz `ErrorMessage` | Prošao | US-21 | Prošao |
| 6 | `Dodaj_Post_ValidanRequest_SpremaIRedirektuje` | Uspješno dodavanje primjeraka | Pozvati POST `Dodaj` sa validnim podacima | Redirect na `Knjiga/Details` | Prošao | US-21 | Prošao |
| 7 | `Dodaj_Post_ValidanRequest_PrikazujePorukuUspjeha` | Feedback nakon uspješnog dodavanja | Pozvati validan POST zahtjev | `TempData["SuccessMessage"]` popunjen | Prošao | US-21 | Prošao |
| 8 | `Dodaj_Post_BrojNovih3_PozivaSeSprema3Puta` | Kreiranje više primjeraka | Pozvati POST sa `brojNovih = 3` | `CreateAsync` pozvan 3 puta | Prošao | US-21 | Prošao |
| 9 | `Dodaj_Post_NoviPrimjerak_StatusJeDostupan` | Početni status primjerka | Dodati nove primjerke | Svaki ima status `"dostupan"` | Prošao | US-23 | Prošao |
| 10 | `Dodaj_Post_PostojeciPrimjerci_RedniBrojSeNastavlja` | Generacija inventarnog broja | Postoje 2 primjerka → dodati novi | Novi ima ispravan redni broj (`INV-...-003`) | Prošao | US-21 | Prošao |
| 11 | `Deaktiviraj_PrimjerakNePostoji_RedirektujeSaGreskom` | Rukovanje greškom pri deaktivaciji | Pozvati `Deaktiviraj(id)` sa nepostojećim ID-em | Redirect uz `ErrorMessage` | Prošao | US-24 | Prošao |
| 12 | `Deaktiviraj_ImaAktivnoZaduzenje_RedirektujeSaGreskom` | Sprječava deaktivaciju zaduženog primjerka | Primjerak ima aktivno zaduženje | Redirect uz grešku | Prošao | US-24 | Prošao |
| 13 | `Deaktiviraj_ImaAktivnoZaduzenje_NePozivaseDeactivate` | Sigurnosna provjera | Pozvati deaktivaciju nad zaduženim primjerkom | `DeactivateAsync` se NE poziva | Prošao | US-24 | Prošao |
| 14 | `Deaktiviraj_VecDeaktiviran_RedirektujeSaGreskom` | Sprječava duplu deaktivaciju | Primjerak već ima status `"deaktiviran"` | Redirect uz `ErrorMessage` | Prošao | US-24 | Prošao |
| 15 | `Deaktiviraj_Uspjesno_RedirektujeSaPorukomUspjeha` | Uspješna deaktivacija | Validan zahtjev bez zaduženja | Redirect uz `SuccessMessage` | Prošao | US-24 | Prošao |
| 16 | `Deaktiviraj_Uspjesno_PozivaseDeactivate` | Integritet operacije | Pozvati validnu deaktivaciju | `DeactivateAsync` pozvan jednom | Prošao | US-24 | Prošao |

---

<a name="detaljni-izvjestaj-security"></a>
### 4.2 Penetracijski / Sigurnosni testovi — Detaljna lista
 
#### 4.2.1 Autentifikacija i SQL Injection
 
Ovi testovi validiraju otpornost login endpointa na klasične napade injektiranja, ispravnost JWT validacije te ponašanje sistema pri radu sa deaktiviranim korisnicima.
 
| # | Naziv testa | Šta se provjerava | Payload / Input | Očekivano | Status |
|:-:|:---|:---|:---|:-:|:-:|
| **1** | `Login_SqlInjectionUEmailu_VracaBadRequestIliUnauthorized` | SQL injection kroz email polje ne smije dati pristup | `' OR '1'='1` | 400 / 401 | Prošao |
| **2** | `Login_SqlInjectionUEmailu_VracaBadRequestIliUnauthorized` | Komentar operator ne smije zaobići autentifikaciju | `admin'--` | 400 / 401 | Prošao |
| **3** | `Login_SqlInjectionUEmailu_VracaBadRequestIliUnauthorized` | Destruktivna naredba se odbija na nivou validacije | `'; DROP TABLE Korisnici;--` | 400 / 401 | Prošao |
| **4** | `Login_SqlInjectionUEmailu_VracaBadRequestIliUnauthorized` | Uvijek-tačan uvjet ne smije omogućiti pristup | `" OR 1=1--` | 400 / 401 | Prošao |
| **5** | `Login_SqlInjectionULozinki_VracaUnauthorized` | SQL injection kroz lozinka polje ne smije dati token | `' OR '1'='1` | 401 | Prošao |
| **6** | `Login_SqlInjectionULozinki_VracaUnauthorized` | Destruktivna naredba u lozinki se odbija | `'; DROP TABLE Korisnici;--` | 401 | Prošao |
| **7** | `Login_SqlInjectionULozinki_VracaUnauthorized` | Uvijek-tačan uvjet u lozinki ne daje pristup | `" OR 1=1--` | 401 | Prošao |
| **8** | `Login_ViseNeuspjelihPokusaja_SvakiVracaUnauthorized` | 10 uzastopnih pogrešnih lozinki ne otključava nalog | 10 uzastopnih pogrešnih lozinki | 401 svaki put | Prošao |
| **9** | `ZasticenaRuta_LazniJwtBezPotpisa_VracaUnauthorized` | Ručno kreiran JWT bez valjanog potpisa biva odbijen | JWT bez potpisa | 401 | Prošao |
| **10** | `ZasticenaRuta_ModifikovanJwtPotpis_VracaUnauthorized` | Validan token sa izmijenjenim potpisom biva odbijen | JWT sa lažnim potpisom | 401 | Prošao |
| **11** | `Login_DeaktiviranKorisnik_StariJwtOstajeValidan_ArchitekturalnaNapomena` | Sigurnosna napomena — već izdati JWT ostaje validan sve dok ne istekne; | Dokumentovano | Dokumentovano | Prošao |
 
---
 
#### 4.2.2 XSS — Cross-Site Scripting zaštita 
 
Ovi testovi provjeravaju da sistem nikada ne pohrani niti vrati neobrađene HTML/JavaScript tagove — svaki zlonamjerni unos mora biti ili odbijen (400) ili neutraliziran prije pohrane.
 
| # | Naziv testa | Šta se provjerava | Payload | Očekivano | Status |
|:-:|:---|:---|:---|:-:|:-:|
| **1** | `KorisnikCreate_XssPayloadUImenuIliPrezimenu_OdbijenIliEscapovan` | Script tag u polju Ime ne smije biti pohranjen ni vraćen | `<script>alert('xss')</script>` | 400 ili odgovor bez taga | Prošao |
| **2** | `KorisnikCreate_XssPayloadUImenuIliPrezimenu_OdbijenIliEscapovan` | Inline event handler u Prezime polju biva odbijen | `<img src=x onerror=alert(1)>` | 400 ili odgovor bez atributa | Prošao |
| **3** | `KategorijaCreate_XssPayloadUNazivu_OdbijenIliEscapovan` | Script tag u nazivu kategorije ne prolazi validaciju | `<script>alert('xss')</script>` | 400 ili odgovor bez taga | Prošao |
| **4** | `KategorijaCreate_XssPayloadUNazivu_OdbijenIliEscapovan` | Inline event handler u opisu kategorije biva odbijen | `<img src=x onerror=alert(1)>` | 400 ili odgovor bez atributa | Prošao |
 
---
 
#### 4.2.3 Path Traversal i Injection u ISBN polju
 
Ovi testovi osiguravaju da ISBN polje ne može biti iskorišteno kao vektor napada — nevažeći, ekstremni i zlonamjerni unosi moraju biti odbijeni validacijom bez curenja internih grešaka.
 
| # | Naziv testa | Šta se provjerava | Payload / Input | Očekivano | Status |
|:-:|:---|:---|:---|:-:|:-:|
| **1** | `KnjigaCreate_InjectionUIsbnPolju_VracaBadRequest` | SQL injection u ISBN polju se odbija validacijom | `' OR '1'='1` | 400 | Prošao |
| **2** | `KnjigaCreate_InjectionUIsbnPolju_VracaBadRequest` | XSS payload u ISBN polju ne prolazi format validaciju | `<script>alert(1)</script>` | 400 | Prošao |
| **3** | `KnjigaCreate_InjectionUIsbnPolju_VracaBadRequest` | Path traversal napad u ISBN polju biva odbijen | `../../../../etc/passwd` | 400 | Prošao |


> **Napomena:** Provjere `401` bez tokena i `403` za eskalaciju privilegija testiraju ispravnost implementacije (poslovnu logiku), a ne napadačke vektore — u potpunosti su pokrivene integracijskim testovima i namjerno su izostavljene u ovom nivou testiranja.

---

<a name="detaljni-izvjestaj-uat"></a>
### 4.3 User acceptance testovi (UAT) - Detaljna lista scenarija

| ID | Scenarij | Koraci izvršavanja | Očekivani rezultat |
|----|---------|-------------------|-------------------|
| UAT-01 | Kreiranje naloga (uspješan tok) | 1. Prijava kao bibliotekar/admin <br> 2. Otvoriti formu za kreiranje člana <br> 3. Unijeti sve podatke <br> 4. Kliknuti na dugme 'Kreiraj člana' | Nalog kreiran, vidljiv u listi članova |
| UAT-02 | Kreiranje naloga (prazna polja) | 1. Prijava kao bibliotekar/admin <br> 2. Otvoriti formu za kreiranje člana <br> 3. Unijeti sve osim emaila <br> 4. Kliknuti na dugme 'Kreiraj člana' | Ispis poruka: Email adresa je obavezna. |
| UAT-03 | Duplikat email | 1. Unijeti postojeći email <br> 2. Kliknuti na dugme 'Kreiraj člana' | Ispis poruke: Ta email adresa je već registrovana. |
| UAT-04 | Kratka lozinka | 1. Unijeti lozinku < 8 znakova <br> 2. Kliknuti na dugme 'Kreiraj člana' | Ispis poruke: Lozinka mora imati najmanje 8 znakova. |
| UAT-05 | Prijava korisnika (validni podaci) | 1. Login sa validnim podacima | Redirect na Home page |
| UAT-06 | Prijava osoblja (validni podaci) | 1. Login kao bibliotekar | Redirect na listu korisnika |
| UAT-07 | Prijava sa pogrešnom lozinkom | 1. Unijeti pogrešnu lozinku za već registorvanog člana | Generička poruka: Prijava nije uspjela. |
| UAT-08 | Prijava sa neispravnim emailom | 1. Unijeti nevalidan email za već registrovanog člana | Generička poruka: Prijava nije uspjela. |
| UAT-09 | Odjava | 1. Kliknuti na dugme 'Logout' <br> 2. Pokušaj pristupa zaštićenoj stranici (npr. Katalog stranici) | Redirect na login stranicu |
| UAT-10 | Dodavanje knjige | 1. Otvoriti formu klikom na dugme 'Nova knjiga' <br> 2. Unijeti obavezne podatke <br> 3. Sačuvati klikom na dugme 'Dodaj u katalog' | Knjiga se prikazuje u katalogu |
| UAT-11 | ISBN validacija | 1. Otvoriti formu klikom na dugme 'Nova knjiga' <br> 2. Unijeti obavezne podatke - za polje ISBN unijeti već postojeći <br> 3. Sačuvati klikom na dugme 'Dodaj u katalog' | Ispis poruke: Knjiga sa ovim ISBN-om već postoji u katalogu. |
| UAT-12 | Broj primjeraka | 1. Unijeti broj (1 ili više) | Primjerci kreirani i prikazani u detaljima knjige |
| UAT-13 | Uređivanje knjige | 1. Ući na detaljne jedne knjige iz kataloga <br> 2. Kliknuti na dugme 'Uredi' <br> 3. Unijeti izmjene <br> 4. Kliknuti na dugme 'Sačuvaj izmjene' | Promjene vidljive |
| UAT-14 | Brisanje knjige | 1. Ući na detaljne jedne knjige iz kataloga <br> 2. Kliknuti na dugme 'Obriši' <br> 3. Potvrditi brisanje | Knjiga uklonjena |
| UAT-15 | Dodavanje primjeraka | 1. Otvoriti detaljne jedne knjige <br> 2. Kliknuti na dugme 'Dodaj primjerak' <br> 3. Unijeti željeni broj <br> 4. Kliknuti na dugme 'Dodaj' | Kreirani novi primjerci |
| UAT-16 | Dodavanje kategorije | 1. Kliknuti na dugme 'Dodaj kategoriiju' u sekciji Kategorija <br> 2. Unijeti naziv i opis (opcionalno) <br> 3. Kliknuti na dugme 'Sačuvaj' | Nova kategorija se pojavljuje u listi kategorija |
| UAT-17 | Duplikat kategorije | 1. Unijeti postojeći naziv | Greška: Kategorija "X" već postoji u sistemu. |
| UAT-18 | Pregled kategorija | 1. Otvoriti sekciju 'Kategorije' | Prikaz postojećih kategorija |
| UAT-19 | Uređivanje kategorije | 1. Kliknuti na dugme 'Uredi' u listi kategorija <br> 2. Izmijeniti naziv | Promjene su prikazane u listi postojećih kategorija |
| UAT-20 | Brisanje kategorije | 1. Kliknuti na dugme 'Obriši' u listi kategorija <br> 2. Kliknuti na dugme 'Potvrdi' | Kategorija uklonjena iz spiska postojećih kategorija |
| UAT-21 | Brisanje u upotrebi | 1. Kliknuti na dugme 'Obriši' za slučaj kad ima knjiga sa tom kategorijom | Greška: Kategorija 'X' ima Y knjiga i ne može biti obrisana. |
| UAT-22 | Prikaz kataloga | 1. Otvoriti katalog klikom na dugme 'Katalog' | Prikazuje se lista dostupnih knjiga |