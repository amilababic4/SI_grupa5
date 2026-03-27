# Naziv Projekta
AmKoLib *(Bibliotečki sistem za upravljanje fondom i korisnicima)*

---

## Problem koji sistem rješava

Biblioteke koje nemaju digitalizovan sistem upravljanja fondom suočavaju se sa problemima poput ručnog praćenja iznajmljivanja knjiga, evidencije korisnika i statusa članarina u papirnoj formi ili improvizovanim rješenjima. To dovodi do grešaka u evidenciji, teškoća u praćenju rokova vraćanja, nepreglednog stanja dostupnih primjeraka i otežanog rada bibliotekara u svakodnevnim zadacima.

AmKoLib rješava ovaj problem pružanjem digitalnog sistema koji bibliotečkom osoblju omogućava centralizovano i pregledno upravljanje bibliotečkim fondom i korisnicima, dok istovremeno klijentima biblioteke pruža uvid u dostupnost literature i status vlastite članarine.

---

## Ciljni korisnici

Aplikacija je namijenjena osoblju biblioteke i klijentima biblioteke koji pristupaju sistemu putem namjenskog interfejsa.

Korisnici sistema su:
- **Bibliotekar** — primarni korisnik sistema, obavlja svakodnevne operacije upravljanja fondom i korisnicima
- **Sistem administrator** — odgovoran za upravljanje korisničkim nalozima i tehničko održavanje sistema
- **Klijent biblioteke** — ima ograničen, read-only uvid u dostupnu literaturu i u status vlastite članarine

Stakeholderi sistema su:
- Osoblje biblioteke (direktni korisnici)
- Klijenti biblioteke (direktni korisnici)
- Distributer knjiga koji snabdijeva biblioteku

---

## Vrijednost sistema

### Za bibliotekara
Bibliotekar dobija pregled:
- Kompletnog bibliotečkog fonda i dostupnosti pojedinih naslova
- Podataka o registrovanim klijentima i statusu njihovih članarina
- Iznajmljenih knjiga po korisniku te rokova vraćanja
- Statusa rezervacija literature

### Za klijenta biblioteke
Klijent putem sistema može:
- Pregledati dostupnu literaturu u biblioteci
- Provjeriti status vlastite članarine
- Obaviti online produžetak članarine
- Primiti obavještenje o isteku članarine i roku povratka iznajmljene literature

### Za sistem administratora
Administrator može upravljati korisničkim nalozima bibliotekara i konfiguracijom sistema.

---

## Scope MVP Verzije

MVP verzija fokusira se na formiranje funkcionalnog kostura aplikacije sa osnovnim CRUD operacijama, bez oslanjanja na eksterne servise ili obavještajne mehanizme.

### Bibliotekar
- Uvid u bibliotečki fond (pregled, dodavanje, uređivanje, brisanje knjiga)
- Uvid u klijente i podatke o klijentima
- Upravljanje iznajmljivanjem i vraćanjem knjiga
- Pregled statusa članarine korisnika

### Sistem administrator
- Upravljanje korisničkim nalozima

### Klijent biblioteke
- Uvid u trenutno dostupnu literaturu
- Uvid u status vlastite članarine

---

## Šta ne ulazi u MVP

Sljedeće funkcionalnosti su svjesno isključene iz MVP-a radi formiranja stabilnog kostura aplikacije:

| Funkcionalnost | Razlog isključivanja |
|---|---|
| Automatska obavještenja o isteku članarine | Zahtijeva integraciju eksternog notifikacionog servisa |
| Automatska obavještenja o roku povratka knjige | Zahtijeva integraciju eksternog notifikacionog servisa |
| Kazne za kasno vraćanje | Ovisi o notifikacionom sistemu i poslovnoj politici |
| Rezervacija literature | Složeniji workflow |
| Online produžetak članarine od strane klijenta | Zahtijeva integraciju ekternog uređaja za plaćanje |
| Integracija sa distributer-om knjiga | Vanjski servis |

---

## Ključna ograničenja i pretpostavke

### Ograničenja MVP verzije

- Sistem podržava samo osnovne CRUD operacije nad knjigama (bez rezervacija)
- Sistem posmatra biblioteku kao izolovan sistem, nema komunikacije između više biblioteka
- Nema implementacije automatskih notifikacija ni eksternih komunikacijskih servisa
- Interfejs je dizajniran isključivo za desktop platforme
- Klijentske profile kreira i upravlja bibliotekar, nema samostalne registracije klijenata

### Pretpostavke MVP verzije

- Uneseni podaci su ispravnog formata (validacija osnovnih inputa je implementirana, ali sistem ne provjerava semantičku ispravnost unosa poput stvarnog postojanja naslova)
- Sve kopije iste knjige smatraju se identičnim primjercima
- Dostupna je stabilna internet konekcija u toku rada sistema
- Sistemsko vrijeme host računara je ispravno postavljeno
- Broj istovremenih korisnika je mali, primarno jedan aktivan bibliotekar u datom trenutku