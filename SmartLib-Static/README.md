# SmartLib — statična verzija (cPanel)

Ovo je potpuno statična (HTML/CSS/JS) verzija SmartLib biblioteke, migrirana iz originalnog
ASP.NET Core projekta (`../Projekat`). Nema servera, baze ni API poziva — svi podaci žive u
**localStorage** vašeg browsera i generišu se iz seed podataka pri prvom otvaranju stranice.

Namijenjeno je za demo/prezentaciju na **cPanel** hostingu (ili bilo kojem statičkom hostingu) —
originalni `.NET` projekat se ne dira i ostaje netaknut u `../Projekat`.

## Demo nalozi

| Uloga | Email | Lozinka |
|---|---|---|
| Administrator | admin@smartlib.ba | admin123 |
| Bibliotekar | bibliotekar@smartlib.ba | biblio123 |
| Bibliotekar (2) | haris.delic@smartlib.ba | biblio123 |
| Član | clan@smartlib.ba | clan123 |
| Član (2) | lejla.music@example.com | clan123 |
| Član (3) | tarik.besic@example.com | clan123 |

Možete se i sami registrovati kao novi Član preko "Registruj se" na stranici za prijavu.

## Kako radi (arhitektura)

- `assets/js/db.js` — mala "baza" preko localStorage (get/set/insert/update/remove/paginate).
- `assets/js/mock-data.js` — seed podaci (knjige, korisnici, zaduženja, itd.) — ubacuju se
  samo jednom, pri prvom učitavanju bilo koje stranice.
- `assets/js/auth.js` — sesija (ulogovani korisnik) u localStorage, login/logout/register,
  `Auth.guard(["Uloga",...])` štiti stranice po ulozi (redirect na login ili "Nemate pristup").
- `assets/js/layout.js` — prikazuje/skriva navigacijske stavke prema trenutnoj ulozi
  (`data-role="..."` i `data-auth="in|out"` atributi), dark mode, bočni meni (drawer),
  broj nepročitanih obavještenja.
- `assets/js/common.js` — zajednički UI helperi (zvjezdice, statusni bedževi, paginacija,
  formatiranje datuma, "flash" poruke nakon redirekcije).
- `assets/js/animations.js` + `assets/css/animations.css` — **kopirano doslovno** iz originalnog
  projekta (bili su već čist klijentski JS/CSS bez server zavisnosti) — cinematic "book opening"
  animacija dobrodošlice, "bookshelf" prelaz pri ulasku u katalog, stagger animacije menija.
- `assets/css/site.css` — **kopiran doslovno** iz originalnog projekta — svi vizuali (boje,
  dugmad, kartice, tabele, tamna tema) su identični originalu.
- `assets/js/pages/*.js` — po jedan JS fajl po stranici, čita/piše mock "bazu" i renderuje DOM.

Svaka HTML stranica je samostalna (nav/drawer markup je inline na svakoj stranici, ne fetch-uje
se sa servera) — radi identično i kad se otvori direktno kao fajl i kad se hostuje na cPanel-u.

## Resetovanje mock podataka

Otvorite konzolu u browseru na bilo kojoj stranici i pokrenite:

```js
DB.resetAll(); location.reload();
```

Ovo briše sve podatke iz localStorage-a; pri sljedećem učitavanju stranice će se ponovo
seed-ovati početni demo podaci.

## Poznata pojednostavljenja u odnosu na original

- Nema pravog servera/baze/emaila — sve poslovne provjere (npr. "ne možeš rezervisati ako imaš
  kašnjenje", "3 prijave = 7 dana banovan") simulirane su u JS-u nad mock podacima.
- Korice knjiga su generisani SVG placeholderi (inicijali + boja po naslovu) umjesto poziva ka
  OpenLibrary/Google Books API-ju iz originala.
- Markdown u vijestima se renderuje preko `marked.js` (CDN) umjesto server-side Markdig-a.
- `Vijest`/`Kalendar` create/edit forme su ugrađene kao modal na `index.html` (originalni
  odvojeni `Upsert.cshtml` fallback nije repliciran — modal pokriva istu funkcionalnost).
- `Admin/Korisnici` je ostao minimalan placeholder, isto kao u originalnom projektu (bio je
  neimplementiran TODO).

## Postavljanje na cPanel

1. Zakačite (zip) cijeli `SmartLib-Static/` folder.
2. U cPanel File Manager-u otpakujte ga u `public_html/` (ili u podfolder ako želite poddomenu).
3. Otvorite `https://vasadomena.com/index.html` (ili samo `https://vasadomena.com/` ako je
   `index.html` postavljen kao root).
4. Nema potrebe za bazom, PHP-om, ili bilo kakvom serverskom konfiguracijom — sve je statično.
