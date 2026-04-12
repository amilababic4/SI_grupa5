# 📚 Use Case Model

**Projekt:** Bibliotečki informacioni sistem

---

## 🎭 Pregled aktera

* **Član biblioteke** – koristi sistem za pregled, rezervaciju i posudbu knjiga
* **Bibliotekar** – upravlja knjigama, članovima i zaduženjima
* **Administrator** – upravlja korisnicima i sistemskim funkcijama
* **Sistem** – automatski izvršava određene procese (notifikacije, kazne, logovi)

---

# 📌 Use Case 1: Registracija člana

* **Akter:** Bibliotekar
* **Opis:** Bibliotekar unosi podatke novog člana u sistem

### Preduslovi

* Bibliotekar je prijavljen u sistem

### Glavni tok

1. Bibliotekar otvara formu za registraciju
2. Unosi ime, prezime, email i lozinku
3. Sistem validira podatke
4. Sistem kreira korisnički nalog

### Alternativni tokovi

* Neispravni podaci → prikaz greške
* Email već postoji → odbijanje registracije

### Ishod

* Novi član je registrovan u sistemu

---

# 📌 Use Case 2: Prijava u sistem

* **Akter:** Korisnik
* **Opis:** Korisnik se prijavljuje u sistem

### Preduslovi

* Korisnik ima kreiran nalog

### Glavni tok

1. Korisnik unosi email i lozinku
2. Sistem provjerava podatke
3. Sistem omogućava pristup

### Alternativni tokovi

* Pogrešni podaci → greška

### Ishod

* Korisnik je prijavljen

---

# 📌 Use Case 3: Pregled kataloga knjiga

* **Akter:** Član biblioteke
* **Opis:** Korisnik pregleda dostupne knjige

### Preduslovi

* Korisnik je prijavljen

### Glavni tok

1. Korisnik otvara katalog
2. Sistem prikazuje listu knjiga
3. Korisnik pregledava knjige

### Ishod

* Prikazan katalog knjiga

---

# 📌 Use Case 4: Pretraga knjiga

* **Akter:** Član biblioteke
* **Opis:** Korisnik pretražuje knjige po naslovu ili autoru

### Glavni tok

1. Korisnik unosi pojam za pretragu
2. Sistem filtrira rezultate
3. Sistem prikazuje odgovarajuće knjige

### Alternativni tokovi

* Nema rezultata → prikaz poruke

### Ishod

* Prikazani rezultati pretrage

---

# 📌 Use Case 5: Dodavanje knjige

* **Akter:** Bibliotekar
* **Opis:** Bibliotekar dodaje novu knjigu u sistem

### Preduslovi

* Bibliotekar je prijavljen

### Glavni tok

1. Bibliotekar unosi podatke o knjizi
2. Sistem validira podatke
3. Sistem sprema knjigu u katalog

### Alternativni tokovi

* Neispravni podaci → greška
* ISBN već postoji → odbijanje

### Ishod

* Knjiga dodana u sistem

---

# 📌 Use Case 6: Uređivanje knjige

* **Akter:** Bibliotekar

### Glavni tok

1. Bibliotekar odabire knjigu
2. Mijenja podatke
3. Sistem sprema izmjene

### Ishod

* Podaci ažurirani

---

# 📌 Use Case 7: Brisanje knjige

* **Akter:** Bibliotekar

### Glavni tok

1. Bibliotekar bira knjigu
2. Potvrđuje brisanje
3. Sistem briše knjigu

### Alternativni tokovi

* Aktivno zaduženje → brisanje nije dozvoljeno

### Ishod

* Knjiga obrisana

---

# 📌 Use Case 8: Evidencija zaduživanja

* **Akter:** Bibliotekar

### Glavni tok

1. Bibliotekar bira člana
2. Bira knjigu i primjerak
3. Potvrđuje zaduživanje
4. Sistem evidentira zaduženje

### Ishod

* Knjiga zadužena

---

# 📌 Use Case 9: Evidencija vraćanja

* **Akter:** Bibliotekar

### Glavni tok

1. Bibliotekar bira aktivno zaduženje
2. Potvrđuje vraćanje
3. Sistem ažurira status

### Ishod

* Knjiga vraćena

---

# 📌 Use Case 10: Rezervacija knjige

* **Akter:** Član biblioteke

### Glavni tok

1. Korisnik otvara detalje knjige
2. Klikne "Rezerviši"
3. Sistem kreira rezervaciju

### Alternativni tokovi

* Knjiga dostupna → nema rezervacije

### Ishod

* Rezervacija kreirana

---

# 📌 Use Case 11: Upravljanje korisnicima

* **Akter:** Administrator

### Glavni tok

1. Administrator pregleda korisnike
2. Mijenja ulogu ili deaktivira nalog

### Ishod

* Korisnik ažuriran

---

# 📌 Use Case 12: Pregled profila

* **Akter:** Korisnik

### Glavni tok

1. Korisnik otvara profil
2. Sistem prikazuje podatke i zaduženja

### Ishod

* Prikazan profil

---

# 📌 Use Case 13: Slanje email notifikacija

* **Akter:** Sistem

### Glavni tok

1. Sistem prati rokove vraćanja
2. Šalje podsjetnike i upozorenja

### Ishod

* Email poslan korisniku

---

# 📌 Use Case 14: Generisanje izvještaja

* **Akter:** Administrator

### Glavni tok

1. Administrator bira tip izvještaja
2. Sistem generiše podatke

### Ishod

* Izvještaj generisan

---

# 🏁 Zaključak

Use case model jasno prikazuje interakciju između aktera i sistema kroz ključne funkcionalnosti. Model je izveden direktno iz user storyja i acceptance criteria, čime se osigurava konzistentnost između zahtjeva i implementacije sistema.

---
