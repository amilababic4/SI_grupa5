# Sprint Retrospective Summary
## Sprint informacije
- **Naziv sprinta:** Sprint 10  
- **Trajanje sprinta:** 1 sedmica  
- **Datum retrospektive:** 01.06.2026.  
- **Tema projekta:** Bibliotečki informacioni sistem  

---
## Cilj retrospektive
Cilj retrospektive bio je analizirati realizaciju desetog sprinta, identifikovati pozitivne aspekte rada tima, izazove koji su se pojavili tokom implementacije funkcionalnosti automatskih email notifikacija, audit loga, sistema kazni, online upravljanja članarinom i integracije sa distributerom knjiga, te definisati prijedloge za unapređenje procesa razvoja u narednom sprintu.

---
## Šta je prošlo dobro
- Tim je uspješno realizovao sve planirane funkcionalnosti za Sprint 10.
- Implementirano je automatsko slanje email podsjetnika i upozorenja članovima o rokovima vraćanja knjiga.
- Uspješno je implementiran audit log koji evidentira sve važnije promjene sistema.
- Online podnošenje i obrada zahtjeva za produženje članarine funkcioniše prema očekivanjima.
- Integracija sa distributerom knjiga putem emaila implementirana je bez većih problema.
- Automatsko obavještavanje bibliotekara o novim rezervacijama uspješno je integrisano sa postojećim sistemom rezervacija.
- Dokumentacija i backlog stavke redovno su ažurirani tokom sprinta.
- Komunikacija između članova tima bila je efikasna, što je doprinijelo blagovremenom završetku svih planiranih stavki.
- Testiranje implementiranih funkcionalnosti nije otkrilo kritične greške sistema.

---
## Problemi i izazovi
- Implementacija audit loga zahtijevala je pažljivo definisanje koje sve akcije trebaju biti evidentirane, jer su se inicijalno pojavljivali dupli zapisi u određenim scenarijima izmjene podataka.
- Prikaz zahtjeva za produženje članarine na strani bibliotekara pokazao je probleme sa preglednošću kada postoji veći broj zahtjeva sa statusom "Na čekanju", što je zahtijevalo dodatno uređenje UI komponenti.

---
## Šta smo naučili
- Implementacija automatskih email notifikacija zahtijeva temeljito testiranje rubnih slučajeva, posebno kada korisnik nema evidentiranu email adresu ili kada se status zaduženja promijeni između dva planirana slanja.
- Audit logovi zahtijevaju precizno definisanje događaja koji se evidentiraju kako bi se izbjeglo nepotrebno dupliciranje zapisa i preopterećenje baze podataka.
- Online obrasci za administrativne procese poput produženja članarine zahtijevaju jasnu povratnu informaciju korisniku o svakom koraku procesa kako bi se smanjila konfuzija oko statusa zahtjeva.

---
## Akcije za naredni sprint
| Akcija | Odgovornost | Prioritet |
|---|---|---|
| Optimizovati logiku grupiranja email notifikacija za članove sa više zaduženja | Backend tim | Srednji |
| Unaprijediti prikaz i filtriranje audit log zapisa za administratore | Frontend tim | Srednji |
| Poboljšati preglednost liste zahtjeva za produženje članarine na strani bibliotekara | Frontend tim | Srednji |
| Nastaviti redovno ažuriranje projektne dokumentacije | Cijeli tim | Srednji |

---
## Zaključak
Sprint 10 uspješno je realizovan sa svim planiranim funkcionalnostima implementiranim u predviđenom vremenskom okviru. Sistem je značajno unaprijeđen kroz automatizaciju email komunikacije sa članovima i bibliotečkim osobljem, uvođenje transparentnog audit loga, sistem obračuna kazni za kašnjenja, te online upravljanje članarinom i nabavkom knjiga.

Iako su se tokom sprinta pojavili izazovi vezani za logiku slanja notifikacija i strukturu audit zapisa, problemi su uspješno riješeni kroz dodatna testiranja i prilagodbe implementacije. Tim je i u ovom sprintu pokazao visok nivo organizacije, međusobne saradnje i efikasnosti u isporuci novih funkcionalnosti sistema.