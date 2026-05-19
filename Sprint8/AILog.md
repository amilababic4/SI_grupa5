# AI Usage Log

## Opis dokumenta
Ovaj dokument evidentira korištenje AI alata tokom razvoja projekta, uključujući svrhu korištenja, rezultate i način primjene.

> Napomena: AI Usage Log je živi dokument koji se ažurira kroz sprintove


## AI Log 1: Implementacija sistema autentifikacije

**Datum:** 25.04.2026  
**Sprint:** 5  
**Alat koji je korišten:** ChatGPT  
**Ko je koristio alat:** Amila 

**Svrha korištenja:**  
Implementacija autentifikacije (PB-17)

**Opis zadatka:**  
Implementacija login sistema, logout funkcionalnosti, JWT autentifikacije i zaštite ruta

**Šta je AI predložio:** 
- Kod za AuthController
- Konfiguraciju Program.cs
- Primjere za [Authorize]

**Šta je tim prihvatio:**  
- Većinu backend i frontend logike

**Šta je tim izmijenio:**  
- Prilagođeni role-based pristup

**Šta je tim odbacio:**  
- Dijelove koji nisu odgovarali strukturi projekta

**Rizici i problemi:**  
- Potrebno razumjeti generisani kod


## AI Log 2: Razvoj funkcionalnosti za kreiranje korisničkog računa

**Datum:**  25.04.2026  
**Sprint:**   5  
**Alat koji je korišten:** GitHub Copilot  
**Ko je koristio alat:**  Amar

**Svrha korištenja:**  
Pomoć pri implementaciji PB-18: Kreiranje naloga člana

**Opis zadatka:**  
Generisanje i dopuna C# koda za user storyje vezane za kreiranje korisnika

**Šta je AI predložio:**  
- Dijelove implementacije kontrolera i validacije

**Šta je tim prihvatio:**  
- Dio generisanog koda

**Šta je tim izmijenio:**  
- Prilagođeno postojećoj arhitekturi

**Šta je tim odbacio:**  
- Neodgovarajuće prijedloge

**Rizici i problemi:**  
- Moguća nekonzistentnost koda


## AI Log 3: Redizajn korisničkog interfejsa

**Datum:** 28.04.2026  
**Sprint:** 5  
**Alat koji je korišten:** Codex  
**Ko je koristio alat:** Amar   

**Svrha korištenja:**  
Dizajn i responzivnost stranice

**Opis zadatka:**  
Redizajniranje naslovne stranice u dogovoru sa timom da bi bila modernija, privlačnija i responzivnija.

**Šta je AI predložio:**  
- Dva dizajna na odabir

**Šta je tim prihvatio:**  
- Prihvaćen je trenutni dizajn stranice

**Šta je tim izmijenio:**  
- Trenutni defaultni dizajn

**Šta je tim odbacio:**  
- Drugi dizajn stranice

**Rizici i problemi:**  
- Nema rizika niti problema


## AI Log 4: Modeliranje i implementacija perzistencije podataka

**Datum:** 28.04.2026  
**Sprint:** 5  
**Alat koji je korišten:** Claude  
**Ko je koristio alat:**  Imran

**Svrha korištenja:**  
Pomoć pri implementacji baze

**Opis zadatka:**  
Implementacija baze preko prethodno dizajniranog domain modela

**Šta je AI predložio:**  
- Korištenje Entity Framework-a za dizajn

**Šta je tim prihvatio:**
- Dijelove koda ključne za implementaciju

**Šta je tim izmijenio:**
- Greške prilikom veza između entiteta

**Šta je tim odbacio:**
- Nepotrebne foreign key tagove u modelima

**Rizici i problemi:**
- Nema rizika niti problema nakon pregleda


## AI Log 5: Implementacija osnove rada sa knjigama i katalogom

**Datum:** 30.04.2026.  
**Sprint broj:** Sprint 6  
**Alat koji je korišten:** Claude Code  
**Ko je koristio alat:** Ilma

**Svrha korištenja:**  
Implementacija osnovnih funkcionalnosti vezanih za rad sa knjigama i katalogom u okviru Sprinta 6.

**Kratak opis zadatka ili upita:**  
Alatu je zadato da osmisli plan implementiranja funkcionalnosti za dodavanje knjige, uređivanje postojećih knjiga, te prikaz knjiga u katalogu.

**Šta je AI predložio ili generisao:**  
AI je predložio i generisao izmjene u postojećem kodu za backend i korisnički interfejs. Izmjene su se odnosile na unos knjige, validaciju podataka, prikaz knjiga u katalogu, paginaciju i povezivanje knjiga sa kategorijama i primjercima.

**Šta je tim prihvatio:**  
Tim je prihvatio implementaciju koja se uklapala u postojeću strukturu projekta.

**Šta je tim izmijenio:**  
Tim je pregledao plan i preporučeni generisani kod i zadržao samo izmjene koje su bile direktno vezane za Sprint 6. Posebno je provjereno da implementacija ne narušava postojeće funkcionalnosti.

**Šta je tim odbacio:**  
Odbačene su ideje i izmjene koje bi zahtijevale veći refactoring, promjenu postojećih naziva ili proširenje funkcionalnosti izvan planiranog opsega Sprinta 6.

**Rizici, problemi ili greške koje su uočene:**  
Uočeni rizici su bili moguća neusklađenost sa postojećom arhitekturom, greške u validaciji ISBN-a i pogrešno povezivanje knjiga sa kategorijama i primjercima. Zbog toga je bilo potrebno dodatno provjeriti build, testirati funkcionalnosti i ručno pregledati ključne scenarije.


## AI Log 6: Popravka brisanja knjiga

**Datum:** 30.04.2026.  
**Sprint broj:** Sprint 6  
**Alat koji je korišten:** Claude  
**Ko je koristio alat:** Imran

**Svrha korištenja:**  
Popravka i kompletiranje funkcionalnosti brisanja knjiga u sistemu.

**Kratak opis zadatka ili upita:**  
Nakon implementacije osnovnih funkcionalnosti, od AI alata je zatraženo da popravi brisanje knjiga tako da korisnik može obrisati knjigu iz sistema, a da se obrisana knjiga više ne prikazuje u aktivnom katalogu.

**Šta je AI predložio ili generisao:**  
AI je predložio logiku brisanja knjiga, dodavanje dugmeta za brisanje u korisnički interfejs i potvrdu prije izvršavanja brisanja. Također je predloženo da se koristi sigurniji pristup, odnosno soft delete ili označavanje knjige kao neaktivne ako takav princip već postoji u projektu.

**Šta je tim prihvatio:**  
Tim je prihvatio princip da se brisanje mora uklopiti u postojeći sistem i da obrisane ili neaktivne knjige ne smiju biti prikazane korisnicima u katalogu.

**Šta je tim izmijenio:**  
Tim je prilagodio implementaciju postojećem modelu projekta, posebno u dijelu gdje se vodi računa o aktivnim i neaktivnim zapisima, kao i o povezanim podacima knjige.

**Šta je tim odbacio:**  
Odbačeno je agresivno brisanje koje bi moglo narušiti povezane podatke, posebno podatke o primjercima knjige ili drugim zavisnim entitetima.

**Rizici, problemi ili greške koje su uočene:**  
Glavni rizik bio je da brisanje knjige može obrisati ili oštetiti povezane podatke. Zbog toga je naglašeno da se koristi sigurniji pristup koji neće narušiti integritet podataka u sistemu.


## AI Log 7: Poboljšanje korisničkog interfejsa

**Datum:** 30.04.2026.  
**Sprint broj:** Sprint 6  
**Alat koji je korišten:** Claude Code  
**Ko je koristio alat:** Ilma

**Svrha korištenja:**  
Poboljšanje korisničkog interfejsa kako bi aplikacija izgledala profesionalnije i više ličila na stvarni bibliotečki informacioni sistem.

**Kratak opis zadatka ili upita:**  
Od AI alata je zatraženo da poboljša izgled aplikacije.

**Šta je AI predložio ili generisao:**  
AI je predložio poboljšanja izgleda glavnog layouta i navigacije. Prijedlog je bio da dizajn bude moderan, čist, jednostavan i konzistentan sa postojećim stilom projekta.

**Šta je tim prihvatio:**  
Tim je prihvatio ideju modernijeg i profesionalnijeg dizajna za rad sa knjigama.

**Šta je tim izmijenio:**  
Tim je ograničio izmjene samo na relevantne dijelove sistema. Na taj način je izbjegnuto nepotrebno mijenjanje cijele aplikacije.

**Šta je tim odbacio:**  
Odbačene su ideje koje bi aplikaciju učinile previše generičkom ili vizuelno prenatrpanom, kao što su pretjerani gradijenti, nasumične slike, previše animacija, neusklađene ikone i elementi koji bi izgledali kao automatski generisan dizajn.

**Rizici, problemi ili greške koje su uočene:**  
Rizik je bio da AI napravi vizuelno nekonzistentan ili pretjerano stilizovan interfejs. Zbog toga je naglašeno da dizajn mora ostati jednostavan, čist, moderan i usklađen sa postojećim projektom.

## AI Log 8: Upravljanje primjercima knjiga i sistem deaktivacije

**Datum:** 01.05.2026.  
**Sprint broj:** Sprint 6  
**Alat koji je korišten:** Claude Code  
**Ko je koristio alat:** Muhamed

**Svrha korištenja:**  
Implementacija funkcionalnosti za rad sa fizičkim primjercima knjiga i praćenje njihovog statusa.

**Kratak opis zadatka ili upita:**  
Zatraženo je da generiše logiku koja omogućava osoblju biblioteke dodavanje više pojedinačnih primjeraka za jednu knjigu, pregled njihovih jedinstvenih identifikatora, te mogućnost deaktivacije oštećenih ili izgubljenih primjeraka.

**Šta je AI predložio ili generisao:**  
Generisana je logika za automatsko kreiranje inventarnih brojeva u formatu `INV-{Id}-{Broj}`, petlja za dodavanje više primjeraka odjednom, te akcija za deaktivaciju koja provjerava aktivna zaduženja preko repozitorija prije promjene statusa

**Šta je tim prihvatio:**  
Tim je prihvatio implementaciju sa asinhronim Task-ovima, korištenje TempData za prikaz poruka o uspjehu/grešci, te strogu validaciju povezanosti primjerka sa knjigom.

**Šta je tim izmijenio:**  
Tim je prilagodio putanje u RedirectToAction metodama kako bi se korisnik nakon akcije uvijek vraćao na Details stranicu matične knjige.

**Šta je tim odbacio:**  
Odbačena je ideja o ručnom unosu inventarnog broja od strane korisnika; zadržana je AI predložena automatizacija kako bi se izbjegli duplikati i osigurala konzistentnost unosa

**Rizici, problemi ili greške koje su uočene:**  
Glavni rizik bio je slučaj deaktivacije već zadužene knjige, što je uspješno riješeno provjerom da li ima aktivnih zaduženja.


## AI Log 9: Upravljanje kategorijama knjiga i validacija zavisnosti

**Datum:** 02.05.2026.  
**Sprint broj:** Sprint 6  
**Alat koji je korišten:** Claude Code  
**Ko je koristio alat:** Muhamed

**Svrha korištenja:**  
Implementacija potpunog CRUD sistema za upravljanje kategorijama knjiga.

**Kratak opis zadatka ili upita:** 
Zatraženo je da generiše kontroler koji podržava dodavanje, pregled, uređivanje i brisanje kategorija, uz implementaciju specifičnih pravila kao što su zabrana duplih naziva i zaštita od brisanja kategorija koje se koriste.

**Šta je AI predložio ili generisao:**  
Implementirana je logika za provjeru postojanja kategorije sa istim nazivom prije kreiranja ili ažuriranja. Također, generisana je Delete akcija za provjeru povezanosti sa knjigama prije samog brisanja.

**Šta je tim prihvatio:**  
Tim je prihvatio strukturu akcija za dobavljanje podataka kategorije, što omogućava lakšu implementaciju inline formi na korisničkom interfejsu.

**Šta je tim izmijenio:**  
Tim je dodao dijelove za čišćenje praznih stringova kako bi se osiguralo da su podaci u bazi čisti i bez nepotrebnih razmaka, te je prilagođen prikaz validacijskih poruka.

**Šta je tim odbacio:**  
Odbačeni su prijedlozi koji su zahtijevali kompleksne view modele za jednostavan CRUD.

**Rizici, problemi ili greške koje su uočene:**  
Glavni rizik bio je brisanje kategorija koje bi ostavilo knjige u nevalidnom stanju. Ovo je riješeno striktnom implementacijom US-32 kroz provjeru povezanosti u bazi uz prikaz jasne poruke korisniku o potrebi premještanja knjiga prije brisanja.


## AI Log 10: Unit testiranje sistema 

**Datum:** 04.05.2026.  
**Sprint broj:** Sprint 6  
**Alat koji je korišten:** Claude Code  
**Ko je koristio alat:** Muhamed

**Svrha korištenja:**  
Implementacija unit testova za backend kako bi se osigurala stabilnost sistema nakon implementacije svih funkcionalnosti koje su implementirane u sprintovima 5 i 6.

**Kratak opis zadatka ili upita:**  
Alatu je zadato da generiše testne klase za sve funkcionalnosti implementirane u sprintovima 5 i 6. Zahtjev je bio da se testovi razdvoje na Web (provjera redirekcija, View modela i TempData poruka) i API (provjera JSON odgovora i HTTP statusnih kodova).

**Šta je AI predložio ili generisao:**  
AI je generisao testne klase koristeći `Xunit` i `Moq`, obuhvatajući ključne scenarije autentifikacije, autorizacije i validacije poslovnih pravila kroz sve slojeve sistema. Predloženi kod simulira rad s bazom podataka putem mock-ovanih repozitorija, čime je omogućena automatska provjera ispravnosti sigurnosnih tokena, integriteta relacija među entitetima, te pravilnog rukovanja greškama i statusnim kodovima. 

**Šta je tim prihvatio:**  
Prihvaćena je arhitektura testova koja striktno odvaja Web i API kontrolere. Također, prihvaćena je i upotreba `Mock` objekata kako bi se testovi izvršavali trenutno, bez zavisnosti o pravoj bazi podataka.

**Šta je tim izmijenio:**  
Tim je ručno dopunio testove za granične slučajeve, dodajući specifične Assert provjere za TempData poruke.

**Šta je tim odbacio:**  
Odbačeni su prijedlozi za generisanje nasumičnih testnih podataka jer je bilo važno imati potpunu kontrolu nad specifičnim stringovima radi preciznije validacije.

**Rizici, problemi ili greške koje su uočene:**  
 Tokom pisanja testova otkriveno je da API kontroler za knjige nije vraćao ispravan status kada se pokuša unijeti isti ISBN, već je bacao internu grešku. Također, otkriveno je i da je sistem vraćao previše detaljne poruke o grešci pri prijavi, što je korigovano na generičke poruke radi povećanja sigurnosti.

## AI Log 11: Implementacija integracijskog testiranja
 
**Datum:** 05.05.2026  
**Sprint broj:** Sprint 6  
**Alat koji je korišten:** Claude  
**Ko je koristio alat:** Esma
 
**Svrha korištenja:**  
Implementacija integracijskog testiranja backend API-ja kako bi se verificiralo ispravno funkcionisanje sistema kroz cijeli HTTP stack, uključujući autorizaciju, validaciju i poslovnu logiku za sve ključne module sistema.
 
**Kratak opis zadatka ili upita:**  
Od AI alata je zatraženo da analizira postojeću strukturu projekta i predloži kompletan set integracionih testova za backend API, pokrivajući CRUD operacije za module knjiga, korisnika, kategorija i primjeraka, provjeru autorizacije po ulogama, validaciju ulaznih podataka i poslovna pravila definisana kroz user storyje.
 
**Šta je AI predložio ili generisao:**  
AI je predložio i generisao kompletnu infrastrukturu za integracijsko testiranje koristeći `WebApplicationFactory`, koja pokreće cijelu ASP.NET Core aplikaciju u memoriji i omogućava slanje realnih HTTP zahtjeva bez potrebe za pokretanjem servera. Predložena je upotreba in-memory baze podataka sa GUID imenima kako bi svaki test run imao vlastitu izoliranu bazu. Generisani su seederi za postavljanje poznatog početnog stanja sa ulogama, korisnicima, kategorijama i podacima potrebnim za testove, kao i helperi za direktno postavljanje složenijih stanja u bazu bez prolaska kroz API. Testovi pokrivaju provjeru HTTP statusnih kodova, strukturu JSON odgovora, autentifikaciju i autorizaciju po ulogama, validaciju ulaznih podataka te provjeru poslovnih pravila kao što je zabrana brisanja resursa koji imaju aktivne zavisnosti. Pokrivenost obuhvata module za knjige, korisnike, kategorije i primjerke knjiga.
 
**Šta je tim prihvatio:**  
Tim je prihvatio kompletnu infrastrukturu i predloženi pristup testiranja kroz realne HTTP pozive. Prihvaćen je pattern gdje testovi kreiraju vlastite resurse umjesto da se oslanjaju na zajednički seeded sadržaj, čime se osigurava međusobna nezavisnost testova.
 
**Šta je tim izmijenio:**  
Nazivi polja u seederima su usklađeni sa stvarnim imenima u modelu projekta. Dijagnostički testovi koji su korišteni tokom debug faze su uklonjeni iz finalne verzije test suite-a.
 
**Šta je tim odbacio:**  
Odbačeni su testovi koji su direktno provjeravali stanje baze podataka umjesto HTTP odgovora, jer integracijsko testiranje treba validirati ponašanje sistema kroz API, a ne internu implementaciju.
 
**Rizici, problemi ili greške koje su uočene:**  
Tokom implementacije otkriveno je da kontroler nije mogao biti aktiviran jer su nedostajale registracije zavisnosti u `Program.cs`, što je uzrokovalo grešku na svim endpointima. 

## AI Log 12: Implementacija UI testiranja (Playwright)
 
**Datum:** 05.05.2026  
**Sprint broj:** Sprint 6  
**Alat koji je korišten:** Cursor  
**Ko je koristio alat:** Esma
 
**Svrha korištenja:**  
Implementacija automatiziranih UI testova kroz browser koristeći Playwright, za provjeru korisničkih tokova i ispravnosti prikaza interfejsa.
 
**Kratak opis zadatka ili upita:**  
Od AI alata je zatraženo da predloži i implementira Playwright UI testove koji pokrivaju ključne korisničke tokove kroz web aplikaciju - autentifikaciju, navigaciju i pristup katalogu knjiga ovisno o ulozi korisnika.
 
**Šta je AI predložio ili generisao:**  
AI je predložio strukturu testova podijeljenu u klase koje nasljeđuju zajedničku baznu klasu `SmartLibUiTest`. Generisani su testovi za prikaz login forme i vidljivost polja, prikaz greške pri pogrešnoj lozinci, redirekciju člana na Home stranicu nakon prijave, redirekciju bibliotekara na stranicu upravljanja članovima, odjavu i povratak na login stranicu, te pristup katalogu knjiga sa pretragom nakon prijave kao član.
 
**Šta je tim prihvatio:**  
Tim je prihvatio Playwright kao alat za UI testiranje.
 
**Šta je tim izmijenio:**  
Tim je uskladio očekivane URL pattern-e i tekstove headinga sa stvarnom implementacijom routinga i Razor View-ova u projektu. Prilagođeni su i email-ovi i lozinke kroz centralizovane `UiTestSettings` kako bi se izbjeglo hardcodiranje u svakom testu.
 
**Šta je tim odbacio:**  
Odbačena je upotreba Seleniuma kao alata za UI testiranje u korist Playwrighta, koji pruža moderniji API, bolju podršku za asinhrono izvršavanje i stabilnije testove kroz .NET okrzženje.
 
**Rizici, problemi ili greške koje su uočene:**  
Uočeno je da odjava nije dostupna na početnoj stranici već samo na ostalim stranicama aplikacije, pa je test za odjavu morao navigirati na `/Knjiga` prije nego što klikne dugme za odjavu. 

 ## AI Log 13: Implementacija penetracijskih / sigurnosnih testova

**Datum:** 05.05.2026.  
**Sprint broj:** Sprint 6  
**Alat koji je korišten:** Claude / GitHub Copilot  
**Ko je koristio alat:** Muhamed

**Svrha korištenja:**  
Implementacija sigurnosnih / penetracijskih testova za SmartLib sistem.

**Kratak opis zadatka ili upita:**  
Od AI alata je zatražena analiza preklapanja između postojećih integracijskim testova i planiranih sigurnosnih testova, te implementacija čistih penetracijskih testova koji pokrivaju isključivo napadačke vektore.

**Šta je AI predložio ili generisao:**  
AI je predložio podjelu testova na osnovu principa "napad vs. ispravnost" — gdje integracijski testovi pokrivaju poslovnu logiku i autorizacijske politike (401/403), a sigurnosni testovi isključivo napadačke vektore. Generisani su testovi za SQL Injection u email i lozinka polju, Brute Force napad, lažni i modificirani JWT token, XSS payload u registracijskom obrascu i nazivu kategorije, te Path Traversal i Injection u ISBN polju.

**Šta je tim prihvatio:**  
Tim je prihvatio predloženu strukturu i granicu između sigurnosnih i integracijskim testova, kao i kompletnu implementaciju svih napadačkih vektora.

**Šta je tim izmijenio:**  
Zadržan je arhitekturalni dokumentacijski test za stari JWT deaktiviranog korisnika (`PT-06`) jer pokriva sigurnosni rizik koji nije pokriven nigdje drugdje u test suiti.

**Šta je tim odbacio:**  
Odbačeni su testovi koji su samo ponavljali provjere iz integracijskim testova — konkretno, provjere `401` bez tokena i `403` za eskalaciju privilegija, jer je utvrđeno da spadaju u integracijsko testiranje, a ne u penetracijsko.

**Rizici, problemi ili greške koje su uočene:**  
Uočeno je da originalna verzija sigurnosnih testova sadržavala preklapanja sa integracijskim testovima, što je moglo dovesti do lažnog osjećaja sigurnosti i nepotrebne duplikacije. 


## AI Log 14: Implementacija evidencije zaduživanja knjiga

**Datum:** 09.05.2026.  
**Sprint broj:** Sprint 7  
**Alat koji je korišten:** Claude Code  
**Ko je koristio alat:** Ilma

**Svrha korištenja:**  
Implementacija funkcionalnosti za evidentiranje zaduživanja knjiga u SmartLib sistemu.

**Kratak opis zadatka ili upita:**  
Od AI alata je zatraženo da pomogne u implementaciji funkcionalnosti koja bibliotekaru omogućava da evidentira zaduživanje knjige članu biblioteke. Funkcionalnost je trebala obuhvatiti izbor člana, izbor dostupnog primjerka knjige, unos datuma zaduživanja i automatsko postavljanje očekivanog datuma vraćanja.

**Šta je AI predložio ili generisao:**  
AI je predložio strukturu kontrolera, repozitorijsku logiku i osnovni tok validacije prije kreiranja zaduženja. Predloženo je da se prije zaduživanja provjeri da li je član aktivan, da li primjerak knjige postoji, da li je primjerak dostupan i da li već postoji aktivno zaduženje za isti primjerak.

**Šta je tim prihvatio:**  
Tim je prihvatio osnovnu logiku validacije i tok kreiranja zaduženja, uključujući provjeru dostupnosti primjerka i povezivanje zaduženja sa članom i bibliotekarom.

**Šta je tim izmijenio:**  
Tim je prilagodio nazive metoda, rute i View modele postojećoj strukturi SmartLib projekta. Dodatno su prilagođene poruke korisniku kako bi bibliotekar jasno vidio da li je zaduženje uspješno evidentirano ili zašto nije moguće zadužiti knjigu.

**Šta je tim odbacio:**  
Odbačeni su prijedlozi koji su uvodili nepotrebno složene statuse zaduženja ili dodatne tokove koji nisu bili dio planiranog opsega Sprinta 7.

**Rizici, problemi ili greške koje su uočene:**  
Glavni rizik bio je mogućnost da se isti primjerak knjige zaduži više puta dok prethodno zaduženje još nije zaključeno. Zbog toga je posebna pažnja posvećena provjeri aktivnih zaduženja prije kreiranja novog zapisa.


## AI Log 15: Implementacija UI testova za kategorije, knjige i primjerke

**Datum:** 18.05.2026.  
**Sprint broj:** Sprint 8  
**Alat koji je korišten:** Claude  
**Ko je koristio alat:** Esma

**Svrha korištenja:**  
Implementacija automatiziranih Playwright UI testova za funkcionalnosti upravljanja kategorijama, knjigama i primjercima kroz web interfejs SmartLib sistema.

**Kratak opis zadatka ili upita:**  
Od AI alata je zatraženo kreiranje i prilagođavanje Playwright UI testova za module kategorija, knjiga i primjeraka knjiga. AI je analizirao Razor View fajlove i kontrolere kako bi testovi odgovarali stvarnoj implementaciji interfejsa, rutama, validacijama i korisničkim tokovima.

**Šta je AI predložio ili generisao:**  
AI je generisao tri Playwright test klase — `KategorijaUiTests`, `KnjigaUiTests` i `PrimjerakUiTests` — koje nasljeđuju zajedničku baznu klasu `SmartLibUiTest`. Implementirani su testovi za: prikaz i kreiranje kategorije, validaciju duplih kategorija, inline uređivanje kategorije, dodavanje knjige, uređivanje knjige, pretragu kataloga, prikaz funkcionalnosti ovisno o ulozi korisnika, dodavanje primjeraka knjige, validaciju broja primjeraka, deaktivaciju primjerka, te prikaz statusa deaktiviranog primjerka.

**Šta je tim prihvatio:**  
Tim je prihvatio kompletnu strukturu Playwright testova, te pristup organizaciji testnih helper metoda.

**Šta je tim izmijenio:**  
Tim je uskladio tekstove headinga, dugmadi, validacijskih poruka i URL pattern-a sa stvarnim Razor View implementacijama i `TempData` porukama iz kontrolera. Zamijenjeni su nepouzdani `GetByLabel` selektori sa direktnim CSS locatorima (`#naziv-novi`, `input[name='Naslov']`, `select[name='KategorijaId']`) kako bi testovi stabilno radili sa skrivenim i dinamički prikazanim formama. 

**Šta je tim odbacio:**  
Odbačena je upotreba `GetByLabel` pristupa za forme čiji inputi nisu eksplicitno povezani sa label elementima, kao i navigacija klikom na `a.stretched-link` elemente jer ih Playwright tretira kao nevidljive zbog nedostatka dimenzija.

**Rizici, problemi ili greške koje su uočene:**  
Uočeno je da browser-side HTML validacija (min="1") ne odobrava submit nevažećih vrijednosti, pa je za test validacije bilo potrebno ukloniti atribut putem page.EvaluateAsync prije slanja forme.

