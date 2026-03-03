using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformacniSystemKlub.Models
{
    public class SportovniVybaveni
    {
        public int IdSportovniVybaveni {get; set;}

        public String Nazev { get; set; }

        public String Velikost { get; set; }

        public DateTime DatumNakupu { get; set; }

        public SportovniVybaveni() { }
    }
}
