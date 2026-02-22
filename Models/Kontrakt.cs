using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformacniSystemKlub.Models
{
    public class Kontrakt
    {

        public int IdHrac { get; set; }

        /// <summary>
        /// Datum začátku platnosti kontraktu
        /// </summary>
        public DateOnly DatumZacatku { get; set; }

        /// <summary>
        /// Datum ukončení platnosti kontraktu
        /// </summary>
        public DateOnly DatumKonce { get; set; }

        public int Plat { get; set; }

        public string TelCisloNaAgenta { get; set; }

        public int VystupniKlauzule { get; set; }

        /// <summary>
        /// Hráč, ke kterému kontrakt patří
        /// </summary>
        public Hrac KontraktHrace { get; set; }

        public Kontrakt() { }

    }
}
