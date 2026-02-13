using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace InformacniSystemKlub
{
    public static class InputValidator
    {
        // Povolená písmena (včetně české diakritiky), bez mezer a bez pomlček
        private static readonly Regex RegexPismena = new Regex(
            @"^[A-Za-zÁÉĚÍÓÚŮÝŽŠČŘĎŤŇáéěíóúůýžščřďťň]+$"
        );

        private static readonly Regex RegexPozice = new Regex(
            @"^[A-Za-zÁÉĚÍÓÚŮÝŽŠČŘĎŤŇáéěíóúůýžščřďťň\s\-]+$"
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

            if (!rodneCislo.All(char.IsDigit))
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

        public static int ValidujHraciVek(string hodnotaText, string nazevPole = "Počet let")
        {
            const int min = 3;
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
    }
}
