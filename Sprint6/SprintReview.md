# Sprint Review – Sprint 6

## Datum održavanja
07.05.2026.

## Učesnici
Tim (svi članovi grupe 5), asistent

---

## Cilj sprinta

Cilj Sprinta 6 bio je implementirati centralne funkcionalnosti za upravljanje bibliotečkim fondom, uključujući dodavanje, uređivanje i brisanje knjiga, upravljanje kategorijama i fizičkim primjercima knjiga, te pregled kataloga sa paginacijom. Fokus sprinta bio je na osiguravanju integriteta podataka i stabilnosti sistema kroz validaciju poslovne logike i testiranje implementiranih funkcionalnosti.

Pored razvoja funkcionalnosti, cilj sprinta bio je i nastavak transparentnog dokumentovanja razvoja projekta kroz AI Usage Log i Decision Log.

---

## Pregled realizovanih stavki

| ID | Naziv stavke | Status | Komentar |
|:--:|:------------:|:------:|:--------:|
| PB-22 | Dodavanje nove knjige | Završeno | Implementirana forma za unos knjiga sa validacijom ISBN-a i generisanjem primjeraka |
| PB-23 | Uređivanje podataka o knjizi | Završeno | Omogućeno ažuriranje podataka o knjigama uz validaciju unosa |
| PB-28 | Pregled kataloga | Završeno | Implementiran pregled kataloga sa paginacijom |
| PB-26 | Upravljanje primjercima knjige | Završeno | Implementirano upravljanje fizičkim primjercima i njihovim statusima |
| PB-27 | Brisanje knjige i deaktivacija primjerka | Završeno | Implementirana zaštita od brisanja zaduženih knjiga i primjeraka |
| PB-25 | Upravljanje kategorijama knjiga | Završeno | Omogućeno dodavanje, uređivanje i brisanje kategorija |

---

## Demonstracija inkrementa

Tokom sprint review-a, demonstriran je funkcionalan inkrement koji uključuje:

- **Dodavanje novih knjiga** – Bibliotekar ili administrator može unijeti novu knjigu kroz formu sa validacijom podataka. Sistem provjerava ispravnost ISBN-a, sprječava duplikate i automatski generiše odgovarajući broj primjeraka knjige.
- **Uređivanje knjiga** – Omogućeno je ažuriranje osnovnih podataka o knjigama, uključujući naslov, autora, godinu izdanja i kategoriju, uz očuvanje integriteta podataka u bazi.
- **Pregled kataloga knjiga** – Korisnici mogu pregledati katalog svih aktivnih knjiga kroz paginirani prikaz, čime je omogućeno efikasno pregledanje većeg broja zapisa.
- **Upravljanje primjercima knjiga** – Sistem omogućava pregled svih primjeraka jedne knjige, prikaz njihovih statusa (dostupan, posuđen i sl.) te deaktivaciju oštećenih ili izgubljenih primjeraka.
- **Brisanje knjiga i zaštita integriteta podataka** – Implementirana je logika koja sprječava brisanje knjige ukoliko postoji aktivno zaduženje. Sistem prije brisanja prikazuje potvrdu korisniku i automatski ažurira katalog nakon izvršene akcije.
- **Upravljanje kategorijama** – Bibliotekar ili administrator može dodavati, uređivati i brisati kategorije knjiga, uz zaštitu od brisanja kategorija koje su povezane sa postojećim knjigama.

---

## Povratne informacije asistenta

Asistent je ocijenio da je sprint uspješno realizovan i da su sve planirane stavke implementirane u skladu sa definisanim acceptance kriterijima iz User storija.

Ključne povratne informacije:
- **Kvalitet implementacije** – Sve demonstrirane funkcionalnosti rade stabilno i u skladu sa očekivanim ponašanjem sistema.
- **Korisnički interfejs** – Interfejs je pregledan i omogućava jednostavno upravljanje katalogom i bibliotečkim fondom.
- **Dokumentacija i organizacija rada** – AI Usage Log i Decision Log su uredno vođeni i redovno ažurirani tokom sprinta.

Tokom prezentacije nisu evidentirane značajne zamjerke niti otvoreni problemi vezani za implementirane funkcionalnosti.

---

## Dogovoreni naredni koraci

- Implementacija funkcionalnosti pretrage knjiga kroz katalog
- Razvoj sistema zaduživanja i vraćanja knjiga
- Daljnje unapređenje korisničkog interfejsa i validacije podataka
- Nastavak održavanja AI Usage Log i Decision Log dokumentacije

---

## Zaključak

Sprint 6 je uspješno završen. Sve planirane backlog stavke su realizovane i demonstrirane bez većih problema. Tim je implementirao ključne funkcionalnosti za upravljanje bibliotečkim fondom, uključujući katalog knjiga, upravljanje kategorijama i primjercima, kao i zaštitu integriteta podataka pri brisanju resursa. Na osnovu pozitivnih povratnih informacija asistenta, tim nastavlja razvoj sistema kroz funkcionalnosti planirane za Sprint 7.