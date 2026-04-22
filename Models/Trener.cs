using System;

namespace InformacniSystemKlub.Models
{
    public class Trener
    {
        public int Id { get; set; }
        public string RodneCislo { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string TypClena { get; } = "Trener";
        public string TelefonniCislo { get; set; }
        public string Specializace { get; set; }

        public int IdTreninkovaSkupina { get; set; }
        public string NazevSkupiny { get; set; }

        public Trener() { }

        public Trener(
            int id,
            string jmeno,
            string prijmeni,
            string rodneCislo,
            string telefonniCislo,
            string specializace,
            int idTreninkovaSkupina,
            string nazevSkupiny)
        {
            this.Id = id;
            this.Jmeno = jmeno;
            this.Prijmeni = prijmeni;
            this.RodneCislo = rodneCislo;
            this.TelefonniCislo = telefonniCislo;
            this.Specializace = specializace;
            this.IdTreninkovaSkupina = idTreninkovaSkupina;
            this.NazevSkupiny = nazevSkupiny;
        }
    }
}