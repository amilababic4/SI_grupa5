# Sprint Backlog – Sprint 7

## Opis sprinta

Sprint 7 fokusira se na unapređenje funkcionalnosti bibliotečkog kataloga kroz implementaciju pretrage knjiga, prikaza detalja knjige i pregleda dostupnosti primjeraka.  
Cilj sprinta je omogućiti korisnicima jednostavnije pronalaženje knjiga i jasan pregled njihove trenutne dostupnosti u sistemu.

Tokom ovog sprinta implementiraju se funkcionalnosti vezane za:

* pretragu knjiga po naslovu, autoru i ključnoj riječi
* pregled detaljnih bibliografskih podataka knjige
* prikaz statusa dostupnosti knjige
* izračun i prikaz broja slobodnih primjeraka

<br>

| ID | Naziv stavke | Kratak opis | Prioritet | Procjena napora | Status |
|:--:| :--- | :--- | :---: | :---: | :---: |
| PB-29 | Pretraga knjiga | Pretraga knjiga po naslovu, autoru i ključnoj riječi uz filtriranje rezultata i reset pretrage. | Visok | M | **Završeno** |
| PB-24 | Prikaz detalja knjige | Otvaranje stranice sa detaljnim informacijama o knjizi i obrada slučaja kada knjiga ne postoji. | Srednji | S | **Završeno** |
| PB-30 | Pregled dostupnosti knjige | Prikaz statusa dostupnosti i broja slobodnih primjeraka knjige u realnom vremenu. | Visok | M | **Završeno** |

<br>

## Sprint Backlog stavke:

## PB-29: Pretraga knjiga
### Naziv: Pretraga po naslovu
### US-34: Kao korisnik sistema, želim da mogu pretraživati knjige po naslovu, kako bih brzo pronašao željenu knjigu.

**Acceptance Criteria:**
- Kada korisnik uđe u katalog knjiga, tada sistem prikazuje polje za pretragu
- Kada korisnik unese naslov knjige u pretragu i klikne na dugme "Traži", tada sistem filtrira rezultate
- Pretraga nije osjetljiva na velika i mala slova
- Ako nema rezultata, sistem prikazuje poruku da knjiga nije pronađena

<br>

---

### Naziv: Pretraga po ključnoj riječi
### US-35: Kao korisnik sistema, želim da mogu pretraživati knjige po naslovu, kako bih brzo pronašao željenu knjigu.
**Acceptance Criteria:**
- Kada korisnik uđe u katalog knjiga, tada sistem prikazuje polje za pretragu
- Kada korisnik unese naslov knjige u pretragu, i klikne na dugme "Traži" tada sistem filtrira rezultate
- Pretraga nije osjetljiva na velika i mala slova
- Ako nema rezultata, sistem prikazuje poruku da knjiga nije pronađena

<br>

---
### Naziv: Pretraga po autorima
### US-36: Kao korisnik sistema, želim da mogu pretraživati knjige po autoru, kako bih pronašao sve knjige određenog autora.
**Acceptance Criteria:**
- Kada korisnik uđe u katalog knjiga, tada sistem prikazuje polje za pretragu
- Kada korisnik unese ime autora u polje za pretragu, i klikne na dugme "Traži" tada sistem filtrira rezultate
- Kada postoji više knjiga istog autora, tada se prikazuju svi rezultati
- Pretraga nije osjetljiva na velika i mala slova
- Ako ne postoji autor u sistemu, tada se prikazuje poruka da nema rezultata
---

### Naziv: Reset pretrage
### US-37: Kao korisnik, želim očistiti aktivnu pretragu, kako bih se brzo vratio na puni katalog.
**Acceptance Criteria:**
- Nakon pretrage postoji opcija "Reset".
- Klik na "Reset" vraća kompletnu listu knjiga.
- Polje za pretragu se prazni.

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
- Kada tražena knjiga ne postoji, sistem prikazuje poruku o grešci ili “Knjiga nije pronađena”.
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

### Naziv: Jasan indikator dostupnosti
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

