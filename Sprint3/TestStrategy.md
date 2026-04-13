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

| Nivo testiranja | Cilj | Alati | Frekvencija | Izlazni kriteriji (kriterij prihvatanja) | Ograničenja |
|-----------------|------|-------|-------------|-------------------|-------------|
| Unit testiranje | Provjera pojedinačnih funkcija i komponenti: validacija forme (registracija, prijava), provjera unosa (email, lozinka), logika autentifikacije, upravljanje statusom knjiga i primjeraka | xUnit (.NET), Visual Studio Test Explorer | Pri svakoj izmjeni koda | Svi testovi prolaze, osnovna logika ispravna | Ne može otkriti probleme koji nastaju u komunikaciji između modula, servisa ili baze podataka |
| Integraciono testiranje | Provjera komunikacije između komponenti: frontend - backend, API - baza podataka, autentifikacija i autorizacija, operacije nad knjigama i korisnicima | Postman, Mailtrap | Nakon integracije modula | API vraća ispravne odgovore, podaci konzistentni | Uzrok grešaka može biti teže izolirati jer uključuje više komponenti. |
| Sistemsko testiranje | Testiranje kompletnog sistema kroz end-to-end tokove: registracija, prijava, pregled kataloga, dodavanje i brisanje knjiga, zaduživanje i vraćanje | Ručno testiranje u pretraživaču, Developer Tools | Nakon integracionog testiranja | Svi ključni tokovi funkcionišu ispravno | Sistemsko testiranje je vremenski najzahtjevnije. Uzrok grešaka teže je izolirati jer uključuje cijeli sistem. |
| Prihvatno testiranje (UAT) | Provjera da sistem zadovoljava acceptance kriterije iz User Story-ja: validacija unosa, uspješna registracija, pravilno upravljanje knjigama i korisnicima| Ručno testiranje u pretraživaču | Prije produckije | Korisnici potvrđuju ispravnost sistema, nema kritičnih grešaka, sistem odobren za isporuku | Zavisi od subjektivne procjene korisnika i obično pokriva samo glavne funkcionalne scenarije, pa neke tehničke greške mogu ostati neotkrivene. |
| Regresiono testiranje | Provjera da postojeće funkcionalnosti (prijava, registracija, katalog, upravljanje knjigama i korisnicima) i dalje rade ispravno nakon izmjena ili dodavanja novih funkcionalnosti | Ručno testiranje / automatizovani testovi - NUnit | Nakon svake izmjene ili dodavanja nove funkcionalnosti | Nema regresija u postojećim funkcionalnostima | Može biti vremenski zahtjevno jer je potrebno ponovo testirati veći broj postojećih funkcionalnosti nakon svake izmjene. |

---

<br> 

## 3. Šta se testira u kojem nivou

Tabela ispod povezuje ključne funkcionalnosti bibliotečkog sistema sa nivoima testiranja. Fokus je na tome šta se provjerava na kojem nivou.

| Funkcionalnost/zahtjev | Unit | Integracijsko | Sistemsko | Sigurnosno | Performansno | Prihvatno |
|------------------------|------|---------------|-----------|------------|--------------|-----------|
| Registracija i prijava korisnika (US-01, US-02, US-03, US-04, US-05) | DA - validacija emaila, lozinke i obaveznih polja | DA - auth API + sesija + baza | DA - kompletan tok prijave i registracije | DA - zaštita ruta i generičke greške | DA - login < 2s (NFR-1) | DA - korisnik potvrđuje uspješan login |
| Upravljanje sesijom i RBAC (US-06, US-07, US-08, US-09) | NE | DA - kreiranje/brisanje sesije i zaštita ruta | DA - kontrola pristupa po ulogama | DA - neovlašten pristup blokiran (NFR-5) | NE | DA - potvrda ograničenja pristupa |
| Upravljanje knjigama (US-12, US-17, US-25) | DA - validacija unosa i poslovna pravila | DA - API + baza konzistentnost | DA - dodavanje, izmjena i brisanje kroz UI | DA - zabrana neovlaštenih akcija | NE | DA - bibliotekar potvrđuje tok |
| Upravljanje primjercima (US-21, US-23, US-24) | DA - statusi i validacija | DA - promjena statusa ↔ baza | DA - prikaz statusa kroz sistem | DA - zabrana nedozvoljenih akcija | NE | DA - potvrda tačnosti statusa |
| Katalog i pretraga (US-19, US-20, US-35, US-36) | DA - logika pretrage i filtera | DA - dohvat podataka iz baze | DA - pregled kataloga i paginacija | DA - korisnik vidi samo dozvoljene podatke | DA - učitavanje < 2s (NFR-1) | DA - korisnik potvrđuje preglednost |
| Zaduživanje i vraćanje knjiga (US-43, US-44, US-45, US-47) | DA - poslovna pravila (npr. dostupnost) | DA - inventar + zaduženje + audit log | DA - end-to-end tok zaduživanja | DA - zaštita pristupa operacijama | NE | DA - bibliotekar potvrđuje tok |
| Upravljanje članarinom (US-56, US-57, US-58, US-59) | DA - validacija datuma i statusa | DA - veza korisnik ↔ članarina | DA - blokiranje zaduživanja | NE | NE | DA - korisnik vidi tačan status |
| Rezervacije (US-69, US-72, US-79, US-80) | DA - pravila rezervacije | DA - rezervacije ↔ katalog | DA - tok rezervacije i otkazivanja | DA - zabrana nedozvoljenih akcija | NE | DA - korisnik potvrđuje tok |
| Upravljanje kategorijama (US-30, US-33, US-34) | DA - validacija naziva i pravila | DA - baza + knjige | DA - prikaz i upravljanje | NE | NE | DA - bibliotekar potvrđuje |
| Upravljanje korisnicima (admin) (US-49, US-50, US-51, US-52, US-53) | NE | DA - API + promjene uloga | DA - tok upravljanja korisnicima | DA - RBAC pravila (NFR-5) | NE | DA - admin potvrđuje |
| Email notifikacije (US-81, US-82, US-83, US-84) | NE | DA - email servis + događaji | DA - slanje u realnim scenarijima | NE | NE | DA - korisnik prima obavijesti |
| Audit log (NFR-11) | NE | DA - zapis akcija u bazu | DA - provjera kroz scenarije | DA - evidencija sigurnosnih akcija | NE | DA - admin potvrđuje zapis |
| Validacija unosa i UX (NFR-2, NFR-3, NFR-4) | DA - validacione funkcije | DA - validacija kroz API | DA - prikaz grešaka u UI | NE | NE | DA - korisnik razumije poruke |
| Sigurnost sistema (NFR-5, NFR-6) | DA - hashiranje lozinki | DA - zaštita endpointa | DA - provjera kroz tokove | DA - potpuna sigurnosna provjera | NE | DA - nema sigurnosnih propusta |
| Performanse sistema (NFR-1) | NE | DA - odziv API-ja | DA - brzina sistema u scenarijima | NE | DA - mjerenje performansi | DA - zadovoljeni NFR |
| Internacionalizacija i upotrebljivost (NFR-8) | NE | NE | DA - prikaz jezika kroz UI | NE | NE | DA - korisnik potvrđuje razumljivost |

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

| Rizik | Vjerovatnoća | Utjecaj | Mjere ublažavanja |
|-------|:------------:|:-------:|-------------------|
| **Nedovoljna pokrivenost testovima** – nisu obuhvaćeni svi rubni scenariji funkcionalnosti | Srednja | Visok | Redovni pregled test slučajeva pri svakom sprintu; review acceptance kriterija |
| **Kašnjenje u otkrivanju grešaka** – greške otkrivene tek u sistemskom testiranju | Srednja | Visok | Obavezno unit testiranje pri implementaciji svake komponente |
| **Nekonzistentnost podataka pri paralelnim zahtjevima** – dva korisnika istovremeno zadužuju isti primjerak | Niska | Kritičan | Transakcije i zaključavanje na nivou baze podataka (IT-15) |
| **Propusti u kontroli pristupa** – korisnik s nižim privilegijama pristupa zabranjenim funkcijama | Niska | Kritičan | RBAC testiranje i na UI i na API nivou (IT-19 do IT-22); sigurnosni review koda |
| **Nestabilno testno okruženje** – rezultati testiranja ne odgovaraju produkcijskom okruženju | Srednja | Srednji | Što je moguće više uskladiti konfiguraciju testnog i produkcijskog okruženja |
| **Regresione greške** – ispravka jedne greške uzrokuje novu u drugom dijelu sistema | Srednja | Visok | Ponavljanje ključnih test slučajeva pri svakoj novoj verziji |
| **Nedovoljna provjera nefunkcionalnih zahtjeva** – performanse i sigurnost se ne testiraju do kasnih sprintova | Visoka | Srednji | Uključiti NFR testove (ST-NF-01 do ST-NF-09) u sistemsko testiranje od Sprinta 8 |
| **Tehnički dug koji otežava testiranje** – kod koji nije modularan teško je testirati izolovano | Srednja | Srednji | Modularna arhitektura (NFR-9); redovni code review |
| **Neadekvatna dokumentacija grešaka** – bug nije dovoljno opisan za reprodukciju | Srednja | Srednji | Obavezan format bug prijave (Bug ID, koraci, očekivano/stvarno ponašanje) |
| **Preskakanje prihvatnog testiranja** – sistem prolazi tehničke testove ali ne zadovoljava korisnička očekivanja | Niska | Visok | UAT scenariji planirani od Sprinta 11 uz uključivanje finalnih korisnika |

---
