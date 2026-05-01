# Sprint Review – Sprint 5

## Datum održavanja
29.04.2026.

## Učesnici
Tim (svi članovi grupe 5), asistent

---

## Cilj sprinta

Cilj Sprinta 5 bio je implementirati osnovne funkcionalnosti sistema autentifikacije i registracije članova biblioteke, uključujući kreiranje naloga, prijavu i odjavu korisnika, upravljanje korisničkom sesijom i zaštitu ruta aplikacije. Pored toga, cilj je bio uspostaviti dokumentacione logove (AI Usage Log i Decision Log) za transparentno praćenje razvoja projekta.

---

## Pregled realizovanih stavki

| ID | Naziv stavke | Status | Komentar |
|:--:|:------------:|:------:|:--------:|
| PB-17 | Sistem prijave korisnika | ✅ Završeno | Implementirana prijava, odjava, zaštita ruta i upravljanje sesijom |
| PB-18 | Kreiranje naloga člana | ✅ Završeno | Bibliotekar/administrator može kreirati nalog novog člana putem forme |
| PB-19 | AI i Decision Log | ✅ Završeno | Uspostavljeni logovi za praćenje korištenja AI alata i tehničkih odluka |

---

## Demonstracija inkrementa

Tokom sprint review-a timu je demonstriran funkcionalan inkrement koji uključuje:

- **Prijava korisnika** – Korisnici se mogu prijaviti unosom email adrese i lozinke. Sistem ispravno validira unesene podatke i preusmjerava korisnika na odgovarajući dashboard prema ulozi (član, bibliotekar, administrator).
- **Odjava korisnika** – Prijavljeni korisnici mogu se odjaviti putem opcije u navigaciji, nakon čega se sesija briše i pristup zaštićenim stranicama više nije moguć bez ponovne prijave.
- **Zaštita ruta** – Neprijavljeni korisnici su spriječeni da pristupe zaštićenim stranicama. Direktan unos URL-a ne zaobilazi autentifikaciju, a korisnik se automatski preusmjerava na stranicu za prijavu.
- **Kreiranje naloga člana** – Bibliotekar ili administrator može kreirati nalog novog člana unosom osnovnih podataka (ime, prezime, email, lozinka). Sistem provjerava ispravnost unesenih podataka i sprječava duplikate email adresa.
- **Upravljanje sesijom** – Nakon uspješne prijave, korisnička sesija se kreira i korisnik ostaje prijavljen tokom navigacije između stranica.

---

## Povratne informacije asistenta

Asistent je ocijenio da je sprint uspješno realizovan i da su sve planirane stavke završene u skladu sa definisanim acceptance kriterijima. Istaknuto je da je tim kvalitetno pristupio implementaciji autentifikacijskog sistema i da je dokumentacija uredno vođena.

Ključne povratne informacije:
- **Kvalitet implementacije** – Implementacija sistema prijave, odjave i zaštite ruta je zadovoljavajuća i funkcionalna. Sve funkcionalnosti rade u skladu sa očekivanjima.
- **Dokumentacija** – AI Usage Log i Decision Log su uspostavljeni i ispunjavaju svoju svrhu. Tim je transparentno dokumentovao korištenje AI alata i važne tehničke odluke.
- **Odluka o promjeni baze podataka** – Dogovoreno je sa asistentom da se izvrši prelazak sa PostgreSQL na MySQL bazu podataka. Ova odluka je donesena radi bolje usklađenosti sa razvojnim okruženjem tima i dostupnim alatima. Promjena će biti implementirana u narednom sprintu.

---

## Dogovoreni naredni koraci

- Prelazak sa PostgreSQL na MySQL bazu podataka (evidentirano u Decision Logu)
- Nastavak implementacije funkcionalnosti vezanih za rad sa knjigama i katalogom (Sprint 6)
- Nastavak poboljšanja korisničkog interfejsa

---

## Zaključak

Sprint 5 je uspješno završen. Sve tri planirane stavke (PB-17, PB-18, PB-19) su realizovane i zadovoljavaju Definition of Done kriterije. Tim je demonstrirao funkcionalan inkrement koji uključuje potpun sistem autentifikacije i registracije članova, te uspostavljene dokumentacione logove. Na osnovu pozitivnih povratnih informacija asistenta, tim nastavlja sa planiranim radom u Sprintu 6.
