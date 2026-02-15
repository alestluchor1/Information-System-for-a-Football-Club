using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformacniSystemKlub.Models
{
    public class Trenink
    {
        public int Id { get; set; }

        /// <summary>
        /// Rodné číslo trenéra
        /// </summary>
        public string RodneCislo { get; set; }

        /// <summary>
        /// Příjmení trenéra
        /// </summary>
        public string Prijmeni { get; set; }

        public DateTime Datum { get; set; }

        public string Misto { get; set; }

        public string? Popis { get; set; }

        public Trenink() { }
    }

}
