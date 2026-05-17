# Sprint Backlog – Sprint 8

## Opis sprinta

Sprint 8 fokusira se na razvoj funkcionalnosti vezanih za upravljanje korisnicima, profilima članova i evidencijom članarina unutar bibliotečkog sistema. Cilj sprinta je omogućiti administratorima i bibliotekarima efikasnije upravljanje korisničkim podacima, dok članovima biblioteke pruža jasan pregled vlastitog profila, zaduženja i statusa članarine.

Tokom ovog sprinta implementiraju se funkcionalnosti vezane za:

* pregled i upravljanje korisničkim profilima,
* pregled historije i trenutnih zaduženja članova,
* administraciju korisničkih naloga i uloga,
* deaktivaciju i zaštitu administratorskih naloga,
* evidenciju, pregled i ažuriranje članarina,
* prikaz statusa članarine za članove biblioteke.


<br>

| ID | Naziv stavke | Kratak opis | Prioritet | Procjena napora | Status |
|:--:| :--- | :--- | :---: | :---: | :---: |
| PB-20 | Pregled profila člana | Pregled osnovnih podataka korisnika i trenutno zaduženih knjiga putem profila člana. | Visok | S | **Završeno** |
| PB-25 | Evidencija vraćanja knjiga | Omogućavanje bibliotekarima evidenciju vraćenih knjiga i oslobađanje zaduženih primjeraka. | Visok | M | **Završeno** |
| PB-37 | Pregled historije zaduženja | Bibliotekar može pregledati ranija zaduženja člana i evidenciju vraćenih knjiga. | Nizak | S | **Završeno** |
| PB-32 | Upravljanje korisnicima od strane admina | Administrator može pregledati, pretraživati i upravljati korisničkim nalozima i ulogama. | Srednji | L | **Završeno** |
| PB-33 | Upravljanje statusom članarine | Bibliotekar može evidentirati i ažurirati članarine članova biblioteke. | Srednji | M | **Završeno** |
| PB-34 | Pregled statusa članarina za člana | Član biblioteke može vidjeti status i datum isteka svoje članarine. | Srednji | S | **Završeno** |

<br>

## Sprint Backlog stavke:
## PB-20: Pregled profila člana

### Naziv: Prikaz osnovnih podataka profila
### US-48: Kao registrovani korisnik (član, bibliotekar ili administrator), želim pregledati svoj profil, kako bih imao uvid u svoje lične podatke i status u biblioteci.
**Acceptance Criteria:**
- Kada korisnik klikne na dugme svog imena i prezimena, tada sistem otvori stranicu sa detaljima korisnika
- Kada se otvori profil člana, sistem prikazuje: ime, prezime, email, ulogu, status i datum kreiranja profila.
- Sistem mora prikazati jasno strukturirane osnovne podatke člana.

<br>

---

### Naziv: Prikaz aktivnih zaduženja člana
### US-62: Kao bibliotekar, želim pregledati aktivna zaduženja člana biblioteke, kako bih imao uvid u trenutno stanje fonda i mogao pravovremeno reagovati u slučaju prekoračenja roka.
**Acceptance Criteria:**
- Kada bibilotekar uđe u profil korisnika, tada sistem prikazuje osnovne informacije o korisniku.
- Kada bibliotekar otvori sekciju "Aktivna zaduženja" na profilu člana, tada sistem prikazuje listu svih aktivnih zaduženja
- Kada postoje zaduženja, tada se prikazuje naziv knjige, primjerak, datum zaduženja i rok vraćanja.
- Kada član nema aktivnih zaduženja, tada sistem prikazuje poruku "Ovaj član nema aktivnih zaduženja."

<br>

---
| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava korisnicima i osoblju biblioteke da brzo pristupe informacijama o članovima i njihovim posuđenim knjigama, što olakšava upravljanje članstvom i praćenje zaduženja. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi evidenciju članova i njihovih zaduženja. <br> Član može vidjeti samo svoj profil. |
 **Veze i zavisnosti** | Domain Model (entitet o korisnicima i zaduženjima).<br> PB-17 Sistem prijave <br> PB-35 Pregled vlastitih zaduženja.  |
 ---

<br>

## PB-25: Evidencija vraćanja knjiga

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

| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava tačno praćenje kretanja knjiga u biblioteci, sprječava gubitak primjeraka i omogućava ažurnu evidenciju zaduženja i vraćanja. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi evidenciju članova i knjiga.  <br> Definisan poslovni rok zaduženja. |
 **Veze i zavisnosti** | PB-26 Upravljanje primjercima knjige. <br> Prikaz detalja knjige. <br> Pregled dostupnosti knjige. |
---

<br>

## PB-37: Pregled historije zaduženja

### Naziv: Pregled prethodnih zaduženja
### US-54: Kao bibliotekar, želim pregledati ranija zaduženja člana, kako bih mogao pratiti korištenje fonda i donositi odluke o zaduživanju ili opomenama.
**Acceptance Criteria:**
- Kada bibliotekar otvori sekciju "Historija zaduženja" na profilu člana, tada sistem prikazuje listu svih ranijih zaduženja
- Kada postoje zaduženja, tada se prikazuje naziv knjige, primjerak, datum zaduženja i datum vraćanja.
- Kada član nema historiju zaduženja, tada sistem prikazuje poruku "Ovaj član nema zatvorenih zaduženja u posljednje 3 godine.".
- Sistem mora prikazati sve zapise bez aktivnih zaduženja.

<br>

---

### Naziv: Pregled svih završenih zaduženja
### US-61: Kao bibliotekar, želim imati pristup historiji svih zaduženja kako bih mogao pratiti cirkulaciju knjiga.

**Acceptance Criteria:**
- Kada bibliotekar u sekciji "Zaduženja" klikne na dugme "Historija zaduženja", sistem prikazuje spisak svih zatvorenih zaduženja.
- Za svaki zapis u historiji sistem prikazuje: ime člana, email, naziv knjige, inventarni broj, datume zaduženja/roka i zeleno istaknut datum povrata.
- Sistem mora omogućiti pretragu historije po imenu ili emailu člana te prikazati ukupan broj zapisa.

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava uvid u prethodna zaduženja članova biblioteke, što pomaže bibliotekarima u praćenju ponašanja korisnika, analizi korištenja fonda i donošenju odluka o budućim zaduženjima. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi kompletnu evidenciju svih zaduženja i vraćanja. <br> Da li član može vidjeti svoju historiju ili samo bibliotekar? |
 **Veze i zavisnosti** | PB-31 Evidencija zaduživanja i vraćanja. <br> PB-20 Pregled profila člana. |
 ---

 <br>

 ## PB-32: Upravljanje korisnicima od strane admina

### Naziv: Pregled korisnika
### US-49: Kao administrator, želim vidjeti sve registrovane korisnike sistema, kako bih imao uvid u korisničku bazu.
**Acceptance Criteria:**
- Kada administrator otvori sekciju "Članovi", tada sistem prikazuje listu svih korisnika.
- Kada se lista učita, tada se prikazuju osnovni podaci: ime, prezime, email, uloga, datum kreiranja profila.
- Kada postoji veliki broj korisnika, tada sistem omogućava scroll.

<br>

--- 

### Naziv: Pretraga korisnika u admin sekciji
### US-50: Kao administrator, želim pretraživati korisnike po imenu ili emailu, kako bih brže pronašao željeni nalog.
**Acceptance Criteria:**
- Sekcija "Članovi" sadrži polje za pretragu.
- Pretraga radi po imenu, prezimenu ili emailu.
- Rezultati se filtriraju nakon potvrde.
- Ako nema rezultata, prikazuje se poruka "Nema rezultata".

<br>

--- 

### Naziv: Izmjena uloga korisnika
### US-51: Kao administrator, želim promijeniti ulogu korisnika kako bih upravljao pristupom funkcionalnostima sistema.
**Acceptance Criteria:**
- Kada administrator klikne na korisnika iz liste, tada se otvara detaljni prikaz korisnika.
- Kada administrator odabere opciju "Promijeni ulogu", tada se prikazuje dropdown sa ulogama.
- Kada administrator potvrdi promjenu, tada se nova uloga sprema u sistem.
- Nakon promjene, korisnik odmah ima novu ulogu pri sljedećem pristupu sistemu.

<br>

---
### Naziv: Deaktivacija korisnika
### US-52: Kao administrator, želim deaktivirati korisnika kako bih onemogućio njegov dalji pristup sistemu.
**Acceptance Criteria:**
- Kada administrator otvori profil korisnika, tada postoji opcija "Deaktiviraj nalog".
- Kada administrator klikne na deaktivaciju, tada sistem traži potvrdu akcije.
- Kada se nalog deaktivira, tada korisnik više ne može pristupiti sistemu.
- Sistem ne smije dozvoliti prijavu deaktiviranom korisniku.


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

## PB-33: Upravljanje statusom članarine

### Naziv: Pregled statusa članarina
### US-55: Kao bibliotekar, želim otvoriti profil člana i vidjeti status njegove članarine kako bih znao da li ima pravo korištenja usluga biblioteke.
**Acceptance Criteria:**
- Kada bibliotekar otvori profil člana, tada sistem prikazuje sekciju "Članarina".
- Kada postoji članarina, tada se prikazuje datum isteka članarine, tj. do kad važi članarina.
- Kada članarina ne postoji, tada se prikazuje poruka "Članarina nije evidentirana".
- Status članarine mora biti jasno vidljiv na profilu člana.

<br>

---

### Naziv: Evidentiranje nove članarine
### US-56: Kao bibliotekar, želim evidentirati novu članarinu za člana, kako bi sistem znao da član ima pravo korištenja usluga.
**Acceptance Criteria:**
- Bibliotekar može otvoriti formu za članarinu sa profila člana.
- Forma sadrži datum početka i datum isteka.
- Nakon spremanja, članarina je vidljiva na profilu.
- Ako su datumi neispravni, spremanje nije dozvoljeno.


<br>

---

### Naziv: Ažuriranje postojeće članarine
### US-57: Kao bibliotekar, želim produžiti ili ispraviti postojeću članarinu člana kako bih osigurao tačne informacije o njenom važenju.
**Acceptance Criteria:**
- Kada bibliotekar klikne na opciju "Upravljanje članarinom", tada se otvara forma.
- Kada bibliotekar unese datum početka i datum isteka, tada sistem sprema članarinu.
- Sistem mora validirati da datum isteka nije prije datuma početka.
- Nakon spremanja, izmjene su odmah vidljive na profilu člana.

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava bibliotekarima da kontrolišu pristup uslugama biblioteke kroz praćenje i upravljanje članarinama. |
| **Pretpostavke / Otvorena pitanja** | Bibliotekar ili administrator je prijavljen u sistem i član već postoji u evidenciji. |
 **Veze i zavisnosti** | PB-14 Pregled profila člana. <br> PB-26 Upravljanje korisnicima. <br> PB-20 Profil člana |

---

<br>

## PB-34: Pregled statusa članarina za člana

### Naziv: Vizuelni indikator statusa članstva
### US-58: Kao član biblioteke, želim otvoriti svoj profil i vidjeti da li mi je članarina aktivna kako bih znao da li mogu koristiti usluge biblioteke.
**Acceptance Criteria:**
- Kada se član prijavi u sistem, tada može otvoriti svoj profil klikom na dugme svog imena i prezimena.
- Kada član otvori profil, tada sistem prikazuje status članarine.
- Kada je članarina aktivna, tada se prikazuje status "Aktivna".
- Kada je članarina istekla, tada se prikazuje status "Istekla".
- Status mora biti jasno vidljiv na profilu člana.

<br>

---

### Naziv: Prikaz datuma isteka članarine
### US-59: Kao član biblioteke, želim vidjeti do kada važi moja članarina kako bih znao kada je potrebno produženje.
**Acceptance Criteria:**
- Kada član otvori svoj profil, tada sistem prikazuje datum isteka članarine
- Kada datum isteka postoji, tada se prikazuje u jasnom formatu (DD.MM.YYYY)
- Kada članarina ne postoji, tada se prikazuje poruka da članarina nije aktivna
- Datum isteka mora biti vidljiv zajedno sa statusom članarine

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava članovima biblioteke jasan uvid u status i trajanje članarine, čime se povećava transparentnost i smanjuje potreba za kontaktiranjem bibliotekara. |
| **Pretpostavke / Otvorena pitanja** | Sistem vodi evidenciju članarine sa datumom isteka. |
 **Veze i zavisnosti** | PB-33 Upravljanje statusom članarine. <br> PB-20 Pregled profila člana. |
