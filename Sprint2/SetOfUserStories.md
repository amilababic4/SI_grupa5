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

<br>

# Sprint 6

## US-05: Implementacija dodavanja nove knjige

| **ID storyja** | US-05 |
|---------------|-------|
| **Naziv storyja** | Implementacija dodavanja nove knjige |
| **Opis** | Kao bibliotekar ili administrator, želim dodati novu knjigu u sistem unosom njenih podataka, kako bi knjiga bila vidljiva u katalogu. |
| **Poslovna vrijednost** | Omogućava bibliotekarima da ažuriraju katalog i korisnicima pruži pristup novim knjigama.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar ili administrator je unio podatke o knjizi: naslov, autor, ISBN, godina izdanja, kategorija, broj primjeraka. <br>Knjiga je odmah vidljiva u katalogu nakon dodavanja. <br> Samo bibliotekar i administrator imaju pristup ovoj funkcionalnosti. <br> Svi obavezni podaci moraju biti uneseni prije spremanja. <br>  |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | Korisnik prijavljen kao biblioteka ili administrator. <br> Domain Model (entitet Knjiga). |

---

## US-06: Implementacija uređivanja podataka o knjizi 

| **ID storyja** | US-06 |
|---------------|-------|
| **Naziv storyja** | Implementacija uređivanja podataka o knjizi |
| **Opis** | Kao bibliotekar ili administrator, želim izmijeniti podatke o knjizi, kako bi informacije u katalogu bile tačne. |
| **Poslovna vrijednost** | Omogućava osoblju da održava tačnost podataka u katalogu, čime se korisnicima pruža pouzdan pregled bibliotečkog fonda.|
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Moguće je izmijeniti podatke: naslov, autor, godina izdavanja, kategorija. <br> Promjene su odmah vidljive svim korisnicima. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-05: Implementirano dodavanje knjiga |

---

## US-07: Implementacija pregleda kataloga

| **ID storyja** | US-07 |
|---------------|-------|
| **Naziv storyja** | Implementacija pregleda kataloga |
| **Opis** | Kao korisnik sistema, želim pregledati listu svih dostupnih knjiga u biblioteci, kako bih pronašao knjige koje me interesuju.|
| **Poslovna vrijednost** | Lak pregled dostupnih knjiga i pronalazak željenih knjiga.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Katalog prikazuje sve knjige u obliku kartica. <br> Za svaku knjigu prikazuje se njen naslov, autor, kategorija i status dostupnosti. <br> Podržana je navigacija kroz stranice.|
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-05: Implementirano dodavanje knjiga |

---

## US-08: Implementacija upravljanja primjercima knjige

| **ID storyja** | US-08 |
|---------------|-------|
| **Naziv storyja** | Implementacija upravljanja primjercima knjige |
| **Opis** | Kao bibliotekar ili administrator, želim upravljati fizičkim primjercima svake knjige, kako bi sistem tačno pratio fizički fond biblioteke.|
| **Poslovna vrijednost** | Održava ažuran fond biblioteke i omogućava korisnicima pouzdane informacije o dostupnosti knjiga. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Moguće je dodati jedan ili više primjeraka za svaku knjigu. <br> Svaki primjerak ima jedinstven inventarni broj. <br> Nije moguće obrisati primjerak koji je trenutno zadužen. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-05: Implementirano dodavanje knjige. <br> Domain Model (entitet Primjerak). |

<br>

# Sprint 7

## US-09: Implementacija pretrage knjiga

| **ID storyja** | US-09 |
|---------------|-------|
| **Naziv storyja** | Implementacija pretrage knjiga |
| **Opis** | Kao korisnik sistema, želim pretraživati knjige po naslovu ili autoru, kako bih brzo pronašao konkretnu knjigu. |
| **Poslovna vrijednost** | Pretraga dramatično poboljšava UX – bez nje pregled velikog kataloga je nepraktičan.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Polje za pretragu je vidljivo na vrhu kataloga. <br> Pretraga radi po naslovu ili autoru. <br> Pretraga nije case-sensitive. <br> Prikazuje se broj pronađenih rezultata. <br> Ako nema rezultata, prikazuje se jasna poruka. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-07: Implementiran katalog knjiga |

---

## US-10: Implementacija prikaza detalja knjige

| **ID storyja** | US-10 |
|---------------|-------|
| **Naziv storyja** | Implementacija prikaza detalja knjige |
| **Opis** | Kao član biblioteke, želim pregledati detaljne informacije o knjizi, kako bih odlučio da li me interesuje. |
| **Poslovna vrijednost** | Detalji knjige smanjuju nepotrebne upite bibliotekarima. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Stranica prikazuje osnovne informacije o knjizi. <br> Vidljiv je ukupan broj primjeraka, kao i broj dostupnih. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-05: Implementirano dodavanje knjiga. |

---

## US-11: Implementacija pregleda dostupnosti knjige

| **ID storyja** | US-11 |
|---------------|-------|
| **Naziv storyja** | Implementacija pregleda dostupnosti knjige |
| **Opis** | Kao član biblioteke, želim vidjeti da li je knjiga dostupna i koliko primjeraka ima, kako bih znao mogu li je odmah zadužiti. |
| **Poslovna vrijednost** | Omogućava članovima da provjere dostupnost knjiga i broj primjeraka, čime se olakšava planiranje posudbe. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Na detaljima knjige jasno je prikazano: Dostupno/Zaduženo/Rezervisano. <br> Prikazuje se tačan broj slobodnih primjeraka. <br> Status se automatski ažurira pri zaduživanju ili vraćanju. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-12: Implementirano zaduživanje knjiga. |

<br>

# Sprint 8

## US-12: Implementacija evidencije zaduživanja knjige

| **ID storyja** | US-12 |
|---------------|-------|
| **Naziv storyja** | Implementacija evidencije zaduživanja knjige |
| **Opis** | Kao bibliotekar, želim evidentirati zaduživanje knjige od strane člana, kako bi sistem znao ko ima koju knjigu. |
| **Poslovna vrijednost** | Omogućava tačnu evidenciju posuđenih knjiga i efikasno upravljanje bibliotečkim fondom. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar može odabrati člana koji zadužuje knjigu. <br> Bibliotekar može odabrati knjigu i konkretan fizički primjerak koji se zadužuje. <br> Ako je primjerak dostupan, sistem evidentira zaduživanje i označava primjerak kao "zadužen". <br> Ako primjerak nije dostupan, sistem prikazuje odgovarajuću poruku i ne dozvoljava zaduživanje. <br> Nakon uspješnog zaduživanja, evidencija je vidljiva u sistemu (u listi zaduženja člana ili knjige). |
| **Pretpostavke / Otvorena pitanja** | Postojanje plana po kojem se računa rok vraćanja. |
| **Veze i zavisnosti** | US-01 i US-02: Implementirana registracija i prijava korisnika. <br> US-08: Implementirano upravljanja primjercima knjige.|

---

## US-13: Implementacija evidencije vraćanja knjige

| **ID storyja** | US-13 |
|---------------|-------|
| **Naziv storyja** | Implementacija evidencije vraćanja knjige |
| **Opis** | Kao bibliotekar, želim evidentirati vraćanje knjige, kako bi primjerak ponovo bio dostupan za zaduživanje. |
| **Poslovna vrijednost** | Vraćanje ažurira dostupnost knjige u katalogu. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar može pronaći zaduženje po članu. <br> Sistem prikazuje podatke o zaduženju (datum zaduživanja, rok vraćanja). <br> Nakon potvrde, primjerak knjige mijenja status u 'Dostupan'. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-12: Implementirano zaduživanje knjiga. |

---

## US-14: Pregled profila člana

| **ID storyja** | US-14 |
|---------------|-------|
| **Naziv storyja** | Pregled profila člana |
| **Opis** | Kao registrovani korisnik (Član, Bibliotekar ili Administrator), želim pregledati svoj profil, kako bih vidio osnovne podatke i zaduženja.|
| **Poslovna vrijednost** | Omogućava korisnicima i osoblju biblioteke da brzo pristupe informacijama o članovima i njihovim posuđenim knjigama, što olakšava upravljanje članstvom i praćenje zaduženja. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Sistem prikazuje osnovne podatke člana: ime, prezime, email. <br> Sistem prikazuje trenutno posuđene knjige i status zaduženja. <br> Knjige s rokovima koji uskoro ističu su vizualno istaknute. |
| **Pretpostavke / Otvorena pitanja** | Član ili osoblje prijavljeno u sistem. |
| **Veze i zavisnosti** | US-01: Registracija korisnika.<br> Domain Model (entitet o korisnicima i zaduženjima). |

<br>

# Sprint 9

## US-15: Pregled vlastitih zaduženja

| **ID storyja** | US-15 |
|---------------|-------|
| **Naziv storyja** | Pregled vlastitih zaduženja|
| **Opis** | Kao član biblioteke, želim pregledati koje knjige trenutno imam zadužene i koji je rok vraćanja, kako bih planirao vraćanje na vrijeme. |
| **Poslovna vrijednost** | Smanjivanje opterećenja bibliotekara i poboljšavanje korisničkog iskustvo. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Prikazana je lista svih aktivnih zaduženja prijavljenog člana. <br> Za svako zaduženje prikazuje se: naslov, datum zaduživanja, rok vraćanja. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 i US-02: Implementirana registracija i prijava korisnika <br> US-12: Implementirano zaduživanje knjiga. |

---

## US-16: Pregled trenutnih zaduženja

| **ID storyja** | US-16 |
|---------------|-------|
| **Naziv storyja** | Pregleda trenutnih zaduženja |
| **Opis** | Kao bibliotekar, želim pregledati sve trenutno aktivne zaduženja u sistemu, kako bih mogao pratiti stanje fonda. |
| **Poslovna vrijednost** | Poboljšava upravljanje posudbama knjiga i omogućava lakše praćenje članova koji trenutno imaju zadužene knjige. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Lista prikazuje sva aktivna zaduženja svih članova. <br> Prikazano je: ime člana, naslov knjige, datum zaduživanja, rok vraćanja. <br> Lista je sortirana po roku vraćanja. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-02: Korisnik je prijavljen kao bibliotekar. <br> US-13: Implementirano zaduživanje knjiga. |

---

## US-17: Rezervacija knjiga

| **ID storyja** | US-17 |
|---------------|-------|
| **Naziv storyja** | Rezervacija knjiga |
| **Opis** | Kao član biblioteke, želim rezervisati knjigu koja trenutno nije dostupna, kako bih bio obaviješten kada postane slobodna. |
| **Poslovna vrijednost** | Pomaže u boljem iskorištavanju bibliotečkog fonda jer omogućava planiranje posudbi čim knjiga postane dostupna. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Član može rezervisati samo knjigu koja nema dostupnih primjeraka. <br> Član može otkazati aktivnu rezervaciju. <br> Kada knjiga postane dostupna, sistem šalje email obavijest članovima koji su je rezervisali. |
| **Pretpostavke / Otvorena pitanja** | Koliko dugo rezervacija vrijedi nakon što knjiga postane dostupna? |
| **Veze i zavisnosti** | Implementirano slanje maila. |

---

## US-18: Pregled aktivnih rezervacija

| **ID storyja** | US-18 |
|---------------|-------|
| **Naziv storyja** | Pregled aktivnih rezervacija |
| **Opis** | Kao bibliotekar, želim pregledati listu aktivnih rezervacija knjiga, kako bih znao koje knjige su rezervisane i od strane kojih članova. |
| **Poslovna vrijednost** | Omogućava bibliotekaru pregled rezervisanih knjiga i bolju organizaciju posudbi. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Bibliotekar može vidjeti listu svih aktivnih rezervacija. <br> Za svaku rezervaciju prikazuje se član, knjiga i datum rezervacije. |
| **Pretpostavke / Otvorena pitanja** | Da li će bibliotekar moći filtrirati rezervacije po knjizi ili članu? |
| **Veze i zavisnosti** | US-17: Implementirana rezervacija knjiga. |

<br>

# Sprint 10


## US-19: Slanje email podsjetnika

| **ID storyja** | US-19 |
|---------------|-------|
| **Naziv storyja** | Slanje email podsjetnika |
| **Opis** | Kao sistem, želim automatski slati podsjetnike članovima za rok vraćanja knjiga, kako bi članovi pravovremeno vratili posuđene knjige. |
| **Poslovna vrijednost** | Smanjuje broj kasnih vraćanja i poboljšava efikasnost biblioteke u praćenju posudbi. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Sistem šalje podsjetnik članovima 2 dana prije roka vraćanja. <br> Email sadrži naziv knjige i datum roka vraćanja. |
| **Pretpostavke / Otvorena pitanja** | Pretpostavlja se da su email adrese članova validne. |
| **Veze i zavisnosti** | Implementirano zaduživanje knjiga. |

---

## US-20: Slanje email upozorenja

| **ID storyja** | US-20 |
|---------------|-------|
| **Naziv storyja** | Slanje email upozorenja |
| **Opis** | Kao sistem, želim automatski slati upozorenja članovima kada im istekne rok vraćanja knjiga, kako bi bili obaviješteni o kašnjenju. |
| **Poslovna vrijednost** | Pomaže u smanjenju izgubljenih ili zakašnjelih knjiga i olakšava bibliotekaru kontrolu nad fondom. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Sistem šalje upozorenje članovima istog dana kada im istekne rok vraćanja. <br> Email sadrži naziv knjige i datum kada je rok istekao. |
| **Pretpostavke / Otvorena pitanja** | Pretpostavlja se da su email adrese članova validne. |
| **Veze i zavisnosti** | Implementirano zaduživanje knjiga. |

---

## US-21: Mjesečni izvještaji za upravu

| **ID storyja** | US-21 |
|---------------|-------|
| **Naziv storyja** | Mjesečni izvještaji za upravu |
| **Opis** | Kao administrator biblioteke, želim generisati mjesečne izvještaje o zaduživanjima, rezervacijama i članstvu, kako bih mogao pratiti stanje biblioteke i donositi informisane odluke. |
| **Poslovna vrijednost** | Omogućava upravi uvid u korištenje biblioteke i pomaže u planiranju resursa i strategija. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Administrator može generisati izvještaj za bilo koji mjesec. <br> Izvještaj sadrži broj i listu zaduženja, aktivnih rezervacija i članova. <br> Izvještaj se može pregledati i preuzeti u PDF formatu. |
| **Pretpostavke / Otvorena pitanja** | Podaci o zaduživanjima, rezervacijama i članstvu su tačni i ažurirani. |
| **Veze i zavisnosti** | Domain Model o zaduženjima, rezervacijama i korisnima biblioteke. |

---

## US-22: Kazne za kasno vraćanje knjiga

| **ID storyja** | US-22 |
|---------------|-------|
| **Naziv storyja** | Kazne za kasno vraćanje knjiga |
| **Opis** | Kao sistem, želim evidentirati i obračunavati kazne kada član prekorači rok vraćanja knjige, kako bi članovi bili odgovorni i fond biblioteke bio zaštićen. |
| **Poslovna vrijednost** | Pomaže u smanjenju kašnjenja, osigurava poštovanje pravila i poboljšava upravljanje fondom. |
| **Prioritet** | Nizak |
| **Acceptance Criteria** | Sistem automatski obračunava kaznu za svaku knjigu koja nije vraćena na vrijeme. <br> Član može vidjeti iznos kazne u svom profilu. |
| **Pretpostavke / Otvorena pitanja** | Utvrđen plan po kojem će se računati kazne. |
| **Veze i zavisnosti** | Knjiga je zadužena i poslan je mail upozorenja. |

<br>

# Sprint 11


## US-23: Sistematsko testiranje i bug fixing

| **ID storyja** | US-23 |
|---------------|-------|
| **Naziv storyja** | Sistematsko testiranje i bug fixing |
| **Opis** | Kao tim, želimo provesti sistematsko testiranje svih implementiranih funkcionalnosti i otkloniti kritične greške, kako bi sistem bio stabilan za demonstraciju. |
| **Poslovna vrijednost** | Osigurava stabilnost i pouzdanost sistema, smanjuje rizik od grešaka tokom demonstracije i korištenja te povećava kvalitet i vjerodostojnost projekta. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Svi test scenariji za funkcionalnosti su definirani i izvršeni. <br> Svi bugovi su identifikovani i otklonjeni. <br> Test evidencija je pohranjena u repozitoriju. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 - US-22: Implementirane sve funkcionalnosti. |


---

## US-24: Izrada liste poznatih ograničenja i tehničkog duga

| **ID storyja** | US-24 |
|---------------|-------|
| **Naziv storyja** | Izrada liste poznatih ograničenja i tehničkog duga |
| **Opis** | Kao tim, želimo napraviti listu stvari koje ograničavaju sistem i nedovršene tehničke dijelove, kako bismo znali šta još treba uraditi i mogli jasno obavijestiti druge o stanju projekta. |
| **Poslovna vrijednost** | Omogućava timu i stakeholderima da jasno vide postojeća ograničenja i nedovršene dijelove sistema, što pomaže u realnom planiranju rada, donošenju odluka i transparentnoj komunikaciji o napretku projekta. |
| **Prioritet** | Srednji |
| **Acceptance Criteria** | Lista svih poznatih ograničenja sistema je dokumentovana. <br> Lista svih elemenata tehničkog duga je dokumentovana. <br> Svaka stavka na listi sadrži kratak opis i utjecaj na projekat. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | Sprovedeno testiranje sistema. |

<br>

# Sprint 12

## US-25: Izrada Release Notes

| **ID storyja** | US-25 |
|---------------|-------|
| **Naziv storyja** | Izrada Release Notes |
| **Opis** | Kao tim, želimo kreirati Release Notes koji opisuju sve implementirane funkcionalnosti, poznata ograničenja i upute za instalaciju, kako bi finalna verzija bila profesionalno dokumentovana. |
| **Poslovna vrijednost** | Release Notes su formalni dokaz šta je isporučeno – obavezan artefakt za završnu odbranu projekta. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Release Notes sadrže listu svih implementiranih funkcionalnosti. <br> Navedena su poznata ograničenja i poznati bugovi. <br> Uključene su upute za instalaciju i pokretanje sistema. <br> Dokument je pohranjen u repozitoriju. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-23: Sistematsko testiranje i bug fixing. <br> US-24: Izrada liste poznatih ograničenja i tehničkog duga. |

---

## US-26: Izrada korisničke dokumentacije

| **ID storyja** | US-26 |
|---------------|-------|
| **Naziv storyja** | Izrada korisničke dokumentacije |
| **Opis** | Kao tim, želimo kreirati korisničku dokumentaciju koja objašnjava kako koristiti sistem, kako bi krajnji korisnici i ocjenjivači mogli razumjeti sistem bez tehničkog predznanja. |
| **Poslovna vrijednost** | Omogućava krajnjim korisnicima da lako razumiju i koriste sistem, smanjuje potrebu za dodatnim objašnjenjima i povećava profesionalnost projekta.|
| **Prioritet** | Visok |
| **Acceptance Criteria** | Dokumentovane su upute za sve korisničke uloge (Član, Bibliotekar, Administrator). <br> Uključeni su screenshot-ovi ili opisi ključnih ekrana. <br> Objašnjen je tok rada za najčešće scenarije (zaduživanje, pretraga, rezervacija). <br> Dokument je pohranjen u repozitoriju. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 - US-22: Implementirane sve funkcionalnosti. |

---

## US-27: Izrada tehničke dokumentacije

| **ID storyja** | US-27 |
|---------------|-------|
| **Naziv storyja** | Izrada tehničke dokumentacije |
| **Opis** | Kao tim, želimo kreirati tehničku dokumentaciju koja opisuje arhitekturu, API- je i razvojno okruženje, kako bi sistem bio razumljiv drugom developeru.|
| **Poslovna vrijednost** | Olakšava održavanje, nadogradnju i smanjuje vrijeme potrebno za integraciju novih developera. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Dokumentovana je arhitektura sistema. <br> Naveden je postupak postavljanja razvojnog okruženja. <br> Ažurirani su svi obavezni logovi (AI Usage Log, Decision Log). |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 - US-22: Implementirane sve funkcionalnosti. |


<br>

# Sprint 13

## US-28: Priprema i izvođenje završne demonstracije

| **ID storyja** | US-28 |
|---------------|-------|
| **Naziv storyja** | Priprema i izvođenje završne demonstracije |
| **Opis** | Kao tim, želimo pripremiti i izvesti završnu demonstraciju sistema, kako bismo prikazali sve implementirane funkcionalnosti i pokazali da sistem radi ispravno.|
| **Poslovna vrijednost** | Završna demonstracija pokazuje funkcionisanje sistema i direktno utiče na ocjenu projekta. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Svaki član tima je u stanju objasniti dijelove sistema za koje je odgovoran. <br> Sistem radi stabilno tokom demonstracije. |
| **Pretpostavke / Otvorena pitanja** | |
| **Veze i zavisnosti** | US-01 - US-22: Implementirane sve funkcionalnosti. <br> US-26: Izrađena korisnička dokumentacija. <br> US-27: Izrađena tehička dokumentacija. |
