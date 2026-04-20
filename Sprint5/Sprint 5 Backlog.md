# Set of User Stories - ažurirana verzija

## Opis dokumenta

Ovaj dokument predstavlja ažuriranu i detaljnije razrađenu verziju User Storyja za projekat Bibliotečkog informacionog sistema. Stavke iz Product Backloga su razrađene u više jasno definisanih User Story jedinica, pri čemu svaki User Story precizno opisuje korisničke zahtjeve, prihvatne kriterije i očekivano ponašanje sistema. Dokument je organizovan prema planiranom sprintu 5 radi bolje preglednosti i praćenja implementacije.


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

---
