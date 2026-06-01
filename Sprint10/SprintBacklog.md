# Sprint Backlog – Sprint 10

## Opis sprinta

Sprint 10 fokusira se na unapređenje komunikacije između sistema i korisnika, te proširenje administrativnih funkcionalnosti unutar SmartLib sistema. Cilj sprinta je automatizovati obavještavanje članova o rokovima vraćanja knjiga, osigurati transparentnost promjena kroz audit log, uvesti sistem kazni za kasno vraćanje, te omogućiti online upravljanje članarinom i integraciju sa distributerima knjiga.

Tokom ovog sprinta implementiraju se funkcionalnosti vezane za:
* automatsko slanje email podsjetnika i upozorenja o rokovima vraćanja knjiga,
* obavještavanje bibliotekara o novim rezervacijama putem emaila,
* automatsko evidentiranje promjena u sistemu u audit logu,
* obračun i pregled kazni za kasno vraćanje knjiga,
* online podnošenje i obrada zahtjeva za produženje članarine,
* integraciju sa distributerom knjiga putem email zahtjeva za nabavku.

<br>

| ID | Naziv stavke | Kratak opis | Prioritet | Procjena napora | Status |
|:--:| :--- | :--- | :---: | :---: | :---: |
| PB-41 | Slanje email upozorenja | Automatsko slanje podsjetnika i upozorenja članovima o isteku roka vraćanja knjiga i kašnjenjima. | Nizak | M | **Završeno** |
| PB-42 | Obavještavanje bibliotekara o novoj rezervaciji | Sistem automatski šalje email bibliotekaru kada član kreira novu rezervaciju knjige. | Nizak | S | **Završeno** |
| PB-46 | Audit log promjena | Automatsko evidentiranje svih promjena knjiga i korisničkih naloga sa detaljima o akciji, vremenu i korisniku. | Nizak | M | **Završeno** |
| PB-47 | Kazne za kasno vraćanje knjiga | Automatski obračun kazni po danu kašnjenja i pregled ukupnog duga na profilu člana. | Nizak | M | **Završeno** |
| PB-48 | Online produžetak članarine | Član može podnijeti zahtjev za produženje članarine, a bibliotekar ga pregledava i odobrava ili odbija. | Srednji | L | **Završeno** |
| PB-49 | Integracija sa distributerom knjiga | Bibliotekar može kreirati i poslati zahtjev za nabavku knjige direktno distributeru putem emaila iz sistema. | Nizak | M | **Završeno** |
<br>

## Sprint Backlog stavke:
## PB-41: Slanje email upozorenja

### Naziv: Slanje emaila samo korisnicima sa validnom email adresom
### US-81: Kao sistem, želim slati podsjetnike samo članovima koji imaju evidentiranu validnu email adresu, kako bih izbjegao neuspješna slanja.
**Acceptance Criteria:**
- Prije slanja sistem provjerava da član ima email adresu.
- Ako email ne postoji, poruka se ne šalje.
- Neuspjelo slanje se evidentira u sistemu ako postoji logging.
- Ostala validna slanja se izvršavaju normalno.

<br>

---

### Naziv: Zaustavljanje podsjetnika nakon vraćanja knjige
### US-82: Kao sistem, želim prestati slati podsjetnike kada je knjiga vraćena, kako član ne bi dobijao netačne poruke.
**Acceptance Criteria:**
- Kada je zaduženje završeno, sistem ga ne uključuje u buduća slanja podsjetnika.
- Knjige vraćene prije roka ne dobijaju podsjetnik na istek.
- Knjige vraćene nakon zakašnjenja ne dobijaju nove mailove nakon povrata.
- Slanje se zasniva samo na aktivnim zaduženjima.

<br>

---
### Naziv: Notifikacija članu o podsjetniku isteka roka vraćanja
### US-83: Kao član biblioteke, želim da dobijem email podsjetnik prije isteka roka vraćanja knjige kako bih je mogao na vrijeme vratiti.
**Acceptance Criteria:**
- Kada je rok vraćanja knjige 2 dana od isteka, tada sistem automatski šalje email podsjetnik
- Kada član ima više zaduženih knjiga, tada se šalje podsjetnik za svaku knjigu posebno
- Email mora sadržavati naziv knjige i datum roka vraćanja
- Mail se šalje samo za aktivna zaduženja

<br>

---

### Naziv: Notifikacija članu o upozorenju isteka roka vraćanja
### US-84: Kao član biblioteke, želim dobiti email upozorenje na dan kada mi ističe rok vraćanja knjige kako bih znao da trebam odmah vratiti knjigu.
**Acceptance Criteria:**
- Kada rok vraćanja knjige istekne, tada sistem automatski šalje email upozorenje
- Kada član ima više knjiga kojima ističe rok, tada se šalje upozorenje za svaku knjigu posebno
- Email mora sadržavati naziv knjige i datum isteka roka
- Upozorenje se šalje samo za knjige koje nisu vraćene

<br>

---

### Naziv: Podsjetnik o kašnjenju
### US-85: Kao član biblioteke, želim dobiti podsjetnik ako kasnim s vraćanjem knjige kako bih bio svjestan da trebam što prije vratiti knjigu.
**Acceptance Criteria:**
- Kada je knjiga zakasnila više od 1 dana, tada sistem šalje podsjetnik o kašnjenju
- Email mora sadržavati naziv knjige i broj dana kašnjenja
- Podsjetnici se zaustavljaju kada se knjiga vrati

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava pravovremeno obavještavanje članova o rokovima vraćanja knjiga, smanjuje broj kašnjenja i unapređuje upravljanje bibliotečkim fondom.  |
| **Pretpostavke / Otvorena pitanja** | Članovi imaju validne email adrese u sistemu. <br> Sistem podržava automatsko slanje emailova. |
 **Veze i zavisnosti** |PB-31 Evidencija zaduživanja i vraćanja knjiga. <br> Pregled vlastitih zaduženja. |

---

<br>

## PB-42: Obavještavanje bibliotekara o novoj rezervaciji

### Naziv: Slanje jedne notifikacije po rezervaciji
### US-86: Kao bibliotekar, želim da dobijem email obavijest svaki put kada član kreira rezervaciju knjige kako bih bio informisan o novim rezervacijama.
**Acceptance Criteria:**
- Kada član kreira novu rezervaciju, tada sistem automatski šalje email bibliotekaru
- Email mora sadržavati ime i prezime člana
- Email mora sadržavati naslov rezervisane knjige
- Email mora sadržavati datum kreiranja rezervacije
- Sistem šalje obavijest samo jednom po svakoj rezervaciji

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava bibliotekaru pravovremeno informisanje o novim rezervacijama, što poboljšava organizaciju rada i pripremu knjiga za korisnike.  |
| **Pretpostavke / Otvorena pitanja** | Sistem podržava automatsko slanje email obavijesti. |
 **Veze i zavisnosti** | Evidencija zaduživanja i vraćanja knjiga. |

---

<br>

## PB-46: Audit log promjena

### Naziv: Automatsko evidentiranje promjena
### US-91: Kao član osoblja, želim da sistem automatski evidentira svako dodavanje, izmjenu i brisanje knjiga kako bih mogao pratiti promjene u bibliotečkom fondu.
**Acceptance Criteria:**
- Kada se knjiga doda, izmijeni ili obriše, tada sistem automatski kreira audit zapis
- Audit zapis sadrži naziv akcije, datum i vrijeme promjene
- Audit zapis sadrži korisnika koji je izvršio promjenu
- Svi zapisi se čuvaju u sistemu i nisu dostupni za izmjenu

<br>

---
### Naziv: Bilježenje promjena korisničkih naloga
### US-92: Kao administrator, želim da sistem bilježi promjene nad korisničkim nalozima kako bih mogao pratiti sigurnost i aktivnosti korisnika.
**Acceptance Criteria:**
- Kada se korisnik kreira, izmijeni ili deaktivira, tada se kreira audit zapis
- Zapis sadrži vrstu promjene i korisnika koji je izvršio akciju
- Zapis sadrži datum i vrijeme promjene

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava transparentnost i sigurnost sistema kroz praćenje svih važnih promjena, olakšava dijagnostiku problema i reviziju aktivnosti korisnika. |
| **Pretpostavke / Otvorena pitanja** | Sve promjene se u sistemu čuvaju 30 dana. |
 **Veze i zavisnosti** | PB-25: Evidencija zaduživanja i vraćanja knjiga. <br> PB-24: Rezervacija knjiga. <br> PB-26: Upravljanje korisnicima. |

---

<br>

## PB-47: Kazne za kasno vraćanje knjiga
### Naziv: Evidentiranje kazne po zaduženju
### US-93: Kao sistem, želim automatski obračunati kaznu za svaku knjigu koja nije vraćena u predviđenom roku kako bi se osigurala disciplina i poštovanje pravila korištenja.
**Acceptance Criteria:**
- Kada je knjiga vraćena nakon isteka roka, tada sistem automatski obračunava kaznu po danu kašnjenja
- Kazna se računa za svaki dan kašnjenja
- Kazna se veže za konkretno zaduženje i člana

<br>

---

### Naziv: Prikaz ukupnog duga člana
### US-94: Kao član biblioteke, želim da mogu pregledati ukupne kazne kako bih bio informisan o svojim obavezama.
**Acceptance Criteria:**
- Kada član pristupi svom profilu, tada vidi pregled svih kazni
- Sistem prikazuje ukupni iznos kazne
- Ako član nema kazni, sistem prikazuje poruku da nema dugovanja
- Podaci o kaznama se ažuriraju nakon svakog vraćanja knjige

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava disciplinu u korištenju bibliotečkog fonda, smanjuje kašnjenja u vraćanju knjiga i obezbjeđuje dodatnu kontrolu nad zaduženjima. |
| **Pretpostavke / Otvorena pitanja** | Postoji definisana pravila obračuna kazni. |
 **Veze i zavisnosti** | PB-25: Evidencija zaduživanja i vraćanja knjiga. |

---

<br>

## PB-48: Online produžetak članarine

### Naziv: Mogućnost produžavanja
### US-95: Kao član biblioteke, želim vidjeti opciju za produženje članarine na svom profilu kako bih znao kada i kako mogu produžiti članarinu.
**Acceptance Criteria:**
- Kada je član prijavljen i otvori profil, tada vidi opciju "Produži članarinu" ako je članarina aktivna ili je istekla 
- Kada članarina nije nikad evidentirana, opcija za produženje nije vidljiva (to radi bibliotekar)
- Sistem prikazuje trenutni datum isteka i status
- Ako član već ima zahtjev za produženje u toku (status "Na čekanju"), prikazuje se info poruka umjesto forme

<br>

---

### Naziv: Podnošenje zahtjeva za produženje
### US-96: Kao član biblioteke, želim podnijeti zahtjev za produženje članarine kako bih inicirao proces obnove svog članstva.

**Acceptance Criteria:**
- Forma prikazuje opcije trajanja produženja: 1, 3, 6 ili 12 mjeseci
- Forma prikazuje novi procijenjeni datum isteka na osnovu odabira
- Član može dodati napomenu (opcionalno polje)
- Klikom na "Pošalji zahtjev" zahtjev se kreira sa statusom "Na čekanju"
- Nakon slanja, prikazuje se potvrda: "Zahtjev je uspješno poslan. Bibliotekar će ga obraditi."

<br>

---
### Naziv: Odobravanje zahtjeva (strana bibliotekara)
### US-97: Kao bibliotekar, želim pregledati i obraditi zahtjeve za produženje članarine kako bih odobrio ili odbio produženje.
**Acceptance Criteria:**
- Bibliotekar ima pregled svih zahtjeva za produženje sa statusom "Na čekanju"
- Za svaki zahtjev vidi: ime člana, trenutni datum isteka,traženo trajanje, novi datum isteka, datum podnošenja zahtjeva
- Bibliotekar može odobriti zahtjev: datum isteka se ažurira, status zahtjeva postaje "Odobreno"
- Bibliotekar može odbiti zahtjev uz razlog: status postaje "Odbijeno"
- Nakon obrade, član može vidjeti rezultat na svom profilu

<br>

---

### Naziv: Pregled statusa zahtjeva (strana člana)
### US-97: Kao član biblioteke, želim vidjeti status svog zahtjeva za produženje kako bih znao da li je obrađen.
**Acceptance Criteria:**
- Na profilu člana postoji sekcija "Zahtjevi za produženje"
- Prikazuje se status zahtjeva: "Na čekanju" / "Odobreno" / "Odbijeno"
- Kada je status "Odbijeno", prikazuje se razlog odbijanja
- Kada je status "Odobreno", prikazuje se novi datum isteka
- Historija zahtjeva je vidljiva (zadnja 3 zahtjeva)

<br>

---
| **Prioritet** | Srednji |
|---------------|-------|
| **Poslovna vrijednost** | Omogućava jednostavno online upravljanje članarinom, smanjuje potrebu za dolaskom u biblioteku i poboljšava korisničko iskustvo. |
| **Pretpostavke / Otvorena pitanja** | Član je prijavljen u sistem. <br> Postoji definisan datum isteka članarine. <br> Sistem podržava ažuriranje članarine. <br> Da li postoji plaćanje ili je besplatno? |
 **Veze i zavisnosti** | PB-27 Upravljanje statusom članarine. <br> PB-28 Pregled statusa članarine |

---

<br>

## PB-49: Integracija sa distributerom knjiga

### Naziv: Unos podataka nabavke
### US-98: Kao bibliotekar, želim unijeti podatke o knjizi koju želim naručiti kako bih pokrenuo proces nabavke.
**Acceptance Criteria:**
- Bibliotekar može otvoriti formu za zahtjev za nabavku knjige
- Forma sadrži polja za naziv knjige, autora, izdavača i broj primjeraka
- Bibliotekar može unijeti dodatni opis ili napomenu
- Sistem validira da su obavezna polja popunjena

<br>

---
### Naziv: Slanje zahtjeva distributeru
### US-99: Kao bibliotekar, želim poslati zahtjev distributeru direktno iz sistema kako bih pojednostavio proces naručivanja knjiga.
**Acceptance Criteria:**
- Kada bibliotekar pošalje zahtjev, sistem generiše email poruku
- Email sadrži podatke o traženoj knjizi
- Email se šalje na unaprijed definisanu adresu distributera
- Sistem evidentira da je zahtjev poslan

<br>

---
### Naziv: Potvrda slanja
### US-100: Kao bibliotekar, želim dobiti potvrdu da je zahtjev uspješno poslan distributeru kako bih znao da je proces nabavke pokrenut.
**Acceptance Criteria:**
- Nakon slanja zahtjeva, sistem prikazuje poruku o uspješnom slanju
- Ako slanje emaila ne uspije, sistem prikazuje odgovarajuću poruku o grešci

<br>

---
| **Prioritet** | Nizak |
|---------------|-------|
| **Poslovna vrijednost** | Olakšava proces nabavke novih knjiga, ubrzava komunikaciju sa distributerima i poboljšava upravljanje bibliotečkim fondom. |
| **Pretpostavke / Otvorena pitanja** | Sistem ima definisanu email adresu distributera. |
 **Veze i zavisnosti** | PB-35: Slanje email notifikacija. |

---

<br>
