# Branching Strategy

## 1. Uvod

Ovaj dokument opisuje odabranu strategiju grananja (branching strategy) za repozitorij projekta **SmartLib**. Strategija je odabrana na osnovu veličine tima, kompleksnosti projekta, razvojnog perioda i platforme za upravljanje kodom (GitHub).

---

## 2. Pregled najčešćih branching strategija

Prije odabira strategije, tim je razmotrio četiri najčešće korištene branching strategije u industriji:

| Strategija | Permanentne grane | Kratkoživuće grane | Kompleksnost | Tipična primjena |
|---|---|---|---|---|
| **GitFlow** | `main`, `develop`, `release/*`, `hotfix/*` | `feature/*` | Visoka | Veći timovi, planirani release ciklusi, više verzija u produkciji |
| **GitLab Flow** | `main`, environment grane (`staging`, `production`) | `feature/*` | Srednja | Uhodani timovi sa CI/CD pipelineom i više deployment okruženja |
| **GitHub Flow** | `main` | `feature/*`, `bugfix/*` | Niska | Manji timovi, jedna aktivna verzija, kontinuirani deployment |
| **Trunk-Based Development** | `main` (trunk) | Vrlo kratkoživuće (ili nikakve) | Vrlo niska | Iskusni timovi sa jakim CI/CD-om i feature flagovima |

### 2.1 GitFlow

GitFlow koristi dvije permanentne grane (`main` i `develop`) te tri tipa kratkoživućih grana (`feature`, `release`, `hotfix`). Razvoj se odvija na `develop` grani, features se merge-aju u `develop`, a zatim se periodično kreira `release` grana za stabilizaciju i konačni merge u `main`.

**Prednosti:** Pogodan za projekte sa više paralelnih verzija u produkciji, striktna separacija razvoja od produkcije.
**Nedostaci:** Značajan overhead — 5 tipova grana, kompleksna merge pravila, duži release ciklus. Za timove bez iskustva u Git-u, ova strategija može biti izvor grešaka i konflikata.

### 2.2 GitLab Flow

GitLab Flow je srednje kompleksna strategija koja dodaje environment grane (npr. `staging`, `production`) na osnovni feature-branch model. Promjene teku: `feature` → `main` → `staging` → `production`, čime se uvodi kontrolirani promotion kod između okruženja.

**Prednosti:** Odličan za timove sa više deployment okruženja i CI/CD integracijama. Prirodno podržava staged rollout.
**Nedostaci:** Zahtijeva uhodan tim koji razumije environment promotion. Pretpostavlja CI/CD infrastrukturu i više deployment okruženja, što za ovaj projekat nije predviđeno u početnoj fazi.

### 2.3 GitHub Flow

GitHub Flow je najjednostavnija strategija: postoji samo jedna permanentna grana (`main`) koja je uvijek deployable. Svaka promjena se razvija na kratkoživućoj feature grani, prolazi code review kroz Pull Request, i merge-a natrag u `main`.

**Prednosti:** Minimalna krivulja učenja, nativna podrška na GitHub-u, fokus na code review kroz PR-ove.
**Nedostaci:** Nema formalnu separaciju okruženja niti release grane — pretpostavlja da je `main` uvijek spreman za deployment.

### 2.4 Trunk-Based Development

Trunk-Based Development podrazumijeva da svi developeri commitaju direktno na `main` (trunk), uz vrlo kratke feature grane (ispod 1 dana). Podrška za nedovršene feature-e se postiže feature flagovima.

**Prednosti:** Najbrži feedback loop, minimalni merge konflikti.
**Nedostaci:** Zahtijeva visok nivo discipline, sveobuhvatan CI/CD, feature flag infrastrukturu i iskusne developere. Rizičan za timove bez prethodnog iskustva.

---

## 3. Odabrana strategija: GitHub Flow

### 3.1 Od čega se sastoji GitHub Flow

GitHub Flow se temelji na sljedećim principima:

1. **Jedna permanentna grana (`main`)** — uvijek sadrži stabilan, deployable kod
2. **Feature grane** — svaka nova funkcionalnost ili ispravka se razvija na zasebnoj grani kreiranoj iz `main`
3. **Pull Request (PR)** — jedini način da se kod merge-a u `main`, obavezan code review
4. **Code Review** — minimalno jedan odobreni review prije merge-a
5. **Merge i brisanje** — nakon odobrenja, grana se merge-a (squash and merge) i briše

### 3.2 Zašto GitHub Flow, a ne druge strategije

| Kriterij | GitFlow | GitLab Flow | GitHub Flow | Trunk-Based |
|---|---|---|---|---|
| Kompleksnost za početnike | Visoka | Srednja | Niska | Zahtijeva disciplinu |
| Potreba za više okruženja | Da | Da | Ne | Ne |
| Više verzija u produkciji | Da | Djelomično | Ne | Ne |
| GitHub nativna podrška | Djelomično | Optimiziran za GitLab | Potpuna | Djelomično |
| CI/CD infrastruktura potrebna | Poželjna | Obavezna | Opciona | Obavezna |

**Konkretno obrazloženje izbora za SmartLib:**

1. **Veličina i iskustvo tima.** Tim od 8 članova na prvom zajedničkom projektu. GitHub Flow je najjednostavnija strategija za brzo usvajanje — GitFlow bi unio nepotrebnu kompleksnost, a Trunk-Based zahtijeva iskustvo koje tim još nema.
2. **Platforma.** Repozitorij je na GitHub-u — Pull Requests, code review i merge su nativno podržani. GitLab Flow je optimiziran za GitLab platformu.
3. **Razvojni period.** Sa ~7 sedmica, overhead kompleksnijih strategija (GitFlow sa 5 tipova grana, GitLab Flow sa environment promotion) nije opravdan.
4. **Jedna aktivna verzija.** SmartLib nema potrebu za više paralelnih verzija u produkciji, čime otpada glavni argument za GitFlow.
5. **Jedno deployment okruženje.** U početnoj fazi projekat nema staging/production separaciju, čime otpada glavni argument za GitLab Flow.
6. **Scrum.** Sprint-baziran razvoj sa user story-jima se prirodno mapira na feature grane u GitHub Flow.
7. **Code review kultura.** Obavezni PR-ovi potiču kvalitet koda i razmjenu znanja unutar tima.

---

## 4. Pravila i konvencije

### 4.1 Struktura grana

| Grana | Svrha | Životni vijek |
|---|---|---|
| `main` | Uvijek stabilan, deployable kod | Permanentna |
| `feature/<opis>` | Nova funkcionalnost | Kratkoživuća (do merge-a) |
| `bugfix/<opis>` | Ispravka greške | Kratkoživuća (do merge-a) |
| `docs/<opis>` | Dokumentacija | Kratkoživuća (do merge-a) |

### 4.2 Imenovanje grana

Format: `<tip>/<kratak-opis>`

Primjeri: `feature/login-sistem`, `feature/crud-knjige`, `bugfix/validacija-email`, `docs/arhitektura-update`

### 4.3 Workflow

```
1. Kreirati granu iz main:     git checkout -b feature/naziv
2. Commitovati promjene:       git commit -m "feat: opis"
3. Pushati na GitHub:           git push origin feature/naziv
4. Otvoriti Pull Request na GitHub-u (base: main)
5. Code review: min. 1 odobrenje
6. Merge: "Squash and merge", obrisati feature granu
```

### 4.4 Konvencija commit poruka

Format: `<tip>: <kratak opis>`

| Tip | Značenje |
|---|---|
| `feat:` | Nova funkcionalnost |
| `fix:` | Ispravka greške |
| `docs:` | Dokumentacija |
| `refactor:` | Refaktorisanje bez promjene funkcionalnosti |
| `test:` | Testovi |
| `chore:` | Održavanje (konfiguracija, zavisnosti) |

### 4.5 Pravila za Pull Requests

- Svaki PR mora imati opis promjena
- Minimalno 1 odobrenje prije merge-a
- Ne commitovati direktno na `main`
- Jedan PR = jedna funkcionalnost ili ispravka
- Riješiti sve komentare reviewera prije merge-a

---

## 5. Branch Protection Rules (GitHub)

- Require a pull request before merging
- Require approvals (1)
- Dismiss stale approvals on new commits
- Do not allow bypassing the above settings

---

## 6. Zaključak

GitHub Flow pruža optimalan balans između jednostavnosti i strukture za SmartLib. Strategija omogućava fokus na razvoj uz minimum ceremonije, dok PR workflow osigurava kvalitet koda kroz code review.
