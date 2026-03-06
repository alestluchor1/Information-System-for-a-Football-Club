using System;

namespace InformacniSystemKlub.Models
{
    public class NominaceNaZapas
    {
        public int IdNominace { get; set; }

        public int IdHrace { get; set; }
        public string RodneCislo { get; set; } = "";
        public string Prijmeni { get; set; } = "";

        public int IdTreninkovaSkupina { get; set; }

        public int IdZapas { get; set; }
        public string DomaciTym { get; set; } = "";
        public string HosteTym { get; set; } = "";
        public string StavZapasu { get; set; } = "";
        public int PocetOdehranychMinut { get; set; }
        public int PocetGolu { get; set; }
        public int ZluteKarty { get; set; }
        public int CerveneKarty { get; set; }

        public NominaceNaZapas()
        {
           
        }
    }
}