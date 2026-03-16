using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného zápasu
    /// Při otevření se formulář předvyplní hodnotami zápasu a po potvrzení se změny uloží do databáze
    /// </summary>
    public partial class EditujZapas : Window
    {
        private Zapas editovanyZapas;

        public EditujZapas(Zapas editovanyZapas)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            this.editovanyZapas = editovanyZapas;

            DpDatum.SelectedDate = editovanyZapas.Datum.Date;
            TxtDomaciTym.Text = editovanyZapas.DomaciTym;
            TxtHosteTym.Text = editovanyZapas.HosteTym;
            TxtVysledek.Text = editovanyZapas.Vysledek;

            if (editovanyZapas.PocetZlutychKaret != null)
            {
                iudPocetZlutychKaret.Value = editovanyZapas.PocetZlutychKaret;
            }
            else
            {
                iudPocetZlutychKaret.Value = null;
            }

            if (editovanyZapas.PocetCervenychKaret != null)
            {
                iudPocetCervenychKaret.Value = editovanyZapas.PocetCervenychKaret;
            }
            else
            {
                iudPocetCervenychKaret.Value = null;
            }

            NastavTypSouteze(editovanyZapas.IdTypSouteze);
            NastavStavZapasu(editovanyZapas.IdStav);
            NastavPoleProStavZapasu();
        }

        /// <summary>
        /// Nastaví vybranou položku ComboBoxu typu soutěže podle enum hodnoty
        /// </summary>
        private void NastavTypSouteze(TypSoutezeEnum typSouteze)
        {
            foreach (object item in CbTypSouteze.Items)
            {
                ComboBoxItem polozka = item as ComboBoxItem;

                if (polozka != null)
                {
                    string text = polozka.Content.ToString().Trim();

                    if (typSouteze == TypSoutezeEnum.Liga && text == "LIGA")
                    {
                        CbTypSouteze.SelectedItem = polozka;
                        return;
                    }

                    if (typSouteze == TypSoutezeEnum.Pohar && text == "POHÁR")
                    {
                        CbTypSouteze.SelectedItem = polozka;
                        return;
                    }

                    if (typSouteze == TypSoutezeEnum.Divize && text == "DIVIZE")
                    {
                        CbTypSouteze.SelectedItem = polozka;
                        return;
                    }

                    if (typSouteze == TypSoutezeEnum.Kraj && text == "KRAJ")
                    {
                        CbTypSouteze.SelectedItem = polozka;
                        return;
                    }

                    if (typSouteze == TypSoutezeEnum.Okres && text == "OKRES")
                    {
                        CbTypSouteze.SelectedItem = polozka;
                        return;
                    }
                }
            }

            CbTypSouteze.SelectedIndex = 0;
        }

        /// <summary>
        /// Nastaví vybranou položku ComboBoxu stavu zápasu podle enum hodnoty
        /// </summary>
        private void NastavStavZapasu(StavZapasuEnum stavZapasu)
        {
            foreach (object item in CbStavZapasu.Items)
            {
                ComboBoxItem polozka = item as ComboBoxItem;

                if (polozka != null)
                {
                    string text = polozka.Content.ToString().Trim();

                    if (stavZapasu == StavZapasuEnum.Odehrano && text == "ODEHRÁNO")
                    {
                        CbStavZapasu.SelectedItem = polozka;
                        return;
                    }

                    if (stavZapasu == StavZapasuEnum.BudeSeHrat && text == "BUDE SE HRÁT")
                    {
                        CbStavZapasu.SelectedItem = polozka;
                        return;
                    }
                }
            }

            CbStavZapasu.SelectedIndex = 0;
        }

        /// <summary>
        /// Načte hodnoty z formuláře, provede validaci a uloží změny do databáze
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

                editovanyZapas.Datum = datum;
                editovanyZapas.DomaciTym = domaciTym;
                editovanyZapas.HosteTym = hosteTym;
                editovanyZapas.IdTypSouteze = ZiskejTypSoutezeEnum(typSoutezeText);
                editovanyZapas.IdStav = ZiskejStavZapasuEnum(stavZapasuText);

                if (editovanyZapas.IdStav == StavZapasuEnum.Odehrano)
                {
                    if (editovanyZapas.Datum > DateTime.Today)
                    {
                        MessageBox.Show(
                            "Nelze uložit odehraný zápas s budoucím datem!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    editovanyZapas.Vysledek = vysledek;
                    editovanyZapas.PocetZlutychKaret = pocetZlutychKaret;
                    editovanyZapas.PocetCervenychKaret = pocetCervenychKaret;
                }

                if (editovanyZapas.IdStav == StavZapasuEnum.BudeSeHrat)
                {
                    if (editovanyZapas.Datum < DateTime.Today)
                    {
                        MessageBox.Show(
                            "Nelze uložit neodehraný zápas s minulým datem!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    editovanyZapas.Vysledek = "-";
                    editovanyZapas.PocetZlutychKaret = null;
                    editovanyZapas.PocetCervenychKaret = null;
                }

                OracleConnection conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeZapasy.EditaceZapasu(conn, editovanyZapas);

                // Když se zápas změní na BUDE SE HRÁT, vynulují se statistiky nominací pro tento zápas
                if (editovanyZapas.IdStav == StavZapasuEnum.BudeSeHrat)
                {
                    DatabazeNominace.VynulujNominaceProZapas(conn, editovanyZapas.IdZapas);
                    HraciOkno.NactiHrace();
                }

                MessageBox.Show("Změny byly úspěšně uloženy", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci zápasu:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při editaci zápasu:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (item == null)
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
                if (TxtVysledek.Text == "-")
                {
                    TxtVysledek.Text = "";
                }

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

       
    }
}