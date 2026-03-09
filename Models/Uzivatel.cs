using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformacniSystemKlub.Models
{
    public class Uzivatel
    {
        /// <summary>
        /// Unikátní uživatelské jméno, které slouží k přihlášení do systému
        /// </summary>
        public string UzivatelskeJmeno { get; set; }

        /// <summary>
        /// Zahashovaná podoba hesla uživatele
        /// Heslo se nikdy neukládá v čitelné (plain text) podobě
        /// </summary>
        public string Heslo { get; set; }

        /// <summary>
        /// Náhodně generovaný řetězec (salt) používaný při hashování hesla
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Role uživatele v systému
        /// Určuje úroveň oprávnění v aplikaci
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Unikátní E-mailová adresa uživatele
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Datum a čas posledního přihlášení uživatele do systému
        /// </summary>
        public DateTime PosledniPrihlaseni { get; set; }

        /// <summary>
        /// Rodné číslo uživatele
        /// Vyplňuje se pouze u rolí "hráč" a "trenér"
        /// </summary>
        public string RodneCislo { get; set; }

        /// <summary>
        /// Pro Notifikace/zprávy potřebné, jestli je vybrán uživatel v checkboxu nebo ne
        /// </summary>
        public bool JeVybran { get; set; }
        public int IdUzivatelskyUcet { get; set; }
        public int IdClenKlubu { get; set; }
        public int IdRole { get; set; }
    }
}
