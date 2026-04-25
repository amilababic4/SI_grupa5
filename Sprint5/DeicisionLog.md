## Decision Log

### DL-01
Datum: 25.04.2026  
Naziv odluke: Korištenje Cookie autentifikacije za Web  
Opis problema: Potrebno upravljati sesijom korisnika  
Razmatrane opcije: JWT, Cookie  
Odabrana opcija: Cookie  
Razlog izbora: Jednostavnija implementacija za MVC aplikaciju  
Posljedice: Lakše upravljanje sesijom  
Status: Aktivna  
Povezani PB: PB-17

---

### DL-02
Datum: 25.04.2026  
Naziv odluke: Korištenje JWT za API  
Opis problema: Potrebna autentifikacija API poziva  
Razmatrane opcije: Cookie, JWT  
Odabrana opcija: JWT  
Razlog izbora: Standard za API sigurnost  
Posljedice: Potrebna konfiguracija tokena  
Status: Aktivna  
Povezani PB: PB-17

---

### DL-03
Datum: 25.04.2026  
Naziv odluke: Generička poruka greške  
Opis problema: Sigurnost login sistema  
Razmatrane opcije: Specifične poruke vs generička  
Odabrana opcija: Generička  
Razlog izbora: Sprječava otkrivanje informacija  
Posljedice: Manje informacija korisniku  
Status: Aktivna 
Povezani PB: PB-17 

