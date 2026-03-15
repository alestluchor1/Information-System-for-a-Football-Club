# Informační systém fotbalového klubu

Bakalářská práce zaměřená na návrh a implementaci moderního informačního systému pro správu sportovního fotbalového klubu.

## Autor

Student Univerzity Pardubice  
Obor: **Informační technologie**

## Téma práce

**Systém pro správu sportovního fotbalového klubu**

Cílem práce je navrhnout a implementovat moderní informační systém pro správu sportovního fotbalového klubu. Systém umožní efektivní správu tréninkových skupin, hráčů, trenérů, tréninků a zápasů. Systém dále umožní trenérům navrhovat sestavy týmu na zápas například podle individuálních výkonových statistiky hráče, zdravotního stavu apod. Systém bude umožňovat přístup k jednotlivým funkcím podle rolí. Součástí práce bude také analýza stávajících řešení, návrh architektury aplikace, výběr vhodných technologií a testování výsledné implementace.

## Thesis goal in English

The aim of the thesis is to design and implement a modern information system for the administration of a sports football club. The system will enable effective management of training groups, players, coaches, training sessions and matches. The system will also allow coaches to design team line-ups for a match, for example, according to individual player performance statistics, health status, etc. The system will allow access to individual functions by role. The work will also include analysis of existing solutions, design of the application architecture, selection of appropriate technologies and testing of the final implementation.

## Funkce systému

Systém obsahuje zejména tyto moduly:


Funkční požadavky
Navrhovaná aplikace musí umožňovat:
•	evidenci hráčů
•	evidenci trenérů
•	evidenci tréninků
•	evidenci disciplinárních opatření
•	evidenci hráčských kontraktů
•	evidenci sportovního vybavení
•	evidenci zápasů
•	přidávání, odebírání, editaci a filtrování evidovaných záznamů
•	správu uživatelských účtů pro role administrátor, hráč, trenér a host
•	přidávání, odebírání, editaci a filtrování uživatelských účtů administrátorem
•	zobrazení kalendáře s plánovanými tréninky
•	zápis statistik hráče v odehraném zápase
•	zasílání e-mailových oznámení uživatelům
•	přepínání mezi světlým a tmavým režimem
•	grafický editor rozmístění hráčů na hrací ploše s možností filtrování podle zvolených kritérií
•	uložení odehraných zápasů s výsledkem a plánovaných zápasů do souboru CSV
•	výběr zápasu a nominace hráčů pro zápas, který se teprve bude hrát
•	evidenci počtu odehraných minut, vstřelených gólů, žlutých a červených karet hráče v zápase
•	zařazování hráčů do tréninkových skupin A-tým, dorost nebo mladší žáci
•	registrace nového uživatele

Nefunkční požadavky
Navrhovaná aplikace musí splňovat:
•	přehledné a uživatelsky přívětivé rozhraní
•	ukládání dat do databáze Oracle
•	implementaci v programovacím jazyce C#
•	dostatečně rychlou odezvu při běžné práci s aplikací


## Uživatelské role

Aplikace podporuje více uživatelských rolí, například:

- administrátor
- trenér
- hráč
- host

Každá role má odlišná oprávnění a přístup pouze k vybraným částem systému.

## Testovací účty

Pro přihlášení do systému lze použít tyto testovací účty:

### Trenér
- Uživatelské jméno: `trener`
- Heslo: `pardubice`

### Hráč
- Uživatelské jméno: `hrac`
- Heslo: `pardubice`

## Použité technologie

Projekt využívá následující technologie:

- **C#**
- **WPF**
- **Oracle Database**

## Databáze

Aplikace je napojena na databázi Oracle Autonomous Database.  

## Poznámka

Projekt vzniká jako součást bakalářské práce na Univerzitě Pardubice.
