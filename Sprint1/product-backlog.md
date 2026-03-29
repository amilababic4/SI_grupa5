# Product Backlog


## Opis dokumenta

Ovaj dokument predstavlja početni Product Backlog projekta **Bibliotečki informacioni sistem**. Backlog sadrži stavke koje su trenutno poznate timu. Njegova svrha je da omogući pregled planiranog obima sistema, 
olakša prioritizaciju rada po sprintovima i služi kao osnova za dalju razradu.



## Tabela 

| ID    | Naziv stavke                              | Kratak opis                                                              | Tip            | Prioritet | Procjena napora | Status               | Veza sa sprintom ili release planom | Napomena                                    |
| :-----:| ----------------------------------------- | ------------------------------------------------------------------------ | :--------------: | :---------: | :---------------: | -------------------- | -------------------------- | ------------------------------------------- |
| PB-1 | Team Charter                     | Izrada i usaglašavanje timskog chartera                                  | Documentation  | Visok     | S               | Završeno u Sprintu 1 | Sprint 1   | Obavezni artefakt                           |
| PB-2 | Product Vision                    | Definisanje problema, korisnika i MVP scope-a                             | Documentation  | Visok     | S               | Završeno u Sprintu 1 | Sprint 1     | Obavezni artefakt                           |
| PB-3 | Stakeholder Map                  | Identifikacija stakeholdera i njihovog utjecaja                           | Documentation  | Visok     | S               | Završeno u Sprintu 1 | Sprint 1                   | Obavezni artefakt                           |
| PB-4 | Početni Product Backlog          | Početno definisanje bitnih artefakata i taskova                            | Documentation  | Visok     | M               | Završeno u Sprintu 1 | Sprint 1                   | Obavezni artefakt                           |
| PB-5 | Definisanje specifičnih poslovnih pravila | Istražiti i zapisati specifična pravila kako sistem treba da funkcioniše                | Research       | Visok     | S               | U backlogu           | Sprint 1 / Sprint 2        | Ključno za logiku sistema                   |
| PB-6 | Definisanje objekata sistema               | Određivanje svih entiteta koje naš sistem treba da posjeduje               | Technical Task       | Visok     | M               | U backlogu           | Sprint 3            |                        |
| PB-7 | Početna struktura projekta            | Odabir tehnologija i arhitekture projekta                             | Technical task | Visok   | S               | U backlogu           | Sprint 4                   |                     |
| PB-8 | Sistem prijave korisnika                | Član, bibliotekar i administrator se registruju, prijavljuju i odjavljuju u sistemu                | Feature        | Visok     | M               | U backlogu           | MVP / Release 1            | Osnova za sve dalje                        |
| PB-9 | Pregled profila člana                     | Sistem prikazuje osnovne podatke člana i njegova zaduženja               | Feature        | Visok     | M               | U backlogu           | MVP / Release 1            |                   |
| PB-10 | Dodavanje nove knjige                 | Administrator ili bibliotekar dodaje novu knjigu u fond                  | Feature        | Visok     | M               | U backlogu           | MVP / Release 1            | Osnova kataloga                            |
| PB-11 | Uređivanje podataka o knjizi              | Administrator ili bibliotekar može izmijeniti osnovne podatke knjige     | Feature        | Srednji   | S               | U backlogu           | MVP / Release 1            | Nema historiju promjena                            |
| PB-12 | Prikaz detalja knjige              | Članovi mogu pregledati osnovne podatke knjiga     | Feature        | Srednji   | S               | U backlogu           | MVP / Release 1            | Sadrži osnovne podatke                           |
| PB-13 | Pregled kataloga                          | Korisnik može pregledati listu dostupnih knjiga               | Feature        | Visok     | M               | U backlogu           | MVP / Release 1            | Zavisi od knjiga i njihovog broja                           |
| PB-14 | Pretraga knjiga                           | Korisnik može pretraživati knjige po naslovu, autoru ili ključnoj riječi | Feature        | Visok     | M               | U backlogu           | MVP / Release 1            | Nadogradnja kataloga                       |
| PB-15 | Pregled dostupnosti knjige                | Sistem prikazuje da li je knjiga dostupna i broj primjeraka              | Feature        | Visok     | M               | U backlogu           | MVP / Release 1            |                       |
| PB-16 | Evidencija zaduživanja i vraćanja                    | Bibliotekar evidentira da je član zadužio određenu knjigu ili je vratio                | Feature        | Visok     | M               | U backlogu           | MVP / Release 1            | Core funkcionalnost                        |
| PB-17 | Pregled vlastitih zaduženja               | Član biblioteke vidi koje knjige trenutno ima zadužene                   | Feature        | Visok   | S               | U backlogu           | MVP / Release 1            | Zavisi od zaduživanja                      |
| PB-18 | Pregled trenutnih zaduženja               | Bibliotekar vidi koje su knjige trenutno zadužene od strane članova                   | Feature        | Visok   | S               | U backlogu           | MVP / Release 1            | Zavisi od zaduživanja                      |
| PB-19 | Upravljanje primjercima knjige | Evidencija više primjeraka iste knjige i njihovog statusa | Feature | Visok | M | U backlogu | MVP / Release 1 | Bitno za dostupnost |
| PB-20 | Pregled historije zaduženja               | Sistem prikazuje ranija zaduženja člana                                  | Feature        | Nizak     | M               | U backlogu           | Release 2       | Nadogradnja evidencije                               |
| PB-21 | Početno testiranje                      | Testiranje prve verzije sistema                                   | Technical task | Nizak     | M               | U backlogu           | Release 1       | Napredna sigurnost                        |
| PB-22 | Rezervacija knjige                        | Član ili bibliotekar može rezervisati nedostupnu knjigu                  | Feature        | Srednji     | M               | U backlogu           | Release 2            | Zavisi od dostupnosti                      |
| PB-23 | Pregled aktivnih rezervacija              | Bibliotekar vidi listu aktivnih rezervacija                              | Feature        | Srednji   | M               | U backlogu           | Release 2            | Nad rezervacijama                          |
| PB-24 | Slanje email podsjetnika                  | Sistem automatski šalje podsjetnike za rok vraćanja                      | Feature        | Nizak     | M               | U backlogu           | Release 2       | Nadogradnja                               |
| PB-25 | Slanje email upozorenja                  | Sistem automatski šalje podsjetnike za istek roka vraćanja                      | Feature        | Nizak     | M               | U backlogu           | Release 2       | Nadogradnja                               |
| PB-26 | Napredna pretraga i filteri               | Filtriranje po žanru, godini, izdavaču i slično                          | Feature        | Nizak     | M               | U backlogu           | Release 2       | Poboljšanje UX                            |
| PB-27 | Mjesečni izvještaji za upravu                      | Pregled statistike zaduženja, rezervacija i članstva                     | Feature        | Nizak     | L               | U backlogu           | Release 2       | Analitika                                 |
| PB-28 | Audit log promjena                        | Evidencija važnih promjena u sistemu                                     | Technical task | Nizak     | M               | U backlogu           | Release 2       | Napredno praćenje                         |
| PB-29 | Kazne za kasno vraćanje | Evidencija i obračun kazni za prekoračenje roka vraćanja | Feature | Nizak | M | U backlogu | Release 2 | Ovisi o notifikacionom sistemu i poslovnoj politici |
| PB-30 | Online produžetak članarine | Omogućiti korisniku produženje članarine putem sistema | Feature | Nizak | M | U backlogu | Release 2 | Zahtijeva integraciju sistema za online plaćanje |
| PB-31 | Integracija sa distributerom knjiga | Povezivanje sistema sa vanjskim servisom za nabavku knjiga | Technical Task | Nizak | L | U backlogu | Release 2 |   |
| PB-32 | Stabilizacija sistema                    | Bug fixing i optimizacija                                          | Technical task | Visok     | M     | U backlogu | Sprint 11    | Nakon početnog testiranja |



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
