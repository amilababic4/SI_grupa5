# Sprint Review – Sprint 7

## Datum održavanja
14.05.2026.

## Učesnici
Tim (svi članovi grupe 5), asistent

---

## Cilj sprinta

Cilj Sprinta 7 bio je unaprijediti funkcionalnosti bibliotečkog sistema kroz implementaciju pretrage knjiga, prikaza detalja knjiga, pregleda dostupnosti primjeraka, te sistema za evidenciju i pregled aktivnih zaduženja. Fokus sprinta bio je na poboljšanju korisničkog iskustva, efikasnijem upravljanju zaduženjima i osiguravanju tačnosti podataka o dostupnosti knjiga.

Pored implementacije novih funkcionalnosti, cilj sprinta bio je održati visok nivo kvaliteta dokumentacije i organizacije rada kroz jasno definisan sprint backlog, acceptance kriterije i kontinuirano ažuriranje AI Usage Log i Decision Log dokumentacije.

---

## Pregled realizovanih stavki

| ID | Naziv stavke | Status | Komentar |
|:--:|:------------:|:------:|:--------:|
| PB-29 | Pretraga knjiga | Završeno | Implementirana pretraga po naslovu i autoru uz filtriranje i reset rezultata |
| PB-24 | Prikaz detalja knjige | Završeno | Omogućen pregled detaljnih bibliografskih podataka knjige |
| PB-30 | Pregled dostupnosti knjige | Završeno | Implementiran prikaz dostupnosti i broja slobodnih primjeraka |
| PB-25 | Evidencija zaduživanja i vraćanja knjiga | Završeno | Implementiran kompletan proces zaduživanja i vraćanja knjiga |
| PB-35 | Pregled vlastitih zaduženja | Završeno | Članovima omogućen pregled aktivnih zaduženja i rokova vraćanja |
| PB-36 | Pregled trenutnih zaduženja | Završeno | Bibliotekarima omogućen pregled i filtriranje aktivnih zaduženja |

---

## Demonstracija inkrementa

Tokom sprint review-a demonstriran je funkcionalan inkrement koji uključuje:

- **Pretragu knjiga** – Korisnicima je omogućena pretraga knjiga po naslovu i autoru uz filtriranje rezultata i reset pretrage. Pretraga nije osjetljiva na velika i mala slova, a sistem prikazuje odgovarajuće poruke ukoliko nema rezultata.
- **Prikaz detalja knjige** – Implementiran je detaljan prikaz bibliografskih podataka knjige, uključujući osnovne informacije i pregled dostupnosti primjeraka.
- **Pregled dostupnosti knjiga** – Sistem prikazuje status dostupnosti svakog primjerka knjige i ukupan broj slobodnih primjeraka u realnom vremenu.
- **Evidencija zaduživanja i vraćanja knjiga** – Bibliotekar može evidentirati nova zaduženja i vraćanja knjiga kroz odgovarajuće forme i pregled aktivnih zaduženja. Implementirana je zaštita od duplog zaduživanja istog primjerka.
- **Automatsko postavljanje roka vraćanja** – Sistem automatski postavlja rok vraćanja ukoliko bibliotekar ručno ne definiše datum povrata.
- **Pregled vlastitih zaduženja** – Članovima biblioteke omogućen je pregled svih aktivnih zaduženja sa jasno prikazanim rokovima vraćanja i vizualnim označavanjem zakašnjelih zaduženja.
- **Pregled aktivnih zaduženja za bibliotekare** – Bibliotekar može pregledati sva aktivna zaduženja, filtrirati ih po članu te pregledati detalje pojedinačnih zaduženja. Lista zaduženja sortirana je po roku vraćanja.

---

## Povratne informacije product ownera

Product owner je ocijenio da je sprint veoma uspješno realizovan i da su sve planirane funkcionalnosti implementirane u skladu sa definisanim acceptance kriterijima.

Ključne povratne informacije:
- **Kvalitet implementacije** – Sve demonstrirane funkcionalnosti rade stabilno i bez uočenih problema tokom demonstracije.
- **Organizacija sprint backloga** – Posebno je pohvaljena preglednost, detaljnost i obim sprint backloga, kao i jasno definisani acceptance kriteriji.
- **Kvalitet rada tima** – Istaknuto je da organizacija rada i kvalitet realizacije mogu poslužiti kao primjer drugim timovima.
- **Korisničko iskustvo** – Funkcionalnosti su intuitivne i omogućavaju jednostavno upravljanje katalogom i zaduženjima.

Tokom prezentacije nisu evidentirane značajne zamjerke niti otvoreni problemi vezani za implementirane funkcionalnosti.

---

## Dogovoreni naredni koraci

- Implementacija dodatnih funkcionalnosti vezanih za historiju zaduženja
- Razvoj sistema obavijesti i upozorenja za rokove vraćanja
- Daljnje unapređenje korisničkog interfejsa i validacije podataka
- Nastavak održavanja AI Usage Log i Decision Log dokumentacije

---

## Zaključak

Sprint 7 je uspješno završen. Sve planirane backlog stavke su realizovane i demonstrirane bez većih problema. Tim je ostvario 100% planiranih sprint bodova i implementirao ključne funkcionalnosti vezane za pretragu knjiga, pregled dostupnosti i upravljanje zaduženjima.

Na osnovu veoma pozitivnih povratnih informacija product ownera i uspješno demonstriranog inkrementa, tim nastavlja razvoj sistema kroz funkcionalnosti planirane za naredni sprint.
