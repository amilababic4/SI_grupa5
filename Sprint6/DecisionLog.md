# Decision Log

## DL-01
Datum: 25.04.2026  
Naziv odluke: Korištenje Cookie autentifikacije za Web  

Opis problema:
Potrebno upravljati korisničkom sesijom u Web aplikaciji  

Razmatrane opcije:
- JWT autentifikacija  
- Cookie autentifikacija  

Odabrana opcija:
Cookie autentifikacija  

Razlog izbora:
Jednostavnija implementacija u MVC aplikaciji i prirodna podrška za sesije  

Posljedice odluke:
- Lakše upravljanje sesijom  
- Manja kompleksnost implementacije  

Status:
Aktivna  

Povezani PB:
PB-17  

---

## DL-02
Datum: 25.04.2026  
Naziv odluke: Korištenje JWT za API  

Opis problema:
Potrebna autentifikacija i zaštita API endpointa  

Razmatrane opcije:
- Cookie autentifikacija  
- JWT autentifikacija  

Odabrana opcija:
JWT autentifikacija  

Razlog izbora:
Standardno rješenje za sigurnu komunikaciju između klijenta i servera  

Posljedice:
- Potrebna konfiguracija tokena  
- Dodatna kompleksnost u odnosu na cookie  

Status:
Aktivna  

Povezani PB:
PB-17  

---

## DL-03
Datum: 25.04.2026  
Naziv odluke: Korištenje generičke poruke greške pri loginu  

Opis problema:
Potrebno spriječiti otkrivanje informacija o korisničkim podacima  

Razmatrane opcije:
- Specifične poruke (email ne postoji / lozinka pogrešna)  
- Generička poruka  

Odabrana opcija:
Generička poruka  

Razlog izbora:
Povećava sigurnost sistema i sprječava napade  

Posljedice:
- Manje informacija korisniku  
- Veća sigurnost sistema  

Status:
Aktivna  

Povezani PB:
PB-17  