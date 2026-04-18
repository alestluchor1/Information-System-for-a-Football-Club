using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.NajdiWindows
{
    /// <summary>
    /// Okno pro filtrování zápasů podle zadaných kritérií
    /// </summary>
    public partial class NajdiZapas : Window
    {
        /// <summary>
        /// Výsledná kolekce zápasů po filtrování
        /// Tuto kolekci následně čte okno, které filtr vyvolalo
        /// </summary>
        public ObservableCollection<Zapas> FiltrovaniZapasy { get; private set; } = new ObservableCollection<Zapas>();

        /// <summary>
        /// Vstupní kolekce zápasů, ze které se filtruje
        /// </summary>
        private readonly ObservableCollection<Zapas> VstupniZapasy;

        public NajdiZapas(ObservableCollection<Zapas> zapasy)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            VstupniZapasy = zapasy;
        }

        /// <summary>
        /// Provede filtrování zápasů na základě vyplněných hodnot ve formuláři
        /// </summary>
        private IEnumerable<Zapas> FiltrujZapasy()
        {
            IEnumerable<Zapas> vysledek = VstupniZapasy;

            // Datum zápasu
            if (DpDatum.SelectedDate != null)
            {
                DateTime datum = InputValidator.ValidujDatumZapasu(DpDatum.SelectedDate);
                vysledek = vysledek.Where(z => z.Datum.Date == datum.Date);
            }

            // Typ soutěže
            if (CbTypSouteze.SelectedItem is ComboBoxItem polozkaTyp)
            {
                string typText = polozkaTyp.Content.ToString();

                if (typText != "—")
                {
                    TypSoutezeEnum typSouteze = ZiskejTypSoutezeEnum(typText);
                    vysledek = vysledek.Where(z => z.IdTypSouteze == typSouteze);
                }
            }

            // Tréninková skupina
            if (CbTreninkovaSkupina.SelectedItem is ComboBoxItem polozkaSkupina)
            {
                if (polozkaSkupina.Tag != null)
                {
                    int idTreninkoveSkupiny = Convert.ToInt32(polozkaSkupina.Tag);

                    if (idTreninkoveSkupiny != 0)
                    {
                        vysledek = vysledek.Where(z => z.IdTreninkovaSkupina == idTreninkoveSkupiny);
                    }
                }
            }

            // Stav zápasu
            if (CbStavZapasu.SelectedItem is ComboBoxItem polozkaStav)
            {
                string stavText = polozkaStav.Content.ToString();

                if (stavText != "—")
                {
                    StavZapasuEnum stavZapasu = ZiskejStavZapasuEnum(stavText);
                    vysledek = vysledek.Where(z => z.IdStav == stavZapasu);
                }
            }

            // Domácí tým
            if (!string.IsNullOrWhiteSpace(TxtDomaciTym.Text))
            {
                string domaciTym = TxtDomaciTym.Text.Trim();
                InputValidator.ValidujNazevTymu(domaciTym, "Domácí tým");

                vysledek = vysledek.Where(z =>
                    !string.IsNullOrWhiteSpace(z.DomaciTym) &&
                    z.DomaciTym.Contains(domaciTym, StringComparison.OrdinalIgnoreCase));
            }

            // Hosté
            if (!string.IsNullOrWhiteSpace(TxtHosteTym.Text))
            {
                string hosteTym = TxtHosteTym.Text.Trim();
                InputValidator.ValidujNazevTymu(hosteTym, "Hosté");

                vysledek = vysledek.Where(z =>
                    !string.IsNullOrWhiteSpace(z.HosteTym) &&
                    z.HosteTym.Contains(hosteTym, StringComparison.OrdinalIgnoreCase));
            }

            // Výsledek
            if (!string.IsNullOrWhiteSpace(TxtVysledek.Text))
            {
                string vysledekZapasu = InputValidator.ValidujVysledekZapasu(TxtVysledek.Text.Trim());
                vysledek = vysledek.Where(z => z.Vysledek == vysledekZapasu);
            }

            // Žluté karty
            if (!string.IsNullOrWhiteSpace(iudPocetZlutychKaret.Text))
            {
                int pocetZlutychKaret = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetZlutychKaret.Text, "Počet žlutých karet");
                vysledek = vysledek.Where(z => z.PocetZlutychKaret == pocetZlutychKaret);
            }

            // Červené karty
            if (!string.IsNullOrWhiteSpace(iudPocetCervenychKaret.Text))
            {
                int pocetCervenychKaret = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetCervenychKaret.Text, "Počet červených karet");
                vysledek = vysledek.Where(z => z.PocetCervenychKaret == pocetCervenychKaret);
            }

            return vysledek;
        }

        /// <summary>
        /// Obsluha tlačítka "Filtrovat"
        /// Pokud dojde k chybě ve vstupu (validace), zobrazí se chybové hlášení a okno zůstane otevřené
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniZapasy = new ObservableCollection<Zapas>(FiltrujZapasy());
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Převede vybraný text typu soutěže na enum
        /// </summary>
        private TypSoutezeEnum ZiskejTypSoutezeEnum(string typSouteze)
        {
            if (typSouteze == "LIGA")
            {
                return TypSoutezeEnum.Liga;
            }

            if (typSouteze == "POHÁR")
            {
                return TypSoutezeEnum.Pohar;
            }

            if (typSouteze == "DIVIZE")
            {
                return TypSoutezeEnum.Divize;
            }

            if (typSouteze == "KRAJ")
            {
                return TypSoutezeEnum.Kraj;
            }

            if (typSouteze == "OKRES")
            {
                return TypSoutezeEnum.Okres;
            }

            throw new Exception("Neplatný typ soutěže!");
        }

        /// <summary>
        /// Převede vybraný text stavu zápasu na enum
        /// </summary>
        private StavZapasuEnum ZiskejStavZapasuEnum(string stavZapasu)
        {
            if (stavZapasu == "ODEHRÁNO")
            {
                return StavZapasuEnum.Odehrano;
            }

            if (stavZapasu == "BUDE SE HRÁT")
            {
                return StavZapasuEnum.BudeSeHrat;
            }

            throw new Exception("Neplatný stav zápasu!");
        }
    }
}