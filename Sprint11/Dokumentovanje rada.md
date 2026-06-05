# SmartLib - Dokumentovanje rada

## 1. Uvod

Ovaj dokument predstavlja dokumentovanje rada tima na projektu **SmartLib**, bibliotečkom informacionom sistemu razvijenom u okviru predmeta Softverski inženjering. Dokument je zasnovan na stvarnoj implementaciji u repozitoriju projekta, sprint artefaktima i isporučenim inkrementima od Sprinta 1 do Sprinta 10.

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

## 7. Šta je završeno, djelimično završeno ili nije završeno

### 7.1 Pregled po sedmicama / sprintovima

U nastavku je pregled rada po sedmicama, u formatu sličnom product backlogu. Statusi su izvedeni iz sprint review dokumenata, ažuriranog product backloga i stvarnog stanja implementacije u kodu.

| Sedmica / Sprint | PB ID | Stavka | Status | Napomena |
|---|---|---|---|---|
| Sedmica 1 / Sprint 1 | PB-1 | Team Charter | Završeno | Definisan način rada tima i osnovna pravila saradnje. |
| Sedmica 1 / Sprint 1 | PB-2 | Product Vision | Završeno | Definisani svrha sistema, problem i MVP okvir. |
| Sedmica 1 / Sprint 1 | PB-3 | Stakeholder Map | Završeno | Identifikovani glavni korisnici i stakeholderi projekta. |
| Sedmica 1 / Sprint 1 | PB-4 | Početni Product Backlog | Završeno | Postavljena početna lista funkcionalnosti i artefakata. |
| Sedmica 2 / Sprint 2 | PB-5 | Definisanje poslovnih pravila | Završeno | Razrađena osnovna pravila rada biblioteke. |
| Sedmica 2 / Sprint 2 | PB-6 | Razrada User Storyja i acceptance kriterija | Završeno | Pripremljena detaljna osnova za implementaciju funkcionalnosti. |
| Sedmica 2 / Sprint 2 | PB-7 | Nefunkcionalni zahtjevi | Završeno | Definisani zahtjevi za kvalitet, sigurnost i održavanje. |
| Sedmica 3 / Sprint 3 | PB-8 | Definisanje objekata sistema | Završeno | Modelovani glavni entiteti sistema. |
| Sedmica 3 / Sprint 3 | PB-9 | Risk Register | Završeno | Identifikovani glavni projektni i tehnički rizici. |
| Sedmica 3 / Sprint 3 | PB-10 | Domain Model i Use Case Model | Završeno | Razrađeni glavni poslovni scenariji. |
| Sedmica 3 / Sprint 3 | PB-11 | Architecture Overview | Završeno | Definisan arhitektonski pristup i podjela odgovornosti. |
| Sedmica 3 / Sprint 3 | PB-12 | Test Strategy | Završeno | Definisan pristup unit, integracionom, UI i security testiranju. |
| Sedmica 4 / Sprint 4 | PB-13 | Dizajn i implementacija baze podataka | Završeno | Postavljena baza i inicijalna podatkovna osnova. |
| Sedmica 4 / Sprint 4 | PB-14 | Početna struktura projekta | Završeno | Kreirani `Core`, `Infrastructure`, `Web` i `API` projekti. |
| Sedmica 4 / Sprint 4 | PB-15 | Definition of Done | Završeno | Dogovoreni kriteriji završenosti backlog stavki. |
| Sedmica 4 / Sprint 4 | PB-16 | Initial Release Plan | Završeno | Isplanirani sprintovi i inkrementi. |
| Sedmica 5 / Sprint 5 | PB-17 | Sistem prijave korisnika | Završeno | Implementirane prijava, odjava, sesija i zaštita ruta. |
| Sedmica 5 / Sprint 5 | PB-18 | Kreiranje naloga člana | Završeno | Bibliotekar i admin mogu kreirati nove članove. |
| Sedmica 5 / Sprint 5 | PB-19 | AI i Decision Log | Završeno | Uspostavljena evidencija AI korištenja i tehničkih odluka. |
| Sedmica 6 / Sprint 6 | PB-22 | Dodavanje nove knjige | Završeno | Validacija ISBN-a i kreiranje primjeraka su implementirani. |
| Sedmica 6 / Sprint 6 | PB-23 | Uređivanje podataka o knjizi | Završeno | Omogućeno ažuriranje knjiga iz administratorskog dijela. |
| Sedmica 6 / Sprint 6 | PB-25 | Upravljanje kategorijama knjiga | Završeno | Dodavanje, izmjena i brisanje kategorija. |
| Sedmica 6 / Sprint 6 | PB-26 | Upravljanje primjercima knjige | Završeno | Evidencija i status fizičkih primjeraka. |
| Sedmica 6 / Sprint 6 | PB-27 | Brisanje knjige i deaktivacija primjerka | Završeno | Uvedena zaštita od brisanja vezanog za aktivna zaduženja. |
| Sedmica 6 / Sprint 6 | PB-28 | Pregled kataloga | Završeno | Katalog sa paginacijom je funkcionalan. |
| Sedmica 7 / Sprint 7 | PB-24 | Prikaz detalja knjige | Završeno | Detalji knjige i dostupnost su dostupni korisnicima. |
| Sedmica 7 / Sprint 7 | PB-29 | Pretraga knjiga | Završeno | Pretraga po naslovu i autoru je implementirana. |
| Sedmica 7 / Sprint 7 | PB-30 | Pregled dostupnosti knjige | Završeno | Prikazan broj slobodnih primjeraka i status knjige. |
| Sedmica 7 / Sprint 7 | PB-31 | Evidencija zaduživanja i vraćanja | Završeno | Implementiran glavni poslovni tok biblioteke. |
| Sedmica 7 / Sprint 7 | PB-35 | Pregled vlastitih zaduženja | Završeno | Član vidi aktivna zaduženja i rok vraćanja. |
| Sedmica 7 / Sprint 7 | PB-36 | Pregled trenutnih zaduženja | Završeno | Bibliotekar vidi sva aktivna zaduženja. |
| Sedmica 8 / Sprint 8 | PB-20 | Pregled profila člana | Završeno | Profil povezan sa podacima o korisniku i zaduženjima. |
| Sedmica 8 / Sprint 8 | PB-21 | Pregled i pretraga članova biblioteke | Završeno | Bibliotekar i admin mogu pretraživati članove. |
| Sedmica 8 / Sprint 8 | PB-32 | Upravljanje korisnicima od strane admina | Završeno | Promjena uloga i deaktivacija su podržani. |
| Sedmica 8 / Sprint 8 | PB-33 | Upravljanje statusom članarine | Završeno | Evidencija i ažuriranje članarine su implementirani. |
| Sedmica 8 / Sprint 8 | PB-34 | Pregled statusa članarine za člana | Završeno | Član vidi trajanje i status članarine. |
| Sedmica 8 / Sprint 8 | PB-37 | Pregled historije zaduženja | Završeno | Dostupan pregled završenih i ranijih zaduženja. |
| Sedmica 8 / Sprint 8 | PB-38 | Početno testiranje | Završeno | Uspostavljen širi testni set za integrisanu verziju. |
| Sedmica 8 / Sprint 8 | PB-56 | Upravljanje lozinkom korisničkog naloga | Završeno | Promjena i reset lozinke putem emaila. |
| Sedmica 9 / Sprint 9 | PB-39 | Rezervacija knjige | Završeno | Rezervacije nedostupnih knjiga rade kroz UI. |
| Sedmica 9 / Sprint 9 | PB-40 | Pregled aktivnih rezervacija | Završeno | Bibliotekar može pregledati aktivne rezervacije. |
| Sedmica 9 / Sprint 9 | PB-43 | Automatsko otkazivanje rezervacije | Završeno | Rezervacije se zatvaraju po isteku roka. |
| Sedmica 9 / Sprint 9 | PB-44 | Napredna pretraga i filteri | Završeno | Kombinovani filteri po kategoriji, izdavaču i godini. |
| Sedmica 9 / Sprint 9 | PB-45 | Mjesečni izvještaji za upravu | Završeno | Implementirani izvještaji i pregled statistike. |
| Sedmica 9 / Sprint 9 | PB-56 | Unapređenje korisničkog interfejsa | Završeno | Poboljšan izgled i responzivnost ključnih stranica. |
| Sedmica 9 / Sprint 9 | PB-57 | Modul vijesti i novosti | Završeno | Dodan modul vijesti i obavještenja biblioteke. |
| Sedmica 9 / Sprint 9 | PB-58 | Kalendar događaja | Završeno | Dodan kalendar i pregled događaja. |
| Sedmica 9 / Sprint 9 | PB-59 | Forum zajednice za članove | Završeno | Implementirane teme, komentari i reakcije. |
| Sedmica 10 / Sprint 10 | PB-41 | Slanje email upozorenja | Završeno | Podsjetnici za rok vraćanja i kašnjenje su implementirani. |
| Sedmica 10 / Sprint 10 | PB-42 | Notifikacija bibliotekara o novoj rezervaciji | Završeno | Bibliotekar prima email obavijest o rezervaciji. |
| Sedmica 10 / Sprint 10 | PB-46 | Audit log promjena | Završeno | Administratorski pregled promjena nad sistemom. |
| Sedmica 10 / Sprint 10 | PB-47 | Kazne za kasno vraćanje | Djelimično završeno | Uvedena zabrana novih akcija za kašnjenja, ali ne i puni obračun novčane kazne. |
| Sedmica 10 / Sprint 10 | PB-48 | Online produženje članarine | Djelimično završeno | Zahtjev i obrada postoje, ali nema prave online naplate. |
| Sedmica 10 / Sprint 10 | PB-49 | Integracija sa distributerom knjiga | Djelimično završeno | Realizovana kao slanje email zahtjeva, bez vanjskog API povezivanja. |
| Sedmica 10 / Sprint 10 | PB-50 (oznaka iz Sprint 10 review-a) | Lista kolekcija člana | Završeno | Član može praviti i uređivati vlastite kolekcije; u dokumentaciji postoji preklapanje oznake PB-50 sa kasnijom backlog stavkom za stabilizaciju sistema. |
| Sedmica 11 / Sprint 11 | PB-50 | Stabilizacija sistema | Nije završeno | Stavka je ostala otvorena u backlogu. |
| Sedmica 11 / Sprint 11 | PB-51 | Lista poznatih ograničenja i tehničkog duga | Nije završeno | Potrebna formalna dokumentacija ograničenja i duga. |
| Sedmica 11 / Sprint 11 | PB-52 | Release Notes | Nije završeno | Nisu formalno pripremljeni release notes kroz verzije. |
| Sedmica 11 / Sprint 11 | PB-53 | Korisnička dokumentacija | Nije završeno | Potrebna dokumentacija po korisničkim ulogama. |
| Sedmica 11 / Sprint 11 | PB-54 | Tehnička dokumentacija | Djelimično završeno | Postoje deployment i CD dokumenti, ali ne kompletan skup planirane tehničke dokumentacije. |
| Sedmica 11 / Sprint 11 | PB-55 | Završna demonstracija | Nije završeno | Ova stavka pripada finalnoj fazi projekta. |

### 7.2 Stavke koje su djelimično završene

Sljedeće stavke postoje u sistemu, ali nisu zatvorene u punom produkcijskom obimu:

| Stavka | Trenutno stanje | Šta nedostaje |
|---|---|---|
| PB-47 Kazne za kasno vraćanje | Sistem blokira nova zaduživanja i rezervacije za članove sa kašnjenjem. | Nema pune evidencije novčanih kazni, iznosa, naplate i istorije plaćanja. |
| PB-48 Online produženje članarine | Član može poslati zahtjev, a bibliotekar ga može odobriti ili odbiti. | Nema integracije sa stvarnim platnim servisom i automatskom naplatom. |
| PB-49 Integracija sa distributerom knjiga | Bibliotekar šalje zahtjev distributeru iz sistema putem emaila. | Nema dvosmjerne API integracije, praćenja statusa narudžbe ni automatizovanog povrata odgovora. |
| API sloj | `SmartLib.API` projekat postoji i koristi JWT autentifikaciju. | API nije proširen na sve poslovne module i nije glavni način korištenja sistema. |
| Moderacija sadržaja | Postoje prijave recenzija i forum sadržaja te osnovne admin/bibliotekar akcije. | Nedostaju naprednija pravila moderacije, automatika i detaljniji moderacijski workflow. |
| PB-54 Tehnička dokumentacija | Dio tehničke dokumentacije postoji kroz arhitekturu, deployment i CD dokumente. | Nije još zatvoren cijeli skup dokumentacije planiran backlogom kao finalni deliverable. |

### 7.3 Stavke koje nisu završene

Pored formalno otvorenih backlog stavki, ostali su i određeni tehnički i operativni aspekti koji bi bili prioritet u nastavku projekta:

| Oblast | Status | Napomena |
|---|---|---|
| Standardizovane EF Core migracije | Nije završeno | Sistem se oslanja na `EnsureCreated()` i ručne SQL izmjene u `Program.cs`. |
| Produkcijska integracija sa servisom naplate | Nije završeno | Nije implementirano stvarno plaćanje članarine. |
| Napredniji backup i disaster recovery | Nije završeno | Nije dokumentovan pun oporavak sistema u slučaju kvara. |
| Horizontalno skaliranje | Nije završeno | Trenutni deployment je predviđen za jednostavnije, monolitno hostovanje. |
| Potpuna automatizacija infrastrukture | Nije završeno | Deployment pipeline postoji, ali ne pokriva kompletno infrastrukturno postavljanje svih servisa. |

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

Tim je kroz deset sprintova uspio izgraditi funkcionalan i širok bibliotečki informacioni sistem koji pokriva osnovne operativne procese biblioteke, ali i više dodatnih modula koji unapređuju korisničko iskustvo. Posebno je značajno što projekat nije ostao samo na CRUD funkcionalnostima, nego je proširen rezervacijama, email podsjetnicima, audit logom, izvještajima, forumom, recenzijama, vijestima, kalendarom, listom želja i kolekcijama.

Najveće snage projekta su obim implementiranih funkcionalnosti, jasan razvoj kroz sprintove, postojanje testova na više nivoa i uspostavljen deployment pipeline. Najveći prostor za napredak nalazi se u tehničkoj stabilizaciji baze, dovršavanju pojedinih poslovnih tokova do produkcijskog nivoa i dodatnom smanjenju tehničkog duga.

U cjelini, SmartLib predstavlja uspješno realizovan studentski projekat koji je izrastao iz osnovnog bibliotečkog sistema u znatno bogatiju platformu za rad biblioteke i interakciju sa njenim članovima.

---

*Dokument pripremila: SI Grupa 5*  
*Datum: Juni 2026.*
