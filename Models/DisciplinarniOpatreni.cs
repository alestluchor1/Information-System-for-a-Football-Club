using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformacniSystemKlub.Models
{

    public class DisciplinarniOpatreni
    {
        public int IdOpatreni { get; set; }

        public DateTime DatumUkonceniOpatreni { get; set; }

        public string Duvod { get; set; }

        /// <summary>
        /// ID hráče
        /// </summary>
        public int IdHrac { get; set; }

        /// <summary>
        /// Jméno hráče s opatřením
        /// </summary>
        public string Jmeno { get; set; }

        /// <summary>
        /// Příjmení hráče s opatřením
        /// </summary>
        public string Prijmeni { get; set; }

        /// <summary>
        /// ID zápasu, ke kterému se opatření vztahuje, zápas může, ale nemusí mít opatření
        /// </summary>
        public int? IdZapas { get; set; }

        public DisciplinarniOpatreni() { }
    }

}
