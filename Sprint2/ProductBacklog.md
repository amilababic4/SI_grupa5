# Product Backlog


## Opis dokumenta

Ovaj dokument predstavlja ažurirani Product Backlog projekta **Bibliotečki informacioni sistem** u sklopu Sprint 2. Backlog sadrži stavke koje su trenutno poznate timu. Njegova svrha je da omogući pregled planiranog obima sistema, 
olakša prioritizaciju rada po sprintovima i služi kao osnova za dalju razradu.



## Tabela 

| ID | Naziv stavke | Kratak opis | Tip | Prioritet | Procjena napora | Status | Veza sa sprintom ili release planom | Napomena |
|:--:| :----------: | :---------: | :-: | :-------: | :-------------: | :----: | :---------------------------------: | :------: |
| PB-1 | Team Charter | Izrada i usaglašavanje timskog chartera | Documentation  | Visok | S | Završeno u Sprintu 1 | Sprint 1 | Obavezni artefakt |
| PB-2 | Product Vision | Definisanje problema, korisnika i MVP scope-a | Documentation  | Visok | S | Završeno u Sprintu 1 | Sprint 1 | Obavezni artefakt |
| PB-3 | Stakeholder Map | Identifikacija stakeholdera i njihovog utjecaja | Documentation  | Visok | S | Završeno u Sprintu 1 | Sprint 1 | Obavezni artefakt |
| PB-4 | Početni Product Backlog | Početno definisanje bitnih artefakata i taskova | Documentation  | Visok | M | Završeno u Sprintu 1 | Sprint 1 | Obavezni artefakt |
| PB-5 | Definisanje specifičnih poslovnih pravila | Istražiti i zapisati specifična pravila kako sistem treba da funkcioniše | Research | Visok | S | Završeno u Sprintu 2 | Sprint 1 / Sprint 2 | Ključno za logiku sistema |
| PB-6 | Razrada prioritetnih User Stories i Acceptance Criteria | Definisanje User Stories i kriterija prihvatanja za ključne stavke backloga | Documentation | Visok | M | Završeno u Sprintu 2 | Sprint 2 | Obavezni artefakt |
| PB-7 | Razrada nefunkcionalnih zahtjeva | Definisanje zahtjeva koji određuju kvalitet sistema, uključujući performanse, sigurnost, pouzdanost, upotrebljivost i održavanje | Documentation | Visok | S | Završeno u Sprintu 2 | Sprint 2 | Obavezni artefakt | 
| PB-8 | Definisanje objekata sistema | Određivanje svih entiteta koje naš sistem treba da posjeduje | Technical Task | Visok | M | U backlogu | Sprint 3 | Preduslov za bazu podataka|
| PB-9 | Dizajn i implementacija baze podataka | Kreiranje sheme baze, migracija i inicijalnih seed podataka | Technical Task | Visok | M | U backlogu | Sprint 4 | Preduslov za sve feature sprintove |
| PB-10 | Početna struktura projekta | Odabir tehnologija i arhitekture projekta | Technical task | Visok | S | U backlogu | Sprint 4 | |
| PB-11 | Sistem prijave korisnika | Registrovani korisnici se prijavljuju i odjavljuju iz sistema u skladu sa svojom ulogom | Feature | Visok | M | U backlogu | Sprint 5 | Osnova za sve dalje |
| PB-12 | Kreiranje naloga člana | Bibliotekar ili administrator kreira nalog novog člana biblioteke unosom njegovih osnovnih podataka | Feature | Visok | S | U backlogu | Sprint 5 | Zamjenjuje samostalnu registraciju člana |
| PB-13 | AI i Decision Log | Uspostava za praćenje rada na projektu | Technical task | Srednji | S | U backlogu | Sprint 5  |  |
| PB-14 | Pregled profila člana | Sistem prikazuje osnovne podatke člana i njegova zaduženja | Feature | Visok | M | U backlogu | Sprint 8 |  |
| PB-15 | Pregled i pretraga članova biblioteke | Bibliotekar ili administrator može pregledati i pretraživati članove biblioteke | Feature | Visok | M | U backlogu | Sprint 8 | Osnova rada sa članovima |
| PB-16 | Dodavanje nove knjige | Administrator ili bibliotekar dodaje novu knjigu u fond | Feature | Visok | M | U backlogu | Sprint 6 | Osnova kataloga |
| PB-17 | Uređivanje podataka o knjizi | Administrator ili bibliotekar može izmijeniti osnovne podatke knjige | Feature | Srednji | S | U backlogu | Sprint 6 | Nema historiju promjena |
| PB-18 | Prikaz detalja knjige | Članovi mogu pregledati osnovne podatke knjiga | Feature | Srednji | S | U backlogu | Sprint 7 | Sadrži osnovne podatke |
| PB-19 | Upravljanje kategorijama knjiga | Administrator ili bibliotekar dodaje, uređuje i briše kategorije koje se koriste pri dodavanju knjiga | Feature | Srednji | S | U backlogu | Sprint 7 | |
| PB-20 | Upravljanje primjercima knjige | Evidencija više primjeraka iste knjige i njihovog statusa | Feature | Visok | M | U backlogu | Sprint 7 | |
| PB-21 | Brisanje knjige i deaktivacija primjerka | Bibliotekar ili administrator može ukloniti knjigu ili deaktivirati primjerak iz sistema | Feature | Srednji | S | U backlogu | Sprint 7 | Nije dozvoljeno ako postoji aktivno zaduženje |
| PB-22 | Pregled kataloga | Korisnik može pregledati listu dostupnih knjiga | Feature | Visok | M | U backlogu | Sprint 6 | Zavisi od knjiga i njihovog broja |
| PB-23 | Pretraga knjiga | Korisnik može pretraživati knjige po naslovu, autoru ili ključnoj riječi | Feature | Visok | M | U backlogu | Sprint 7 | Nadogradnja kataloga |
| PB-24 | Pregled dostupnosti knjige | Sistem prikazuje da li je knjiga dostupna i broj primjeraka | Feature | Visok | M | U backlogu | Sprint 7 | |
| PB-25 | Evidencija zaduživanja i vraćanja | Bibliotekar evidentira da je član zadužio određenu knjigu ili je vratio | Feature | Visok | M | U backlogu | Sprint 8 | Core funkcionalnost |
| PB-26 | Upravljanje korisnicima od strane admina | Administrator može pregledati sve korisnike, promijeniti ulogu ili deaktivirati nalog | Feature | Srednji | M | U backlogu | Sprint 8 | Osnova za administraciju sistema |
| PB-27 | Upravljanje statusom članarine | Bibliotekar ili administrator može pregledati, ažurirati i evidentirati status članarine korisnika | Feature | Srednji | M | U backlogu | Sprint 8 | Osnova za administraciju sistema |
| PB-28 | Pregled statusa članarine za člana |Član biblioteke može vidjeti trenutni status i datum isteka svoje članarine | Feature | Srednji | S | U backlogu | Sprint 8 | |
| PB-29 | Pregled vlastitih zaduženja | Član biblioteke vidi koje knjige trenutno ima zadužene | Feature | Visok | S | U backlogu | Sprint 9 | Zavisi od zaduživanja |
| PB-30 | Pregled trenutnih zaduženja | Bibliotekar vidi koje su knjige trenutno zadužene od strane članova | Feature | Visok | S | U backlogu | Sprint 9 | Zavisi od zaduživanja |
| PB-31 | Pregled historije zaduženja | Sistem prikazuje ranija zaduženja člana | Feature | Nizak | M | U backlogu | Sprint 8 | Nadogradnja evidencije |
| PB-32 | Početno testiranje | Testiranje prve verzije sistema | Technical task | Nizak | M | U backlogu | Sprint 8 | Testiranje ključnih funkcionalnosti prve integrisane verzije sistema|
| PB-33 | Rezervacija knjige | Član ili bibliotekar može rezervisati nedostupnu knjigu | Feature | Srednji | M | U backlogu | Sprint 9 | Zavisi od dostupnosti |
| PB-34 | Pregled aktivnih rezervacija | Bibliotekar vidi listu aktivnih rezervacija | Feature | Srednji | M | U backlogu | Sprint 9 | Nad rezervacijama |
| PB-35 | Slanje email upozorenja | Sistem automatski šalje podsjetnike za istek roka vraćanja | Feature | Nizak | M | U backlogu | Sprint 10 | Nadogradnja |
| PB-36 | Notifikacija bibliotekara o novoj rezervaciji | Bibliotekar dobija email obavijest kada član kreira rezervaciju | Feature | Nizak | S | U backlogu | Sprint 10 | Zavisi od email sistema |
| PB-37 | Automatsko otkazivanje rezervacije | Ako član ne preuzme knjigu u roku X dana, rezervacija se automatski otkazuje | Feature | Nizak | M | U backlogu | Sprint 10 | Zavisi od CRON joba i poslovnog pravila |
| PB-38 | Napredna pretraga i filteri | Filtriranje po žanru, godini, izdavaču i slično | Feature | Nizak | M | U backlogu | Sprint 9 | Poboljšanje UX |
| PB-39 | Mjesečni izvještaji za upravu | Pregled statistike zaduženja, rezervacija i članstva | Feature | Nizak | L | U backlogu | Sprint 10 | Analitika |
| PB-40 | Audit log promjena | Evidencija važnih promjena u sistemu | Technical task | Nizak | M | U backlogu | Sprint 10 | Napredno praćenje |
| PB-41 | Kazne za kasno vraćanje | Evidencija i obračun kazni za prekoračenje roka vraćanja | Feature | Nizak | M | U backlogu | Sprint 10 | Ovisi o notifikacionom sistemu i poslovnoj politici |
| PB-42 | Online produžetak članarine | Omogućiti korisniku produženje članarine putem sistema | Feature | Nizak | M | U backlogu | Sprint 10 | Zahtijeva integraciju sistema za online plaćanje |
| PB-43 | Integracija sa distributerom knjiga | Povezivanje sistema sa vanjskim servisom za nabavku knjiga | Technical Task | Nizak | L | U backlogu | Sprint 10 |   |
| PB-44 | Stabilizacija sistema | Bug fixing i optimizacija | Technical task | Visok | M | U backlogu | Sprint 11 | Nakon početnog testiranja |
| PB-45 | Izrada liste poznatih ograničenja i tehničkog duga | Popis ograničenja i nedovršenih dijelova | Documentation| Visok | M | U backlogu | Sprint 11 |  |
| PB-46 | Release Notes | Opis funkcionalnosti kroz verzije | Documentation | Visok | M | U backlogu | Sprint 12 |  |
| PB-47 | Korisnička dokumentacija | Izrada dokumentacije za upotrebu | Documentation | Visok | M | U backlogu | Sprint 12 | Bez tehničkih dijelova |
| PB-48 | Tehnička dokumentacija | Izrada dokumentacije za opis sistema | Documentation | Visok | M | U backlogu | Sprint 12 | Detaljan opis |
| PB-49 | Završna demonstracija | Finalizacija projekta | Documentation | Visok | M | U backlogu | Sprint 13 |  |



## Legenda

### Tip stavke
- **Feature** - funkcionalnost vidljiva korisniku ili važna mogućnost sistema
- **Technical Task** - tehnički zadatak potreban za implementaciju, arhitekturu ili kvalitet sistema
- **Research** - istraživačka stavka za razjašnjavanje pravila i sličnog
- **Documentation** - projektni artefakt ili dokument
- **Bug** - ispravka greške u postojećoj funkcionalnosti

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
