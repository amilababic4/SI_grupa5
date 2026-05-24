# Sprint Review – Sprint 8

## Datum održavanja
20.05.2026.

## Učesnici
Tim (svi članovi grupe 5), Product Owner - asistent

---

## Cilj sprinta

Cilj Sprinta 8 bio je unaprijediti upravljanje korisnicima unutar bibliotečkog sistema kroz implementaciju funkcionalnosti vezanih za profile članova, administraciju korisničkih naloga i upravljanje članarinama. Fokus sprinta bio je na omogućavanju administratorima i bibliotekarima efikasnog upravljanja korisnicima, dok je članovima omogućen jasan i pregledan uvid u vlastite podatke, status članarine i historiju aktivnosti.

Pored implementacije novih funkcionalnosti, cilj sprinta bio je unaprijediti sigurnost sistema kroz kontrolu pristupa, upravljanje lozinkama te zaštitu administratorskih naloga, uz održavanje kvaliteta koda i dokumentacije.

---

## Pregled realizovanih stavki

| ID | Naziv stavke | Status | Komentar |
|:--:|:------------:|:------:|:--------:|
| PB-20 | Pregled profila člana | Završeno | Prikaz osnovnih podataka korisnika i aktivnih zaduženja |
| PB-25 | Evidencija vraćanja knjiga | Završeno | Bibliotekari evidentiraju vraćanje knjiga i ažuriraju status primjeraka |
| PB-37 | Pregled historije zaduženja | Završeno | Omogućen pregled ranijih i zatvorenih zaduženja članova |
| PB-32 | Upravljanje korisnicima od strane admina | Završeno | Admin upravlja korisnicima, ulogama i deaktivacijom naloga |
| PB-33 | Upravljanje statusom članarine | Završeno | Bibliotekar evidentira i ažurira članarine članova |
| PB-34 | Pregled statusa članarine za člana | Završeno | Članovi vide status i datum isteka članarine |
| PB-56 | Upravljanje lozinkom korisničkog naloga | Završeno | Implementiran reset i promjena lozinke korisnika |

---

## Demonstracija inkrementa

Tokom sprint review-a demonstriran je funkcionalan inkrement koji uključuje:

- **Pregled profila člana** – Korisnici mogu pregledati osnovne informacije o svom profilu, uključujući ulogu, status i aktivna zaduženja. Bibliotekari imaju dodatni uvid u aktivna i historijska zaduženja članova.
- **Evidencija vraćanja knjiga** – Bibliotekari mogu označiti zaduženje kao vraćeno, čime se ažurira status primjerka i dostupnost knjige u sistemu.
- **Pregled historije zaduženja** – Omogućen je pregled svih ranijih zaduženja sa datumima zaduženja i vraćanja, uz mogućnost filtriranja po članu.
- **Upravljanje korisnicima** – Administratori mogu pregledati korisnike, mijenjati uloge, deaktivirati naloge i pretraživati korisnike po imenu ili emailu.
- **Zaštita administratorskog naloga** – Sistem sprječava deaktivaciju trenutno prijavljenog admin naloga.
- **Upravljanje članarinama** – Bibliotekari mogu evidentirati, ažurirati i produžavati članarine, dok članovi imaju pregled statusa i datuma isteka.
- **Upravljanje lozinkama** – Korisnici mogu resetovati zaboravljene lozinke putem emaila i sigurno mijenjati postojeću lozinku.

---

## Povratne informacije product ownera

Product owner je ocijenio da je sprint uspješno realizovan i da su sve planirane funkcionalnosti implementirane u skladu sa definisanim acceptance kriterijima.

Ključne povratne informacije:
- **Kvalitet implementacije** – Funkcionalnosti su stabilne i bez uočenih grešaka tokom demonstracije.
- **Pokrivenost funkcionalnosti** – Sistem sada ima zaokružen modul upravljanja korisnicima i članarinama.
- **Organizacija rada** – Backlog i dokumentacija su i dalje jasno strukturirani i uredni.
- **Velocity feedback** – Product Owner je napomenuo da bi u narednim sprintovima trebalo povećati obim isporučenih funkcionalnosti (velocity), uz zadržavanje kvaliteta. Sprint je ipak uspješno završen sa 100% realizovanih bodova.

---

## Dogovoreni naredni koraci

- Povećati obim funkcionalnosti u narednim sprintovima (veći velocity)
- Dalje unapređenje korisničkog interfejsa i UX-a
- Rad na naprednijim izvještajima i analitici sistema
- Optimizacija i dodatno poboljšanje sigurnosnih aspekata
- Nastavak održavanja AI Usage Log i Decision Log dokumentacije

---

## Zaključak

Sprint 8 je uspješno završen uz realizaciju svih planiranih backlog stavki i postignutih 100% sprint bodova. Isporučen je stabilan inkrement koji značajno unapređuje upravljanje korisnicima, članarinama i sigurnosnim aspektima sistema.

Na osnovu pozitivnih povratnih informacija Product Ownera i sugestije za povećanje velocity-ja, tim nastavlja razvoj sistema kroz funkcionalnosti planirane za naredni sprint.