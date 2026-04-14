#  Use Case Model

**Projekt:** Bibliotečki informacioni sistem

---

##  Pregled aktera

* **Član biblioteke** – koristi sistem za pregled, rezervaciju i posudbu knjiga
* **Bibliotekar** – upravlja knjigama, članovima i zaduženjima
* **Administrator** – upravlja korisnicima i sistemskim funkcijama
* **Sistem** – automatski izvršava određene procese (notifikacije, kazne, logovi)

---

#  Use Case 1: Registracija člana

* **Akter:** Bibliotekar
* **Opis:** Bibliotekar unosi podatke novog člana u sistem

### Preduslovi

* Bibliotekar je prijavljen u sistem

### Glavni tok

1. Bibliotekar otvara formu za registraciju
2. Unosi ime, prezime, email i lozinku
3. Sistem validira podatke
4. Sistem prikazuje potvrdu o uspješnpj registraciji
5. Sistem kreira korisnički nalog

### Alternativni tokovi

* Neispravni podaci - prikaz poruke o grešci
* Lozinka prekratka - prikaz poruke da lozinka nije dovoljno duga
* Email već postoji - odbijanje registracije

### Ishod

* Novi član je registrovan u sistemu i može se prijaviti

---

#  Use Case 2: Prijava u sistem

* **Akter:** Korisnik sistema (član/bibliotekar/administrator)
* **Opis:** Korisnik se prijavljuje u sistem

### Preduslovi

* Korisnik ima kreiran nalog

### Glavni tok

1. Korisnik unosi email i lozinku
2. Sistem provjerava podatke
3. Sistem preusmjerava na odgovarajući dashboard
4. Sistem omogućava pristup

### Alternativni tokovi

* Pogrešni podaci - prikaz poruke o grešci
* Deaktiviran korisnik - odbijena prijava

### Ishod

* Korisnik je prijavljen

---

#  Use Case 3: Pregled kataloga knjiga

* **Akter:** Član biblioteke
* **Opis:** Korisnik pregleda dostupne knjige

### Preduslovi

* Korisnik je prijavljen

### Glavni tok

1. Korisnik otvara katalog
2. Sistem prikazuje listu knjiga
3. Korisnik pregledava knjige

### Alternativni tokovi
* Nema dostupnih knjiga - prikaz poruke

### Ishod

* Prikazan katalog knjiga

---

#  Use Case 4: Pretraga knjiga

* **Akter:** Član biblioteke
* **Opis:** Korisnik pretražuje knjige po naslovu ili autoru

### Glavni tok

1. Korisnik unosi pojam za pretragu
2. Sistem filtrira rezultate
3. Sistem prikazuje odgovarajuće knjige

### Alternativni tokovi

* Nema rezultata - prikaz poruke
* Korisnik klikne dugme "Očisti" - prikaz svih knjiga

### Ishod

* Prikazani rezultati pretrage

---

#  Use Case 5: Dodavanje knjige

* **Akter:** Bibliotekar
* **Opis:** Bibliotekar dodaje novu knjigu u sistem

### Preduslovi

* Bibliotekar je prijavljen

### Glavni tok

1. Bibliotekar unosi podatke o knjizi
2. Bibliotekar unosi kategoriju knjige
3. Bibliotekar unosi broj primjeraka
4. Sistem validira podatke
5. Sistem sprema knjigu u katalog

### Alternativni tokovi

* Neispravni podaci - greška
* ISBN već postoji - odbijanje

### Ishod

* Knjiga dodana u sistem

---

#  Use Case 6: Uređivanje knjige

* **Akter:** Bibliotekar
* **Opis:** Bibliotekar mijenja postojeće podatke o knjizi

### Preduslovi

* Bibliotekar je prijavljen
* Knjiga postoji u sistemu

### Glavni tok

1. Bibliotekar odabire knjigu
2. Mijenja podatke
3. Sistem sprema izmjene

### Alternativni tokovi

* Neispravni podaci - sistem prikazuje grešku
  
### Ishod

* Podaci ažurirani

---

#  Use Case 7: Brisanje knjige

* **Akter:** Bibliotekar

### Preduslovi

* Bibliotekar je prijavljen
  
### Glavni tok

1. Bibliotekar bira knjigu
2. Sistem prikazuje potvrdu za brisanje
3. Bibliotekar potvrđuje brisanje
4. Sistem briše knjigu

### Alternativni tokovi

* Aktivno zaduženje - brisanje nije dozvoljeno

### Ishod

* Knjiga obrisana

---

#  Use Case 8: Evidencija zaduživanja

* **Akter:** Bibliotekar

### Preduslovi

* Bibliotekar je prijavljen

### Glavni tok

1. Bibliotekar bira člana
2. Sistem provjerava da li član ima aktivnu članarinu
3. Bibliotekar bira knjigu i primjerak
4. Sistem provjerava da knjiga nije već zadužena
5. Sistem postavlja rok vraćanja
6. Bibliotekar potvrđuje zaduživanje
7. Sistem evidentira zaduženje

### Alternativni tokovi

* Član nema aktivnu članarinu - zaduživanje nije dozvoljeno
* Knjiga već zadužena - akcija odbijena

### Ishod

* Knjiga zadužena

---

#  Use Case 9: Evidencija vraćanja

* **Akter:** Bibliotekar

### Preduslovi

* Bibliotekar je prijavljen

### Glavni tok

1. Bibliotekar bira aktivno zaduženje
2. Potvrđuje vraćanje
3. Sistem ažurira status primjerka knjige na "Dostupan"

### Ishod

* Knjiga vraćena

---

#  Use Case 10: Rezervacija knjige

* **Akter:** Član biblioteke

### Preduslovi

* Knjiga nema dostupnih primjeraka

### Glavni tok

1. Korisnik otvara detalje knjige
2. Klikne "Rezerviši"
3. Sistem kreira rezervaciju

### Alternativni tokovi

* Knjiga dostupna - nema rezervacije
* Korisnik već ima rezervaciju - akcija odbijena

### Ishod

* Rezervacija kreirana

---

#  Use Case 11: Pregled korisnika

* **Akter:** Administrator
* **Opis:** Administrator pregledava listu korisnika

### Glavni tok

1. Administrator otvara listu korisnika
2. Sistem prikazuje korisnike

### Ishod

* Prikazana lista korisnika

---

#  Use Case 12: Promjena uloge

* **Akter:** Administrator

### Glavni tok

1. Administrator bira korisnika
2. Mijenja ulogu
3. Sistem sprema izmjenu

### Ishod

* Uloga ažurirana

---

#  Use Case 13: Deaktivacija korisnika

* **Akter:** Administrator

### Glavni tok

1. Administrator bira korisnika
2. Deaktivira nalog
3. Sistem sprema promjenu

### Ishod

* Korisnik deaktiviran

---

#  Use Case 14: Pregled profila

* **Akter:** Korisnik

### Glavni tok

1. Korisnik otvara profil
2. Sistem prikazuje podatke i listu posuđenih knjiga

### Ishod

* Prikazan profil

---

#  Use Case 15: Slanje email notifikacija

* **Akter:** Sistem

### Glavni tok

1. Sistem prati rokove vraćanja
2. Šalje podsjetnike i upozorenja

### Alternativni tokovi

* Email nije validan - notifikacija se ne šalje
* Knjiga vraćena - prestanak slanja podsjetnika

### Ishod

* Email poslan korisniku

---

#  Use Case 16: Generisanje izvještaja

* **Akter:** Administrator

### Glavni tok

1. Administrator bira tip izvještaja
2. Administrator bira mjesec i godinu
3. Sistem generiše podatke

### Ishod

* Izvještaj generisan

---
#  Use Case 17: Pregled detalja knjige

* **Akter:** Član biblioteke
* **Opis:** Korisnik pregleda detalje knjige

### Preduslovi:
* Korisnik je prijavljen u sistem
  
### Glavni tok

1. Korisnik otvara knjigu
2. Sistem prikazuje naslov, autora, opis
3. Sistem prikazuje status dostupnosti
4. Sistem prikazuje broj dostupnih primjeraka

### Alternativni tok:
* Nema dostupnih primjeraka - sistem prikazuje status “Zaduženo” i broj dostupnih 0

### Ishod

Prikazana dostupnost

---
#  Use Case 18: Upravljanje kategorijama

* **Akter:** Bibliotekar
* **Opis:** Bibliotekar upravlja kategorijama knjiga, uključujući pregled, dodavanje, uređivanje i brisanje kategorija

### Preduslovi:
* Bibliotekar je prijavljen u sistem
  
### Glavni tok:
1. Bibliotekar ulazi u sekciju "Kategorije"
2. Sistem prikazuje listu svih kategorija
3. Bibliotekar bira jednu od akcija: dodavanje, uređivanje ili brisanje

- Dodavanje kategorije 
4. Bibliotekar klikne "Dodaj kategoriju"
5. Sistem prikazuje formu za unos
6. Bibliotekar unosi naziv kategorije i potvrđuje
7. Sistem sprema kategoriju i prikazuje je u listi

- Uređivanje kategorije 
8. Bibliotekar klikne "Uredi" pored kategorije
9. Sistem prikazuje postojeće podatke
10. Bibliotekar mijenja naziv i potvrđuje
11. Sistem sprema izmjene

- Brisanje kategorije 
12. Bibliotekar klikne "Obriši"
13. Sistem prikazuje potvrdu brisanja
14. Bibliotekar potvrđuje
15. Sistem briše kategoriju

### Alternativni tokovi:
* Nema kategorija - sistem prikazuje odgovarajuću poruku
* Kategorija već postoji - sistem prikazuje grešku
* Naziv kategorije je prazan - sistem prikazuje grešku
* Naziv već postoji prilikom uređivanja - izmjena se odbija
* Kategorija ima povezane knjige - sistem ne dozvoljava brisanje i prikazuje poruku

### Ishod:
Kategorije su uspješno ažurirane i prikazane 

---
#  Use Case 19: Upravljanje primjercima

* **Akter:** Bibliotekar
* **Opis:** Bibliotekar upravlja primjercima knjiga, uključujući dodavanje, pregled i deaktivaciju primjeraka

### Preduslovi:
* Bibliotekar je prijavljen
* Knjiga postoji u sistemu


### Glavni tok

1. Bibliotekar otvara detalje knjige
2. Sistem prikazuje listu svih primjeraka knjige sa jedinstvenim ID-evima
3. Sistem prikazuje status svakog primjerka (Dostupan / Posuđen / Deaktiviran)
4. Bibliotekar dodaje novi primjerak knjige
5. Sistem kreira novi primjerak povezan sa knjigom
6. Bibliotekar može deaktivirati primjerak
7. Sistem ažurira status primjerka

### Alternativni tokovi:
* Pokušaj dodavanja primjerka bez knjige - sistem ne dozvoljava akciju
* Nevalidan status primjerka - sistem prikazuje grešku
* Pokušaj deaktivacije primjerka koji je zadužen - akcija nije dozvoljena
* Nema primjeraka - sistem prikazuje poruku
  
### Ishod

Primjerci ažurirani i evidentirani

---
#  Use Case 20: Pregled zaduženja (član biblioteke)

* **Akter:** Član biblioteke
* **Opis:** Korisnik pregledava svoja zaduženja

### Glavni tok

1. Korisnik otvara profil
2. Sistem prikazuje aktivna i prošla zaduženja

### Ishod

Prikazana zaduženja

---
#  Use Case 21: Aktivna zaduženja (bibliotekar)

* **Akter:** Bibliotekar
* **Opis:** Bibliotekar pregledava, filtrira, sortira i upravlja svim aktivnim zaduženjima u sistemu

### Preduslovi:
* Bibliotekar je prijavljen u sistem
* Postoje evidentirana zaduženja

### Glavni tok

1. Bibliotekar otvara sekciju “Aktivna zaduženja”
2. Sistem prikazuje listu svih aktivnih zaduženja
3. Sistem za svako zaduženje prikazuje: ime i email člana, naziv knjige, datum zaduženja i rok vraćanja
4. Sistem automatski sortira zaduženja po roku vraćanja (najbliži rok prvi)
5. Bibliotekar može otvoriti detalje odabranog zaduženja
6. Sistem prikazuje detalje zaduženja (član, knjiga, primjerak, datum zaduženja, rok vraćanja)

## Alternativni tokovi:
* Bibliotekar filtrira listu po članu- sistem prikazuje samo zaduženja odabranog člana

* Nema aktivnih zaduženja - sistem prikazuje poruku

### Ishod

* Prikazana aktivna zaduženja sa mogućnošću filtriranja, sortiranja i pregleda detalja

---
#  Use Case 22: Upravljanje članarinom

* **Akter:** Bibliotekar
* **Opis:** Bibliotekar pregledava, unosi ili ažurira članarinu

### Preduslovi:
* Bibliotekar je prijavljen u sistem
* Član postoji u evidenciji

### Glavni tok

1. Bibliotekar otvara profil člana
2. Sistem prikazuje sekciju „Članarina“
3. Bibliotekar bira opciju za upravljanje članarinom
4. Sistem prikazuje formu za unos/izmjenu članarine
5. Bibliotekar unosi datum početka i datum isteka
6. Sistem validira unesene datume
7. Sistem sprema članarinu
8. Sistem prikazuje ažuriran status i datum isteka na profilu člana

### Alternativni tokovi

* Članarina ne postoji - sistem prikazuje poruku
* Neispravni datumi (datum isteka prije datuma početka) - sistem prikazuje grešku

### Ishod

* Članarina ažurirana ili evidentirana

#  Use Case 23: Pregled članarine (član)

* **Akter:** Član biblioteke
* **Opis:** Član pregleda status i trajanje svoje članarine

### Preduslovi:
* Član je prijavljen u sistem

### Glavni tok

1. Član otvara sekciju „Moj profil“
2. Sistem prikazuje status članarine
3. Sistem prikazuje datum isteka članarine

### Alternativni tokovi

* Članarina ne postoji – sistem prikazuje poruku
* Članarina je istekla – sistem prikazuje status „Istekla“

### Ishod

* Status i datum isteka članarine su prikazani

---

#  Use Case 24: Odjava iz sistema

* **Akter:** Korisnik sistema
* **Opis:** Korisnik se odjavljuje iz sistema

### Preduslovi:
* Korisnik je prijavljen u sistem

### Glavni tok

1. Korisnik klikne na opciju „Odjava“
2. Sistem briše korisničku sesiju
3. Sistem preusmjerava korisnika na stranicu za prijavu

### Alternativni tokovi

* Sesija je već istekla – sistem preusmjerava korisnika na prijavu

### Ishod

* Korisnik je uspješno odjavljen iz sistema
  
---
# Domain Model

## 1. Glavni entiteti

Ova sekcija definiše osnovne domenske objekte sistema. Entiteti predstavljaju ključne koncepte u poslovnoj domeni bibliotečkog sistema i direktno odgovaraju tabelama u bazi podataka.

| Entitet | Opis |
|--------|------|
| **Korisnik** | Predstavlja registrovanog korisnika sistema |
| **Uloga** | Definiše tip korisnika i nivo pristupa sistemu |
| **Knjiga** | Bibliografski zapis knjige |
| **Kategorija** | Klasifikacija knjiga po tematici |
| **Primjerak** | Fizički primjerak knjige u biblioteci |
| **Zaduženje** | Evidencija posudbe knjige od strane korisnika |
| **Članarina** | Evidencija članstva korisnika |
| **Rezervacija** | Evidencija rezervacije knjige |
| **AuditLog** | Evidencija svih kritičnih akcija u sistemu |

---

## 2. Ključni atributi

Ova sekcija opisuje najvažnije atribute svakog entiteta. Atributi definišu strukturu podataka, njihove tipove i osnovna ograničenja koja osiguravaju integritet sistema.

### Korisnik

| Atribut | Tip | Napomena |
|--------|-----|---------|
| id | INT | Primarni ključ |
| ime | VARCHAR | Obavezno |
| prezime | VARCHAR | Obavezno |
| email | VARCHAR | Jedinstven |
| lozinka_hash | VARCHAR | Hashirana lozinka |
| status | ENUM | aktivan / deaktiviran |
| datum_kreiranja | TIMESTAMP | Automatski |
| uloga_id | FK | Referenca na Uloga |

---

### Uloga

| Atribut | Tip | Napomena |
|--------|-----|---------|
| id | INT | Primarni ključ |
| naziv | VARCHAR | Jedinstven (Član, Bibliotekar, Administrator) |
| opis | TEXT | Opcionalno |

---

### Knjiga

| Atribut | Tip | Napomena |
|--------|-----|---------|
| id | INT | Primarni ključ |
| naslov | VARCHAR | Obavezno |
| autor | VARCHAR | Obavezno |
| isbn | VARCHAR | Jedinstven |
| izdavač | VARCHAR | Opcionalno |
| godina_izdanja | INT | Validacija godine |
| kategorija_id | FK | Referenca na Kategorija |

---

### Kategorija

| Atribut | Tip | Napomena |
|--------|-----|---------|
| id | INT | Primarni ključ |
| naziv | VARCHAR | Jedinstven |
| opis | TEXT | Opcionalno |

---

### Primjerak

| Atribut | Tip | Napomena |
|--------|-----|---------|
| id | INT | Primarni ključ |
| knjiga_id | FK | Referenca na Knjiga |
| inventarni_broj | VARCHAR | Jedinstven |
| status | ENUM | dostupan / zadužen |
| datum_nabave | DATE | Opcionalno |

---

### Zaduženje

| Atribut | Tip | Napomena |
|--------|-----|---------|
| id | INT | Primarni ključ |
| korisnik_id | FK | Referenca na Korisnik |
| primjerak_id | FK | Referenca na Primjerak |
| datum_zaduživanja | DATE | Obavezno |
| datum_planiranog_vraćanja | DATE | Obavezno |
| datum_stvarnog_vraćanja | DATE | Nullable |
| status | ENUM | aktivno / zatvoreno / zakašnjelo |

---

### Članarina

| Atribut | Tip | Napomena |
|--------|-----|---------|
| id | INT | Primarni ključ |
| korisnik_id | FK | Referenca na Korisnik |
| datum_početka | DATE | Obavezno |
| datum_isteka | DATE | Obavezno |
| status | ENUM | aktivna / istekla |

---

### Rezervacija

| Atribut | Tip | Napomena |
|--------|-----|---------|
| id | INT | Primarni ključ |
| korisnik_id | FK | Referenca na Korisnik |
| knjiga_id | FK | Referenca na Knjiga |
| datum_rezervacije | DATE | Obavezno |
| datum_isteka | DATE | Obavezno |
| status | ENUM | aktivna / realizovana / otkazana / istekla |

---

### AuditLog

| Atribut | Tip | Napomena |
|--------|-----|---------|
| id |  INT | Primarni ključ |
| korisnik_id | FK | Opcionalno |
| akcija | VARCHAR | Tip akcije |
| entitet_tip | VARCHAR | Npr. knjiga, korisnik |
| entitet_id |  INT | ID entiteta |
| vrijednosti_prije | JSONB | Staro stanje |
| vrijednosti_nakon | JSONB | Novo stanje |
| datum_akcije | TIMESTAMP | Automatski |

---

## 3. Veze između entiteta

Ova sekcija opisuje kako su entiteti međusobno povezani. Relacije definišu strukturu baze podataka i način na koji se podaci povezuju i koriste u poslovnoj logici.

- **Korisnik → Uloga (N:1)**  
  Više korisnika može imati istu ulogu, ali svaki korisnik ima tačno jednu ulogu. Ova veza definiše prava pristupa u sistemu.

- **Knjiga → Kategorija (N:1)**  
  Više knjiga može pripadati jednoj kategoriji, dok svaka knjiga mora biti svrstana u tačno jednu kategoriju.

- **Primjerak → Knjiga (1:N)**  
  Jedna knjiga može imati više fizičkih primjeraka. Svaki primjerak predstavlja konkretnu kopiju knjige u biblioteci.

- **Zaduženje → Korisnik (1:N)**  
  Jedan korisnik može imati više zaduženja kroz vrijeme. Ova veza prati historiju posudbi.

- **Zaduženje → Primjerak (N:1)**  
  Svako zaduženje se odnosi na tačno jedan primjerak knjige.

- **Članarina → Korisnik (1:N)**  
  Korisnik može imati više članarina kroz vrijeme.

- **Rezervacija → Korisnik (1:N)**  
  Korisnik može imati više rezervacija uz određena ograničenja.

- **Rezervacija → Knjiga (N:1)**  
  Rezervacija se odnosi na knjigu kao naslov, a ne na konkretan primjerak.

- **AuditLog → Korisnik (N:1, opciono)**  
  Evidentira korisnika koji je izvršio akciju (ili sistem).

---

## 4. Poslovna pravila važna za model

Ova sekcija definiše ključna pravila poslovne logike koja sistem mora poštovati. Pravila osiguravaju ispravno ponašanje sistema i konzistentnost podataka.

### Korisnici
- Svaki korisnik mora imati jedinstvenu email adresu.
- Korisnik mora imati tačno jednu ulogu.
- Deaktivirani korisnici ne mogu pristupiti sistemu.
- Lozinke se čuvaju isključivo kao hash vrijednosti.

---

### Knjige i primjerci
- ISBN jedinstveno identifikuje knjigu.
- Knjiga može imati više primjeraka.
- Svaki primjerak ima jedinstven inventarni broj.
- Primjerak mora biti vezan za knjigu.
- Status određuje dostupnost primjerka.

---

### Zaduživanje
- Dozvoljeno samo za dostupne primjerke.
- Potrebna aktivna članarina.
- Nema duplog zaduživanja.
- Automatsko praćenje rokova i kašnjenja.

---

### Članarina
- Mora imati validne datume.
- Status se određuje automatski.
- Neaktivna članarina blokira zaduživanje.

---

### Rezervacije
- Dozvoljene samo ako nema dostupnih primjeraka.
- Jedna aktivna rezervacija po knjizi.
- FIFO red čekanja.
- Automatsko istecanje rezervacija.

---

### Integritet podataka
- Zabranjeno brisanje povezanih podataka.
- Soft delete se koristi.
- Historija se čuva.

---

### Audit log
- Sve kritične akcije se bilježe.
- Čuvaju se stare i nove vrijednosti.
- Dostupno samo administratorima.

---

### Transakcije
- Operacije moraju biti atomske.
- Sprječava se nekonzistentno stanje.
- Podržava se konkurentni pristup podacima.
---

# Zaključak

Use case model jasno prikazuje interakciju između aktera i sistema kroz ključne funkcionalnosti. Model je izveden direktno iz user storyja i acceptance criteria, čime se osigurava konzistentnost između zahtjeva i implementacije sistema.

Domain model dodatno proširuje razumijevanje sistema kroz definisanje ključnih entiteta, njihovih atributa, veza i poslovnih pravila. Na ovaj način se apstraktni korisnički zahtjevi prevode u strukturirani domenski prikaz koji predstavlja osnovu za dizajn baze podataka i implementaciju backend logike.

Kombinacija use case i domain modela omogućava potpunu pokrivenost sistema iz dva aspekta:
- **funkcionalnog (šta sistem radi)** kroz use case model
- **strukturnog (kako su podaci organizovani)** kroz domain model

Ovakav pristup osigurava bolju konzistentnost između poslovnih zahtjeva i tehničke implementacije, smanjuje rizik od nejasnoća u dizajnu i olakšava dalji razvoj sistema kroz jasno definisane granice odgovornosti.
