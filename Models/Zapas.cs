using System;

namespace InformacniSystemKlub.Models
{
    public class Zapas
    {
        public int IdZapas { get; set; }

        public DateTime Datum { get; set; }

        /// <summary>
        /// ID typu soutěže z číselníku TYP_SOUTEZE
        /// </summary>
        public TypSoutezeEnum IdTypSouteze { get; set; }

        /// <summary>
        /// ID stavu zápasu z číselníku STAV_ZAPASU
        /// </summary>
        public StavZapasuEnum IdStav { get; set; }

        public string DomaciTym { get; set; }

        public string HosteTym { get; set; }

        public string Vysledek { get; set; }

        public int? PocetZlutychKaret { get; set; }

        public int? PocetCervenychKaret { get; set; }

        // Pro zobrazení v gridu
        public string NazevSouteze
        {
            get
            {
                switch (IdTypSouteze)
                {
                    case TypSoutezeEnum.Liga: return "LIGA";
                    case TypSoutezeEnum.Pohar: return "POHÁR";
                    case TypSoutezeEnum.Divize: return "DIVIZE";
                    case TypSoutezeEnum.Kraj: return "KRAJ";
                    case TypSoutezeEnum.Okres: return "OKRES";
                    default: return "-";
                }
            }
        }

        // Pro zobrazení v gridu
        public string StavZapasu
        {
            get
            {
                switch (IdStav)
                {
                    case StavZapasuEnum.Odehrano: return "ODEHRÁNO";
                    case StavZapasuEnum.BudeSeHrat: return "BUDE SE HRÁT";
                    default: return "-";
                }
            }
        }

        public Zapas() { }
    }
}