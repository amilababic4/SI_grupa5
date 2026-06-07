# Final AI Usage Summary — SmartLib

## Opis dokumenta

Ovaj dokument predstavlja završni sažetak korištenja AI alata tokom cjelokupnog razvoja SmartLib sistema, obuhvatajući sprintove 5, 6, 7, 8, 9 i 10. 

> Ukupno evidentirano AI interakcija: **21**  
> Sprintovi: **Sprint 5, 6, 7, 8, 9, 10**

---

## 1. Pregled korištenih AI alata

| Alat                    | Broj korištenja |
| ----------------------- | --------------- |
| Claude                  | 9               |
| Claude Code             | 7               |
| GitHub Copilot          | 1               |
| ChatGPT                 | 1               |
| Codex                   | 1               |
| Cursor                  | 1               |
| Claude + GitHub Copilot | 1               |

---

## 2. Za šta je AI korišten
 
AI alati su korišteni u sljedećim domenama razvoja:
 
**Backend implementacija:**
Autentifikacija i JWT zaštita ruta, kreiranje korisničkih naloga, implementacija baze podataka putem Entity Framework-a, CRUD operacije za knjige, kategorije i primjerke, logika brisanja i soft-delete pristup, sistem zaduživanja knjiga, upravljanje članarinama, audit log sistem, integracija sa eksternim distributerom knjiga, te sistem napredne pretrage i filtera.
 
**Frontend i korisnički interfejs:**
Redizajn naslovne stranice, poboljšanje layouta i navigacije, vizualni indikatori statusa (članarina, deaktivacija primjeraka), forma za online produženje članarine i forma za slanje nabavnih zahtjeva.
 
**Testiranje:**
Unit testovi s XUnit i Moq frameworkom, integracijsko testiranje s WebApplicationFactory, automatizirani UI testovi putem Playwright-a, te penetracijski i sigurnosni testovi.
 
**Izvještaji i analitika:**
Generisanje mjesečnih izvještaja, PDF eksport, vizualizacija statistike putem Chart.js, te implementacija filtera po periodu.
 
---
 
## 3. Šta je tim prihvatio
 
- Većinu backend i frontend logike za autentifikaciju i zaštitu ruta
- Dio generisanog koda za kreiranje korisničkih naloga, prilagođen postojećoj arhitekturi
- Dijelove koda ključne za implementaciju baze podataka
- Implementaciju za knjige i katalog koja se uklapala u postojeću strukturu projekta
- Princip da se brisanje mora uklopiti u postojeći sistem i da obrisane ili neaktivne knjige ne smiju biti prikazane korisnicima u katalogu
- Ideju modernijeg i profesionalnijeg dizajna za rad sa knjigama
- Implementaciju sa asinhronim Task-ovima, korištenje TempData za prikaz poruka o uspjehu/grešci, strogu validaciju povezanosti primjerka sa knjigom, te AI predloženu automatizaciju inventarnih brojeva u formatu `INV-{Id}-{Broj}`
- Strukturu akcija za dobavljanje podataka kategorije
- Arhitekturu unit testova koja striktno odvaja Web i API kontrolere, te upotrebu Mock objekata
- Kompletnu infrastrukturu za integracijsko testiranje i pattern gdje testovi kreiraju vlastite resurse umjesto zajedničkog seed sadržaja
- Playwright kao alat za UI testiranje
- Osnovnu logiku validacije i tok kreiranja zaduženja, uključujući provjeru dostupnosti primjerka i povezivanje zaduženja sa članom i bibliotekarom
- Logiku validacije datuma, prikaz statusa članarine kroz jasne vizuelne oznake, te integraciju funkcionalnosti članarine direktno unutar profila korisnika
- Kompletnu strukturu Playwright testova i pristup organizaciji testnih helper metoda
- Implementaciju forme za izbor mjeseca i godine, logiku filtriranja podataka po periodu, generisanje PDF izvještaja i korištenje Chart.js grafikona
- Implementaciju kombinovanih filtera i logiku dinamičkog filtriranja knjiga po više kriterija, te pristup sa inline filter formom
- Kompletnu strukturu modela za produženje članarine, logiku izračuna novog datuma isteka, live preview pri odabiru trajanja i historiju zahtjeva s razlogom odbijanja vidljivom članu
- Kompletnu strukturu repozitorija za audit log, audit log pozive u svim CRUD akcijama, vlastiti AuditLog.cshtml s Base64 enkodiranjem JSON vrijednosti u `data-` atribute i JavaScript diff prikazom, te UTC+2 fiksni offset
- Kompletnu strukturu modela, repozitorija, kontrolera i viewa za integraciju s distributerom; pristup kreiranja tabela kroz `ExecuteSqlRaw` u `Program.cs`; email adresu distributera u `AppPostavke` tabeli s mogućnošću izmjene direktno kroz UI
---
 
## 4. Šta je tim izmijenio
 
- Prilagođen role-based pristup u autentifikacijskom sistemu
- Ispravljene greške u vezama između entiteta u bazi podataka
- Prilagođene putanje u `RedirectToAction` metodama kako bi se korisnik uvijek vraćao na Details stranicu matične knjige
- Zadržane samo izmjene za knjige i katalog direktno vezane za Sprint 6; posebno provjereno da implementacija ne narušava postojeće funkcionalnosti
- Prilagođena implementacija brisanja knjiga postojećem modelu projekta, posebno u dijelu gdje se vodi računa o aktivnim i neaktivnim zapisima
- Ograničene izmjene korisničkog interfejsa samo na relevantne dijelove sistema
- Dodani dijelovi za čišćenje praznih stringova u kategorijama; prilagođen prikaz validacijskih poruka
- Ručno dopunjeni unit testovi za granične slučajeve, s preciznim Assert provjerama za TempData poruke
- Usklađeni nazivi polja u seederima sa stvarnim imenima u modelu projekta; uklonjeni dijagnostički testovi iz finalne verzije test suite-a
- Usklađeni očekivani URL patterne i tekstovi headinga u Playwright testovima sa stvarnom implementacijom routinga i Razor View-ova; prilagođeni emailovi i lozinke kroz centralizovane `UiTestSettings`
- Prilagođeni nazivi metoda, rute i View modeli za zaduživanje postojećoj strukturi SmartLib projekta; prilagođene poruke korisniku
- Prilagođeni nazivi ruta, View modela i prikaz statusnih oznaka za članarine postojećem dizajnu; format prikaza datuma usklađen na `DD.MM.YYYY`
- Usklađeni tekstovi headinga, dugmadi, validacijskih poruka i URL patterna u Playwright testovima sa stvarnim Razor View implementacijama i TempData porukama; zamijenjeni `GetByLabel` selektori direktnim CSS locatorima (`#naziv-novi`, `input[name='Naslov']`, `select[name='KategorijaId']`)
- Prilagođen izgled PDF dokumenta, ručno korigovani stilovi i raspored elemenata, optimizovan prikaz grafikona i tabela
- Prilagođen izgled filter sekcije, optimizovan raspored filter polja i prilagođene validacione i statusne poruke
- Viewovi za produženje članarine vizualno prilagođavani kroz više iteracija koristeći postojeće CSS komponente (`izvjestaj-card`, `prod2-*` klase)
- Korišten engleski naziv akcija (`CREATE`, `UPDATE`, `DELETE`) umjesto bosanskog u audit logu; view prilagođen da koristi `audit-*` CSS klase umjesto `al-*` klasa koje je AI generisao u kasnijim iteracijama
- Property `PodnosiocId` preimenovan u `PodnosilacId` radi konzistentnosti s bosanskim jezičkim pravilima
- Ručno analizirani i ispravljeni nezatvoreni CSS blokovi u `site.css` nastali iz merge konflikata između grana
---
 
## 5. Šta je tim odbacio
 
- Dijelove generisanog koda koji nisu odgovarali strukturi projekta
- Neodgovarajuće prijedloge za kreiranje korisničkih naloga
- Drugi dizajn naslovne stranice
- Nepotrebne foreign key tagove u modelima
- Ideje i izmjene koje bi zahtijevale veći refactoring, promjenu postojećih naziva ili proširenje funkcionalnosti izvan planiranog opsega Sprinta 6
- Agresivno brisanje koje bi moglo narušiti povezane podatke, posebno podatke o primjercima knjige ili drugim zavisnim entitetima
- Ideje koje bi aplikaciju učinile previše generičkom ili vizuelno prenatrpanom: pretjerani gradijenti, nasumične slike, previše animacija, neusklađene ikone i elementi koji bi izgledali kao automatski generisan dizajn
- Ideju o ručnom unosu inventarnog broja od strane korisnika
- Prijedloge koji su zahtijevali kompleksne view modele za jednostavan CRUD
- Prijedloge za generisanje nasumičnih testnih podataka
- Testove koji su direktno provjeravali stanje baze podataka umjesto HTTP odgovora
- Upotrebu Seleniuma kao alata za UI testiranje u korist Playwrighta, koji pruža moderniji API, bolju podršku za asinhrono izvršavanje i stabilnije testove kroz .NET okruženje
- Testove koji su samo ponavljali provjere iz integracijskim testova — provjere 401 bez tokena i 403 za eskalaciju privilegija
- Prijedloge koji su uvodili nepotrebno složene statuse zaduženja ili dodatne tokove koji nisu bili dio planiranog opsega Sprinta 7
- Prijedloge koji su uvodili kompleksniji sistem nivoa članstva i automatskog obračuna članarina
- `GetByLabel` pristup za forme čiji inputi nisu eksplicitno povezani sa label elementima; navigaciju klikom na `a.stretched-link` elemente jer ih Playwright tretira kao nevidljive zbog nedostatka dimenzija
- Prijedloge koji su zahtijevali kompleksnije biblioteke za generisanje PDF dokumenata
- Pristup sa posebnim modalnim prozorom za filtere
- Model A (automatsko produženje članarine); implementaciju notifikacija pri obradi zahtjeva za produženje
- Kasniji viewovi koje je AI generisao za audit log jer je tim već imao vlastiti funkcionalni view; `TimeZoneInfo` helper zamijenjen fiksnim UTC+2 offsetom
- Info panel "Kako funkcioniše" iz desne kolone viewa za nabavku — zamijenjen prikazom historije zadnjih nabavki i panelom za upravljanje email adresom distributera
---
 
## 6. Greške koje je AI napravio
 
**Greška 1 — Pogrešne veze između entiteta (Sprint 5, AI-04)**
Tokom implementacije baze podataka uočene su greške u vezama između entiteta koje je AI predložio, što je zahtijevalo ručnu korekciju.
 
**Greška 2 — Neispravni HTTP status kod za dupli ISBN (Sprint 6, AI-10)**
Tokom pisanja unit testova otkriveno je da API kontroler za knjige nije vraćao ispravan status kada se pokuša unijeti isti ISBN, već je bacao internu grešku.
 
**Greška 3 — Previše detaljne poruke o grešci pri prijavi (Sprint 6, AI-10)**
Otkriveno je da je sistem vraćao previše detaljne poruke o grešci pri prijavi, što je korigovano na generičke poruke radi povećanja sigurnosti.
 
**Greška 4 — Nedostajuće registracije zavisnosti u Program.cs (Sprint 6, AI-11)**
Tokom implementacije integracijskog testiranja otkriveno je da kontroler nije mogao biti aktiviran jer su nedostajale registracije zavisnosti u `Program.cs`, što je uzrokovalo grešku na svim endpointima.
 
**Greška 5 — Odjava nedostupna na početnoj stranici (Sprint 6, AI-12)**
Uočeno je da odjava nije dostupna na početnoj stranici već samo na ostalim stranicama aplikacije, pa je test za odjavu morao navigirati na `/Knjiga` prije nego što klikne dugme za odjavu.
 
**Greška 6 — Preklapanje sigurnosnih i integracijskim testova (Sprint 6, AI-13)**
Uočeno je da je originalna verzija sigurnosnih testova sadržavala preklapanja sa integracijskim testovima, što je moglo dovesti do lažnog osjećaja sigurnosti i nepotrebne duplikacije.
 
**Greška 7 — EF Core tracking bug u audit logu (Sprint 10, AI-20)**
Vrijednosti "prije izmjene" bile su identične vrijednostima "nakon" jer je serijalizacija rađena na već izmijenjenom tracked objektu. Riješeno kopiranjem vrijednosti u lokalne primitivne varijable prije izmjene.
 
**Greška 8 — Neispravna Razor sintaksa za komentare (Sprint 10, AI-19)**
U jednoj verziji AI je koristio neispravnu Razor sintaksu za komentare (`{{!-- --}}` umjesto `@* *@`), što je uzrokovalo build grešku koja je uočena iz Docker loga.
 
**Greška 9 — Neusklađenost naziva propertija (Sprint 10, AI-21)**
Neusklađenost između naziva propertija u modelu (`PodnosiocId`) i kolone u bazi (`PodnosilacId`) uzrokovala je `DbUpdateException` na produkciji.
 
**Greška 10 — Browser HTML validacija blokira testove (Sprint 8, AI-16)**
Uočeno je da browser-side HTML validacija (`min="1"`) ne odobrava submit nevažećih vrijednosti, pa je za test validacije bilo potrebno ukloniti atribut putem `page.EvaluateAsync` prije slanja forme.
 
---
 
## 7. Dijelovi sistema razvijani uz AI pomoć

Ključni dijelovi sistema razvijeni uz pomoć AI alata:
 
**Sistem autentifikacije i autorizacije (Sprint 5, AI-01)**
Implementacija login sistema, logout funkcionalnosti, JWT autentifikacije i zaštite ruta — kod za AuthController, konfiguracija Program.cs, primjeri za `[Authorize]`. 
 
**Entity Framework model i relacije (Sprint 5, AI-04)**
Implementacija baze podataka preko prethodno dizajniranog domain modela korištenjem Entity Framework-a. Tim je ispravio greške u vezama između entiteta i odbacio nepotrebne foreign key tagove.
 
**Sistem upravljanja primjercima knjiga i deaktivacija (Sprint 6, AI-08)**
Logika automatskog kreiranja inventarnih brojeva u formatu `INV-{Id}-{Broj}`, dodavanje više primjeraka odjednom, te akcija za deaktivaciju s provjerom aktivnih zaduženja.
 
**Upravljanje kategorijama i zaštita od brisanja (Sprint 6, AI-09)**
CRUD sistem za kategorije s provjerom duplikata naziva i zaštitom od brisanja kategorija koje su povezane s knjigama (US-32).
 
**Testna arhitektura — unit, integracijski, UI i penetracijski testovi (Sprint 6, AI-10 do AI-13)**
Unit testovi s XUnit i Moq, integracijsko testiranje s WebApplicationFactory i in-memory bazama, Playwright UI testovi umjesto Seleniuma, te penetracijski testovi zasnovani na principu "napad vs. ispravnost".
 
**Sistem zaduživanja knjiga (Sprint 7, AI-14)**
Logika validacije i kreiranje zaduženja s provjerom: da li je član aktivan, da li primjerak postoji, da li je primjerak dostupan i da li već postoji aktivno zaduženje za isti primjerak.
 
**Audit log sistem (Sprint 10, AI-20)**
`IAuditLogRepository` i `AuditLogRepository` s paginacijom i filterima, izmjene controller akcija s audit log pozivima, `AuditLog.cshtml` s Base64 enkodiranjem JSON vrijednosti u `data-` atribute i JavaScript diff prikazom, UTC+2 fiksni offset. Ključni problem: EF Core tracking bug riješen kopiranjem vrijednosti u lokalne varijable prije izmjene.
 
**Online produženje članarine (Sprint 10, AI-19)**
Model `ZahtjevProduzenja` s bibliotekaredskom verifikacijom, pet DTO/ViewModel klasa, četiri controller akcije, Razor stranice za člana i bibliotekara. Problem: kod je deployovan na Render prije pokretanja EF migracije, što je uzrokovalo grešku `Table 'ZahtjeviProduzenja' doesn't exist`, privremeno riješenu ručnim SQL skriptom.
 
**Integracija s distributerom knjiga (Sprint 10, AI-21)**
`NabavkaZahtjev` i `AppPostavka` modeli, `INabavkaRepository`, `NabavkaController`, `Zahtjev.cshtml`. Email adresa distributera čuva se u `AppPostavke` tabeli. Neusklađenost naziva `PodnosiocId` vs `PodnosilacId` uzrokovala je `DbUpdateException` na produkciji.
 
**Napredna pretraga i kombinovani filteri (Sprint 9, AI-18)**
Logika kombinovanih filtera po kategoriji, izdavaču i godini izdanja s inline filter formom unutar stranice kataloga. Problem: očuvanje odabranih vrijednosti filtera nakon osvježavanja stranice zahtijevalo je dodatnu prilagodbu View modela.
 
**Generisanje PDF izvještaja (Sprint 9, AI-17)**
Sistem izvještaja s filterom po mjesecu i godini, Chart.js grafikoni i PDF eksport. Problemi: integracija postojećih CSS stilova unutar PDF dokumenta i prikaz Chart.js grafikona unutar stranice zahtijevali su ručne korekcije.

---

## 8. Zaključna ocjena korištenja AI alata

Tokom razvoja SmartLib sistema AI alati su korišteni u gotovo svim fazama — od implementacije backend logike i testiranja do dizajna korisničkog interfejsa. Generisani kod nikada nije prihvaćan bez pregleda, promišljeno je odlučivano šta prihvatiti, prilagoditi ili odbaciti, a sve greške AI-ja su identifikovane i ispravljene.

Najznačajniji doprinos AI alata bio je u ubrzavanju inicijalne implementacije i kreiranju testnih infrastruktura koje bi ručno zahtijevale mnogo više vremena. Najznačajniji rizici odnosili su se na greške vidljive tek u produkcijskom okruženju (neusklađenost naziva kolona, nedostajuće migracije), zbog čega je bilo ključno izvršiti detaljan code review i testiranje prije deploya.