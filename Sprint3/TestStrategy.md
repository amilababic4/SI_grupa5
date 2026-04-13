# Test Strategy 

## 1. Cilj testiranja

Testiranje Bibliotečkog informacionog sistema provodi se s jasnom namjerom: osigurati da svaka implementirana funkcionalnost — od registracije člana do rezervacije knjige — radi ispravno, sigurno i u skladu s definisanim zahtjevima.

| Cilj | Zašto je važan |
|------|-------------------|
| **Ispravnost poslovne logike** | Knjiga se ne smije označiti kao dostupna dok postoji aktivno zaduženje (NFR-7). Primjerak se ne smije deaktivirati dok je zadužen (US-24). Rezervacija nije dozvoljena sa dostupnim primjercima knjige (US-769). |
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
###  Definicija uspjeha

Testiranje se smatra uspješnim kada su ispunjeni sljedeći uvjeti:

- Svi ključni poslovni procesi sistema funkcionišu ispravno i bez blokirajućih grešaka
- Ne postoji niti jedan otvoreni bug s oznakom **Kritično** ili **Visoko** pred završnu demonstraciju
- UAT scenariji su prošli iz perspektive sva tri tipa korisnika (Član, Bibliotekar, Administrator)
- Sistem zadovoljava definisane nefunkcionalne zahtjeve (performanse, sigurnost, dostupnost i validacija unosa)

<br>

## 2. Nivoi testiranja

Različiti nivoi testiranja omogućavaju provjeru sistema od pojedinačnih funkcija i komponenti, preko njihove međusobne integracije, pa sve do provjere cjelokupnog rada sistema iz perspektive krajnjeg korisnika. Na ovaj način se osigurava da implementirane funkcionalnosti zadovoljavaju definisane zahtjeve i da sistem radi očekivano u realnim scenarijima korištenja.


---

### 2.1 Unit testiranje

**Cilj:**  
Provjera pojedinačnih funkcija i komponenti: validacija forme (registracija, prijava), provjera unosa (email, lozinka), logika autentifikacije, upravljanje statusom knjiga i primjeraka.

**Alati:**  
xUnit (.NET), Visual Studio Test Explorer.

**Izlazni kriteriji (kriterij prihvatanja):**  
Svi testovi prolaze, osnovna logika ispravna.

**Ograničenja:**  
Ne može otkriti probleme koji nastaju u komunikaciji između modula, servisa ili baze podataka.


---

### 2.2 Integracijsko testiranje

**Cilj:**  
Provjera komunikacije između komponenti: frontend – backend, API – baza podataka, autentifikacija i autorizacija, operacije nad knjigama i korisnicima.

**Alati:**  
Postman, Mailtrap.


**Izlazni kriteriji:**  
API vraća ispravne odgovore, podaci konzistentni.

**Ograničenja:**  
Uzrok grešaka može biti teže izolirati jer uključuje više komponenti.


---

### 2.3 Sistemsko testiranje

**Cilj:**  
Testiranje kompletnog sistema kroz end-to-end tokove: registracija, prijava, pregled kataloga, dodavanje i brisanje knjiga, zaduživanje i vraćanje. Obuhvatiti funkcionalne i nefunkcionalne aspekte (performanse, sigurnost, upotrebljivost).  

**Alati:**  
Ručno testiranje u pretraživaču, Developer Tools.

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
Ručno testiranje / automatizovani testovi – NUnit.

**Izlazni kriteriji:**  
Nema regresija u postojećim funkcionalnostima.

**Ograničenja:**  
Može biti vremenski zahtjevno jer je potrebno ponovo testirati veći broj postojećih funkcionalnosti nakon svake izmjene.

---

### 2.6 UI testiranje

**Cilj:**  
Provjera korisničkog interfejsa: ispravnost prikaza formi, poruka grešaka uz odgovarajuće polje, navigacija po ulogama, responsivnost na različitim rezolucijama, konzistentnost elemenata kroz pretraživače (Chrome, Firefox, Edge).

**Alati:**  
Ručno testiranje u pretraživaču, Browser DevTools, Selenium.

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
Moguć velik broj lažne pozitivne rezultate (false positives) koje je potrebno ručno verifikovati, što produžava vrijeme analize.

---
<br>
Kombinacija svih navedenih nivoa testiranja osigurava visok kvalitet softvera, smanjuje rizik od otkrivanja ozbiljnih grešaka u produkciji i povećava povjerenje krajnjih korisnika u ispravnost sistema.

<br>

---
<br> 

## 3. Šta se testira u kojem nivou

Tabela ispod povezuje ključne funkcionalnosti bibliotečkog sistema sa nivoima testiranja. Fokus je na tome šta se provjerava na kojem nivou.

| Funkcionalnost/zahtjev | Unit | Integracijsko | Sistemsko | Prihvatno (UAT) | Regresiono | UI | Penetracijsko |
|------------------------|------|---------------|-----------|----------------|------------|----|----------------|
| **Registracija i prijava korisnika** (US-01, US-02, US-03, US-04, US-05) | DA - validacija emaila, lozinke i obaveznih polja | DA - auth API + sesija + baza | DA - kompletan tok prijave i registracije | DA - korisnik potvrđuje uspješan login | DA - provjera nakon izmjena | DA - prikaz formi i grešaka | DA - SQL injection, XSS na login poljima |
| **Upravljanje sesijom i RBAC** (US-06, US-07, US-08, US-09) | NE | DA - kreiranje/brisanje sesije i zaštita ruta | DA - kontrola pristupa po ulogama | DA - potvrda ograničenja pristupa | DA - nakon dodavanja novih uloga | NE | DA - testiranje pristupa bez tokena, promjene role kroz zahtjev |
| **Upravljanje knjigama** (US-12, US-17, US-25) | DA - validacija unosa i poslovna pravila | DA - API + baza konzistentnost | DA - dodavanje, izmjena i brisanje kroz UI | DA - bibliotekar potvrđuje tok | DA - nakon svake CRUD izmjene | DA - prikaz forme, dugmad, poruke | DA - neovlašteni korisnik ne može mijenjati knjige |
| **Upravljanje primjercima** (US-21, US-23, US-24) | DA - statusi i validacija | DA - promjena statusa ↔ baza | DA - prikaz statusa kroz sistem | DA - potvrda tačnosti statusa | DA - nakon izmjene statusa | DA - vizuelni prikaz statusa | DA - zabrana nedozvoljenih akcija preko API |
| **Katalog i pretraga** (US-19, US-20, US-35, US-36) | DA - logika pretrage i filtera | DA - dohvat podataka iz baze | DA - pregled kataloga i paginacija | DA - korisnik potvrđuje preglednost | DA - nakon izmjene indeksa pretrage | DA - responzivnost tabele, filteri | DA - korisnik vidi samo dozvoljene podatke (ne osjetljive) |
| **Zaduživanje i vraćanje knjiga** (US-43, US-44, US-45, US-47) | DA - poslovna pravila (npr. dostupnost) | DA - inventar + zaduženje + audit log | DA - end-to-end tok zaduživanja | DA - bibliotekar potvrđuje tok | DA - nakon promjene pravila zaduživanja | DA - prikaz dugmadi samo za prijavljene | DA - testiranje višestrukog zaduživanja istog primjerka |
| **Upravljanje članarinom** (US-56, US-57, US-58, US-59) | DA - validacija datuma i statusa | DA - veza korisnik ↔ članarina | DA - blokiranje zaduživanja | DA - korisnik vidi tačan status | DA - nakon produženja članarine | DA - prikaz statusa u profilu | NE |
| **Rezervacije** (US-69, US-72, US-79, US-80) | DA - pravila rezervacije | DA - rezervacije ↔ katalog | DA - tok rezervacije i otkazivanja | DA - korisnik potvrđuje tok | DA - nakon izmjene logike rezervacija | DA - prikaz liste rezervacija | DA - zabrana rezervacije bez aktivne članarine |
| **Upravljanje kategorijama** (US-30, US-33, US-34) | DA - validacija naziva i pravila | DA - baza + knjige | DA - prikaz i upravljanje | DA - bibliotekar potvrđuje | DA - nakon dodavanja nove kategorije | DA - prikaz u padajućem meniju | NE |
| **Upravljanje korisnicima (admin)** (US-49, US-50, US-51, US-52, US-53) | NE | DA - API + promjene uloga | DA - tok upravljanja korisnicima | DA - admin potvrđuje | DA - nakon dodavanja novog admina | DA - tabela korisnika, dugmad za akcije | DA - testiranje eskalacije privilegija |
| **Email notifikacije** (US-81, US-82, US-83, US-84) | NE | DA - email servis + događaji | DA - slanje u realnim scenarijima | DA - korisnik prima obavijesti | DA - nakon izmjene template-a | NE | NE |
| **Audit log** (NFR-11) | NE | DA - zapis akcija u bazu | DA - provjera kroz scenarije | DA - admin potvrđuje zapis | DA - nakon svake akcije | NE | DA - log se ne može brisati od strane običnog korisnika |
| **Validacija unosa i UX** (NFR-2, NFR-3, NFR-4) | DA - validacione funkcije | DA - validacija kroz API | DA - prikaz grešaka u UI | DA - korisnik razumije poruke | DA - nakon izmjene frontenda | DA - poruke tačno uz polja | NE |
| **Sigurnost sistema** (NFR-5, NFR-6) | DA - hashiranje lozinki | DA - zaštita endpointa | DA - provjera kroz tokove | DA - nema sigurnosnih propusta | DA - nakon dodavanja novih ruta | NE | DA - puni pentest: SQLi, XSS, CSRF, JWT, RBAC bypass |

<br>

## 4. Veza sa acceptance kriterijima

Ova sekcija prikazuje kako se acceptance kriteriji iz dokumenta *Set of User Stories* mapiraju na nivoe testiranja i koje artefakte koristimo kao dokaz njihovog ispunjenja. 

| Referenca | Ključni acceptance kriterij | Nivoi verifikacije | Dokaz ispunjenja |
|-----------|---------------------------|-------------------|------------------|
| US-01, US-02, US-03 | Registracija korisnika uspješna uz validne podatke; neispravan unos se odbija uz jasnu poruku | Unit, Integracijsko, Sistemsko, Prihvatno | Validacija forme (unit), API odgovor (Postman), demo registracije |
| US-04, US-05 | Prijava uspješna uz tačne kredencijale; pogrešni podaci vraćaju generičku poruku | Unit, Integracijsko, Sistemsko, Sigurnosno, Prihvatno | CI testovi za login, API provjera autentifikacije, demo login toka |
| US-08, US-09 | Korisnik vidi samo funkcionalnosti dozvoljene njegovoj ulozi (RBAC) | Integracijsko, Sistemsko, Sigurnosno, Prihvatno | API testovi (403), provjera UI prikaza po ulozi, demo |
| US-12, US-13 | Knjiga se može dodati samo uz validne podatke; ISBN mora biti ispravan i jedinstven | Unit, Integracijsko, Sistemsko, Prihvatno | Unit validacija, API zapis dodavanja knjige, demo u UI |
| US-21, US-23, US-24 | Status primjerka ispravno se mijenja; zadužen primjerak se ne može deaktivirati | Unit, Integracijsko, Sistemsko, Prihvatno | Testovi poslovne logike, zapis promjene statusa, audit log |
| US-24, US-25, US-28 | Knjiga se ne može obrisati ako ima aktivna zaduženja | Unit, Integracijsko, Sistemsko, Prihvatno | Validacija pravila, API odgovor, demo brisanja |
| US-30, US-33 | Kategorija mora imati jedinstven naziv; ne može se obrisati ako sadrži knjige | Unit, Integracijsko, Sistemsko, Prihvatno | Unit testovi validacije, API zapis, UI provjera |
| US-35, US-36, US-40 | Katalog prikazuje tačne podatke i omogućava pretragu i filtriranje | Unit, Integracijsko, Sistemsko, Performansno, Prihvatno | Testovi filtera, API pretraga, mjerenje vremena odziva |
| US-43, US-44, US-45 | Zaduživanje moguće samo uz aktivnu članarinu; status knjige se ažurira | Integracijsko, Sistemsko, Prihvatno | API zapis zaduživanja, promjena statusa primjerka, demo |
| US-47 | Vraćanje knjige ažurira status i uklanja aktivno zaduženje | Integracijsko, Sistemsko, Prihvatno | Evidencija vraćanja, promjena statusa u bazi |
| US-56, US-57, US-58 | Status članarine (Aktivna/Istekla) tačno se prikazuje i utiče na zaduživanje | Unit, Integracijsko, Sistemsko, Prihvatno | Validacija datuma, API odgovor, UI prikaz statusa |
| US-62, US-63, US-64 | Korisnik vidi svoja zaduženja i historiju bez pristupa tuđim podacima | Integracijsko, Sistemsko, Sigurnosno, Prihvatno | API filtriranje po korisniku, UI prikaz, demo |
| US-69, US-72, US-80 | Rezervacija moguća samo kad nema dostupnih primjeraka; automatsko otkazivanje po isteku | Unit, Integracijsko, Sistemsko, Prihvatno | Testovi pravila rezervacije, API zapis, simulacija isteka |
| US-81, US-82, US-83, US-84 | Email notifikacije se šalju u odgovarajućim situacijama | Integracijsko, Sistemsko, Prihvatno | Mailtrap evidencija, log slanja, demo |
| US-49, US-50, US-51, US-52 | Administrator može upravljati korisnicima uz poštivanje RBAC pravila | Integracijsko, Sistemsko, Sigurnosno, Prihvatno | API zapis izmjena, provjera privilegija, demo |
| NFR-1 | Sistem odgovara unutar 2 sekunde za ključne operacije | Performansno, Sistemsko, Prihvatno | JMeter izvještaj, mjerenje odziva |
| NFR-2, NFR-4 | Poruke grešaka i validacije su jasne i prikazane uz odgovarajuće polje | Unit, Sistemsko, Prihvatno | UI provjera, validacija forme |
| NFR-5 | Neovlašten pristup je blokiran na API nivou | Integracijsko, Sigurnosno, Sistemsko, Prihvatno | API testovi (401/403), sigurnosni logovi |
| NFR-6 | Lozinke su hashirane i nisu dostupne u plain-text obliku | Unit, Integracijsko, Sigurnosno | Pregled baze, test hashiranja |
| NFR-7 | Sistem održava konzistentnost podataka (status knjiga, zaduženja) | Unit, Integracijsko, Sistemsko | Testovi poslovne logike, audit log |
| NFR-8 | Sistem je u potpunosti na bosanskom jeziku | Sistemsko, Prihvatno | UI provjera |
| NFR-11 | Sve ključne akcije su evidentirane u audit logu | Integracijsko, Sistemsko, Sigurnosno | Audit log zapis i provjera |

<br>

## 5. Način evidentiranja rezultata testiranja

### 5.1 Evidencija bug prijava

Svaka pronađena greška evidentira se kao bug izvještaj koji sadrži sljedeće informacije:

| Polje | Opis |
|-------|------|
| **Bug ID** | Jedinstveni identifikator (npr. BUG-001) |
| **Naziv** | Kratak opisni naslov greške |
| **Test Case** | ID test slučaja koji je otkrio grešku |
| **User Story** | Povezani US (npr. US-02) |
| **Opis** | Detaljan opis pronađene greške |
| **Koraci za reprodukciju** | Precizni koraci kojima se greška može reproducirati |
| **Očekivano ponašanje** | Šta bi sistem trebao uraditi |
| **Stvarno ponašanje** | Šta sistem zapravo radi |
| **Ozbiljnost** | Kritično / Visoko / Srednje / Nisko |
| **Prioritet popravke** | Visok / Srednji / Nizak |
| **Status** | Otvoreno / U rješavanju / Riješeno / Zatvoreno |
| **Datum prijave** | Datum kada je bug evidentiran |
| **Datum rješavanja** | Datum kada je bug ispravljen |

<br>

## 6. Glavni rizici kvaliteta
U nastavku, navedeni su neki od potencijalnih rizika i podijeljeni su u četiri kategorije: funkcionalni, sigurnosni, UI/UX i procesni. Svaki rizik sadrži i preporučene mjere mitigacije.
### 6.1 Funkcionalni rizici
Funkcionalni rizici se odnose na ispravnost rada sistema u skladu sa definisanim poslovnim zahtjevima. Fokus ove kategorije je na preciznosti obrade podataka, pokrivanju specifičnih "rubnih" slučajeva i održavanju stabilnosti sistema kroz regresiono testiranje.

| Rizik | Mjere mitigacije |
|-------|-------------------|
| **Nekonzistentnost podataka pri paralelnim zahtjevima** – dva korisnika istovremeno zadužuju isti primjerak knjige | Transakcije i zaključavanje na nivou baze podataka (IT-15); testirati scenarij s dva paralelna zahtjeva |
| **Pogrešan prikaz statusa članarine** – status prikazan kao aktivan iako je istekao, što omogućava zaduživanje | Unit testovi za granične datume (dan isteka, dan poslije); integracijsko testiranje s realnim datumima |
| **Regresione greške** – ispravka jedne greške uzrokuje novu u drugom dijelu sistema | Smoke testiranje nakon svakog deploya; regresioni test slučajevi za sve ključne tokove |

---

### 6.2 Sigurnosni rizici
Sigurnosni rizici obuhvataju potencijalne prijetnje integritetu podataka i privatnosti korisnika. Identifikacija ovih rizika osigurava zaštitu od zlonamjernih napada, neovlaštenog pristupa osjetljivim informacijama i osigurava stabilnost autorizacijskih mehanizama.

| Rizik | Mjere mitigacije |
|-------|-------------------|
| **Propusti u kontroli pristupa (RBAC)** – korisnik s nižim privilegijama direktnim API pozivom pristupa zabranjenim resursima | RBAC testiranje i na UI i na API nivou; penetracijsko testiranje autorizacijskih ruta |
| **SQL Injection** – zlonamjerni unos u polja za pretragu prosljeđuje se direktno u SQL upit | Penetracijsko testiranje i parametrizovani SQL upiti 
| **Brute force napad na prijavu** – automatizovano pogađanje lozinki bez zaštite | Testirati odgovor sistema na 50+ uzastopnih neuspjelih pokušaja; postaviti limit uzastopnih pokušaja prijave |
| **Neispravno upravljanje sesijom** – sesija ostaje aktivna nakon odjave ili token ne ističe | Testirati odjavu i provjeru da token više ne funkcioniše; testirati pristup zaštićenoj ruti sa starim tokenom |

---

### 6.3 UI/UX rizici
UI/UX rizici su fokusirani na interakciju korisnika sa sistemom. Cilj je osigurati intuitivno iskustvo, vizuelnu konzistentnost na različitim uređajima i preglednicima, te spriječiti korisničke greške kroz jasne povratne informacije.

| Rizik | Mjere mitigacije |
|-------|-------------------|
| **Nekonzistentan prikaz u pretraživačima** – razlike u prikazu između Chrome, Firefox i Edge pretraživača | UI testiranje u Chrome, Firefox i Edge pretraživačima za sve ključne ekrane |
| **Destruktivne akcije bez potvrde** – slučajno brisanje korisnika ili knjige bez dijaloga za potvrdu | UI provjera svih akcija brisanja; testirati da se dijalog prikazuje i da otkazivanje zaustavlja akciju |
| **RBAC nije reflektovan u UI** – korisnik vidi opcije koje nema pravo izvršiti, što vodi do tehničkih grešaka | UI testiranje za svaku ulogu (Član, Bibliotekar, Administrator); provjera da nedozvoljeni elementi nisu vidljivi |
| **Responsivnost nije zadovoljena** – stranica se ne prikazuje ispravno na mobilnim uređajima ili tabletima | Provjera ključnih ekrana u DevTools Responsive Modu (375px, 768px, 1280px) |

---

### 6.4 Procesni rizici
Procesni rizici se bave samom metodologijom rada, tokom testiranja i dokumentovanjem problema. Oni identifikuju uska grla u komunikaciji i organizaciji koja mogu dovesti do kašnjenja ili isporuke softvera koji ne ispunjava očekivanja korisnika.

| Rizik | Mjere mitigacije |
|-------|-------------------|
| **Kašnjenje u otkrivanju grešaka** – greške otkrivene prekasno u razvojnom ciklusu | Obavezno unit testiranje pri implementaciji; integracioni testovi odmah po spajanju modula |
| **Nedovoljna provjera NFR-ova** – performanse i sigurnost se testiraju samo u kasnim fazama | Uključiti NFR testove u sistemsko testiranje od ranijih sprintova; ne odlagati sigurnost za kraj |
| **Neadekvatna dokumentacija grešaka** – bug nije dovoljno opisan za brzu reprodukciju | Obavezan format bug prijave; ne zatvarati bug bez priloženog dokaza o ispravci |
| **Preskakanje prihvatnog testiranja** – sistem je tehnički ispravan, ali ne odgovara potrebama korisnika | UAT scenariji planirani unaprijed uz uključivanje finalnih korisnika za sve uloge |
<br>

*Ovaj dokument predstavlja Test Strategy za Sprint 3. Detalji implementacije test slučajeva, kao i rezultati testiranja, bit će razrađeni i evidentirani u sklopu kasnijih sprintova (Sprint 8 – Sprint 11) kako sistem bude implementiran.*


---
