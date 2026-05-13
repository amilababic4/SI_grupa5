# Decision Log

## Opis dokumenta
Decision Log se koristi za evidentiranje važnih projektnih, zahtjevnih, arhitektonskih, tehničkih i
procesnih odluka.


## DL-01
**Datum**: 11.05.2026  
**Naziv odluke:** Premještanje Product Backlog itema PB-35, PB-36 i PB-37 iz planiranih budućih sprintova u Sprint 7 zbog njihove funkcionalne povezanosti sa modulom zaduživanja i optimizacije razvoja sistema.

**Opis problema:**
Tokom razvoja funkcionalnosti vezanih za zaduživanje knjiga u Sprintu 7 uočeno je da su Product Backlog itemi PB-35, PB-36 i PB-37 direktno povezani sa postojećom implementacijom evidencije zaduživanja. Ostavljenje ovih funkcionalnosti za Sprint 8 i Sprint 9 dovelo bi do dodatnog prilagođavanja postojećeg koda i složenije integracije.

**Razmatrane opcije:**
- Ostaviti PB-35 i PB-36 u Sprintu 9 te PB-37 u Sprintu 8
- Premjestiti sve povezane backlog iteme u Sprint 7  

**Odabrana opcija:**
Premještanje PB-35, PB-36 i PB-37 u Sprint 7.  

**Razlog izbora:**
Na ovaj način omogućena je implementacija svih funkcionalnosti vezanih za zaduživanje unutar iste razvojne cjeline, čime je smanjena kompleksnost razvoja i olakšano testiranje sistema.  

**Posljedice odluke:**
- Povećan obim posla u Sprintu 7  
- Jednostavnija integracija funkcionalnosti zaduživanja
- Lakše testiranje i održavanje modula
- Smanjena potreba za kasnijim izmjenama baze i logike sistema 

**Status:**
Aktivna  

**Povezani PB:**
PB-35, PB-36, PB-37