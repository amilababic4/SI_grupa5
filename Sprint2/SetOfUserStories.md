# Set of User Stories

## Opis dokumenta

Ovaj dokument sadrži sve User Stories za projekat Bibliotečkog informacionog sistema, raspoređene prema planu sprintova. Svaka stavka iz Product Backloga je razrađena u jednu ili više User Story jedinica.


# Sprint 5

## US-01: Implementacija registracije korisnika

| **ID storyja** | US-01 |
|---------------|-------|
| **Naziv storyja** | Implementacija registracije korisnika |
| **Opis** | Kao bibliotekar ili administrator, želim kreirati nalog za novog člana unosom njegovih podataka, kako bi član mogao koristiti usluge biblioteke. |
| **Poslovna vrijednost** | Centralizovana kontrola nad kreiranjem naloga osigurava tačnost podataka i sprječava neovlašten pristup sistemu. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Bibliotekar ili administrator je unio podatke: ime, prezime, email i lozinku.<br>Email mora biti jedinstven. <br>Lozinka mora imati minimalno 8 znakova.<br>Nakon uspješnog unosa, kreira se nalog sa ulogom 'Član'.|
| **Pretpostavke / Otvorena pitanja** | Član fizički dolazi u biblioteku i daje svoje podatke osoblju. |
| **Veze i zavisnosti** | Domain Model (entitet Korisnik) |

---

## US-02: Implementacija prijave korisnika

| **ID storyja** | US-02 |
|---------------|-------|
| **Naziv storyja** | Implementacija prijave korisnika u sistem |
| **Opis** | Kao registrovani korisnik (Član, Bibliotekar ili Administrator), želim se prijaviti putem email-a i lozinke, kako bih pristupio funkcionalnostima prema svojoj ulozi. |
| **Poslovna vrijednost** | Prijava je osnova sigurnosti sistema – bez nje nije moguće kontrolisati pristup. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Korisnik se može prijaviti sa email-om i lozinkom.<br>Nakon prijave, preusmjeren je na odgovarajući dashboard prema ulozi.|
| **Pretpostavke / Otvorena pitanja** | Korisnici imaju definisane uloge: Član, Bibliotekar ili Administrator. |
| **Veze i zavisnosti** | US-01: registracija korisnika |

---

## US-03: Implementacija odjave korisnika

| **ID storyja** | US-03 |
|---------------|-------|
| **Naziv storyja** | Implementacija odjave korisnika |
| **Opis** | Kao prijavljeni korisnik, želim se odjaviti iz sistema, kako bi moj nalog bio zaštićen. |
| **Poslovna vrijednost** | Odjava je ključna sigurnosna funkcionalnost. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | Dugme za odjavu je dostupno u navigaciji na svim stranicama.<br>Korisnik je preusmjeren na stranicu za prijavu. |
| **Pretpostavke / Otvorena pitanja** |  |
| **Veze i zavisnosti** | US-02: prijava |

---

## US-04: Uspostava AI Usage Loga i Decision Loga

| **ID storyja** | US-04 |
|---------------|-------|
| **Naziv storyja** | Uspostava AI Usage Loga i Decision Loga |
| **Opis** | Kao tim, želimo uspostaviti strukturirane logove za praćenje korištenja AI alata i tehničkih odluka, kako bi naš proces bio transparentan i dokumentovan. |
| **Poslovna vrijednost** | AI Usage Log i Decision Log su obavezni artefakti od Sprinta 5. |
| **Prioritet** | Visok |
| **Acceptance Criteria** | AI Usage Log je kreiran u repozitoriju sa definisanom strukturom unosa.<br>Decision Log je kreiran sa definisanom strukturom.<br>Interni dogovor tima o procesu ažuriranja logova. |
| **Pretpostavke / Otvorena pitanja** |  |
| **Veze i zavisnosti** | Definisana struktura projektne dokumentacije |