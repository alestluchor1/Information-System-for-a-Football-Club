using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformacniSystemKlub.Models
{
    public class Hrac
    {
        public int Id { get; set; }
        public int IdTreninkovaSkupina { get; set; }
        public String RodneCislo { get; set; }
        public String Jmeno { get; set; }
        public String Prijmeni { get; set; }
        public String TypClena { get; set; }
        public String TelefonniCislo { get; set; }
        public int PocetVstrelenychGolu { get; set; }
        public int PocetZlutychKaret { get; set; }
        public int PocetCervenychKaret { get; set; }
        public String Pozice { get; set; } = "";
        public char ZdravotniStav { get; set; }
        public string TreninkovaSkupinaText { get; set; } = "";

        public Hrac() { }
    }
}
