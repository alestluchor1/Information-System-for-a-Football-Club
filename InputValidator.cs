using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Controls;


namespace InformacniSystemKlub
{
    public static class InputValidator
    {
        // Povolená písmena 
        private static readonly Regex RegexPismena = new Regex(@"^[A-Za-zÁÉĚÍÓÚŮÝŽŠČŘĎŤŇáéěíóúůýžščřďťň]+$", RegexOptions.Compiled);

        private static readonly Regex RegexCisla = new Regex(@"^\d+$", RegexOptions.Compiled);

        private static readonly Regex RegexPozice = new Regex(
            @"^[A-Za-zÁÉĚÍÓÚŮÝŽŠČŘĎŤŇáéěíóúůýžščřďťň\s\-]+$"
        );

        private static readonly Regex RegexNazevTymu = new Regex(
            @"^[A-Za-zÁÉĚÍÓÚŮÝŽŠČŘĎŤŇáéěíóúůýžščřďťň0-9\s\.\-]+$",
        RegexOptions.Compiled
        );

        private static readonly Regex RegexVysledekZapasu = new Regex(
            @"^\d{1,2}:\d{1,2}$",
            RegexOptions.Compiled
        );

        public static void ValidujJmeno(string jmeno)
        {
            if (string.IsNullOrWhiteSpace(jmeno))
            {
                throw new Exception("Jméno nesmí být prázdné!");
            }

            if (!RegexPismena.IsMatch(jmeno))
            {
                throw new Exception("Jméno může obsahovat pouze písmena!");
            }
        }

        public static void ValidujPrijmeni(string prijmeni)
        {
            if (string.IsNullOrWhiteSpace(prijmeni))
            {
                throw new Exception("Příjmení nesmí být prázdné!");
            }

            if (!RegexPismena.IsMatch(prijmeni))
            {
                throw new Exception("Příjmení může obsahovat pouze písmena!");
            }
        }

        public static void ValidujUnikatniRodneCislo(string rodneCislo)
        {
            if (string.IsNullOrWhiteSpace(rodneCislo))
            {
                throw new Exception("Rodné číslo nesmí být prázdné!");
            }

            string rc = rodneCislo.Trim();

            foreach (Hrac hrac in HraciOkno.HraciData)
            {
                if (hrac != null && hrac.RodneCislo != null && hrac.RodneCislo.Trim() == rc)
                {
                    throw new Exception("Hráč nebo trenér s tímto rodným číslem už v systému existuje!");
                }
            }

            foreach (Trener trener in TreneriOkno.TreneriData)
            {
                if (trener != null && trener.RodneCislo != null && trener.RodneCislo.Trim() == rc)
                {
                    throw new Exception("Hráč nebo trenér s tímto rodným číslem už v systému existuje!");
                }
            }
        }

        public static void ValidujRodneCislo(string rodneCislo)
        {
            if (string.IsNullOrWhiteSpace(rodneCislo))
            {
                throw new Exception("Rodné číslo nesmí být prázdné!");
            }

            if (rodneCislo.Length != 10)
            {
                throw new Exception("Rodné číslo musí mít přesně 10 číslic!");
            }

            if (!RegexCisla.IsMatch(rodneCislo))
            {
                throw new Exception("Rodné číslo musí obsahovat pouze číslice!");
            }
               
        }

        public static void ValidujTelefonniCislo(string telefonniCislo)
        {
            if (string.IsNullOrWhiteSpace(telefonniCislo))
            {
                throw new Exception("Telefonní číslo nesmí být prázdné!");
            }

            if (!telefonniCislo.All(char.IsDigit))
            {
                throw new Exception("Telefonní číslo smí obsahovat pouze číslice!");
            }

            if (telefonniCislo.Length != 12)
            {
                throw new Exception("Telefonní číslo musí být 12 místné číslo!");
            }
        }

        public static void ValidujPozice(string pozice)
        {
            if (string.IsNullOrWhiteSpace(pozice))
            {
                throw new Exception("Pozice nesmí být prázdná!");
            }

            pozice = pozice.Trim();

            if (pozice.Length > 50)
            {
                throw new Exception("Pozice může mít maximálně 50 znaků!");
            }

            if (!RegexPozice.IsMatch(pozice))
            {
                throw new Exception("Pozice může obsahovat pouze písmena, mezery a pomlčku!");
            }
        }

        public static char ValidujZdravotniStav(object selectedItem)
        {
            ComboBoxItem item = selectedItem as ComboBoxItem;
            if (item == null)
            {
                throw new Exception("Vyber zdravotní stav (A/N)!");
            }

            if (item.Content == null)
            {
                throw new Exception("Zdravotní stav musí být A nebo N!");
            }

            string s = item.Content.ToString();
            if (s == null)
            {
                throw new Exception("Zdravotní stav musí být A nebo N!");
            }

            s = s.Trim();

            if (s.Length == 0 || (s != "A" && s != "N"))
            {
                throw new Exception("Zdravotní stav musí být A nebo N!");
            }

            return s[0];
        }


        public static int ValidujHraciSpinnerGolyKarty(string hodnotaText, string nazevPole = "Počet gólů")
        {
            const int min = 0;
            const int max = 999;

            if (string.IsNullOrWhiteSpace(hodnotaText))
            {
                throw new Exception($"{nazevPole} nesmí být prázdné");
            }

            if (!int.TryParse(hodnotaText.Trim(), out int hodnota))
            {
                throw new Exception($"{nazevPole} musí být celé číslo");
            }

            if (hodnota < min)
            {
                throw new Exception($"{nazevPole} nesmí být menší než {min}");
            }

            if (hodnota > max)
            {
                throw new Exception($"{nazevPole} nesmí být větší než {max}");
            }

            return hodnota; 
        }

        public static int ValidujId(string idText)
        {
            const int minimalniHodnota = 0;

            if (string.IsNullOrWhiteSpace(idText))
            {
                throw new Exception("ID nesmí být prázdné!");
            }

            if (!idText.All(char.IsDigit))
            {
                throw new Exception("ID smí obsahovat pouze číslice!");
            }

            if (!int.TryParse(idText, out int id))
            {
                throw new Exception("ID je mimo rozsah typu int!");
            }

            if (id <= minimalniHodnota)
            {
                throw new Exception("ID musí být větší než 0!");
            }

            return id;
        }

        public static void ValidujTrenerskaLicence(string trenerskaLicence)
        {
            if (string.IsNullOrWhiteSpace(trenerskaLicence))
            {
                throw new Exception("Trenérská licence nesmí být prázdná!");
            }

            trenerskaLicence = trenerskaLicence.Trim();

            if (trenerskaLicence.Length > 30)
            {
                throw new Exception("Trenérská licence může mít maximálně 30 znaků!");
            }

        }

        public static void ValidujSpecializace(string specializace)
        {
            if (string.IsNullOrWhiteSpace(specializace))
            {
                throw new Exception("Specializace nesmí být prázdná!");
            }

            specializace = specializace.Trim();

            if (specializace.Length > 40)
            {
                throw new Exception("Specializace může mít maximálně 40 znaků!");
            }

        }

        public static int ValidujTreneriPocetLetPraxe(string hodnotaText, string nazevPole = "Počet let")
        {
            const int min = 0;
            const int max = 100;

            if (string.IsNullOrWhiteSpace(hodnotaText))
            {
                throw new Exception($"{nazevPole} nesmí být prázdné");
            }

            if (!int.TryParse(hodnotaText.Trim(), out int hodnota))
            {
                throw new Exception($"{nazevPole} musí být celé číslo");
            }

            if (hodnota < min)
            {
                throw new Exception($"{nazevPole} nesmí být menší než {min}");
            }

            if (hodnota > max)
            {
                throw new Exception($"{nazevPole} nesmí být větší než {max}");
            }

            return hodnota;
        }

        public static void ValidujVyberTrenera(object selectedItem)
        {
            if (selectedItem == null)
            {
                throw new Exception("Vyber trenéra!");
            }
        }

        public static DateTime ValidujDatumTreninku(DateTime? datum, string nazevPole = "Datum")
        {
            if (datum == null)
            {
                throw new Exception($"{nazevPole} nesmí být prázdné!");
            }
            
            // Datum se nesmí zadat v minulosti
            if (datum.Value.Date < DateTime.Today)
            {
                throw new Exception($"{nazevPole} nesmí být v minulosti!");
            }

            return datum.Value.Date;
        }

        public static string ValidujMistoTreninku(string misto, string nazevPole = "Místo")
        {
            if (string.IsNullOrWhiteSpace(misto))
            {
                throw new Exception($"{nazevPole} nesmí být prázdné!");
            }

            misto = misto.Trim();

            if (misto.Length > 50)
            {
                throw new Exception($"{nazevPole} může mít maximálně 50 znaků!");
            }

            return misto;
        }

        public static string? ValidujPopisTreninku(string popis, string nazevPole = "Popis")
        {
            if (string.IsNullOrWhiteSpace(popis))
            {
                return null; // dovolíme NULL do DB
            }

            popis = popis.Trim();

            if (popis.Length > 50)
            {
                throw new Exception($"{nazevPole} může mít maximálně 50 znaků!");
            }

            return popis;
        }

        public static int ValidujVyberHrace(object selectedItem)
        {
            if (selectedItem == null)
            {
                throw new Exception("Vyber hráče!");
            }

            Hrac hrac = selectedItem as Hrac;
            if (hrac == null)
            {
                throw new Exception("Neplatný výběr hráče!");
            }

            if (hrac.Id <= 0)
            {
                throw new Exception("Neplatné ID hráče!");
            }

            return hrac.Id;
        }

        public static void ValidujRozsahDatumuKontraktu(DateTime? datumOd, DateTime? datumDo)
        {
            if (datumOd == null)
            {
                throw new Exception("Datum od nesmí být prázdné!");
            }

            if (datumDo == null)
            {
                throw new Exception("Datum do nesmí být prázdné!");
            }

            DateTime od = datumOd.Value.Date;
            DateTime doo = datumDo.Value.Date;

            if (doo < od)
            {
                throw new Exception("Datum do nesmí být menší než datum od!");
            }
        }

        public static long ValidujPlatKontraktuLong(string platText, string nazevPole = "Plat")
        {
            const long minimalniHodnota = 1;

            if (string.IsNullOrWhiteSpace(platText))
            {
                throw new Exception($"{nazevPole} nesmí být prázdný!");
            }

            platText = platText.Trim();

            if (!RegexCisla.IsMatch(platText))
            {
                throw new Exception($"{nazevPole} smí obsahovat pouze číslice!");
            }

            if (!long.TryParse(platText, out long plat))
            {
                throw new Exception($"{nazevPole} je mimo rozsah typu long!");
            }

            if (plat < minimalniHodnota)
            {
                throw new Exception($"{nazevPole} musí být alespoň {minimalniHodnota}!");
            }

            return plat;
        }

        public static long ValidujVystupniKlauzuliLong(string klauzuleText, string nazevPole = "Výstupní klauzule")
        {
            if (string.IsNullOrWhiteSpace(klauzuleText))
            {
                return 0;
            }

            klauzuleText = klauzuleText.Trim();

            if (!RegexCisla.IsMatch(klauzuleText))
            {
                throw new Exception($"{nazevPole} smí obsahovat pouze číslice!");
            }

            if (!long.TryParse(klauzuleText, out long klauzule))
            {
                throw new Exception($"{nazevPole} je mimo rozsah typu long!");
            }

            if (klauzule < 0)
            {
                throw new Exception($"{nazevPole} nesmí být záporná!");
            }

            return klauzule;
        }

        public static DateTime ValidujDatumUkonceniOpatreni(DateTime? datum)
        {
            if (datum == null)
            {
                throw new Exception("Datum ukončení nesmí být prázdné!");
            }

            return datum.Value.Date;
        }

        public static string ValidujDuvodOpatreni(string duvod)
        {
            if (string.IsNullOrWhiteSpace(duvod))
            {
                throw new Exception("Důvod nesmí být prázdný!");
            }

            duvod = duvod.Trim();

            if (duvod.Length > 50)
            {
                throw new Exception("Může být zadáno maximálně 50 znaků!");
            }

            return duvod;
        }

        // Zápasy

        public static DateTime ValidujDatumZapasu(DateTime? datum, string nazevPole = "Datum zápasu")
        {
            if (datum == null)
            {
                throw new Exception($"{nazevPole} nesmí být prázdné!");
            }

            return datum.Value.Date;
        }

        public static string ValidujNazevTymu(string tym, string nazevPole = "Název týmu")
        {
            if (string.IsNullOrWhiteSpace(tym))
            {
                throw new Exception($"{nazevPole} nesmí být prázdný!");
            }

            tym = tym.Trim();

            if (tym.Length > 30)
            {
                throw new Exception($"{nazevPole} může mít maximálně 30 znaků!");
            }

            if (!RegexNazevTymu.IsMatch(tym))
            {
                throw new Exception($"{nazevPole} může obsahovat pouze písmena, číslice, mezery, tečku a pomlčku!");
            }

            return tym;
        }

        public static void ValidujRozdilneTymy(string domaciTym, string hosteTym)
        {
            if (string.IsNullOrWhiteSpace(domaciTym) || string.IsNullOrWhiteSpace(hosteTym))
            {
                throw new Exception("Domácí tým i hosté musí být vyplněni!");
            }

            if (domaciTym.Trim().Equals(hosteTym.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Domácí tým a hosté nemohou být stejní!");
            }
        }

        public static string ValidujTypSouteze(object selectedItem)
        {
            ComboBoxItem item = selectedItem as ComboBoxItem;
            if (item == null || item.Content == null)
            {
                throw new Exception("Vyber typ soutěže!");
            }

            string typSouteze = item.Content.ToString().Trim();

            if (typSouteze != "LIGA" &&
                typSouteze != "POHÁR" &&
                typSouteze != "DIVIZE" &&
                typSouteze != "KRAJ" &&
                typSouteze != "OKRES")
            {
                throw new Exception("Neplatný typ soutěže!");
            }

            return typSouteze;
        }

        public static string ValidujStavZapasu(object selectedItem)
        {
            ComboBoxItem item = selectedItem as ComboBoxItem;
            if (item == null || item.Content == null)
            {
                throw new Exception("Vyber stav zápasu!");
            }

            string stavZapasu = item.Content.ToString().Trim();

            if (stavZapasu != "ODEHRÁNO" && stavZapasu != "BUDE SE HRÁT")
            {
                throw new Exception("Neplatný stav zápasu!");
            }

            return stavZapasu;
        }

        public static string? ValidujVysledekZapasu(string vysledek)
        {
  
            if (string.IsNullOrWhiteSpace(vysledek))
            {
                throw new Exception("Výsledek nesmí být prázdný, pokud je zápas odehrán!");
            }

            vysledek = vysledek.Trim();

            if (vysledek.Length > 5)
            {
                throw new Exception("Výsledek může mít maximálně 5 znaků!");
            }

            if (!RegexVysledekZapasu.IsMatch(vysledek))
            {
                throw new Exception("Výsledek musí být ve formátu např. 2:1 !!");
            }

            return vysledek;
        }

        public static int? ValidujPocetKaretZapasu(string hodnotaText, string stavZapasu, string nazevPole)
        {
            if (stavZapasu == "BUDE SE HRÁT")
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(hodnotaText))
            {
                throw new Exception($"{nazevPole} nesmí být prázdný!");
            }

            if (!int.TryParse(hodnotaText.Trim(), out int hodnota))
            {
                throw new Exception($"{nazevPole} musí být celé číslo!");
            }

            if (hodnota < 0)
            {
                throw new Exception($"{nazevPole} nesmí být záporný!");
            }

            return hodnota;
        }

        // Sportovní vybavení
        public static string ValidujNazevSportovnihoVybaveni(string nazev, string nazevPole = "Název")
        {
            if (string.IsNullOrWhiteSpace(nazev))
            {
                throw new Exception($"{nazevPole} nesmí být prázdný!");
            }

            nazev = nazev.Trim();

            if (nazev.Length > 50)
            {
                throw new Exception($"{nazevPole} může mít maximálně 50 znaků!");
            }

            return nazev;
        }

        public static string ValidujVelikostSportovnihoVybaveni(string velikost, string nazevPole = "Velikost")
        {
            if (string.IsNullOrWhiteSpace(velikost))
            {
                throw new Exception($"{nazevPole} nesmí být prázdná!");
            }

            velikost = velikost.Trim();

            if (velikost.Length > 3)
            {
                throw new Exception($"{nazevPole} může mít maximálně 3 znaky!");
            }

            return velikost;
        }

        public static DateTime ValidujDatumNakupuSportovnihoVybaveni(DateTime? datum, string nazevPole = "Datum nákupu")
        {
            if (datum == null)
            {
                throw new Exception($"{nazevPole} nesmí být prázdné!");
            }

            if (datum.Value.Date > DateTime.Today)
            {
                throw new Exception($"{nazevPole} nesmí být v budoucnosti!");
            }

            return datum.Value.Date;
        }

        // Registrace

        public static string ValidujUzivatelskeJmeno(string uzivJmeno)
        {
            if (string.IsNullOrWhiteSpace(uzivJmeno))
            {
                throw new Exception("Uživatelské jméno nesmí být prázdné!");
            }

            uzivJmeno = uzivJmeno.Trim();

            if ((uzivJmeno.Length < 3) || (uzivJmeno.Length > 30))
            {
                throw new Exception("Uživatelské jméno musí mít 3 až 30 znaků!");
            }

            // povolíme písmena/čísla/._-
            if (!Regex.IsMatch(uzivJmeno, @"^[A-Za-z0-9._-]+$"))
            {
                throw new Exception("Uživatelské jméno smí obsahovat jen písmena, číslice a znaky . _ -");
            }

            return uzivJmeno;
        }

        public static string ValidujEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("E-mail nesmí být prázdný!");
            }

            email = email.Trim();

            if (email.Length > 100)
            {
                throw new Exception("E-mail je příliš dlouhý (max 100 znaků)!");
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new Exception("E-mail není ve správném formátu!");
            }

            return email;
        }

        public static string ValidujHeslo(string heslo)
        {
            if (string.IsNullOrWhiteSpace(heslo))
            {
                throw new Exception("Heslo nesmí být prázdné!");
            }

            heslo = heslo.Trim();

            if ((heslo.Length < 4) || (heslo.Length > 20))
            {
                throw new Exception("Heslo musí mít 4 až 20 znaků!");
            }

            return heslo;
        }

        public static void ValidujShoduHesel(string heslo, string heslo2)
        {
            if (heslo != heslo2)
            {
                throw new Exception("Hesla se neshodují!");
            }
        }

        // Nominace

        public static int ValidujNominaciMinuty(string hodnotaText, string nazevPole = "Počet odehraných minut")
        {
            const int min = 0;
            const int max = 300;

            if (string.IsNullOrWhiteSpace(hodnotaText))
            {
                throw new Exception($"{nazevPole} nesmí být prázdné");
            }

            if (!int.TryParse(hodnotaText.Trim(), out int hodnota))
            {
                throw new Exception($"{nazevPole} musí být celé číslo");
            }

            if (hodnota < min)
            {
                throw new Exception($"{nazevPole} nesmí být menší než {min}");
            }

            if (hodnota > max)
            {
                throw new Exception($"{nazevPole} nesmí být větší než {max}");
            }

            return hodnota;
        }

        // Nastavení - uživatelské účty
        public static void ValidujVyberRole(object selectedItem)
        {
            ComboBoxItem item = selectedItem as ComboBoxItem;

            if (item == null)
            {
                throw new Exception("Vyber roli!");
            }

            if (item.Tag == null)
            {
                throw new Exception("Neplatná role!");
            }
        }

        public static int ValidujIdClenaKlubuProRoli(string idText, string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new Exception("Role nesmí být prázdná!");
            }

            if (role != "Hrac" && role != "Trener")
            {
                throw new Exception("ID člena klubu se zadává pouze pro roli Hrac nebo Trener!");
            }

            int id = ValidujId(idText);
            return id;
        }
    }
}
