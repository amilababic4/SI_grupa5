# Initial Release Plan  
**Projekat:** Bibliotečki informacioni sistem  
**Sprint:** 4  

---

## 1. Svrha dokumenta

Ovaj dokument definiše **početni plan release-ova** za projekat *Bibliotečki informacioni sistem* na osnovu aktuelnog **Product Backloga**, razrađenih **User Storyja** i dogovorenog ritma rada tima. Cilj dokumenta je da pokaže:

- kako će se funkcionalnosti grupisati u **smislene release cjeline**
- zašto su pojedine funkcionalnosti planirane baš u određenim periodima
- kako release plan prati principe inkrementalnog razvoja i Scrum logiku
- kada sistem dostiže nivo dovoljne vrijednosti da može biti posmatran kao **release kandidat**

Ovaj artefakt nastaje u **Sprintu 4**, zajedno sa Definition of Done, tehničkim skeletonom sistema i osnovnim setupom repozitorija. Sprint 4 je predviđen upravo za postavljanje temelja projekta vlastitim inžinjerskim promišljanjem, prije jačeg ulaska u implementaciju. 

---

## 2. Polazne osnove za planiranje release-ova

Release plan je napravljen na osnovu tri ključna ulaza:

1. **Raspored funkcionalnosti po sprintovima u Product Backlogu**, gdje su već naznačene glavne stavke po sprintovima, uključujući osnovne feature-e kao što su prijava, katalog, zaduživanje, rezervacije i notifikacije. 
2. **Detaljno razrađeni User Storyji**, koji preciznije pokazuju šta je stvarna funkcionalna vrijednost svakog backlog itema i koje zavisnosti između modula postoje. 
3. **Dogovor tima da se release pravi nakon svakog drugog sprinta**, odnosno nakon Sprinta **6, 8 i 10**.

Takva odluka je logična jer se sprintovi 6-10 po uputama PO-a posmatraju kao period **kontinuiranog razvoja inkremenata, testiranja, refaktorisanja, proširenja funkcionalnosti i upravljanja tehničkim dugom**, a od tima se očekuje da u svakom sprintu pokaže realizovani inkrement ili značajan napredak. Release nakon svaka dva sprinta daje dovoljno vremena da se funkcionalnosti ne isporučuju previše sitno i fragmentirano, nego kao **zaokružene korisničke cjeline**. 

---

## 3. Principi koji su korišteni pri izradi release plana

Pri planiranju release cjelina tim se vodio sljedećim principima:

### 3.1. Release mora imati poslovni smisao

PO je naglasio da release ne znači samo “nešto završeno”, nego inkrement koji donosi **dovoljno vrijednosti korisniku** da bi ga Product Owner mogao smatrati kandidatom za isporuku. Zato release nije planiran nakon svakog sprinta, nego nakon svakog drugog sprinta, kada se može pokazati veća i korisnija funkcionalna cjelina.

### 3.2. Release mora pratiti zavisnosti između funkcionalnosti

Neke funkcionalnosti ne mogu imati puni smisao bez prethodno implementiranih modula. Na primjer:

- pregled kataloga zavisi od toga da knjige postoje u sistemu
- pregled dostupnosti zavisi od upravljanja primjercima
- zaduživanje zavisi od članova, knjiga, primjeraka i statusa članarine
- rezervacije zavise od zaduživanja i dostupnosti knjiga

Te veze su jasno vidljive i u backlogu i u user storyjima. 

### 3.3. Release treba da pokaže progresiju sistema

Planirani release-ovi su organizovani tako da sistem raste kroz tri jasne faze:

- od **osnovnog digitalnog sistema i kataloga**
- preko **operativnog bibliotečkog rada**
- do **naprednijih korisničkih i administrativnih funkcionalnosti**

### 3.4. Release cadence mora biti realan

Dogovor o release-u nakon svakog drugog sprinta omogućava timu da:

- u prvom sprintu unutar release ciklusa implementira jezgro funkcionalnosti
- u drugom sprintu doradi, integriše, testira i zaokruži funkcionalnosti
- na kraju drugog sprinta isporuči smisleniji i stabilniji inkrement

To je posebno važno jer PO za sprintove 6–10 traži ne samo razvoj, nego i **testiranje, refaktorisanje, upravljanje promjenama i tehničkim dugom**. Zato release na svaka dva sprinta bolje odgovara stvarnom tempu ozbiljnog timskog rada nego release nakon svakog sprinta. 

---

## 4. Strategija release planiranja

Tim je usvojio strategiju da se **release pravi nakon svakog drugog sprinta**, odnosno:

- **Release 1 nakon Sprinta 6**
- **Release 2 nakon Sprinta 8**
- **Release 3 nakon Sprinta 10**

Ova odluka je obrazložena na sljedeći način:

- **Sprint 5** je prvi implementacioni sprint AI-enabled faze i služi za validaciju toka rada, uspostavu AI Usage Loga i Decision Loga, te izradu prvog funkcionalnog inkrementa. Sam po sebi je više početni produkcioni iskorak nego trenutak za veći release. 
- **Sprint 6** omogućava da se zajedno zaokruže funkcionalnosti iz Sprinta 5 i 6 i pretvore u prvi stvarno upotrebljiv release.  
- **Sprintovi 7 i 8** zajedno omogućavaju da se osnovni katalog proširi u pravi operativni sistem biblioteke sa zaduživanjem, članarinama i administracijom članova.  
- **Sprintovi 9 i 10** zajedno donose naprednije funkcionalnosti poput rezervacija, korisničkih pregleda zaduženja, email obavijesti i dodatnih proširenja sistema.  
- **Sprint 11 i 12** nisu planirani kao novi poslovni release-ovi, nego kao period stabilizacije, testiranja, dokumentacije i pripreme finalne verzije za demonstraciju. 

Drugim riječima, release cadence je namjerno podešen tako da se **ne isporučuju polovične funkcionalnosti**, nego inkrementi koji imaju dovoljno širine, koherencije i korisničke vrijednosti.

---

## 5. Planirani inkrementi / release cjeline

## Release 1 — Osnovni rad sistema i katalog fonda

### Naziv inkrementa
**Release 1 – Autentifikacija, članovi i osnovni katalog**

### Cilj inkrementa
Cilj prvog release-a je da sistem dobije **prvu stvarno upotrebljivu verziju**. Nakon ovog release-a biblioteka treba imati mogućnost da:

- prijavi korisnike u sistem
- kreira naloge novih članova
- evidentira knjige u fond
- prikaže osnovni katalog knjiga

Ovo je prvi trenutak kada sistem prestaje biti samo tehnički prototip i postaje **funkcionalna osnova bibliotečkog informacionog sistema**.

### Glavne funkcionalnosti

U ovaj release ulaze backlog stavke iz Sprinta 5 i Sprinta 6 koje čine osnovu sistema:

- **PB-17 Sistem prijave korisnika**  
  Uključuje prijavu, odjavu, sesije, zaštitu ruta i zabranu pristupa deaktiviranim korisnicima. 

- **PB-18 Kreiranje naloga člana**  
  Omogućava bibliotekaru unos člana, validaciju podataka i uspješno kreiranje naloga. 

- **PB-19 AI Usage Log i Decision Log**  
  Iako nije krajnja korisnička funkcionalnost, važan je dio početka AI-enabled faze i transparentnog rada tima. 

- **PB-22 Dodavanje nove knjige**  
  Unos osnovnih podataka o knjizi, validacija ISBN-a, izbor kategorije i broj primjeraka. 

- **PB-23 Uređivanje podataka o knjizi**  
  Izmjena osnovnih podataka o knjizi radi tačnosti fonda. 

- **PB-28 Pregled kataloga knjiga**  
  Prikaz svih knjiga u sistemu i osnovna paginacija. 

### Zašto su ove funkcionalnosti grupisane u Release 1

Ove funkcionalnosti su stavljene zajedno zato što čine **minimalnu upotrebljivu jezgru sistema**.

Bez prijave korisnika sistem nema kontrolu pristupa.  
Bez kreiranja članova nema stvarnih korisnika sistema.  
Bez dodavanja knjiga i kataloga nema fonda nad kojim sistem radi.

Zato bi release nakon Sprinta 5 bio prerano napravljen: postojao bi login i registracija članova, ali bez dovoljno razvijenog rada sa knjigama sistem još ne bi donosio dovoljnu poslovnu vrijednost. Tek nakon Sprinta 6 ove funkcionalnosti zajedno čine prvu smislenu release cjelinu. To je u skladu i sa backlogom, gdje su PB-17, PB-18, PB-22 i PB-28 označeni kao visoko prioritetni i rani feature-i sistema. 

### Zavisnosti

- **PB-13 Dizajn i implementacija baze podataka**
- **PB-14 Početna struktura projekta**
- **PB-15 Definition of Done**
- **PB-16 Initial Release Plan**

Sve navedeno je planirano u Sprintu 4 kao osnova za dalju implementaciju.

Dodatno:

- PB-22 zavisi od entiteta knjige i osnovne baze podataka
- PB-28 zavisi od toga da knjige već postoje u sistemu
- PB-18 i PB-17 zavise od modela korisnika i uloga. 

### Glavni rizici

- problemi sa autentifikacijom i sesijama mogu blokirati pristup ostatku sistema
- greške u modelu korisnika i uloga mogu otežati kasnije feature-e
- loša validacija ISBN-a i unosa knjiga može unijeti nekonzistentne podatke u katalog
- nesinhronizovanost između modula za dodavanje knjige i prikaza kataloga može umanjiti vrijednost prvog release-a

### Okvirni sprintovi u kojima se očekuje realizacija
**Sprint 5 – Sprint 6**

### Planirani trenutak release-a
**Nakon Sprinta 6**

---

## Release 2 — Operativni bibliotečki rad

### Naziv inkrementa
**Release 2 – Upravljanje fondom, članovima i zaduživanjem**

### Cilj inkrementa
Cilj drugog release-a je da sistem preraste iz osnovnog kataloga u **operativni bibliotečki alat**. Nakon ovog release-a bibliotekar treba moći raditi sa stvarnim fondom i članovima kroz:

- upravljanje kategorijama i primjercima
- pregled detalja knjige i dostupnosti
- vođenje zaduživanja i vraćanja
- upravljanje članarinama
- administraciju korisnika i pregled profila

Ovo je najvažniji release iz poslovne perspektive, jer pokriva **glavni radni tok biblioteke**.

### Glavne funkcionalnosti

U ovaj release ulaze prioritetne funkcionalnosti iz Sprinta 7 i 8:

- **PB-25 Upravljanje kategorijama knjiga**  
  Dodavanje, uređivanje i brisanje kategorija. 

- **PB-26 Upravljanje primjercima knjige**  
  Evidencija više primjeraka i njihovih statusa. 

- **PB-27 Brisanje knjige i deaktivacija primjerka**  
  Kontrolisano uklanjanje knjiga i primjeraka uz poslovna ograničenja.

- **PB-29 Pretraga knjiga**  
  Pretraga po naslovu i autoru. 

- **PB-24 Prikaz detalja knjige**  
  Detaljni prikaz osnovnih bibliografskih podataka. 

- **PB-30 Pregled dostupnosti knjige**  
  Prikaz statusa dostupnosti i broja slobodnih primjeraka. 

- **PB-31 Evidencija zaduživanja i vraćanja knjiga**  
  Centralni proces biblioteke: odabir člana, odabir primjerka, rok vraćanja, evidentiranje povrata i zaštita od duplog zaduženja. 

- **PB-20 Pregled profila člana**  
  Osnovni podaci i zaduženja člana. 

- **PB-32 Upravljanje korisnicima od strane admina**  
  Pregled korisnika, pretraga, promjena uloge, deaktivacija. 

- **PB-33 Upravljanje statusom članarine**  
  Evidentiranje i ažuriranje članarina.

- **PB-34 Pregled statusa članarine za člana**  
  Član vidi status i datum isteka članarine. 

- **PB-37 Pregled historije zaduženja**  
  Niži prioritet, ali logična nadogradnja zaduživanja. 

- **PB-38 Početno testiranje sistema**  
  Testiranje prve integrisane verzije sistema. 

### Zašto su ove funkcionalnosti grupisane u Release 2

Ove funkcionalnosti su grupisane zajedno zato što Release 2 treba da bude trenutak kada sistem počinje podržavati **stvarni svakodnevni rad biblioteke**.

Sprint 7 sam po sebi donosi važna poboljšanja kataloga, ali bez Sprinta 8 još uvijek ne postoji glavni poslovni proces sistema: **zaduživanje i vraćanje knjiga**. Tek kada se uz katalog, detalje knjige, primjerke i dostupnost dodaju:

- zaduživanje
- članarine
- profili korisnika
- administracija korisnika

sistem postaje prava operativna aplikacija, a ne samo katalog.

Zato release nakon Sprinta 8 ima mnogo više smisla nego release nakon Sprinta 7. Ovaj release okuplja funkcionalnosti koje su međusobno snažno povezane i zajedno čine najveći poslovni skok u projektu. Backlog to potvrđuje, jer su PB-26, PB-30 i PB-31 označeni kao visoko prioritetni i direktno povezani sa osnovnim poslovanjem biblioteke.

### Zavisnosti

- Zavisi od Release-a 1, jer bez korisnika, knjiga i kataloga nema rada sa fondom
- PB-30 zavisi od PB-24 i PB-26. 
- PB-31 zavisi od upravljanja primjercima i dostupnošću knjiga. 
- PB-33 i PB-34 zavise od postojanja članova i profila korisnika. 
- PB-32 zavisi od već implementiranog sistema prijave i korisničkih naloga. 

### Glavni rizici

- pogrešna poslovna logika kod zaduživanja može narušiti tačnost evidencije
- statusi primjeraka mogu postati nekonzistentni ako nisu dobro povezani sa vraćanjem i brisanjem
- loše upravljanje članarinama može blokirati validna zaduživanja ili dopustiti nevalidna
- administracija korisnika nosi sigurnosni rizik ako uloge i deaktivacije nisu ispravno implementirane
- integracija većeg broja modula u ovoj fazi povećava potrebu za ozbiljnijim testiranjem

### Okvirni sprintovi u kojima se očekuje realizacija
**Sprint 7 – Sprint 8**

### Planirani trenutak release-a
**Nakon Sprinta 8**

---

## Release 3 — Napredne korisničke i administrativne funkcionalnosti

### Naziv inkrementa
**Release 3 – Rezervacije, notifikacije i napredna podrška korisnicima**

### Cilj inkrementa
Cilj trećeg release-a je da se sistem nadogradi funkcionalnostima koje poboljšavaju korisničko iskustvo, unapređuju organizaciju rada bibliotekara i uvode veću automatizaciju procesa. Nakon ovog release-a sistem treba podržavati:

- pregled vlastitih zaduženja za člana
- pregled aktivnih zaduženja za bibliotekara
- rezervacije knjiga
- pregled rezervacija
- email podsjetnike i upozorenja
- dodatne napredne i administrativne funkcije

### Glavne funkcionalnosti

U ovaj release ulaze funkcionalnosti iz Sprinta 9 i 10:

- **PB-35 Pregled vlastitih zaduženja**  
  Član vidi aktivna zaduženja, rok vraćanja i zakašnjela zaduženja. 

- **PB-36 Pregled trenutnih zaduženja**  
  Bibliotekar vidi sva aktivna zaduženja, detalje i sortiranje po rokovima. 

- **PB-39 Rezervacija knjiga**  
  Rezervacija nedostupnih knjiga, pregled i otkazivanje rezervacija. 

- **PB-40 Pregled aktivnih rezervacija**  
  Bibliotekar vidi sve aktivne rezervacije. 

- **PB-44 Napredna pretraga i filteri**  
  Filteri po kategoriji, izdavaču i godini. 

- **PB-43 Automatsko otkazivanje rezervacije**  
  Sistemski rok važenja rezervacije i otkazivanje po isteku.

- **PB-41 Slanje email upozorenja**  
  Podsjetnici prije isteka, na dan isteka i za kašnjenje. 

- **PB-42 Obavještavanje bibliotekara o novoj rezervaciji**  
  Email obavijest bibliotekaru o rezervaciji.

- **PB-45 Mjesečni izvještaji za upravu**  
  Izvještaji o zaduženjima, rezervacijama i članovima. 

- **PB-46 Audit log promjena**  
  Praćenje promjena nad knjigama i korisnicima. 

- **PB-47 Kazne za kasno vraćanje knjiga**  
  Obračun kazni i pregled duga. 

- **PB-48 Online produžetak članarine**  
  Mogućnost online produženja članarine. 

- **PB-49 Integracija sa distributerom knjiga**  
  Slanje zahtjeva za nabavku knjiga distributeru. 

### Zašto su ove funkcionalnosti grupisane u Release 3

Ovaj release je planiran kao **nadogradnja na stabilan operativni sistem** iz Release-a 2.

Nema smisla uvoditi rezervacije prije nego što postoje:

- status dostupnosti knjige
- evidencija zaduženja
- pregled aktivnih zaduženja
- pouzdani podaci o članovima i fondu

Isto tako, email notifikacije, kazne i online produženje članarine nemaju puni smisao bez već implementiranih osnovnih procesa. Zato je ova grupa funkcionalnosti stavljena tek nakon Sprinta 10.

Release 3 je zamišljen kao inkrement koji ne gradi jezgro sistema, nego ga **širi i čini zrelijim, korisnički bogatijim i operativno efikasnijim**. Backlog ove funkcionalnosti i označava kao kasnije nadogradnje, većinom srednjeg ili nižeg prioriteta, što dodatno potvrđuje ispravnost njihovog smještanja u treći release.

### Zavisnosti

- PB-35 i PB-36 zavise od PB-31 Evidencije zaduživanja i vraćanja.  
- PB-39 i PB-40 zavise od PB-30 Pregleda dostupnosti i rada sa zaduženjima.
- PB-41, PB-42 i PB-43 zavise od rezervacija, email mehanizma i aktivnih zaduženja. 
- PB-47 zavisi od pravilnog evidentiranja rokova vraćanja. 
- PB-48 zavisi od postojećeg modula članarina. 

### Glavni rizici

- složenija pravila rezervacija i isteka rezervacija mogu dovesti do bugova u poslovnoj logici
- email funkcionalnosti nose tehnički rizik zbog eksternih servisa i scheduler logike
- kazne i notifikacije mogu izazvati probleme ako datumi i statusi zaduženja nisu pouzdani
- previsok scope Sprintova 9 i 10 može povećati tehnički dug ako se ne kontroliše prioritet implementacije
- integracija više naprednih funkcionalnosti u isto vrijeme može tražiti dodatno refaktorisanje

### Okvirni sprintovi u kojima se očekuje realizacija
**Sprint 9 – Sprint 10**

### Planirani trenutak release-a
**Nakon Sprinta 10**

---

## 6. Zašto nema novog release-a u Sprintu 11 i 12

Sprint 11 i Sprint 12 nisu planirani kao novi poslovni release-ovi, nego kao period za:

- **stabilizaciju sistema**
- **testiranje i bug fixing**
- **zatvaranje funkcionalnih rupa**
- **izlistavanje tehničkog duga i ograničenja**
- **izdavanje release notes-a**
- **izradu korisničke i tehničke dokumentacije**
- **pripremu završne demonstracije**. 

To znači da se nakon Release-a 3 sistem ne širi primarno novim korisničkim cjelinama, nego se:

- stabilizuje
- dokumentuje
- priprema za finalnu verziju i demonstraciju

Zato Sprint 11 i 12 treba posmatrati kao **finalization / hardening fazu**, a ne kao zasebne release cikluse.

---

## 7. Sažetak release plana

| Release | Naziv | Sprintovi | Glavni fokus | Planirani trenutak release-a |
|---|---|---:|---|---|
| Release 1 | Autentifikacija, članovi i osnovni katalog | 5–6 | login, članovi, knjige, katalog | nakon Sprinta 6 |
| Release 2 | Upravljanje fondom, članovima i zaduživanjem | 7–8 | primjerci, dostupnost, zaduživanje, članarine, administracija | nakon Sprinta 8 |
| Release 3 | Rezervacije, notifikacije i napredna podrška korisnicima | 9–10 | rezervacije, pregledi zaduženja, emailovi, napredne funkcije | nakon Sprinta 10 |

---

## 8. Zaključak

Predloženi Initial Release Plan prati:

- prioritete iz Product Backloga
- raspored User Storyja po sprintovima
- zavisnosti između modula
- Scrum ideju da svaki sprint treba dati potentially shippable inkrement
- i timski dogovor da se **release radi nakon svakog drugog sprinta**

Takav pristup je dobar jer balansira dvije stvari:

- s jedne strane omogućava **kontinuitet isporuke**
- a s druge strane sprečava da release bude previše sitan, nedovršen ili bez dovoljne korisničke vrijednosti

Zbog toga su sprintovi 6, 8 i 10 odabrani kao prirodne tačke release-a:

- nakon Sprinta 6 sistem prvi put postaje upotrebljiv
- nakon Sprinta 8 sistem podržava glavni operativni proces biblioteke
- nakon Sprinta 10 sistem dobija napredne funkcionalnosti i veću zrelost

Ovaj release plan daje timu jasan okvir za razvoj, Product Owneru jasan pregled očekivanih inkremenata, a projektu realističan i smislen tempo isporuke.

---

## 9. Kriterij za odluku da je release spreman

Release se smatra spremnim kada:

- sve planirane user story stavke za tu release cjelinu ispunjavaju acceptance criteria
- postoji dokaz testiranja relevantnih funkcionalnosti
- backlog, Decision Log i AI Usage Log su ažurirani
- inkrement je dovoljno stabilan da ga tim može demonstrirati kao smislenu funkcionalnu cjelinu

To je dobro povezano sa očekivanim deliverable-ima po sprintovima 6-10, gdje PO eksplicitno traži dokaz o testiranju, ažurirane logove i review summary. 
