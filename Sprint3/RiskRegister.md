# Risk Register
## Projekat: Bibliotečki informacioni sistem

## 1. Svrha dokumenta

Ovaj dokument predstavlja rezultat analize rizika za projekat **Bibliotečki informacioni sistem**.  
Risk Register služi kao pregled identificiranih rizika koji mogu negativno uticati na:

- kvalitet projektnih artefakata
- realizaciju sprint ciljeva
- kontinuitet rada tima
- implementaciju sistema u narednim sprintovima
- kvalitet završnog rješenja
- ocjenu projekta i gubitak bodova

---

## 3. Risk Register tabela
<div style="font-size: 8px;">
  
> **Napomena:** Tabela risk registra sadrži veći broj kolona zbog razrađenih planova mitigacije, koji uključuju preventivne mjere, reakciju ako se rizik desi, te mogućnost eliminacije ili prenosa rizika. Zbog toga je za njen pregled potreban horizontalni scroll.
  
| ID | Opis rizika | Uzrok | Vjero-<br>vatnoća | Uticaj | Posljedica za projekat | Prioritet | Preventivna mitigacija (da ne dođe do rizika) | Korektivna mitigacija (kako smanjiti posljedicu) | Eliminacija / prenos rizika | Odgovorna osoba / uloga | Status |
|---|---|---|---|---|---|---|---|---|---|---|---|
| R-01 | Tim precijeni koliko može završiti u sprintu | Nerealna procjena napora, potcjenjivanje složenosti, previše ambiciozan plan | Visoka | Visok | Kašnjenje sprint cilja, nezavršene stavke, gubitak bodova | Kritičan | Sprint backlog puniti samo realnim stavkama; veće zadatke razbijati na manje; planirati buffer za nepredviđene stvari | Odmah smanjiti scope sprinta, zadržati najvažnije stavke, jasno objasniti šta je pomjereno i zašto | Rizik se ne može prenijeti, ali se može ublažiti boljim planiranjem i ranijim usaglašavanjem s PO/asistentom | Scrum koordinator / cijeli tim | Aktivan |
| R-02 | Poslovna pravila sistema nisu dovoljno razjašnjena | Nejasnoće oko članarine, rezervacija, prava korisnika, ograničenja zaduživanja | Visoka | Visok | Pogrešan domain model, pogrešna arhitektura, rework u kasnijim sprintovima | Kritičan | Voditi listu otvorenih pitanja; dokumentovati pretpostavke; ne zaključavati rješenja dok pravila nisu dovoljno jasna | Revidirati pogođene artefakte, ažurirati model i backlog, jasno označiti šta je izmijenjeno | Dio rizika se prenosi na Product Ownera kroz potrebu za pojašnjenjem, ali tim mora na vrijeme prepoznati nejasnoću | Osoba za backlog / zahtjeve | Aktivan |
| R-03 | Domain model bude pogrešno postavljen | Modeliranje bez dovoljno razumijevanja stvarnih poslovnih pravila | Srednja | Visok | Problemi u dizajnu baze i implementaciji logike sistema | Visok | Prvo definisati pravila i use caseove, pa tek onda model; raditi peer review modela | Ispraviti model prije početka implementacije baze; uskladiti zavisne artefakte | Može se djelimično eliminisati internim pregledom i validacijom prije Sprinta 4 | Tehnički lead / osoba za modeliranje | Aktivan |
| R-04 | Arhitektonske odluke budu nasumične ili slabo obrazložene | Biranje tehnologije po navici, bez veze sa zahtjevima | Srednja | Visok | Slaba odbrana arhitekture, kasniji tehnički problemi, pad kvaliteta implementacije | Visok | Svaku odluku povezati sa konkretnim zahtjevom ili problemom sistema | Preispitati odluku, pojednostaviti arhitekturu i dokumentovati zašto je promijenjena | Rizik se ne prenosi, ali se može ublažiti zajedničkim donošenjem odluka | Tehnički lead | Aktivan |
| R-05 | Tim nema dovoljno znanja za dio zadataka | Slabije iskustvo sa bazama, autentikacijom, testiranjem, modeliranjem ili web arhitekturom | Visoka | Srednji | Spor napredak, nesigurnost, površni artefakti i slabija implementacija | Visok | Raspodijeliti zadatke prema jačim stranama članova; planirati vrijeme za učenje i istraživanje | Pojednostaviti rješenje, prebaciti složeniji dio članu koji ga bolje razumije, tražiti konsultacije | Rizik se djelimično prenosi unutar tima preraspodjelom zadataka | Cijeli tim | Aktivan |
| R-06 | Član tima bude odsutan zbog bolesti, privatnih obaveza | Akademske i privatne obaveze, zdravstveni problemi | Visoka | Srednji | Usporavanje rada i manja sposobnost tima da ispuni sprint cilj | Visok | Ključne informacije i materijale držati zajednički; ne vezati kritične zadatke za jednu osobu | Preraspodijeliti obaveze, smanjiti scope, fokusirati se na minimum za sprint goal | Rizik se ne može ukloniti, ali se posljedice smanjuju dijeljenjem znanja i backup raspodjelom | Scrum koordinator | Aktivan |
| R-07 | Praznik ili važan datum padne na termin calla ili radnog sastanka | Preklapanje sa akademskim, državnim ili vjerskim kalendarom | Srednja | Srednji | Slabija prisutnost tima, slabija priprema za review, kašnjenje zadataka | Srednji | Ranije provjeravati kalendar i planirati kritične dijelove prije takvih dana | Organizovati asinhroni pregled, pomjeriti interna zaduženja unaprijed | Djelimično se prenosi kroz raniji dogovor u timu ili s nastavnim osobljem | Scrum koordinator | Aktivan |
| R-08 | Neko od članova tima značajno smanji doprinos ili odustane | Pad motivacije, preopterećenost, loša raspodjela rada | Srednja | Visok | Neravnomjerna raspodjela posla, pad tempa, lošiji kvalitet | Visok | Jasno definisati odgovornosti, pratiti izvršenje zadataka i rano reagovati na probleme | Redistribuirati zadatke, smanjiti scope, evidentirati problem i po potrebi eskalirati | Rizik se ne može prenijeti van tima, ali se može formalno evidentirati i eskalirati | Scrum koordinator / cijeli tim | Aktivan |
| R-09 | Loša komunikacija i nesporazumi u timu | Nejasni dogovori, neodgovaranje na poruke, nepostojanje centralnog kanala | Srednja | Srednji | Dupliranje rada, propuštene obaveze, nepotrebni konflikti | Srednji | Dogovoriti glavni kanal komunikacije, očekivano vrijeme odgovora i način potvrde zadataka | Održati usaglašavajući sastanak i dokumentovati konačan dogovor | Rizik se može gotovo eliminisati disciplinovanom komunikacijom | Cijeli tim | Aktivan |
| R-10 | Više članova radi na istom artefaktu pa nastane konflikt verzija | Nejasno vlasništvo nad dokumentima ili kodom, paralelne izmjene | Srednja | Srednji | Gubitak vremena, gubitak dijela sadržaja, haos u finalnoj verziji | Srednji | Svakom artefaktu dodijeliti vlasnika; jasno podijeliti sekcije rada | Spojiti verzije ručno i uraditi zajednički pregled finalne verzije | Rizik se može znatno smanjiti pravilima rada i verzionisanjem | Vlasnik artefakta / cijeli tim | Aktivan |
| R-11 | Gubitak dokumenta, koda ili važnih izmjena | Rad samo lokalno, nepravljenje backup-a, slučajno brisanje | Niska | Visok | Potreba za ponovnim radom i kašnjenje sprinta | Srednji | Koristiti cloud i repozitorij; redovno spremati verzije | Vratiti zadnju dostupnu verziju; rekonstruisati samo izgubljeni dio | Rizik se velikim dijelom eliminiše backup-om i verzionisanjem | Cijeli tim | Aktivan |
| R-12 | Kvar računara ili tehnički problem kod člana tima | Hardverski kvar, softverski problem, zastarjela oprema | Niska | Srednji | Privremena nedostupnost člana i njegovog rada | Nizak | Držati materijale u cloudu; imati pristup alternativnom uređaju | Drugi član privremeno preuzima posao, koristiti drugi uređaj ili fakultetski resurs | Djelimičan prenos na alternativni uređaj ili drugog člana | Svaki član individualno | Aktivan |
| R-13 | Nestanak struje ili interneta u bitnom trenutku | Vanjski tehnički problem, nestabilna mreža | Niska | Srednji | Ometen sastanak, kašnjenje predaje ili sinhronizacije | Nizak | Ne ostavljati završne stvari za zadnji sat; važan rad završavati ranije | Prebaciti se na mobilni internet, raditi offline, drugi član preuzima online dio | Rizik se ne može eliminisati, ali se može ublažiti ranijom pripremom | Svaki član / Scrum koordinator | Aktivan |
| R-14 | Risk Register ostane generičan, trivijalan | Površno popunjavanje tabele bez stvarne analize | Srednja | Visok | Loš utisak kod asistenta i Product Ownera, manja ocjena | Visok | Birati samo realne i relevantne rizike, sa konkretnim posljedicama i akcijama | Revidirati registar, izbaciti trivijalne stavke i dopuniti mitigacije | Rizik se može gotovo potpuno eliminisati internim reviewom | Osoba za dokumentaciju / cijeli tim | Aktivan |
| R-15 | Plan mitigacije bude previše opšt i neupotrebljiv | Rizik jeste prepoznat, ali odgovor na njega nije razrađen | Srednja | Visok | Artefakt djeluje formalno, ali ne pokazuje inžinjersku ozbiljnost | Visok | Za svaki rizik pisati prevenciju, odgovor ako se desi i opciju prenosa/eliminacije | Doraditi mitigacije za sve visoke i kritične rizike prije predaje | Rizik se može eliminisati dodatnom doradom i reviewom | Cijeli tim | Aktivan |
| R-16 | Sprint 3 artefakti ne budu međusobno usklađeni | Risk register, modeli, arhitektura i test strategy rade se odvojeno | Srednja | Visok | Dokumenti djeluju nepovezano i slabo se brane | Visok | Na kraju sprinta uraditi zajednički pregled svih artefakata | Ispraviti nedosljednosti prije predaje i uskladiti termine, pojmove i pretpostavke | Rizik se ne prenosi, mora se riješiti internim usaglašavanjem | Scrum koordinator / cijeli tim | Aktivan |
| R-17 | Product backlog se promijeni i učini neke pretpostavke zastarjelim | Nova pojašnjenja PO-a, promjena prioriteta ili scope-a | Srednja | Srednji | Potreba za izmjenama modela, prioriteta i kasnijih planova | Srednji | Jasno razlikovati potvrđene stavke od pretpostavki; ne graditi previše na neprovjerenim pretpostavkama | Ažurirati artefakte i dokumentovati promjenu bez odlaganja | Dio rizika se prenosi na proces rada s PO, ali tim mora brzo reagovati | Osoba za backlog | Aktivan |
| R-18 | Članovi tima ne razumiju dovoljno ono što je napisano i ne mogu odbraniti artefakte | Pisanje lijepog teksta bez stvarnog razumijevanja | Srednja | Visok | Slaba odbrana na reviewu, gubitak bodova i loš utisak | Visok | Sve ključne dijelove proći zajedno i osigurati da svaki član razumije osnovu | Prije sastanka uraditi internu probu pitanja i odgovora | Rizik se može gotovo eliminisati zajedničkom pripremom | Cijeli tim | Aktivan |
| R-19 | Test strategy bude previše opšta i nepovezana sa zahtjevima | Fokus na formu umjesto na stvarne scenarije sistema | Srednja | Srednji | Kasnije testiranje bude nejasno i slabo korisno | Srednji | Povezati testove sa konkretnim storyjima i acceptance kriterijima | Dopuniti strategiju konkretnim scenarijima prije narednog sprinta | Rizik se može eliminisati kvalitetnim reviewom | Osoba za kvalitet / testiranje | Aktivan |
| R-20 | Loše postavljen Sprint 3 ugrozi kasnije sprintove | Slabi temelji u modelu, arhitekturi i analizi rizika | Srednja | Visok | Rework u Sprintu 4+, sporija implementacija, tehnički dug | Visok | Sprint 3 tretirati kao temeljni sprint i ne raditi ga formalno | Na početku narednog sprinta prvo ispraviti temelje pa tek onda graditi dalje | Rizik se ne prenosi, samo rano prepoznaje i popravlja | Cijeli tim | Aktivan |
| R-21 | Dizajn baze podataka u Sprintu 4 ne bude usklađen sa domenom | Preuranjeno modeliranje tabela bez validiranog domain modela | Srednja | Visok | Problemi u CRUD funkcionalnostima, komplikovane migracije, rework | Visok | U Sprintu 3 jasno odvojiti entitete, atribute i poslovna pravila | Revidirati shemu baze prije implementacije ključnih featurea | Rizik se djelimično eliminiše validacijom modela prije baze | Tehnički lead / osoba za bazu | Aktivan |
| R-22 | Autentikacija i autorizacija budu loše zamišljene | Nedovoljno razrađene korisničke uloge i prava pristupa | Srednja | Visok | Sigurnosni propusti, neispravna ograničenja po ulozi, veći rework kasnije | Visok | Već sada jasno definisati šta član, bibliotekar i administrator smiju raditi | Ako se problem otkrije kasnije, revidirati role matrix i kritične tokove pristupa | Rizik se ne može prenijeti, ali se može ublažiti ranim definisanjem prava | Tehnički lead / osoba za zahtjeve | Aktivan |
| R-23 | Implementacija kasnijih funkcionalnosti bude sporija zbog loše raspodjele featurea po sprintovima | Previše zavisnosti u jednom sprintu, loš release plan | Srednja | Srednji | Gomilanje nezavršenih funkcionalnosti u kasnijoj fazi projekta | Srednji | Na vrijeme mapirati zavisnosti između backloga i featurea | Replanirati sprintove i izbaciti niže prioritete iz MVP-a | Djelimično se može prenijeti na odluku o smanjenju scope-a | Scrum koordinator / backlog owner | Aktivan |
| R-24 | Kasni featurei kao rezervacije, email upozorenja i izvještaji ostanu bez vremena | Fokus samo na core funkcionalnosti bez dovoljno rezerve za nadogradnje | Srednja | Srednji | Nepotpuna realizacija planiranog backloga | Srednji | Jasno odvojiti MVP od nadogradnji i ne trošiti previše vremena na manje važne detalje rano | Smanjiti scope release-a i ostaviti manje kritične featuree kao backlog za kasnije | Rizik se može djelimično eliminisati dobrim release planom | Product backlog odgovorna osoba / cijeli tim | Aktivan |
| R-25 | Kvalitet implementacije opadne jer se sve radi pred kraj | Odgađanje teških zadataka, gomilanje tehničkog duga, “samo da proradi” pristup | Srednja | Visok | Bugovi, slabija stabilnost, loša završna demonstracija | Visok | Kontinuirano raditi male isporuke, a ne ostavljati kritične dijelove za kraj | Uvesti stabilizacioni sprint pristup ranije nego što je planirano ako se pojavi problem | Rizik se ne prenosi, ali se može smanjiti disciplinom i ranijim testiranjem | Cijeli tim | Aktivan |
| R-26 | Testiranje ostane preslabo ili prekasno | Fokus na implementaciju bez vremena za provjeru | Srednja | Visok | Greške ostanu neotkrivene do reviewa ili završne demonstracije | Visok | Od početka planirati makar osnovne test scenarije po feature-u | Prioritizirati testiranje najkritičnijih tokova i evidenciju grešaka | Rizik se može djelimično ublažiti smanjenjem scope-a da ostane vrijeme za testove | Osoba za kvalitet / cijeli tim | Aktivan |
| R-27 | Tim ne vodi dovoljno ažurno backlog, decision log i ostale artefakte u kasnijim sprintovima | Fokus samo na “kod”, zanemarivanje projektne evidencije | Srednja | Srednji | Slabija transparentnost rada i lošija odbrana projekta | Srednji | Odmah odrediti ko vodi koji artefakt i kada se ažuriraju | Nadoknaditi evidenciju odmah nakon sprint reviewa, ne tek pred kraj semestra | Rizik se može ublažiti jasnom odgovornošću po artefaktu | Scrum koordinator / dokumentacija | Aktivan |
| R-28 | Tim previše zavisi od AI alata bez provjere u fazi pisanja koda | Prihvatanje AI prijedloga bez provjere smisla i usklađenosti sa projektom | Srednja | Visok | Pogrešna rješenja, nerazumijevanje sistema, loša odbrana rada | Visok | Sve AI prijedloge provjeravati ručno i prilagoditi projektu | Odbaciti problematične dijelove, ručno prepraviti i dokumentovati odluku | Rizik se ne prenosi; odgovornost ostaje na timu | Cijeli tim | Aktivan |
| R-29 | Tim potroši previše vremena na “uljepšavanje” dok osnovne funkcionalnosti kasne | Fokus na izgled dokumenta ili UI prije rješavanja suštine | Srednja | Srednji | Kašnjenje core funkcionalnosti i slabiji napredak po sprintovima | Srednji | Prvo završavati suštinske stvari pa tek onda dotjerivanje | Odrezati nepotrebne detalje i vratiti fokus na prioritete | Rizik se može djelimično eliminisati jasnom definicijom MVP-a | Scrum koordinator / cijeli tim | Aktivan |
| R-30 | Završna demonstracija bude slabija od stvarnog stanja projekta | Tim ne uvježba demo, ne pripremi tok prikaza ili ne zna objasniti odluke | Srednja | Srednji | Slab utisak iako je projekat možda solidan | Srednji | Tokom sprintova bilježiti šta i kako se demonstrira; pripremati se za objašnjenja | Napraviti demo skriptu i raspodijeliti ko šta govori | Rizik se može dosta smanjiti ranijom pripremom i probom | Cijeli tim | Aktivan |
| R-31 | Pogrešno odvajanje pojmova "knjiga" i "primjerak knjige" | Nedovoljno razumijevanje razlike između bibliografskog zapisa i fizičkog primjerka | Srednja | Visok | Neispravna dostupnost, zaduživanje i rezervacije; ozbiljni problemi u bazi i logici | Kritičan | Jasno definisati da knjiga predstavlja naslov, a primjerak konkretan fizički zapis sa statusom | Ako se greška uoči kasnije, revidirati model i prijeći na odvojene entitete prije implementacije većeg broja featurea | Rizik se može eliminisati ranim modeliranjem i validacijom poslovnih pravila | Osoba za modeliranje / osoba za bazu | Aktivan |
| R-32 | Prava korisničkih uloga budu loše definisana | Nejasno razgraničenje šta smiju član, bibliotekar i administrator | Srednja | Visok | Sigurnosni problemi, pogrešan pristup funkcijama, rework autentikacije i autorizacije | Visok | Napraviti pregled dozvola po ulozi prije implementacije login i role-based funkcionalnosti | Revidirati role matrix i prioritetno ispraviti kritične tokove pristupa | Rizik se ne može prenijeti, ali se može rano smanjiti jasnim definisanjem uloga | Osoba za zahtjeve / tehnički lead | Aktivan |
| R-33 | Ključne funkcionalnosti imaju skrivene zavisnosti koje tim kasno uoči | Backlog stavke izgledaju odvojeno, ali zavise od istih podataka i pravila | Visoka | Srednji | Kašnjenje više sprintova jer jedna nezavršena stavka blokira druge | Visok | Mapirati zavisnosti između stavki backloga prije planiranja sprintova | Replanirati sprint i pomjeriti blokirane stavke; jasno označiti dependency chain | Djelimično se može ublažiti boljim release planom | Scrum koordinator / backlog owner | Aktivan |
| R-34 | Sprint backlog bude prenatrpan "bitnim" stvarima | Tim sve doživljava kao prioritet i ne pravi realan izbor | Visoka | Visok | Neispunjen sprint goal, loš review, slabija ocjena i frustracija tima | Kritičan | Ograničiti sprint backlog na ono što je stvarno završivo; koristiti MVP logiku | Odmah smanjiti scope sprinta i zaštititi najvažnije stavke | Rizik se ne prenosi, ali se može smanjiti disciplinom planiranja | Scrum koordinator / cijeli tim | Aktivan |
| R-35 | Tim izgradi sistem koji "radi", ali ne prati stvarna poslovna pravila | Fokus na tehničku izvedbu bez dovoljno validacije sa domenom | Srednja | Visok | Sistem funkcioniše formalno, ali je pogrešan iz perspektive korisnika i PO-a | Visok | Svaki važan feature provjeravati i kroz poslovno pravilo, ne samo tehnički rezultat | Ispraviti pravilo i logiku prije daljeg proširenja funkcionalnosti | Rizik se ne prenosi; mora se rješavati unutar tima kroz bolju validaciju | Cijeli tim | Aktivan |
| R-36 | Prevelika zavisnost implementacije od jednog člana tima | Jedna osoba drži znanje o bazi, backendu ili arhitekturi | Srednja | Visok | Zastoj ako osoba kasni, izostane ili ne stigne prenijeti znanje | Visok | Kritične odluke i rješenja dokumentovati; bar još jedna osoba mora razumjeti svaki ključni dio | Preraspodijeliti zadatke i organizovati brzi prenos znanja na ostatak tima | Rizik se djelimično eliminiše dijeljenjem znanja i dokumentacijom | Scrum koordinator / tehnički lead | Aktivan |
| R-37 | Implementacija autentikacije kasnije uspori više featurea odjednom | Login i role-based pristup su temelj za više budućih stavki | Srednja | Visok | Blokada profila, administracije, upravljanja članovima i drugih funkcionalnosti | Visok | Rano mapirati koje stavke zavise od autentikacije i ne odgađati njen dizajn previše | Privremeno koristiti jednostavniji pristup za demo, ali planirati punu korekciju | Djelimično se može ublažiti faznom implementacijom | Tehnički lead / backlog owner | Aktivan |
| R-38 | Rezervacije budu pogrešno zamišljene kao jednostavna funkcija, iako nose mnogo pravila | Rezervacija zavisi od dostupnosti, rokova, statusa članarine i ponašanja korisnika | Srednja | Visok | Komplikacije u logici, nekonzistentni statusi i problemi pri implementaciji | Visok | Rezervacije tretirati kao kompleksniji feature i unaprijed razraditi pravila | Ako se pokaže previše složenim, svesti MVP rezervacija na osnovni scenario | Rizik se djelimično može ublažiti smanjenjem scope-a funkcionalnosti | Osoba za zahtjeve / tehnički lead | Aktivan |
| R-39 | Članarina bude modelirana previše površno | Fokus samo na "aktivan/istekao" bez razrade rokova, historije i administracije | Srednja | Srednji | Problemi pri provjeri prava člana i kasnijim administrativnim funkcijama | Srednji | Već u modelu predvidjeti osnovne podatke o trajanju i statusu članarine | Doraditi model prije implementacije featurea koji zavise od članarine | Rizik se može značajno smanjiti boljim modeliranjem u Sprintu 3 | Osoba za modeliranje / osoba za zahtjeve | Aktivan |
| R-40 | Tim kasno shvati da su neki featurei preveliki za planirani sprint | User story izgleda jednostavno dok se ne razloži na podzadatke | Visoka | Srednji | Kašnjenje i nedovršene stavke | Visok | Svaki veći feature razložiti na manje taskove prije ulaska u sprint | Pomjeriti dio featurea u naredni sprint i završiti minimum korisne vrijednosti | Rizik se ne prenosi, ali se može ublažiti boljom granularnošću backloga | Scrum koordinator / backlog owner | Aktivan |
| R-41 | Tim previše vjeruje AI prijedlozima za modele, arhitekturu ili logiku | AI daje uvjerljiv, ali ne nužno ispravan prijedlog | Srednja | Visok | Pogrešni artefakti, nerazumijevanje sistema i slaba odbrana odluka | Visok | Sve AI prijedloge ručno provjeriti kroz backlog, pravila i logiku sistema | Odbaciti ili preraditi problematične dijelove i dokumentovati zašto su korigovani | Rizik se ne može prenijeti; odgovornost ostaje na timu | Cijeli tim | Aktivan |
| R-42 | Tehnički dug se počne gomilati jer tim ide linijom manjeg otpora | Brza rješenja bez dovoljno razmišljanja o održivosti | Srednja | Srednji | Kasniji sprintovi postaju sporiji, ispravke skuplje, stabilnost slabija | Srednji | Bilježiti poznata ograničenja i nepraviti “privremena” rješenja bez evidencije | Refaktorisati najkritičnije dijelove prije završne faze | Rizik se djelimično može ublažiti disciplinom i evidencijom tehničkog duga | Tehnički lead / cijeli tim | Aktivan |
| R-43 | Testiranje krene prekasno | Tim sav fokus prebaci na implementaciju pa testiranje ostavi za kraj | Srednja | Visok | Bugovi ostanu skriveni do reviewa ili završne demonstracije | Visok | Za svaki inkrement planirati makar osnovne test scenarije odmah | Prioritet dati testiranju kritičnih tokova i smanjiti scope manje važnih stvari | Rizik se može djelimično ublažiti ako se na vrijeme reže scope | Osoba za kvalitet / cijeli tim | Aktivan |
| R-44 | Završna verzija projekta ostane funkcionalno nepotpuna jer se MVP ne zaštiti dovoljno rano | Tim troši vrijeme na sporedne funkcionalnosti prije nego osigura osnovne | Srednja | Visok | Osnovni sistem ostane nedovršen, iako postoje djelimično urađene naprednije stavke | Visok | Jasno zaključati šta je MVP i braniti ga pri planiranju sprintova | Odbaciti ili odgoditi niže prioritete čim se vidi da tempo nije dovoljan | Rizik se može djelimično prenijeti na odluku o smanjenju scope-a, ali tim mora to uraditi na vrijeme | Scrum koordinator / Product backlog odgovorna osoba | Aktivan |

</div>

---

## 4. Dodatna analiza rizika

### Najkritičniji rizici
Najveći rizici za projekat nisu samo tehnički, nego i organizacioni i procesni. Posebno se izdvajaju:

- **R-01** nerealno planiranje sprinta
- **R-02** nedovoljno razjašnjena poslovna pravila
- **R-03** pogrešno modeliranje domene
- **R-20** loši temelji Sprinta 3
- **R-22** loše definisana autentikacija i autorizacija
- **R-25** pad kvaliteta implementacije pred kraj
- **R-26** preslabo testiranje

To su rizici koji mogu proizvesti domino-efekat kroz više sprintova.

### Organizacijski rizici
Projekat je nekako posebno osjetljiv na:
- odsustvo članova
- praznike i preklapanje rokova
- slabiju komunikaciju
- odustajanje člana
- konflikte verzija
- nejasnu raspodjelu odgovornosti

Zato su ovi rizici uključeni iako nisu "tehnički", jer u praksi često prave više problema od samog koda.

### Rizici koji rastu u kasnijim sprintovima
Neki rizici u Sprintu 3 djeluju samo kao upozorenje, ali kasnije postaju mnogo opasniji:

- problematičan dizajn baze
- loša raspodjela featurea po sprintovima
- kašnjenje testiranja
- zanemarivanje projektnih artefakata
- gomilanje tehničkog duga

---

## 5. Strategija praćenja rizika

Tim će rizike pratiti na sljedeći način:

1. Na kraju svakog sprinta pregledati da li se promijenila vjerovatnoća ili uticaj nekog rizika.
2. Ako se rizik djelimično ili potpuno ostvari, promijeniti njegov status.
3. Ako se pojavi novi ozbiljan rizik, dodati ga u registar.
4. Za sve visoke i kritične rizike provjeriti da li su preventivne mjere zaista primijenjene.
5. Rizike koristiti kao ulaz za planiranje sprinta, a ne samo kao formalni dokument.

---

## 6. Mogući statusi rizika

- **Aktivan** - rizik je relevantan i treba ga pratiti
- **U porastu** - vjerovatnoća ili uticaj su povećani
- **Ublažen** - uvedene su mjere koje su smanjile rizik
- **Ostvaren** - rizik se desio i sada se rješava kao problem
- **Zatvoren** - više nije relevantan

Trenutno je većina rizika označena kao **Aktivan**, jer projekat još nije dovoljno odmakao da bi veći broj njih bio zatvoren ili potpuno ublažen.

---
