# Sprint Backlog – Sprint 9

## Opis sprinta

Sprint 9 fokusira se na unapređenje funkcionalnosti rezervacije knjiga, napredne pretrage kataloga i generisanja administrativnih izvještaja unutar SmartLib sistema. Cilj sprinta je poboljšati korisničko iskustvo članova biblioteke kroz efikasnije pronalaženje i rezervaciju knjiga, te omogućiti upravi biblioteke detaljniji pregled aktivnosti sistema putem mjesečnih izvještaja.

Tokom ovog sprinta implementiraju se funkcionalnosti vezane za:

* rezervaciju knjiga koje trenutno nisu dostupne,
* pregled i otkazivanje aktivnih rezervacija,
* filtriranje knjiga po kategoriji, izdavaču i godini izdanja,
* kombinovanje više filtera prilikom pretrage kataloga,
* generisanje mjesečnih izvještaja o zaduživanjima, rezervacijama i članovima,
* forum zajednicu

<br>

| ID | Naziv stavke | Kratak opis | Prioritet | Procjena napora | Status |
|:--:| :--- | :--- | :---: | :---: | :---: |
| PB-39 | Rezervacija knjiga | Omogućava članovima rezervaciju nedostupnih knjiga, pregled i upravljanje vlastitim rezervacijama. | Srednji | M | **Završeno** |
| PB-40 | Pregled aktivnih rezervacija | Bibliotekar može pregledati sve aktivne rezervacije u sistemu. | Srednji | S | **Završeno** |
| PB-43 | Automatsko otkazivanje rezervacije | Sistem automatski prati rok trajanja rezervacija i otkazuje istekle rezervacije. | Nizak | M | **Završeno** |
| PB-44 | Napredna pretraga i filteri | Filtriranje knjiga po kategoriji, izdavaču i godini uz mogućnost kombinovanja filtera. | Nizak | M | **Završeno** |
| PB-45 | Mjesečni izvještaji za upravu | Generisanje mjesečnih izvještaja o zaduživanjima, rezervacijama i članovima biblioteke. | Nizak | L | **Završeno** |
| PB-57 | Pregled i filtriranje forumskih objava | Pregled i filtriranje objava u forumu | Nizak | M | **Završeno** |
| PB-58 | Kreiranje i interakcija sa forumskim objavama | Korisnici mogu komentarisati postojeće forum objave i učestvovati u diskusiji | Nizak | M | **Završeno** |
| PB-59 | Moderacija forumskog sadržaja | Administrator ili bibliotekar može obrisati neprimjerene forum objave i komentare | Nizak | S | **Završeno** |

<br>

## Sprint Backlog stavke:
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
- Kada član klikne na dugme "Otkaži" za neku knjigu, tada se rezervacija uklanja iz sistema. 
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
- Kada je bibliotekar prijavljen u sistem, tada može otvoriti sekciju "Rezervacije".
- Kada se sekcija otvori, tada se prikazuje lista svih aktivnih rezervacija. 
- Za svaku rezervaciju prikazuju se: ime i prezime člana, email člana, naslov knjige i datum rezervacije. 
- Ako nema aktivnih rezervacija, tada se prikazuje poruka "Nema aktivnih rezervacija". 
- Sistem ne smije prikazivati otkazane ili realizovane rezervacije.

<br>

---

### Naziv: Pretraga aktivnih rezervacija
### US-81: Kao bibliotekar, želim pretraživati aktivne rezervacije po imenu člana, email adresi ili naslovu knjige, kako bih brže pronašao tražene rezervacije.

**Acceptance Criteria:**
- Kada bibliotekar otvori sekciju "Rezervacije", tada se na vrhu stranice prikazuje forma za pretragu rezervacija.
- Forma za pretragu sadrži polje za unos pojma pretrage i dugme "Traži".
- Kada bibliotekar unese ime člana, email člana ili naslov knjige i klikne na dugme "Traži", tada sistem prikazuje samo rezervacije koje odgovaraju unesenom kriteriju.
- Ako nema rezultata pretrage, tada se prikazuje poruka "Nema pronađenih rezervacija".
- Sistem ne smije prikazivati otkazane ili realizovane rezervacije u rezultatima pretrage.

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava bibliotekarima bolju kontrolu i pregled nad rezervacijama, što poboljšava organizaciju posudbi i upravljanje fondom.  |
| **Pretpostavke / Otvorena pitanja** | Sistem podržava evidenciju rezervacija. <br> Da li će postojati historija rezervacija ili samo aktivne? |
 **Veze i zavisnosti** | Rezervacija knjiga. |

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

## PB-45: Mjesečni izvještaji za upravu

### Naziv: Izbor mjeseca i godine za izvještaj
### US-87: Kao bibliotekar ili administrator, želim izabrati tačan mjesec i godinu za izvještaj, kako bih generisao pregled za željeni period.
**Acceptance Criteria:**
- Forma za izvještaj omogućava izbor mjeseca.
- Forma omogućava izbor godine.
- Bez izbora perioda nije moguće generisati izvještaj.
- Izvještaj odgovara odabranom periodu.

<br>

---
### Naziv: Mjesečni izvještaj o zaduživanjima knjiga
### US-88: Kao bibliotekar ili administrator biblioteke, želim generisati mjesečni izvještaj o zaduživanjima knjiga kako bih mogao pratiti korištenje bibliotečkog fonda.
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
### US-89: Kao bibliotekar ili administrator biblioteke, želim generisati mjesečni izvještaj o rezervacijama knjiga kako bih imao uvid u potražnju za knjigama.
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
### US-90: Kao bibliotekar ili administrator biblioteke, želim generisati mjesečni izvještaj o članovima kako bih pratio aktivnost i stanje članstva.
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

## PB-57: Pregled i filtriranje forumskih objava

### Naziv: Pregled forumskih objava
### US-112: Kao korisnik sistema (član, bibliotekar ili administrator), želim pregledati sve objave na forumu kako bih mogao pratiti diskusije i aktivnosti zajednice.
**Acceptance Criteria:**
- Kada korisnik otvori sekciju "Forum", tada sistem prikazuje listu svih objava.
- Kada se objave učitaju, tada se prikazuju u obliku kartica.
- Na svakoj kartici sistem prikazuje naslov objave, autora, kategoriju, broj reakcija i broj komentara.
- Sistem mora omogućiti scroll ukoliko postoji veliki broj objava.

<br>

---

### Naziv: Filtriranje objava po kategoriji
### US-113: Kao korisnik sistema, želim filtrirati objave prema kategorijama kako bih lakše pronašao relevantne diskusije.
**Acceptance Criteria:**
- Kada korisnik otvori sekciju "Forum", tada sistem prikazuje kategorije: "Opšta diskusija", "Preporuke knjiga", "Pitanja", "Recenzije" i "Sve objave".
- Kada korisnik odabere određenu kategoriju, tada sistem prikazuje samo objave iz te kategorije.
- Kada korisnik odabere opciju "Sve objave", tada sistem prikazuje sve objave bez filtriranja.
- Aktivna kategorija mora biti jasno vizuelno označena.

<br>

---

### Naziv: Pregled detalja objave
### US-114: Kao korisnik sistema, želim otvoriti pojedinačnu objavu kako bih mogao pregledati kompletan sadržaj i komentare.
**Acceptance Criteria:**
- Kada korisnik klikne na karticu objave, tada sistem otvara detaljan prikaz objave.
- Detaljan prikaz sadrži naslov, autora, kategoriju, datum objave i kompletan sadržaj objave.
- Sistem prikazuje listu svih komentara vezanih za objavu.
- Broj reakcija i komentara mora biti vidljiv na stranici objave.

<br>

---

| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava korisnicima jednostavan pregled i pretragu sadržaja foruma, čime se podstiče aktivnost i komunikacija unutar bibliotečke zajednice. |
| **Pretpostavke / Otvorena pitanja** | Objave postoje u sistemu i povezane su sa registrovanim korisnicima. |
| **Veze i zavisnosti** | PB-17 Sistem prijave. <br> Modul korisničkih uloga i autorizacije. |
---

<br>

## PB-58: Kreiranje i interakcija sa forumskim objavama

### Naziv: Kreiranje nove objave
### US-115: Kao korisnik sistema (član, bibliotekar ili administrator), želim kreirati novu objavu na forumu kako bih mogao učestvovati u diskusijama zajednice.
**Acceptance Criteria:**
- Kada korisnik klikne na dugme "Nova objava", tada sistem otvara formu za kreiranje objave.
- Forma sadrži polja: kategorija, naslov i sadržaj objave.
- Kada korisnik klikne na dugme "Objavi na forumu", tada sistem objavljuje novu objavu.
- Nakon uspješne objave, nova objava je odmah vidljiva na forumu.
- Sistem ne dozvoljava objavu ukoliko neka obavezna polja nisu unesena.

<br>

---

### Naziv: Reagovanje na objavu
### US-116: Kao korisnik sistema, želim lajkovati objavu kako bih mogao pokazati da mi se sadržaj sviđa ili da je koristan.
**Acceptance Criteria:**
- Kada korisnik otvori objavu, tada sistem prikazuje opciju za reakciju "Like".
- Kada korisnik klikne na dugme za reakciju, tada sistem evidentira reakciju.
- Nakon reakcije, broj lajkova se ažurira.
- Jedan korisnik može ostaviti samo jednu reakciju na istu objavu.

<br>

---

### Naziv: Dodavanje komentara na objavu
### US-117: Kao korisnik sistema, želim komentarisati objavu kako bih mogao učestvovati u diskusiji sa drugim korisnicima.
**Acceptance Criteria:**
- Kada korisnik otvori objavu, tada sistem prikazuje formu za unos komentara.
- Kada korisnik unese komentar i klikne na dugme "Objavi komentar", tada sistem sprema komentar.
- Novi komentar se odmah prikazuje ispod objave.
- Sistem prikazuje autora i datum komentara.
- Sistem ne dozvoljava objavu praznog komentara.

<br>

---

| **Prioritet** | Visok |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava korisnicima aktivno učešće u zajednici kroz objavljivanje sadržaja, komentarisanje i reakcije na objave. |
| **Pretpostavke / Otvorena pitanja** | Korisnik mora biti prijavljen u sistem kako bi mogao kreirati objave i komentare. |
| **Veze i zavisnosti** | PB-17 Sistem prijave. <br> Modul korisničkih uloga i autorizacije. |
---

<br>

## PB-59: Moderacija forumskog sadržaja

### Naziv: Brisanje komentara od strane moderatora
### US-118: Kao bibliotekar ili administrator, želim obrisati neprimjeren komentar kako bih održavao kvalitet diskusije na forumu.
**Acceptance Criteria:**
- Kada bibliotekar ili administrator otvori objavu, tada sistem prikazuje opciju za brisanje komentara.
- Kada moderator klikne na opciju "Obriši komentar", tada sistem traži potvrdu akcije.
- Nakon potvrde, komentar se uklanja iz sistema.
- Član biblioteke nema mogućnost brisanja komentara drugih korisnika.

<br>

---

| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava bibliotekarima i administratorima održavanje sigurnog i kvalitetnog prostora za komunikaciju korisnika. |
| **Pretpostavke / Otvorena pitanja** | Bibliotekar i administrator imaju moderatorske privilegije nad forumskim sadržajem. |
| **Veze i zavisnosti** | PB-58 Kreiranje i interakcija sa forumskim objavama. <br> Modul korisničkih uloga i autorizacije. |
---
