using InformacniSystemKlub.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového zápasu
    /// </summary>
    public partial class PridejZapas : Window
    {
        private Zapas novyZapas;

        public PridejZapas(Zapas zapas)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            novyZapas = zapas;

            if (novyZapas == null)
            {
                novyZapas = new Zapas();
            }

            DpDatum.SelectedDate = DateTime.Today;
            NastavPoleProStavZapasu();
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="Zapas"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validace
                DateTime datum = InputValidator.ValidujDatumZapasu(DpDatum.SelectedDate);
                string typSoutezeText = InputValidator.ValidujTypSouteze(CbTypSouteze.SelectedItem);
                string stavZapasuText = InputValidator.ValidujStavZapasu(CbStavZapasu.SelectedItem);

                string domaciTym = InputValidator.ValidujNazevTymu(TxtDomaciTym.Text, "Domácí tým");
                string hosteTym = InputValidator.ValidujNazevTymu(TxtHosteTym.Text, "Hosté");

                InputValidator.ValidujRozdilneTymy(domaciTym, hosteTym);

                novyZapas.Datum = datum;
                novyZapas.DomaciTym = domaciTym;
                novyZapas.HosteTym = hosteTym;

                novyZapas.IdTypSouteze = ZiskejTypSoutezeEnum(typSoutezeText);
                novyZapas.IdStav = ZiskejStavZapasuEnum(stavZapasuText);
                novyZapas.IdTreninkovaSkupina = ZiskejIdTreninkoveSkupiny();

                if (novyZapas.IdStav == StavZapasuEnum.Odehrano)
                {
                    if (novyZapas.Datum > DateTime.Today)
                    {
                        MessageBox.Show(
                            "Nelze přidat odehraný zápas s budoucím datem!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Validace výsledku a počtu karet
                    string vysledek = InputValidator.ValidujVysledekZapasu(TxtVysledek.Text);

                    int? pocetZlutychKaret = InputValidator.ValidujPocetKaretZapasu(
                        iudPocetZlutychKaret.Text,
                        stavZapasuText,
                        "Počet žlutých karet");

                    int? pocetCervenychKaret = InputValidator.ValidujPocetKaretZapasu(
                        iudPocetCervenychKaret.Text,
                        stavZapasuText,
                        "Počet červených karet");

                    novyZapas.Vysledek = vysledek;
                    novyZapas.PocetZlutychKaret = pocetZlutychKaret;
                    novyZapas.PocetCervenychKaret = pocetCervenychKaret;
                }
                else if (novyZapas.IdStav == StavZapasuEnum.BudeSeHrat)
                {
                    if (novyZapas.Datum < DateTime.Today)
                    {
                        MessageBox.Show("Nelze přidat neodehraný zápas s minulým datem!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        novyZapas.Vysledek = "-";
                        novyZapas.PocetZlutychKaret = null;
                        novyZapas.PocetCervenychKaret = null;
                    }
                }

                MessageBox.Show("Zápas byl úspěšně přidán", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Po změně stavu zápasu upraví formulář
        /// </summary>
        private void CbStavZapasu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
            {
                return;
            }

            NastavPoleProStavZapasu();
        }

        /// <summary>
        /// Podle stavu zápasu povolí nebo zakáže pole pro výsledek a karty
        /// </summary>
        private void NastavPoleProStavZapasu()
        {
            ComboBoxItem item = CbStavZapasu.SelectedItem as ComboBoxItem;

            if (item == null || item.Content == null)
            {
                return;
            }

            string stavZapasu = item.Content.ToString();

            if (stavZapasu == "BUDE SE HRÁT")
            {
                TxtVysledek.Text = "-";
                TxtVysledek.IsEnabled = false;

                iudPocetZlutychKaret.Value = null;
                iudPocetZlutychKaret.IsEnabled = false;

                iudPocetCervenychKaret.Value = null;
                iudPocetCervenychKaret.IsEnabled = false;
            }
            else
            {
                TxtVysledek.Text = "";
                TxtVysledek.IsEnabled = true;

                iudPocetZlutychKaret.IsEnabled = true;
                iudPocetCervenychKaret.IsEnabled = true;
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

        /// <summary>
        /// Vrátí ID vybrané tréninkové skupiny z ComboBoxu
        /// </summary>
        private int ZiskejIdTreninkoveSkupiny()
        {
            ComboBoxItem vybranaPolozka = CbTreninkovaSkupina.SelectedItem as ComboBoxItem;

            if (vybranaPolozka == null)
            {
                throw new Exception("Musíte vybrat tréninkovou skupinu!");
            }

            if (vybranaPolozka.Tag == null)
            {
                throw new Exception("Vybraná tréninková skupina nemá platné ID!");
            }

            return Convert.ToInt32(vybranaPolozka.Tag);
        }
    }
}