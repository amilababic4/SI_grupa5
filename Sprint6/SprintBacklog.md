# Sprint Backlog – Sprint 6

## Opis sprinta
Sprint 6 fokusira se na implementaciju funkcionalnosti za upravljanje bibliotečkim fondom, uz osiguranje tehničke stabilnosti sistema kroz sveobuhvatno unit testiranje. 
Tokom ovog sprinta implementiraju se funkcionalnosti vezane za:

* upravljanje katalogom knjiga i kategorizaciju fonda
* administraciju fizičkih primjeraka i praćenje njihovog statusa
* sigurnosnu autorizaciju korisnika na osnovu dodijeljenih uloga
* verifikaciju poslovne logike kroz testove

<br>


| ID | Naziv stavke | Kratak opis | Prioritet | Procjena napora | Status |
|:--:| :--- | :--- | :---: | :---: | :---: |
| PB-22 | Dodavanje nove knjige | Forma za unos knjiga sa validacijom ISBN-a i automatskim generisanjem inventara. | Visok | M | **Završeno** |
| PB-23 | Uređivanje podataka o knjizi | Ažuriranje metapodataka o knjigama uz očuvanje integriteta baze. | Srednji | S | **Završeno** |
| PB-28 | Pregled kataloga | Prikaz knjiga sa implementiranom paginacijom za optimalne performanse. | Visok | S | **Završeno** |
| PB-26 | Upravljanje primjercima knjige | Praćenje statusa pojedinačnih fizičkih knjiga (dostupno, oštećeno, posuđeno). | Visok | M | **Završeno** |
| PB-27 | Brisanje knjige i deaktivacija primjerka | Sigurno uklanjanje knjiga iz kataloga uz provjeru aktivnih zaduženja. | Srednji | XS | **Završeno** |
| PB-25 | Upravljanje kategorijama knjiga | Organizacija fonda kroz hijerarhiju žanrova i oblasti. | Srednji | S | **Završeno** |
| PB-19 | AI i Decision Log | Dokumentovanje arhitektonskih odluka i upotrebe AI alata tokom razvoja. | Nizak | XS | **Završeno** |

<br>

## Sprint Backlog stavke:

## PB-22: Dodavanje nove knjige

### Naziv: Prikaz i unos forme za dodavanje knjige
### US-12: Kao bibliotekar ili admin, želim unijeti podatke o novoj knjizi kroz formu za dodavanje, kako bi knjiga bila evidentirana u sistemu biblioteke.
**Acceptance Criteria:**
- Kada bibliotekar ili admin otvori formu za dodavanje knjige, tada sistem prikazuje polja za unos podataka
- Sistem mora omogućiti unos: naslov, autor, ISBN, godina izdanja, kategorija i broj primjeraka
- Kada korisnik klikne na "Sačuvaj", tada sistem provjerava ispravnost unesenih podataka
- Kada obavezni podaci nedostaju, tada sistem prikazuje poruku o grešci

 <br>

---

### Naziv: Validacija ISBN-a
### US-13: Kao bibliotekar ili admin, želim da sistem validira ISBN prije spremanja knjige, kako bi podaci u katalogu bili tačni.
**Acceptance Criteria:**
- Kada ISBN nije u prihvatljivom formatu, sistem prikazuje grešku.
- Kada ISBN već postoji, sistem odbija spremanje.
- Greška je vezana baš za ISBN polje.
- Nakon ispravke ISBN-a spremanje je moguće.

---

### Naziv: Dodjela početnog broja primjeraka
### US-14: Kao bibliotekar ili admin, želim da prilikom dodavanja knjige odredim početni broj primjeraka, kako bi katalog odmah prikazivao realno stanje fonda.
**Acceptance Criteria:**
- Forma sadrži polje za broj primjeraka.
- Dozvoljen je unos samo nenegativnog cijelog broja.
- Nakon spremanja knjige kreira se odgovarajući broj primjeraka.
- Ako je broj primjeraka 0, knjiga postoji u katalogu ali nije dostupna za zaduženje.

---

### Naziv: Dodjela kategorije pri unosu knjige
### US-15: Kao bibliotekar ili admin, želim izabrati kategoriju knjige pri unosu, kako bi knjiga odmah bila pravilno klasifikovana.
**Acceptance Criteria:**
- Forma za knjigu prikazuje listu postojećih kategorija.
- Bibliotekar ili admin može odabrati jednu kategoriju.
- Bez validne kategorije sistem ne sprema knjigu ako je kategorija obavezna.
- Odabrana kategorija se vidi u katalogu i detaljima knjige.

---
### Naziv: Finalizacija unosa i objava u katalogu
### US-16: Kao bibliotekar ili admin, želim da se nakon uspješnog unosa knjiga automatski dodaje u katalog, kako bi bila dostupna korisnicima sistema.
**Acceptance Criteria:**
- Kada su svi podaci ispravni, tada se knjiga sprema u sistem
- Nakon uspješnog spremanja, knjiga se prikazuje u katalogu
- Nova knjiga mora biti dostupna svim korisnicima za pregled i pretragu
- Svaka knjiga mora imati jedinstven ISBN u sistemu

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava kontinuirano ažuriranje bibliotečkog fonda i osigurava da korisnici imaju pristup novim knjigama u realnom vremenu kroz katalog sistema. |
| **Pretpostavke / Otvorena pitanja** |  Da li knjiga može biti dodana bez određenih podataka (npr. kategorija)? <br> Kategorije postoje unaprijed. <br> Primjerci se kreiraju automatski. |
| **Veze i zavisnosti** | Zavisi od definisanog entiteta Knjiga u sistemu. <br> Zavisi od implementiranog kataloga knjiga. <br> PB-17 Sistem prijave korisnika. <br> PB-25 Upravljanje kategorijama knjiga. <br> PB-26 Upravljanje primjercima knjige.|
---

<br>

## PB-23: Uređivanje podataka o knjizi 

### Naziv: Otvaranje postojećih podataka knjige za izmjenu
### US-17: Kao bibliotekar ili admin, želim pristupiti postojećoj knjizi i izmijeniti njene osnovne podatke, kako bi informacije u katalogu bile tačne i ažurirane.
**Acceptance Criteria:**
- Kada bibliotekar ili admin odabere knjigu iz kataloga, tada sistem prikazuje trenutne podatke o knjizi
- Sistem mora omogućiti izmjenu sljedećih podataka: naslov, autor, godina izdanja i kategorija
- Kada korisnik izmijeni podatke i klikne na "Sačuvaj izmjene", tada se vrši validacija unosenih vrijednosti
- Sistem ne smije dozvoliti spremanje ako obavezni podaci nisu popunjeni

<br>

---
### Naziv: Validacija i potvrda o uspješnom uređivanju
### US-18: Kao bibliotekar ili admin, želim da se nakon uspješnog uređivanja podataka promjene sačuvaju i budu vidljive u katalogu, kako bi korisnici uvijek imali ažurne informacije.
**Acceptance Criteria:**
- Kada su uneseni podaci ispravni, tada se promjene uspješno spremaju u sistem
- Nakon spremanja, ažurirani podaci se odmah prikazuju u katalogu
- Kada podaci nisu ispravni, tada sistem prikazuje poruku o grešci i ne sprema promjene
- Samo članovi osoblja ima pristup uređivanju knjiga

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava održavanje tačnosti i ažurnosti podataka u bibliotečkom katalogu, čime se osigurava da korisnici uvijek imaju ispravne informacije o knjigama. |
| **Pretpostavke / Otvorena pitanja** |  Knjiga koju uređujemo postoji u sistemu. <br> Da li postoji ograničenje koja polja se mogu mijenjati nakon kreiranja knjige? |
| **Veze i zavisnosti** | PB-22: Dodavanje nove knjige |
---

<br>

## PB-28: Pregled kataloga knjiga

### Naziv: Učitavanje aktivnih knjiga u katalog
### US-19: Kao korisnik sistema, želim vidjeti listu svih dostupnih knjiga u biblioteci, kako bih mogao pretražiti i pronaći knjige koje me zanimaju.
**Acceptance Criteria:**
- Kada korisnik pristupi stranici kataloga knjiga, tada sistem prikazuje listu svih knjiga
- Kada u sistemu postoji više knjiga, tada se sve knjige prikazuju u formi liste ili kartica
- Kada korisnik otvori katalog, tada se podaci automatski učitavaju iz baze podataka
- Kada nema dostupnih knjiga u sistemu, tada se prikazuje poruka da katalog trenutno nema knjiga
- Sistem ne smije prikazivati knjige koje nisu aktivne ili obrisane iz sistema

<br>

---
### Naziv: Uređeni prikaz liste kataloga
### US-20: Kao korisnik sistema, želim da mogu pregledati katalog kroz stranice, kako bih lakše pregledao veći broj knjiga.
**Acceptance Criteria:**
- Kada katalog sadrži veliki broj knjiga, tada sistem mora omogućiti paginaciju
- Kada korisnik otvori katalog, tada se prikazuje ograničen broj knjiga po stranici
- Kada korisnik klikne na narednu ili prethodnu stranicu, tada se učitava odgovarajući set knjiga
- Kada korisnik mijenja stranicu, tada se podaci ispravno ažuriraju bez gubitka konteksta

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava korisnicima jednostavan i strukturiran pregled svih dostupnih knjiga u biblioteci, čime se olakšava pretraga i pronalazak željene literature. Takođe poboljšava korisničko iskustvo i efikasnost korištenja bibliotečkog sistema. |
| **Pretpostavke / Otvorena pitanja** | Knjige su već unesene u sistem. <br> Samo prijavljeni korisni imaju pristup katalogu knjiga. <br> Koliko knjiga se prikazuje po stranici? |
| **Veze i zavisnosti** | PB-22: Dodavanje nove knjige. <br> Entitet Knjiga u bazi podataka.  <br> PB-27 Brisanje knjige i deaktivacija primjerka.|
---

<br>

## PB-26: Upravljanje primjercima knjige

### Naziv: Dodavanje novog primjerka postojećoj knjizi
### US-21: Kao bibliotekar ili admin, želim da mogu dodati fizičke primjerke iste knjige u sistem, kako bih imao tačnu evidenciju dostupnog fonda.
**Acceptance Criteria:**
- Kada član osoblja unosi novu knjigu, tada sistem omogućava unos broja primjeraka
- Kada postoji više primjeraka iste knjige, tada svaki primjerak mora biti zaseban zapis u sistemu
- Sistem ne smije dozvoliti kreiranje primjeraka bez povezane knjige

<br>

---

### Naziv: Pregled primjeraka
### US-22: Kao bibliotekar ili admin, želim da vidim sve primjerke jedne knjige, kako bih mogao pratiti njihov status i raspoloživost.
**Acceptance Criteria:**
- Kada korisnik otvori detalje knjige, tada sistem prikazuje listu svih primjeraka te knjige
- Kada knjiga ima više primjeraka, tada svaki primjerak mora biti jasno prikazan u listi
- Kada se prikazuju primjerci, tada moraju biti prikazani jedinstveni identifikatori svakog primjerka
- Sistem ne smije prikazivati primjerke koji ne pripadaju odabranoj knjizi

<br>

---

### Naziv: Pregled pojedinačnih statusa primjeraka knjiga
### US-23: Kao bibliotekar ili admin, želim da vidim status svakog primjerka knjige (dostupan, posuđen), kako bih znao njegovo trenutno stanje.
**Acceptance Criteria:**
- Kada primjerak knjige postoji u sistemu, tada mora imati definisan status
- Kada se status primjerka promijeni, tada sistem mora ažurirati prikaz statusa
- Kada korisnik pregleda primjerke, tada sistem prikazuje trenutni status svakog primjerka
- Sistem ne smije dozvoliti nevalidne statuse primjeraka

---

### Naziv: Deaktivacija pojedinačnog primjerka
### US-24: Kao bibliotekar ili admin, želim deaktivirati oštećen ili izgubljen primjerak knjige, kako bi stanje fonda bilo tačno.
**Acceptance Criteria:**
- Član osoblja može otvoriti listu primjeraka knjige.
- Za primjerak postoji opcija “Deaktiviraj”.
- Deaktivirani primjerak se više ne računa kao dostupan za zaduživanje.
- Sistem ne dozvoljava deaktivaciju primjerka koji je trenutno aktivno zadužen.
  
<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava precizno upravljanje fizičkim primjercima knjiga u biblioteci, što poboljšava kontrolu nad dostupnošću, zaduživanjem i evidencijom bibliotečkog fonda. |
| **Pretpostavke / Otvorena pitanja** | Sistem ima definisan entitet Knjiga i Primjerak. <br> Da li se primjerci kreiraju automatski prilikom dodavanja knjige ili ručno? |
 **Veze i zavisnosti** | PB-16: Implementirano dodavanje knjige. <br> PB-27 Brisanje knjige i deaktivacija primjerka.|
---

<br>

## PB-27: Brisanje knjige i deaktivacija primjerka

### Naziv: Mogućnost brisanja
### US-25: Kao bibliotekar ili admin, želim da mogu obrisati knjigu iz sistema, kako bi katalog sadržavao samo relevantne i dostupne knjige.
**Acceptance Criteria:**
- Kada bibliotekar ili admin odabere knjigu, tada sistem prikazuje dugme "Obriši knjigu" za brisanje knjige
- Kada korisnik potvrdi brisanje knjige, ako nema zaduženih primjeraka, tada se knjiga uklanja iz sistema
- Kada je knjiga obrisana, tada više nije vidljiva u katalogu
- Sistem mora prikazati potvrdu prije izvršenja brisanja

<br>

---
### Naziv: Potvrda prije brisanja knjige
### US-26: Kao bibliotekar ili admin, želim dobiti potvrdu prije brisanja knjige, kako bih spriječio slučajno uklanjanje podataka.
**Acceptance Criteria:**
- Klik na "Obriši knjigu" otvara dijalog potvrde.
- Dijalog jasno navodi koju knjigu korisnik briše.
- Akcija se izvršava tek nakon potvrde.
- Otkazivanje zatvara dijalog bez promjene podataka.

<br>

---
### Naziv: Logičko uklanjanje knjige iz kataloga
### US-27: Kao sistem, želim da obrisana knjiga više ne bude vidljiva korisnicima, kako bi katalog ostao tačan.
**Acceptance Criteria:**
- Nakon brisanja knjiga nije vidljiva u katalogu.
- Knjiga se ne pojavljuje u rezultatima pretrage.
- Direktan pokušaj otvaranja detalja te knjige prikazuje poruku da nije dostupna.
- Ako se koristi soft delete, zapis ostaje interno označen kao neaktivan.
<br>

---

### Naziv: Nemougćnost brisanja uz aktivno zaduženje
### US-28: Kao bibliotekar ili admin, želim da ne mogu obrisati knjigu ako postoji aktivno zaduženje, kako bi se spriječio gubitak podataka.
**Acceptance Criteria:**
- Kada knjiga ima aktivno zaduženje, tada sistem ne dozvoljava brisanje
- Kada korisnik pokuša obrisati zaduženu knjigu, tada se prikazuje poruka o grešci
- Kada ne postoji aktivno zaduženje, tada je brisanje omogućeno
- Sistem mora provjeriti status svih primjeraka prije brisanja knjige

<br>

---
### Naziv: Ažurnost kataloga
### US-29: Kao korisnik sistema, želim da se promjene odmah reflektuju u katalogu, kako bih uvijek imao tačne informacije o dostupnim knjigama.
**Acceptance Criteria:**
- Kada se knjiga obriše, tada se odmah uklanja iz kataloga
- Kada korisnik osvježi stranicu kataloga, tada obrisana knjiga više nije vidljiva

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava održavanje tačnosti i konzistentnosti bibliotečkog fonda, sprječava prikaz zastarjelih ili nevažećih podataka i osigurava da korisnici imaju ažuran pregled dostupnih knjiga. <br> Smanjuje rizik od slučajnog brisanja. |
| **Pretpostavke / Otvorena pitanja** | Knjige su povezane sa primjercima u sistemu. <br> Da li se koristi hard delete ili soft delete. <br> UI podržava confirm dijalog.|
 **Veze i zavisnosti** | PB-16: Implementirano dodavanje knjige. <br> PB-26: Upravljanje primjercima knjige. <br> PB-28 Pregled kataloga <br> PB-29 Pretraga knjiga|

---

<br>

## PB-25: Upravljanje kategorijama knjiga
### Naziv: Dodavanje nove kategorije
### US-30: Kao bibliotekar ili admin, želim da mogu dodati novu kategoriju knjiga u sistem, kako bi se knjige mogle pravilno organizovati u katalogu.
**Acceptance Criteria:**
- Kada član osoblja uđe u sekciju "Kategorije", tada sistem prikazuje listu kategorija
- Kada član osoblja klikne na dugme "Dodaj kategoriju", tada se otvara forma za unos
- Kada član osoblja unese naziv i klikne "Sačuvaj", tada se kategorija sprema u sistem
- Kada je kategorija uspješno dodana, tada se prikazuje u listi
- Kada kategorija već postoji, tada sistem prikazuje poruku o grešci


<br>

---

### Naziv: Prikaz liste postojećih kategorija
### US-31: Kao bibliotekar ili admin, želim vidjeti sve postojeće kategorije na jednom mjestu, kako bih njima mogao lakše upravljati.
**Acceptance Criteria:**
- Sekcija "Kategorije" prikazuje listu svih kategorija.
- Lista prikazuje naziv svake kategorije.
- Ako nema nijedne kategorije, prikazuje se odgovarajuća poruka.
- Svaka kategorija ima dostupne akcije uredi i obriši.

<br>

---

### Naziv: Zabrana brisanja kategorije koja je u upotrebi
### US-32: Kao bibliotekar ili admin, želim da sistem spriječi brisanje kategorije koja je povezana s knjigama, kako se ne bi narušila konzistentnost podataka.
**Acceptance Criteria:**
- Kada kategorija ima povezane knjige, sistem ne dozvoljava brisanje.
- Prikazuje se jasna poruka zašto brisanje nije dozvoljeno.
- Kada kategorija nema povezane knjige, brisanje je moguće.
- Sistem ne ostavlja knjige bez validne kategorije ako je ona obavezna.

<br>

---

### Naziv: Uređivanje pojedinačnih kategorija
### US-33: Kao bibliotekar ili admin, želim da mogu izmijeniti naziv kategorije, kako bi podaci u sistemu bili tačni i ažurni.
**Acceptance Criteria:**
- Kada član osoblja uđe u sekciju "Kategorije", tada sistem prikazuje listu kategorija
- Kada član osoblja klikne "Uredi" pored kategorije, tada se otvara forma sa postojećim podacima
- Kada član osoblja izmijeni naziv i klikne "Sačuvaj", tada se izmjena sprema
- Kada je naziv prazan, tada sistem prikazuje grešku
- Kada naziv već postoji, tada sistem odbija izmjenu

<br>

---
### Naziv: Brisanje kategorije iz sistema
### US-34: Kao bibliotekar ili admin, želim da mogu obrisati kategoriju iz sistema, kako bih održavao uredan katalog knjiga.
**Acceptance Criteria:**
- Kada član osoblja uđe u sekciju "Kategorije", tada sistem prikazuje listu kategorija
- Kada član osoblja klikne "Obriši" na željenu kategoriju, tada se prikazuje potvrda brisanja
- Kada član osoblja potvrdi brisanje, tada se kategorija briše iz sistema
- Kada je kategorija obrisana, tada se više ne prikazuje u listi

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** |  Omogućava strukturirano upravljanje kategorijama knjiga, što poboljšava organizaciju kataloga, pretragu knjiga i konzistentnost podataka u sistemu. |
| **Pretpostavke / Otvorena pitanja** | Sistem ima admin panel sa sekcijom "Kategorije". <br> Kategorije se koriste pri dodavanju i uređivanju knjiga. <br>  Knjige su povezane sa kategorijama. |
 **Veze i zavisnosti** | Implementirano dodavanje i uređivanje knjiga, kao i pregled kataloga. <br> PB-22 Dodavanje nove knjige, PB-23 Uređivanje podataka o knjizi. |