# Architecture Overview

## 1. Kratak opis arhitektonskog pristupa

SmartLib je projektovan kao **web aplikacija** zasnovana na **klijent-server arhitekturi** sa **modularnim monolitnim backendom**. Frontend i backend su odvojene aplikacije koje komuniciraju putem REST API-ja, dok backend interno koristi modularnu organizaciju po funkcionalnim cjelinama.

Arhitektura se sastoji od tri fundamentalna nivoa:

- **Prezentacioni sloj (Web klijent)** je zasebna web aplikacija koja služi kao jedini interfejs za interakciju korisnika sa sistemom. Klijent komunicira isključivo sa backend API-jem putem HTTP(S) poziva i nikada direktno ne pristupa bazi podataka. Odgovoran je za renderovanje korisničkog interfejsa, klijentsku validaciju formi i prilagođavanje prikaza prema ulozi prijavljenog korisnika (Član, Bibliotekar, Administrator).

- **Aplikacioni sloj (API server, modularni monolit)** je centralna backend aplikacija koja implementira kompletnu poslovnu logiku sistema. Organizovana je po funkcionalnim modulima (Auth, Korisnici, Katalog, Inventar, Zaduženja, Članarina, Rezervacije, Audit/Log), pri čemu svaki modul ima jasno definisanu odgovornost i interni interfejs.

- **Sloj podataka (Relacijska baza podataka)** je centralizovana trajna pohrana svih podataka sistema. Baza provodi integritetna ograničenja, referencijalne veze i jedinstvene vrijednosti, čime se osigurava konzistentnost podataka nezavisno od aplikacione logike.

Važno je naglasiti da je ovo **web aplikacija sa razdvojenim frontend i backend dijelom**. Pojam „monolit" odnosi se isključivo na backend: umjesto da se backend razbija na mnogo zasebnih servisa, sva poslovna logika živi unutar jedne backend aplikacije koja je interno organizovana po modulima. Frontend je zasebna web aplikacija i ne zavisi od toga kako je backend interno strukturiran.

### Akteri sistema

U sistemu postoje tri korisničke uloge i jedan sistemski akter:

- **Član biblioteke** koristi sistem za pregled, rezervaciju i posudbu knjiga
- **Bibliotekar** upravlja knjigama, članovima i zaduženjima
- **Administrator** upravlja korisničkim nalozima, ulogama i sistemskim funkcijama
- **Sistem** (automatski akter) izvršava procese koji se ne pokreću ručno od strane korisnika, već periodično ili na osnovu događaja (npr. automatsko otkazivanje isteklih rezervacija, generisanje audit log zapisa)

### Obrazloženje odabranog pristupa

Modularni monolit je izabran umjesto mikroservisne arhitekture iz sljedećih konkretnih razloga:

1. **Veličina tima i vremenski okvir.** Projekat razvija tim od osam članova uz raspoloživi razvojni period od približno sedam sedmica. Mikroservisna arhitektura bi zahtijevala dodatno vrijeme za postavljanje zasebnih servisa, njihovu orkestraciju i deployment, što u ovom kontekstu nije opravdano.

2. **Domen sistema.** Bibliotečki informacioni sistem je dobro razumljiv domen sa jasno definisanim entitetima i relacijama. Poslovna pravila (zaduživanje, vraćanje, članarine, rezervacije) su inherentno transakcijska i zahtijevaju atomske operacije nad više entiteta unutar iste baze, što monolit prirodno podržava.

3. **Operativna jednostavnost.** Jedan deployment artefakt, jedan proces i jedan izvor logova značajno smanjuju kompleksnost rada u razvoju, testiranju i produkciji. Za biblioteku koja pretpostavlja mali broj istovremenih korisnika, ovo je optimalan pristup.

4. **Evolutivnost.** Striktna modularna organizacija koda (jasni interni interfejsi između modula) ostavlja mogućnost kasnijeg izdvajanja pojedinih modula u zasebne servise ako obim sistema to opravda, bez potrebe za potpunim redizajnom.

---

## 2. Glavne komponente sistema

Sistem je organizovan u tri primarne infrastrukturne komponente i osam funkcionalnih modula unutar backend aplikacije, uz planirane pomoćne podsisteme za buduće sprintove.

### 2.1 Infrastrukturne komponente

| Komponenta | Opis |
|---|---|
| **Web klijent (Frontend)** | Zasebna web aplikacija za sve korisničke uloge. Pruža responsivni interfejs prilagođen za desktop i mobilne uređaje. Komunicira sa backendom putem REST API poziva. |
| **API server (Backend monolit)** | Centralna backend aplikacija sa modularnom internom organizacijom. Jedina ulazna tačka za sve poslovne operacije. Izlaže REST API koji koristi frontend. |
| **Relacijska baza podataka (PostgreSQL)** | Trajna pohrana svih podataka sistema. Provodi integritetna ograničenja i referencijalne veze. |

### 2.2 Funkcionalni moduli backend aplikacije

| Modul | Kratki opis |
|---|---|
| **Auth modul** | Autentifikacija, upravljanje JWT tokenima, hashiranje lozinki |
| **Korisnici modul** | Registracija članova, upravljanje nalozima, pregled i pretraga korisnika |
| **Katalog modul** | Upravljanje knjigama i kategorijama, pretraga i filtriranje |
| **Inventar modul** | Upravljanje fizičkim primjercima knjiga i njihovim statusima |
| **Zaduženja modul** | Evidencija zaduživanja i vraćanja primjeraka |
| **Članarina modul** | Upravljanje statusom i trajanjem članstva korisnika |
| **Rezervacije modul** | Evidencija i upravljanje rezervacijama nedostupnih knjiga |
| **Audit/Log modul** | Evidentiranje kritičnih akcija u sistemu (vidljivo administratorima) |

### 2.3 Planirani pomoćni podsistemi (kasniji sprintovi)

- **Notifikacioni podsistem** za slanje email podsjetnika o roku vraćanja (2 dana prije isteka), upozorenja na dan isteka roka, te obavijesti o dostupnosti rezervisanih knjiga i novim rezervacijama za bibliotekare
- **Periodični zadaci** za automatsko otkazivanje rezervacija po isteku definisanog roka preuzimanja
- **Izvještavanje** za generisanje mjesečnih izvještaja o zaduživanjima, rezervacijama i članstvu za upravu biblioteke u PDF formatu
- **Kazne/Naknade** za evidenciju i obračun kazni za prekoračenje roka vraćanja
- **Online produžetak članarine** za mogućnost produženja članarine putem sistema sa simulacijom sistema naplate

---

## 3. Odgovornosti komponenti

### 3.1 Web klijent (Frontend)

Web klijent je prezentaciona komponenta koja služi kao jedini interfejs između korisnika i sistema. Njegova odgovornost je strogo ograničena na prikaz podataka i prikupljanje korisničkog unosa, dok se sva poslovna logika izvršava na backendu.

**Odgovornosti prema ulogama:**

**Član biblioteke:**
- Prijava i odjava iz sistema
- Pregled kataloga knjiga sa paginacijom
- Pretraga knjiga po naslovu i autoru
- Prikaz detalja knjige sa statusom dostupnosti i brojem slobodnih primjeraka
- Pregled vlastitog profila sa osnovnim podacima i zaduženjima
- Pregled vlastitih aktivnih zaduženja sa rokovima vraćanja i vizuelnim označavanjem zakašnjelih
- Pregled statusa i datuma isteka vlastite članarine
- Kreiranje i otkazivanje rezervacija za nedostupne knjige
- Napredna pretraga sa filterima po kategoriji, izdavaču i godini izdanja

**Bibliotekar:**
- Sve pristupne mogućnosti člana, plus:
- Kreiranje naloga novih članova biblioteke
- Dodavanje, uređivanje i brisanje knjiga u katalogu
- Upravljanje kategorijama knjiga: dodavanje, uređivanje, brisanje
- Upravljanje primjercima knjiga: dodavanje, pregled statusa
- Evidencija zaduživanja i vraćanja knjiga
- Pregled svih aktivnih zaduženja sa sortiranjem i filtriranjem po članu
- Pregled historije zaduženja članova
- Upravljanje članarinom članova: evidentiranje i ažuriranje
- Pregled aktivnih rezervacija u sistemu
- Pregled članova biblioteke i njihovih profila
- Deaktivacija naloga članova (isključivanje člana iz biblioteke)

**Administrator:**
- Sve pristupne mogućnosti bibliotekara, plus:
- Pregled i pretraga svih korisnika sistema (uključujući bibliotekare)
- Promjena uloga korisnika
- Deaktivacija naloga svih korisnika (uključujući bibliotekare) uz zaštitu od deaktivacije vlastitog naloga
- Generisanje mjesečnih izvještaja (planirano)
- Pregled audit log zapisa

**Razlika između deaktivacije kod bibliotekara i administratora:** Bibliotekar može deaktivirati samo naloge članova (npr. kada član želi napustiti biblioteku ili se isčlaniti). Administrator može deaktivirati naloge svih korisnika, uključujući i bibliotekare, te ima dodatnu mogućnost promjene uloga.

**Jako važno!** Frontend prilagođava korisnički interfejs prema ulozi prijavljenog korisnika (skrivanje/prikazivanje navigacijskih stavki i formi), ali ovo nikada ne smije biti jedini mehanizam kontrole pristupa. Svaka restrikcija mora biti provjerena i na backendu.

### 3.2 API server (Backend monolit)

Backend je centralna tačka sistema i jedini izvor istine za sva poslovna pravila. Organizovan je po modulima koji međusobno sarađuju unutar istog procesa kroz jasno definirane interne interfejse.

#### Auth modul

Odgovoran za kompletnu autentifikaciju korisnika i upravljanje pristupom sistemu.

- Prijava korisnika putem email-a i lozinke sa verifikacijom hash vrijednosti
- Izdavanje JWT tokena pri uspješnoj prijavi sa kratkotrajnim rokom važenja
- Invalidacija tokena pri odjavi
- Blokiranje pristupa zaštićenim rutama bez validnog tokena
- Odbijanje prijave deaktiviranim korisnicima
- Pohranjivanje lozinki isključivo u hashiranom obliku, bez čuvanja ili vraćanja u čitljivom formatu
- Generička poruka pri neuspješnoj prijavi, jer sistem nikada ne otkriva koji od unesenih podataka (email ili lozinka) je neispravan

**Povezane uloge:** Sve uloge (Član, Bibliotekar, Administrator).

#### Korisnici modul

Odgovoran za upravljanje korisničkim nalozima i profilima.

- Kreiranje naloga novih članova od strane bibliotekara ili administratora sa validacijom obaveznih polja i jedinstvenosti email adrese
- Automatska dodjela uloge „Član" novim korisnicima
- Pregled članova biblioteke za bibliotekare, pregled svih korisnika sistema za administratore
- Promjena korisničke uloge od strane administratora
- Deaktivacija naloga članova od strane bibliotekara ili administratora
- Deaktivacija naloga bilo kojeg korisnika (uključujući bibliotekare) od strane administratora, sa zabranom deaktivacije vlastitog naloga
- Prikaz korisničkog profila sa osnovnim podacima i trenutnim zaduženjima

**Povezane uloge:** Bibliotekar (kreiranje članova, pregled članova, deaktivacija članova), Administrator (upravljanje svim nalozima i ulogama), Član (pregled vlastitog profila).

#### Katalog modul

Odgovoran za upravljanje bibliografskim zapisima i organizacijom bibliotečkog fonda.

- Dodavanje novih knjiga sa validacijom obaveznih podataka (naslov, autor, ISBN, godina izdanja, kategorija) i automatskim kreiranjem početnog broja primjeraka
- Validacija jedinstvenosti ISBN-a pri dodavanju i uređivanju
- Uređivanje podataka o knjizi sa promjenama odmah vidljivim svim korisnicima
- Brisanje knjiga uz provjeru da ne postoje aktivna zaduženja, ako postoje, brisanje se odbija
- Pregled kataloga sa paginacijom i prikazom u formi kartica
- Pretraga po naslovu i autoru, case-insensitive, sa resetom pretrage
- Prikaz detalja knjige sa statusom dostupnosti i brojem slobodnih primjeraka
- Upravljanje kategorijama: dodavanje, uređivanje, brisanje uz zabranu brisanja kategorije koja ima povezane knjige
- Napredna pretraga sa kombinovanim filterima po kategoriji, izdavaču i godini

ISBN služi kao standardni bibliografski identifikator koji jednoznačno identifikuje naslov knjige. Sve kopije iste knjige dijele isti ISBN, dok se razlikuju po inventarnom broju primjerka. Ovo omogućava jasno razdvajanje logičkog zapisa (knjiga) od fizičkog primjerka.

**Povezane uloge:** Bibliotekar i Administrator (CRUD operacije), Član (pregled i pretraga).

#### Inventar modul

Odgovoran za upravljanje fizičkim primjercima knjiga i njihovim životnim ciklusom.

- Kreiranje primjeraka pri dodavanju knjige, pri čemu svaki primjerak dobija jedinstven inventarni broj
- Pregled svih primjeraka knjige sa jasnim prikazom statusa (Dostupan / Zadužen)
- Izračun broja slobodnih primjeraka, gdje se zaduženi primjerci ne računaju kao dostupni
- Ažuriranje statusa primjerka pri zaduživanju (-> Zadužen) i vraćanju (-> Dostupan) kao dio atomske transakcije sa Zaduženja modulom
- Zabrana kreiranja primjeraka bez pripadajuće knjige

**Povezane uloge:** Bibliotekar i Administrator (upravljanje primjercima), Član (pregled dostupnosti).

#### Zaduženja modul

Odgovoran za kompletnu evidenciju zaduživanja i vraćanja knjiga.

- Kreiranje novog zaduženja s provjerom: aktivnosti članarine, dostupnosti primjerka, te sprečavanja duplog zaduženja istog primjerka
- Automatsko postavljanje roka vraćanja prema standardizovanom poslovnom pravilu
- Evidencija vraćanja knjige sa zatvaranjem zaduženja i oslobađanjem primjerka
- Atomska transakcija pri zaduživanju: kreiranje zapisa zaduženja + promjena statusa primjerka u „Zadužen" + upis u audit log, sve u jednoj transakciji ili ništa
- Atomska transakcija pri vraćanju: zatvaranje zaduženja + promjena statusa primjerka u „Dostupan" + provjera kašnjenja + upis u audit log
- Pregled vlastitih zaduženja za člana, sa vizuelnim isticanjem zakašnjelih
- Pregled svih aktivnih zaduženja za bibliotekara, sa sortiranjem po roku vraćanja i filtriranjem po članu
- Pregled historije zaduženja članova sa kompletnim detaljima

**Povezane uloge:** Bibliotekar i Administrator (evidencija zaduživanja/vraćanja, pregled aktivnih i historijskih zaduženja), Član (pregled vlastitih zaduženja).

#### Članarina modul

Odgovoran za upravljanje članstvom korisnika i provjerom prava na korištenje bibliotečkih usluga.

- Evidentiranje nove članarine sa datumom početka i datumom isteka
- Ažuriranje (produženje ili ispravka) postojeće članarine sa validacijom datuma, pri čemu datum isteka ne smije biti prije datuma početka
- Automatsko određivanje statusa (Aktivna / Istekla) na osnovu datuma isteka u odnosu na trenutni datum
- Pružanje informacije o statusu članarine drugim modulima, posebno Zaduženja modulu koji provjerava aktivnost članarine kao preduslov zaduživanja
- Prikaz statusa i datuma isteka članarine na profilu člana

**Povezane uloge:** Bibliotekar i Administrator (upravljanje članarinama), Član (pregled vlastitog statusa).

#### Rezervacije modul

Odgovoran za evidenciju i upravljanje rezervacijama knjiga koje nemaju dostupne primjerke.

- Kreiranje rezervacije za knjigu bez dostupnih primjeraka, uz zabrane: rezervacija knjige koja ima slobodne primjerke, te više aktivnih rezervacija istog naslova od istog člana
- Pregled vlastitih aktivnih rezervacija za člana
- Otkazivanje rezervacije od strane člana
- Pregled svih aktivnih rezervacija za bibliotekara sa detaljima (ime člana, email, naslov knjige, datum rezervacije)
- Automatsko otkazivanje rezervacija po isteku definisanog roka preuzimanja, realizacija putem periodičnog zadatka (akter: Sistem)
- Upravljanje redom čekanja prema vremenu kreiranja rezervacije

**Povezane uloge:** Član (kreiranje, pregled, otkazivanje rezervacija), Bibliotekar i Administrator (pregled aktivnih rezervacija), Sistem (automatsko otkazivanje isteklih rezervacija).

#### Audit/Log modul

Odgovoran za automatsko evidentiranje kritičnih akcija u sistemu. Ovaj modul bilježi svaku važnu promjenu u sistemu radi transparentnosti i mogućnosti naknadne analize. Audit log zapisi su vidljivi isključivo administratorima.

- Automatsko evidentiranje akcija iz ostalih modula: dodavanje/uređivanje/brisanje knjiga, kreiranje/deaktivacija korisnika, kreiranje/zatvaranje zaduženja, upravljanje članarinama, kreiranje rezervacija
- Struktura zapisa: korisnik koji je izvršio akciju, tip entiteta, ID entiteta, vrijednosti prije promjene, vrijednosti nakon promjene, datum i vrijeme akcije
- Korištenje JSONB polja za prije/nakon vrijednosti koje pruža elastičnost za razne tipove promjena bez potrebe za dodatnim tabelama
- Pružanje audit trail podataka za pregled od strane administratora

**Povezane uloge:** Sistem (automatsko evidentiranje pri kritičnim akcijama), Administrator (pregled audit zapisa).

### 3.3 Relacijska baza podataka

Baza podataka osigurava trajnu pohranu, integritet i konzistentnost svih podataka sistema. Projektovana je da reflektuje domenski model uz striktna ograničenja.

#### Entiteti i detaljni pregled tabela

| Tabela | Ključne kolone | Opis |
|---|---|---|
| **uloga** | id, naziv, opis | Definira uloge u sistemu: Član, Bibliotekar, Administrator. |
| **korisnik** | id, ime, prezime, email, lozinka_hash, uloga_id, status, datum_kreiranja | Registrirani korisnici sistema. Email je jedinstven identifikator naloga. Status može biti: aktivan / deaktiviran. |
| **kategorija** | id, naziv, opis | Kategorije za klasifikaciju knjiga. Naziv je jedinstven. |
| **knjiga** | id, naslov, autor, isbn, kategorija_id, izdavač, godina_izdanja | Kataloški (bibliografski) zapisi. Predstavlja logički opis knjige, ne fizički primjerak. ISBN je jedinstven identifikator naslova. |
| **primjerak** | id, knjiga_id, inventarni_broj, status, datum_nabave | Fizički primjerci knjiga sa jedinstvenim inventarnim brojem. Status: dostupan / zadužen. Sve kopije iste knjige smatraju se identičnim primjercima. |
| **članarina** | id, korisnik_id, status, datum_početka, datum_isteka | Članstvo i prava korisnika. Status se derivira na osnovu datuma isteka: aktivna / istekla. |
| **zaduženje** | id, korisnik_id, primjerak_id, datum_zaduživanja, datum_planiranog_vraćanja, datum_stvarnog_vraćanja, status | Evidencija zaduživanja/vraćanja. Status: aktivno / zatvoreno / zakašnjelo. |
| **rezervacija** | id, korisnik_id, knjiga_id, datum_rezervacije, datum_isteka, status | Evidencija rezervacija nedostupnih knjiga. Status: aktivna / realizovana / otkazana / istekla. |
| **audit_log** | id, korisnik_id, akcija, entitet_tip, entitet_id, vrijednosti_prije (JSONB), vrijednosti_nakon (JSONB), datum_akcije | Audit trail svih kritičnih promjena u sistemu. |

#### Relacije i referencijalni integritet

- **korisnik -> uloga**: svaki korisnik pripada tačno jednoj ulozi
- **knjiga -> kategorija**: svaka knjiga pripada jednoj kategoriji
- **primjerak -> knjiga**: svaki primjerak pripada jednoj knjizi (jedna knjiga može imati više primjeraka)
- **zaduženje -> korisnik**: evidencija koji član je zadužio
- **zaduženje -> primjerak**: evidencija koji konkretni primjerak je zadužen
- **članarina -> korisnik**: članstvo pripada jednom korisniku
- **rezervacija -> korisnik**: evidencija koji član je rezervisao
- **rezervacija -> knjiga**: evidencija za koju knjigu je rezervacija
- **audit_log -> korisnik**: opciona veza, ko je izvršio akciju

#### Jedinstvene vrijednosti (UNIQUE constraints)

- `korisnik.email`: svaki email mora biti jedinstven u sistemu
- `primjerak.inventarni_broj`: svaki inventarni broj mora biti jedinstven
- `uloga.naziv`: svaki naziv uloge mora biti jedinstven
- `knjiga.isbn`: svaki ISBN mora biti jedinstven
- `kategorija.naziv`: svaki naziv kategorije mora biti jedinstven

#### Statusne provjere i pravila

- **primjerak.status**: `dostupan` / `zadužen`, pri čemu samo primjerak sa statusom „dostupan" može biti zadužen
- **zaduženje.status**: `aktivno` / `zatvoreno` / `zakašnjelo`, pri čemu se zakašnjelost utvrđuje poređenjem datuma planiranog vraćanja sa tekućim datumom
- **članarina.status**: derivira se iz `datum_isteka` u odnosu na tekući datum, aktivna ako datum isteka nije prošao
- **korisnik.status**: `aktivan` / `deaktiviran`, pri čemu deaktivirani korisnik ne može pristupiti sistemu
- **rezervacija.status**: `aktivna` / `realizovana` / `otkazana` / `istekla`, pri čemu samo aktivna rezervacija blokira primjerak

#### Transakcije za kritične procese

Sljedeće operacije su atomske, što znači da se izvršavaju u jednoj transakciji ili se kompletno poništavaju:

- **Zaduživanje**: kreiranje zapisa zaduženja + promjena statusa primjerka u „zadužen" + evidentiranje u audit log
- **Vraćanje**: zatvaranje zaduženja (datum stvarnog vraćanja) + promjena statusa primjerka u „dostupan" + detekcija kašnjenja + evidentiranje u audit log
- **Brisanje knjige**: provjera da li postoje aktivna zaduženja za ijedan primjerak → ako postoje, transakcija se odbija

---

## 4. Komunikaciona arhitektura i kontrola pristupa

### 4.1 Komunikaciona arhitektura

Sistem se sastoji od tri komponente koje komuniciraju na sljedeći način:

**Web klijent (Frontend)** komunicira isključivo sa **API serverom (Backend)** putem HTTP(S) REST poziva. Frontend nikada ne pristupa bazi podataka direktno. Svaki zahtjev prema backendu sadrži JWT token za identifikaciju i autorizaciju korisnika.

**API server (Backend)** je jedina tačka pristupa bazi podataka. Interno, backend je organizovan po modulima (Auth, Korisnici, Katalog, Inventar, Zaduženja, Članarina, Rezervacije, Audit/Log) koji sarađuju unutar istog procesa bez mrežne komunikacije. Audit/Log modul je pomoćna komponenta koja se aktivira iz ostalih modula pri kritičnim akcijama.

**Relacijska baza podataka (PostgreSQL)** prima SQL upite isključivo od backend aplikacije i provodi integritetna ograničenja na nivou baze.

### 4.2 RBAC: kontrola pristupa po ulogama

Kontrola pristupa je implementirana na dva nivoa:

| Nivo | Mehanizam | Svrha |
|---|---|---|
| **Frontend (UI)** | Dinamičko skrivanje/prikazivanje navigacijskih stavki, formi i akcija prema ulozi | Poboljšanje korisničkog iskustva, korisnik ne vidi opcije za koje nema dozvolu |
| **Backend (API)** | Server-side autorizacija na svakom API endpoint-u, provjera uloge iz JWT tokena | Sigurnosni mehanizam, čak i ako klijent pošalje neovlašten zahtjev, backend ga blokira |

**Backend je izvor istine za autorizaciju.** Frontend restrikcije postoje samo radi boljeg UX-a.

Pregled dozvola po ulogama:

| Funkcionalnost | Član | Bibliotekar | Administrator |
|---|:---:|:---:|:---:|
| Prijava/Odjava | ✓ | ✓ | ✓ |
| Pregled kataloga i pretraga | ✓ | ✓ | ✓ |
| Pregled detalja knjige | ✓ | ✓ | ✓ |
| Pregled vlastitog profila | ✓ | ✓ | ✓ |
| Pregled vlastitih zaduženja | ✓ | ✓ | ✓ |
| Pregled statusa članarine | ✓ | ✓ | ✓ |
| Rezervacija knjige | ✓ |  ✓ |  ✓ |
| Kreiranje naloga člana | | ✓ | ✓ |
| CRUD knjiga, kategorija, primjeraka | | ✓ | ✓ |
| Evidencija zaduživanja/vraćanja | | ✓ | ✓ |
| Pregled svih aktivnih zaduženja | | ✓ | ✓ |
| Upravljanje članarinama | | ✓ | ✓ |
| Pregled aktivnih rezervacija | | ✓ | ✓ |
| Pregled članova biblioteke | | ✓ | ✓ |
| Deaktivacija naloga članova | | ✓ | ✓ |
| Pregled svih korisnika sistema | | | ✓ |
| Promjena uloga korisnika | | | ✓ |
| Deaktivacija naloga bibliotekara | | | ✓ |
| Pregled audit loga | | | ✓ |
| Generisanje izvještaja | | | ✓ |

---

## 5. Ključne tehničke odluke

### Odluka 1: Klijent-server arhitektura sa centralizovanim backendom

- **Razlog:** Sistem koristi tri korisničke uloge sa različitim nivoima pristupa i centralizovana poslovna pravila. Potrebno je jedinstveno mjesto za autorizaciju, validaciju i konzistentnost podataka koje nije zavisno od klijenta.
- **Prednosti:** Sigurnost pravila nezavisna od frontend implementacije; jedna tačka za sve validacije; lakša kontrola pristupa.
- **Potencijalni nedostaci:** Single point of failure, jer kvar backend procesa utiče na cijeli sistem. Ublažava se monitoringom i planom oporavka.

### Odluka 2: Modularni monolit umjesto mikroservisa

- **Razlog:** Tim od 8 članova sa razvojnim periodom od približno 7 sedmica i pretpostavkom malog broja istovremenih korisnika. Mikroservisna arhitektura bi zahtijevala dodatno vrijeme za postavljanje infrastrukture koje u ovom kontekstu nije opravdano.
- **Prednosti:** Brži razvoj, jednostavniji deployment i debugging, prirodna podrška za atomske transakcije preko više modula, manji operativni overhead.
- **Potencijalni nedostaci:** Monolit može postati teže skalabilan kako sistem raste. Moguće je jačanje sprezanja modula ako granice nisu disciplinovano održavane.
- **Mjera:** Striktna modularna organizacija sa jasnim internim interfejsima, čime se ostavlja mogućnost kasnijeg izdvajanja modula.

### Odluka 3: PostgreSQL kao relacijska baza podataka

- **Razlog:** Domen bibliotečkog sistema ima jasno definisane entitete sa čvrstim relacijama i jake zahtjeve za integritet podataka. PostgreSQL je izabran jer pruža nativnu podršku za JSONB tip podataka, što je ključno za audit_log tabelu. JSONB omogućava indeksiranje i efikasno pretraživanje unutar JSON strukture, po čemu je superiorniji od alternativa poput MySQL-a.
- **Prednosti:** Transakcije (ACID), referencijalni integritet, unique constraints, stroga tipizacija, zrelost, pouzdanost i napredna podrška za JSONB.
- **Potencijalni nedostaci:** Nešto složenija inicijalna konfiguracija u odnosu na SQLite ili MySQL. Ublažava se standardizovanim razvojnim okruženjem.

### Odluka 4: JWT autentifikacija sa kratkotrajnim tokenima

- **Razlog:** S obzirom na klijent-server arhitekturu sa odvojenim frontendom i backend API-jem, JWT je industrijski standard za ovaj tip komunikacije. Stateless pristup eliminira potrebu za server-side pohranom sesija.
- **Prednosti:** Stateless autentifikacija, jednostavna integracija sa REST API-jem, mogućnost prenosa podataka o ulozi unutar tokena.
- **Potencijalni nedostaci:** Otežana invalidacija tokena prije isteka (npr. pri deaktivaciji korisnika). Ublažava se kratkotrajnim tokenima (npr. 30 minuta) i opcionalnom listom poništenih tokena za kritične slučajeve.

### Odluka 5: Razdvajanje entiteta „knjiga" i „primjerak"

- **Razlog:** „Knjiga" predstavlja bibliografski zapis (naslov, autor, ISBN), dok „primjerak" predstavlja konkretan fizički primjerak tog naslova sa vlastitim inventarnim brojem i statusom. Sve kopije iste knjige smatraju se identičnim primjercima (pretpostavka iz Product Vision dokumenta).
- **Prednosti:** Mogućnost inventure po primjerku, tačna evidencija dostupnosti, detaljnija historija zaduženja.
- **Potencijalni nedostaci:** Povećava broj entiteta i kompleksnost upita za dostupnost. Ublažava se centralizovanom servisnom logikom u Inventar modulu.

### Odluka 6: RBAC na backendu kao izvor istine za autorizaciju

- **Razlog:** Sigurnosna pravila ne smiju zavisiti samo od frontend prikaza. Direktan API poziv (npr. putem alata poput Postman ili curl) mora biti odbijen ako korisnik nema odgovarajuću ulogu.
- **Prednosti:** Zaštita od IDOR i privilege escalation napada; konzistentna sigurnost nezavisna od klijenta.
- **Potencijalni nedostaci:** Svaki endpoint zahtijeva provjeru autorizacije, što dodaje implementacioni napor. Ublažava se middleware/interceptor pristupom.

### Odluka 7: Atomske transakcije za zaduživanje i vraćanje

- **Razlog:** Zaduživanje zahtijeva istovremenu promjenu stanja na više entiteta (zaduženje, primjerak, audit_log). Bez atomske transakcije postoji rizik od nekonzistentnog stanja.
- **Prednosti:** Garantovana konzistentnost; eliminacija djelomičnih ažuriranja; zaštita od konkurentnih pristupa istom primjerku.
- **Potencijalni nedostaci:** Dugotrajne transakcije mogu uzrokovati zaključavanje. Ublažava se kratkotrajnim transakcijama i optimističkim zaključavanjem gdje je primjenjivo.

### Odluka 8: Audit log sa JSONB poljima

- **Razlog:** Različiti entiteti imaju različite atribute. JSONB polje u PostgreSQL-u pruža elastičnost za evidentiranje bilo koje promjene uz mogućnost indeksiranja.
- **Prednosti:** Jedna generička tabela za sve tipove audita; mogućnost praćenja tačno šta se promijenilo; JSONB u PostgreSQL-u podržava GIN indekse za brzo pretraživanje.
- **Potencijalni nedostaci:** Rast tabele može usporiti performanse. Ublažava se indeksiranjem po datumu i periodičnim arhiviranjem.

### Odluka 9: Soft Delete umjesto Hard Delete

- **Razlog:** U bibliotečkom sistemu fizičko brisanje korisnika ili knjiga iz baze narušava referencijalni integritet, jer bi se izgubile veze sa historijskim zaduženjima, rezervacijama i audit log zapisima. Brisanje korisnika bi zahtijevalo kaskadno brisanje cijele historije, što je poslovno neprihvatljivo.
- **Prednosti:** Čuvanje kompletne historije; održavanje referencijalnog integriteta; mogućnost oporavka slučajno „obrisanih" zapisa.
- **Potencijalni nedostaci:** Svi upiti moraju filtrirati neaktivne zapise, što dodaje mali overhead. Ublažava se korištenjem database view-ova ili aplikacionih filtera.
- **Implementacija:** Korištenje postojeće `status` kolone (npr. `deaktiviran` za korisnike) kao mehanizma logičkog brisanja.

### Odluka 10: Validacija na dva nivoa (klijent + server)

- **Razlog:** Klijentska validacija poboljšava korisničko iskustvo (brze poruke greške uz polje sa greškom), ali nije dovoljna za sigurnost. Server-side validacija je obavezna.
- **Prednosti:** UX poboljšanje bez kompromisa u sigurnosti, smanjenje nepotrebnih API poziva.
- **Potencijalni nedostaci:** Dupliranje validacione logike. Ublažava se definisanjem zajedničkih pravila.

---

## 6. Ograničenja i rizici arhitekture

### 6.1 Tehnički rizici

**Monolit kao single point of failure (R-53)**

Backend aplikacija je jedinstveni proces i kvar ovog procesa utiče na cijeli sistem i onemogućava rad biblioteke. Za biblioteku koja zavisi od sistema za svakodnevne operacije (zaduživanje, vraćanje), ovo je kritičan rizik.

*Mjera ublažavanja:* Osnovni monitoring aplikacije, redovan backup baze podataka, definisan plan oporavka sa rollback procedurom.

**Nekonzistentnost pri konkurentnim akcijama**

Dva paralelna zahtjeva mogu pokušati zadužiti isti primjerak ili napraviti konfliktne izmjene na istom zapisu. U bibliotečkom okruženju sa više aktivnih bibliotekara, ovo je realan scenarij.

*Mjera ublažavanja:* Transakcijsko zaključavanje na nivou baze, provjera statusa primjerka neposredno prije potvrde akcije, stroga statusna pravila koja sprečavaju nevalidne prijelaze stanja.

**Rast audit log tabele i degradacija performansi**

Audit log bilježi svaku kritičnu akciju, što u aktivnoj biblioteci može generisati značajan volumen zapisa. Bez upravljanja, ova tabela može usporiti upite.

*Mjera ublažavanja:* Indeksiranje po `datum_akcije` i `entitet_tip`, periodično arhiviranje starih zapisa, paginacija pri čitanju.

**Notifikacije: in-app statusi kao prioritet za MVP**

U MVP verziji sistema, eksterne email notifikacije nisu uključene u scope. Za početnu fazu, sistem koristi in-app statuse i vizuelna upozorenja (npr. vizuelno isticanje zakašnjelih zaduženja, prikaz statusa članarine na profilu) kao primarni mehanizam informisanja korisnika. Email obavijesti su planirane kao kasniji feature i ne smiju blokirati isporuku osnovnih funkcionalnosti.

*Mjera ublažavanja:* Fokus na in-app obavijesti za MVP, email implementirati kao dodatni kanal tek nakon stabilizacije core funkcionalnosti.

**Performanse pretrage na većem katalogu**

Bez paginacije i indeksiranja, pretraga kataloga sa velikim brojem zapisa može prekoračiti zahtjev od ≤2 sekunde učitavanja. Za MVP, pretraga koristi SQL LIKE operator koji može degradirati performanse na većem katalogu. Ovo je svjesno prihvaćen tehnički dug koji se planira riješiti migracijom na PostgreSQL Full-Text Search u kasnijim sprintovima.

*Mjera ublažavanja:* Paginacija na UI i API nivou, indeksiranje ključnih polja (naslov, autor, ISBN, kategorija_id), optimizacija upita.

### 6.2 Organizacioni rizici

**Rast kompleksnosti pri uvođenju planiranih funkcionalnosti**

Rezervacije, notifikacije, kazne i izvještaji značajno povećavaju kompleksnost sistema. Rezervacije posebno nose mnogo poslovnih pravila koja mogu izazvati jaču spregu između modula ako se ne uvedu jasne granice.

*Mjera ublažavanja:* Postepeno uvođenje kroz zasebne sprintove, definisanje jasnih internih interfejsa između modula.

**Potencijalno jačanje sprezanja modula**

Bez disciplinovanog pridržavanja granica modula postoji rizik da se direktne zavisnosti između komponenti neplanirano prošire.

*Mjera ublažavanja:* Code review sa fokusom na poštivanje granica modula, evidentiranje tehničkog duga.

### 6.3 Sigurnosni rizici

**Neovlašten pristup administratorskim funkcijama**

Nedovoljno striktna autorizacija na API nivou može dozvoliti korisniku s nižim privilegijama pristup zaštićenim funkcijama.

*Mjera ublažavanja:* Server-side autorizacija na svakom endpoint-u, sistematski sigurnosni review koda.

**Nedovoljna zaštita korisničkih podataka**

Nekorektno hashiranje lozinki, nekorištenje HTTPS-a ili izlaganje osjetljivih podataka u API odgovorima predstavljaju ozbiljne sigurnosne propuste.

*Mjera ublažavanja:* Hashiranje lozinki standardnim algoritmom, ograničenje podataka u API odgovorima, korištenje sigurnih konekcija.

**Mogućnost pristupa tuđim podacima**

Neispravna autorizacija na nivou API-ja može dozvoliti članu da vidi tuđa zaduženja ili podatke profila, što je ozbiljan problem privatnosti.

*Mjera ublažavanja:* Svaki upit vezati za identitet prijavljenog korisnika, testirati pristup tuđim podacima.

---

## 7. Otvorena pitanja

Sljedeća pitanja još nisu jednoznačno definisana:

1. **Pravila rezervacionog sistema.** Koliko dugo vrijedi rezervacija nakon što knjiga postane dostupna? Kako se određuje prioritet u redu čekanja? Kako sistem tretira situaciju kada rezervacija istekne, a postoje i drugi čekajući?

2. **Model kazni za kašnjenje.** Fiksni iznos po danu, progresivni model, ili privremena blokada zaduživanja dok se knjiga ne vrati? Definicija utiče na entitet kazne i Zaduženja modul.

3. **API dokumentacija.** Da li API treba biti javno dokumentovan kroz OpenAPI/Swagger već u MVP-u ili je to odgođeno za kasniju fazu?

4. **Politika čuvanja audit log zapisa.** Koliko dugo čuvati zapise? Da li je potrebna rotacija, arhivacija ili eksterna pohrana?

5. **Pravila za timeout i automatske akcije.** Koliko dana nakon planiranog roka se zaduženje označava kao „zakašnjelo"? Da li postoji limit broja istovremenih zaduženja po članu?

6. **Strategija za reset lozinke.** Da li u MVP-u bibliotekar/administrator ručno resetuje lozinku ili se implementira neki drugi mehanizam?

7. **Pravila migracija baze podataka.** Koji alat će se koristiti za migracije, kako se migracije verzionišu i da li postoji rollback procedura?
