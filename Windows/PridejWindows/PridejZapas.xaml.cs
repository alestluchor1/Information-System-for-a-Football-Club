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
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

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

                if (novyZapas.IdStav == StavZapasuEnum.Odehrano)
                {
                    if (novyZapas.Datum > DateTime.Today)
                    {
                        MessageBox.Show(
                            "Nelze přidat odehraný zápas s budoucím datem!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

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
                        MessageBox.Show(
                            "Nelze přidat neodehraný zápas s minulým datem!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
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

                iudPocetZlutychKaret.Value = 0;
                iudPocetZlutychKaret.IsEnabled = false;

                iudPocetCervenychKaret.Value = 0;
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
        /// Nastaví (dark / light) mode
        /// </summary>
        /// <param name="darkMode">True = dark mode, false = light mode</param>
        private void ApplyTheme(bool darkMode)
        {
            if (darkMode == true)
            {
                Resources["BgApp"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x0F, 0x19));
                Resources["Surface"] = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
                Resources["TextMain"] = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB));
                Resources["BorderSoft"] = new SolidColorBrush(Color.FromRgb(0x1F, 0x29, 0x37));
                Resources["InputBg"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));

                Resources["Orange"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                Resources["OrangeHover"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));

                Resources["SidebarGradient"] = new LinearGradientBrush(
                    Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16),
                    90);
            }
            else
            {
                Resources["BgApp"] = new SolidColorBrush(Color.FromRgb(0xF6, 0xF7, 0xFB));
                Resources["Surface"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                Resources["TextMain"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));
                Resources["BorderSoft"] = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));
                Resources["InputBg"] = new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC));

                Resources["Orange"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                Resources["OrangeHover"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));

                Resources["SidebarGradient"] = new LinearGradientBrush(
                    Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16),
                    90);
            }
        }
    }
}