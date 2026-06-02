# Sprint Review – Sprint 10

## Datum održavanja
02.06.2026.

## Učesnici
Tim (svi članovi grupe 5), Product Owner - asistent

---

## Cilj sprinta

Cilj Sprinta 10 bio je unapređenje komunikacije između sistema i korisnika, te proširenje administrativnih funkcionalnosti unutar SmartLib sistema. Fokus sprinta bio je na automatizaciji obavještavanja članova o rokovima vraćanja knjiga, osiguravanju transparentnosti promjena kroz audit log, uvođenju sistema kazni za kasno vraćanje knjiga, te omogućavanju online upravljanja članarinom i integracije sa distributerima knjiga.

Pored navedenog, implementirana je i funkcionalnost lista kolekcija koja unapređuje korisničko iskustvo kroz personalizovano organizovanje knjiga.

---

## Pregled realizovanih stavki

| ID | Naziv stavke | Status | Komentar |
|:--:|:------------:|:------:|:--------:|
| PB-41 | Slanje email upozorenja | Završeno | Automatski podsjetnici i upozorenja članovima o rokovima vraćanja i kašnjenjima |
| PB-42 | Obavještavanje bibliotekara o novoj rezervaciji | Završeno | Sistem automatski šalje email bibliotekaru pri kreiranju svake nove rezervacije |
| PB-46 | Audit log promjena | Završeno | Evidentirane sve promjene knjiga i korisničkih naloga sa detaljima akcije, vremena i korisnika |
| PB-47 | Kazne za kasno vraćanje knjiga | Završeno | Onemogućeno kreiranje novih zaduženja i rezervacija za članove sa zakašnjelim zaduženjima |
| PB-48 | Online produžetak članarine | Završeno | Član može podnijeti zahtjev za produženje, a bibliotekar ga pregledava i odobrava ili odbija |
| PB-49 | Integracija sa distributerom knjiga | Završeno | Bibliotekar može kreirati i poslati zahtjev za nabavku knjige direktno distributeru putem emaila |
| PB-50 | Lista kolekcija člana | Završeno | Član može kreirati kolekcije, dodavati knjige u iste te ih pretraživati i filtrirati |

---

## Demonstracija inkrementa

Tokom sprint review-a demonstriran je funkcionalan inkrement koji uključuje:

- **Slanje email upozorenja** – Sistem automatski šalje podsjetnike članovima 2 dana prije isteka roka vraćanja, upozorenja na dan isteka te podsjetnike o kašnjenju. Podsjetnici se zaustavljaju po povratku knjige i šalju se isključivo za aktivna zaduženja članova sa evidentiranom email adresom.
- **Obavještavanje bibliotekara o novoj rezervaciji** – Sistem automatski šalje email bibliotekaru svaki put kada član kreira rezervaciju, uz podatke o članu, rezervisanoj knjizi i datumu kreiranja. Obavijest se šalje samo jednom po rezervaciji.
- **Audit log promjena** – Administratori mogu pristupiti stranici Audit Log putem sidebara, pregledati sve zapise sortirane od najnovijeg prema najstarijem, filtrirati ih po entitetu, vrsti akcije i datumskom rasponu, te otvoriti detalje pojedinog zapisa s prikazom stanja entiteta prije i nakon izmjene.
- **Kazne za kasno vraćanje knjiga** – Sistem sprječava kreiranje novih zaduženja i rezervacija za članove koji imaju jedno ili više zakašnjelih nevraćenih zaduženja, uz odgovarajuće poruke upozorenja. Po povratku knjige, pristup se automatski vraća.
- **Online produžetak članarine** – Članovi mogu podnijeti zahtjev za produženje članarine odabirom trajanja (1, 3, 6 ili 12 mjeseci), a bibliotekari mogu pregledati sve zahtjeve na čekanju te ih odobriti ili odbiti uz razlog. Član može pratiti status i historiju zahtjeva na svom profilu.
- **Integracija sa distributerom knjiga** – Bibliotekari mogu popuniti formu za nabavku knjige (naziv, autor, izdavač, broj primjeraka) i direktno iz sistema poslati zahtjev distributeru putem emaila, uz potvrdu o uspješnom slanju.
- **Lista kolekcija člana** – Članovi mogu kreirati vlastite kolekcije knjiga (javne ili privatne), pretraživati ih i sortirati, te dodavati knjige u kolekcije direktno sa stranice detalja knjige.

---

## Povratne informacije product ownera

Product Owner je ocijenio da je sprint uspješno realizovan.

Ključne povratne informacije:
- **Kvalitet implementacije** – Sve demonstrirane funkcionalnosti su stabilne i bez uočenih grešaka.
- **Bez dodatnih pitanja i kritika** – Product Owner nije imao primjedbi ni otvorenih pitanja tokom demonstracije.

---

## Naredni koraci

- Izrada korisničke i tehničke dokumentacije
- Izrada liste poznatih ograničenja sistema

---

## Zaključak

Sprint 10 je uspješno završen uz realizaciju svih sedam planiranih backlog stavki i postignutih 100% sprint bodova. Isporučen je stabilan inkrement koji značajno unapređuje SmartLib sistem u područjima automatske komunikacije, administrativnog nadzora, upravljanja članarinom i personalizovanog korisničkog iskustva.