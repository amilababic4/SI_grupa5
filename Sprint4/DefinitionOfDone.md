# Definition of Done (DoD) - Bibliotečki informacioni sistem

Ovaj dokument definiše jedinstvene kriterije na osnovu kojih se procjenjuje da li je određeni User Story završen.  
Definition of Done osigurava konzistentan kvalitet implementacije i zajedničko razumijevanje unutar tima.

User Story se smatra *Done* isključivo ako su ispunjeni svi sljedeći kriteriji:

---

### 1. Implementacija
- Funkcionalnost je implementirana u skladu sa opisom User Story-ja, uključujući sve definisane scenarije i poslovna pravila  
- Aplikacija se uspješno builda bez grešaka i upozorenja, te je spremna za pokretanje u testnom okruženju  

---

### 2. Acceptance kriteriji
- Svi [acceptance kriteriji](../Sprint3/UpdatedSetOfUserStories.md) iz odgovarajućeg User Story-ja su u potpunosti ispunjeni i verifikovani  
- Funkcionalnost je validirana kroz definisane scenarije korištenja i ponaša se u skladu sa očekivanim rezultatima    

---

### 3. Testiranje
- Implementirani su odgovarajući testovi u skladu sa [test strategijom](../Sprint3/TestStrategy.md)  
- Svi testovi su uspješno izvršeni i prolaze bez grešaka  
- Ne postoje otvoreni kritični niti blokirajući defekti povezani sa ovom funkcionalnošću  

---

### 4. Code review
- Kod je pregledan od strane najmanje jednog člana tima kroz proces code review-a  
- Sve primjedbe iz pregleda koda su riješene ili adekvatno obrazložene    

---

### 5. Repozitorij i integracija
- Promjene su verzionisane kroz commit i push na repozitorij  
- Izvršen je merge na odgovarajući branch u skladu sa definisanim timskim workflow-om   

---

### 6. Spremnost za demonstraciju
- Funkcionalnost se može uspješno pokrenuti lokalno iz repozitorija i dostupna je za testiranje svim članovima tima.  
- Spremna je za demonstraciju u okviru sprint review-a bez poznatih kritičnih problema   

---

<br>

> Napomena: Nijedan User Story ne može biti označen kao *Done* ukoliko makar jedan od navedenih kriterija nije ispunjen.