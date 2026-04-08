# Set of User Stories

## Opis dokumenta

Ovaj dokument sadrži sve User Stories za projekat Bibliotečkog informacionog sistema, raspoređene prema planu sprintova. Svaka stavka iz Product Backloga je razrađena u jednu ili više User Story jedinica.


# Sprint 5

## US-01: Registracija korisnika

| **ID storyja** | US-01 |
|---------------|-------|
| **Naziv storyja** | Registracija korisnika |
| **Opis** | Kao bibliotekar ili administrator, želim kreirati nalog za novog člana unosom njegovih podataka, kako bi član mogao koristiti usluge biblioteke. |
| **Poslovna vrijednost** | Omogućava evidentiranje novih članova u sistemu i pristup bibliotečkim uslugama. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar ili administrator je unio podatke: ime, prezime, email i lozinku.<br>Email mora biti jedinstven. <br>Lozinka mora imati minimalno 8 znakova. <br> Ako je neki podatak neispravan ili nedostaje, sistem prikazuje odgovarajuću poruku o grešci. <br>Nakon uspješnog unosa, kreira se nalog sa ulogom 'Član'.|
| **Pretpostavke / Otvorena pitanja** | Član fizički dolazi u biblioteku i daje svoje podatke osoblju. |
| **Veze i zavisnosti** | Postojanje entiteta Korisnik u bazi podataka. |
| **Veza sa Product Backlog-om** | PB-12 |

---

## US-02: Prijava korisnika

| **ID storyja** | US-02 |
|---------------|-------|
| **Naziv storyja** | Prijava korisnika |
| **Opis** | Kao registrovani korisnik (Član, Bibliotekar ili Administrator), želim se prijaviti putem email-a i lozinke, kako bih pristupio funkcionalnostima prema svojoj ulozi. |
| **Poslovna vrijednost** | Omogućava sigurnu autentifikaciju korisnika i kontrolu pristupa funkcionalnostima sistema prema njihovoj ulozi. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Korisnik se može prijaviti sa email-om i lozinkom.<br>Ako su podaci ispravni, korisnik je preusmjeren na odgovarajući dashboard prema ulozi. <br> Ako nisu, sistem prikazuje poruku o grešci. |
| **Pretpostavke / Otvorena pitanja** | Korisnici imaju definisane uloge: Član, Bibliotekar ili Administrator. |
| **Veze i zavisnosti** | US-01: korisnik je registrovan. |
| **Veza sa Product Backlog-om** | PB-11 |

---

## US-03: Odjava korisnika

| **ID storyja** | US-03 |
|---------------|-------|
| **Naziv storyja** | Odjava korisnika |
| **Opis** | Kao prijavljeni korisnik, želim se odjaviti iz sistema, kako bi moj nalog bio zaštićen. |
| **Poslovna vrijednost** | Omogućava korisnicima sigurno završavanje sesije i smanjuje rizik od neovlaštenog pristupa računu. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Dugme za odjavu je dostupno u navigaciji na svim stranicama.<br>Korisnik je preusmjeren na stranicu za prijavu. |
| **Pretpostavke / Otvorena pitanja** |  |
| **Veze i zavisnosti** | US-02: korisnik je prijavljen u sistem. |
| **Veza sa Product Backlog-om** | PB-11 |

---

## US-04: Uspostava AI Usage Loga i Decision Loga

| **ID storyja** | US-04 |
|---------------|-------|
| **Naziv storyja** | Uspostava AI Usage Loga i Decision Loga |
| **Opis** | Kao tim, želimo uspostaviti evidenciju za praćenje korištenja AI alata i tehničkih odluka, kako bi naš proces bio transparentan i dokumentovan. |
| **Poslovna vrijednost** | Pomaže timu u dokumentovanju važnih odluka i načina korištenja AI alata, čime se postiže bolji uvid u razvoj projekta. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | AI Usage Log je kreiran u repozitoriju sa definisanom strukturom unosa.<br>Decision Log je kreiran sa definisanom strukturom.<br>Interni dogovor tima o procesu ažuriranja logova. |
| **Pretpostavke / Otvorena pitanja** |  |
| **Veze i zavisnosti** | Definisana struktura projektne dokumentacije. |
| **Veza sa Product Backlog-om** | PB-13 |

<br>

# Sprint 6

## US-05: Dodavanje nove knjige

| **ID storyja** | US-05 |
|---------------|-------|
| **Naziv storyja** | Dodavanje nove knjige |
| **Opis** | Kao bibliotekar ili administrator, želim dodati novu knjigu u sistem unosom njenih podataka, kako bi knjiga bila vidljiva u katalogu. |
| **Poslovna vrijednost** | Omogućava bibliotekarima da ažuriraju katalog i korisnicima pruži pristup novim knjigama.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar ili administrator je unio podatke o knjizi: naslov, autor, ISBN, godina izdanja, kategorija, broj primjeraka. <br>Knjiga je odmah vidljiva u katalogu nakon dodavanja. <br> Samo bibliotekar i administrator imaju pristup ovoj funkcionalnosti. <br> Svi obavezni podaci moraju biti uneseni prije spremanja, u suprotnom izbacuje grešku upozorenja. <br>  |
| **Pretpostavke / Otvorena pitanja** | Sistem ima kreirane uloge Bibliotekar i Administrator. <br> ISBN svake knjige je jedinstven u sistemu. |
| **Veze i zavisnosti** | Korisnik prijavljen kao biblioteka ili administrator. <br> Domain Model (entitet Knjiga). |
| **Veza sa Product Backlog-om** | PB-16 |

---

## US-06: Uređivanje podataka o knjizi 

| **ID storyja** | US-06 |
|---------------|-------|
| **Naziv storyja** | Uređivanja podataka o knjizi |
| **Opis** | Kao bibliotekar ili administrator, želim izmijeniti podatke o knjizi, kako bi informacije u katalogu bile tačne. |
| **Poslovna vrijednost** | Omogućava osoblju da održava tačnost podataka u katalogu, čime se korisnicima pruža pouzdan pregled bibliotečkog fonda.|
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Moguće je izmijeniti podatke: naslov, autor, godina izdavanja, kategorija. <br> Ako je unos neispravan, sistem prikazuje poruku o grešci. <br>  Promjene su odmah vidljive svim korisnicima. <br> Samo bibliotekar i administrator imaju pristup ovoj funkcionalnosti. |
| **Pretpostavke / Otvorena pitanja** | Knjiga koju uređujemo postoji u sistemu. |
| **Veze i zavisnosti** | US-05: Implementirano dodavanje knjiga |
| **Veza sa Product Backlog-om** | PB-17 |

---

## US-07: Pregled kataloga knjiga

| **ID storyja** | US-07 |
|---------------|-------|
| **Naziv storyja** | Pregleda kataloga knjiga |
| **Opis** | Kao korisnik sistema, želim pregledati listu svih dostupnih knjiga u biblioteci, kako bih pronašao knjige koje me interesuju.|
| **Poslovna vrijednost** | Lak pregled dostupnih knjiga i pronalazak željenih knjiga.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Katalog prikazuje sve knjige u obliku kartica. <br> Za svaku knjigu prikazuje se njen naslov, autor, kategorija i status dostupnosti. <br> Podržana je navigacija kroz stranice.|
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-05: Implementirano dodavanje knjiga |
| **Veza sa Product Backlog-om** | PB-22 |

---

## US-08: Upravljanje primjercima knjige

| **ID storyja** | US-08 |
|---------------|-------|
| **Naziv storyja** | Upravljanje primjercima knjige |
| **Opis** | Kao bibliotekar ili administrator, želim upravljati primjercima svake knjige, kako bi sistem tačno pratio fizički fond biblioteke.|
| **Poslovna vrijednost** | Održava ažuran fond biblioteke i omogućava korisnicima pouzdane informacije o dostupnosti knjiga. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Moguće je dodati jedan ili više primjeraka za svaku knjigu. <br> Svaki primjerak ima jedinstven inventarni broj. <br> Nije moguće obrisati primjerak koji je trenutno zadužen. <br> Nije moguće dodati primjerak ako knjiga ne postoji u sistemu. <br> Nakon uspješnog dodavanja ili izmjene, promjene su odmah vidljive u sistemu. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-05: Implementirano dodavanje knjige. <br> Domain Model (entitet Primjerak). |
| **Veza sa Product Backlog-om** | PB-20 |

---

## US-09: Brisanje knjige

| **ID storyja** | US-09 |
|---------------|-------|
| **Naziv storyja** | Brisanje knjige |
| **Opis** | Kao bibliotekar ili administrator, želim ukloniti knjigu iz sistema, kako bi katalog bio ažuran i fizički fond biblioteke precizno praćen. |
| **Poslovna vrijednost** | Održava tačnost bibliotečkog fonda i sprječava prikaz knjiga koje više nisu dostupne korisnicima.|
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Sistem ne dozvoljava brisanje knjige ako postoji aktivno zaduženje korisnika. <br> Nakon brisanja, promjene su odmah vidljive u katalogu. <br> Samo korisnici sa ulogom Bibliotekar ili Administrator imaju pristup ovoj funkcionalnosti. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-05: Knjiga je dodana u sistem. |
| **Veza sa Product Backlog-om** | PB-21 |

---

## US-10: Upravljanje kategorijama knjiga

| **ID storyja** | US-10 |
|---------------|-------|
| **Naziv storyja** | Upravljanje kategorijama knjiga |
| **Opis** | Kao bibliotekar ili administrator, želim upravljati kategorijama knjiga i mijenjati kategoriju već unesenih knjiga, kako bi sistem uvijek prikazivao tačne informacije. |
| **Poslovna vrijednost** | Omogućava bolju organizaciju knjiga u katalogu, olakšava pretragu i filtriranje za korisnike i bibliotekare, te osigurava tačnost podataka u sistemu.|
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Bibliotekar ili administrator može dodati novu kategoriju, urediti postojeću ili obrisati kategoriju. <br> Sve promjene su odmah vidljive u sistemu. |
| **Pretpostavke / Otvorena pitanja** | Samo bibliotekar i administrator imaju pristup ovoj funkcionalnosti. |
| **Veze i zavisnosti** | US-05: Knjiga je dodana u sistem. |
| **Veza sa Product Backlog-om** | PB-19 |

<br>

# Sprint 7

## US-11: Pretraživanje knjiga

| **ID storyja** | US-11 |
|---------------|-------|
| **Naziv storyja** | Pretraživanje knjiga |
| **Opis** | Kao korisnik sistema, želim pretraživati knjige po naslovu ili autoru, kako bih brzo pronašao konkretnu knjigu. |
| **Poslovna vrijednost** | Pretraga poboljšava UX i bez nje pregled velikog kataloga je nepraktičan.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Polje za pretragu je vidljivo na vrhu kataloga. <br> Pretraga radi po naslovu ili autoru. <br> Pretraga nije case-sensitive. <br> Prikazuje se broj pronađenih rezultata. <br> Ako nema rezultata, prikazuje se jasna poruka. <br> Rezultati pretrage su prikazani u istom formatu kao katalog. |
| **Pretpostavke / Otvorena pitanja** | Da li rezultati pretrage trebaju biti sortirani po nekom kriterijumu? |
| **Veze i zavisnosti** | US-07: Implementiran katalog knjiga |
| **Veza sa Product Backlog-om** | PB-23 |

---

## US-12: Prikaz detalja knjige

| **ID storyja** | US-12 |
|---------------|-------|
| **Naziv storyja** | Prikaza detalja knjige |
| **Opis** | Kao član biblioteke, želim pregledati detaljne informacije o knjizi, kako bih odlučio da li me interesuje. |
| **Poslovna vrijednost** | Detalji knjige smanjuju nepotrebne upite bibliotekarima. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Korisnik može otvoriti stranicu s detaljima klikom na knjigu iz kataloga. <br> Stranica prikazuje osnovne informacije o knjizi. <br> Vidljiv je ukupan broj primjeraka, kao i broj dostupnih. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-05: Knjige su već unesene u sistem. |
| **Veza sa Product Backlog-om** | PB-18 |

---

## US-13: Pregled dostupnosti knjige

| **ID storyja** | US-13 |
|---------------|-------|
| **Naziv storyja** | Pregled dostupnosti knjige |
| **Opis** | Kao član biblioteke, želim vidjeti da li je knjiga dostupna i koliko primjeraka ima, kako bih znao mogu li je odmah zadužiti. |
| **Poslovna vrijednost** | Omogućava članovima da provjere dostupnost knjiga i broj primjeraka, čime se olakšava planiranje posudbe. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Na detaljima knjige jasno je prikazan status: Dostupno/Zaduženo/Rezervisano. <br> Prikazuje se tačan broj slobodnih primjeraka. <br> Status se automatski ažurira pri zaduživanju ili vraćanju. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-14: Implementirano zaduživanje knjiga. |
| **Veza sa Product Backlog-om** | PB-24 |

<br>

# Sprint 8

## US-14: Evidencija zaduživanja knjiga

| **ID storyja** | US-14 |
|---------------|-------|
| **Naziv storyja** | Evidencija zaduživanja knjiga |
| **Opis** | Kao bibliotekar ili administrator, želim evidentirati zaduživanje knjige od strane člana, kako bi sistem znao ko ima koju knjigu. |
| **Poslovna vrijednost** | Omogućava tačnu evidenciju posuđenih knjiga i efikasno upravljanje bibliotečkim fondom. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar može odabrati člana koji zadužuje knjigu. <br> Bibliotekar može odabrati knjigu i konkretan fizički primjerak koji se zadužuje. <br> Ako je primjerak dostupan, sistem evidentira zaduživanje i označava primjerak kao "zadužen". <br> Ako primjerak nije dostupan, sistem prikazuje odgovarajuću poruku i ne dozvoljava zaduživanje. <br> Nakon uspješnog zaduživanja, evidencija je vidljiva u sistemu (u listi zaduženja člana ili knjige). |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 i US-02: Implementirana registracija i prijava korisnika. <br> US-08: Implementirano upravljanja primjercima knjige.|
| **Veza sa Product Backlog-om** | PB-25 |

---

## US-15: Evidencija vraćanja knjiga

| **ID storyja** | US-15 |
|---------------|-------|
| **Naziv storyja** | Evidencije vraćanja knjiga |
| **Opis** | Kao bibliotekar ili administrator, želim evidentirati vraćanje knjige, kako bi primjerak ponovo bio dostupan za zaduživanje. |
| **Poslovna vrijednost** | Vraćanje ažurira dostupnost knjige u katalogu. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar može pronaći zaduženje po članu. <br> Sistem prikazuje detalje zaduženja: naziv knjige, primjerak, datum zaduživanja, rok vraćanja i ime člana koji je zadužio knjigu. <br> Bibliotekar može evidentirati vraćanje klikom na dugme. <br> Nakon potvrde, primjerak knjige mijenja status u 'Dostupan'. <br> Promjene su odmah vidljive svim korisnicima sistema. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-14: Implementirano zaduživanje knjiga. |
| **Veza sa Product Backlog-om** | PB-25 |

---

## US-16: Pregled profila člana

| **ID storyja** | US-16 |
|---------------|-------|
| **Naziv storyja** | Pregled profila člana |
| **Opis** | Kao registrovani korisnik (član, bibliotekar ili administrator), želim pregledati svoj profil, kako bih vidio osnovne podatke i zaduženja. |
| **Poslovna vrijednost** | Omogućava korisnicima i osoblju biblioteke da brzo pristupe informacijama o članovima i njihovim posuđenim knjigama, što olakšava upravljanje članstvom i praćenje zaduženja. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Sistem prikazuje osnovne podatke člana: ime, prezime, email. <br> Sistem prikazuje trenutno posuđene knjige i status zaduženja. <br> Knjige s rokovima koji uskoro ističu su vizualno istaknute. |
| **Pretpostavke / Otvorena pitanja** | Član ili osoblje prijavljeno u sistem. |
| **Veze i zavisnosti** | US-01: Registracija korisnika.<br> Domain Model (entitet o korisnicima i zaduženjima). |
| **Veza sa Product Backlog-om** | PB-14 |


---

## US-17: Upravljanje korisnicima

| **ID storyja** | US-17 |
|---------------|-------|
| **Naziv storyja** | Upravljanje korisnicima |
| **Opis** | Kao administrator, želim pregledati sve korisnike sistema, mijenjati njihove uloge i deaktivirati naloge, kako bi pristup funkcionalnostima sistema bio siguran. |
| **Poslovna vrijednost** | Omogućava administratoru da održava sigurnost i integritet sistema, osigurava da korisnici imaju odgovarajuće privilegije i da deaktivirani korisnici ne mogu pristupati funkcionalnostima. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Administrator može vidjeti listu svih korisnika sistema sa osnovnim informacijama: ime i prezime, email, uloga. <br> Administrator može promijeniti ulogu korisnika. <br> Administrator može deaktivirati nalog, čime korisnik gubi pristup sistemu. <br> Promjene se odmah primjenjuju u sistemu i ažuriraju pristup korisnika. |
| **Pretpostavke / Otvorena pitanja** | Samo administrator ima pristup ovoj funkcionalnosti.  |
| **Veze i zavisnosti** | US-01: Korisnici su registrovani u sistem. |
| **Veza sa Product Backlog-om** | PB-26 |

---

## US-18: Pregled historije zaduženja

| **ID storyja** | US-18 |
|---------------|-------|
| **Naziv storyja** | Pregled historije zaduženja |
| **Opis** | Kao bibliotekar ili administrator, želim pregledati ranija zaduženja člana, kako bih mogao pratiti korištenje fonda i donositi odluke o zaduživanju ili opomenama. |
| **Poslovna vrijednost** | Omogućava uvid u historiju zaduženja, olakšava upravljanje fondom, praćenje kašnjenja i unapređuje korisničku podršku. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Bibliotekar ili administrator može pretraživati člana po imenu i prezimenu. <br> Sistem prikazuje listu svih zaduženja člana: naziv knjige, primjerak, datum zaduživanja, datum vraćanja. <br> Lista historije je sortirana po datumu zaduživanja (najnovije prvo). |
| **Pretpostavke / Otvorena pitanja** | Bibliotekar ili administrator je prijavljen u sistem. |
| **Veze i zavisnosti** | US-14: Evidencija kad je knjiga zadužena. <br> US-15: Evidencija kad je knjiga vraćena. |
| **Veza sa Product Backlog-om** | PB-31 |

---

## US-19: Upravljanje statusom članarine

| **ID storyja** | US-19 |
|---------------|-------|
| **Naziv storyja** | Upravljanje statusom članarine |
| **Opis** | Kao bibliotekar ili administrator, želim pregledati i ažurirati status članarine člana biblioteke, kako bih znao da li član ima pravo koristiti bibliotečke usluge. |
| **Poslovna vrijednost** | Omogućava osoblju biblioteke da prati važenje članarine, ažurira njen status i osigura da samo aktivni članovi koriste bibliotečke usluge. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Sistem prikazuje trenutni status članarine člana.<br>Moguće je evidentirati datum početka i datum isteka članarine.<br>Moguće je ažurirati status članarine.<br>Promjene su odmah vidljive u sistemu. |
| **Pretpostavke / Otvorena pitanja** | Bibliotekar ili administrator je prijavljen u sistem i član već postoji u evidenciji. |
| **Veze i zavisnosti** | US-01: Registracija korisnika.<br>US-17 Upravljanje korisnicima |
| **Veza sa Product Backlog-om** | PB-27 |

---

## US-20: Pregled statusa članarine za člana

| **ID storyja** | US-20 |
|---------------|-------|
| **Naziv storyja** | Pregled statusa članarine za člana |
| **Opis** | Kao član biblioteke, želim vidjeti status i datum isteka svoje članarine, kako bih znao da li mi je članarina aktivna. |
| **Poslovna vrijednost** | Omogućava članu biblioteke jasan uvid u važenje članarine, smanjuje potrebu za dodatnim upitima bibliotekaru i pomaže pravovremenom produženju članstva. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Član može vidjeti trenutni status članarine.<br>Član može vidjeti datum isteka članarine.<br>Ako članarina uskoro ističe ili je istekla, sistem to jasno prikazuje. |
| **Pretpostavke / Otvorena pitanja** | Član je prijavljen u sistem i njegova članarina je evidentirana u bazi podataka. |
| **Veze i zavisnosti** | US-02: Prijava korisnika.<br>US-19: Upravljanje statusom članarine |
| **Veza sa Product Backlog-om** | PB-28 |


---

## US-21: Početno testiranje sistema

| **ID storyja** | US-21 |
|---------------|-------|
| **Naziv storyja** | Početno testiranje sistema |
| **Opis** | Kao član tima, želim izvršiti početno testiranje prve verzije sistema, kako bismo osigurali da osnovne funkcionalnosti rade ispravno i da sistem može biti dalje razvijan bez kritičnih grešaka. |
| **Poslovna vrijednost** | Otkriva osnovne bugove i nedosljednosti u ranim fazama, smanjuje rizik od problema u kasnijim sprintovima i povećava kvalitet sistema. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Testiranje pokriva sve osnovne funkcionalnosti implementirane do sada. <br> Testiranje uključuje provjeru integracije između povezanih funkcionalnosti. <br> Dokumentirani su svi pronađeni bugovi i anomalije. <br>  |
| **Pretpostavke / Otvorena pitanja** | Svi prethodni user storiji implementirani do datog sprinta su spremni za testiranje. |
| **Veze i zavisnosti** | Zavisi od implementacije svih user storija do sprinta 9. |
| **Veza sa Product Backlog-om** | PB-32 |

<br>

# Sprint 9

## US-22: Pregled vlastitih zaduženja

| **ID storyja** | US-22 |
|---------------|-------|
| **Naziv storyja** | Pregled vlastitih zaduženja |
| **Opis** | Kao član biblioteke, želim pregledati koje knjige trenutno imam zadužene i koji je rok vraćanja, kako bih planirao vraćanje na vrijeme. |
| **Poslovna vrijednost** | Smanjivanje opterećenja bibliotekara i poboljšava korisničko iskustvo. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Prikazana je lista svih aktivnih zaduženja prijavljenog člana. <br> Za svako zaduženje prikazuje se: naslov knjige, datum zaduživanja, rok vraćanja. <br> Ako član nema aktivnih zaduženja, prikazuje se jasna poruka. <br> Član može vidjeti samo svoja vlastita zaduženja. <br> Lista se automatski ažurira kada se knjiga vrati. |
| **Pretpostavke / Otvorena pitanja** | Treba li uključiti informacije o eventualnim kašnjenjima ili kaznama? |
| **Veze i zavisnosti** | US-01 i US-02: Implementirana registracija i prijava korisnika <br> US-14: Implementirano zaduživanje knjiga. |
| **Veza sa Product Backlog-om** | PB-29 |

---

## US-23: Pregled trenutnih zaduženja

| **ID storyja** | US-23 |
|---------------|-------|
| **Naziv storyja** | Pregleda trenutnih zaduženja |
| **Opis** | Kao bibliotekar, želim pregledati sva trenutno aktivna zaduženja u sistemu, kako bih mogao pratiti stanje fonda. |
| **Poslovna vrijednost** | Poboljšava upravljanje posudbama knjiga i omogućava lakše praćenje članova koji trenutno imaju zadužene knjige. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Lista prikazuje sva aktivna zaduženja svih članova. <br> Prikazano je: ime člana, email člana, naslov knjige, datum zaduživanja, rok vraćanja. <br> Lista je sortirana po roku vraćanja. <br> Sistem prikazuje jasnu poruku ako nema aktivnih zaduženja. <br> Lista se automatski ažurira kada se knjige zaduže ili vrate. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-02: Korisnik je prijavljen kao bibliotekar. <br> US-14: Implementirano zaduživanje knjiga. |
| **Veza sa Product Backlog-om** | PB-30 |

---

## US-24: Rezervacija knjiga

| **ID storyja** | US-24 |
|---------------|-------|
| **Naziv storyja** | Rezervacija knjiga |
| **Opis** | Kao član biblioteke, želim rezervisati knjigu koja trenutno nije dostupna, kako bih bio obaviješten kada postane slobodna. |
| **Poslovna vrijednost** | Pomaže u boljem iskorištavanju bibliotečkog fonda jer omogućava planiranje posudbi čim knjiga postane dostupna. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Član može rezervisati samo knjigu koja nema dostupnih primjeraka. <br> Član može otkazati aktivnu rezervaciju. <br> Kada knjiga postane dostupna, sistem šalje email obavijest članovima koji su je rezervisali. |
| **Pretpostavke / Otvorena pitanja** | Koliko dugo rezervacija vrijedi nakon što knjiga postane dostupna? |
| **Veze i zavisnosti** | Implementirano slanje maila. |
| **Veza sa Product Backlog-om** | PB-33 |

---

## US-25: Pregled aktivnih rezervacija

| **ID storyja** | US-25 |
|---------------|-------|
| **Naziv storyja** | Pregled aktivnih rezervacija |
| **Opis** | Kao bibliotekar ili administrator, želim pregledati listu aktivnih rezervacija knjiga, kako bih znao koje knjige su rezervisane i od strane kojih članova. |
| **Poslovna vrijednost** | Omogućava osoblju biblioteke pregled rezervisanih knjiga i bolju organizaciju posudbi. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Bibliotekar ili administrator može vidjeti listu svih aktivnih rezervacija. <br> Za svaku rezervaciju prikazuje se ime i prezime člana, email člana, naslov knjige i datum rezervacije. <br> Sistem prikazuje jasnu poruku ako trenutno nema aktivnih rezervacija. <br> Lista se ažurira kada se rezervacija realizuje ili otkaže. |
| **Pretpostavke / Otvorena pitanja** | Prikaz rezervacija je dostupan samo bibliotekarima i administratorima. <br> Da li će se moći filtrirati rezervacije po knjizi ili članu? |
| **Veze i zavisnosti** | US-24: Implementirana rezervacija knjiga. |
| **Veza sa Product Backlog-om** | PB-34 |

---

## US-26: Napredna pretraga i filteri

| **ID storyja** | US-26 |
|---------------|-------|
| **Naziv storyja** | Napredna pretraga i filteri |
| **Opis** | Kao član biblioteke, želim pretraživati knjige koristeći dodatne filtere poput kategorije, godine izdanja i izdavača, kako bih brže pronašao knjige koje me interesuju. |
| **Poslovna vrijednost** | Poboljšava korisničko iskustvo i olakšava pronalazak relevantnih knjiga u katalogu, posebno kada katalog sadrži veliki broj knjiga. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Sistem omogućava filtriranje po kategoriji, izdavaču, godini izdavanja. <br> Filteri se mogu kombinovati. <br> Rezultati pretrage se ažuriraju nakon primjene filtera. <br> Sistem prikazuje broj rezultata koji odgovaraju pretrazi. <br> Ako nema rezultata, prikazuje se jasna poruka. |
| **Pretpostavke / Otvorena pitanja** | Da li filteri uključuju i knjige koje su rezervisane? |
| **Veze i zavisnosti** | US-07: Implementiran katalog knjiga. <br> US-11: Pretraga knjiga - proširenje osnovne funkcionalnosti pretrage. |
| **Veza sa Product Backlog-om** | PB-38 |

---

## US-27: Automatsko otkazivanje rezervacije

| **ID storyja** | US-27 |
|---------------|-------|
| **Naziv storyja** | Automatsko otkazivanje rezervacije|
| **Opis** | Kao sistem, želim automatski otkazati rezervaciju ako član ne preuzme knjigu u predviđenom roku, kako bi knjiga postala dostupna drugim članovima i smanjila broj neiskorištenih rezervacija. |
| **Poslovna vrijednost** | Poboljšava upravljanje fondom, povećava dostupnost knjiga i smanjuje administrativni posao osoblja biblioteke. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Sistem automatski prati sve aktivne rezervacije. <br> Ako član ne preuzme knjigu u definisanom roku, rezervacija se automatski otkazuje. <br> Nakon otkazivanja, knjiga postaje dostupna za nove rezervacije ili zaduživanje. <br> Automatsko otkazivanje se izvršava periodično. |
| **Pretpostavke / Otvorena pitanja** | Tim je postigao interni dogovor o broju dana nakon kojih rezervacija prestaje važiti. |
| **Veze i zavisnosti** | US-24: Knjiga je rezervisana. |
| **Veza sa Product Backlog-om** | PB-37 |

<br>

# Sprint 10


## US-28: Slanje email podsjetnika

| **ID storyja** | US-28 |
|---------------|-------|
| **Naziv storyja** | Slanje email podsjetnika |
| **Opis** | Kao korisnik, želim da dobijem automatski email podsjetnik prije isteka roka za vraćanje knjige, kako bih je mogao pravovremeno vratiti. |
| **Poslovna vrijednost** | Smanjuje broj kasnih vraćanja i poboljšava efikasnost biblioteke u praćenju posudbi. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Sistem šalje podsjetnik članovima 2 dana prije roka vraćanja. <br> Email sadrži naziv knjige i datum roka vraćanja. |
| **Pretpostavke / Otvorena pitanja** | Pretpostavlja se da su email adrese članova validne. |
| **Veze i zavisnosti** | Implementirano zaduživanje knjiga. |
| **Veza sa Product Backlog-om** | PB-35 |

---

## US-29: Slanje email upozorenja

| **ID storyja** | US-29 |
|---------------|-------|
| **Naziv storyja** | Slanje email upozorenja |
| **Opis** | Kao član biblioteke, želim primiti automatsko upozorenje kada mi istekne rok vraćanja knjige, kako bih znao da kasnim sa vraćanjem. |
| **Poslovna vrijednost** | Pomaže u smanjenju izgubljenih ili zakašnjelih knjiga i olakšava bibliotekaru kontrolu nad fondom. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Sistem šalje upozorenje članovima istog dana kada im istekne rok vraćanja. <br> Email sadrži naziv knjige i datum kada je rok istekao. |
| **Pretpostavke / Otvorena pitanja** | Pretpostavlja se da su email adrese članova validne. |
| **Veze i zavisnosti** | Implementirano zaduživanje knjiga. |
| **Veza sa Product Backlog-om** | PB-35 |

---

## US-30: Obavještavanje bibliotekara o novoj rezervaciji

| **ID storyja** | US-30 |
|---------------|-------|
| **Naziv storyja** | Obavještavanje bibliotekara o novoj rezervaciji |
| **Opis** | Kao bibliotekar, želim da dobijem email obavijest svaki put kada član kreira rezervaciju, kako bih bio informisan o novim rezervacijama. |
| **Poslovna vrijednost** | Poboljšava upravljanje rezervacijama, smanjuje rizik da se rezervisane knjige ne pripreme na vrijeme i poboljšava korisničko iskustvo. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Kada član kreira novu rezervaciju, sistem automatski šalje email bibliotekaru. <br> Email sadrži ime i prezime člana, naslov rezervisane knjige, datum rezervacije. |
| **Pretpostavke / Otvorena pitanja** | Rezervacije su pravilno evidentirane. |
| **Veze i zavisnosti** | US-24: Knjiga je rezervisana. |
| **Veza sa Product Backlog-om** | PB-36 |

---

## US-31: Mjesečni izvještaji za upravu

| **ID storyja** | US-31 |
|---------------|-------|
| **Naziv storyja** | Mjesečni izvještaji za upravu |
| **Opis** | Kao administrator biblioteke, želim generisati mjesečne izvještaje o zaduživanjima, rezervacijama i članstvu, kako bih mogao pratiti stanje biblioteke i donositi odluke. |
| **Poslovna vrijednost** | Omogućava uvid u korištenje bibliotečkog fonda i pomaže u planiranju resursa i strategija. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Administrator može generisati izvještaj za bilo koji mjesec. <br> Izvještaj sadrži broj i listu zaduženja, aktivnih rezervacija i članova. <br> Izvještaj se može pregledati i preuzeti u PDF formatu. |
| **Pretpostavke / Otvorena pitanja** | Podaci o zaduživanjima, rezervacijama i članstvu su tačni i ažurirani. |
| **Veze i zavisnosti** | US-01 i US-02: Korisnik je registrovan i prijavljen kao administrator. <br> Domain Model o zaduženjima, rezervacijama i korisnima biblioteke. |
| **Veza sa Product Backlog-om** | PB-39 |

---

## US-32: Audit log promjena

| **ID storyja** | US-32 |
|---------------|-------|
| **Naziv storyja** | Audit log promjena |
| **Opis** | Kao član osoblja, želim da sistem evidentira važne promjene u sistemu, kako bi se omogućilo praćenje aktivnosti i povećala sigurnost sistema.|
| **Poslovna vrijednost** | Omogućava reviziju i praćenje promjena u sistemu, pomaže u rješavanju problema i  praćenju grešaka. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Sistem evidentira sve važnije akcije korisnika: dodavanje, uređivajne i brisanje knjiga, dodavanje, uređivanje i deaktivaciju korisnika, kreiranje rezervacija. <br> Svaka promjena sadrži datum i vrijeme, ime i prezime korisnika. <br> Log se čuva u skladu s politikom podataka definisanom u sistemu. |
| **Pretpostavke / Otvorena pitanja** | Treba li log biti dostupan samo administratorima ili i bibliotekarima? |
| **Veze i zavisnosti** | Log prati sve važnije funkcionalnosti sistema. |
| **Veza sa Product Backlog-om** | PB-40 |

---

## US-33: Kazne za kasno vraćanje knjiga

| **ID storyja** | US-33 |
|---------------|-------|
| **Naziv storyja** | Kazne za kasno vraćanje knjiga |
| **Opis** | Kao sistem, želim evidentirati i obračunavati kazne kada član prekorači rok vraćanja knjige, kako bi članovi bili odgovorni i fond biblioteke bio zaštićen. |
| **Poslovna vrijednost** | Pomaže u smanjenju kašnjenja, osigurava poštovanje pravila i poboljšava upravljanje fondom. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Sistem automatski obračunava kaznu po danu za svaku knjigu koja nije vraćena na vrijeme. <br> Član može vidjeti iznos kazne u svom profilu. |
| **Pretpostavke / Otvorena pitanja** | Utvrđen plan po kojem će se računati kazne. |
| **Veze i zavisnosti** | Knjiga je zadužena i poslan je mail upozorenja. |
| **Veza sa Product Backlog-om** | PB-41 |

---

## US-34: Online produžetak članarine

| **ID storyja** | US-34 |
|---------------|-------|
| **Naziv storyja** | Online produžetak članarine |
| **Opis** | Kao član biblioteke, želim online produžiti svoju članarinu, kako bih mogao nastaviti koristiti bibliotečke usluge bez dolaska u biblioteku. |
| **Poslovna vrijednost** | Omogućava članovima jednostavno i brzo produženje članarine, smanjuje opterećenje osoblja i unapređuje korisničko iskustvo. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Član može pokrenuti produženje članarine putem sistema.<br>Sistem omogućava izbor trajanja produženja (npr. 1, 3, 6 ili 12 mjeseci).<br>Nakon uspješnog produženja, datum isteka članarine se ažurira.<br>Status članarine se automatski postavlja na "Aktivna".<br>Sistem prikazuje potvrdu o uspješnom produženju. |
| **Pretpostavke / Otvorena pitanja** | Na koji način simulirati sistem online naplate članarine? |
| **Veze i zavisnosti** | US-19: Upravljanje statusom članarine<br>US-20: Pregled statusa članarine |
| **Veza sa Product Backlog-om** | PB-42 |

---

## US-35: Integracija sa distributerom knjiga

| **ID storyja** | US-35 |
|---------------|-------|
| **Naziv storyja** | Integracija sa distributerom knjiga |
| **Opis** | Kao bibliotekar ili administrator, želim poslati zahtjev distributeru knjiga putem sistema, kako bih mogao naručiti nove knjige za biblioteku. |
| **Poslovna vrijednost** | Olakšava proces nabavke novih knjiga i unapređuje upravljanje bibliotečkim fondom. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Bibliotekar može unijeti podatke o knjizi koju želi naručiti. <br> Sistem generiše i šalje email zahtjev distributeru knjiga. <br> Sistem prikazuje potvrdu da je zahtjev poslan. |
| **Pretpostavke / Otvorena pitanja** | Email adresa distributera je poznata i dostupna sistemu. |
| **Veze i zavisnosti** | Implementirano slanje maila.|
| **Veza sa Product Backlog-om** | PB-43 |

<br>

# Sprint 11


## US-36: Sistematsko testiranje i bug fixing

| **ID storyja** | US-36 |
|---------------|-------|
| **Naziv storyja** | Sistematsko testiranje i bug fixing |
| **Opis** | Kao tim, želimo provesti sistematsko testiranje svih implementiranih funkcionalnosti i otkloniti kritične greške, kako bi sistem bio stabilan za demonstraciju. |
| **Poslovna vrijednost** | Osigurava stabilnost i pouzdanost sistema, pa smanjuje rizik od grešaka tokom demonstracije i korištenja te povećava kvalitet i vjerodostojnost projekta. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Svi test scenariji za funkcionalnosti su definirani i izvršeni. <br> Svi bugovi su identifikovani i otklonjeni. <br> Test evidencija je pohranjena u repozitoriju. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 - US-35: Implementirane sve funkcionalnosti. |
| **Veza sa Product Backlog-om** | PB-44 |


---

## US-37: Izrada liste poznatih ograničenja i tehničkog duga

| **ID storyja** | US-37 |
|---------------|-------|
| **Naziv storyja** | Izrada liste poznatih ograničenja i tehničkog duga |
| **Opis** | Kao tim, želimo napraviti listu stvari koje ograničavaju sistem i nedovršene tehničke dijelove, kako bismo znali šta još treba uraditi i mogli jasno obavijestiti druge o stanju projekta. |
| **Poslovna vrijednost** | Omogućava timu i stakeholderima da jasno vide postojeća ograničenja i nedovršene dijelove sistema, što pomaže u realnom planiranju rada, donošenju odluka i transparentnoj komunikaciji o napretku projekta. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Lista svih poznatih ograničenja sistema je dokumentovana. <br> Lista svih elemenata tehničkog duga je dokumentovana. <br> Svaka stavka na listi sadrži kratak opis i utjecaj na projekat. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | Sprovedeno testiranje sistema. |
| **Veza sa Product Backlog-om** | PB-45 |

<br>

# Sprint 12

## US-38: Izrada Release Notes

| **ID storyja** | US-38 |
|---------------|-------|
| **Naziv storyja** | Izrada Release Notes |
| **Opis** | Kao tim, želimo kreirati Release Notes koji opisuju sve implementirane funkcionalnosti, poznata ograničenja i upute za instalaciju, kako bi finalna verzija bila profesionalno dokumentovana. |
| **Poslovna vrijednost** | Release Notes su formalni dokaz šta je isporučeno, te obavezan artefakt za završnu odbranu projekta. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Release Notes sadrže listu svih implementiranih funkcionalnosti. <br> Navedena su poznata ograničenja i poznati bugovi. <br> Uključene su upute za instalaciju i pokretanje sistema. <br> Dokument je pohranjen u repozitoriju. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-36: Sistematsko testiranje i bug fixing. <br> US-36: Izrada liste poznatih ograničenja i tehničkog duga. |
| **Veza sa Product Backlog-om** | PB-46 |

---

## US-39: Izrada korisničke dokumentacije

| **ID storyja** | US-38 |
|---------------|-------|
| **Naziv storyja** | Izrada korisničke dokumentacije |
| **Opis** | Kao tim, želimo kreirati korisničku dokumentaciju koja objašnjava kako koristiti sistem, kako bi krajnji korisnici i ocjenjivači mogli razumjeti sistem bez tehničkog predznanja. |
| **Poslovna vrijednost** | Omogućava krajnjim korisnicima da lako razumiju i koriste sistem, smanjuje potrebu za dodatnim objašnjenjima i povećava profesionalnost projekta.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Dokumentovane su upute za sve korisničke uloge (Član, Bibliotekar, Administrator). <br> Uključeni su screenshotovi ili opisi ključnih ekrana. <br> Objašnjen je tok rada za najčešće scenarije (zaduživanje, pretraga, rezervacija). <br> Dokument je pohranjen u repozitoriju. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 - US-35: Implementirane sve funkcionalnosti. |
| **Veza sa Product Backlog-om** | PB-47 |

---

## US-40: Izrada tehničke dokumentacije

| **ID storyja** | US-40 |
|---------------|-------|
| **Naziv storyja** | Izrada tehničke dokumentacije |
| **Opis** | Kao tim, želimo kreirati tehničku dokumentaciju koja opisuje arhitekturu, API-je i razvojno okruženje, kako bi sistem bio razumljiv drugom developeru.|
| **Poslovna vrijednost** | Olakšava održavanje, nadogradnju i smanjuje vrijeme potrebno za integraciju novih developera. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Dokumentovana je arhitektura sistema. <br> Naveden je postupak postavljanja razvojnog okruženja. <br> Ažurirani su svi obavezni logovi (AI Usage Log, Decision Log). |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 - US-35: Implementirane sve funkcionalnosti. |
| **Veza sa Product Backlog-om** | PB-48 |


<br>

# Sprint 13

## US-41: Priprema i izvođenje završne demonstracije

| **ID storyja** | US-41 |
|---------------|-------|
| **Naziv storyja** | Priprema i izvođenje završne demonstracije |
| **Opis** | Kao tim, želimo pripremiti i izvesti završnu demonstraciju sistema, kako bismo prikazali sve implementirane funkcionalnosti i pokazali da sistem radi ispravno.|
| **Poslovna vrijednost** | Završna demonstracija pokazuje funkcionisanje sistema i direktno utiče na ocjenu projekta. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Svaki član tima je u stanju objasniti dijelove sistema za koje je odgovoran. <br> Sistem radi stabilno tokom demonstracije. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 - US-35: Implementirane sve funkcionalnosti. <br> US-37: Izrađena korisnička dokumentacija. <br> US-40: Izrađena tehička dokumentacija. |
| **Veza sa Product Backlog-om** | PB-49 |
