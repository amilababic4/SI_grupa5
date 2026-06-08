# Release Notes — Bibliotečki informacioni sistem

**Verzija:** 1.0.0 — Finalna isporuka  
**Datum:** Juni 2026  
**Pokriva sprintove:** Sprint 1 – Sprint 11  

---

## 1. Pregled isporuke

Ova verzija predstavlja finalnu isporuku projekta **Bibliotečki informacioni sistem**. Sistem pokriva osnovne procese bibliotečkog poslovanja: upravljanje katalogom knjiga, evidenciju zaduživanja i vraćanja, upravljanje članovima i njihovim članarinama, sistem rezervacija, te administrativne i analitičke funkcionalnosti.

---

## 2. Šta je uključeno u finalnu verziju

Sve stavke označene statusom **Done** u Product Backlogu smatraju se isporučenim u okviru ove verzije. U nastavku su navedene po kategorijama.

### 2.1 Autentifikacija i upravljanje korisnicima

| ID | Funkcionalnost |
|:--:|:--------------|
| PB-17 | Sistem prijave i odjave korisnika prema ulozi (član, bibliotekar, administrator) |
| PB-18 | Kreiranje naloga novog člana od strane bibliotekara ili administratora |
| PB-56 | Reset zaboravljene lozinke putem emaila i promjena postojeće lozinke |
| PB-32 | Upravljanje korisnicima od strane admina (pregled, promjena uloge, deaktivacija naloga) |

### 2.2 Katalog knjiga

| ID | Funkcionalnost |
|:--:|:--------------|
| PB-22 | Dodavanje nove knjige u fond |
| PB-23 | Uređivanje podataka o knjizi |
| PB-24 | Prikaz detalja knjige |
| PB-25 | Upravljanje kategorijama knjiga |
| PB-26 | Upravljanje primjercima knjige i praćenje njihovog statusa |
| PB-27 | Brisanje knjige i deaktivacija primjerka |
| PB-28 | Pregled kataloga dostupnih knjiga |
| PB-29 | Pretraga knjiga po naslovu, autoru ili ključnoj riječi |
| PB-30 | Pregled dostupnosti knjige i broja primjeraka |
| PB-44 | Napredna pretraga i filtriranje po žanru, godini, izdavaču |

### 2.3 Zaduživanje i rezervacije

| ID | Funkcionalnost |
|:--:|:--------------|
| PB-31 | Evidencija zaduživanja i vraćanja knjiga |
| PB-35 | Pregled vlastitih zaduženja (za člana) |
| PB-36 | Pregled trenutnih zaduženja (za bibliotekara) |
| PB-37 | Pregled historije zaduženja člana |
| PB-39 | Rezervacija nedostupne knjige |
| PB-40 | Pregled aktivnih rezervacija (za bibliotekara) |
| PB-43 | Automatsko otkazivanje rezervacije ako član ne preuzme knjigu u zadanom roku |

### 2.4 Upravljanje članovima i članarinom

| ID | Funkcionalnost |
|:--:|:--------------|
| PB-20 | Pregled profila člana sa zaduženjima |
| PB-21 | Pregled i pretraga članova biblioteke |
| PB-33 | Upravljanje statusom članarine (pregled, ažuriranje, evidencija) |
| PB-34 | Pregled statusa i datuma isteka članarine (za člana) |
| PB-48 | Online produžetak članarine putem sistema |

### 2.5 Notifikacije i kazne

| ID | Funkcionalnost |
|:--:|:--------------|
| PB-41 | Automatsko slanje email upozorenja za istek roka vraćanja |
| PB-42 | Email notifikacija bibliotekara o novoj rezervaciji |
| PB-47 | Evidencija i obračun kazni za prekoračenje roka vraćanja |

### 2.6 Izvještaji i administracija

| ID | Funkcionalnost |
|:--:|:--------------|
| PB-45 | Mjesečni izvještaji za upravu (statistike zaduženja, rezervacija i članstva) |
| PB-46 | Audit log važnih promjena u sistemu |
| PB-49 | Integracija sa distributerom knjiga (vanjski servis za nabavku) |

### 2.7 Korisnički interfejs i zajednica

| ID | Funkcionalnost |
|:--:|:--------------|
| PB-56 | Unapređenje korisničkog interfejsa — responzivnost, navigacija, vizuelni stil |
| PB-57 | Modul vijesti i novosti — javna stranica sa bibliotečkim obavještenjima |
| PB-58 | Kalendar događaja — prikaz planiranih bibliotečkih aktivnosti |
| PB-59 | Forum zajednice — diskusije i razmjena preporuka između članova |

---

## 3. Najvažnije funkcionalnosti

Sljedeće funkcionalnosti predstavljaju srž sistema i smatraju se kritičnim isporukama:

1. **Sistem autentifikacije s rolama** — prijava i odvojena prava pristupa za tri tipa korisnika (član, bibliotekar, administrator).
2. **Katalog i pretraga knjiga** — potpuno funkcionalan katalog s naprednom pretragom i filterima.
3. **Evidencija zaduživanja i vraćanja** — centralna bibliotečka funkcionalnost s praćenjem historije.
4. **Sistem rezervacija s automatskim otkazivanjem** — rezervacija nedostupnih knjiga uz CRON-bazirano automatsko upravljanje.
5. **Upravljanje članarinom** — evidencija statusa, online produžetak i pregled za člana i bibliotekara.
6. **Notifikacijski sistem** — automatski email podsjetnici za isteke i nove rezervacije.
7. **Kazne za kasno vraćanje** — automatski obračun i evidencija kazni.
8. **Audit log** — praćenje svih važnih promjena u sistemu.
9. **Izvještaji za upravu** — mjesečna statistika poslovanja.

---

## 4. Poznata ograničenja

Sljedeća ograničenja su identifikovana i svjesno prihvaćena u okviru trenutne verzije:

- **Nema historije promjena podataka o knjizi** — izmjene podataka o knjizi (PB-23) ne čuvaju historiju prethodnih vrijednosti.
- **Samostalna registracija člana nije podržana** — nalog može kreirati isključivo bibliotekar ili administrator (PB-18); ne postoji javna forma za registraciju.
- **Napredna pretraga ograničena na definisane filtere** — nije moguće kombinovanje proizvoljnih parametara izvan ponuđenih (žanr, godina, izdavač).
- **Integracija s distributerom knjiga** (PB-49) — implementirana bazična integracija; nije obuhvaćena automatizacija narudžbi ni sinhronizacija inventara u realnom vremenu.
- **Online plaćanje** — produžetak članarine (PB-48) je implementiran, ali ne uključuje integraciju s eksternim platnim sistemom; plaćanje se evidentira ručno.
- **Email notifikacije** — sistem oslanja na konfigurisan SMTP server; bez odgovarajuće konfiguracije notifikacije neće biti dostavljene.
- **Mobilna responzivnost** — interfejs je poboljšan (PB-56), ali nije u potpunosti optimizovan za sve veličine ekrana.

---

## 5. Poznati bugovi

U trenutku finalne isporuke nisu evidentirani kritični bugovi koji blokiraju rad sistema. Međutim, tokom faze stabilizacije (PB-50) identifikovani su sljedeći manji problemi:

- **Automatsko otkazivanje rezervacije** — u rijetkim slučajevima CRON job ne okida na tačno predviđeno vrijeme u zavisnosti od opterećenja servera; funkcionalno ispravno, ali sa mogućim kašnjenjem od nekoliko minuta.
- **Prikaz statusa primjerka** — nakon evidencije vraćanja, status primjerka se u nekim slučajevima ne osvježava odmah na stranici kataloga bez ručnog osvježavanja stranice.
- **Pretraga s posebnim znakovima** — pretraga pojmova koji sadrže određene specijalne znakove (npr. spojnice, navodnici) može vratiti nepotpune rezultate.

---

## 6. Šta nije dio finalne isporuke

Sljedeće stavke su bile planirane, ali **nisu implementirane** u finalnoj verziji sistema:

> Napomena: Nijedna stavka iz Product Backloga nije ostala u statusu "Not Done" — sve planirane stavke su markirane kao Done. Međutim, određene funkcionalnosti isporučene su u **ograničenom obimu** u odnosu na originalnu viziju:

| Stavka | Razlog izostavljanja / ograničenja |
|:-------|:----------------------------------|
| Integracija s eksternim platnim sistemom | Zahtijeva eksternu infrastrukturu (payment gateway) van opsega projekta |
| Automatizirana nabavka knjiga putem distributera | Implementirana samo bazična integracija; automatizacija narudžbi nije uključena |
| Historija promjena podataka knjige | Svjesno izostavljeno kao niži prioritet (navedeno u napomeni PB-23) |
| Samostalna registracija člana | Dizajnom odlučeno da registraciju vrši bibliotekar, ne član samostalno |
| Potpuna mobilna optimizacija | Djelimično urađeno u PB-56; detaljna optimizacija odgođena za budući rad |

---

## 7. Tehnički dug i preporuke za budući razvoj

- Uvesti historiju promjena za podatke o knjigama i korisnicima.
- Implementirati pravu integraciju platnog sistema za online plaćanje članarine.
- Proširiti notifikacijski sistem na in-app notifikacije (pored emaila).
- Poboljšati mobilnu responzivnost za sve ključne stranice.
- Razmotriti uvođenje role "moderatora" za upravljanje forumom zajednice (PB-59).
- Optimizovati pouzdanost CRON joba za automatsko otkazivanje rezervacija.

---