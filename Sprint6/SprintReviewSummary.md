# Sprint Review Summary - Sprint 6

## Sprint broj

Sprint 6

## Planirani sprint goal

Cilj Sprinta 6 bio je nastaviti kontinuirani razvoj SmartLib bibliotečkog informacionog sistema kroz proširenje funkcionalnosti za upravljanje bibliotečkim fondom, testiranje relevantnih funkcionalnosti i evidentiranje tehničkih odluka i korištenja AI alata.

Fokus sprinta bio je na funkcionalnostima vezanim za knjige, kategorije, fizičke primjerke knjiga, validaciju poslovnih pravila i pripremu sistema za naredne funkcionalnosti kao što su rezervacije, zaduženja i naprednije upravljanje članovima biblioteke.

Također, jedan od ciljeva Sprinta 6 bio je nastaviti rad u skladu sa odlukom iz Sprinta 5 da se projekat dalje razvija koristeći MySQL bazu podataka.

## Šta je završeno

U Sprintu 6 završene su ili značajno unaprijeđene sljedeće aktivnosti:

- Implementirano dodavanje nove knjige u sistem.
- Implementirano uređivanje postojećih podataka o knjizi.
- Implementiran pregled kataloga knjiga.
- Implementirana osnovna paginacija kataloga.
- Implementirano povezivanje knjiga sa kategorijama.
- Implementirano upravljanje kategorijama knjiga.
- Implementirano upravljanje fizičkim primjercima knjiga.
- Implementirana osnovna zaštita podataka prilikom brisanja ili deaktivacije resursa.
- Ažuriran Sprint Backlog.
- Ažuriran AI Usage Log.
- Ažuriran Decision Log.
- Pripremljen dokaz o testiranju relevantnih funkcionalnosti.
- Nastavljen razvoj u skladu sa odlukom iz Sprinta 5 o prelasku sa PostgreSQL na MySQL.

## Šta nije završeno

Nisu u potpunosti završene sljedeće aktivnosti:



Ove stavke se prenose kao tehnički dug i ulazne aktivnosti za naredni sprint.

## Demonstrirane funkcionalnosti ili artefakti

Za Sprint Review pripremljene su sljedeće funkcionalnosti i artefakti:

- Forma za dodavanje nove knjige.
- Validacija podataka prilikom unosa knjige.
- Prikaz knjiga u katalogu.
- Uređivanje postojećih knjiga.
- Upravljanje kategorijama knjiga.
- Upravljanje fizičkim primjercima knjige.
- Osnovna zaštita od nekonzistentnog brisanja povezanih podataka.
- Ažurirani Sprint Backlog.
- Ažurirani Decision Log.
- Ažurirani AI Usage Log.
- Dokument sa dokazom o testiranju.
- Sprint Retrospective Summary.

## Glavni problemi i blokeri

Glavni problemi tokom Sprinta 6 bili su:

- Preostala neusklađenost između ranije dokumentacije koja je spominjala PostgreSQL i odluke iz Sprinta 5 da se projekat prebaci na MySQL.
- Potreba da se pažljivo validira brisanje knjiga, kategorija i primjeraka kako ne bi došlo do narušavanja povezanih podataka.
- Rizik da AI-generisani kod uvede funkcionalnosti izvan planiranog obima sprinta.
- Potreba za dodatnim ručnim pregledom testova i validacijskih pravila.
- Ograničeno vrijeme za kompletno refaktorisanje i vizuelno usklađivanje svih stranica.

## Ključne odluke donesene u sprintu

Ključne odluke i potvrde u Sprintu 6 bile su:

- Nastaviti razvoj u skladu sa MySQL bazom podataka.
- Preostale reference na PostgreSQL u dokumentaciji ili konfiguraciji tretirati kao tehnički dug.
- Za brisanje knjiga, kategorija i primjeraka koristiti oprezniji pristup koji čuva integritet podataka.
- Kategorije koje su povezane sa knjigama ne smiju se brisati bez provjere povezanih zapisa.
- AI alati se koriste kao pomoć pri implementaciji i dokumentaciji, ali tim ručno pregleda i prilagođava generisane prijedloge.
- Testiranje ostaje obavezni dio svakog novog inkrementa.

## Povratna informacija Product Ownera


Ovaj dio će biti dopunjen nakon sastanka, na osnovu komentara Product Ownera o demonstriranim funkcionalnostima, prioritetima za naredni sprint i eventualnim promjenama u Product Backlogu.

## Zaključak za naredni sprint

Sprint 6 je isporučio značajan funkcionalni napredak u radu sa bibliotečkim fondom. Sistem sada ima konkretnu osnovu za upravljanje knjigama, kategorijama i fizičkim primjercima knjiga.

U narednom sprintu fokus treba biti na:

- dopuni Product Owner feedback sekcije nakon Sprint Review sastanka,
- usklađivanju dokumentacije sa MySQL odlukom iz Sprinta 5,
- proširenju funkcionalnosti vezanih za rezervacije i zaduženja,
- jačanju testne pokrivenosti,
- refaktorisanju tehničkog duga,
- poboljšanju korisničkog interfejsa.
