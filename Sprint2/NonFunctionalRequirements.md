# Non-Functional Requirements (NFR)

## Tabela nefunkcionalnih zahtjeva

| ID | Kategorija | Opis zahtjeva | Kako će se provjeravati | Prioritet | Napomena |
|:---:|---|---|---|:---:|---|
| NFR-1 | Performanse | Svaka stranica sistema mora se učitati u roku od 2 sekunde u normalnim uslovima rada | Manualno testiranje učitavanja stranica u pretraživaču uz mjerenje vremena odgovora u developer alatima | Visok | Odnosi se na stabilnu internet konekciju i mali broj istovremenih korisnika, ne uključuje inicijalno pokretanje servera |
| NFR-2 | Upotrebljivost | Sistem prikazuje razumljive poruke o grešci korisniku, bez tehničkih detalja ili interne informacije o grešci | Manualno testiranje rubnih slučajeva, unos neispravnih podataka, gubitak konekcije, pristup nepostojećim stranicama | Visok | Poruke moraju biti jasne, napisane na odgovarajućem jeziku i sadržavati prijedlog sljedećeg koraka za korisnika |
| NFR-3 | Upotrebljivost | Sistem traži potvrdu korisnika prije izvršavanja destruktivnih akcija, poput brisanja knjige ili korisničkog naloga | Manualno testiranje, pokušati obrisati entitet i provjeriti pojavljuje li se dijalog za potvrdu | Visok | Primjenjuje se na sve akcije koje su nepovratne |
| NFR-4 | Upotrebljivost | Sve interaktivne forme u sistemu moraju prikazivati poruke o validaciji direktno uz polje s greškom, a ne samo kao opću poruku na vrhu stranice | Manualno testiranje, unijeti neispravne podatke u svaku formu i provjeriti poziciju i jasnoću poruke greške | Srednji | Primjenjuje se na forme za prijavu, registraciju, dodavanje knjige i upravljanje pozajmicama |
| NFR-5 | Sigurnost | Pristup funkcionalnostima sistema ograničen je prema ulozi korisnika, član, bibliotekar i administrator imaju različite nivoe pristupa | Manualno testiranje, pokušati pristupiti zaštićenim dijelovima sistema kao korisnik s nižim ovlaštenjima i provjeriti da li je pristup odbijen | Visok | Provjera pristupa mora biti implementirana i na nivou korisničkog interfejsa i na nivou serverske logike |
| NFR-6 | Sigurnost | Lozinke korisnika se pohranjuju u hashiranom obliku, sistem nikada ne spašava lozinke u čitljivom tekstualnom obliku | Pregled baze podataka uz provjeru developer alata, provjeriti da lozinke nisu vidljive ni u odgovorima servera ni u bazi podataka | Visok | Koristiti standardni algoritam za hashiranje lozinki, lozinke se ne smiju pojavljivati ni u kakvim sistemskim zapisima |
| NFR-7 | Pouzdanost | Sistem mora čuvati konzistentnost podataka o pozajmicama, knjiga ne smije biti označena kao dostupna dok postoji aktivna pozajmica za tu knjigu | Manualno testiranje, kreirati pozajmicu i provjeriti da je status knjige promijenjen, pokušati kreirati drugu pozajmicu za istu knjigu | Visok | Ovo pravilo mora biti primijenjeno na nivou poslovne logike ili baze podataka, a ne samo na nivou korisničkog interfejsa, uz to postoji mogućnost duplikata knjige, više primjeraka iste |
| NFR-8 | Internacionalizacija | Sistem podržava prikaz sadržaja na bosanskom jeziku kao primarnom jeziku, uz mogućnost proširenja na dodatne jezike u budućim verzijama. | Manualno testiranje, pregled svih ekrana i provjera da su svi tekstovi prikazani na bosanskom jeziku | Srednji | Podrška za dodatne jezike nije planirana u MVP verziji |

---

## Legenda

### Prioritet
- **Visok** — mora biti ispunjeno za MVP
- **Srednji** — važno, ali nije preduvjet za prvu verziju
- **Nizak** — korisno proširenje ili poboljšanje za kasniju fazu