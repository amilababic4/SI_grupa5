# Set of User Stories - ažurirana verzija

## Opis dokumenta

Ovaj dokument predstavlja ažuriranu i detaljnije razrađenu verziju User Storyja za projekat Bibliotečkog informacionog sistema. Stavke iz Product Backloga su razrađene u više jasno definisanih User Story jedinica, pri čemu svaki User Story precizno opisuje korisničke zahtjeve, prihvatne kriterije i očekivano ponašanje sistema. Dokument je organizovan prema planiranim sprintovima radi bolje preglednosti i praćenja implementacije.


# Sprint 5

## PB-12: Kreiranje naloga člana

### US-01: Kao bibliotekar, želim unijeti osnovne podatke novog člana kroz formu za registraciju, kako bi član bio evidentiran u sistemu biblioteke.
**Acceptance Criteria:**
- Kada bibliotekar pristupi stranici za registraciju člana, tada sistem mora prikazati formu za unos podataka 
- Sistem mora omogućiti unos imena, prezimena, email adrese i lozinke
- Kada bibliotekar popuni sva obavezna polja i klikne na dugme "Kreiraj nalog", tada sistem nastavlja proces registracije
- Sistem ne smije dozvoliti nastavak registracije bez unosa obaveznih podataka

### US-02: Kao bibliotekar, želim provjeriti ispravnost unesenih podataka prilikom registracije člana, kako bi se spriječio unos pogrešnih ili nepotpunih informacija.
**Acceptance Criteria:**
- Kada bibliotekar unese podatke u formu za registraciju, tada se provjerava da li su sva obavezna polja popunjena
- Kada email adresa nije u ispravnom formatu, tada se prikazuje poruka o grešci
- Kada unesena lozinka ima manje od 8 znakova, tada se prikazuje poruka da lozinka nije dovoljno duga
- Kada email adresa već postoji u sistemu, tada se prikazuje poruka da je ta email adresa već registrovana

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
| **Pretpostavke / Otvorena pitanja** | Član fizički dolazi u biblioteku i daje svoje podatke osoblju. <br> Bibliotekar unosi podatke u sistem. |
| **Veze i zavisnosti** | Postojanje entiteta Korisnik u bazi podataka. |
---
<br>


## PB-11: Sistem prijave korisnika

### US-04: Kao registrovani korisnik, želim da se prijavim u sistem unosom email adrese i lozinke, kako bih pristupio funkcionalnostima sistema.
**Acceptance Criteria:**
- Kada korisnik pristupi stranici za prijavu, tada sistem prikazuje formu za unos emaila i lozinke
- Kada korisnik unese ispravne podatke i klikne na dugme "Prijava", tada se uspješno prijavljuje
- Kada je prijava uspješna, tada se korisnik preusmjerava na odgovarajući dashboard prema ulozi
- Kada podaci nisu ispravni, tada se prikazuje poruka o grešci
- Sistem ne smije omogućiti pristup aplikaciji bez uspješne prijave
---
### US-05: Kao korisnik sistema, želim dobiti jasnu informaciju kada prijava ne uspije, kako bih znao da trebam ponovo unijeti podatke.
**Acceptance Criteria:**
- Kada korisnik unese pogrešan email ili lozinku, tada se prijava odbija
- Kada prijava ne uspije, tada se prikazuje poruka o grešci
- Sistem ne prikazuje da li je greška u emailu ili u lozinci
- Korisnik može ponovo pokušati prijavu 
---
### US-06: Kao prijavljeni korisnik, želim se odjaviti iz sistema kako bih spriječio neovlašten pristup svom nalogu. 
**Acceptance Criteria:**
- Kada je korisnik prijavljen, tada je opcija "Odjava" dostupna u navigaciji
- Kada korisnik klikne na dugme "Odjava", tada se preusmjerava na stranicu za prijavu
- Nakon odjave sistem ne smije dozvoliti pristup zaštićenim stranicama bez ponovne prijave

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava sigurnu autentifikaciju i kontrolu pristupa sistemu biblioteke u skladu sa korisničkim ulogama. Ova funkcionalnost predstavlja osnovu sistema jer bez prijave korisnici ne mogu pristupiti nijednoj funkcionalnosti stranice. |
| **Pretpostavke / Otvorena pitanja** | Korisnici već imaju kreirane naloge u sistemu. <br> Korisnici imaju definisane uloge: Član, Bibliotekar ili Administrator. |
| **Veze i zavisnosti** | Zavisi od postojanja korisničkih naloga u bazi podataka. |
---

<br>

## PB-13: Uspostava AI Usage Loga i Decision Loga

### US-07: Kao tim, želimo kreirati AI Usage Log u okviru projekta, kako bismo evidentirali korištenje AI alata tokom razvoja i osigurali transparentnost rada.
**Acceptance Criteria:**
- Kada tim kreira projektnu dokumentaciju, tada se kreira AI Usage Log fajl u repozitoriju
- Log mora imati definisanu strukturu unosa (datum, alat, svrha korištenja, opis)
- Kada član tima koristi AI alat, tada je dužan evidentirati korištenje u logu
- Log mora biti dostupan svim članovima tima u repozitoriju
---
### US-08: Kao tim, želimo voditi Decision Log kako bismo dokumentovali važne tehničke i arhitekturalne odluke tokom razvoja sistema.
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
| **Veze i zavisnosti** | Definisana struktura projektne dokumentacije. |
---

<br>

# Sprint 6

## PB-16: Dodavanje nove knjige

### US-09: Kao bibliotekar, želim unijeti podatke o novoj knjizi kroz formu za dodavanje, kako bi knjiga bila evidentirana u sistemu biblioteke.
**Acceptance Criteria:**
- Kada bibliotekar otvori formu za dodavanje knjige, tada sistem prikazuje polja za unos podataka
- Sistem mora omogućiti unos: naslov, autor, ISBN, godina izdanja, kategorija i broj primjeraka
- Kada korisnik klikne na "Sačuvaj", tada sistem provjerava ispravnost unesenih podataka
- Kada obavezni podaci nedostaju, tada sistem prikazuje poruku o grešci
- Kada ISBN već postoji ili nije validan, tada sistem odbija unos

 <br>

---

### US-10: Kao bibliotekar, želim da se nakon uspješnog unosa knjiga automatski dodaje u katalog, kako bi bila dostupna korisnicima sistema.
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
| **Pretpostavke / Otvorena pitanja** |  Da li knjiga može biti dodana bez određenih podataka (npr. kategorija)? |
| **Veze i zavisnosti** | Zavisi od definisanog entiteta Knjiga u sistemu. <br> Zavisi od implementiranog kataloga knjiga. |
---

<br>

## PB-17: Uređivanje podataka o knjizi 

### US-11: Kao bibliotekar, želim pristupiti postojećoj knjizi i izmijeniti njene osnovne podatke, kako bi informacije u katalogu bile tačne i ažurirane.
**Acceptance Criteria:**
- Kada bibliotekar odabere knjigu iz kataloga, tada sistem prikazuje trenutne podatke o knjizi
- Sistem mora omogućiti izmjenu sljedećih podataka: naslov, autor, godina izdanja i kategorija
- Kada korisnik izmijeni podatke i klikne na "Sačuvaj izmjene", tada se vrši validacija unosenih vrijednosti
- Sistem ne smije dozvoliti spremanje ako obavezni podaci nisu popunjeni

<br>

---
### US-12: Kao bibliotekar, želim da se nakon uspješnog uređivanja podataka promjene sačuvaju i budu vidljive u katalogu, kako bi korisnici uvijek imali ažurne informacije.
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
| **Veze i zavisnosti** | PB-16: Dodavanje nove knjige |
---

<br>

## PB-17: Pregled kataloga knjiga

### US-13: Kao korisnik sistema, želim vidjeti listu svih dostupnih knjiga u biblioteci, kako bih mogao pretražiti i pronaći knjige koje me zanimaju.
**Acceptance Criteria:**
- Kada korisnik pristupi stranici kataloga knjiga, tada sistem prikazuje listu svih knjiga
- Kada u sistemu postoji više knjiga, tada se sve knjige prikazuju u formi liste ili kartica
- Kada korisnik otvori katalog, tada se podaci automatski učitavaju iz baze podataka
- Kada nema dostupnih knjiga u sistemu, tada se prikazuje poruka da katalog trenutno nema knjiga
- Sistem ne smije prikazivati knjige koje nisu aktivne ili obrisane iz sistema

<br>

---
### US-14: Kao korisnik sistema, želim da mogu pregledati katalog kroz stranice, kako bih lakše pregledao veći broj knjiga.
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
| **Veze i zavisnosti** | PB-16: Dodavanje nove knjige. <br> Entitet Knjiga u bazi podataka. |
---

<br>

## PB-20: Upravljanje primjercima knjige

### US-15: Kao bibliotekar, želim da mogu dodati fizičke primjerke iste knjige u sistem, kako bih imao tačnu evidenciju dostupnog fonda.
**Acceptance Criteria:**
- Kada bibliotekar unosi novu knjigu, tada sistem omogućava unos broja primjeraka
- Kada postoji više primjeraka iste knjige, tada svaki primjerak mora biti zaseban zapis u sistemu
- Sistem ne smije dozvoliti kreiranje primjeraka bez povezane knjige

<br>

---

### US-16: Kao bibliotekar, želim da vidim sve primjerke jedne knjige, kako bih mogao pratiti njihov status i raspoloživost.
**Acceptance Criteria:**
- Kada korisnik otvori detalje knjige, tada sistem prikazuje listu svih primjeraka te knjige
- Kada knjiga ima više primjeraka, tada svaki primjerak mora biti jasno prikazan u listi
- Kada se prikazuju primjerci, tada moraju biti prikazani jedinstveni identifikatori svakog primjerka
- Sistem ne smije prikazivati primjerke koji ne pripadaju odabranoj knjizi

<br>

---

### US-17: Kao bibliotekar, želim da vidim status svakog primjerka knjige (dostupan, posuđen), kako bih znao njegovo trenutno stanje.
**Acceptance Criteria:**
- Kada primjerak knjige postoji u sistemu, tada mora imati definisan status
- Kada se status primjerka promijeni, tada sistem mora ažurirati prikaz statusa
- Kada korisnik pregleda primjerke, tada sistem prikazuje trenutni status svakog primjerka
- Sistem ne smije dozvoliti nevalidne statuse primjeraka

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava precizno upravljanje fizičkim primjercima knjiga u biblioteci, što poboljšava kontrolu nad dostupnošću, zaduživanjem i evidencijom bibliotečkog fonda. |
| **Pretpostavke / Otvorena pitanja** | Sistem ima definisan entitet Knjiga i Primjerak. <br> Da li se primjerci kreiraju automatski prilikom dodavanja knjige ili ručno? |
 **Veze i zavisnosti** | PB-16: Implementirano dodavanje knjige. |
---

<br>

## PB-21: Brisanje knjige i deaktivacija primjerka

### US-18: Kao bibliotekar, želim da mogu obrisati knjigu iz sistema, kako bi katalog sadržavao samo relevantne i dostupne knjige.
**Acceptance Criteria:**
- Kada bibliotekar odabere knjigu, tada sistem prikazuje dugme "Obriši knjigu" za brisanje knjige
- Kada korisnik potvrdi brisanje knjige, ako nema zaduženih primjeraka, tada se knjiga uklanja iz sistema
- Kada je knjiga obrisana, tada više nije vidljiva u katalogu
- Sistem mora prikazati potvrdu prije izvršenja brisanja

<br>

---

### US-19: Kao bibliotekar, želim da ne mogu obrisati knjigu ako postoji aktivno zaduženje, kako bi se spriječio gubitak podataka.
**Acceptance Criteria:**
- Kada knjiga ima aktivno zaduženje, tada sistem ne dozvoljava brisanje
- Kada korisnik pokuša obrisati zaduženu knjigu, tada se prikazuje poruka o grešci
- Kada ne postoji aktivno zaduženje, tada je brisanje omogućeno
- Sistem mora provjeriti status svih primjeraka prije brisanja knjige

<br>

---

### US-20: Kao korisnik sistema, želim da se promjene odmah reflektuju u katalogu, kako bih uvijek imao tačne informacije o dostupnim knjigama.
**Acceptance Criteria:**
- Kada se knjiga obriše, tada se odmah uklanja iz kataloga
- Kada korisnik osvježi stranicu kataloga, tada obrisana knjiga više nije vidljiva

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava održavanje tačnosti i konzistentnosti bibliotečkog fonda, sprječava prikaz zastarjelih ili nevažećih podataka i osigurava da korisnici imaju ažuran pregled dostupnih knjiga. |
| **Pretpostavke / Otvorena pitanja** | Knjige su povezane sa primjercima u sistemu. |
 **Veze i zavisnosti** | PB-16: Implementirano dodavanje knjige. <br> PB-20: Upravljanje primjercima knjige. |

---

<br>

## PB-19: Upravljanje kategorijama knjiga

### US-21: Kao bibliotekar, želim da mogu dodati novu kategoriju knjiga u sistem, kako bi se knjige mogle pravilno organizovati u katalogu.
**Acceptance Criteria:**
- Kada korisnik uđe u sekciju "Kategorije", tada sistem prikazuje listu kategorija
- Kada korisnik klikne na dugme "Dodaj kategoriju", tada se otvara forma za unos
- Kada korisnik unese naziv i klikne "Sačuvaj", tada se kategorija sprema u sistem
- Kada je kategorija uspješno dodana, tada se prikazuje u listi
- Kada kategorija već postoji, tada sistem prikazuje poruku o grešci


<br>

---

### US-22: Kao bibliotekar, želim da mogu izmijeniti naziv kategorije, kako bi podaci u sistemu bili tačni i ažurni.
**Acceptance Criteria:**
- Kada korisnik uđe u sekciju "Kategorije", tada sistem prikazuje listu kategorija
- Kada korisnik klikne "Uredi" pored kategorije, tada se otvara forma sa postojećim podacima
- Kada korisnik izmijeni naziv i klikne "Sačuvaj", tada se izmjena sprema
- Kada je naziv prazan, tada sistem prikazuje grešku
- Kada naziv već postoji, tada sistem odbija izmjenu

<br>

---

### US-23: Kao bibliotekar, želim da mogu obrisati kategoriju iz sistema, kako bih održavao uredan katalog knjiga.
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
| **Pretpostavke / Otvorena pitanja** | Sistem ima admin panel sa sekcijom "Kategorije". <br> Kategorije se koriste pri dodavanju i uređivanju knjiga. |
 **Veze i zavisnosti** | Implementirano dodavanje i uređivanje knjiga, kao i pregled kataloga. |

---

<br>

# Sprint 7

## PB-23: Pretraga knjiga

### US-24: Kao korisnik sistema, želim da mogu pretraživati knjige po naslovu, kako bih brzo pronašao željenu knjigu.
**Acceptance Criteria:**
- Kada korisnik uđe u katalog knjiga, tada sistem prikazuje polje za pretragu
- Kada korisnik unese naslov knjige u pretragu, i klikne na dugme "Pretraži" tada sistem filtrira rezultate
- Pretraga nije osjetljiva na velika i mala slova
- Ako nema rezultata, sistem prikazuje poruku da knjiga nije pronađena

<br>

---

### US-25: Kao korisnik sistema, želim da mogu pretraživati knjige po autoru, kako bih pronašao sve knjige određenog autora.
**Acceptance Criteria:**
- Kada korisnik uđe u katalog knjiga, tada sistem prikazuje polje za pretragu
- Kada korisnik unese ime autora u polje za pretragu, i klikne na dugme "Pretraži" tada sistem filtrira rezultate
- Kada postoji više knjiga istog autora, tada se prikazuju svi rezultati
- Pretraga nije osjetljiva na velika i mala slova
- Ako ne postoji autor u sistemu, tada se prikazuje poruka da nema rezultata

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** |  Omogućava brže i efikasnije pronalaženje knjiga u katalogu, čime se poboljšava korisničko iskustvo i smanjuje vrijeme pretrage. |
| **Pretpostavke / Otvorena pitanja** | Katalog knjiga već postoji i prikazuje sve knjige. <br> Knjige imaju definisan naslov i autora u sistemu. <br> Da li se pretraga treba proširiti i na kategorije? |
 **Veze i zavisnosti** | Implementiran katalog knjiga. |

---

<br>

## PB-18: Prikaz detalja knjige

### US-26: Kao član biblioteke, želim da mogu otvoriti stranicu sa detaljima knjige, kako bih vidio više informacija o knjizi.
**Acceptance Criteria:**
- Kada korisnik u katalogu klikne na karticu knjige, tada se otvara stranica sa detaljima knjige
- Kada se stranica otvori, tada sistem prikazuje osnovne informacije o knjizi
- Korisnik može otvoriti detalje samo klikom na određenu knjigu
- Sistem mora učitati podatke za izabranu knjigu

<br>

---

### US-27: Kao član biblioteke, želim da vidim osnovne informacije o knjizi, kako bih odlučio da li me interesuje.
**Acceptance Criteria:**
- Kada se otvore detalji knjige, tada sistem prikazuje osnovne informacije o knjizi: naslov, autor, kategoriju, izdavač, godinu izdavanja.
- Kada knjiga postoji u sistemu, tada se prikazuju tačni podaci
- Sistem mora prikazati informacije u čitljivom formatu

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** |  Omogućava korisnicima da dobiju detaljne informacije o knjizi prije zaduživanja, čime se smanjuje potreba za dodatnim upitima i poboljšava korisničko iskustvo. |
| **Pretpostavke / Otvorena pitanja** | Knjige već postoje u sistemu. <br> Svaka knjiga ima definisane osnovne podatke. |
 **Veze i zavisnosti** | Implementiran katalog knjiga. |

---

<br>

## PB-24: Pregled dostupnosti knjige

### US-28: Kao član biblioteke, želim vidjeti da li je knjiga dostupna ili zadužena, kako bih znao da li je mogu odmah posuditi.
**Acceptance Criteria:**
- Kada korisnik otvori detalje knjige, tada sistem prikazuje status dostupnosti (Dostupno / Zaduženo)
- Kada knjiga ima slobodne primjerke, tada se prikazuje status “Dostupno”
- Kada knjiga nema slobodnih primjeraka, tada se prikazuje status “Zaduženo”
- Status mora biti jasno vidljiv na stranici detalja knjige

<br>

---

### US-29: Kao član biblioteke, želim vidjeti koliko je primjeraka knjige trenutno dostupno, kako bih znao mogu li je odmah uzeti.
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

### US-30: Kao bibliotekar, želim evidentirati zaduživanje knjige tako što odaberem člana i primjerak knjige, kako bih znao koja knjiga je kod kojeg člana.
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

### US-31: Kao bibliotekar, želim evidentirati vraćanje knjige kako bih ažurirao dostupnost primjerka u sistemu.
**Acceptance Criteria:**
- Kada bibliotekar otvori listu zaduženja, otvori mu se lista aktivnih zaduženja
- Kada bibliotekar odabere neko aktivno zaduženje, tada sistem prikazuje detalje zaduženja
- Kada bibliotekar klikne na dugme "Evidentiraj vraćanje", tada sistem traži potvrdu
- Kada se vraćanje potvrdi, tada se zaduženje označava kao završeno
- Kada je knjiga vraćena, tada se status primjerka mijenja u "Dostupan"
- Promjena je odmah vidljiva u katalogu i detaljima knjige

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava tačno praćenje kretanja knjiga u biblioteci, sprječava gubitak primjeraka i omogućava ažurnu evidenciju zaduženja i vraćanja. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi evidenciju članova i knjiga. |
 **Veze i zavisnosti** |  Upravljanje primjercima knjige. <br> Prikaz detalja knjige. <br> Pregled dostupnosti knjige. |

---

<br>

## PB-14: Pregled profila člana

### US-32: Kao registrovani korisnik (član, bibliotekar ili administrator), želim pregledati svoj profil, kako bih vidio osnovne podatke i zaduženja.
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
 **Veze i zavisnosti** | Domain Model (entitet o korisnicima i zaduženjima). |

---

<br>

## PB-26: Upravljanje korisnicima

### US-33: Kao administrator, želim vidjeti sve registrovane korisnike sistema, kako bih imao uvid u korisničku bazu.
**Acceptance Criteria:**
- Kada administrator otvori sekciju "Korisnici", tada sistem prikazuje listu svih korisnika
- Kada se lista učita, tada se prikazuju osnovni podaci: ime, prezime, email i uloga
- Kada postoji veliki broj korisnika, tada sistem omogućava scroll

<br>

--- 

### US-34: Kao administrator, želim promijeniti ulogu korisnika kako bih upravljao pristupom funkcionalnostima sistema.
**Acceptance Criteria:**
- Kada administrator klikne na korisnika iz liste, tada se otvara detaljni prikaz korisnika
- Kada administrator odabere opciju "Promijeni ulogu", tada se prikazuje dropdown sa ulogama
- Kada administrator potvrdi promjenu, tada se nova uloga sprema u sistem
- Nakon promjene, korisnik odmah ima novu ulogu pri sljedećem pristupu sistemu

<br>

---

### US-35: Kao administrator, želim deaktivirati korisnika kako bih onemogućio njegov dalji pristup sistemu.
**Acceptance Criteria:**
- Kada administrator otvori profil korisnika, tada postoji opcija "Deaktiviraj nalog"
- Kada administrator klikne na deaktivaciju, tada sistem traži potvrdu akcije
- Kada se nalog deaktivira, tada korisnik više ne može pristupiti sistemu
- Sistem ne smije dozvoliti prijavu deaktiviranom korisniku


<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava administratoru potpunu kontrolu nad korisničkim nalozima, čime se osigurava sigurnost sistema, pravilna raspodjela uloga i održavanje ažurne baze korisnika. |
| **Pretpostavke / Otvorena pitanja** | Samo administrator ima pristup upravljanju korisnicima. <br>  Korisnici su već registrovani u sistemu. |
 **Veze i zavisnosti** | Korisnik je registrovan. <br> Pregled profila člana. |

---

<br>

## PB-31: Pregled historije zaduženja

### US-36: Kao bibliotekar, želim pregledati ranija zaduženja člana, kako bih mogao pratiti korištenje fonda i donositi odluke o zaduživanju ili opomenama.
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
 **Veze i zavisnosti** | PB-25 Evidencija zaduživanja i vraćanja. <br> PB-14 Pregled profila člana. |

---

<br>

## PB-27: Upravljanje statusom članarine

### US-37: Kao bibliotekar, želim otvoriti profil člana i vidjeti status njegove članarine kako bih znao da li ima pravo korištenja usluga biblioteke.
**Acceptance Criteria:**
- Kada bibliotekar otvori profil člana, tada sistem prikazuje sekciju "Članarina"
- Kada postoji članarina, tada se prikazuje datum isteka članarine, tj. do kad važi članarina.
- Kada članarina ne postoji, tada se prikazuje poruka "Članarina nije evidentirana"
- Status članarine mora biti jasno vidljiv na profilu člana

<br>

---
### US-38: Kao bibliotekar, želim unijeti ili ažurirati podatke o članarini kako bih osigurao tačne informacije o njenom važenju.
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
PB-26 Upravljanje korisnicima. |

---

<br>

## PB-28: Pregled statusa članarina za člana

### US-39: Kao član biblioteke, želim otvoriti svoj profil i vidjeti da li mi je članarina aktivna kako bih znao da li mogu koristiti usluge biblioteke.
**Acceptance Criteria:**
- Kada se član prijavi u sistem, tada može otvoriti sekciju "Moj profil"
- Kada član otvori profil, tada sistem prikazuje status članarine
- Kada je članarina aktivna, tada se prikazuje status "Aktivna"
- Kada je članarina istekla, tada se prikazuje status "Istekla"
- Status mora biti jasno vidljiv na profilu člana

<br>

---

### US-40: Kao član biblioteke, želim vidjeti do kada važi moja članarina kako bih znao kada je potrebno produženje.
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
 **Veze i zavisnosti** | PB-27 Upravljanje statusom članarine. <br> PB-14 Pregled profila člana. |

---

<br>

## PB-32: Početno testiranje sistema

### US-41: Kao član tima, želim testirati implementirane funkcionalnosti sistema kako bih provjerio da li sistem radi ispravno u osnovnim scenarijima.
**Acceptance Criteria:**
- Testiranje uključuje provjeru prijave, pregleda kataloga i zaduživanja knjiga
- Svaka funkcionalnost mora imati definisan očekivani rezultat
- Rezultati testiranja se dokumentuju u obliku liste uspješnih i neuspješnih testova

<br>

---

### US-42: Kao član tima, želim provjeriti da li različiti dijelovi sistema pravilno rade zajedno kako bih osigurao stabilan rad aplikacije.
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

## PB-29: Pregled vlastitih zaduženja

### US-43: Kao član biblioteke, želim vidjeti sve knjige koje trenutno imam zadužene, kako bih imao pregled svojih obaveza. 
**Acceptance Criteria:**
- Kada je korisnik prijavljen u sistem, tada može otvoriti sekciju "Moja zaduženja". 
- Kada korisnik otvori ovu sekciju, tada se prikazuje lista svih aktivnih zaduženja. 
- Za svako zaduženje prikazuje se naziv knjige i datum zaduživanja. 
- Ako član nema aktivnih zaduženja, tada se prikazuje poruka "Nema aktivnih zaduženja". 
- Sistem ne smije prikazivati zaduženja drugih korisnika.

<br>

--- 

### US-44: Kao član biblioteke, želim vidjeti rok vraćanja za svaku zaduženu knjigu, kako bih mogao planirati vraćanje na vrijeme. 
**Acceptance Criteria:**
- Kada je korisnik prijavljen u sistem, tada može otvoriti sekciju "Moja zaduženja". 
- Kada korisnik otvori ovu sekciju, tada se prikazuje lista svih aktivnih zaduženja.
- Kada član pregleda listu zaduženja, tada za svaku knjigu vidi i rok vraćanja. 
- Kada se rok vraćanja približava, sistem jasno označava zaduženje (vizualno isticanje). 
- Kada je knjiga vraćena, ona se više ne prikazuje u aktivnim zaduženjima. 
- Podaci o rokovima moraju biti tačni i ažurirani u realnom vremenu.

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava članovima jasan pregled trenutnih zaduženja i rokova vraćanja, što smanjuje kašnjenja i rasterećuje bibliotekare. |
| **Pretpostavke / Otvorena pitanja** | Zaduženja i rokovi su već evidentirani u sistemu.  |
 **Veze i zavisnosti** | Evidencija zaduživanja i vraćanja knjiga. |

---

<br>

## PB-30: Pregled trenutnih zaduženja

### US-45:  Kao bibliotekar, želim vidjeti sva trenutno aktivna zaduženja u sistemu, kako bih mogao pratiti koje knjige su trenutno posuđene i kod kojih članova.
**Acceptance Criteria:**
- Kada je bibliotekar prijavljen u sistem, tada može otvoriti sekciju "Aktivna zaduženja". 
- Kada se sekcija otvori, tada se prikazuje lista svih aktivnih zaduženja. 
- Za svako zaduženje prikazuju se: ime i email člana, naziv knjige, datum zaduživanja i rok vraćanja. 
- Ako ne postoje aktivna zaduženja, tada se prikazuje poruka "Nema aktivnih zaduženja".

<br>

---

### US-46: Kao bibliotekar, želim da aktivna zaduženja budu sortirana po roku vraćanja, kako bih lakše identifikovao knjige koje treba uskoro vratiti.
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
 **Veze i zavisnosti** | Evidencija zaduživanja i vraćanja knjiga.  |

---

<br>

## PB-33: Rezervacija knjiga

### US-47: Kao član biblioteke, želim rezervisati knjigu koja trenutno nema dostupnih primjeraka, kako bih bio obaviješten kada knjiga ponovo postane dostupna.
**Acceptance Criteria:**
- Kada je član prijavljen u sistem, tada u katalogu knjiga može otvoriti detalje knjige. 
- Kada knjiga nema dostupnih primjeraka, tada je opcija "Rezerviši" aktivna. 
- Kada član klikne na "Rezerviši", tada se rezervacija uspješno kreira. 
- Sistem ne smije dozvoliti rezervaciju knjige koja ima dostupne primjerke. 
- Član ne može napraviti više aktivnih rezervacija iste knjige.

<br>

---
### US-48: Kao član biblioteke, želim imati mogućnost otkazivanja rezervacije kako bih upravljao svojim aktivnim rezervacijama.
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
 **Veze i zavisnosti** | Pregled dostupnosti knjiga. |

---

<br>

## PB-34: Pregled aktivnih rezervacija

### US-49: Kao bibliotekar, želim vidjeti sve aktivne rezervacije knjiga u sistemu, kako bih imao pregled koje knjige su rezervisane i od strane kojih članova.
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

## PB-38: Napredna pretraga i filteri

### US-50: Kao član biblioteke, želim filtrirati knjige po kategoriji, kako bih lakše pronašao knjige iz određene oblasti.
**Acceptance Criteria:**
- Kada je korisnik na stranici kataloga knjiga, tada može izabrati filter "Kategorija". 
- Kada korisnik odabere kategoriju, tada se prikazuju samo knjige iz te kategorije. 
- Kada korisnik promijeni kategoriju, tada se lista knjiga ažurira. 
- Ako ne postoji nijedna knjiga u odabranoj kategoriji, tada se prikazuje poruka "Nema rezultata".

<br>

---

### US-51: Kao član biblioteke, želim filtrirati knjige po izdavaču, kako bih pronašao knjige određenog izdavača.
**Acceptance Criteria:**
- Kada je korisnik na katalogu knjiga , tada može izabrati filter "Izdavač". 
- Kada korisnik odabere izdavača, tada se prikazuju samo knjige tog izdavača.
- Lista se ažurira nakon izbora.
- Ako nema rezultata, prikazuje se poruka "Nema knjiga za odabranog izdavača".

<br>

---

### US-52: Kao član biblioteke, želim filtrirati knjige po godini izdanja, kako bih pronašao novije ili starije knjige.
**Acceptance Criteria:**
- Kada je korisnik na katalogu knjiga , tada može izabrati filter "Godina izdanja". 
- Kada se unese godina, tada se prikazuju samo knjige iz te godine. 
- Lista se ažurira nakon promjene filtera.
- Ako nema rezultata, prikazuje se poruka "Nema knjiga za odabranu godinu".

<br>

---

### US-53: Kao član biblioteke, želim kombinovati više filtera istovremeno, kako bih preciznije pronašao željene knjige.
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

## PB-37: Automatsko otkazivanje rezervacije

### US-54: Kao sistem, želim evidentirati i pratiti datum isteka rezervacije, kako bih znao kada rezervacija treba biti otkazana ako knjiga nije preuzeta.
**Acceptance Criteria:**
- Kada se rezervacija kreira, tada se upisuje datum isteka rezervacije. 
- Sistem mora čuvati informaciju o roku važenja svake aktivne rezervacije.
- Svaka rezervacija mora imati definisan vremenski period važenja. 

<br>

---

### US-55: Kao sistem, želim automatski otkazati rezervacije koje su istekle, kako bi knjige ponovo postale dostupne drugim članovima.
**Acceptance Criteria:**
- Sistem periodično provjerava sve aktivne rezervacije.
- Kada rezervacija istekne, tada se automatski označava kao otkazana. 
- Otkazana rezervacija više nije aktivna u sistemu. 
- Nakon otkazivanja, knjiga postaje dostupna za zaduživanje ili novu rezervaciju.

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

## PB-35: Slanje email upozorenja

### US-56: Kao član biblioteke, želim da dobijem email podsjetnik prije isteka roka vraćanja knjige kako bih je mogao na vrijeme vratiti.
**Acceptance Criteria:**
- Kada je rok vraćanja knjige 2 dana od isteka, tada sistem automatski šalje email podsjetnik
- Kada član ima više zaduženih knjiga, tada se šalje podsjetnik za svaku knjigu posebno
- Email mora sadržavati naziv knjige i datum roka vraćanja
- Mail se šalje samo za aktivna zaduženja

<br>

---

### US-57: Kao član biblioteke, želim dobiti email upozorenje na dan kada mi ističe rok vraćanja knjige kako bih znao da trebam odmah vratiti knjigu.
**Acceptance Criteria:**
- Kada rok vraćanja knjige istekne, tada sistem automatski šalje email upozorenje
- Kada član ima više knjiga kojima ističe rok, tada se šalje upozorenje za svaku knjigu posebno
- Email mora sadržavati naziv knjige i datum isteka roka
- Upozorenje se šalje samo za knjige koje nisu vraćene

<br>

---

### US-58: Kao član biblioteke, želim dobiti podsjetnik ako kasnim s vraćanjem knjige kako bih bio svjestan da trebam što prije vratiti knjigu.
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
 **Veze i zavisnosti** | Evidencija zaduživanja i vraćanja knjiga. <br> Pregled vlastitih zaduženja. |

---

<br>

## PB-36: Obavještavanje bibliotekara o novoj rezervaciji

### US-59: Kao bibliotekar, želim da dobijem email obavijest svaki put kada član kreira rezervaciju knjige kako bih bio informisan o novim rezervacijama.
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

## PB-39: Mjesečni izvještaji za upravu

### US-60: Kao administrator biblioteke, želim generisati mjesečni izvještaj o zaduživanjima knjiga kako bih mogao pratiti korištenje bibliotečkog fonda.
**Acceptance Criteria:**
- Kada administrator odabere sekciju "Generiši izvještaj", otvara se forma o tipu izvještaja
- Kada administrator odabere "Izvještaj o  zaduživanjima", otvara se forma o periodu izvještaja
- Kada administartor odabere mjesec i godinu, tada sistem generiše izvještaj o zaduživanjima
- Izvještaj prikazuje broj ukupnih zaduženja u tom periodu
- Izvještaj prikazuje listu zaduženih knjiga i članova
- Ako nema podataka, sistem prikazuje poruku da nema zaduživanja za taj period

<br>

---

### US-61: Kao administrator biblioteke, želim generisati mjesečni izvještaj o rezervacijama knjiga kako bih imao uvid u potražnju za knjigama.
**Acceptance Criteria:**
- Kada administrator odabere sekciju "Generiši izvještaj", otvara se forma o tipu izvještaja
- Kada administrator odabere "Izvještaj o  rezervacijama", otvara se forma o periodu izvještaja
- Kada administartor odabere mjesec i godinu, tada sistem generiše izvještaj o rezervacijama
- Izvještaj prikazuje broj aktivnih i završenih rezervacija
- Izvještaj prikazuje listu rezervisanih knjiga i članova
- Ako nema rezervacija, sistem prikazuje odgovarajuću poruku

<br>

---

### US-62: Kao administrator biblioteke, želim generisati mjesečni izvještaj o članovima kako bih pratio aktivnost i stanje članstva.
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

## PB-40: Audit log promjena

### US-63: Kao član osoblja, želim da sistem automatski evidentira svako dodavanje, izmjenu i brisanje knjiga kako bih mogao pratiti promjene u bibliotečkom fondu.
**Acceptance Criteria:**
- Kada se knjiga doda, izmijeni ili obriše, tada sistem automatski kreira audit zapis
- Audit zapis sadrži naziv akcije, datum i vrijeme promjene
- Audit zapis sadrži korisnika koji je izvršio promjenu
- Svi zapisi se čuvaju u sistemu i nisu dostupni za izmjenu

<br>

---

### US-64: Kao administrator, želim da sistem bilježi promjene nad korisničkim nalozima kako bih mogao pratiti sigurnost i aktivnosti korisnika.
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

## PB-41: Kazne za kasno vraćanje knjiga

### US-65: Kao sistem, želim automatski obračunati kaznu za svaku knjigu koja nije vraćena u predviđenom roku kako bi se osigurala disciplina i poštovanje pravila korištenja.
**Acceptance Criteria:**
- Kada je knjiga vraćena nakon isteka roka, tada sistem automatski obračunava kaznu po danu kašnjenja
- Kazna se računa za svaki dan kašnjenja
- Kazna se veže za konkretno zaduženje i člana

<br>

---

### US-66: Kao član biblioteke, želim da mogu pregledati ukupne kazne kako bih bio informisan o svojim obavezama.
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

## PB-42: Online produžetak članarine

### US-67: Kao član biblioteke, želim da mogu pristupiti opciji za produženje članarine kako bih započeo proces produženja.
**Acceptance Criteria:**
- Kada je član prijavljen, tada na svom profilu može vidjeti opciju "Produži članarinu"
- Kada član klikne na tu opciju, tada se otvara stranica za produženje
- Sistem prikazuje trenutni status i datum isteka članarine

<br>

---

### US-68: Kao član biblioteke, želim da odaberem trajanje produženja članarine kako bih prilagodio period svojih potreba.
**Acceptance Criteria:**
- Sistem prikazuje opcije produženja (1, 3, 6, 12 mjeseci)
- Član može izabrati samo jednu opciju
- Ako ništa nije odabrano, sistem ne dozvoljava nastavak procesa

<br>

---

### US-69: Kao član biblioteke, želim da mogu potvrditi produženje članarine kako bi se moj status ažurirao u sistemu.
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

## PB-43: Integracija sa distributerom knjiga

### US-70: Kao bibliotekar, želim unijeti podatke o knjizi koju želim naručiti kako bih pokrenuo proces nabavke.
**Acceptance Criteria:**
- Bibliotekar može otvoriti formu za zahtjev za nabavku knjige
- Forma sadrži polja za naziv knjige, autora, izdavača i broj primjeraka
- Bibliotekar može unijeti dodatni opis ili napomenu
- Sistem validira da su obavezna polja popunjena

<br>

---

### US-71: Kao bibliotekar, želim poslati zahtjev distributeru direktno iz sistema kako bih pojednostavio proces naručivanja knjiga.
**Acceptance Criteria:**
- Kada bibliotekar pošalje zahtjev, sistem generiše email poruku
- Email sadrži podatke o traženoj knjizi
- Email se šalje na unaprijed definisanu adresu distributera
- Sistem evidentira da je zahtjev poslan

<br>

---

### US-72: Kao bibliotekar, želim dobiti potvrdu da je zahtjev uspješno poslan distributeru kako bih znao da je proces nabavke pokrenut.
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

## PB-44: Sistemsko testiranje i bug fixing

### US-73: Kao član tima, želim definisati test scenarije za implementirane funkcionalnosti kako bi se mogla provjeriti ispravnost rada sistema.
**Acceptance Criteria:**
- Za svaku funkcionalnost postoji definisan test scenarij
- Test scenariji sadrže opis koraka testiranja i očekivani rezultat
- Test scenariji su dokumentovani u projektnoj dokumentaciji 

<br>

---

### US-74: Kao član tima, želim izvršiti testiranje implementiranih funkcionalnosti kako bih provjerio da li sistem radi prema očekivanjima.
**Acceptance Criteria:**
- Svi definisani test scenariji su izvršeni
- Rezultati testiranja su evidentirani
- Ako funkcionalnost ne radi prema očekivanju, bilježi se bug

<br>

---

### US-75: Kao član tima, želim evidentirati pronađene greške i otkloniti ih kako bi sistem bio stabilan i spreman za demonstraciju.
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
 **Veze i zavisnosti** | US-01 – US-74: Implementirane funkcionalnosti sistema. |

---

<br>

## PB-45: Izrada liste poznatih ograničenja i tehničkog duga

### US-76: Kao član tima, želim identifikovati funkcionalna i tehnička ograničenja sistema kako bismo imali jasan uvid u nedostatke trenutne implementacije.
**Acceptance Criteria:**
- Identifikovana su glavna ograničenja sistema
- Svako ograničenje ima kratak opis
- Ograničenja su dokumentovana u projektnoj dokumentaciji

<br>

---

### US-77: Kao član tima, želim evidentirati tehnički dug u projektu kako bismo znali koje dijelove sistema treba unaprijediti.
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
 **Veze i zavisnosti** | PB-44: Sistematsko testiranje i bug fixing. <br> US-01 – US-74: Implementirane funkcionalnosti sistema|

---

<br>

# Sprint 12

## PB-46: Izrada Relase Notes

### US-78: Kao tim, želimo kreirati Release Notes koji opisuju implementirane funkcionalnosti, poznata ograničenja i upute za instalaciju sistema kako bi finalna verzija projekta bila jasno dokumentovana. 
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

## PB-47: Izrada korisničke dokumentacije

### US-79: Kao tim, želimo kreirati korisničku dokumentaciju koja objašnjava kako koristiti sistem, kako bi krajnji korisnici mogli razumjeti sistem bez tehničkog predznanja.
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
 **Veze i zavisnosti** | US-01 – US-74: Implementirane funkcionalnosti sistema. |

---

<br>

## PB-48: Izrada tehničke dokumentacije

### US-80: Kao tim, želimo kreirati tehničku dokumentaciju koja opisuje arhitekturu, API-je i razvojno okruženje, kako bi sistem bio razumljiv drugom developeru.
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
 **Veze i zavisnosti** | US-01 – US-74: Implementirane funkcionalnosti sistema. |

---

<br>

## PB-49: Priprema i izvođenje završne demonstracije

### US-81: Kao tim, želimo pripremiti i izvesti završnu demonstraciju sistema, kako bismo prikazali sve implementirane funkcionalnosti i pokazali da sistem radi ispravno.
**Acceptance Criteria:**
- Svaki član tima je u stanju objasniti dijelove sistema za koje je bio odgovoran. 
- Sistem radi stabilno tokom demonstracije.

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Završna demonstracija pokazuje funkcionisanje sistema i direktno utiče na ocjenu projekta. |
| **Pretpostavke / Otvorena pitanja** |  |
 **Veze i zavisnosti** | US-01 - US-74: Implementirane sve funkcionalnosti. <br> US-79: Izrađena korisnička dokumentacija. <br> US-80: Izrađena tehička dokumentacija. |

---
