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


## DL-07
**Datum**: 11.05.2026  
**Naziv odluke:** Premještanje Product Backlog itema PB-35 i PB-36 iz planiranih budućih sprintova u Sprint 7 zbog njihove funkcionalne povezanosti sa modulom zaduživanja i optimizacije razvoja sistema.

**Opis problema:**
Tokom razvoja funkcionalnosti vezanih za zaduživanje knjiga u Sprintu 7 uočeno je da su Product Backlog itemi PB-35 i PB-36 direktno povezani sa implementacijom evidencije zaduživanja. Ostavljenje ovih funkcionalnosti za Sprint 8 i Sprint 9 dovelo bi do dodatnog prilagođavanja postojećeg koda i složenije integracije.

**Razmatrane opcije:**
- Ostaviti PB-35 i PB-36 u Sprintu 8
- Premjestiti sve povezane backlog iteme u Sprint 7  

**Odabrana opcija:**
Premještanje PB-35 i PB-36 u Sprint 7.  

**Razlog izbora:**
Na ovaj način omogućena je implementacija svih funkcionalnosti vezanih za zaduživanje unutar iste razvojne cjeline, čime je smanjena kompleksnost razvoja i olakšano testiranje sistema.  

**Posljedice odluke:**
- Povećan obim posla u Sprintu 7  
- Jednostavnija integracija funkcionalnosti zaduživanja
- Lakše testiranje i održavanje modula
- Smanjena potreba za kasnijim izmjenama baze i logike sistema 

**Status:**
Aktivna  

**Povezani PB:**
PB-35, PB-36



## DL-08
**Datum**: 15.05.2026.  
**Naziv odluke:** Dodavanje funkcionalnosti promjene zaboravljene lozinke van inicijalno planiranog Product Backloga radi unapređenja sigurnosti i korisničkog iskustva sistema.

**Opis problema:**  
Tokom razvoja modula za upravljanje korisnicima uočeno je da sistem nema mogućnost oporavka korisničkog naloga u slučaju zaboravljene lozinke. Oslanjanje isključivo na administratorsku intervenciju predstavljalo bi ograničenje za korisnike i povećalo opterećenje administracije sistema.

**Razmatrane opcije:**  
- Ostaviti funkcionalnost za budući sprint  
- Omogućiti reset lozinke isključivo putem administratora  
- Dodati funkcionalnost promjene zaboravljene lozinke u okviru Sprinta 8  

**Odabrana opcija:**  
Dodavanje funkcionalnosti promjene zaboravljene lozinke u Sprint 8.

**Razlog izbora:**  
Funkcionalnost direktno unapređuje sigurnost i upotrebljivost sistema te smanjuje zavisnost korisnika od administratora prilikom pristupa nalogu. Također, njena implementacija je prirodno povezana sa postojećim funkcionalnostima upravljanja korisnicima i autentifikacije.

**Posljedice odluke:**  
- Proširen obim funkcionalnosti Sprinta 8  
- Poboljšano korisničko iskustvo i pristupačnost sistema  
- Smanjena potreba za administrativnim intervencijama  
- Unaprijeđena sigurnost i upravljanje korisničkim nalozima  

**Status:**  
Aktivna  

**Povezani PB:**  
PB-32


## DL-09
**Datum**: 20.05.2026.  
**Naziv odluke:** Uvođenje Upstash Redis distribuiranog keširanja za produkciju

**Opis problema:**  
Aplikacija je inicijalno koristila isključivo lokalno in-memory keširanje (`IMemoryCache`). U produkcijskom okruženju (Render) ovo dovodi do gubitka keširanih podataka prilikom svakog restarta kontejnera, što nepotrebno opterećuje eksterne API-je (Google Books, OpenLibrary, prevodilac) i može uzrokovati probijanje *rate limita*. Bilo je potrebno trajno i distribuirano rješenje za keširanje.

**Razmatrane opcije:**  
- Zadržavanje in-memory keširanja
- Pokretanje lokalnog Redis kontejnera uz aplikaciju
- Upotreba cloud-based rješenja (Upstash Redis) uz fallback na in-memory za lokalni razvoj

**Odabrana opcija:**  
Upotreba cloud-based Upstash Redis servisa za produkcijsko okruženje uz in-memory fallback za lokalni razvoj.

**Razlog izbora:**  
Upstash pruža *managed* Redis kojem se može pristupiti direktno putem sigurne TLS/TCP konekcije, eliminišući potrebu za konfigurisanjem i održavanjem lokalnog Redis kontejnera u infrastrukturi. Dodavanjem fallback mehanizma zadržali smo jednostavnost lokalnog razvoja (programeri ne moraju imati instaliran Redis da bi radili na projektu), dok produkcija dobija trajan i brz distribuirani keš ulančavanjem `IDistributedCache` i `StackExchange.Redis`.

**Posljedice odluke:**  
- Značajno poboljšane performanse i otpornost keša u produkciji
- Izbjegnuto probijanje *rate limita* prema eksternim API-jima
- Uvedena potreba za dodavanjem `UPSTASH_REDIS_REST_URL` i `UPSTASH_REDIS_REST_TOKEN` environment varijabli na produkcijskom hostingu
- Poboljšana i promijenjena arhitektura aplikacije (uveden `IDistributedCache`)

**Status:**  
Aktivna  

**Povezani PB:**  
Tehničko unaprjeđenje (Van okvira originalnog backloga)


## DL-10
**Datum:** 22.05.2026.  
**Naziv odluke:** Generisanje izvještaja direktno u PDF format umjesto HTML eksportovanja

**Opis problema:**  
Prilikom implementacije mjesečnih izvještaja bilo je potrebno odlučiti da li će se izvještaji prvo generisati kao HTML stranice sa mogućnošću štampe ili direktno kao PDF dokumenti.

**Razmatrane opcije:**  
- HTML prikaz sa browser print/export funkcionalnošću  
- Direktno generisanje PDF dokumenta putem backend biblioteke  

**Odabrana opcija:**  
Direktno generisanje PDF dokumenta putem backend biblioteke.

**Razlog izbora:**  
Direktno PDF generisanje omogućava konzistentan izgled izvještaja nezavisno od browsera i uređaja korisnika. Također, omogućava lakše preuzimanje i arhiviranje izvještaja unutar bibliotečkog sistema.

**Posljedice odluke:**  
- Konzistentan izgled izvještaja na svim uređajima  
- Potrebna dodatna implementacija stilizacije PDF dokumenata  
- Veća kontrola nad rasporedom elemenata i sadržaja izvještaja  
- Povećana kompleksnost implementacije grafikona i CSS stilova unutar PDF-a  

**Status:**  
Aktivna  

**Povezani PB:**  
PB-45