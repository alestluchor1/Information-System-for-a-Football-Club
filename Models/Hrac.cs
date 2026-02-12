using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformacniSystemKlub.Models
{
    public class Hrac
    {
        public int id { get; set; }
        public String rodneCislo { get; set; }
        public String jmeno { get; set; }
        public String prijmeni { get; set; }
        public String typClena { get; set; }
        public String telefonniCislo { get; set; }
        public int pocetVstrelenychGolu { get; set; }
        public int pocetZlutychKaret { get; set; }
        public int pocetCervenychKaret { get; set; }
        public String pozice { get; set; } = "";
        public int vek { get; set; }
        public char zdravotniStav { get; set; }

        public Hrac(int id, int pocetVstrelenychGolu, int pocetZlutychKaret, int pocetCervenychKaret, String pozice, int vek, char zdravotniStav)
        { 
            this.id = id;
            this.pocetVstrelenychGolu = pocetVstrelenychGolu;
            this.pocetZlutychKaret = pocetZlutychKaret;
            this.pocetCervenychKaret = pocetCervenychKaret;
            this.pozice = pozice;
            this.vek = vek;
            this.zdravotniStav = zdravotniStav;
        }

        public Hrac() { }
    }
}
