# Naziv Projekta
**SmartLib** 

## Problem koji sistem rješava
Biblioteke koje nemaju digitalizovan sistem upravljanja fondom suočavaju se sa problemima poput ručnog praćenja iznajmljivanja knjiga, evidencije korisnika i statusa članarina u papirnoj formi ili improvizovanim rješenjima. To dovodi do grešaka u evidenciji, teškoća u praćenju rokova vraćanja, nepreglednog stanja dostupnih primjeraka i otežanog rada bibliotekara u svakodnevnim zadacima.

**SmartLib** rješava ovaj problem pružanjem digitalnog sistema koji bibliotečkom osoblju omogućava centralizovano i pregledno upravljanje bibliotečkim fondom i korisnicima, dok istovremeno članovima biblioteke pruža uvid u dostupnost literature i status vlastite članarine.


## Ciljni korisnici i Stakeholderi
Aplikacija je namijenjena osoblju biblioteke i njenim korisnicima koji pristupaju sistemu putem namjenskog interfejsa.

### Korisnici sistema:
1. **Bibliotekar** 

Operativni korisnik sistema, obavlja svakodnevne operacije upravljanja fondom i korisnicima.

2. **Član biblioteke (Klijent)** 

Aktivni korisnik koji ima uvid u dostupnu literaturu i status vlastite članarine, ujedno mogućnost rezervacije literature.

3. **Direktor biblioteke**

Upravno osoblje koje sistem koristi za pregled poslovanja kroz statistike i izvještaje.

4. **Administrator sistema** 

Odgovoran za upravljanje korisničkim nalozima i tehničko održavanje sistema.

### Ostali poslovni stakeholderi sistema:
5. **IT tim** 

Podrška i razvoj, odgovoran za tehničku realizaciju sistema i usklađivanje zahtjeva s implementacijom.

6. **Dobavljači knjiga (Distributeri)** 

Vanjski saradnici čija se komunikacija i proces nabavke olakšava preciznim uvidom u trenutno stanje bibliotečkog fonda.


7. **Autori i izdavači**

Vanjski saradnici s niskim direktnim uticajem na sistem, relevantni u kontekstu poštovanja autorskih prava i vidljivosti publikacija zastupljenih u fondu.


## Vrijednost sistema

### Bibliotekar dobija pregled:
* Kompletnog bibliotečkog fonda i dostupnosti pojedinih naslova.
* Podataka o registrovanim članovima i statusu njihovih članarina.
* Iznajmljenih knjiga po korisniku te rokova vraćanja.
* Statusa rezervacija literature.

### Član biblioteke putem sistema može:
* Registrovati se.
* Pregledati dostupnu literaturu u biblioteci.
* Provjeriti status vlastite članarine.
* Obaviti online produžetak članarine.
* Primiti obavještenje o isteku članarine i roku povratka iznajmljene literature.

### Direktor biblioteke:
* Pristup preciznim izvještajima i statistikama o radu biblioteke (npr. najčitanije knjige, broj aktivnih članova) koji olakšavaju donošenje poslovnih i finansijskih odluka.

### Sistem administrator:
* Administrator može upravljati korisničkim nalozima i konfiguracijom sistema.



## Scope MVP Verzije
MVP verzija fokusira se na formiranje funkcionalnog kostura aplikacije sa osnovnim CRUD operacijama, bez oslanjanja na eksterne servise, obavještajne mehanizme ili naprednu analizu podataka.

### Bibliotekar
* Uvid u bibliotečki fond (pregled, dodavanje, uređivanje, brisanje knjiga).
* Uvid u članove i podatke o članovima.
* Upravljanje iznajmljivanjem i vraćanjem knjiga.
* Pregled statusa članarine korisnika.

### Sistem administrator
* Upravljanje korisničkim nalozima.

### Član biblioteke
* Može kreirati vlastiti račun.
* Uvid u trenutno dostupnu literaturu (pretraga fonda).
* Uvid u status vlastite članarine.



## Šta ne ulazi u MVP
Sljedeće funkcionalnosti su svjesno isključene iz MVP-a radi formiranja stabilnog kostura aplikacije:

| Funkcionalnost | Razlog isključivanja |
| :--- | :--- |
| **Napredni izvještaji i statistika (za Direktora)** | MVP se fokusira na operativni rad (CRUD), analitika zahtijeva suvišni početni napor. |
| **Automatska obavještenja o isteku članarine** | Zahtijeva integraciju eksternog notifikacionog servisa. |
| **Automatska obavještenja o roku povratka knjige** | Zahtijeva integraciju eksternog notifikacionog servisa. |
| **Kazne za kasno vraćanje** | Ovisi o notifikacionom sistemu i poslovnoj politici. |
| **Rezervacija literature** | Složeniji workflow. |
| **Online produžetak članarine od strane člana** | Zahtijeva integraciju eksternog servisa za plaćanje. |
| **Integracija sa dobavljačima/distributerima** | Vanjski servisi. |



## Ključna ograničenja i pretpostavke

### Ograničenja MVP verzije
* Sistem podržava samo osnovne CRUD operacije nad knjigama i članovima.
* Sistem posmatra biblioteku kao izolovan sistem, nema komunikacije između više biblioteka.
* Nema implementacije automatskih notifikacija ni eksternih komunikacijskih servisa.
* Interfejs je dizajniran isključivo za desktop platforme.
* Klijentske profile kreira i njima upravljaju isključivo bibliotekar i sistem administrator, dakle nema samostalne registracije članova kroz sistem u ovoj fazi.

### Pretpostavke MVP verzije
* Uneseni podaci su ispravnog formata (osnovna validacija je tu, ali bez duboke semantičke provjere, zanemarene su greške prilikom unošenja imena, naslova literature).
* Sve kopije iste knjige smatraju se identičnim primjercima.
* Dostupna je stabilna internet konekcija u toku rada sistema.
* Sistemsko vrijeme host računara je ispravno postavljeno.
* Mali broj istovremenih korisnika.