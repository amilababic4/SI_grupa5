# AI Usage Log - Sprint 6

## AI Log 1: Implementacija osnove rada sa knjigama i katalogom

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


## AI Log 2: Popravka brisanja knjiga

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


## AI Log 3: Poboljšanje korisničkog interfejsa

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
