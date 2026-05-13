# Sprint Retrospective Summary

## Sprint informacije
- **Naziv sprinta:** Sprint 7  
- **Trajanje sprinta:** 1 sedmica  
- **Datum retrospektive:** 13.05.2026.  
- **Tema projekta:** Bibliotečki informacioni sistem  

---

## Cilj retrospektive
Cilj retrospektive bio je analizirati realizaciju sedmog sprinta, identifikovati pozitivne aspekte rada tima, probleme koji su se pojavili tokom implementacije funkcionalnosti, te definisati prijedloge za unapređenje procesa rada u narednim fazama projekta.

---

## Šta je prošlo dobro

- Tim je uspješno realizovao sve planirane funkcionalnosti predviđene za Sprint 7.
- Implementirane su kritične funkcionalnosti: pretraga knjiga, prikaz detalja knjige, pregled dostupnosti, evidencija zaduživanja/vraćanja, te pregled zaduženja za članove i bibliotekare.
- Kvaliteta koda ostala je na visokom nivou, a integracija sa postojećim sistemom prošla je bez većih problema.
- Komunikacija među članovima tima bila je redovna i efikasna, što je omogućilo brzinu razvoja.
- Svi zadaci završeni su u planiranom vremenskom okviru.
- Dokumentacija projekta kontinuirano je ažurirana tokom razvoja.
- Testiranje implementiranih funkcionalnosti nije otkrilo kritične greške.
- Tim je pokazao odličnu saradnju pri integraciji različitih modula sistema.

---

## Problemi i izazovi

- Privremeni problemi sa API-jem koji dohvaća naslovne slike za knjige - inicijalno je bilo otežano dobijanje i prikazivanja slika u katalogu i detaljima knjige. Problem je brzo riješen kroz optimizaciju zahtjeva i cache mehanizama.
- Pojedine validacije pri zaduživanju i vraćanju knjiga zahtijevale su dodatne izmjene nakon što su identifikovani edge-case scenariji.

---

## Šta smo naučili

- Kompleksnije funkcionalnosti poput zaduživanja imaju više međuovisnosti nego što se inicijalno čini — promjena statusa primjerka utiče na dostupnost knjige, evidenciju člana i historiju zaduženja, što zahtijeva pažljivo planiranje redosljeda implementacije.
- Prikaz dostupnosti knjige nije samo broj — potrebno je razlikovati ukupan broj primjeraka, broj dostupnih i broj zaduženih, što je zahtijevalo pažljivije modeliranje podataka.
- Redovna komunikacija između članova tima direktno ubrzava integraciju modula i smanjuje broj naknadnih izmjena.

---

## Akcije za naredni sprint

| Akcija | Odgovornost | Prioritet |
|---|---|---|
| Bolje definisati tehničke zadatke prije početka sprinta | Scrum Master | Srednji |
| Implementirati dodatne sigurnosne mjere pri rukovanju sa zaduživanjem (kada se uvede status članarine) | Cijeli tim | Visok |
| Proširiti funkcionalnosti sa email notifikacijama za rokove vraćanja | Cijeli tim | Srednji |
| Unaprijediti UI pri prikazu statusa dostupnosti knjiga | Frontend tim | Srednji |
| Nastaviti sa dokumentacijom testnih slučajeva | Cijeli tim | Srednji |

---

## Zaključak

Sprint 7 uspješno je realizovan sa sve planirane funkcionalnosti implementirane na vrijeme. Sistem je sada u mogućnosti da omogući korisnicima efikasnu pretragu knjiga, pregled detalja, te upravljanje procesima zaduživanja i vraćanja. Identifikovani problemi su bili minimalni i brzo riješeni. Tim je pokazao odličnu dinamiku i saradnju, što je rezultat kvalitetnog planiranja i organizacije iz prethodnih sprintova.