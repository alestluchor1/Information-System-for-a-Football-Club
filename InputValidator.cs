using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformacniSystemKlub
{
    public static class InputValidator
    {
        /// <summary>
        /// Validuje jméno
        /// </summary>
        public static void ValidujJmeno(string jmeno)
        {
            if (string.IsNullOrWhiteSpace(jmeno))
            {
                throw new Exception("Jméno nesmí být prázdné!");
            }
        }

        /// <summary>
        /// Validuje příjmení
        /// </summary>
        public static void ValidujPrijmeni(string prijmeni)
        {
            if (string.IsNullOrWhiteSpace(prijmeni))
            {
                throw new Exception("Příjmení nesmí být prázdné!");
            }

        }

        /// <summary>
        /// Validuje rodné číslo – musí mít 10 číslic
        /// </summary>
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

        }

    }
}
