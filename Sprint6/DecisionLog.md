# Decision Log

## DL-04
Datum: 30.04.2026  
Naziv odluke: Uvođenje dvostrukog unosa lozinke prilikom kreiranja naloga  

Opis problema:  
Prilikom kreiranja korisničkog naloga postojala je mogućnost da korisnik napravi grešku pri unosu lozinke, što bi moglo rezultirati nemogućnošću prijave u sistem odmah nakon registracije. Potrebno je bilo odlučiti kako smanjiti rizik od pogrešno unesene lozinke.  

Razmatrane opcije:  
- Jednostruki unos lozinke sa naknadnom promjenom putem administratora  
- Dvostruki unos lozinke (polje za potvrdu lozinke) prilikom kreiranja naloga  

Odabrana opcija:  
Dvostruki unos lozinke sa poljem za potvrdu  

Razlog izbora:  
Dvostruki unos lozinke je standardna praksa u web aplikacijama koja značajno smanjuje rizik od grešaka pri unosu. Ovaj pristup ne zahtijeva dodatne sistemske resurse i pruža korisnicima trenutnu povratnu informaciju ukoliko se unosi ne poklapaju. Alternativa sa naknadnom promjenom preko administratora bi stvorila nepotreban dodatni posao za osoblje biblioteke.  

Posljedice odluke:  
- Smanjen rizik od pogrešno unesene lozinke prilikom registracije  
- Potrebna validacija na frontendu i backendu da se oba polja poklapaju  
- Blago proširena forma za kreiranje naloga  
- Poboljšano korisničko iskustvo pri prvoj prijavi  

Status:  
Aktivna  

Povezani PB:  
PB-18  

---

## DL-05
Datum: 30.04.2026  
Naziv odluke: Implementacija funkcionalnosti za promjenu zaboravljene lozinke  

Opis problema:  
Korisnici koji zaborave svoju lozinku nisu imali mogućnost da je samostalno promijene. Jedini način je bio da kontaktiraju administratora ili bibliotekara, što je neefikasno i nije u skladu sa standardnim očekivanjima korisnika od web aplikacije. Potrebno je bilo odlučiti da li i na koji način omogućiti korisnicima promjenu lozinke.  

Razmatrane opcije:  
- Ručna promjena lozinke od strane administratora na zahtjev korisnika  
- Implementacija dugmeta "Forgot Password / Change Password" sa mogućnošću samostalne promjene lozinke  

Odabrana opcija:  
Implementacija dugmeta za promjenu lozinke (Forgot / Change Password)  

Razlog izbora:  
Samostalna promjena lozinke smanjuje opterećenje na administrativno osoblje i omogućava korisnicima brz i siguran pristup svom nalogu bez čekanja. Ovaj pristup je u skladu sa uobičajenim standardima za upravljanje korisničkim nalozima u web aplikacijama i poboljšava ukupno korisničko iskustvo.  

Posljedice odluke:  
- Korisnici mogu samostalno promijeniti lozinku bez kontaktiranja administratora  
- Potrebna implementacija korisničkog interfejsa za promjenu lozinke  
- Potrebna backend logika za validaciju i ažuriranje lozinke u bazi podataka  
- Smanjeno opterećenje na osoblje biblioteke  

Status:  
Aktivna  

Povezani PB:  
PB-18  

---

## DL-06
Datum: 30.04.2026  
Naziv odluke: Ciljane UI izmjene umjesto potpunog redizajna aplikacije  

Opis problema:  
Korisnički interfejs aplikacije je zahtijevao vizualna poboljšanja kako bi sistem izgledao profesionalnije i bio prikladniji za upotrebu u kontekstu bibliotečkog informacionog sistema. Pitanje je bilo da li izvršiti potpuni redizajn cijelog interfejsa ili se fokusirati samo na ključne dijelove.  

Razmatrane opcije:  
- Potpuni redizajn cijelog korisničkog interfejsa  
- Ciljane izmjene samo na relevantnim dijelovima interfejsa (navigacija, layout, forme za rad sa knjigama)  

Odabrana opcija:  
Ciljane izmjene na relevantnim dijelovima interfejsa  

Razlog izbora:  
Potpuni redizajn bi donio značajan rizik regresija i zahtijevao bi više vremena nego što je bilo predviđeno za Sprint 6. Ciljane izmjene omogućavaju brzu isporuku vidljivih poboljšanja bez narušavanja postojećih funkcionalnosti. Ovaj pristup zadržava konzistentnost sa već implementiranim dijelovima aplikacije i smanjuje rizik od uvođenja novih grešaka.  

Posljedice odluke:  
- Poboljšan izgled navigacije, layouta i ključnih formi  
- Profesionalniji i moderniji izgled aplikacije  
- Izbjegnut rizik velikog refaktora postojećih view-ova i stilova  
- Neke stranice zadržavaju prethodni izgled do budućih sprintova  

Status:  
Aktivna  

Povezani PB:  
PB-22, PB-28  