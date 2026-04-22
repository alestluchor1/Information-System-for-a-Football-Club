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
        /// ID trenéra
        /// </summary>
        public int IdTrener { get; set; }

        /// <summary>
        /// Rodné číslo trenéra
        /// </summary>
        public string RodneCislo { get; set; }

        /// <summary>
        /// Příjmení trenéra
        /// </summary>
        public string Prijmeni { get; set; }

        /// <summary>
        /// ID tréninkové skupiny trenéra
        /// </summary>
        public int IdTreninkovaSkupina { get; set; }

        /// <summary>
        /// Název tréninkové skupiny trenéra
        /// </summary>
        public string NazevSkupiny { get; set; }

        public DateTime Datum { get; set; }

        public string Misto { get; set; }

        public string? Popis { get; set; }

        public Trenink() { }
    }
}