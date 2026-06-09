# SmartLib - Dokumentovanje rada

## 1. Uvod

Ovaj dokument predstavlja dokumentovanje rada tima na projektu **SmartLib**, bibliotečkom informacionom sistemu razvijenom u okviru predmeta Softverski inženjering. Dokument je zasnovan na stvarnoj implementaciji u repozitoriju projekta, sprint artefaktima i isporučenim inkrementima od Sprinta 1 do Sprinta 11.

SmartLib je realizovan kao web aplikacija sa odvojenim slojevima `Core`, `Infrastructure`, `Web` i `API`. Glavni korisnički interfejs sistema je MVC web aplikacija, dok je REST API razvijen kao dodatni ulazni sloj za dio funkcionalnosti.

---

## 2. Svrha projekta

Svrha projekta bila je razviti centralizovan sistem koji digitalizuje osnovne i naprednije procese rada biblioteke. Sistem je trebao omogućiti bibliotekaru i administratoru efikasnije upravljanje knjigama, primjercima, korisnicima, članarinama, rezervacijama i zaduženjima, dok članovima biblioteke treba omogućiti jednostavan pregled kataloga, vlastitih aktivnosti i dodatnih korisničkih funkcionalnosti.

Pored osnovne evidencije fonda, cilj projekta bio je da biblioteka dobije sistem koji:

- smanjuje ručni rad i greške u evidenciji
- ubrzava pristup informacijama o dostupnosti knjiga
- uvodi transparentnost nad promjenama kroz audit log
- poboljšava komunikaciju sa članovima kroz email obavještenja i notifikacije
- proširuje klasični bibliotečki sistem funkcijama zajednice, preporuka i personalizacije

---

## 3. Problem koji sistem rješava

Projekat rješava problem biblioteka koje nemaju jedinstven digitalni sistem za praćenje:

- bibliotečkog fonda i fizičkih primjeraka
- aktivnih i završenih zaduženja
- statusa članarina
- rezervacija nedostupnih knjiga
- korisničkih naloga i uloga
- interne evidencije promjena i administrativnog nadzora

Bez ovakvog sistema bibliotekar mora ručno pratiti ko je posudio knjigu, kada knjiga treba biti vraćena, da li je članarina važeća i da li je određena knjiga rezervisana. Takav način rada vodi do nekonzistentnih podataka, sporijeg rada i lošijeg korisničkog iskustva za članove biblioteke.

SmartLib te probleme rješava tako što uvodi jedinstveno mjesto za rad sa katalogom, korisnicima i bibliotečkim procesima, uz dodatne funkcije kao što su forum, vijesti, kalendar, recenzije, liste želja i kolekcije.

---

## 4. Glavne korisničke uloge

U implementiranom sistemu prepoznate su tri glavne korisničke uloge:

### Član biblioteke

Član može:

- pregledati katalog i detalje knjiga
- vidjeti status dostupnosti i broj slobodnih primjeraka
- pregledati vlastita aktivna i historijska zaduženja
- pregledati status članarine
- rezervisati nedostupne knjige i otkazati rezervaciju
- slati zahtjev za produženje članarine
- koristiti forum zajednice
- ostavljati recenzije i ocjene knjiga
- koristiti listu želja i vlastite kolekcije
- pratiti vijesti, kalendar i personalizovane preporuke na početnoj stranici

### Bibliotekar

Bibliotekar može:

- kreirati naloge članova
- upravljati knjigama, kategorijama i primjercima
- evidentirati zaduženja i vraćanja
- pregledati aktivna i historijska zaduženja
- upravljati članarinama
- pregledati rezervacije
- obrađivati zahtjeve za produženje članarine
- slati zahtjeve za nabavku knjiga distributeru
- moderirati dio sadržaja i raditi sa prijavama neadekvatnog sadržaja

### Administrator

Administrator ima sve mogućnosti bibliotekara, uz dodatne administrativne ovlasti:

- pregled svih korisnika
- promjenu korisničkih uloga
- deaktivaciju naloga, uključujući bibliotekarske naloge
- pregled audit log zapisa
- nadzor nad sistemskim promjenama i administrativnim funkcijama

---

## 5. Glavne implementirane funkcionalnosti

Na osnovu pregleda koda i sprint inkremenata, u projektu su implementirane sljedeće glavne funkcionalne cjeline:

### 5.1 Autentifikacija i korisnički nalozi

- prijava i odjava korisnika
- cookie autentifikacija u web aplikaciji
- JWT autentifikacija u API sloju
- zaštita ruta i role-based pristup
- kreiranje naloga novih članova
- promjena lozinke
- reset lozinke putem email linka
- deaktivacija naloga i automatska zabrana prijave deaktiviranim korisnicima

### 5.2 Upravljanje katalogom i fondom

- dodavanje, uređivanje i brisanje knjiga
- upravljanje kategorijama
- upravljanje fizičkim primjercima knjiga
- prikaz detalja knjige
- prikaz dostupnosti i broja slobodnih primjeraka
- pretraga i napredni filteri po autoru, kategoriji, izdavaču i godini
- paginirani katalog

### 5.3 Zaduženja i članarine

- kreiranje zaduženja
- evidencija vraćanja knjiga
- automatsko postavljanje roka vraćanja
- pregled aktivnih zaduženja
- pregled historije zaduženja
- pregled vlastitih zaduženja člana
- upravljanje statusom članarine
- pregled statusa članarine za člana
- online zahtjev za produženje članarine sa obradom od strane bibliotekara

### 5.4 Rezervacije i obavijesti

- rezervacija nedostupnih knjiga
- pregled i otkazivanje vlastitih rezervacija
- pregled svih aktivnih rezervacija
- automatsko otkazivanje isteklih rezervacija
- email upozorenja za rok vraćanja i kašnjenje
- email obavijest bibliotekaru o novoj rezervaciji
- sistemske notifikacije u aplikaciji

### 5.5 Administracija i nadzor

- upravljanje korisnicima i ulogama
- audit log promjena nad ključnim entitetima
- osnovni izvještaji o članstvu, rezervacijama i zaduženjima
- generisanje PDF izvještaja
- health check endpoint za Redis cache

### 5.6 Dodatne korisničke funkcionalnosti

- forum zajednice
- komentari i reakcije na forum objave
- prijava neadekvatnog forum sadržaja
- recenzije i ocjene knjiga
- prijava neadekvatnih recenzija
- vijesti i obavještenja biblioteke
- kalendar događaja
- lista želja
- javne i privatne kolekcije knjiga
- preporuke knjiga na početnoj stranici
- zahtjevi za nabavku knjiga prema distributeru putem emaila

---

## 6. Pregled rada kroz sprintove

### Sprint 1

U prvom sprintu definisana je početna projektna osnova: product vision, stakeholder mapa, team charter i početni product backlog. Ovdje je postavljen problem koji SmartLib rješava i okvir MVP-a.

### Sprint 2

Razrađene su prioritetne user story stavke, acceptance kriteriji i nefunkcionalni zahtjevi. Time je tim pripremio osnovu za konzistentan razvoj i testiranje.

### Sprint 3

Urađeni su domain model, use case model, arhitektonski pregled, test strategija i risk register. Ovaj sprint bio je ključan za definisanje granica sistema i plan implementacije.

### Sprint 4

Postavljen je tehnički kostur projekta: inicijalna struktura rješenja, branching strategija, definition of done, release plan i baza podataka kao osnova za naredne implementacione sprintove.

### Sprint 5

Implementiran je autentifikacijski modul:

- prijava
- odjava
- zaštita ruta
- upravljanje sesijom
- kreiranje naloga novog člana

Takođe su uvedeni AI log i decision log radi transparentnosti rada.

### Sprint 6

Implementiran je kataloški dio sistema:

- dodavanje i uređivanje knjiga
- upravljanje kategorijama
- upravljanje primjercima
- brisanje knjiga uz zaštitu integriteta
- pregled kataloga sa paginacijom

### Sprint 7

Implementirane su osnovne bibliotečke operacije:

- pretraga knjiga
- detalji knjige
- prikaz dostupnosti
- zaduživanje i vraćanje
- pregled vlastitih zaduženja
- pregled aktivnih zaduženja za bibliotekare

Ovaj sprint donio je prvi puni operativni tok rada biblioteke.

### Sprint 8

Proširen je rad sa korisnicima i članarinama:

- profil člana
- historija zaduženja
- administracija korisnika
- upravljanje članarinama
- reset i promjena lozinke

Ovim sprintom sistem je postao funkcionalno zaokružen za osnovni rad sa korisnicima.

### Sprint 9

Sistem je značajno proširen dodatnim modulima:

- rezervacije knjiga
- automatsko otkazivanje rezervacija
- napredna pretraga
- mjesečni izvještaji
- UI unapređenja
- vijesti
- kalendar događaja
- forum zajednice

Sprint 9 bio je jedan od najobimnijih po isporučenim funkcionalnostima.

### Sprint 10

Uvedene su naprednije i završne funkcionalnosti:

- email upozorenja
- obavijest bibliotekaru o novim rezervacijama
- audit log
- logika kazni za kašnjenje
- online produženje članarine
- integracija sa distributerom putem email zahtjeva
- kolekcije knjiga

Ovim sprintom SmartLib je dobio i operativne i administrativne nadogradnje koje ga čine znatno bližim realnom proizvodu.

---

## 7. Product Backlog

## Opis dokumenta

Ovaj dokument predstavlja ažurirani Product Backlog projekta **Bibliotečki informacioni sistem** u sklopu Sprint 11. Ovo je posljednja verzija Product Backloga koja sadrži finalno stanje. Backlog sadrži stavke koje su trenutno poznate timu. Njegova svrha je da omogući pregled planiranog obima sistema, olakša prioritizaciju rada po sprintovima i služi kao osnova za dalju razradu.

> **Napomena o ažuriranju Product Backloga:**  
> U Product Backlogu su ažurirani statusi stavki koje odgovaraju deliverable-ima **Sprinta 11**.  
> Konkretno, sljedeće stavke:
> - **Dokumentovanje rada**
> - **Deployment procedura**
> - **Continuous Deployment skripta / pipeline**
> - **Korisnički priručnik**
> - **Konačni status Product Backloga**
> - **Release Notes**
> - **Test Summary / QA izvještaj**
> - **Arhitektonski / tehnički pregled**
> - **Finalni sažetak AI upotrebe**
> - **Poznata ograničenja / limiti**
>
> Ove stavke, kao i sve ostale stavke u backlogu su označene kao:
> - **Završeno u Sprintu X** - za potpuno dovršene stavke
> - **Djelimično završeno** - za stavke koje nisu u potpunosti dovršene
> - **Nije završeno** - za stavke koje nisu dovršene ili je odlučeno da ne budu implementirane
> - **Odgođeno** / ostavljeno za budući rad - za stavke koje mogu biti urađene u budućnosti
>

## Tabela 


| ID | Naziv stavke | Kratak opis | Tip | Prioritet | Procjena napora | Status | Veza sa sprintom ili release planom | Napomena |
|:--:| :----------: | :---------: | :-: | :-------: | :-------------: | :----: | :---------------------------------: | :------: |
| PB-1 | Team Charter | Izrada i usaglašavanje timskog chartera | Documentation | Visok | S | Završeno u Sprintu 1 | Sprint 1 | Obavezni artefakt |
| PB-2 | Product Vision | Definisanje problema, korisnika i MVP scope-a | Documentation | Visok | S | Završeno u Sprintu 1 | Sprint 1 | Obavezni artefakt |
| PB-3 | Stakeholder Map | Identifikacija stakeholdera i njihovog utjecaja | Documentation | Visok | S | Završeno u Sprintu 1 | Sprint 1 | Obavezni artefakt |
| PB-4 | Početni Product Backlog | Početno definisanje bitnih artefakata i taskova | Documentation | Visok | M | Završeno u Sprintu 1 | Sprint 1 | Obavezni artefakt |
| PB-5 | Definisanje specifičnih poslovnih pravila | Istražiti i zapisati specifična pravila kako sistem treba da funkcioniše | Research | Visok | S | Završeno u Sprintu 2 | Sprint 1 / Sprint 2 | Ključno za logiku sistema |
| PB-6 | Razrada prioritetnih User Stories i Acceptance Criteria | Definisanje User Stories i kriterija prihvatanja za ključne stavke backloga | Documentation | Visok | M | Završeno u Sprintu 2 | Sprint 2 | Obavezni artefakt |
| PB-7 | Razrada nefunkcionalnih zahtjeva | Definisanje zahtjeva koji određuju kvalitet sistema, uključujući performanse, sigurnost, pouzdanost, upotrebljivost i održavanje | Documentation | Visok | S | Završeno u Sprintu 2 | Sprint 2 | Obavezni artefakt |
| PB-8 | Definisanje objekata sistema | Određivanje svih entiteta koje naš sistem treba da posjeduje | Technical Task | Visok | M | Završeno u Sprintu 3 | Sprint 3 | Preduslov za bazu podataka |
| PB-9 | Risk Register | Identifikacija projektnih rizika, procjena njihovog uticaja i definisanje planova mitigacije | Documentation | Visok | M | Završeno u Sprintu 3 | Sprint 3 | Obavezni artefakt |
| PB-10 | Domain Model i Use Case Model | Modeliranje domene sistema kroz entitete, odnose i ključne use-case scenarije | Documentation | Visok | M | Završeno u Sprintu 3 | Sprint 3 | Obavezni artefakt |
| PB-11 | Architecture Overview | Definisanje osnovnog arhitektonskog pravca sistema, glavnih komponenti i njihovih odgovornosti | Documentation | Visok | M | Završeno u Sprintu 3 | Sprint 3 | Obavezni artefakt |
| PB-12 | Test Strategy | Definisanje pristupa testiranju, nivoa testiranja i veze sa acceptance kriterijima | Documentation | Visok | S | Završeno u Sprintu 3 | Sprint 3 | Obavezni artefakt |
| PB-13 | Dizajn i implementacija baze podataka | Kreiranje sheme baze, migracija i inicijalnih seed podataka | Technical Task | Visok | M | Završeno u Sprintu 4 | Sprint 4 | Preduslov za sve feature sprintove |
| PB-14 | Početna struktura projekta | Odabir tehnologija i arhitekture projekta | Technical Task | Visok | S | Završeno u Sprintu 4 | Sprint 4 | Osnovni tehnički setup i struktura projekta |
| PB-15 | Definition of Done | Definisanje zajedničkih kriterija na osnovu kojih tim smatra stavku završenom | Documentation | Visok | S | Završeno u Sprintu 4 | Sprint 4 | Obavezni artefakt |
| PB-16 | Initial Release Plan | Planiranje inkremenata, glavnih funkcionalnosti, zavisnosti i okvirnih sprintova realizacije | Documentation | Visok | M | Završeno u Sprintu 4 | Sprint 4 | Obavezni artefakt |
| PB-17 | Sistem prijave korisnika | Registrovani korisnici se prijavljuju i odjavljuju iz sistema u skladu sa svojom ulogom | Feature | Visok | M | Završeno u Sprintu 5 | Sprint 5 | Osnova za sve dalje |
| PB-18 | Kreiranje naloga člana | Bibliotekar ili administrator kreira nalog novog člana biblioteke unosom njegovih osnovnih podataka | Feature | Visok | S | Završeno u Sprintu 5 | Sprint 5 | Zamjenjuje samostalnu registraciju člana |
| PB-19 | AI i Decision Log | Uspostava za praćenje rada na projektu | Technical Task | Srednji | S | Završeno u Sprintu 5 | Sprint 5 |  |
| PB-20 | Pregled profila člana | Sistem prikazuje osnovne podatke člana i njegova zaduženja | Feature | Visok | M | Završeno u Sprintu 8 | Sprint 8 |  |
| PB-21 | Pregled i pretraga članova biblioteke | Bibliotekar ili administrator može pregledati i pretraživati članove biblioteke | Feature | Visok | M | Završeno u Sprintu 8 | Sprint 8 | Osnova rada sa članovima |
| PB-22 | Dodavanje nove knjige | Administrator ili bibliotekar dodaje novu knjigu u fond | Feature | Visok | M | Završeno u Sprintu 6 | Sprint 6 | Osnova kataloga |
| PB-23 | Uređivanje podataka o knjizi | Administrator ili bibliotekar može izmijeniti osnovne podatke knjige | Feature | Srednji | S | Završeno u Sprintu 6 | Sprint 6 | Nema historiju promjena |
| PB-24 | Prikaz detalja knjige | Članovi mogu pregledati osnovne podatke knjiga | Feature | Srednji | S | Završeno u Sprintu 6 | Sprint 6 | Sadrži osnovne podatke |
| PB-25 | Upravljanje kategorijama knjiga | Administrator ili bibliotekar dodaje, uređuje i briše kategorije koje se koriste pri dodavanju knjiga | Feature | Srednji | S | Završeno u Sprintu 6 | Sprint 6 |  |
| PB-26 | Upravljanje primjercima knjige | Evidencija više primjeraka iste knjige i njihovog statusa | Feature | Visok | M | Završeno u Sprintu 6 | Sprint 6 |  |
| PB-27 | Brisanje knjige i deaktivacija primjerka | Bibliotekar ili administrator može ukloniti knjigu ili deaktivirati primjerak iz sistema | Feature | Srednji | S | Završeno u Sprintu 6 | Sprint 6 | Nije dozvoljeno ako postoji aktivno zaduženje |
| PB-28 | Pregled kataloga | Korisnik može pregledati listu dostupnih knjiga | Feature | Visok | M | Završeno u Sprintu 6 | Sprint 6 | Zavisi od knjiga i njihovog broja |
| PB-29 | Pretraga knjiga | Korisnik može pretraživati knjige po naslovu, autoru ili ključnoj riječi | Feature | Visok | M | Završeno u Sprintu 7 | Sprint 7 | Nadogradnja kataloga |
| PB-30 | Pregled dostupnosti knjige | Sistem prikazuje da li je knjiga dostupna i broj primjeraka | Feature | Visok | M | Završeno u Sprintu 7 | Sprint 7 |  |
| PB-35 | Pregled vlastitih zaduženja | Član biblioteke vidi koje knjige trenutno ima zadužene | Feature | Visok | S | Završeno u Sprintu 8 | Sprint 8 | Zavisi od zaduživanja |
| PB-36 | Pregled trenutnih zaduženja | Bibliotekar vidi koje su knjige trenutno zadužene od strane članova | Feature | Visok | S | Završeno u Sprintu 8 | Sprint 8 | Zavisi od zaduživanja |
| PB-37 | Pregled historije zaduženja | Sistem prikazuje ranija zaduženja člana | Feature | Nizak | M | Završeno u Sprintu 8 | Sprint 8 | Nadogradnja evidencije |
| PB-31 | Evidencija zaduživanja i vraćanja | Bibliotekar evidentira da je član zadužio određenu knjigu ili je vratio | Feature | Visok | M | Završeno u Sprintu 7 | Sprint 7 | Core funkcionalnost |
| PB-32 | Upravljanje korisnicima od strane admina | Administrator može pregledati sve korisnike, promijeniti ulogu ili deaktivirati nalog | Feature | Srednji | M | Završeno u Sprintu 8 | Sprint 8 | Osnova za administraciju sistema |
| PB-33 | Upravljanje statusom članarine | Bibliotekar ili administrator može pregledati, ažurirati i evidentirati status članarine korisnika | Feature | Srednji | M | Završeno u Sprintu 8 | Sprint 8 | Osnova za administraciju sistema |
| PB-34 | Pregled statusa članarine za člana | Član biblioteke može vidjeti trenutni status i datum isteka svoje članarine | Feature | Srednji | S | Završeno u Sprintu 8 | Sprint 8 |  |
| PB-38 | Početno testiranje | Testiranje prve verzije sistema | Technical Task | Nizak | M | Završeno u Sprintu 8 | Sprint 8 | Testiranje ključnih funkcionalnosti prve integrisane verzije sistema |
| PB-39 | Rezervacija knjige | Član ili bibliotekar može rezervisati nedostupnu knjigu | Feature | Srednji | M | Završeno u Sprintu 9 | Sprint 9 | Zavisi od dostupnosti |
| PB-40 | Pregled aktivnih rezervacija | Bibliotekar vidi listu aktivnih rezervacija | Feature | Srednji | M | Završeno u Sprintu 9 | Sprint 9 | Nad rezervacijama |
| PB-41 | Slanje email upozorenja | Sistem automatski šalje podsjetnike za istek roka vraćanja | Feature | Nizak | M | Završeno u Sprintu 10 | Sprint 10 | Nadogradnja |
| PB-42 | Notifikacija bibliotekara o novoj rezervaciji | Bibliotekar dobija email obavijest kada član kreira rezervaciju | Feature | Nizak | S | Završeno u Sprintu 10 | Sprint 10 | Zavisi od email sistema |
| PB-43 | Automatsko otkazivanje rezervacije | Ako član ne preuzme knjigu u roku X dana, rezervacija se automatski otkazuje | Feature | Nizak | M | Završeno u Sprintu 9 | Sprint 9 | Zavisi od CRON joba i poslovnog pravila |
| PB-44 | Napredna pretraga i filteri | Filtriranje po žanru, godini, izdavaču i slično | Feature | Nizak | M | Završeno u Sprintu 9 | Sprint 9 | Poboljšanje UX |
| PB-45 | Mjesečni izvještaji za upravu | Pregled statistike zaduženja, rezervacija i članstva | Feature | Nizak | L | Završeno u Sprintu 10 | Sprint 10 | Analitika |
| PB-46 | Audit log promjena | Evidencija važnih promjena u sistemu | Technical Task | Nizak | M | Završeno u Sprintu 10 | Sprint 10 | Napredno praćenje |
| PB-47 | Kazne za kasno vraćanje | Evidencija i obračun kazni za prekoračenje roka vraćanja | Feature | Nizak | M | Završeno u Sprintu 10 | Sprint 10 | Ovisi o notifikacionom sistemu i poslovnoj politici |
| PB-48 | Online produžetak članarine | Omogućiti korisniku produženje članarine putem sistema | Feature | Nizak | M | Završeno u Sprintu 10 | Sprint 10 | Zahtijeva integraciju sistema za online plaćanje |
| PB-49 | Integracija sa distributerom knjiga | Povezivanje sistema sa vanjskim servisom za nabavku knjiga | Technical Task | Nizak | L | Završeno u Sprintu 10 | Sprint 10 |  |
| PB-56 | Upravljanje lozinkom korisničkog naloga | Reset zaboravljene lozinke putem emaila i promjena postojeće lozinke | Feature | Visok | L | Završeno u Sprintu 8 | Sprint 8 |
| PB-50 | Stabilizacija sistema | Bug fixing i optimizacija | Technical Task | Visok | M | Završeno u Sprintu 11 | Sprint 11 | Nakon početnog testiranja |
| PB-51 | Izrada liste poznatih ograničenja i tehničkog duga | Popis ograničenja i nedovršenih dijelova | Documentation | Visok | M | Završeno u Sprintu 11 | Sprint 11 |  |
| PB-52 | Release Notes | Opis funkcionalnosti kroz verzije | Documentation | Visok | M | Završeno u Sprintu 11 | Sprint 11 |  |
| PB-53 | Korisnička dokumentacija | Izrada dokumentacije za upotrebu | Documentation | Visok | M | Završeno u Sprintu 11 | Sprint 11 | Bez tehničkih dijelova |
| PB-54 | Tehnička dokumentacija | Izrada dokumentacije za opis sistema | Documentation | Visok | M | Završeno u Sprintu 11 | Sprint 11 | Detaljan opis |
| PB-55 | Završna demonstracija | Finalizacija projekta | Documentation | Visok | M | Završeno u Sprintu 11 | Sprint 11 |  |
| PB-56 | Unapređenje korisničkog interfejsa | Poboljšanje izgleda i responzivnosti ključnih stranica sistema, uz bolju navigaciju i usklađen vizuelni stil. | Feature | Srednji | M | Završeno u Sprintu 9 | Sprint 9 | Fokus na bolji UX i preglednost sistema. |
| PB-57 | Modul vijesti i novosti | Prikaz bibliotečkih vijesti i obavještenja korisnicima kroz posebnu javnu stranicu. | Feature | Srednji | M | Završeno u Sprintu 9 | Sprint 9 | Informisanje korisnika o novostima u biblioteci. |
| PB-58 | Kalendar događaja | Prikaz planiranih bibliotečkih događaja kroz kalendar i listu predstojećih aktivnosti. | Feature | Srednji | M | Završeno u Sprintu 9 | Sprint 9 | Omogućava korisnicima lakše praćenje aktivnosti. |
| PB-59 | Forum zajednice za članove | Omogućavanje članovima biblioteke da učestvuju u diskusijama i razmjeni preporuka kroz forum zajednice. | Feature | Srednji | M | Završeno u Sprintu 9 | Sprint 9 | Podstiče interakciju i angažman članova. |


## Legenda

### Tip stavke
- **Feature** - funkcionalnost vidljiva korisniku ili važna mogućnost sistema
- **Technical Task** - tehnički zadatak potreban za implementaciju, arhitekturu ili kvalitet sistema
- **Research** - istraživačka stavka za razjašnjavanje pravila i sličnog
- **Documentation** - projektni artefakt ili dokument
- **Bug** - ispravka greške u postojeće funkcionalnosti

### Prioritet
- **Visok** - bitno za MVP ili početne sprintove
- **Srednji** - važno, međutim nije presudno za prvu verziju sistema
- **Nizak** - korisno proširenje ili neka kasnija nadogradnja

### Procjena napora
- **S** - mali zadatak
- **M** - srednji zadatak
- **L** - veliki zadatak

### Status
- **Završeno** - stavka realizovana i pregledana
- **U toku** - rad na stavci je započet
- **U backlogu** - stavka je evidentirana, ali još nije započeta
- **Odgođeno** - stavka je svjesno pomjerena za kasniji period

---

## 8. Glavne tehničke odluke

Tokom razvoja izdvajaju se sljedeće ključne tehničke odluke:

### Modularna organizacija rješenja

Kod je organizovan kroz projekte `SmartLib.Core`, `SmartLib.Infrastructure`, `SmartLib.Web` i `SmartLib.API`. Ova odluka olakšala je podjelu odgovornosti između domenskog modela, pristupa podacima, web UI-ja i API sloja.

### ASP.NET Core MVC kao glavni interfejs

Tim je odabrao server-side renderovani MVC pristup sa Razor view-ovima. To je ubrzalo implementaciju, pojednostavilo autentifikaciju i omogućilo da se cijeli sistem razvija unutar jednog tehnološkog stacka u C#-u.

### Dodatni REST API sa JWT autentifikacijom

Iako je glavni interfejs MVC web aplikacija, razvijen je i API sloj sa JWT autentifikacijom za dio funkcionalnosti. Time je ostavljena mogućnost širenja sistema i integracija izvan web aplikacije.

### Prelazak sa PostgreSQL na MySQL

Prema sprint dokumentaciji, u Sprintu 5 donesena je odluka da se projekat prebaci sa PostgreSQL na MySQL radi bolje usklađenosti sa okruženjem i alatima kojima tim raspolaže. Ova promjena uticala je na dalju infrastrukturu, deployment i rad sa bazom.

### EnsureCreated i ručno proširivanje šeme

Umjesto klasičnog oslanjanja na EF migracije, u `Program.cs` je uveden pristup gdje aplikacija pri pokretanju koristi `EnsureCreated()` i dodatne `ALTER TABLE` / `CREATE TABLE IF NOT EXISTS` naredbe. Ovo je ubrzalo isporuku, ali je uvelo i tehnički dug.

### Redis cache sa fallback mehanizmom

Za keširanje se koristi Upstash Redis kada su produkcijske varijable dostupne, a lokalno se koristi `DistributedMemoryCache` kao fallback. Time je sistem ostao pokretljiv i bez pune cloud infrastrukture.

### Email servis sa više fallback nivoa

Email komunikacija izvedena je tako da primarno koristi Brevo HTTP API, zatim SMTP fallback za lokalni razvoj i na kraju log fallback. Ovo je bila praktična odluka zbog ograničenja Render platforme.

### Uvođenje background servisa

U sistem su uključeni background servisi za podsjetnike i čišćenje deaktiviranih naloga. Time je dio periodičnih zadataka automatizovan bez uvođenja zasebnih mikroservisa.

---

## 9. Najveći problemi tokom razvoja i način rješavanja

### Promjena baze podataka usred razvoja

Jedan od većih problema bio je prelazak sa PostgreSQL na MySQL nakon što je dio projektne dokumentacije već bio zasnovan na ranijoj odluci. Tim je ovo riješio tako što je promjenu formalno evidentirao u sprint review i decision log dokumentima, a zatim prilagodio konfiguraciju, ORM provider i deployment.

### Održavanje konzistentnosti podataka kroz više modula

Zaduženja, primjerci, rezervacije, članarine i audit log zavise jedni od drugih. Rizik je bio da djelimične izmjene ostave sistem u nekonzistentnom stanju. Ovo je rješavano uvođenjem kontrola u repozitorijima i servisima, te poslovnim pravilima koja blokiraju nevalidne akcije.

### Širenje scope-a projekta

Projekat je počeo kao klasični bibliotečki informacioni sistem, ali je vremenom proširen forumom, recenzijama, vijestima, kalendarom, listom želja, kolekcijama, email notifikacijama i nabavkom. Problem je bio kako sačuvati preglednost sistema dok raste broj modula. Tim je to rješavao održavanjem podjele po slojevima i po funkcionalnim cjelinama.

### Razvoj cloud deploymenta sa više eksternih servisa

Produkcijsko okruženje uključuje Render, Docker Hub, TiDB Cloud, Upstash Redis i Brevo. Najveći izazovi bili su konekcije, environment varijable, ograničenja SMTP portova i ponovljiv deployment. To je riješeno pisanjem GitHub Actions pipeline-a, Dockerfile-a i detaljne deployment dokumentacije.

### Sigurnosni i korisnički tokovi za lozinke

Reset lozinke zahtijevao je sigurno generisanje tokena, email dostavu i validaciju isteka. Tim je to riješio hashiranjem reset tokena prije pohrane i ograničavanjem njihovog trajanja.

### Dokumentovanje paralelno sa razvojem

Veliki broj sprint artefakata i promjena u backlogu zahtijevao je disciplinu u dokumentovanju. Tim je ovo rješavao kroz redovno održavanje sprint review, backlog, AI log i decision log dokumenata.

---

## 10. Šta bi tim unaprijedio da se projekat nastavlja

Ako bi se projekat nastavljao, najveći prioriteti za unapređenje bili bi:

### Tehnička stabilizacija

- prelazak sa `EnsureCreated()` pristupa na čiste i kontrolisane EF Core migracije
- smanjenje količine schema logike u `Program.cs`
- dodatna refaktorizacija najvećih kontrolera i servisa

### Završavanje poslovnih tokova do pune produkcijske verzije

- integracija stvarnog sistema online plaćanja za članarinu
- puniji model kazni sa evidencijom iznosa i izmirenja
- prava API integracija sa distributerima knjiga

### Kvalitet i održavanje

- veća test pokrivenost za novije module kao što su forum, recenzije, kolekcije i notifikacije
- uvođenje jačih monitoring i logging mehanizama
- formalna lista poznatih ograničenja, bugova i tehničkog duga

### Infrastruktura i operativnost

- backup strategija za bazu
- bolje upravljanje tajnama i konfiguracijom
- odvajanje razvojne i produkcijske šeme baze
- eventualno izdvajanje nekih modula u zasebne servise ako sistem dodatno poraste

### Korisničko iskustvo

- dalje poliranje interfejsa
- naprednije preporuke knjiga
- bogatiji sistem notifikacija unutar aplikacije
- preciznija moderacija i upravljanje zajednicom

---

## 11. Zaključak

Tim je kroz jedanaest sprintova izgradio, stabilizovao, dokumentovao i pripremio za završnu prezentaciju funkcionalan i širok bibliotečki informacioni sistem koji pokriva osnovne operativne procese biblioteke, ali i više dodatnih modula koji unapređuju korisničko iskustvo. Posebno je značajno što projekat nije ostao samo na CRUD funkcionalnostima, nego je proširen rezervacijama, email podsjetnicima, audit logom, izvještajima, forumom, recenzijama, vijestima, kalendarom, listom želja i kolekcijama.

Najveće snage projekta su obim implementiranih funkcionalnosti, jasan razvoj kroz sprintove, postojanje testova na više nivoa i uspostavljen deployment pipeline. Najveći prostor za napredak nalazi se u tehničkoj stabilizaciji baze, dovršavanju pojedinih poslovnih tokova do produkcijskog nivoa i dodatnom smanjenju tehničkog duga.

U cjelini, SmartLib predstavlja uspješno realizovan studentski projekat koji je izrastao iz osnovnog bibliotečkog sistema u znatno bogatiju platformu za rad biblioteke i interakciju sa njenim članovima.
