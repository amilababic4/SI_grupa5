# Architecture Overview

## Kratak opis arhitektonskog pristupa

Bibliotečki informacioni sistem je planiran kao **klijent-server rješenje** sa **modularnim monolitnim backendom**.

- Klijent (web aplikacija) služi za prikaz podataka i unos korisničkih akcija.
- Server implementira poslovna pravila, validaciju, autentifikaciju/autorizaciju i pristup podacima.
- Baza podataka centralizovano čuva sve informacije o korisnicima, knjigama, primjercima, zaduženjima i članarinama.

## Glavne komponente sistema

### 1) Web klijent (Frontend)
Interfejs za korisničke uloge (Član, Bibliotekar, Administrator), uključujući:
- prijavu/odjavu (Član, Bibliotekar, Administrator)
- pregled i pretragu kataloga (Član, Bibliotekar, Administrator)
- prikaz detalja knjige i dostupnosti (Član, Bibliotekar, Administrator)
- administrativne forme (upravljanje knjigama, korisnicima, članarinama) (Bibliotekar, Administrator)
- pregled zaduženja i historije (Član, Bibliotekar, Administrator)
- upravljanje uposlenicima (Administrator)

### 2) API server (Backend monolit)
Centralna aplikacija organizovana po modulima:
- **Auth modul**: prijava, sesija/token, hash lozinki, kontrola pristupa
- **Korisnici modul**: registracija člana, upravljanje ulogama, deaktivacija naloga, profil
- **Katalog modul**: knjige, kategorije, pretraga, detalji knjige
- **Inventar modul**: upravljanje primjercima i statusima primjeraka
- **Zaduženja modul**: evidencija zaduživanja/vraćanja, historija zaduženja
- **Članarina modul**: status članarine, datumi početka i isteka
- **Audit/Log modul**: evidencija važnih administratorskih akcija

### 3) Relacijska baza podataka
Sloj za trajnu pohranu podataka i očuvanje integriteta:

#### Entiteti:
- **uloga** - tipovi korisnika (Član, Bibliotekar, Administrator)
- **korisnik** - registrirani korisnici sistema
- **kategorija** - kategorije knjiga
- **knjiga** - kataloški zapisi (logički opis knjige)
- **primjerak** - fizički primjerci knjiga sa jedinstvenim inventarnim brojevima
- **članarina** - status i validnost članstva korisnika
- **zaduženje** - evidencija zaduživanja/vraćanja primjeraka
- **audit_log** - audit trail svih kritičnih akcija

#### Relacije i ograničenja:
- **FK relacije**: 
  - korisnik → uloga
  - zaduženje → korisnik (član koji zadužuje)
  - zaduženje → primjerak (koji primjerak je zadužen)
  - primjerak → knjiga (kojem katalogu pripada)
  - članarina → korisnik (kojem članu pripada)
  - audit_log → korisnik (opciono - ko je izvršio akciju)

- **Jedinstvene vrijednosti**: 
  - korisnik.email
  - primjerak.inventarni_broj
  - uloga.naziv

- **Statusne provjere**: 
  - primjerak.status: dostupan/zadužen/oštećen/otpisan
  - zaduženje.status: aktivno/zatvoreno/zakašnjelo
  - članarina.status: aktivna/neaktivna/istekla
  - korisnik.status: aktivan/deaktiviran

- **Napomena**: Bibliotekar i Administrator su uloge (tipovi korisnika sa specifičnim dozvolama), ne odvojeni entiteti

#### Transakcije za kritične procese:
- **Zaduživanje**: atomska operacija (kreiraj zaduženje + promijeni status primjerka u "zadužen" + loguj događaj)
- **Vraćanje**: atomska operacija (zatvori zaduženje + oslobodi primjerak postavi status "dostupan" + loguj događaj)
- **Inventar**: ažuriranje statusa primjerka (npr. označavanje kao oštećeno/otpisano)

#### Detaljni pregled tabela:

| Tabela | Ključne kolone | Opis |
|---|---|---|
| **uloga** | id, naziv, opis | Definira dostupne uloge u sistemu |
| **korisnik** | id, email, lozinka_hash, uloga_id, status, datum_kreiranja | Korisničke kredencijale i metadata |
| **kategorija** | id, naziv, opis | Kategorije za klasifikaciju knjiga |
| **knjiga** | id, naslov, autor, isbn, kategorija_id, izdavač, godina_izdanja | Kataloški zapisi книг |
| **primjerak** | id, knjiga_id, inventarni_broj, status, lokacija, datum_nabave | Fizički primjerci sa statusom i lokacijom |
| **članarina** | id, korisnik_id, status, datum_početka, datum_isteka, tip_članarine | Članstvo i prava zaduživanja |
| **zaduženje** | id, korisnik_id, primjerak_id, datum_zaduživanja, datum_planog_vraćanja, datum_stvarnog_vraćanja, status | Evidencija zaduživanja/vraćanja |
| **audit_log** | id, korisnik_id, akcija, entitet_tip, entitet_id, vrijednosti_prije (JSON), vrijednosti_nakon (JSON), datum_akcije | Audit trail svih izmjena |

### 4) Planirani pomoćni podsistemi (u kasnijim sprintovima)
- **Notifikacije** (email podsjetnici, obavijesti rezervacija)
- **Periodični zadaci** (npr. automatsko otkazivanje rezervacija)
- **Izvještavanje** (mjesečni izvještaji za upravu)
- **Rezervacije** (evidencija rezervisanih primjeraka)
- **Kazne/Naknade** (pracenje i naplata kasnina)

## Odgovornosti komponenti

| Komponenta | Primarna odgovornost | Interakcije |
|---|---|---|
| Web klijent | Prikaz podataka, unos akcija, validacija osnovnih formi na klijentu | Poziva API server, prikazuje odgovore i poruke greške |
| API server | Poslovna logika, validacija na serveru, RBAC, obrada zahtjeva | Prima zahtjeve od klijenta, čita/piše bazu, upisuje audit log |
| Baza podataka | Trajna pohrana, integritet i konzistentnost podataka | Servisira upite API servera, provodi ograničenja i relacije |
| Audit/Log sloj | Praćenje kritičnih promjena i aktivnosti | Prima događaje iz backend modula, čuva zapis u bazi |
| Notifikacije (planirano) | Slanje email poruka i podsjetnika | Dobija događaje iz backenda ili periodičnih zadataka |

## Tok podataka i interakcija

U nastavku je tekstualni prikaz glavnih tokova (zamjena za activity/data-flow dijagram):

### Tok 1: Prijava korisnika
1. Korisnik unosi email i lozinku u web klijentu.
2. Frontend šalje zahtjev Auth modulu na backendu.
3. Backend provjerava kredencijale (hash lozinke) i ulogu korisnika.
4. Ako je prijava uspješna, backend vraća token/sesiju i osnovne podatke o ulozi.
5. Frontend otvara odgovarajući dashboard prema ulozi.

### Tok 2: Dodavanje nove knjige
1. Bibliotekar/Admin unosi podatke o knjizi.
2. Frontend šalje zahtjev Katalog modulu.
3. Backend provodi validaciju (obavezna polja).
4. Podaci se upisuju u bazu i vraća se potvrda.
5. Katalog se osvježava i nova knjiga postaje vidljiva korisnicima.

### Tok 3: Zaduživanje primjerka
1. Bibliotekar bira člana i konkretan primjerak.
2. Frontend šalje zahtjev Zaduženja modulu.
3. Backend provjerava:
   - da li korisnik ima pravo zaduživanja (uloga/status članarine)
   - da li je primjerak dostupan
   - da li je korisnik dostigao maksimalni broj istovremeno zaduženih knjiga
4. U jednoj transakciji backend:
   - kreira zapis zaduženja sa rokom vraćanja
   - mijenja status primjerka u "zadužen"
   - evidentira događaj u audit log
5. Frontend prikazuje potvrdu i ažurirano stanje dostupnosti.

### Tok 4: Evidencija vraćanja knjige
1. Bibliotekar pronalazi aktivno zaduženje i potvrđuje vraćanje.
2. Backend:
   - zatvara zaduženje (datum stvarnog vraćanja)
   - vraća status primjerka na "dostupan"
   - provjerava da li je zaduženje zakašnjelo i loguje ako jeste
   - evidentira događaj u audit log
3. Sistem odmah ažurira prikaz u katalogu i profilu člana.

### Tekstualni opis komponentnog dijagrama
- Web klijent komunicira isključivo sa API serverom putem HTTP(S) poziva.
- API server je centralna tačka i pristupa bazi podataka.
- Backend moduli međusobno sarađuju unutar istog procesa (monolit), kroz jasno odvojene module/slojeve.
- Audit/Log je pomoćna komponenta backenda koja se aktivira pri kritičnim akcijama.
- Planirane komponente za notifikacije i periodične zadatke se oslanjaju na podatke iz backenda i baze.

## Ključne tehničke odluke

1. **Klijent-server arhitektura**
   - Razlog: sistem koristi više korisničkih uloga i centralna poslovna pravila. Potrebno je jedinstveno mjesto za autorizaciju, validaciju i konzistentnost podataka.

2. **Modularni monolit (umjesto mikroservisa u MVP fazi)**
   - Razlog: manji tim i ograničen rok projekta, brži razvoj, jednostavniji deployment i lakše debugovanje.
   - Dodatno: modularna struktura ostavlja mogućnost kasnijeg izdvajanja modula ako opseg poraste.

3. **Relacijska baza podataka**
   - Razlog: domen ima jasno definisane relacije i jake zahtjeve za integritet (korisnik-zaduženje-primjerak).
   - Prednost: transakcije i integritet referenci smanjuju rizik nekonzistentnih podataka.

4. **Primjerak kao odvojena entitet (ne samo broj dostupnih primjeraka)**
   - Razlog: svaki fizički primjerak ima svoj životni vijek, lokaciju i status.
   - Prednost: mogućnost inventure, praćenja oštećenja po primjerku i detaljne historije.

5. **RBAC (Role-Based Access Control) na backendu kao izvor istine**
   - Razlog: sigurnosna pravila ne smiju zavisiti samo od frontend prikaza.
   - Posljedica: čak i ako klijent pošalje neispravan zahtjev, backend blokira neovlaštenu akciju.

6. **Audit log za kritične administrativne akcije**
   - Razlog: praćenje promjena, odgovornost i lakša analiza incidenata.

7. **JSON polje u audit_log za prije/nakon vrijednosti**
   - Razlog: elastičnost za razne tipove promjena bez dodatnih tabela.
   - Prednost: lakše praćenje što se točno promijenilo i mogućnost povratka na stare vrijednosti.

## Ograničenja i rizici arhitekture

1. **Monolit može postati teško skalabilan kako sistem raste**
   - Rizik: veće vrijeme build/deploy ciklusa i jača međuzavisnost modula.
   - Mjera ublažavanja: striktna modularna organizacija koda i jasni interni interfejsi.

2. **Single point of failure backend aplikacije**
   - Rizik: kvar backend procesa utiče na cijeli sistem.
   - Mjera ublažavanja: monitoring, backup baza, plan oporavka i postupno uvođenje redundanse.

3. **Rizik nekonzistentnosti pri konkurentnim akcijama (zaduživanje istog primjerka)**
   - Rizik: dva paralelna zahtjeva mogu pokušati zadužiti isti primjerak.
   - Mjera ublažavanja: transakcije, zaključavanje reda/optimističko zaključavanje i stroga statusna pravila.

4. **Sigurnosni rizici (neispravna kontrola pristupa)**
   - Rizik: neovlašten korisnik može pristupiti administrativnim funkcijama.
   - Mjera ublažavanja: server-side autorizacija, testovi privilegija, audit zapisi.

5. **Nedovoljna observabilnost u ranim fazama**
   - Rizik: sporije otkrivanje uzroka grešaka u produkcionom okruženju.
   - Mjera ublažavanja: standardizovan logging format i minimum metriken performansi.

6. **Buduće funkcionalnosti (rezervacije, notifikacije, kazne) povećavaju kompleksnost**
   - Rizik: poslovna pravila mogu postati previše spregnuta ako se ne uvedu jasne granice modula.
   - Mjera ublažavanja: postepeno uvođenje domenskih servisa i refaktorisanje prije većih nadogradnji.

7. **Auditor log može postati velik i sporiti upite**
   - Rizik: neograničeni rast audit zapisa može degradirati performanse.
   - Mjera ublažavanja: arhiviranje starih zapisa, indexiranje po datumima, periodičko čišćenje.

## Otvorena pitanja

1. Da li autentifikacija koristi JWT, session/cookie pristup ili hibridni model?
2. Koja je ciljna baza (npr. PostgreSQL/MySQL) i koja pravila migracija će biti standard?
3. Kako tačno definisati pravila rezervacije (rok preuzimanja, prioritet liste čekanja, auto-otkazivanje)?
4. Koji kanal notifikacija je MVP prioritet (email, in-app, SMS) i koji su SLA zahtjevi?
5. Kako tretirati kazne za kašnjenje: fiksni iznos, progresivni model ili blokada zaduživanja?
6. Da li je potreban soft delete za knjige/korisnike radi historije i audit zahtjeva?
7. Koji su minimalni tehnički zahtjevi za produkciju (monitoring, backup politika, RTO/RPO)?
8. Da li API treba javno dokumentovati kroz OpenAPI/Swagger već u MVP-u ili u kasnijoj fazi?
9. Koliko dugo čuvati audit log zapise i da li je potrebna arhivacija starih podataka?
10. Koji je timeout za vraćanje knjige nakon planiranog roka, tj. kada se aktivira notifikacija?

---

Ovaj dokument predstavlja početni arhitektonski pregled. Detalji implementacije (API ugovori, model baze i deployment specifikacije) biće razrađeni u sklopu tehničke dokumentacije kroz naredne sprintove.