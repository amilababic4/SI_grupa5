# Test Strategy 

Ovaj dokument definiše strategiju testiranja Bibliotečkog informacionog sistema i opisuje pristup osiguravanju kvaliteta kroz različite nivoe testiranja. Dokument obuhvata ciljeve testiranja, vrste testova, mapiranje funkcionalnosti na nivoe testiranja, vezu sa acceptance kriterijima, način evidentiranja rezultata te identifikaciju ključnih rizika kvaliteta.

## 1. Cilj testiranja

Testiranje Bibliotečkog informacionog sistema ima za cilj provjeriti da sve funkcionalnosti sistema rade kako treba, od registracije članova do rezervacije knjiga, u skladu sa definisanim zahtjevima i pravilima sistema.

| Cilj | Šta se time provjerava |
|------|-------------------|
| **Ispravnost poslovne logike** | Knjiga se ne smije označiti kao dostupna dok postoji aktivno zaduženje (NFR-7). Primjerak se ne smije deaktivirati dok je zadužen (US-24). Rezervacija nije dozvoljena sa dostupnim primjercima knjige (US-69). |
| **Tačnost upravljanja katalogom** | Dodavanje, uređivanje i brisanje knjiga, primjeraka i kategorija mora biti konzistentno (US-12, US-17, US-25). |
| **Kontrola pristupa po ulogama** | Svaka uloga smije vidjeti isključivo funkcionalnosti koje su joj dozvoljene — provjera mora biti i na UI i na API nivou (NFR-5, US-08, US-09). |
| **Sigurnost korisničkih podataka** | Lozinke se pohranjuju isključivo u hashiranom obliku (NFR-6). Nijedan API poziv ne smije propustiti neautorizovani zahtjev (NFR-5). |
| **Pouzdanost upravljanja članarinama** | Status članarine direktno blokira zaduživanje — sistem mora u svakom trenutku prikazivati tačan status (US-58 i US-59). |
| **Integritet rezervacionog toka** | Rezervacija se automatski otkazuje po isteku roka (US-79). Preuzimanje rezervisane knjige kreira validno zaduženje (Use Case: Evidencija zaduživanja). |
| **Validacija korisničkog unosa** | Sve forme (registracija, dodavanje knjige, članarina) moraju odbiti neispravne podatke s jasnom porukom uz polje koje sadrži grešku (US-02, US-13, NFR-4). |
| **Performanse i odziv sistema** | Svaka stranica mora se učitati unutar 2 sekunde na stabilnoj konekciji — uključujući katalog s knjigama (NFR-1). |
| **Auditabilnost administratorskih akcija** | Sve važne promjene (izmjena korisnika, knjige, zaduženje, vraćanje) moraju biti evidentirane u audit logu (NFR-11). |
| **Upotrebljivost i internacionalizacija** | Poruke grešaka su jasne i razumljive (NFR-2), destruktivne akcije traže potvrdu (NFR-3), svi tekstovi prikazani su na bosanskom jeziku (NFR-8). |

---

<br>

## 2. Nivoi testiranja

Različiti nivoi testiranja omogućavaju provjeru sistema od pojedinačnih funkcija i komponenti, preko njihove međusobne integracije, pa sve do provjere cjelokupnog rada sistema iz perspektive krajnjeg korisnika. Na ovaj način se osigurava da implementirane funkcionalnosti zadovoljavaju definisane zahtjeve i da sistem radi očekivano u realnim scenarijima korištenja.


---

### 2.1 Unit testiranje

**Cilj:**  
Provjera pojedinačnih funkcija i komponenti: validacija forme (registracija, prijava), provjera unosa (email, lozinka), logika autentifikacije, upravljanje statusom knjiga i primjeraka.

**Alati:**  
xUnit (.NET) - pisanje i automatsko pokretanje unit testova, Visual Studio Test Explorer - pokretanje, pregled rezultata i praćenje uspješnost.

**Izlazni kriteriji (kriterij prihvatanja):**  
Svi testovi prolaze, osnovna logika ispravna.

**Ograničenja:**  
Ne može otkriti probleme koji nastaju u komunikaciji između modula, servisa ili baze podataka.


---

### 2.2 Integracijsko testiranje

**Cilj:**  
Provjera komunikacije između komponenti: frontend – backend, API – baza podataka, autentifikacija i autorizacija, operacije nad knjigama i korisnicima.

**Alati:**  
Postman, Mailtrap - testiranje slanja email notifikacija u sigurnom testnom okruženju bez slanja stvarnih emailova korisnicima.


**Izlazni kriteriji:**  
API vraća ispravne odgovore, podaci konzistentni.

**Ograničenja:**  
Uzrok grešaka može biti teže izolirati jer uključuje više komponenti.


---

### 2.3 Sistemsko testiranje

**Cilj:**  
Testiranje kompletnog sistema kroz end-to-end tokove: registracija, prijava, pregled kataloga, dodavanje i brisanje knjiga, zaduživanje i vraćanje. Obuhvatiti funkcionalne i nefunkcionalne aspekte (performanse, sigurnost, upotrebljivost).  

**Alati:**  
Ručno testiranje u pretraživaču, Developer Tools - praćenje mrežnih zahtjeva, grešaka u konzoli i performansi tokom testiranja.

**Izlazni kriteriji:**  
Svi ključni tokovi funkcionišu ispravno.

**Ograničenja:**  
Sistemsko testiranje je vremenski najzahtjevnije. Uzrok grešaka teže je izolirati jer uključuje cijeli sistem.

---

### 2.4 Prihvatno testiranje (UAT – User Acceptance Testing)

**Cilj:**  
Provjera da sistem zadovoljava acceptance kriterije iz User Story-ja: validacija unosa, uspješna registracija, pravilno upravljanje knjigama i korisnicima.

**Alati:**  
Ručno testiranje u pretraživaču.

**Izlazni kriteriji:**  
Korisnici potvrđuju ispravnost sistema, nema kritičnih grešaka, sistem odobren za isporuku.

**Ograničenja:**  
Zavisi od subjektivne procjene korisnika i obično pokriva samo glavne funkcionalne scenarije, pa neke tehničke greške mogu ostati neotkrivene.

---

### 2.5 Regresiono testiranje

**Cilj:**  
Provjera da postojeće funkcionalnosti (prijava, registracija, katalog, upravljanje knjigama i korisnicima) i dalje rade ispravno nakon izmjena ili dodavanja novih funkcionalnosti.

**Alati:**  
Ručno testiranje, automatizovani testovi (xUnit) - automatsko pokretanje postojećih testova.

**Izlazni kriteriji:**  
Nema regresija u postojećim funkcionalnostima.

**Ograničenja:**  
Može biti vremenski zahtjevno jer je potrebno ponovo testirati veći broj postojećih funkcionalnosti nakon svake izmjene.

---

### 2.6 UI testiranje

**Cilj:**  
Provjera korisničkog interfejsa: ispravnost prikaza formi, poruka grešaka uz odgovarajuće polje, navigacija po ulogama, responsivnost na različitim rezolucijama, konzistentnost elemenata kroz pretraživače (Chrome, Firefox, Edge).

**Alati:**  
Ručno testiranje u pretraživaču, Browser DevTools, Selenium - simuliranje ponašanja korisnika.

**Izlazni kriteriji:**  
UI je konzistentan na svim ciljnim pretraživačima; responzivan prikaz funkcioniše na mobilnim i desktop rezolucijama.

**Ograničenja:**  
Vizualne razlike između pretraživača teško je u potpunosti eliminisati; ne otkriva logičke greške na backendu.


---


### 2.7 Penetracijsko / sigurnosno testiranje

**Cilj:**  
Aktivna provjera sigurnosnih propusta: SQL injection, XSS, CSRF, neispravno upravljanje sesijom, izloženost osjetljivih podataka, testiranje RBAC zaštite direktnim API pozivima bez autentifikacije, provjera hashiranja lozinki.

**Alati:**  
OWASP ZAP (automatsko skeniranje), Postman (manualni API napadi), pregled baze podataka.

**Izlazni kriteriji:**  
Nema kritičnih sigurnosnih ranjivosti; svi neautorizirani API pozivi vraćaju 401/403; lozinke nisu čitljive u bazi; XSS i SQL injection pokušaji su odbijeni.

**Ograničenja:**  
Moguć je veliki broj lažnih pozitivnih rezultata (false positives) koje je potrebno ručno verifikovati, što produžava vrijeme analize.

---
<br> 

## 3. Šta se testira u kojem nivou

Tabela ispod povezuje ključne funkcionalnosti bibliotečkog sistema sa nivoima testiranja. Fokus je na tome šta se provjerava na kojem nivou.

| Funkcionalnost | Unit | Integracijsko | Sistemsko | Prihvatno (UAT) | Regresiono | UI | Penetracijsko |
|------------------------|------|---------------|-----------|----------------|------------|----|----------------|
| **Registracija i prijava korisnika** (US-01, US-02, US-03, US-04, US-05) | DA – validacija emaila, formata lozinke i obaveznih polja | DA – provjera auth servisa, kreiranja sesije i upisa u bazu | DA – kompletan tok: unos podataka, validacija, prijava i preusmjeravanje na dashboard | DA – korisnik potvrđuje uspješan login i razumljive poruke o grešci | DA – provjera nakon izmjena auth logike ili formi | DA – prikaz formi, poruke o grešci, redirect na odgovarajući dashboard | DA – SQL injection i XSS na login i registracijskim poljima |
| **Upravljanje sesijom i RBAC** (US-06, US-07, US-08, US-09) | NE | DA – provjera kreiranja i brisanja sesije, te zaštite ruta po ulozi | DA – kontrola pristupa po ulogama: član ne može otvoriti bibliotekarsku sekciju i sl. | DA – admin potvrđuje da korisnici s nižim pravima ne mogu pristupiti zaštićenim sekcijama | DA – provjera nakon dodavanja novih ruta ili izmjene uloga | NE | DA – pokušaj promjene uloge manipulacijom zahtjeva |
| **Upravljanje knjigama** (US-12, US-17, US-25) | DA – validacija ISBN-a, obaveznih polja i poslovnih pravila (npr. brisanje nije moguće uz aktivno zaduženje) | DA – konzistentnost između API-ja i baze pri dodavanju, izmjeni i brisanju | DA – kompletan CRUD tok kroz korisničko sučelje | DA – bibliotekar prolazi kroz dodavanje i izmjenu knjige te potvrđuje ispravan tok | DA – provjera nakon svake izmjene CRUD logike | DA – forma za unos, dugmad, poruke o grešci i dijaloški prozori za potvrdu | DA – neovlašteni korisnik ne smije moći mijenjati knjige direktnim pozivom API-ja |
| **Upravljanje primjercima** (US-21, US-23, US-24) | DA – tranzicije statusa primjerka i validacija pri deaktivaciji | DA – promjena statusa primjerka ispravno se reflektuje u bazi | DA – prikaz statusa kroz cijeli sistem: katalog, detalji knjige, zaduživanje | DA – bibliotekar potvrđuje tačnost prikaza statusa za svaki primjerak | DA – provjera nakon izmjene logike statusa | DA – vizuelni prikaz statusa Dostupan / Posuđen / Deaktiviran | NE |
| **Katalog i pretraga** (US-19, US-20, US-35, US-36) | DA – logika filtriranja po naslovu, autoru i ključnoj riječi | DA – dohvat i filtriranje podataka iz baze | DA – pregled kataloga, pretraga i prikaz rezultata | DA – korisnik potvrđuje preglednost kataloga i relevantnost rezultata pretrage | DA – provjera nakon izmjene logike pretrage | DA – responzivnost tabele, rad filtera, prikaz broja rezultata | NE |
| **Zaduživanje i vraćanje knjiga** (US-43, US-44, US-45, US-47) | DA – provjera dostupnosti primjerka, roka vraćanja | DA – ažuriranje statusa primjerka i evidencija zaduženja u bazi | DA – kompletan tok: odabir člana, odabir knjige, potvrda i evidencija | DA – bibliotekar prolazi kroz cijeli proces zaduživanja i vraćanja | DA – provjera nakon izmjene poslovnih pravila zaduživanja | DA – prikaz forme, odabir člana i knjige, potvrda akcije | DA – pokušaj višestrukog zaduživanja istog primjerka direktnim pozivom API-ja |
| **Upravljanje članarinom** (US-56, US-57, US-58, US-59) | DA – validacija datuma: datum isteka ne smije biti prije datuma početka | DA – veza između korisnika i članarine i njen uticaj na dozvolu zaduživanja | DA – blokiranje zaduživanja kada je članarina istekla ili ne postoji | DA – korisnik vidi tačan status i datum isteka u svom profilu | DA – provjera nakon produženja ili izmjene datuma članarine | DA – prikaz statusa članarine u profilu člana | NE |
| **Rezervacije** (US-69, US-72, US-79, US-80) | DA – provjera pravila: knjiga mora biti nedostupna, jedan član može imati jednu aktivnu rezervaciju | DA – rezervacija se ispravno vezuje za knjigu i reflektuje u katalogu | DA – tok kreiranja, pregleda i otkazivanja rezervacije | DA – korisnik prolazi kroz rezervaciju i potvrđuje prikaz u profilu | DA – provjera nakon izmjene logike rezervacija | DA – prikaz liste aktivnih rezervacija i dugme za otkazivanje | DA – pokušaj rezervacije bez aktivne članarine direktnim pozivom API-ja |
| **Upravljanje kategorijama** (US-30, US-33, US-34) | DA – validacija naziva: prazno polje i duplikati se odbijaju | DA – provjera da brisanje kategorije s povezanim knjigama nije dozvoljeno | DA – pregled, dodavanje, izmjena i brisanje kategorija kroz korisničko sučelje | DA – bibliotekar prolazi kroz upravljanje kategorijama | DA – provjera nakon dodavanja novih kategorija ili izmjene logike | DA – prikaz kategorija u padajućem meniju pri dodavanju knjige | NE |
| **Upravljanje korisnicima (admin)** (US-49, US-50, US-51, US-52, US-53) | NE | DA – promjena uloge i deaktivacija ispravno se reflektuju kroz cijeli sistem | DA – kompletan tok: pregled korisnika, promjena uloge i deaktivacija naloga | DA – admin potvrđuje da se promjene odmah primjenjuju | DA – provjera nakon dodavanja novih uloga ili izmjene admin logike | DA – tabela korisnika, dugmad za izmjenu uloge i deaktivaciju | DA – korisnik s nižim pravima pokušava izvršiti admin akciju direktnim pozivom API-ja |
| **Email notifikacije** (US-81, US-82, US-83, US-84) | NE | DA – email servis se aktivira pri odgovarajućim događajima (istek roka vraćanja, nova rezervacija) | DA – notifikacije se šalju u realnim scenarijima zaduživanja i rezervacije | DA – korisnik prima obavijest na pravu adresu u testnom okruženju | DA – provjera nakon izmjene email predložaka | NE | NE |
| **Audit log** (NFR-11) | NE | DA – akcije  se evidentiraju pri svakoj izvršenoj promjeni | DA – provjera da se log popunjava tokom realnih scenarija | DA – admin potvrđuje da zapis sadrži tačne podatke o akciji i korisniku | DA – provjera nakon svake nove funkcionalnosti | NE | DA – obični korisnik ne smije moći brisati niti mijenjati audit zapise |
| **Validacija unosa i UX poruke** (NFR-2, NFR-3, NFR-4) | DA – validacione funkcije za svako polje u sistemu | DA – validacija se jednako provodi na nivou API-ja, a ne samo na frontendu | DA – poruke o grešci prikazuju se uz ispravna polja u svim formama | DA – korisnik razumije poruku i zna šta treba ispraviti | DA – provjera nakon izmjene frontenda ili validacijske logike | DA – vizuelna pozicija i sadržaj poruke direktno uz polje s greškom | NE |
| **Sigurnost i kontrola pristupa** (NFR-5, NFR-6) | DA – hashiranje lozinki, lozinka se ne pojavljuje ni u jednom logu ni odgovoru servera | DA – zaštita svih endpointa: neautorizovani zahtjev vraća odgovarajuću grešku | DA – provjera kontrole pristupa kroz realne tokove sistema | DA – nema sigurnosnih propusta uočljivih u standardnom korištenju | DA – provjera nakon dodavanja novih ruta ili izmjene auth logike | NE | DA – SQLi, XSS, CSRF i zaobilaženje RBAC-a |

<br>

## 4. Veza sa acceptance kriterijima

Ova sekcija prikazuje kako se acceptance kriteriji iz dokumenta *Set of User Stories* mapiraju na nivoe testiranja i koje artefakte koristimo kao dokaz njihovog ispunjenja. 

<br>

| Veza | Acceptance Criteria | Nivoi testiranja | Dokaz ispunjenja |
|-----------|----------------------------|-------------------|-----------------|
| Kreiranje naloga člana (**US-01, US-02, US-03**) | Forma prikazuje obavezna polja; sistem odbija unos bez popunjenih polja, s neispravnim emailom, s lozinkom kraćom od 8 znakova ili s već registrovanim emailom; nakon uspješne registracije novi član se pojavljuje u listi i automatski dobiva ulogu Član | Unit, Integracijsko, Sistemsko, Prihvatno | Testovi validacije forme (unit), API odgovor pri registraciji, provjera baze da je korisnik kreiran s ulogom Član |
| Prijava i obavijest o neuspjehu (**US-04, US-05**) | Ispravni kredencijali vode na odgovarajući dashboard prema ulozi; neispravni podaci vraćaju generičku poruku bez otkrivanja da li greška leži u emailu ili lozinci; korisnik može ponoviti pokušaj | Unit, Integracijsko, Sistemsko, Sigurnosno, Prihvatno | Unit testovi validacije, API provjera autentifikacije, provjera da odgovor servera ne otkriva detalje greške |
| Odjava i čuvanje sesije (**US-06, US-07**) | Nakon odjave sistem preusmjerava na prijavu i ne dozvoljava pristup zaštićenim stranicama bez ponovne prijave; sesija ostaje aktivna pri kretanju između stranica i briše se pri odjavi | Integracijsko, Sistemsko, Prihvatno | Provjera brisanja sesije u bazi, pokušaj pristupa zaštićenoj stranici nakon odjave |
| Zaštita ruta i deaktivirani korisnik (**US-08, US-09**) | Neprijavljeni korisnik se preusmjerava na prijavu čak i pri direktnom unosu URL-a; deaktivirani korisnik ne može pristupiti sistemu ni putem stare sesije; prikazuje se generička poruka bez otkrivanja razloga blokade | Integracijsko, Sistemsko, Sigurnosno, Prihvatno | API testovi (401/403), provjera da stara sesija deaktiviranog korisnika ne prolazi, UI provjera prikaza poruke |
| Dodavanje knjige (**US-12, US-13, US-14, US-15, US-16**) | Forma obuhvata sva obavezna polja (naslov, autor, ISBN, godina, kategorija, primjerci); ISBN se validira po formatu i jedinstvenosti; broj primjeraka mora biti nenegativan cijeli broj; knjiga se odmah pojavljuje u katalogu | Unit, Integracijsko, Sistemsko, Prihvatno | Unit testovi validacije ISBN-a i broja primjeraka, API zapis dodavanja knjige, provjera prikaza u katalogu |
| Uređivanje knjige (**US-17, US-18**) | Forma prikazuje postojeće podatke; dozvoljena je izmjena naslova, autora, godine i kategorije; ažurirani podaci se odmah prikazuju u katalogu; samo bibliotekar ima pristup izmjeni | Integracijsko, Sistemsko, Prihvatno | API provjera izmjene, provjera da član ne može pristupiti ruti za izmjenu, provjera ažuriranog prikaza u katalogu |
| Katalog knjiga (**US-19, US-20**) | Katalog prikazuje sve aktivne knjige učitane iz baze; neaktivne i obrisane knjige se ne prikazuju; pri većem broju knjiga radi paginacija | Integracijsko, Sistemsko, Prihvatno | API provjera da obrisane knjige nisu u odgovoru, UI provjera paginacije |
| Upravljanje primjercima (**US-21, US-22, US-23, US-24**) | Svaki primjerak je zaseban zapis s jedinstvenim ID-em i vidljivim statusom; status se ažurira pri promjenama; deaktivacija nije moguća za primjerak koji je trenutno zadužen | Unit, Integracijsko, Sistemsko, Prihvatno | Unit testovi tranzicija statusa, API provjera zabrane deaktivacije zaduženog primjerka, UI prikaz statusa svakog primjerka |
| Brisanje knjige i deaktivacija primjerka (**US-25, US-26, US-27, US-28**) | Brisanje nije dozvoljeno ako postoji aktivno zaduženje; sistem traži potvrdu prije brisanja; deaktivirani primjerak ne ulazi u broj dostupnih primjeraka | Unit, Integracijsko, Sistemsko, Prihvatno | Unit test poslovnog pravila, API provjera odbijanja brisanja uz aktivno zaduženje, UI provjera dijaloga za potvrdu |
| Upravljanje kategorijama (**US-30 – US-34**) | Naziv kategorije mora biti jedinstven i neprazan; izmjena se odbija ako naziv već postoji; brisanje nije dozvoljeno ako kategorija sadrži knjige; kategorija je odmah vidljiva u padajućem meniju pri dodavanju knjige | Unit, Integracijsko, Sistemsko, Prihvatno | Unit testovi validacije naziva, API provjera zabrane brisanja kategorije s knjigama, UI provjera padajućeg menija |
| Pretraga knjiga (**US-35, US-36**) | Pretraga filtrira po naslovu, autoru i ključnoj riječi; klik na "Očisti" vraća punu listu; ako nema rezultata prikazuje se odgovarajuća poruka | Unit, Integracijsko, Sistemsko, Prihvatno | Unit testovi logike pretrage, API provjera filtriranja, UI provjera prikaza rezultata i poruke pri praznom skupu |
| Dostupnost knjige (**US-40, US-41, US-42**) | Status Dostupno / Zaduženo tačno odražava broj slobodnih primjeraka; zaduženi i deaktivirani primjerci se ne računaju; broj se ažurira odmah nakon zaduživanja i vraćanja | Unit, Integracijsko, Sistemsko, Prihvatno | Unit test izračuna broja primjeraka, API provjera ažuriranog broja nakon transakcije, UI provjera prikaza na stranici detalja |
| Zaduživanje i vraćanje (**US-43, US-44, US-45, US-46, US-47**) | Zaduživanje nije moguće bez aktivne članarine; nedostupan primjerak se ne može zadužiti; status primjerka se odmah ažurira; rok vraćanja se automatski kreira; vraćanje ažurira status primjerka u Dostupan; isti primjerak ne može biti zadužen dva puta istovremeno | Integracijsko, Sistemsko, Sigurnosno, Prihvatno | API provjera svih poslovnih pravila, provjera promjene statusa primjerka u bazi |
| Pregled profila (**US-48**) | Profil prikazuje ime, prezime, email i trenutno posuđene knjige; član vidi samo vlastiti profil | Integracijsko, Sistemsko, Sigurnosno, Prihvatno | API provjera da član ne može dohvatiti profil drugog korisnika, UI provjera prikaza podataka |
| Upravljanje korisnicima (admin) (**US-49, US-50, US-51, US-52, US-53**) | Administrator vidi listu svih korisnika s osnovnim podacima i može pretraživati po imenu ili emailu; promjena uloge se odmah primjenjuje; deaktivirani korisnik ne može pristupiti sistemu; administrator ne može deaktivirati vlastiti nalog | Integracijsko, Sistemsko, Sigurnosno, Prihvatno | API zapis izmjena uloge i deaktivacije, provjera da deaktivirani korisnik ne prolazi auth, API provjera zabrane samodeaktivacije |
| Historija zaduženja (**US-54**) | Bibliotekar vidi listu svih ranijih zaduženja člana s nazivom knjige, primjerkom, datumom zaduženja i datumom vraćanja; prikazuju se samo završena zaduženja | Integracijsko, Sistemsko, Prihvatno | API provjera da aktivna zaduženja nisu u historiji, UI provjera prikaza podataka historije |
| Upravljanje članarinom (bibliotekar) (**US-55, US-56, US-57**) | Forma sadrži datum početka i datum isteka; datum isteka ne smije biti prije datuma početka; izmjene su odmah vidljive na profilu; ako članarina ne postoji prikazuje se jasna poruka | Unit, Integracijsko, Sistemsko, Prihvatno | Unit test validacije datuma, API provjera odbijanja neispravnih datuma, UI provjera prikaza statusa na profilu |
| Pregled članarine (član) (**US-58, US-59**) | Član vidi status Aktivna ili Istekla i datum isteka u formatu DD-MM-YYYY; ako članarina ne postoji prikazuje se odgovarajuća poruka | Integracijsko, Sistemsko, Prihvatno | UI provjera prikaza statusa i datuma, provjera poruke kada članarina ne postoji |
| Pregled vlastitih zaduženja (**US-62, US-63, US-64**) | Član vidi samo vlastita aktivna zaduženja s rokom vraćanja; zakašnjela zaduženja su vizualno označena; vraćene knjige se ne prikazuju u aktivnim zaduženjima | Integracijsko, Sistemsko, Sigurnosno, Prihvatno | API provjera da odgovor sadrži samo zaduženja prijavljenog korisnika, UI provjera vizualnog isticanja zakašnjelih, provjera da vraćene knjige nestaju iz liste |
| Aktivna zaduženja (bibliotekar) (**US-65, US-66, US-67, US-68**) | Lista prikazuje sva aktivna zaduženja sortirana po roku vraćanja (najbliži rok prvi); filtriranje po članu ispravno sužava rezultate; detalji prikazuju člana, knjigu, primjerak i datume | Integracijsko, Sistemsko, Prihvatno | API provjera sortiranja i filtriranja, UI provjera prikaza detalja |
| Rezervacije (član) (**US-69, US-70, US-71, US-72**) | Rezervacija je moguća samo kada knjiga nema dostupnih primjeraka; isti član ne može kreirati dvije aktivne rezervacije iste knjige; svaka rezervacija ima zabilježen datum i vidljiva je u sekciji "Moje rezervacije"; otkazivanje odmah uklanja rezervaciju iz liste | Unit, Integracijsko, Sistemsko, Prihvatno | Unit testovi pravila rezervacije, API provjera odbijanja rezervacije dostupne knjige i duplikta, UI provjera prikaza i otkazivanja |
| Pregled rezervacija (bibliotekar) (**US-73**) | Lista prikazuje sve aktivne rezervacije s imenom i emailom člana, naslovom knjige i datumom; otkazane i realizovane rezervacije se ne prikazuju | Integracijsko, Sistemsko, Prihvatno | API provjera filtriranja statusa, UI provjera prikaza podataka |
| Napredna pretraga i filteri (**US-74, US-75, US-76, US-78**) | Filtriranje po kategoriji, izdavaču i godini prikazuje samo odgovarajuće knjige; kombinovanje više filtera ispravno sužava rezultate; pri praznom skupu prikazuje se poruka "Nema rezultata" | Unit, Integracijsko, Sistemsko, Prihvatno | Unit testovi logike kombinovanih filtera, API provjera rezultata za svaki filter, UI provjera ažuriranja liste |
| Automatsko otkazivanje rezervacije (**US-79, US-80**) | Svaka rezervacija ima definisan datum isteka; po isteku status prelazi u Otkazana i rezervacija se ne računa kao aktivna; knjiga postaje dostupna za novu rezervaciju ili zaduživanje | Integracijsko, Sistemsko, Prihvatno | Simulacija isteka u testnom okruženju, API provjera promjene statusa, provjera dostupnosti knjige nakon otkazivanja |
| Email notifikacije (član) (**US-81, US-82, US-83, US-84, US-85**) | Podsjetnik se šalje 2 dana prije isteka roka; upozorenje se šalje na dan isteka; email sadrži naziv knjige i datum roka; podsjetnici se zaustavljaju nakon vraćanja; email se ne šalje ako adresa nije evidentirana | Integracijsko, Sistemsko, Prihvatno | Mailtrap evidencija primljenih emailova, log slanja u sistemu, simulacija isteka roka u testnom okruženju |
| Email notifikacija bibliotekara  (**US-86**)| Kada član kreira rezervaciju, sistem automatski šalje email bibliotekaru s imenom člana, naslovom knjige i datumom; šalje se samo jednom po rezervaciji | Integracijsko, Sistemsko, Prihvatno | Mailtrap evidencija, provjera da se email ne šalje duplo pri istoj rezervaciji |
| Mjesečni izvještaji (**US-88, US-89, US-90**) | Administrator može odabrati tip izvještaja (zaduženja, rezervacije, članovi) i period (mjesec i godina); izvještaj prikazuje tačne podatke za odabrani period; ako nema podataka prikazuje se odgovarajuća poruka | Integracijsko, Sistemsko, Prihvatno | API provjera tačnosti podataka u izvještaju, UI provjera generisanja za različite periode i tipove |
| Audit log (**US-91, US-92**) | Svako dodavanje, izmjena ili brisanje knjige kreira audit zapis s akcijom, datumom, vremenom i korisnikom; promjene nad korisničkim nalozima se bilježe na isti način; zapisi nisu dostupni za izmjenu | Integracijsko, Sistemsko, Sigurnosno, Prihvatno | Provjera baze da je zapis kreiran nakon svake admin akcije, API provjera da obični korisnik ne može mijenjati audit zapise |
| Kazne za kasno vraćanje (**US-93, US-94**) | Sistem obračunava kaznu po danu kašnjenja i veže je za konkretno zaduženje i člana; član vidi ukupan iznos kazni na profilu; ako nema kazni prikazuje se poruka da nema dugovanja | Unit, Integracijsko, Sistemsko, Prihvatno | Unit test obračuna kazne, API provjera iznosa nakon vraćanja s kašnjenjem, UI provjera prikaza na profilu |

<br>

## 5. Način evidentiranja rezultata testiranja

Rezultati testiranja evidentiraju se kroz strukturisane test zapise koji nastaju tokom izvršavanja test aktivnosti. Svaki test ima jasno definisan ishod:

- **USPJEH** – funkcionalnost radi ispravno i u skladu sa acceptance kriterijima
- **GREŠKA** – uočeno odstupanje od očekivanog ponašanja sistema
- **NEIZVRŠENO** – test nije mogao biti sproveden zbog tehničkih ili eksternih ograničenja

---

### Podaci koji se evidentiraju

Za svaki test zapis bilježe se sljedeće informacije:

- **Datum izvršavanja** – kada je test proveden  
- **User Story ili NFR referenca** – na šta se test odnosi 
- **Nivo testiranja** – unit, integracijsko, sistemsko, regresiono ili prihvatno testiranje  
- **Testni scenario** – šta se tačno provjerava u okviru testa  
- **Očekivani rezultat** – kako bi sistem trebao reagovati  
- **Stvarni rezultat** – šta se desilo tokom izvršavanja testa  
- **Okruženje** – development, test ili staging okruženje  
- **Status testa** – USPJEH / GREŠKA / NEIZVRŠENO  
- **Dokaz izvršenja** – log, API odgovor, screenshot ili drugi oblik evidencije  

---

### Evidencija grešaka

U slučaju da test ima status **GREŠKA**, uz test zapis se vodi i evidencija defekta koja omogućava kasniju analizu i ispravku problema. Svaka greška mora biti dovoljno opisana da se može ponoviti i razumjeti bez dodatnih informacija. Greške se označavaju jednostavnom internom oznakom u formatu: **BG-XX** gdje je XX redni broj greške u okviru projekta.


<br>

## 6. Glavni rizici kvaliteta
U nastavku, navedeni su neki od potencijalnih rizika kvaliteta, koji su podijeljeni u četiri kategorije: funkcionalni, sigurnosni, UI/UX i procesni. Svaki rizik sadrži i preporučene mjere prevencije i ublažavanja.
### 6.1 Funkcionalni rizici
Funkcionalni rizici se odnose na ispravnost rada sistema u skladu sa definisanim poslovnim zahtjevima. Fokus ove kategorije je na preciznosti obrade podataka, pokrivanju specifičnih "rubnih" slučajeva i održavanju stabilnosti sistema kroz regresiono testiranje.

| Rizik | Mjere prevencije i ublažavanja |
|-------|-------------------|
| **Nekonzistentnost podataka pri paralelnim zahtjevima** – dva korisnika istovremeno zadužuju isti primjerak knjige | Osigurati zaključavanje podataka na nivou baze kako bi se spriječili konflikti pri istovremenim operacijama |
| **Pogrešan prikaz statusa članarine** – status prikazan kao aktivan iako je istekao, što omogućava zaduživanje | Implementirati validaciju statusa članarine u svim relevantnim poslovnim operacijama, posebno prije zaduživanja knjige |
| **Regresione greške** – ispravka jedne greške uzrokuje novu u drugom dijelu sistema | Regresiono testiranje ključnih funkcionalnosti nakon svake izmjene sistema |

---

### 6.2 Sigurnosni rizici
Sigurnosni rizici obuhvataju potencijalne prijetnje integritetu podataka i privatnosti korisnika. Identifikacija ovih rizika osigurava zaštitu od zlonamjernih napada, neovlaštenog pristupa osjetljivim informacijama i osigurava stabilnost autorizacijskih mehanizama.

| Rizik | Mjere prevencije i ublažavanja |
|-------|-------------------|
| **Propusti u kontroli pristupa (RBAC)** – korisnik s nižim privilegijama direktnim API pozivom pristupa zabranjenim resursima | RBAC testiranje i na UI i na API nivou; penetracijsko testiranje autorizacijskih ruta |
| **SQL Injection** – zlonamjerni unos u polja za pretragu prosljeđuje se direktno u SQL upit |Koristiti parametrizovane upite i validaciju ulaznih podataka
| **Brute force napad na prijavu** – automatizovano pogađanje lozinki bez zaštite | Uvesti ograničenje broja pokušaja prijave i mehanizme zaštite od prekomjernih zahtjeva |
| **Neispravno upravljanje sesijom** – sesija ostaje aktivna nakon odjave | Osigurati pravilno invalidiranje sesije nakon odjave |

---

### 6.3 UI/UX rizici
UI/UX rizici su fokusirani na interakciju korisnika sa sistemom. Cilj je osigurati intuitivno iskustvo, vizuelnu konzistentnost na različitim uređajima i preglednicima, te spriječiti korisničke greške kroz jasne povratne informacije.

| Rizik | Mjere prevencije i ublažavanja|
|-------|-------------------|
| **Nekonzistentan prikaz u pretraživačima** – razlike u prikazu između Chrome, Firefox i Edge pretraživača | UI testiranje u Chrome, Firefox i Edge pretraživačima za sve ključne ekrane |
| **Destruktivne akcije bez potvrde** – slučajno brisanje korisnika ili knjige bez dijaloga za potvrdu | Uvesti potvrdu za sve kritične akcije koje mijenjaju ili brišu podatke |
| **RBAC nije reflektovan u UI** – korisnik vidi opcije koje nema pravo izvršiti, što vodi do tehničkih grešaka | UI testiranje za svaku ulogu (Član, Bibliotekar, Administrator); provjera da nedozvoljeni elementi nisu vidljivi |
| **Responsivnost nije zadovoljena** – stranica se ne prikazuje ispravno na mobilnim uređajima ili tabletima | Osigurati prilagodljiv dizajn za različite veličine ekrana |

---

### 6.4 Procesni rizici
Procesni rizici se bave samom metodologijom rada, tokom testiranja i dokumentovanjem problema. Oni identifikuju uska grla u komunikaciji i organizaciji koja mogu dovesti do kašnjenja ili isporuke softvera koji ne ispunjava očekivanja korisnika.

| Rizik | Mjere prevencije i ublažavanja |
|-------|-------------------|
| **Kašnjenje u otkrivanju grešaka** – greške otkrivene prekasno u razvojnom ciklusu | Obavezno unit testiranje pri implementaciji; integracioni testovi odmah po spajanju modula |
| **Nedovoljna provjera NFR-ova** – performanse i sigurnost se testiraju samo u kasnim fazama | Uključiti NFR testove u sistemsko testiranje od ranijih sprintova; ne odlagati sigurnost za kraj |
| **Neadekvatna dokumentacija grešaka** – bug nije dovoljno opisan za brzu reprodukciju | Definisati standardizovan način opisivanja grešaka radi lakše reprodukcije i ispravke |
| **Preskakanje prihvatnog testiranja** – sistem je tehnički ispravan, ali ne odgovara potrebama korisnika | Planirati prihvatno testiranje sa definisanim scenarijima za sve ključne uloge |

---
