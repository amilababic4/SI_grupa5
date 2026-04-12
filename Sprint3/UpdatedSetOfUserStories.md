# Set of User Stories - ažurirana verzija

## Opis dokumenta

Ovaj dokument predstavlja ažuriranu i detaljnije razrađenu verziju User Storyja za projekat Bibliotečkog informacionog sistema. Stavke iz Product Backloga su razrađene u više jasno definisanih User Story jedinica, pri čemu svaki User Story precizno opisuje korisničke zahtjeve, prihvatne kriterije i očekivano ponašanje sistema. Dokument je organizovan prema planiranim sprintovima radi bolje preglednosti i praćenja implementacije.


# Sprint 5

## PB-18: Kreiranje naloga člana

### Naziv: Prikaz forme za kreiranje člana
### US-01: Kao bibliotekar, želim unijeti osnovne podatke novog člana kroz formu za registraciju, kako bi član bio evidentiran u sistemu biblioteke.
**Acceptance Criteria:**
- Kada bibliotekar pristupi stranici za registraciju člana, tada sistem mora prikazati formu za unos podataka 
- Sistem mora omogućiti unos imena, prezimena, email adrese i lozinke
- Kada bibliotekar popuni sva obavezna polja i klikne na dugme "Kreiraj nalog", tada sistem nastavlja proces registracije
- Sistem ne smije dozvoliti nastavak registracije bez unosa obaveznih podataka

---
### Naziv: Unos ispravnih podataka
### US-02: Kao bibliotekar, želim provjeriti ispravnost unesenih podataka prilikom registracije člana, kako bi se spriječio unos pogrešnih ili nepotpunih informacija.
**Acceptance Criteria:**
- Kada bibliotekar unese podatke u formu za registraciju, tada se provjerava da li su sva obavezna polja popunjena
- Kada email adresa nije u ispravnom formatu, tada se prikazuje poruka o grešci
- Kada unesena lozinka ima manje od 8 znakova, tada se prikazuje poruka da lozinka nije dovoljno duga
- Kada email adresa već postoji u sistemu, tada se prikazuje poruka da je ta email adresa već registrovana
---

### Naziv: Potvrda uspješnog kreiranja člana
### US-03: Kao bibliotekar, želim da se nakon unosa ispravnih podataka novi član registruje u sistemu, kako bi mogao koristiti usluge biblioteke
**Acceptance Criteria:**
- Kada su svi podaci ispravno uneseni, tada se novi član uspješno registruje u sistemu
- Nakon uspješne registracije, sistem mora prikazati potvrdu da je nalog kreiran
- Novi član se pojavljuje u listi članova biblioteke
- Novom korisniku se automatski dodjeljuje uloga Član

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava evidentiranje novih članova u sistemu biblioteke i predstavlja osnovu za korištenje svih bibliotečkih usluga. |
| **Pretpostavke / Otvorena pitanja** | Član fizički dolazi u biblioteku i daje svoje podatke osoblju. <br> Bibliotekar unosi podatke u sistem. <br> Email je jedinstveni identifikator naloga.  |
| **Veze i zavisnosti** | Postojanje entiteta Korisnik u bazi podataka. <br> PB-17 Sistem prijave korisnika. <br> PB-21 Pregled i pretraga članova biblioteke.  |
---
<br>


## PB-17: Sistem prijave korisnika

### Naziv: Validacija obaveznih polja pri prijavi
### US-04: Kao registrovani korisnik, želim da se prijavim u sistem unosom email adrese i lozinke, kako bih pristupio funkcionalnostima sistema.
**Acceptance Criteria:**
- Kada korisnik pristupi stranici za prijavu, tada sistem prikazuje formu za unos emaila i lozinke
- Kada korisnik unese ispravne podatke i klikne na dugme "Prijava", tada se uspješno prijavljuje
- Kada je prijava uspješna, tada se korisnik preusmjerava na odgovarajući dashboard prema ulozi
- Kada podaci nisu ispravni, tada se prikazuje poruka o grešci
- Sistem ne smije omogućiti pristup aplikaciji bez uspješne prijave
---

### Naziv: Obavijest o neuspješnoj prijavi
### US-05: Kao korisnik sistema, želim dobiti jasnu informaciju kada prijava ne uspije, kako bih znao da trebam ponovo unijeti podatke.
**Acceptance Criteria:**
- Kada korisnik unese pogrešan email ili lozinku, tada se prijava odbija
- Kada prijava ne uspije, tada se prikazuje poruka o grešci
- Sistem ne prikazuje da li je greška u emailu ili u lozinci
- Korisnik može ponovo pokušati prijavu 
---

### Naziv: Odjava korisnika iz trenutne sesije
### US-06: Kao prijavljeni korisnik, želim se odjaviti iz sistema kako bih spriječio neovlašten pristup svom nalogu. 
**Acceptance Criteria:**
- Kada je korisnik prijavljen, tada je opcija "Odjava" dostupna u navigaciji
- Kada korisnik klikne na dugme "Odjava", tada se preusmjerava na stranicu za prijavu
- Nakon odjave sistem ne smije dozvoliti pristup zaštićenim stranicama bez ponovne prijave
---
### Naziv: Čuvanje sesije prijavljenog korisnika
### US-07: Kao prijavljeni korisnik, želim da ostanem prijavljen tokom korištenja sistema, kako ne bih morao stalno ponavljati prijavu.
**Acceptance Criteria:**
- Nakon uspješne prijave sistem kreira korisničku sesiju.
- Korisnik ostaje prijavljen prilikom kretanja između dozvoljenih stranica.
- Kada sesija istekne, korisnik se mora ponovo prijaviti.
- Nakon odjave sesija se briše.

---
### Naziv: Zaštita ruta za neprijavljene korisnike
### US-08: Kao sistem, želim blokirati pristup zaštićenim stranicama neprijavljenim korisnicima, kako bi podaci bili sigurni. 
**Acceptance Criteria:**
- Kada neprijavljeni korisnik pokuša pristupiti zaštićenoj stranici, sistem ga preusmjerava na prijavu.
- Neprijavljeni korisnik ne vidi sadržaj zaštićene stranice.
- Nakon uspješne prijave korisnik može pristupiti stranici u skladu sa ulogom.
- Direktan unos URL-a ne zaobilazi autentifikaciju.
---
### Naziv: Onemogućavanje prijave deaktiviranom korisniku
### US-09: Kao administrator, želim da deaktivirani korisnik ne može pristupiti sistemu, kako bi deaktivacija imala stvarni efekat.
**Acceptance Criteria:**
- Kada deaktivirani korisnik pokuša prijavu, sistem odbija pristup.
- Prikazuje se generička poruka da prijava nije uspjela.
- Sistem ne otkriva razlog blokade drugim korisnicima.
- Deaktivirani korisnik ne može pristupiti sistemu ni preko stare sesije.




<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava sigurnu autentifikaciju i kontrolu pristupa sistemu biblioteke u skladu sa korisničkim ulogama. Ova funkcionalnost predstavlja osnovu sistema jer bez prijave korisnici ne mogu pristupiti nijednoj funkcionalnosti stranice. <br> Smanjuje broj nepotrebnih zahtjeva prema backendu. |
| **Pretpostavke / Otvorena pitanja** | Korisnici već imaju kreirane naloge u sistemu. <br> Korisnici imaju definisane uloge: Član, Bibliotekar ili Administrator. <br> Definisan način čuvanja sesije. <br> Postoje zaštićene rute u aplikaciji. |
| **Veze i zavisnosti** | Zavisi od postojanja korisničkih naloga u bazi podataka. <br> PB-32 Upravljanje korisnicima od strane admina.|
---

<br>

## PB-19: Uspostava AI Usage Loga i Decision Loga

### Naziv: Standardizovani unos u AI Usage Log
### US-10: Kao tim, želimo kreirati AI Usage Log u okviru projekta, kako bismo evidentirali korištenje AI alata tokom razvoja i osigurali transparentnost rada.
**Acceptance Criteria:**
- Kada tim kreira projektnu dokumentaciju, tada se kreira AI Usage Log fajl u repozitoriju
- Log mora imati definisanu strukturu unosa (datum, alat, svrha korištenja, opis)
- Kada član tima koristi AI alat, tada je dužan evidentirati korištenje u logu
- Log mora biti dostupan svim članovima tima u repozitoriju
---
### Naziv: Standardizovani unos u Decision Log
### US-11: Kao tim, želimo voditi Decision Log kako bismo dokumentovali važne tehničke i arhitekturalne odluke tokom razvoja sistema.
**Acceptance Criteria:**
- Kada se donese važna tehnička odluka, tada se ona mora zapisati u Decision Log
- Decision Log mora sadržavati datum, opis odluke i obrazloženje
- Sve ključne odluke o arhitekturi i implementaciji moraju biti dokumentovane
- Log mora biti dostupan svim članovima tima u repozitoriju

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava transparentno praćenje razvoja projekta, dokumentovanje korištenja AI alata i tehničkih odluka, čime se povećava odgovornost tima i olakšava kasnija analiza i održavanje sistema. |
| **Pretpostavke / Otvorena pitanja** |  Svi članovi tima imaju pristup repozitoriju. <br> Da li postoji odgovorna osoba za provjeru logova? |
| **Veze i zavisnosti** | Definisana struktura projektne dokumentacije.  |
---

<br>

# Sprint 6

## PB-22: Dodavanje nove knjige

### Naziv: Prikaz i unos forme za dodavanje knjige
### US-12: Kao bibliotekar, želim unijeti podatke o novoj knjizi kroz formu za dodavanje, kako bi knjiga bila evidentirana u sistemu biblioteke.
**Acceptance Criteria:**
- Kada bibliotekar otvori formu za dodavanje knjige, tada sistem prikazuje polja za unos podataka
- Sistem mora omogućiti unos: naslov, autor, ISBN, godina izdanja, kategorija i broj primjeraka
- Kada korisnik klikne na "Sačuvaj", tada sistem provjerava ispravnost unesenih podataka
- Kada obavezni podaci nedostaju, tada sistem prikazuje poruku o grešci
- Bibliotekar može odustati bez spremanja.

 <br>

---

### Naziv: Validacija ISBN-a
### US-13: Kao bibliotekar, želim da sistem validira ISBN prije spremanja knjige, kako bi podaci u katalogu bili tačni.
**Acceptance Criteria:**
- Kada ISBN nije u prihvatljivom formatu, sistem prikazuje grešku.
- Kada ISBN već postoji, sistem odbija spremanje.
- Greška je vezana baš za ISBN polje.
- Nakon ispravke ISBN-a spremanje je moguće.

---

### Naziv: Dodjela početnog broja primjeraka
### US-14: Kao bibliotekar, želim da prilikom dodavanja knjige odredim početni broj primjeraka, kako bi katalog odmah prikazivao realno stanje fonda.
**Acceptance Criteria:**
- Forma sadrži polje za broj primjeraka.
- Dozvoljen je unos samo nenegativnog cijelog broja.
- Nakon spremanja knjige kreira se odgovarajući broj primjeraka.
- Ako je broj primjeraka 0, knjiga postoji u katalogu ali nije dostupna za zaduženje.

---

### Naziv: Dodjela kategorije pri unosu knjige
### US-15: Kao bibliotekar, želim izabrati kategoriju knjige pri unosu, kako bi knjiga odmah bila pravilno klasifikovana.
**Acceptance Criteria:**
- Forma za knjigu prikazuje listu postojećih kategorija.
- Bibliotekar može odabrati jednu kategoriju.
- Bez validne kategorije sistem ne sprema knjigu ako je kategorija obavezna.
- Odabrana kategorija se vidi u katalogu i detaljima knjige.

---
### Naziv:
### US-16: Kao bibliotekar, želim da se nakon uspješnog unosa knjiga automatski dodaje u katalog, kako bi bila dostupna korisnicima sistema.
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
### US-17: Kao bibliotekar, želim pristupiti postojećoj knjizi i izmijeniti njene osnovne podatke, kako bi informacije u katalogu bile tačne i ažurirane.
**Acceptance Criteria:**
- Kada bibliotekar odabere knjigu iz kataloga, tada sistem prikazuje trenutne podatke o knjizi
- Sistem mora omogućiti izmjenu sljedećih podataka: naslov, autor, godina izdanja i kategorija
- Kada korisnik izmijeni podatke i klikne na "Sačuvaj izmjene", tada se vrši validacija unosenih vrijednosti
- Sistem ne smije dozvoliti spremanje ako obavezni podaci nisu popunjeni

<br>

---
### Naziv: Validacija i potvrda o uspješnom uređivanju
### US-18: Kao bibliotekar, želim da se nakon uspješnog uređivanja podataka promjene sačuvaju i budu vidljive u katalogu, kako bi korisnici uvijek imali ažurne informacije.
**Acceptance Criteria:**
- Kada su uneseni podaci ispravni, tada se promjene uspješno spremaju u sistem
- Nakon spremanja, ažurirani podaci se odmah prikazuju u katalogu
- Kada podaci nisu ispravni, tada sistem prikazuje poruku o grešci i ne sprema promjene
- Samo bibliotekar ima pristup uređivanju knjiga

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
### US-21: Kao bibliotekar, želim da mogu dodati fizičke primjerke iste knjige u sistem, kako bih imao tačnu evidenciju dostupnog fonda.
**Acceptance Criteria:**
- Kada bibliotekar unosi novu knjigu, tada sistem omogućava unos broja primjeraka
- Kada postoji više primjeraka iste knjige, tada svaki primjerak mora biti zaseban zapis u sistemu
- Sistem ne smije dozvoliti kreiranje primjeraka bez povezane knjige

<br>

---

### Naziv: Pregled primjeraka
### US-22: Kao bibliotekar, želim da vidim sve primjerke jedne knjige, kako bih mogao pratiti njihov status i raspoloživost.
**Acceptance Criteria:**
- Kada korisnik otvori detalje knjige, tada sistem prikazuje listu svih primjeraka te knjige
- Kada knjiga ima više primjeraka, tada svaki primjerak mora biti jasno prikazan u listi
- Kada se prikazuju primjerci, tada moraju biti prikazani jedinstveni identifikatori svakog primjerka
- Sistem ne smije prikazivati primjerke koji ne pripadaju odabranoj knjizi

<br>

---

### Naziv: Pregled pojedinačnih statusa primjeraka knjiga
### US-23: Kao bibliotekar, želim da vidim status svakog primjerka knjige (dostupan, posuđen), kako bih znao njegovo trenutno stanje.
**Acceptance Criteria:**
- Kada primjerak knjige postoji u sistemu, tada mora imati definisan status
- Kada se status primjerka promijeni, tada sistem mora ažurirati prikaz statusa
- Kada korisnik pregleda primjerke, tada sistem prikazuje trenutni status svakog primjerka
- Sistem ne smije dozvoliti nevalidne statuse primjeraka

---

### Naziv: Deaktivacija pojedinačnog primjerka
### US-24: Kao bibliotekar, želim deaktivirati oštećen ili izgubljen primjerak knjige, kako bi stanje fonda bilo tačno.
**Acceptance Criteria:**
- Bibliotekar može otvoriti listu primjeraka knjige.
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
### US-25: Kao bibliotekar, želim da mogu obrisati knjigu iz sistema, kako bi katalog sadržavao samo relevantne i dostupne knjige.
**Acceptance Criteria:**
- Kada bibliotekar odabere knjigu, tada sistem prikazuje dugme "Obriši knjigu" za brisanje knjige
- Kada korisnik potvrdi brisanje knjige, ako nema zaduženih primjeraka, tada se knjiga uklanja iz sistema
- Kada je knjiga obrisana, tada više nije vidljiva u katalogu
- Sistem mora prikazati potvrdu prije izvršenja brisanja

<br>

---
### Naziv: Potvrda prije brisanja knjige
### US-26: Kao bibliotekar, želim dobiti potvrdu prije brisanja knjige, kako bih spriječio slučajno uklanjanje podataka.
**Acceptance Criteria:**
- Klik na “Obriši knjigu” otvara dijalog potvrde.
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
### US-28: Kao bibliotekar, želim da ne mogu obrisati knjigu ako postoji aktivno zaduženje, kako bi se spriječio gubitak podataka.
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
### US-30: Kao bibliotekar, želim da mogu dodati novu kategoriju knjiga u sistem, kako bi se knjige mogle pravilno organizovati u katalogu.
**Acceptance Criteria:**
- Kada korisnik uđe u sekciju "Kategorije", tada sistem prikazuje listu kategorija
- Kada korisnik klikne na dugme "Dodaj kategoriju", tada se otvara forma za unos
- Kada korisnik unese naziv i klikne "Sačuvaj", tada se kategorija sprema u sistem
- Kada je kategorija uspješno dodana, tada se prikazuje u listi
- Kada kategorija već postoji, tada sistem prikazuje poruku o grešci


<br>

---

### Naziv: Prikaz liste postojećih kategorija
### US-31: Kao bibliotekar, želim vidjeti sve postojeće kategorije na jednom mjestu, kako bih njima mogao lakše upravljati.
**Acceptance Criteria:**
- Sekcija “Kategorije” prikazuje listu svih kategorija.
- Lista prikazuje naziv svake kategorije.
- Ako nema nijedne kategorije, prikazuje se odgovarajuća poruka.
- Svaka kategorija ima dostupne akcije uredi i obriši.

<br>

---

### Naziv: Zabrana brisanja kategorije koja je u upotrebi
### US-32: Kao bibliotekar, želim da sistem spriječi brisanje kategorije koja je povezana s knjigama, kako se ne bi narušila konzistentnost podataka.
**Acceptance Criteria:**
- Kada kategorija ima povezane knjige, sistem ne dozvoljava brisanje.
- Prikazuje se jasna poruka zašto brisanje nije dozvoljeno.
- Kada kategorija nema povezane knjige, brisanje je moguće.
- Sistem ne ostavlja knjige bez validne kategorije ako je ona obavezna.

<br>

---

### Naziv: Uređivanje pojedinačnih kategorija
### US-33: Kao bibliotekar, želim da mogu izmijeniti naziv kategorije, kako bi podaci u sistemu bili tačni i ažurni.
**Acceptance Criteria:**
- Kada korisnik uđe u sekciju "Kategorije", tada sistem prikazuje listu kategorija
- Kada korisnik klikne "Uredi" pored kategorije, tada se otvara forma sa postojećim podacima
- Kada korisnik izmijeni naziv i klikne "Sačuvaj", tada se izmjena sprema
- Kada je naziv prazan, tada sistem prikazuje grešku
- Kada naziv već postoji, tada sistem odbija izmjenu

<br>

---
### Naziv: Brisanje kategorije iz sistema
### US-34: Kao bibliotekar, želim da mogu obrisati kategoriju iz sistema, kako bih održavao uredan katalog knjiga.
**Acceptance Criteria:**
- Kada korisnik uđe u sekciju "Kategorije", tada sistem prikazuje listu kategorija
- Kada korisnik klikne "Obriši" na željenu kategoriju, tada se prikazuje potvrda brisanja
- Kada korisnik potvrdi brisanje, tada se kategorija briše iz sistema
- Kada je kategorija obrisana, tada se više ne prikazuje u listi

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** |  Omogućava strukturirano upravljanje kategorijama knjiga, što poboljšava organizaciju kataloga, pretragu knjiga i konzistentnost podataka u sistemu. |
| **Pretpostavke / Otvorena pitanja** | Sistem ima admin panel sa sekcijom "Kategorije". <br> Kategorije se koriste pri dodavanju i uređivanju knjiga. <br>  Knjige su povezane sa kategorijama. |
 **Veze i zavisnosti** | Implementirano dodavanje i uređivanje knjiga, kao i pregled kataloga. <br> PB-22 Dodavanje nove knjige, PB-23 Uređivanje podataka o knjizi. |

---

<br>

# Sprint 7

## PB-29: Pretraga knjiga
### Naziv: Pretraga po ključnoj riječi
### US-35: Kao korisnik sistema, želim da mogu pretraživati knjige po naslovu, kako bih brzo pronašao željenu knjigu.
**Acceptance Criteria:**
- Kada korisnik uđe u katalog knjiga, tada sistem prikazuje polje za pretragu
- Kada korisnik unese naslov knjige u pretragu, i klikne na dugme "Pretraži" tada sistem filtrira rezultate
- Pretraga nije osjetljiva na velika i mala slova
- Ako nema rezultata, sistem prikazuje poruku da knjiga nije pronađena

<br>

---
### Naziv: Pretraga po autorima
### US-36: Kao korisnik sistema, želim da mogu pretraživati knjige po autoru, kako bih pronašao sve knjige određenog autora.
**Acceptance Criteria:**
- Kada korisnik uđe u katalog knjiga, tada sistem prikazuje polje za pretragu
- Kada korisnik unese ime autora u polje za pretragu, i klikne na dugme "Pretraži" tada sistem filtrira rezultate
- Kada postoji više knjiga istog autora, tada se prikazuju svi rezultati
- Pretraga nije osjetljiva na velika i mala slova
- Ako ne postoji autor u sistemu, tada se prikazuje poruka da nema rezultata
---

## Naziv: Reset pretrage
### US-37: Kao korisnik, želim očistiti aktivnu pretragu, kako bih se brzo vratio na puni katalog.
**Acceptance Criteria:**
- Nakon pretrage postoji opcija “Očisti”.
- Klik na “Očisti” vraća kompletnu listu knjiga.
- Polje za pretragu se prazni.
- Reset ne zahtijeva ručno osvježavanje stranice.

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** |  Omogućava brže i efikasnije pronalaženje knjiga u katalogu, čime se poboljšava korisničko iskustvo i smanjuje vrijeme pretrage. |
| **Pretpostavke / Otvorena pitanja** | Katalog knjiga već postoji i prikazuje sve knjige. <br> Knjige imaju definisan naslov i autora u sistemu. <br> Da li se pretraga treba proširiti i na kategorije? |
 **Veze i zavisnosti** | Implementiran katalog knjiga. |

---

<br>

## PB-24: Prikaz detalja knjige
### Naziv: Prikaz kompletnih bibliografskih podataka
### US-38: Kao član biblioteke, želim da mogu otvoriti stranicu sa detaljima knjige, kako bih vidio više informacija o knjizi.
**Acceptance Criteria:**
- Kada korisnik u katalogu klikne na karticu knjige, tada se otvara stranica sa detaljima knjige
- Kada se stranica otvori, tada sistem prikazuje osnovne informacije o knjizi
- Korisnik može otvoriti detalje samo klikom na određenu knjigu
- Sistem mora učitati podatke za izabranu knjigu

<br>

---
### Naziv: Obrada slučaja kada knjiga ne postoji
### US-39: Kao korisnik, želim dobiti jasnu poruku ako pokušam otvoriti nepostojeću knjigu, kako bih znao da zapis nije dostupan.
**Acceptance Criteria:**
- KKada tražena knjiga ne postoji, sistem prikazuje poruku o grešci ili “Knjiga nije pronađena”.
- Sistem ne prikazuje prazan ili pokvaren ekran.
- Korisniku je dostupna opcija povratka na katalog.
- Neispravan ID knjige ne ruši aplikaciju.

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** |  Omogućava korisnicima da dobiju detaljne informacije o knjizi prije zaduživanja, čime se smanjuje potreba za dodatnim upitima i poboljšava korisničko iskustvo. |
| **Pretpostavke / Otvorena pitanja** | Knjige već postoje u sistemu. <br> Svaka knjiga ima definisane osnovne podatke. |
 **Veze i zavisnosti** | Implementiran katalog knjiga. |

---

<br>

## PB-30: Pregled dostupnosti knjige

### Naziv: Jasan indikator dostupno/nedostupno
### US-40: Kao član biblioteke, želim vidjeti da li je knjiga dostupna ili zadužena, kako bih znao da li je mogu odmah posuditi.
**Acceptance Criteria:**
- Kada korisnik otvori detalje knjige, tada sistem prikazuje status dostupnosti (Dostupno / Zaduženo)
- Kada knjiga ima slobodne primjerke, tada se prikazuje status “Dostupno”
- Kada knjiga nema slobodnih primjeraka, tada se prikazuje status “Zaduženo”
- Status mora biti jasno vidljiv na stranici detalja knjige

<br>

---

### Naziv: Izračun ukupnog broja slobodnih primjeraka
### US-41: Kao sistem, želim izračunati broj slobodnih primjeraka knjige, kako bi član vidio stvarno stanje dostupnosti.
**Acceptance Criteria:**
- Sistem računa broj primjeraka sa statusom “Dostupan”.
- Zaduženi i deaktivirani primjerci se ne računaju kao dostupni.
- Broj dostupnih primjeraka se ažurira nakon zaduživanja i vraćanja.
- Ako nema dostupnih primjeraka, prikazuje se 0.

---
### Naziv: Broj dostupnih primjeraka
### US-42: Kao član biblioteke, želim vidjeti koliko je primjeraka knjige trenutno dostupno, kako bih znao mogu li je odmah uzeti.
**Acceptance Criteria:**
- Kada korisnik otvori detalje knjige, tada sistem prikazuje broj dostupnih primjeraka
- Kada su svi primjerci zaduženi, tada se prikazuje 0 dostupnih primjeraka
- Brojevi se prikazuju jasno i u čitljivom formatu na stranici detalja knjige

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** |  Omogućava članovima biblioteke jasan uvid u dostupnost knjiga i broj primjeraka, što poboljšava iskustvo pretrage i smanjuje nepotrebne pokušaje zaduživanja nedostupnih knjiga. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi evidenciju o ukupnom i dostupnom broju primjeraka knjiga. <br> Da li se rezervisani primjerci računaju kao nedostupni? |
 **Veze i zavisnosti** | Implementirano zaduživanje i vraćanje knjiga, kao i prikaz detalja knjige. |

---

<br>

# Sprint 8

## PB-25: Evidencija zaduživanja i vraćanja knjiga


### Naziv: Validacija prava člana na zaduživanje
### US-43: Kao bibliotekar, želim da sistem provjeri da li član ima aktivnu članarinu prije zaduživanja, kako bi zaduživanje bilo u skladu s pravilima biblioteke.
**Acceptance Criteria:**
- Kada bibliotekar odabere člana za zaduživanje, sistem provjerava status članarine.
- Ako članarina nije aktivna, zaduživanje nije dozvoljeno.
- Sistem prikazuje poruku da član nema pravo zaduženja.
- Ako je članarina aktivna, proces se može nastaviti.

<br>

---

### Naziv: Odabir članova i knjiga za zaduživanje
### US-44: Kao bibliotekar, želim evidentirati zaduživanje knjige tako što odaberem člana i primjerak knjige, kako bih znao koja knjiga je kod kojeg člana.
**Acceptance Criteria:**
- Kada bibliotekar otvori sekciju "Zaduživanja", tada sistem prikazuje opciju za novo zaduživanje 
- Kada bibliotekar klikne na “Novo zaduživanje”, tada se otvara forma za unos
- Kada bibliotekar odabere člana iz liste, tada sistem omogućava izbor knjige i dostupnog primjerka
- Kada bibliotekar klikne "Potvrdi zaduživanje", tada se kreira zapis o zaduženju
- Kada je zaduživanje uspješno, tada se primjerak označava kao "Zadužen"
- Sistem ne smije dozvoliti zaduživanje nedostupnog primjerka
- Nakon uspješnog zaduživanja, zapis je vidljiv u listi zaduženja člana

<br>

---

### Naziv: Evidentiranje vraćanja
### US-45: Kao bibliotekar, želim evidentirati vraćanje knjige kako bih ažurirao dostupnost primjerka u sistemu.
**Acceptance Criteria:**
- Kada bibliotekar otvori listu zaduženja, otvori mu se lista aktivnih zaduženja
- Kada bibliotekar odabere neko aktivno zaduženje, tada sistem prikazuje detalje zaduženja
- Kada bibliotekar klikne na dugme "Evidentiraj vraćanje", tada sistem traži potvrdu
- Kada se vraćanje potvrdi, tada se zaduženje označava kao završeno
- Kada je knjiga vraćena, tada se status primjerka mijenja u "Dostupan"
- Promjena je odmah vidljiva u katalogu i detaljima knjige

<br>

---

### Naziv: Automatsko postavljanje roka vraćanja
### US-46: Kao bibliotekar, želim da sistem automatski postavi rok vraćanja pri zaduživanju, kako bi evidencija bila standardizovana.
**Acceptance Criteria:**
- Kada se kreira zaduženje, sistem automatski generiše rok vraćanja.
- Rok vraćanja je vidljiv odmah nakon kreiranja zaduženja.
- Rok se čuva uz zapis zaduženja.
- Pravilo roka je isto za sve članove ako nije drugačije definisano.

<br>

---

### Naziv: Sprečavanje duplog aktivnog zaduženja istog primjerka
### US-47: Kao sistem, želim spriječiti da isti primjerak bude zadužen više puta istovremeno, kako bi evidencija ostala tačna.
**Acceptance Criteria:**
- Kada je primjerak već zadužen, ne može biti ponuđen za novo zaduživanje.
- Ako dođe do pokušaja duplog zaduženja, sistem odbija akciju.
- Status primjerka se provjerava neposredno prije potvrde.
- Nakon vraćanja primjerak se ponovo može zadužiti.

<br>

---

| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava tačno praćenje kretanja knjiga u biblioteci, sprječava gubitak primjeraka i omogućava ažurnu evidenciju zaduženja i vraćanja. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi evidenciju članova i knjiga.  <br> Definisan poslovni rok zaduženja. <br> Aktivna članarina je uslov za korištenje usluga. <br> Sistem čuva datumske podatke.|
 **Veze i zavisnosti** | PB-26 Upravljanje primjercima knjige. <br> Prikaz detalja knjige. <br> Pregled dostupnosti knjige. <br> PB-37 Pregled historije zaduženja. <br> PB-41, PB-47|

---

<br>

## PB-20: Pregled profila člana

### Naziv: Prikaz osnovnih podataka profila
### US-48: Kao registrovani korisnik (član, bibliotekar ili administrator), želim pregledati svoj profil, kako bih vidio osnovne podatke i zaduženja.
**Acceptance Criteria:**
- Kada korisnik klikne na dugme "Moj profil", tada sistem otvori stranicu sa detaljima korisnika
- Kada se otvori profil člana, sistem prikazuje: ime, prezime, email .
- Sistem mora prikazati jasno strukturirane osnovne podatke člana
- Sistem prikazuje trenutno posuđene knjige.

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava korisnicima i osoblju biblioteke da brzo pristupe informacijama o članovima i njihovim posuđenim knjigama, što olakšava upravljanje članstvom i praćenje zaduženja. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi evidenciju članova i njihovih zaduženja. <br> Član može vidjeti samo svoj profil. |
 **Veze i zavisnosti** | Domain Model (entitet o korisnicima i zaduženjima).<br> PB-17 Sistem prijave <br> PB-35 Pregled vlastitih zaduženja.  |

---

<br>

## PB-32: Upravljanje korisnicima od stranje admina

### Naziv: Pregled korisnika
### US-49: Kao administrator, želim vidjeti sve registrovane korisnike sistema, kako bih imao uvid u korisničku bazu.
**Acceptance Criteria:**
- Kada administrator otvori sekciju "Korisnici", tada sistem prikazuje listu svih korisnika
- Kada se lista učita, tada se prikazuju osnovni podaci: ime, prezime, email i uloga
- Kada postoji veliki broj korisnika, tada sistem omogućava scroll

<br>

--- 

### Naziv: Pretraga korisnika u admin sekciji
### US-50: Kao administrator, želim pretraživati korisnike po imenu ili emailu, kako bih brže pronašao željeni nalog.
**Acceptance Criteria:**
- Admin sekcija sadrži polje za pretragu.
- Pretraga radi po imenu, prezimenu ili emailu.
- Rezultati se filtriraju dinamički ili nakon potvrde.
- Ako nema rezultata, prikazuje se poruka “Nema rezultata”.

<br>

--- 

### Naziv: Izmjena uloga korisnika
### US-51: Kao administrator, želim promijeniti ulogu korisnika kako bih upravljao pristupom funkcionalnostima sistema.
**Acceptance Criteria:**
- Kada administrator klikne na korisnika iz liste, tada se otvara detaljni prikaz korisnika
- Kada administrator odabere opciju "Promijeni ulogu", tada se prikazuje dropdown sa ulogama
- Kada administrator potvrdi promjenu, tada se nova uloga sprema u sistem
- Nakon promjene, korisnik odmah ima novu ulogu pri sljedećem pristupu sistemu

<br>

---
### Naziv: Deaktivacija korisnika
### US-52: Kao administrator, želim deaktivirati korisnika kako bih onemogućio njegov dalji pristup sistemu.
**Acceptance Criteria:**
- Kada administrator otvori profil korisnika, tada postoji opcija "Deaktiviraj nalog"
- Kada administrator klikne na deaktivaciju, tada sistem traži potvrdu akcije
- Kada se nalog deaktivira, tada korisnik više ne može pristupiti sistemu
- Sistem ne smije dozvoliti prijavu deaktiviranom korisniku


<br>

---

### Naziv: Zaštita od deaktivacije vlastitog admin naloga
### US-53: Kao administrator, želim da sistem spriječi slučajnu deaktivaciju mog vlastitog naloga, kako ne bih izgubio pristup administraciji.
**Acceptance Criteria:**
- Administrator ne može deaktivirati trenutno prijavljeni vlastiti nalog.
- Ako pokuša, sistem prikazuje poruku o zabrani.
- Drugi administratorski nalozi se mogu deaktivirati prema pravilima.
- Sistem ne ostavlja aplikaciju bez aktivnog administratora ako je to poslovno pravilo.


<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava administratoru potpunu kontrolu nad korisničkim nalozima, čime se osigurava sigurnost sistema, pravilna raspodjela uloga i održavanje ažurne baze korisnika. |
| **Pretpostavke / Otvorena pitanja** | Samo administrator ima pristup upravljanju korisnicima. <br>  Korisnici su već registrovani u sistemu. |
 **Veze i zavisnosti** | Korisnik je registrovan. <br> Pregled profila člana.  |

---

<br>

## PB-37: Pregled historije zaduženja

### Naziv: Pregled prethodnih zaduženja
### US-54: Kao bibliotekar, želim pregledati ranija zaduženja člana, kako bih mogao pratiti korištenje fonda i donositi odluke o zaduživanju ili opomenama.
**Acceptance Criteria:**
- Kada bibliotekar otvori sekciju "Historija zaduženja" na profilu člana, tada sistem prikazuje listu svih ranijih zaduženja
- Kada postoje zaduženja, tada se prikazuje naziv knjige, primjerak, datum zaduženja i datum vraćanja
- Kada član nema historiju zaduženja, tada sistem prikazuje poruku "Nema historije zaduženja"
- Sistem mora prikazati sve zapise bez aktivnih zaduženja

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava uvid u prethodna zaduženja članova biblioteke, što pomaže bibliotekarima u praćenju ponašanja korisnika, analizi korištenja fonda i donošenju odluka o budućim zaduženjima. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi kompletnu evidenciju svih zaduženja i vraćanja. <br> Da li član može vidjeti svoju historiju ili samo bibliotekar? |
 **Veze i zavisnosti** | PB-31 Evidencija zaduživanja i vraćanja. <br> PB-20 Pregled profila člana. |

---

<br>

## PB-33: Upravljanje statusom članarine

### Naziv: Pregled statusa članarina
### US-55: Kao bibliotekar, želim otvoriti profil člana i vidjeti status njegove članarine kako bih znao da li ima pravo korištenja usluga biblioteke.
**Acceptance Criteria:**
- Kada bibliotekar otvori profil člana, tada sistem prikazuje sekciju "Članarina"
- Kada postoji članarina, tada se prikazuje datum isteka članarine, tj. do kad važi članarina.
- Kada članarina ne postoji, tada se prikazuje poruka "Članarina nije evidentirana"
- Status članarine mora biti jasno vidljiv na profilu člana

<br>

---

### Naziv: Evidentiranje nove članarine
### US-56: Kao bibliotekar, želim evidentirati novu članarinu za člana, kako bi sistem znao da član ima pravo korištenja usluga.
**Acceptance Criteria:**
- Bibliotekar može otvoriti formu za članarinu sa profila člana.
- Forma sadrži datum početka i datum isteka.
- Nakon spremanja, članarina je vidljiva na profilu.
- Ako su datumi neispravni, spremanje nije dozvoljeno.
- 
<br>

---

### Naziv: Ažuriranje postojeće članarine
### US-57: Kao bibliotekar, želim produžiti ili ispraviti postojeću članarinu člana kako bih osigurao tačne informacije o njenom važenju.
**Acceptance Criteria:**
- Kada bibliotekar klikne na opciju "Upravljanje članarinom", tada se otvara forma
- Kada bibliotekar unese datum početka i datum isteka, tada sistem sprema članarinu
- Sistem mora validirati da datum isteka nije prije datuma početka
- Nakon spremanja, izmjene su odmah vidljive na profilu člana

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava bibliotekarima da kontrolišu pristup uslugama biblioteke kroz praćenje i upravljanje članarinama. |
| **Pretpostavke / Otvorena pitanja** | Bibliotekar ili administrator je prijavljen u sistem i član već postoji u evidenciji. |
 **Veze i zavisnosti** | PB-14 Pregled profila člana. <br>
PB-26 Upravljanje korisnicima. <br> PB-20 Profil člana|

---

<br>

## PB-34: Pregled statusa članarina za člana

### Naziv:
### US-58: Kao član biblioteke, želim otvoriti svoj profil i vidjeti da li mi je članarina aktivna kako bih znao da li mogu koristiti usluge biblioteke.
**Acceptance Criteria:**
- Kada se član prijavi u sistem, tada može otvoriti sekciju "Moj profil"
- Kada član otvori profil, tada sistem prikazuje status članarine
- Kada je članarina aktivna, tada se prikazuje status "Aktivna"
- Kada je članarina istekla, tada se prikazuje status "Istekla"
- Status mora biti jasno vidljiv na profilu člana

<br>

---

### Naziv:
### US-59: Kao član biblioteke, želim vidjeti do kada važi moja članarina kako bih znao kada je potrebno produženje.
**Acceptance Criteria:**
- Kada član otvori svoj profil, tada sistem prikazuje datum isteka članarine
- Kada datum isteka postoji, tada se prikazuje u jasnom formatu (DD-MM-YYYY)
- Kada članarina ne postoji, tada se prikazuje poruka da članarina nije aktivna
- Datum isteka mora biti vidljiv zajedno sa statusom članarine

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava članovima biblioteke jasan uvid u status i trajanje članarine, čime se povećava transparentnost i smanjuje potreba za kontaktiranjem bibliotekara. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi evidenciju članarine sa datumom isteka. |
 **Veze i zavisnosti** | PB-33 Upravljanje statusom članarine. <br> PB-20 Pregled profila člana. |

---

<br>

## PB-38: Početno testiranje sistema

### Naziv: Testiranje funkcionalnosti
### US-60: Kao član tima, želim testirati implementirane funkcionalnosti sistema kako bih provjerio da li sistem radi ispravno u osnovnim scenarijima.
**Acceptance Criteria:**
- Testiranje uključuje provjeru prijave, pregleda kataloga i zaduživanja knjiga
- Svaka funkcionalnost mora imati definisan očekivani rezultat
- Rezultati testiranja se dokumentuju u obliku liste uspješnih i neuspješnih testova

<br>

---

### Naziv: Integracijsko testiranje
### US-61: Kao član tima, želim provjeriti da li različiti dijelovi sistema pravilno rade zajedno kako bih osigurao stabilan rad aplikacije.
**Acceptance Criteria:**
- Kada se testira sistem, tada se provjerava povezanost između modula (korisnici, knjige, zaduživanja)
- Sistem mora ispravno prenositi podatke između povezanih funkcionalnosti
- Ako postoji greška u integraciji, tada se ona evidentira 
- Testiranje mora obuhvatiti minimalno jedan kompletan korisnički tok

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava rano otkrivanje grešaka i problema u sistemu, smanjuje rizik od ozbiljnih grešaka u kasnijim fazama razvoja i poboljšava ukupni kvalitet softvera. |
| **Pretpostavke / Otvorena pitanja** | Sve osnovne funkcionalnosti su implementirane prije testiranja. <br> Tim ima definisane test scenarije. <br> Da li se koriste automatizovani ili ručni testovi? |
 **Veze i zavisnosti** | Zavisi od implementacije svih user storija do sprinta 9. |

---

<br>

# Sprint 9

## PB-35: Pregled vlastitih zaduženja

### Naziv: Prikaz detalja svakog aktivnog zaduženja
### US-62: Kao član biblioteke, želim vidjeti sve knjige koje trenutno imam zadužene, kako bih imao pregled svojih obaveza. 
**Acceptance Criteria:**
- Kada je korisnik prijavljen u sistem, tada može otvoriti sekciju "Moja zaduženja". 
- Kada korisnik otvori ovu sekciju, tada se prikazuje lista svih aktivnih zaduženja. 
- Za svako zaduženje prikazuje se naziv knjige i datum zaduživanja. 
- Ako član nema aktivnih zaduženja, tada se prikazuje poruka "Nema aktivnih zaduženja". 
- Sistem ne smije prikazivati zaduženja drugih korisnika.

<br>

--- 

### Naziv: Pregled roka vraćanja
### US-63: Kao član biblioteke, želim vidjeti rok vraćanja za svaku zaduženu knjigu, kako bih mogao planirati vraćanje na vrijeme. 
**Acceptance Criteria:**
- Kada je korisnik prijavljen u sistem, tada može otvoriti sekciju "Moja zaduženja". 
- Kada korisnik otvori ovu sekciju, tada se prikazuje lista svih aktivnih zaduženja.
- Kada član pregleda listu zaduženja, tada za svaku knjigu vidi i rok vraćanja. 
- Kada se rok vraćanja približava, sistem jasno označava zaduženje (vizualno isticanje). 
- Kada je knjiga vraćena, ona se više ne prikazuje u aktivnim zaduženjima. 
- Podaci o rokovima moraju biti tačni i ažurirani u realnom vremenu.

<br>

---

### Naziv: Vizualno označavanje zakašnjelih zaduženja
### US-64: Kao član biblioteke, želim da zakašnjela zaduženja budu jasno označena, kako bih odmah znao da kasnim sa vraćanjem.
**Acceptance Criteria:**
- Kada je rok vraćanja prošao, zaduženje je vizualno označeno kao zakašnjelo.
- Kada rok još nije prošao, takvo označavanje se ne prikazuje.
- Oznaka je vidljiva bez otvaranja dodatnih detalja.
- Prikaz je usklađen sa stvarnim datumom roka.

<br>

---

| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava članovima jasan pregled trenutnih zaduženja i rokova vraćanja, što smanjuje kašnjenja i rasterećuje bibliotekare. |
| **Pretpostavke / Otvorena pitanja** | Zaduženja i rokovi su već evidentirani u sistemu.  |
 **Veze i zavisnosti** | PB-31 Evidencija zaduživanja i vraćanja knjiga. <br> PB-41 Slanje email upozorenja, PB-47 Kazne|

---

<br>

## PB-36: Pregled trenutnih zaduženja

### Naziv: Pregled aktivnih zaduženja po članovima
### US-65:  Kao bibliotekar, želim vidjeti sva trenutno aktivna zaduženja u sistemu, kako bih mogao pratiti koje knjige su trenutno posuđene i kod kojih članova.
**Acceptance Criteria:**
- Kada je bibliotekar prijavljen u sistem, tada može otvoriti sekciju "Aktivna zaduženja". 
- Kada se sekcija otvori, tada se prikazuje lista svih aktivnih zaduženja. 
- Za svako zaduženje prikazuju se: ime i email člana, naziv knjige, datum zaduživanja i rok vraćanja. 
- Ako ne postoje aktivna zaduženja, tada se prikazuje poruka "Nema aktivnih zaduženja".

<br>

---

### Naziv: Filtriranje aktivnih zaduženja po članu
### US-66:  Kao bibliotekar, želim filtrirati aktivna zaduženja po članu, kako bih brže pronašao zaduženja određene osobe.
**Acceptance Criteria:**
- Kada je bibliotekar prijavljen u sistem, tada može otvoriti sekciju "Aktivna zaduženja". 
- Kada se sekcija otvori, tada se prikazuje lista svih aktivnih zaduženja. 
- Za svako zaduženje prikazuju se: ime i email člana, naziv knjige, datum zaduživanja i rok vraćanja. 
- Ako ne postoje aktivna zaduženja, tada se prikazuje poruka "Nema aktivnih zaduženja".

<br>

---

### Naziv: Otvaranje detalja aktivnog zaduženja
### US-67:  Kao bibliotekar, želim otvoriti detalje aktivnog zaduženja, kako bih mogao brzo vidjeti kome pripada i koji primjerak je u pitanju.
**Acceptance Criteria:**
- Klik na zapis zaduženja otvara detaljni prikaz.
- Detalji prikazuju člana, knjigu, primjerak, datum zaduženja i rok vraćanja.
- Iz detalja je moguće evidentirati vraćanje ako je dozvoljeno.
- Podaci odgovaraju tačno odabranom zaduženju.

<br>

---

### Naziv: Sortirani pregled zaduženja
### US-68: Kao bibliotekar, želim da aktivna zaduženja budu sortirana po roku vraćanja, kako bih lakše identifikovao knjige koje treba uskoro vratiti.
**Acceptance Criteria:**
- Kada se prikazuje lista aktivnih zaduženja, tada su ona sortirana po roku vraćanja (najbliži rok prvi). 
- Kada se novo zaduženje doda ili knjiga vrati, tada se lista ažurira. 
- Sistem mora uvijek prikazivati ažuran redoslijed zaduženja.

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava bibliotekaru bolji pregled aktivnih zaduženja i efikasnije upravljanje bibliotečkim fondom, posebno u praćenju rokova vraćanja knjiga.  |
| **Pretpostavke / Otvorena pitanja** | Bibliotekar je prijavljen u sistem. <br> Zaduženja i korisnici su već evidentirani u bazi. <br> Da li dodati filtere (po članu, datumu ili knjizi)? |
 **Veze i zavisnosti** | PB-21 Pregled i pretraga članova biblioteke. <br> PB-31 Evidencija zaduživanja i vraćanja knjiga.  |

---

<br>

## PB-39: Rezervacija knjiga

### Naziv: Mogućnost rezervacije
### US-69: Kao član biblioteke, želim rezervisati knjigu koja trenutno nema dostupnih primjeraka, kako bih bio obaviješten kada knjiga ponovo postane dostupna.
**Acceptance Criteria:**
- Kada je član prijavljen u sistem, tada u katalogu knjiga može otvoriti detalje knjige. 
- Kada knjiga nema dostupnih primjeraka, tada je opcija "Rezerviši" aktivna. 
- Kada član klikne na "Rezerviši", tada se rezervacija uspješno kreira. 
- Sistem ne smije dozvoliti rezervaciju knjige koja ima dostupne primjerke. 
- Član ne može napraviti više aktivnih rezervacija iste knjige.

<br>

---
### Naziv: Kreiranje rezervacije sa datumom nastanka

### US-70: Kao sistem, želim sačuvati datum kreiranja rezervacije, kako bi bibliotekar i član mogli pratiti redoslijed rezervacija.
**Acceptance Criteria:**
- Svaka nova rezervacija ima zabilježen datum i vrijeme kreiranja.
- Datum rezervacije je vidljiv u listi rezervacija.
- Redoslijed rezervacija može se odrediti po vremenu kreiranja.
- Podatak se čuva uz rezervaciju tokom njenog životnog ciklusa.
<br>

---

### Naziv: Pregled vlastitih aktivnih rezervacija
### US-71: Kao član biblioteke, želim vidjeti svoje aktivne rezervacije, kako bih znao koje knjige čekam.
**Acceptance Criteria:**
- Član može otvoriti sekciju “Moje rezervacije”.
- Lista prikazuje samo rezervacije prijavljenog člana.
- Za svaku rezervaciju se prikazuje naslov knjige i datum rezervacije.
- Ako nema rezervacija, prikazuje se odgovarajuća poruka.

<br>

---

### Naziv: Otkazivanje rezervacije
### US-72: Kao član biblioteke, želim imati mogućnost otkazivanja rezervacije kako bih upravljao svojim aktivnim rezervacijama.
**Acceptance Criteria:**
- Kada član otvori sekciju "Moje rezervacije", tada vidi listu aktivnih rezervacija. 
- Kada član klikne na dugme "Otkaži rezervaciju" za neku knjigu, tada se rezervacija uklanja iz sistema. 
- Nakon otkazivanja, knjiga više nije prikazana u listi aktivnih rezervacija člana. 
- Sistem odmah ažurira status rezervacije nakon promjene.

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava članovima bolje planiranje posudbe knjiga i efikasnije korištenje bibliotečkog fonda, posebno za knjige koje su trenutno zauzete.  |
| **Pretpostavke / Otvorena pitanja** | Sistem podržava evidenciju rezervacija. <br> Koliko dugo rezervacija ostaje aktivna nakon što knjiga postane dostupna? |
 **Veze i zavisnosti** | Pregled dostupnosti knjiga. <br> PB-40 Pregled aktivnih rezervacija. <br> PB-39 Rezervacija knjige.|

---

<br>

## PB-40: Pregled aktivnih rezervacija

### Naziv: Mogućnost pregleda rezervacija
### US-73: Kao bibliotekar, želim vidjeti sve aktivne rezervacije knjiga u sistemu, kako bih imao pregled koje knjige su rezervisane i od strane kojih članova.
**Acceptance Criteria:**
- Kada je bibliotekar prijavljen u sistem, tada može otvoriti sekciju "Aktivne rezervacije".
- Kada se sekcija otvori, tada se prikazuje lista svih aktivnih rezervacija. 
- Za svaku rezervaciju prikazuju se: ime i prezime člana, email člana, naslov knjige i datum rezervacije. 
- Ako nema aktivnih rezervacija, tada se prikazuje poruka "Nema aktivnih rezervacija". 
- Sistem ne smije prikazivati otkazane ili realizovane rezervacije.

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava bibliotekarima bolju kontrolu i pregled nad rezervacijama, što poboljšava organizaciju posudbi i upravljanje fondom.  |
| **Pretpostavke / Otvorena pitanja** | Sistem podržava evidenciju rezervacija. <br> Da li će postojati historija rezervacija ili samo aktivne? |
 **Veze i zavisnosti** | Rezervacija knjiga. |

---

<br>

## PB-44: Napredna pretraga i filteri

### Naziv: Mogućnost filtriranja knjiga po kategorijama
### US-74: Kao član biblioteke, želim filtrirati knjige po kategoriji, kako bih lakše pronašao knjige iz određene oblasti.
**Acceptance Criteria:**
- Kada je korisnik na stranici kataloga knjiga, tada može izabrati filter "Kategorija". 
- Kada korisnik odabere kategoriju, tada se prikazuju samo knjige iz te kategorije. 
- Kada korisnik promijeni kategoriju, tada se lista knjiga ažurira. 
- Ako ne postoji nijedna knjiga u odabranoj kategoriji, tada se prikazuje poruka "Nema rezultata".

<br>

---

### Naziv: Mogućnost filtriranja knjiga po izdavaču
### US-75: Kao član biblioteke, želim filtrirati knjige po izdavaču, kako bih pronašao knjige određenog izdavača.
**Acceptance Criteria:**
- Kada je korisnik na katalogu knjiga , tada može izabrati filter "Izdavač". 
- Kada korisnik odabere izdavača, tada se prikazuju samo knjige tog izdavača.
- Lista se ažurira nakon izbora.
- Ako nema rezultata, prikazuje se poruka "Nema knjiga za odabranog izdavača".

<br>

---

### Naziv: Mogućnost filtriranja knjiga po godini izdanja
### US-76: Kao član biblioteke, želim filtrirati knjige po godini izdanja, kako bih pronašao novije ili starije knjige.
**Acceptance Criteria:**
- Kada je korisnik na katalogu knjiga , tada može izabrati filter "Godina izdanja". 
- Kada se unese godina, tada se prikazuju samo knjige iz te godine. 
- Lista se ažurira nakon promjene filtera.
- Ako nema rezultata, prikazuje se poruka "Nema knjiga za odabranu godinu".

<br>

---

### Naziv: Mogućnost kominacije filtera knjiga
### US-78: Kao član biblioteke, želim kombinovati više filtera istovremeno, kako bih preciznije pronašao željene knjige.
**Acceptance Criteria:**
- Kada korisnik odabere više filtera (kategorija, izdavač,ili godina), tada se prikazuju samo knjige koje zadovoljavaju sve uslove.
- Kada se bilo koji filter promijeni, tada se rezultat  ažurira. 
- Ako nijedna knjiga ne zadovoljava kombinovane filtere, tada se prikazuje poruka "Nema rezultata". 
- Sistem mora ispravno kombinovati sve aktivne filtere.

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava efikasnije pretraživanje velikog broja knjiga i poboljšava korisničko iskustvo kroz preciznije i brže pronalaženje željenog sadržaja.  |
| **Pretpostavke / Otvorena pitanja** | Katalog knjiga već postoji. <br> Podaci o kategoriji, izdavaču i godini su dostupni za svaku knjigu. |
 **Veze i zavisnosti** | Pregled kataloga knjiga. <br> Osnovna pretraga knjiga. |

---

<br>

## PB-43: Automatsko otkazivanje rezervacije

### Naziv: Definisanje roka važenja rezervacije
### US-79: Kao sistem, želim evidentirati i pratiti datum isteka rezervacije, kako bih znao kada rezervacija treba biti otkazana ako knjiga nije preuzeta.
**Acceptance Criteria:**
- Kada se rezervacija kreira, tada se upisuje datum isteka rezervacije. 
- Sistem mora čuvati informaciju o roku važenja svake aktivne rezervacije.
- Svaka rezervacija mora imati definisan vremenski period važenja. 

<br>

---

### Naziv: Oslobađanje knjige nakon isteka rezervacije
### US-80: Kao sistem, želim da po isteku rezervacije knjiga ponovo postane raspoloživa za druge procese, kako fond ne bi ostao blokiran.
**Acceptance Criteria:**
- Kada rezervacija istekne, njen status prelazi u "Otkazana".
- Otkazana rezervacija više se ne računa kao aktivna.
- Nakon otkazivanja knjiga može biti dostupna za novu rezervaciju ili zaduživanje prema pravilima.
- Promjena statusa je vidljiva bibliotekaru.

<br>

---

| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava automatsko upravljanje rezervacijama bez intervencije bibliotekara, povećava dostupnost knjiga i optimizuje korištenje bibliotečkog fonda.  |
| **Pretpostavke / Otvorena pitanja** | Definisan je rok važenja rezervacije. |
 **Veze i zavisnosti** | Rezervacija knjiga. |

---

<br>

# Sprint 10

## PB-41: Slanje email upozorenja

### Naziv: Slanje emaila samo korisnicima sa validnom email adresom
### US-81: Kao sistem, želim slati podsjetnike samo članovima koji imaju evidentiranu validnu email adresu, kako bih izbjegao neuspješna slanja.
**Acceptance Criteria:**
- Prije slanja sistem provjerava da član ima email adresu.
- Ako email ne postoji, poruka se ne šalje.
- Neuspjelo slanje se evidentira u sistemu ako postoji logging.
- Ostala validna slanja se izvršavaju normalno.

<br>

---

### Naziv: Zaustavljanje podsjetnika nakon vraćanja knjige
### US-82: Kao sistem, želim prestati slati podsjetnike kada je knjiga vraćena, kako član ne bi dobijao netačne poruke.
**Acceptance Criteria:**
- Kada je zaduženje završeno, sistem ga ne uključuje u buduća slanja podsjetnika.
- Knjige vraćene prije roka ne dobijaju podsjetnik na istek.
- Knjige vraćene nakon zakašnjenja ne dobijaju nove mailove nakon povrata.
- Slanje se zasniva samo na aktivnim zaduženjima.

<br>

---
### Naziv: Notifikacija članu o podsjetniku isteka roka vraćanja
### US-83: Kao član biblioteke, želim da dobijem email podsjetnik prije isteka roka vraćanja knjige kako bih je mogao na vrijeme vratiti.
**Acceptance Criteria:**
- Kada je rok vraćanja knjige 2 dana od isteka, tada sistem automatski šalje email podsjetnik
- Kada član ima više zaduženih knjiga, tada se šalje podsjetnik za svaku knjigu posebno
- Email mora sadržavati naziv knjige i datum roka vraćanja
- Mail se šalje samo za aktivna zaduženja

<br>

---

### Naziv: Notifikacija članu o upozorenju isteka roka vraćanja
### US-84: Kao član biblioteke, želim dobiti email upozorenje na dan kada mi ističe rok vraćanja knjige kako bih znao da trebam odmah vratiti knjigu.
**Acceptance Criteria:**
- Kada rok vraćanja knjige istekne, tada sistem automatski šalje email upozorenje
- Kada član ima više knjiga kojima ističe rok, tada se šalje upozorenje za svaku knjigu posebno
- Email mora sadržavati naziv knjige i datum isteka roka
- Upozorenje se šalje samo za knjige koje nisu vraćene

<br>

---

### Naziv: Podsjetnik o kašnjenju
### US-85: Kao član biblioteke, želim dobiti podsjetnik ako kasnim s vraćanjem knjige kako bih bio svjestan da trebam što prije vratiti knjigu.
**Acceptance Criteria:**
- Kada je knjiga zakasnila više od 1 dana, tada sistem šalje podsjetnik o kašnjenju
- Email mora sadržavati naziv knjige i broj dana kašnjenja
- Podsjetnici se zaustavljaju kada se knjiga vrati

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava pravovremeno obavještavanje članova o rokovima vraćanja knjiga, smanjuje broj kašnjenja i unapređuje upravljanje bibliotečkim fondom.  |
| **Pretpostavke / Otvorena pitanja** | Članovi imaju validne email adrese u sistemu. <br> Sistem podržava automatsko slanje emailova. |
 **Veze i zavisnosti** |PB-31 Evidencija zaduživanja i vraćanja knjiga. <br> Pregled vlastitih zaduženja. |

---

<br>

## PB-42: Obavještavanje bibliotekara o novoj rezervaciji

### Naziv: Slanje jedne notifikacije po rezervaciji
### US-86: Kao bibliotekar, želim da dobijem email obavijest svaki put kada član kreira rezervaciju knjige kako bih bio informisan o novim rezervacijama.
**Acceptance Criteria:**
- Kada član kreira novu rezervaciju, tada sistem automatski šalje email bibliotekaru
- Email mora sadržavati ime i prezime člana
- Email mora sadržavati naslov rezervisane knjige
- Email mora sadržavati datum kreiranja rezervacije
- Sistem šalje obavijest samo jednom po svakoj rezervaciji

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava bibliotekaru pravovremeno informisanje o novim rezervacijama, što poboljšava organizaciju rada i pripremu knjiga za korisnike.  |
| **Pretpostavke / Otvorena pitanja** | Sistem podržava automatsko slanje email obavijesti. |
 **Veze i zavisnosti** | Evidencija zaduživanja i vraćanja knjiga. |

---

<br>

## PB-45: Mjesečni izvještaji za upravu

### Naziv: Izbor mjeseca i godine za izvještaj
### US-87: Kao administrator, želim izabrati tačan mjesec i godinu za izvještaj, kako bih generisao pregled za željeni period.
**Acceptance Criteria:**
- Forma za izvještaj omogućava izbor mjeseca.
- Forma omogućava izbor godine.
- Bez izbora perioda nije moguće generisati izvještaj.
- Izvještaj odgovara odabranom periodu.

<br>

---
### Naziv: Mjesečni izvještaj o zaduživanjima knjiga
### US-88: Kao administrator biblioteke, želim generisati mjesečni izvještaj o zaduživanjima knjiga kako bih mogao pratiti korištenje bibliotečkog fonda.
**Acceptance Criteria:**
- Kada administrator odabere sekciju "Generiši izvještaj", otvara se forma o tipu izvještaja
- Kada administrator odabere "Izvještaj o  zaduživanjima", otvara se forma o periodu izvještaja
- Kada administartor odabere mjesec i godinu, tada sistem generiše izvještaj o zaduživanjima
- Izvještaj prikazuje broj ukupnih zaduženja u tom periodu
- Izvještaj prikazuje listu zaduženih knjiga i članova
- Ako nema podataka, sistem prikazuje poruku da nema zaduživanja za taj period

<br>

---
### Naziv: Mjesečni izvještaj o rezervacijama knjiga
### US-89: Kao administrator biblioteke, želim generisati mjesečni izvještaj o rezervacijama knjiga kako bih imao uvid u potražnju za knjigama.
**Acceptance Criteria:**
- Kada administrator odabere sekciju "Generiši izvještaj", otvara se forma o tipu izvještaja
- Kada administrator odabere "Izvještaj o  rezervacijama", otvara se forma o periodu izvještaja
- Kada administartor odabere mjesec i godinu, tada sistem generiše izvještaj o rezervacijama
- Izvještaj prikazuje broj aktivnih i završenih rezervacija
- Izvještaj prikazuje listu rezervisanih knjiga i članova
- Ako nema rezervacija, sistem prikazuje odgovarajuću poruku

<br>

---
### Naziv: Mjesečni izvještaj o članovima
### US-90: Kao administrator biblioteke, želim generisati mjesečni izvještaj o članovima kako bih pratio aktivnost i stanje članstva.
**Acceptance Criteria:**
- Kada administrator odabere sekciju "Generiši izvještaj", otvara se forma o tipu izvještaja
- Kada administrator odabere "Izvještaj o  članovima", otvara se forma o periodu izvještaja
- Kada administrator odabere mjesec i godinu, tada sistem generiše izvještaj o članovima
- Izvještaj prikazuje ukupan broj aktivnih članova

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava upravi biblioteke detaljan uvid u rad sistema, korištenje resursa i aktivnost članova, što pomaže u donošenju strateških odluka i planiranju razvoja biblioteke.  |
| **Pretpostavke / Otvorena pitanja** | Svi podaci o zaduženjima, rezervacijama i članovima su tačni i ažurirani. <br> Da li se izvještaji mogu eksportovati u PDF ili Excel? |
 **Veze i zavisnosti** | PB-25: Evidencija zaduživanja i vraćanja knjiga. <br> PB-24: Rezervacija knjiga. <br> PB-14: Pregled profila člana. <br> PB-26: Upravljanje korisnicima. |

---

<br>

## PB-46: Audit log promjena

### Naziv: Automatsko evidentiranje promjena
### US-91: Kao član osoblja, želim da sistem automatski evidentira svako dodavanje, izmjenu i brisanje knjiga kako bih mogao pratiti promjene u bibliotečkom fondu.
**Acceptance Criteria:**
- Kada se knjiga doda, izmijeni ili obriše, tada sistem automatski kreira audit zapis
- Audit zapis sadrži naziv akcije, datum i vrijeme promjene
- Audit zapis sadrži korisnika koji je izvršio promjenu
- Svi zapisi se čuvaju u sistemu i nisu dostupni za izmjenu

<br>

---
### Naziv: Bilježenje promjena korisničkih naloga
### US-92: Kao administrator, želim da sistem bilježi promjene nad korisničkim nalozima kako bih mogao pratiti sigurnost i aktivnosti korisnika.
**Acceptance Criteria:**
- Kada se korisnik kreira, izmijeni ili deaktivira, tada se kreira audit zapis
- Zapis sadrži vrstu promjene i korisnika koji je izvršio akciju
- Zapis sadrži datum i vrijeme promjene

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava transparentnost i sigurnost sistema kroz praćenje svih važnih promjena, olakšava dijagnostiku problema i reviziju aktivnosti korisnika. |
| **Pretpostavke / Otvorena pitanja** | Sve promjene se u sistemu čuvaju 30 dana. |
 **Veze i zavisnosti** | PB-25: Evidencija zaduživanja i vraćanja knjiga. <br> PB-24: Rezervacija knjiga. <br> PB-26: Upravljanje korisnicima. |

---

<br>

## PB-47: Kazne za kasno vraćanje knjiga
### Naziv: Evidentiranje kazne po zaduženju
### US-93: Kao sistem, želim automatski obračunati kaznu za svaku knjigu koja nije vraćena u predviđenom roku kako bi se osigurala disciplina i poštovanje pravila korištenja.
**Acceptance Criteria:**
- Kada je knjiga vraćena nakon isteka roka, tada sistem automatski obračunava kaznu po danu kašnjenja
- Kazna se računa za svaki dan kašnjenja
- Kazna se veže za konkretno zaduženje i člana

<br>

---

### Naziv: Prikaz ukupnog duga člana
### US-94: Kao član biblioteke, želim da mogu pregledati ukupne kazne kako bih bio informisan o svojim obavezama.
**Acceptance Criteria:**
- Kada član pristupi svom profilu, tada vidi pregled svih kazni
- Sistem prikazuje ukupni iznos kazne
- Ako član nema kazni, sistem prikazuje poruku da nema dugovanja
- Podaci o kaznama se ažuriraju nakon svakog vraćanja knjige

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava disciplinu u korištenju bibliotečkog fonda, smanjuje kašnjenja u vraćanju knjiga i obezbjeđuje dodatnu kontrolu nad zaduženjima. |
| **Pretpostavke / Otvorena pitanja** | Postoji definisana pravila obračuna kazni. |
 **Veze i zavisnosti** | PB-25: Evidencija zaduživanja i vraćanja knjiga. |

---

<br>

## PB-48: Online produžetak članarine

### Naziv: Mogućnost produžavanja
### US-95: Kao član biblioteke, želim da mogu pristupiti opciji za produženje članarine kako bih započeo proces produženja.
**Acceptance Criteria:**
- Kada je član prijavljen, tada na svom profilu može vidjeti opciju "Produži članarinu"
- Kada član klikne na tu opciju, tada se otvara stranica za produženje
- Sistem prikazuje trenutni status i datum isteka članarine

<br>

---

### Naziv: Izbor trajanja članarine
### US-96: Kao član biblioteke, želim da odaberem trajanje produženja članarine kako bih prilagodio period svojih potreba.
**Acceptance Criteria:**
- Sistem prikazuje opcije produženja (1, 3, 6, 12 mjeseci)
- Član može izabrati samo jednu opciju
- Ako ništa nije odabrano, sistem ne dozvoljava nastavak procesa

<br>

---
### Naziv: Potvrda produženja
### US-97: Kao član biblioteke, želim da mogu potvrditi produženje članarine kako bi se moj status ažurirao u sistemu.
**Acceptance Criteria:**
- Kada član potvrdi produženje, tada se datum isteka članarine ažurira
- Status članarine se postavlja na "Aktivna"
- Sistem prikazuje poruku o uspješnom produženju
- Nove informacije su odmah vidljive u profilu člana

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava jednostavno online upravljanje članarinom, smanjuje potrebu za dolaskom u biblioteku i poboljšava korisničko iskustvo. |
| **Pretpostavke / Otvorena pitanja** | Član je prijavljen u sistem. <br> Postoji definisan datum isteka članarine. <br> Sistem podržava ažuriranje članarine. <br> Da li postoji plaćanje ili je besplatno? |
 **Veze i zavisnosti** | PB-27 Upravljanje statusom članarine. <br> PB-28 Pregled statusa članarine |

---

<br>

## PB-49: Integracija sa distributerom knjiga

### Naziv: Unos podataka nabavke
### US-98: Kao bibliotekar, želim unijeti podatke o knjizi koju želim naručiti kako bih pokrenuo proces nabavke.
**Acceptance Criteria:**
- Bibliotekar može otvoriti formu za zahtjev za nabavku knjige
- Forma sadrži polja za naziv knjige, autora, izdavača i broj primjeraka
- Bibliotekar može unijeti dodatni opis ili napomenu
- Sistem validira da su obavezna polja popunjena

<br>

---
### Naziv: Slanje zahtjeva distributeru
### US-99: Kao bibliotekar, želim poslati zahtjev distributeru direktno iz sistema kako bih pojednostavio proces naručivanja knjiga.
**Acceptance Criteria:**
- Kada bibliotekar pošalje zahtjev, sistem generiše email poruku
- Email sadrži podatke o traženoj knjizi
- Email se šalje na unaprijed definisanu adresu distributera
- Sistem evidentira da je zahtjev poslan

<br>

---
### Naziv: Potvrda slanja
### US-100: Kao bibliotekar, želim dobiti potvrdu da je zahtjev uspješno poslan distributeru kako bih znao da je proces nabavke pokrenut.
**Acceptance Criteria:**
- Nakon slanja zahtjeva, sistem prikazuje poruku o uspješnom slanju
- Ako slanje emaila ne uspije, sistem prikazuje odgovarajuću poruku o grešci

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Olakšava proces nabavke novih knjiga, ubrzava komunikaciju sa distributerima i poboljšava upravljanje bibliotečkim fondom. |
| **Pretpostavke / Otvorena pitanja** | Sistem ima definisanu email adresu distributera. |
 **Veze i zavisnosti** | PB-35: Slanje email notifikacija. |

---

<br>

# Sprint 11

## PB-50: Sistemsko testiranje i bug fixing

### Naziv: Test scenariji
### US-101: Kao član tima, želim definisati test scenarije za implementirane funkcionalnosti kako bi se mogla provjeriti ispravnost rada sistema.
**Acceptance Criteria:**
- Za svaku funkcionalnost postoji definisan test scenarij
- Test scenariji sadrže opis koraka testiranja i očekivani rezultat
- Test scenariji su dokumentovani u projektnoj dokumentaciji 

<br>

---

### Naziv: Test funkcionalnosti
### US-102: Kao član tima, želim izvršiti testiranje implementiranih funkcionalnosti kako bih provjerio da li sistem radi prema očekivanjima.
**Acceptance Criteria:**
- Svi definisani test scenariji su izvršeni
- Rezultati testiranja su evidentirani
- Ako funkcionalnost ne radi prema očekivanju, bilježi se bug

<br>

---

### Naziv: Evidentiranje pronađenih grešaka
### US-103: Kao član tima, želim evidentirati pronađene greške i otkloniti ih kako bi sistem bio stabilan i spreman za demonstraciju.
**Acceptance Criteria:**
- Svaki pronađeni bug ima evidentiran opis
- Bug sadrži korake za reprodukciju greške
- Greške su ispravljene prije završne demonstracije

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Osigurava stabilnost i pouzdanost sistema, smanjuje rizik od grešaka tokom demonstracije i povećava kvalitet projekta. |
| **Pretpostavke / Otvorena pitanja** | Sve funkcionalnosti sistema su implementirane. |
 **Veze i zavisnosti** | US-01 – US-100: Implementirane funkcionalnosti sistema. |

---

<br>

## PB-51: Izrada liste poznatih ograničenja i tehničkog duga

### Naziv: Identifikacija ograničenja
### US-104: Kao član tima, želim identifikovati funkcionalna i tehnička ograničenja sistema kako bismo imali jasan uvid u nedostatke trenutne implementacije.
**Acceptance Criteria:**
- Identifikovana su glavna ograničenja sistema
- Svako ograničenje ima kratak opis
- Ograničenja su dokumentovana u projektnoj dokumentaciji

<br>

---
### Naziv: Evidentiranje tehničkog duga
### US-105: Kao član tima, želim evidentirati tehnički dug u projektu kako bismo znali koje dijelove sistema treba unaprijediti.
**Acceptance Criteria:**
- Identifikovani su dijelovi sistema koji predstavljaju tehnički dug
- Svaka stavka sadrži opis problema
- Svaka stavka navodi potencijalni utjecaj na sistem

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava transparentan pregled postojećih ograničenja i tehničkog duga, što pomaže u realnom planiranju razvoja sistema i donošenju odluka o budućim unapređenjima. |
| **Pretpostavke / Otvorena pitanja** | Testiranje sistema je već sprovedeno. |
 **Veze i zavisnosti** | PB-44: Sistematsko testiranje i bug fixing. <br> US-01 – US-100: Implementirane funkcionalnosti sistema|

---

<br>

# Sprint 12

## PB-52: Izrada Relase Notes

### US-106: Kao tim, želimo kreirati Release Notes koji opisuju implementirane funkcionalnosti, poznata ograničenja i upute za instalaciju sistema kako bi finalna verzija projekta bila jasno dokumentovana. 
**Acceptance Criteria:**
- Release Notes sadrže listu svih implementiranih funkcionalnosti sistema
- Release Notes sadrže poznata ograničenja i eventualne poznate bugove
- Dokument sadrži upute za instalaciju i pokretanje sistema
- Release Notes su pohranjeni u repozitoriju projekta

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Predstavljaju formalnu dokumentaciju isporučene verzije sistema i omogućava jasno predstavljanje projekta tokom demonstracije i završne odbrane. |
| **Pretpostavke / Otvorena pitanja** | Testiranje sistema je već sprovedeno. |
 **Veze i zavisnosti** | PB-44: Sistematsko testiranje i bug fixing. <br> PB-45: Lista poznatih ograničenja i tehničkog duga. |

---

<br>

## PB-53: Izrada korisničke dokumentacije

### US-107: Kao tim, želimo kreirati korisničku dokumentaciju koja objašnjava kako koristiti sistem, kako bi krajnji korisnici mogli razumjeti sistem bez tehničkog predznanja.
**Acceptance Criteria:**
- Dokumentacija sadrži upute za sve korisničke uloge (Član, Bibliotekar, Administrator)
- Dokumentacija sadrži opis ključnih funkcionalnosti sistema
- Uključeni su screenshotovi ili opisi glavnih ekrana sistema
- Objašnjeni su osnovni korisnički scenariji (pretraga knjiga, rezervacija, zaduživanje)
- Dokumentacija je pohranjena u repozitoriju projekta

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava korisnicima lakše razumijevanje sistema i njegovih funkcionalnosti, smanjuje potrebu za dodatnim objašnjenjima i povećava profesionalnost projekta. |
| **Pretpostavke / Otvorena pitanja** | Sve ključne funkcionalnosti sistema su implementirane. |
 **Veze i zavisnosti** | US-01 – US-100: Implementirane funkcionalnosti sistema. |

---

<br>

## PB-54: Izrada tehničke dokumentacije

### US-108: Kao tim, želimo kreirati tehničku dokumentaciju koja opisuje arhitekturu, API-je i razvojno okruženje, kako bi sistem bio razumljiv drugom developeru.
**Acceptance Criteria:**
- Dokumentovana je arhitektura sistema. 
- Naveden je postupak postavljanja razvojnog okruženja. 
- Ažurirani su svi obavezni logovi (AI Usage Log, Decision Log).

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Olakšava održavanje, nadogradnju i smanjuje vrijeme potrebno za integraciju novih developera. |
| **Pretpostavke / Otvorena pitanja** |  |
 **Veze i zavisnosti** | US-01 – US-100: Implementirane funkcionalnosti sistema. |

---

<br>

## PB-55: Priprema i izvođenje završne demonstracije

### US-109: Kao tim, želimo pripremiti i izvesti završnu demonstraciju sistema, kako bismo prikazali sve implementirane funkcionalnosti i pokazali da sistem radi ispravno.
**Acceptance Criteria:**
- Svaki član tima je u stanju objasniti dijelove sistema za koje je bio odgovoran. 
- Sistem radi stabilno tokom demonstracije.

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Završna demonstracija pokazuje funkcionisanje sistema i direktno utiče na ocjenu projekta. |
| **Pretpostavke / Otvorena pitanja** |  |
 **Veze i zavisnosti** | US-01 - US-100: Implementirane sve funkcionalnosti. <br> US-107: Izrađena korisnička dokumentacija. <br> US-108: Izrađena tehička dokumentacija. |

---
