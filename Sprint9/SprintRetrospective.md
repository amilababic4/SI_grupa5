# Sprint Retrospective Summary

## Sprint informacije
- **Naziv sprinta:** Sprint 9  
- **Trajanje sprinta:** 1 sedmica  
- **Datum retrospektive:** 24.05.2026.  
- **Tema projekta:** Bibliotečki informacioni sistem  

---

## Cilj retrospektive
Cilj retrospektive bio je analizirati realizaciju devetog sprinta, identifikovati pozitivne aspekte rada tima, izazove koji su se pojavili tokom implementacije funkcionalnosti rezervacija, napredne pretrage, izvještavanja i forumske sekcije, te definisati prijedloge za unapređenje procesa razvoja i integracije novih modula sistema.

---

## Šta je prošlo dobro

- Tim je uspješno realizovao sve planirane funkcionalnosti za Sprint 9.
- Implementirane su funkcionalnosti rezervacije knjiga, pregleda i otkazivanja rezervacija.
- Uspješno su implementirani napredni filteri za pretragu knjiga i kombinovanje više filtera.
- Generisani su mjesečni izvještaji o zaduživanjima, rezervacijama i članovima biblioteke.
- Dodana je sekcija „Forum i zajednica“ sa mogućnošću objava i komentarisanja.
- Integracija novih funkcionalnosti sa postojećim sistemom prošla je bez većih problema.
- Dokumentacija i backlog stavke redovno su ažurirani tokom sprinta.
- Komunikacija između članova tima bila je efikasna, što je ubrzalo razvoj i integraciju funkcionalnosti.
- Testiranje implementiranih funkcionalnosti nije otkrilo kritične greške sistema.

---

## Problemi i izazovi

- Prilikom implementacije PDF izvještaja pojavili su se problemi sa rasporedom elemenata u dokumentu, posebno kod tabela gdje su se pojedine kolone prelamale ili izlazile iz okvira stranice, što je zahtijevalo ručno podešavanje layouta.

- Tokom generisanja izvještaja pojavili su se problemi sa renderovanjem Chart.js grafova, gdje se prikaz nije uvijek ispravno osvježavao nakon promjene filtera i perioda izvještaja, što je zahtijevalo dodatno usklađivanje logike za refresh i ponovno crtanje grafova.

- Prikaz liste rezervacija sa većim brojem podataka uzrokovao je pretrpanost UI-a, pa je bilo potrebno dodatno optimizovati raspored elemenata kako bi stranica ostala pregledna i čitljiva.

---

## Šta smo naučili

- Integracija generisanja PDF izvještaja i grafova zahtijeva dodatno testiranje kompatibilnosti između biblioteka i frontend prikaza.
- Kombinovanje više filtera može značajno povećati kompleksnost backend logike i testiranja.
- Redovna komunikacija između članova tima ubrzava rješavanje problema tokom integracije frontend i backend komponenti.

---

## Akcije za naredni sprint

| Akcija | Odgovornost | Prioritet |
|---|---|---|
| Unaprijediti prikaz i optimizaciju Chart.js grafova u izvještajima | Cijeli tim | Srednji |
| Proširiti funkcionalnosti forumske sekcije dodatnim opcijama moderacije | Backend tim | Srednji |
| Optimizovati prikaz većeg broja rezervacija i rezultata pretrage | Frontend tim | Srednji |
| Nastaviti redovno ažuriranje projektne dokumentacije | Cijeli tim | Srednji |

---

## Zaključak

Sprint 9 uspješno je realizovan sa svim planiranim funkcionalnostima implementiranim u predviđenom vremenskom okviru. Sistem je unaprijeđen kroz funkcionalnosti rezervacije knjiga, napredne pretrage kataloga, automatskog upravljanja rezervacijama i generisanja mjesečnih izvještaja. Posebno značajan napredak ostvaren je implementacijom forumske sekcije koja omogućava veću interakciju i razmjenu sadržaja između članova biblioteke.

Iako su se tokom sprinta pojavili izazovi vezani za integraciju PDF izvještaja i prikaz grafova, problemi su uspješno riješeni kroz dodatna testiranja i prilagodbe implementacije. Tim je nastavio pokazivati visok nivo saradnje, organizacije i efikasnosti u razvoju novih funkcionalnosti sistema.