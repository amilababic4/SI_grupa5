# Test Strategy

## Cilj testiranja
Cilj testiranja bibliotečkog informacionog sistema je osigurati da sve implementirane funkcionalnosti rade u skladu sa prethodno definisanim zahtjevima i acceptance kriterijima.

Testiranjem se nastoji potvrditi ispravnost osnovnih funkcionalnosti (registracija, prijava, upravljanje knjigama, zaduživanje), osigurati tačnost i konzistentnost podataka u sistemu, provjeriti međusobnu integraciju različitih modula sistema, te identifikovati i evidentirati greške prije puštanja samog sistema u upotrebu.

Uz različite tehnike testiranja za krajnji cilj želi se postići stabilan, pouzdan i funkcionalan sistem spreman za korištenje od strane krajnjih korisnika.

# Nivoi testiranja
## Unit testiranje
Unit testiranje obuhvata testiranje pojedinačnih komponenti sistema, kao što su funkcije, smatrajući da su izolovane od ostatka sistema. Cilj ovog nivoa testiranja je provjeriti ispravnost implementirane logike prije same integracije u veće cjeline.

Bitno je napomenuti da Unit testiranje koje se vrši u toku same implementacije omogućava rano otkrivanje grešaka i smanjuje rizik od problema u kasnijim fazama razvoja, posebno tokom integracionog i sistemskog testiranja.

Radi lakše preglednosti, Unit testiranje će biti grupisano na osnovu međusobno povezanih User Stories, koji su prethodno definisani, uz navedene pripadajuće Acceptance Criterie.



### Unit 1 - Registracija člana (US-01, US-02, US-03)

**Acceptance Criteria:**  
Kada email adresa nije u ispravnom formatu, tada se prikazuje poruka o grešci

| Test Case | Ulaz              | Očekivani rezultat                         |
|----------|------------------|-------------------------------------------|
| UT-01    | 'ime@gmail.com' |  validan                                 |
| UT-02    | "imegmail.com"  | greška: nedostaje @                    |
| UT-03    | "ime@"          | greška: nema domene                    |
| UT-04    | "" (prazno)       | greška: obavezno polje                 |
| UT-05    | "ime@gmail"     | greška: nema TLD                |
---
<br>

**Acceptance Criteria:**  
Kada unesena lozinka ima manje od 8 znakova, tada se prikazuje poruka da lozinka nije dovoljno duga

| Test Case | Ulaz              | Očekivani rezultat                         |
|----------|------------------|-------------------------------------------|
| UT-06    | "Lozinka1"        |  validan                                 |
| UT-07    | "abc123"          |  greška: prekratka                      |
| UT-08    | "ab"              |  greška: prekratka                      |
| UT-09    | "" (prazno)       |  greška: obavezno polje                 |
| UT-10    | "12345678"        |  validan                                 |

---
<br>

**Acceptance Criteria:**  
"Sistem ne smije dozvoliti nastavak registracije bez unosa obaveznih podataka"

| Test Case | Ulaz                     | Očekivani rezultat              |
|----------|--------------------------|---------------------------------|
| UT-11    | Sva polja popunjena      |  validan           |
| UT-12    | Ime prazno               |  greška: ime obavezno        |
| UT-13    | Prezime prazno           |  greška: prezime obavezno    |
| UT-14    | Email prazno             |  greška: email obavezan      |
| UT-15    | Lozinka prazna           |  greška: lozinka obavezna    |
| UT-16    | Sva polja prazna         |  greška: sva polja su obavezna        |

<br>

### Unit 2 - Prijava korisnika (US-04, US-05)

**Acceptance Criteria:**  
Kada korisnik unese pogrešan email ili lozinku, tada se prijava odbija;<br>
Sistem ne prikazuje da li je greška u emailu ili u lozinci

| Test Case | Situacija           | Očekivani rezultat                                  |
|----------|--------------------|-----------------------------------------------------|
| UT-17    | Pogrešan email     | "Email ili lozinka nisu ispravni"                   |
| UT-18    | Pogrešna lozinka   | "Email ili lozinka nisu ispravni"                   |
| UT-19    | Oba pogrešna       | "Email ili lozinka nisu ispravni"                   |

<br>

### Unit 3 - Dodavanje/validacija knjige (US-09)

**Acceptance Criteria:**  
Kada ISBN već postoji ili nije validan, tada sistem odbija unos

| Test Case | Ulaz                     | Očekivani rezultat                  |
|----------|--------------------------|-------------------------------------|
| UT-20    | "978-3-16-148410-0"      |  validan format                   |
| UT-21    | "12345"                  |  greška: prekratak               |
| UT-22    | "ABCDEFGHIJ"             |  greška: nije numerički          |
| UT-23    | "" (prazno)              |  greška: obavezno polje          |

---


**Acceptance Criteria:**  
Kada obavezni podaci nedostaju, tada sistem prikazuje poruku o grešci

| Test Case | Ulaz                 | Očekivani rezultat |
|----------|----------------------|--------------------|
| UT-24    | Sva polja popunjena  |  validan          |
| UT-25    | Naslov prazan        |  greška           |
| UT-26    | Autor prazan         |  greška           |
| UT-27    | Broj primjeraka = 0  |  greška           |

<br>

### Unit 4 - Upravljanje kategorijama (US-21, US-22)

**Acceptance Criteria:**  
Kada kategorija već postoji, tada sistem prikazuje poruku o grešci; <br> Kada je naziv prazan, tada sistem prikazuje grešku

| Test Case | Ulaz                         | Očekivani rezultat              |
|----------|------------------------------|---------------------------------|
| UT-28    | "Roman", lista bez "Roman"   |  validan                      |
| UT-29    | "" (prazno)                  |  greška: naziv obavezan      |
| UT-30    | "Roman", lista sadrži "Roman"|  greška: kategorija već postoji         |
| UT-31    | "roman", lista sadrži "Roman"|  greška: case-insensitive    |

<br>

### Unit 5 - Upravljanje članarinom (US-38, US-39)

**Acceptance Criteria:**  
Sistem mora validirati da datum isteka nije prije datuma početka

| Test Case | Ulaz                                      | Očekivani rezultat           |
|----------|--------------------------------------------|------------------------------|
| UT-32    | start: 01.01.2025, end: 01.01.2026         |  validan                   |
| UT-33    | start: 01.01.2025, end: 31.12.2024         |  greška: kraj prije početka |
| UT-34    | isti datumi                                |  greška                    |
| UT-35    | start: prazno                              |  greška                    |
| UT-36    | end: prazno                                |  greška                    |

---

**Acceptance Criteria:**  
Kada je članarina aktivna, tada se prikazuje status 'Aktivna'

| Test Case | Ulaz                              | Očekivani rezultat |
|----------|-----------------------------------|--------------------|
| UT-37    | end: 31.12.2026, today: 12.04.2026|  Aktivna          |
| UT-38    | end: 01.01.2026, today: 12.04.2026|  Istekla          |
| UT-39    | isti dan                          |  Aktivna |

<br>

### Unit 6 - Rezervacija knjiga (US-47)

**Acceptance Criteria:**  
Sistem ne smije dozvoliti rezervaciju knjige koja nema dostupne primjerke;<br>Član ne može napraviti više aktivnih rezervacija iste knjige

| Test Case | Ulaz                                              | Očekivani rezultat         |
|----------|--------------------------------------------------------|----------------------------|
| UT-40    | Dostupni primjerci, nema aktivne rezervacije                      |  dozvoljeno              |
| UT-41    | Ne postoje dostupni primjerci                             |  zabranjeno              |
| UT-42    | Ima dostupnih primjeraka, ali već postoji rezervacija           |  zabranjeno       |

<br>

### Unit 7 - Brisanje knjige (US-19)

**Acceptance Criteria:**  
Kada knjiga ima aktivno zaduženje, tada sistem ne dozvoljava brisanje

| Test Case | Situacija                | Očekivani rezultat     |
|----------|--------------------------|------------------------|
| UT-43    | Svi primjerci slobodni   |  dozvoljeno           |
| UT-44    | Jedan primjerak zadužen  |  zabranjeno           |
| UT-45    | Nema primjeraka          |  dozvoljeno           |

### Rizici Unit testiranja
Iako Unit testiranje omogućava rano otkrivanje grešaka i provjeru ispravnosti pojedinačnih komponenti, sa sobom nosi rizike i ograničenja.

Pri tome, glavni rizici uključuju nemogućnost otkrivanja problema u interakciji između različitih komponenti sistema što sa sobom povlači potencijalno stvaranje lažnog osjećaja sigurnosti, jer svi Unit testovi mogu proći iako sistem u cjelini ne funkcioniše ispravno.
Sa druge strane, tu je potreba za stalnim održavanjem testova prilikom izmjena u kodu kao i moguća nedovoljna pokrivenost testovima ukoliko nisu obuhvaćeni svi scenariji kojih za određene funkcionalnosti može biti iznimno veći broj.