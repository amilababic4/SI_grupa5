# Set of User Stories

## Opis dokumenta

Ovaj dokument sadrži sve User Stories za projekat Bibliotečkog informacionog sistema, raspoređene prema planu sprintova. Svaka stavka iz Product Backloga je razrađena u jednu ili više User Story jedinica.


# Sprint 5

## US-01: Implementacija registracije korisnika

| **ID storyja** | US-01 |
|---------------|-------|
| **Naziv storyja** | Implementacija registracije korisnika |
| **Opis** | Kao bibliotekar ili administrator, želim kreirati nalog za novog člana unosom njegovih podataka, kako bi član mogao koristiti usluge biblioteke. |
| **Poslovna vrijednost** | Omogućava evidentiranje novih članova u sistemu i pristup bibliotečkim uslugama. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar ili administrator je unio podatke: ime, prezime, email i lozinku.<br>Email mora biti jedinstven. <br>Lozinka mora imati minimalno 8 znakova.<br>Nakon uspješnog unosa, kreira se nalog sa ulogom 'Član'.|
| **Pretpostavke / Otvorena pitanja** | Član fizički dolazi u biblioteku i daje svoje podatke osoblju. |
| **Veze i zavisnosti** | Domain Model (entitet Korisnik) |

---

## US-02: Implementacija prijave korisnika

| **ID storyja** | US-02 |
|---------------|-------|
| **Naziv storyja** | Implementacija prijave korisnika u sistem |
| **Opis** | Kao registrovani korisnik (Član, Bibliotekar ili Administrator), želim se prijaviti putem email-a i lozinke, kako bih pristupio funkcionalnostima prema svojoj ulozi. |
| **Poslovna vrijednost** | Omogućava sigurnu autentifikaciju korisnika i kontrolu pristupa funkcionalnostima sistema prema njihovoj ulozi. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Korisnik se može prijaviti sa email-om i lozinkom.<br>Nakon prijave, preusmjeren je na odgovarajući dashboard prema ulozi.|
| **Pretpostavke / Otvorena pitanja** | Korisnici imaju definisane uloge: Član, Bibliotekar ili Administrator. |
| **Veze i zavisnosti** | US-01: registracija korisnika |

---

## US-03: Implementacija odjave korisnika

| **ID storyja** | US-03 |
|---------------|-------|
| **Naziv storyja** | Implementacija odjave korisnika |
| **Opis** | Kao prijavljeni korisnik, želim se odjaviti iz sistema, kako bi moj nalog bio zaštićen. |
| **Poslovna vrijednost** | Omogućava korisnicima sigurno završavanje sesije i smanjuje rizik od neovlaštenog pristupa nalogu. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Dugme za odjavu je dostupno u navigaciji na svim stranicama.<br>Korisnik je preusmjeren na stranicu za prijavu. |
| **Pretpostavke / Otvorena pitanja** |  |
| **Veze i zavisnosti** | US-02: prijava |

---

## US-04: Uspostava AI Usage Loga i Decision Loga

| **ID storyja** | US-04 |
|---------------|-------|
| **Naziv storyja** | Uspostava AI Usage Loga i Decision Loga |
| **Opis** | Kao tim, želimo uspostaviti strukturirane logove za praćenje korištenja AI alata i tehničkih odluka, kako bi naš proces bio transparentan i dokumentovan. |
| **Poslovna vrijednost** | Pomaže timu u dokumentovanju važnih odluka i načina korištenja AI alata, čime se postiže bolji uvid u razvoja projekta. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | AI Usage Log je kreiran u repozitoriju sa definisanom strukturom unosa.<br>Decision Log je kreiran sa definisanom strukturom.<br>Interni dogovor tima o procesu ažuriranja logova. |
| **Pretpostavke / Otvorena pitanja** |  |
| **Veze i zavisnosti** | Definisana struktura projektne dokumentacije. |

---

## US-05: Pregled profila člana

| **ID storyja** | US-05 |
|---------------|-------|
| **Naziv storyja** | Pregled profila člana |
| **Opis** | Kao registrovani korisnik (Član, Bibliotekar ili Administrator), želim pregledati svoj profil, kako bih vidio osnovne podatke i zaduženja.|
| **Poslovna vrijednost** | Omogućava korisnicima i osoblju biblioteke da brzo pristupe informacijama o članovima i njihovim posuđenim knjigama, što olakšava upravljanje članstvom i praćenje zaduženja. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Sistem prikazuje osnovne podatke člana: ime, prezime, email. <br> Sistem prikazuje trenutno posuđene knjige i status zaduženja. <br> Podaci su ažurirani i odgovaraju stvarnom stanju u bazi.|
| **Pretpostavke / Otvorena pitanja** | Član ili osoblje prijavljeno u sistem. |
| **Veze i zavisnosti** | US-01: Registracija korisnika.<br>Domain Model (entitet o korisnicima i zaduženjima). |

<br>

# Sprint 6

## US-06: Implementacija dodavanja nove knjige

| **ID storyja** | US-06 |
|---------------|-------|
| **Naziv storyja** | Implementacija dodavanja nove knjige |
| **Opis** | Kao bibliotekar ili administrator, želim dodati novu knjigu u sistem unosom njenih podataka, kako bi knjiga bila vidljiva u katalogu. |
| **Poslovna vrijednost** | Omogućava bibliotekarima da ažuriraju katalog i korisnicima pruži pristup novim knjigama.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar ili administrator je unio podatke o knjizi: naslov, autor, ISBN, godina izdanja, kategorija, broj primjeraka. <br>Knjiga je odmah vidljiva u katalogu nakon dodavanja. <br> Samo bibliotekar i administrator imaju pristup ovoj funkcionalnosti. <br> Svi obavezni podaci moraju biti uneseni prije spremanja. <br>  |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | Korisnik prijavljen kao biblioteka ili administrator. <br> Domain Model (entitet Knjiga). |

---

## US-07: Implementacija uređivanja podataka o knjizi 

| **ID storyja** | US-07 |
|---------------|-------|
| **Naziv storyja** | Implementacija uređivanja podataka o knjizi |
| **Opis** | Kao bibliotekar ili administrator, želim izmijeniti podatke o knjizi, kako bi informacije u katalogu bile tačne. |
| **Poslovna vrijednost** | Omogućava osoblju da održava tačnost podataka u katalogu, čime se korisnicima pruža pouzdan pregled bibliotečkog fonda.|
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Moguće je izmijeniti podatke: naslov, autor, godina izdavanja, kategorija. <br> Promjene su odmah vidljive svim korisnicima. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-06: Implementirano dodavanje knjiga |

---

## US-08: Implementacija pregleda kataloga

| **ID storyja** | US-08 |
|---------------|-------|
| **Naziv storyja** | Implementacija pregleda kataloga |
| **Opis** | Kao korisnik sistema, želim pregledati listu svih dostupnih knjiga u biblioteci, kako bih pronašao knjige koje me interesuju.|
| **Poslovna vrijednost** | Lak pregled dostupnih knjiga i pronalazak željenih knjiga.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Katalog prikazuje sve knjige u obliku kartica. <br> Za svaku knjigu prikazuje se njen naslov, autor, kategorija i status dostupnosti. <br> Podržana je navigacija kroz stranice.|
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-06: Implementirano dodavanje knjiga |

---

## US-09: Implementacija upravljanja primjercima knjige

| **ID storyja** | US-09 |
|---------------|-------|
| **Naziv storyja** | Implementacija upravljanja primjercima knjige |
| **Opis** | Kao bibliotekar ili administrator, želim upravljati fizičkim primjercima svake knjige, kako bi sistem tačno pratio fizički fond biblioteke.|
| **Poslovna vrijednost** | Održava ažuran fond biblioteke i omogućava korisnicima pouzdane informacije o dostupnosti knjiga. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Moguće je dodati jedan ili više primjeraka za svaku knjigu. <br> Svaki primjerak ima jedinstven inventarni broj. <br> Nije moguće obrisati primjerak koji je trenutno zadužen. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-06: Implementirano dodavanje knjige. <br> Domain Model (entitet Primjerak). |

<br>

# Sprint 7

## US-10: Implementacija pretrage knjiga

| **ID storyja** | US-10 |
|---------------|-------|
| **Naziv storyja** | Implementacija pretrage knjiga |
| **Opis** | Kao korisnik sistema, želim pretraživati knjige po naslovu ili autoru, kako bih brzo pronašao konkretnu knjigu. |
| **Poslovna vrijednost** | Pretraga dramatično poboljšava UX – bez nje pregled velikog kataloga je nepraktičan.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Polje za pretragu je vidljivo na vrhu kataloga. <br> Pretraga radi po naslovu ili autoru. <br> Pretraga nije case-sensitive. <br> Prikazuje se broj pronađenih rezultata. <br> Ako nema rezultata, prikazuje se jasna poruka. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-08: Implementiran katalog knjiga |

---

## US-11: Implementacija prikaza detalja knjige

| **ID storyja** | US-11 |
|---------------|-------|
| **Naziv storyja** | Implementacija prikaza detalja knjige |
| **Opis** | Kao član biblioteke, želim pregledati detaljne informacije o knjizi, kako bih odlučio da li me interesuje. |
| **Poslovna vrijednost** | Detalji knjige smanjuju nepotrebne upite bibliotekarima. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Stranica prikazuje osnovne informacije o knjizi. <br> Vidljiv je ukupan broj primjeraka, kao i broj dostupnih. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-06: Implementirano dodavanje knjiga. |

---

## US-12: Implementacija pregleda dostupnosti knjige

| **ID storyja** | US-12 |
|---------------|-------|
| **Naziv storyja** | Implementacija pregleda dostupnosti knjige |
| **Opis** | Kao član biblioteke, želim vidjeti da li je knjiga dostupna i koliko primjeraka ima, kako bih znao mogu li je odmah zadužiti. |
| **Poslovna vrijednost** | Omogućava članovima da provjere dostupnost knjiga i broj primjeraka, čime se olakšava planiranje posudbe. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Na detaljima knjige jasno je prikazano: Dostupno/Zaduženo/Rezervisano. <br> Prikazuje se tačan broj slobodnih primjeraka. <br> Status se automatski ažurira pri zaduživanju ili vraćanju. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-13: Implementirano zaduživanje knjiga. |

