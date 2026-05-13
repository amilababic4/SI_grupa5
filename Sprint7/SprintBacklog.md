# Sprint Backlog – Sprint 7

## Opis sprinta
Sprint 7 fokusira se na unapređenje funkcionalnosti bibliotečkog sistema kroz implementaciju pretrage knjiga, prikaza detalja, te uspostavljanje sistema za evidenciju i pregled zaduženja. Cilj sprinta je omogućiti korisnicima lakše pronalaženje knjiga, dok se bibliotekarima i članovima omogućava jasan uvid u trenutni status posudbi.

Tokom ovog sprinta implementiraju se funkcionalnosti vezane za:

* pretragu knjiga po naslovu i autoru,
* pregled detaljnih bibliografskih podataka knjige,
* prikaz statusa dostupnosti i broja slobodnih primjeraka,
* upravljanje procesima zaduživanja i vraćanja knjiga,
* pregled aktivnih zaduženja za članove i bibliotekare.


<br>

| ID | Naziv stavke | Kratak opis | Prioritet | Procjena napora | Status |
|:--:| :--- | :--- | :---: | :---: | :---: |
| PB-29 | Pretraga knjiga | Pretraga knjiga po naslovu i autoru uz filtriranje rezultata i reset pretrage. | Visok | M | **Završeno** |
| PB-24 | Prikaz detalja knjige | Otvaranje stranice sa detaljnim informacijama o knjizi i obrada slučaja kada knjiga ne postoji. | Srednji | S | **Završeno** |
| PB-30 | Pregled dostupnosti knjige | Prikaz statusa dostupnosti i broja slobodnih primjeraka knjige u realnom vremenu. | Visok | M | **Završeno** |
| PB-25 | Evidencija zaduživanja i vraćanja knjiga | Upravljanje procesima posudbe i povrata knjiga po korisniku. | Visok | M | **Završeno** |
| PB-35 | Pregled vlastitih zaduženja | Član biblioteke vidi koje knjige trenutno ima zadužene. | Visok | S | **Završeno** |
| PB-36 | Pregled trenutnih zaduženja | Bibliotekar vidi koje su knjige trenutno zadužene od strane članova. | Visok | S | **Završeno** |

<br>

## Sprint Backlog stavke:

## PB-29: Pretraga knjiga

### Naziv: Pretraga po naslovu
### US-35: Kao korisnik sistema, želim da mogu pretraživati knjige po naslovu, kako bih brzo pronašao željenu knjigu.
**Acceptance Criteria:**
- Kada korisnik uđe u katalog knjiga, tada sistem prikazuje polje za pretragu
- Kada korisnik unese naslov knjige u pretragu, i klikne na dugme "Traži" tada sistem filtrira rezultate
- Pretraga nije osjetljiva na velika i mala slova
- Ako nema rezultata, sistem prikazuje poruku da knjiga nije pronađena

<br>

---
### Naziv: Pretraga po autoru
### US-36: Kao korisnik sistema, želim da mogu pretraživati knjige po autoru, kako bih pronašao sve knjige određenog autora.
**Acceptance Criteria:**
- Kada korisnik uđe u katalog knjiga, tada sistem prikazuje polje za pretragu
- Kada korisnik unese ime autora u polje za pretragu, i klikne na dugme "Traži" tada sistem filtrira rezultate
- Kada postoji više knjiga istog autora, tada se prikazuju svi rezultati
- Pretraga nije osjetljiva na velika i mala slova
- Ako ne postoji autor u sistemu, tada se prikazuje poruka da nema rezultata
---

### Naziv: Reset pretrage
### US-37: Kao korisnik, želim očistiti aktivnu pretragu, kako bih ponovo dobio prikaz svih dostupnih naslova.
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
### Naziv: Povratak sa detalja knjige na katalog
### US-39: Kao korisnik, želim se moći vratiti na katalog sa stranice detalja knjige, kako bih nastavio pregled knjiga.
**Acceptance Criteria:**
- Na stranici detalja neke knjige postoji dugme "Nazad na katalog" za povratak na katalog knjiga.
- Klik na dugme vraća korisnika na katalog.

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
- Kada korisnik otvori detalje knjige, tada sistem prikazuje status dostupnosti (Dostupno / Zaduženo) za svaki primjerak
- Kada knjiga ima slobodne primjerke, tada se prikazuje status “Dostupno”
- Kada knjiga nema slobodnih primjeraka, tada se prikazuje status “Zaduženo”
- Status primjeraka mora biti jasno vidljiv na stranici detalja knjige

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
### Naziv: Prikaza broja dostupnih primjeraka
### US-42: Kao član biblioteke, želim vidjeti koliko je primjeraka knjige trenutno dostupno, kako bih mogao planirati dolazak u biblioteku.
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


<br>

---

## PB-25: Evidencija zaduživanja i vraćanja knjiga

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
### US-46: Kao bibliotekar, želim da sistem automatski postavi rok povrata ukoliko ga ručno ne definišem, kako bih ubrzao proces zaduživanja i osigurao standardizaciju.
**Acceptance Criteria:**
- Ako bibliotekar ne unese datum, sistem automatski postavlja rok vraćanja na dva mjeseca od dana zaduživanja.
- Rok vraćanja je vidljiv u sistemu odmah nakon potvrde zaduženja.
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
- Kada je bibliotekar prijavljen u sistem, tada može otvoriti sekciju "Zaduženja". 
- Kada se sekcija otvori, tada se prikazuje lista svih aktivnih zaduženja. 
- Za svako zaduženje prikazuju se: ime i email člana, naziv knjige, primjerak, datum zaduživanja i rok vraćanja. 
- Ako ne postoje aktivna zaduženja, tada se prikazuje poruka "Nema aktivnih zaduženja".

<br>

---

### Naziv: Filtriranje aktivnih zaduženja po članu
### US-66:  Kao bibliotekar, želim filtrirati aktivna zaduženja po članovima, kako bih brže pronašao zaduženja određene osobe.
**Acceptance Criteria:**
- Kada je bibliotekar prijavljen u sistem, tada može otvoriti sekciju "Zaduženja". 
- Kada se sekcija otvori, tada se prikazuje lista svih aktivnih zaduženja. 
- Prikazana je forma za pretragu
- Bibliotekar moze filtrirati zaduženja unosom imena ili emaila člana
- Ako ne postoje aktivna zaduženja, tada se prikazuje poruka "Nema aktivnih zaduženja za zadani filter".

<br>

---

### Naziv: Otvaranje detalja aktivnog zaduženja
### US-67:  Kao bibliotekar, želim otvoriti detalje aktivnog zaduženja, kako bih mogao brzo vidjeti kome pripada i koji primjerak je u pitanju.
**Acceptance Criteria:**
- Klik na zapis zaduženja otvara detaljni prikaz.
- Detalji prikazuju člana, knjigu, primjerak, datum zaduženja i rok vraćanja.
- Iz detalja je moguće evidentirati vraćanje knjige.
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