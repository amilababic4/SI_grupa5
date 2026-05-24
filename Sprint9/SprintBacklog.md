# Sprint Backlog – Sprint 9

## Opis sprinta

Sprint 9 fokusira se na unapređenje funkcionalnosti rezervacije knjiga, napredne pretrage kataloga i generisanja administrativnih izvještaja unutar SmartLib sistema. Cilj sprinta je poboljšati korisničko iskustvo članova biblioteke kroz efikasnije pronalaženje i rezervaciju knjiga, te omogućiti upravi biblioteke detaljniji pregled aktivnosti sistema putem mjesečnih izvještaja.

Tokom ovog sprinta implementiraju se funkcionalnosti vezane za:

* rezervaciju knjiga koje trenutno nisu dostupne,
* pregled i otkazivanje aktivnih rezervacija,
* automatsko upravljanje istekom rezervacija,
* filtriranje knjiga po kategoriji, izdavaču i godini izdanja,
* kombinovanje više filtera prilikom pretrage kataloga,
* generisanje mjesečnih izvještaja o zaduživanjima,
* generisanje izvještaja o rezervacijama i članovima biblioteke.

<br>

| ID | Naziv stavke | Kratak opis | Prioritet | Procjena napora | Status |
|:--:| :--- | :--- | :---: | :---: | :---: |
| PB-39 | Rezervacija knjiga | Omogućava članovima rezervaciju nedostupnih knjiga, pregled i upravljanje vlastitim rezervacijama. | Srednji | M | **Završeno** |
| PB-40 | Pregled aktivnih rezervacija | Bibliotekar može pregledati sve aktivne rezervacije u sistemu. | Srednji | S | **Završeno** |
| PB-43 | Automatsko otkazivanje rezervacije | Sistem automatski prati rok trajanja rezervacija i otkazuje istekle rezervacije. | Nizak | M | **Završeno** |
| PB-44 | Napredna pretraga i filteri | Filtriranje knjiga po kategoriji, izdavaču i godini uz mogućnost kombinovanja filtera. | Nizak | M | **Završeno** |
| PB-45 | Mjesečni izvještaji za upravu | Generisanje mjesečnih izvještaja o zaduživanjima, rezervacijama i članovima biblioteke. | Nizak | L | **Završeno** |

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
