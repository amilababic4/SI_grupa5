# Sprint 10
## Sprint cilj
Cilj ovog sprinta je unaprijediti komunikaciju između sistema i korisnika, te proširiti administrativne i upravljačke funkcionalnosti SmartLib sistema. Fokus sprinta je na automatizaciji obavještavanja članova o rokovima vraćanja knjiga, uvođenju transparentnog praćenja promjena u sistemu, te olakšavanju administrativnih procesa vezanih za članarinu i nabavku knjiga.

Tokom sprinta implementirane su funkcionalnosti za automatsko slanje email podsjetnika i upozorenja članovima biblioteke o predstojećim i isteklim rokovima vraćanja knjiga, kao i obavještavanje o kašnjenjima. Uvedeno je i automatsko slanje email obavijesti bibliotekaru prilikom kreiranja svake nove rezervacije, čime se unapređuje organizacija rada bibliotečkog osoblja.

Pored toga, implementiran je audit log koji automatski evidentira sve promjene nad knjigama i korisničkim nalozima, čime se osigurava transparentnost i sigurnost sistema. Uveden je i sistem automatskog obračuna kazni za kasno vraćanje knjiga, uz mogućnost pregleda ukupnog duga na profilu člana.

U okviru sprinta omogućeno je i online podnošenje zahtjeva za produženje članarine od strane članova, uz pregled i obradu tih zahtjeva od strane bibliotekara. Implementirana je također integracija sa distributerom knjiga koja bibliotekaru omogućava kreiranje i slanje zahtjeva za nabavku knjige direktno iz sistema putem emaila.

Pored razvoja funkcionalnosti, nastavljeno je dokumentovanje procesa kroz AI Usage Log i Decision Log, uz evidentiranje odluka vezanih za implementaciju email notifikacija, audit loga, sistema kazni, upravljanja članarinom i integracije sa distributerom knjiga.

---
## Očekivani rezultati sprinta
- Implementirano automatsko slanje email podsjetnika članovima prije isteka roka vraćanja
- Implementirano slanje email upozorenja na dan isteka roka vraćanja knjige
- Implementirano slanje podsjetnika o kašnjenju za knjige prekoračenog roka vraćanja
- Osigurano zaustavljanje podsjetnika nakon vraćanja knjige
- Implementirano automatsko slanje email obavijesti bibliotekaru pri svakoj novoj rezervaciji
- Implementiran audit log za praćenje dodavanja, izmjena i brisanja knjiga
- Implementirano evidentiranje promjena korisničkih naloga u audit logu
- Implementiran automatski obračun kazni po danu kašnjenja pri vraćanju knjige
- Implementirano podnošenje zahtjeva za produženje članarine sa opcijama trajanja
- Omogućen pregled i obrada zahtjeva za produženje od strane bibliotekara
- Implementiran pregled statusa zahtjeva za produženje na profilu člana
- Omogućeno slanje zahtjeva za nabavku distributeru putem emaila direktno iz sistema
- Ažurirani AI Usage Log i Decision Log sa ključnim odlukama tokom sprinta