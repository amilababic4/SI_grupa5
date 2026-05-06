# Decision Log

## Opis dokumenta
Decision Log se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i
procesnih odluka.


## DL-01
**Datum**: 25.04.2026  
**Naziv odluke:** Korištenje Cookie autentifikacije za Web  

**Opis problema:**
Potrebno upravljati korisničkom sesijom u Web aplikaciji  

**Razmatrane opcije:**
- JWT autentifikacija  
- Cookie autentifikacija  

**Odabrana opcija:**
Cookie autentifikacija  

**Razlog izbora:**
Jednostavnija implementacija u MVC aplikaciji i prirodna podrška za sesije  

**Posljedice odluke:**
- Lakše upravljanje sesijom  
- Manja kompleksnost implementacije  

**Status:**
Aktivna  

**Povezani PB:**
PB-17  


## DL-02
**Datum:** 25.04.2026  
**Naziv odluke:** Korištenje JWT za API  

**Opis problema:**
Potrebna autentifikacija i zaštita API endpointa  

**Razmatrane opcije:**
- Cookie autentifikacija  
- JWT autentifikacija  

**Odabrana opcija:**
JWT autentifikacija  

**Razlog izbora:**
Standardno rješenje za sigurnu komunikaciju između klijenta i servera  

**Posljedice:**
- Potrebna konfiguracija tokena  
- Dodatna kompleksnost u odnosu na cookie  

**Status:**
Aktivna  

**Povezani PB:**
PB-17  


## DL-03
**Datum:** 25.04.2026  
**Naziv odluke:** Korištenje generičke poruke greške pri loginu  

**Opis problema:**
Potrebno spriječiti otkrivanje informacija o korisničkim podacima  

**Razmatrane opcije:**
- Specifične poruke (email ne postoji / lozinka pogrešna)  
- Generička poruka  

**Odabrana opcija:**
Generička poruka  

**Razlog izbora:**
Povećava sigurnost sistema i sprječava napade  

**Posljedice:**
- Manje informacija korisniku  
- Veća sigurnost sistema  

**Status:**
Aktivna  

**Povezani PB:**
PB-17  


## DL-04
**Datum:** 30.04.2026  
**Naziv odluke:** Uvođenje dvostrukog unosa lozinke prilikom kreiranja naloga  

**Opis problema:**
Prilikom kreiranja korisničkog naloga postojala je mogućnost da korisnik napravi grešku pri unosu lozinke, što bi moglo rezultirati nemogućnošću prijave u sistem odmah nakon registracije. Potrebno je bilo odlučiti kako smanjiti rizik od pogrešno unesene lozinke.  

**Razmatrane opcije:**  
- Jednostruki unos lozinke sa naknadnom promjenom putem administratora  
- Dvostruki unos lozinke (polje za potvrdu lozinke) prilikom kreiranja naloga  

**Odabrana opcija:** 
Dvostruki unos lozinke sa poljem za potvrdu  

**Razlog izbora:** 
Dvostruki unos lozinke je standardna praksa u web aplikacijama koja značajno smanjuje rizik od grešaka pri unosu. Ovaj pristup ne zahtijeva dodatne sistemske resurse i pruža korisnicima trenutnu povratnu informaciju ukoliko se unosi ne poklapaju. Alternativa sa naknadnom promjenom preko administratora bi stvorila nepotreban dodatni posao za osoblje biblioteke.  

**Posljedice odluke:**
- Smanjen rizik od pogrešno unesene lozinke prilikom registracije  
- Potrebna validacija na frontendu i backendu da se oba polja poklapaju  
- Blago proširena forma za kreiranje naloga  
- Poboljšano korisničko iskustvo pri prvoj prijavi  

**Status:**
Aktivna  

**Povezani PB:**
PB-18  


## DL-05
**Datum:**  30.04.2026  
**Naziv odluke:** Ciljane UI izmjene umjesto potpunog redizajna aplikacije  

**Opis problema:** 
Korisnički interfejs aplikacije je zahtijevao vizualna poboljšanja kako bi sistem izgledao profesionalnije i bio prikladniji za upotrebu u kontekstu bibliotečkog informacionog sistema. Pitanje je bilo da li izvršiti potpuni redizajn cijelog interfejsa ili se fokusirati samo na ključne dijelove.  

**Razmatrane opcije:**
- Potpuni redizajn cijelog korisničkog interfejsa  
- Ciljane izmjene samo na relevantnim dijelovima interfejsa (navigacija, layout, forme za rad sa knjigama)  

**Odabrana opcija:** 
Ciljane izmjene na relevantnim dijelovima interfejsa  

**Razlog izbora:** 
Potpuni redizajn bi donio značajan rizik regresija i zahtijevao bi više vremena nego što je bilo predviđeno za Sprint 6. Ciljane izmjene omogućavaju brzu isporuku vidljivih poboljšanja bez narušavanja postojećih funkcionalnosti. Ovaj pristup zadržava konzistentnost sa već implementiranim dijelovima aplikacije i smanjuje rizik od uvođenja novih grešaka.  

**Posljedice odluke:**
- Poboljšan izgled navigacije, layouta i ključnih formi  
- Profesionalniji i moderniji izgled aplikacije  
- Izbjegnut rizik velikog refaktora postojećih view-ova i stilova  
- Neke stranice zadržavaju prethodni izgled do budućih sprintova  

**Status:** 
Aktivna  

**Povezani PB:** 
PB-22, PB-28  


## DL-06
**Datum:** 30.04.2026  
**Naziv odluke:** Prelazak sa PostgreSQL na MySQL bazu podataka  

**Opis problema:** 
Projekat je inicijalno koristio PostgreSQL kao sistem za upravljanje bazom podataka. Tokom Sprint Review-a za Sprint 5, u dogovoru sa asistentom, razmatrana je mogućnost promjene baze podataka kako bi se bolje uskladila sa razvojnim okruženjem i alatima dostupnim timu.  

**Razmatrane opcije:**
- Nastavak korištenja PostgreSQL  
- Prelazak na MySQL  

**Odabrana opcija:** 
Prelazak na MySQL  

**Razlog izbora:** 
Odluka je donesena u dogovoru sa asistentom tokom Sprint Review-a. MySQL je timu poznatiji i pristupačniji, što olakšava razvoj, testiranje i lokalno postavljanje razvojnog okruženja. Također, MySQL nudi dovoljnu funkcionalnost za potrebe bibliotečkog informacionog sistema, a migracija na ovoj fazi projekta (prije implementacije kompleksnijih funkcionalnosti) minimizira rizik od problema pri prelasku.  

**Posljedice odluke:**
- Potrebna migracija postojeće sheme baze podataka i migracionih skripti  
- Prilagodba konfiguracije projekta za MySQL konekciju  
- Provjera kompatibilnosti postojećih upita i Entity Framework konfiguracije  
- Moguće manje razlike u ponašanju pojedinih tipova podataka između PostgreSQL i MySQL  

**Status:** 
Aktivna  

**Povezani PB:**
PB-13  