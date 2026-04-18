# Branching Strategy

## 1. Uvod

Ovaj dokument opisuje odabranu strategiju grananja (branching strategy) za repozitorij projekta **SmartLib**. Strategija je odabrana na osnovu veličine tima, kompleksnosti projekta, razvojnog perioda i platforme za upravljanje kodom (GitHub).

---

## 2. Odabrana strategija: GitHub Flow

### 2.1 Obrazloženje izbora

1. **Veličina i iskustvo tima.** Tim od 8 članova na prvom zajedničkom projektu. GitHub Flow je najjednostavnija strategija za brzo usvajanje.
2. **Platforma.** Repozitorij je na GitHub-u — Pull Requests, code review i merge su nativno podržani.
3. **Razvojni period.** Sa ~7 sedmica, overhead kompleksnijih strategija nije opravdan.
4. **Jedna aktivna verzija.** SmartLib nema potrebu za više paralelnih verzija.
5. **Scrum.** Sprint-baziran razvoj sa user story-jima se prirodno mapira na feature grane.
6. **Code review kultura.** Obavezni PR-ovi potiču kvalitet koda i razmjenu znanja.

---

## 3. Pravila i konvencije

### 3.1 Struktura grana

| Grana | Svrha | Životni vijek |
|---|---|---|
| `main` | Uvijek stabilan, deployable kod | Permanentna |
| `feature/<opis>` | Nova funkcionalnost | Kratkoživuća (do merge-a) |
| `bugfix/<opis>` | Ispravka greške | Kratkoživuća (do merge-a) |
| `docs/<opis>` | Dokumentacija | Kratkoživuća (do merge-a) |

### 3.2 Imenovanje grana

Format: `<tip>/<kratak-opis>`

Primjeri: `feature/login-sistem`, `feature/crud-knjige`, `bugfix/validacija-email`, `docs/arhitektura-update`

### 3.3 Workflow

```
1. Kreirati granu iz main:     git checkout -b feature/naziv
2. Commitovati promjene:       git commit -m "feat: opis"
3. Pushati na GitHub:           git push origin feature/naziv
4. Otvoriti Pull Request na GitHub-u (base: main)
5. Code review: min. 1 odobrenje
6. Merge: "Squash and merge", obrisati feature granu
```

### 3.4 Konvencija commit poruka

Format: `<tip>: <kratak opis>`

| Tip | Značenje |
|---|---|
| `feat:` | Nova funkcionalnost |
| `fix:` | Ispravka greške |
| `docs:` | Dokumentacija |
| `refactor:` | Refaktorisanje bez promjene funkcionalnosti |
| `test:` | Testovi |
| `chore:` | Održavanje (konfiguracija, zavisnosti) |

### 3.5 Pravila za Pull Requests

- Svaki PR mora imati opis promjena
- Minimalno 1 odobrenje prije merge-a
- Ne commitovati direktno na `main`
- Jedan PR = jedna funkcionalnost ili ispravka
- Riješiti sve komentare reviewera prije merge-a

---

## 4. Branch Protection Rules (GitHub)

- Require a pull request before merging
- Require approvals (1)
- Dismiss stale approvals on new commits
- Do not allow bypassing the above settings

---

## 5. Zaključak

GitHub Flow pruža optimalan balans između jednostavnosti i strukture za SmartLib. Strategija omogućava fokus na razvoj uz minimum ceremonije, dok PR workflow osigurava kvalitet koda kroz code review.
