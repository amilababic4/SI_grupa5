# Sprint Retrospective Summary

## Sprint informacije
- **Naziv sprinta:** Sprint 8  
- **Trajanje sprinta:** 1 sedmica  
- **Datum retrospektive:** 19.05.2026.  
- **Tema projekta:** Bibliotečki informacioni sistem  

---

## Cilj retrospektive
Cilj retrospektive bio je analizirati realizaciju osmog sprinta, identifikovati pozitivne aspekte rada tima, probleme koji su se pojavili tokom implementacije funkcionalnosti vezanih za upravljanje korisnicima, članarinama i sigurnošću naloga, te definisati prijedloge za unapređenje procesa rada u narednim fazama razvoja sistema.

---

## Šta je prošlo dobro

- Tim je uspješno realizovao sve planirane funkcionalnosti predviđene za Sprint 8.
- Implementirane su funkcionalnosti pregleda korisničkih profila, aktivnih i historijskih zaduženja članova biblioteke.
- Uspješno je implementirana evidencija vraćanja knjiga uz automatsko ažuriranje statusa primjeraka.
- Administratorima je omogućeno upravljanje korisnicima, uključujući promjenu uloga i deaktivaciju naloga.
- Implementirana je zaštita od deaktivacije vlastitog administratorskog naloga.
- Dodane su funkcionalnosti upravljanja članarinama i prikaza statusa članarine za članove biblioteke.
- Implementirana je funkcionalnost resetovanja zaboravljene lozinke putem emaila, kao i sigurna promjena postojeće lozinke korisnika.
- Integracija novih funkcionalnosti sa postojećim autentifikacijskim sistemom prošla je bez većih problema.
- Dokumentacija projekta i backlog stavke redovno su ažurirani tokom sprinta.
- Komunikacija između članova tima bila je efikasna, što je omogućilo brzu integraciju frontend i backend dijelova sistema.
- Testiranje implementiranih funkcionalnosti nije otkrilo kritične greške.

---

## Problemi i izazovi

- Integracija email servisa za resetovanje lozinke zahtijevala je dodatnu konfiguraciju i validaciju.
- Pojedine validacije pri evidentiranju i ažuriranju članarina morale su biti dodatno prilagođene zbog edge-case scenarija sa datumima.
- Implementacija deaktivacije korisničkih naloga zahtijevala je dodatne provjere kako bi se spriječilo zaključavanje administratorskog pristupa sistemu.
- Prikaz većeg broja historijskih zaduženja zahtijevao je dodatne prilagodbe korisničkog interfejsa radi bolje preglednosti podataka.

---

## Šta smo naučili

- Funkcionalnosti vezane za autentifikaciju i korisničke naloge zahtijevaju dodatnu pažnju pri validaciji i sigurnosti.
- Edge-case scenariji kod rada sa datumima mogu uzrokovati probleme ako nisu na vrijeme testirani.
- Redovna komunikacija između članova tima ubrzava integraciju novih funkcionalnosti.

---

## Akcije za naredni sprint

| Akcija | Odgovornost | Prioritet |
|---|---|---|
| Dodati email notifikacije za istek članarine | Backend tim | Srednji |
| Unaprijediti prikaz historije zaduženja za veći broj zapisa | Frontend tim | Srednji |
| Nastaviti proširivanje automatizovanih testova za autentifikaciju i korisničke profile | Cijeli tim | Visok |
| Poboljšati korisničko iskustvo formi za upravljanje članarinama i lozinkama | Frontend tim | Srednji |
| Nastaviti redovno ažuriranje dokumentacije | Cijeli tim | Srednji |
---

## Zaključak

Sprint 8 uspješno je realizovan sa svim planiranim funkcionalnostima implementiranim u predviđenom vremenskom okviru. Sistem je unaprijeđen kroz funkcionalnosti upravljanja korisnicima, pregled profila članova, evidenciju članarina i sigurnije upravljanje korisničkim nalozima. Posebno značajan napredak ostvaren je implementacijom resetovanja i promjene lozinke, čime je povećana sigurnost sistema i olakšano upravljanje pristupom korisničkim nalozima. Identifikovani problemi bili su manjeg obima i riješeni su tokom razvoja sprinta, dok je tim nastavio pokazivati visok nivo saradnje, organizacije i kvaliteta implementacije.