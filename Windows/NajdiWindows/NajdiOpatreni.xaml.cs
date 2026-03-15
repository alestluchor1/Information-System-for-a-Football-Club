using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.NajdiWindows
{
    /// <summary>
    /// Okno pro filtrování opatření podle zadaných kritérií
    /// </summary>
    public partial class NajdiOpatreni : Window
    {
        /// <summary>
        /// Kolekce disciplinárních opatření po filtrování
        /// </summary>
        public ObservableCollection<DisciplinarniOpatreni> FiltrovaniDisciplinarniOpatreni { get; private set; } = new ObservableCollection<DisciplinarniOpatreni>();

        /// <summary>
        /// Kolekce disciplinárních opatření, ze které se filtruje
        /// </summary>
        private ObservableCollection<DisciplinarniOpatreni> VstupniDisciplinarniOpatreni;


        public NajdiOpatreni(ObservableCollection<DisciplinarniOpatreni> opatreni)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            VstupniDisciplinarniOpatreni = opatreni;

            if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
            {
                HraciOkno.NactiHrace();
            }

            cbHrac.ItemsSource = HraciOkno.HraciData;
            cbHrac.DisplayMemberPath = "Prijmeni";
            cbHrac.SelectedItem = null;

            dpDatumUkonceni.SelectedDate = null;
            TxtDuvod.Text = "";
            chkByloVZapase.IsChecked = false;
            chkNebylVZapase.IsChecked = false;
        }

        /// <summary>
        /// Spustí filtrování opatření podle zadaných hodnot
        /// a při úspěchu zavře dialog s potvrzeným výsledkem
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniDisciplinarniOpatreni = new ObservableCollection<DisciplinarniOpatreni>(FiltrujOpatreni());
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Vymaže všechny zadané filtry a nastaví formulář do výchozího stavu
        /// </summary>
        private void BtnVymazat_Click(object sender, RoutedEventArgs e)
        {
            cbHrac.SelectedItem = null;
            dpDatumUkonceni.SelectedDate = null;
            TxtDuvod.Text = "";
            chkByloVZapase.IsChecked = false;
            chkNebylVZapase.IsChecked = false;
        }

        /// <summary>
        /// Provede filtrování disciplinárních opatření podle aktuálně vyplněných hodnot ve formuláři
        /// </summary>
        /// <returns>Vrací vyfiltrovanou kolekci disciplinárních opatření</returns>
        private IEnumerable<DisciplinarniOpatreni> FiltrujOpatreni()
        {
            IEnumerable<DisciplinarniOpatreni> vysledek = VstupniDisciplinarniOpatreni;

            // Hráč
            if (cbHrac.SelectedItem != null)
            {
                Hrac vybranyHrac = cbHrac.SelectedItem as Hrac;
                if (vybranyHrac == null)
                {
                    throw new Exception("Vyberte hráče");
                }

                vysledek = vysledek.Where(o => o.IdHrac == vybranyHrac.Id);
            }

            // Datum ukončení
            if (dpDatumUkonceni.SelectedDate != null)
            {
                DateTime datum = dpDatumUkonceni.SelectedDate.Value.Date;
                vysledek = vysledek.Where(o => o.DatumUkonceniOpatreni.Date == datum);
            }

            // Důvod
            if (!string.IsNullOrWhiteSpace(TxtDuvod.Text))
            {
                string duvod = TxtDuvod.Text.Trim();
                vysledek = vysledek.Where(o => o.Duvod != null && o.Duvod.ToLower().Contains(duvod.ToLower()));
            }

            // Opatření bylo v zápase / nebylo v zápase
            if (chkByloVZapase.IsChecked == true && chkNebylVZapase.IsChecked == false)
            {
                vysledek = vysledek.Where(o => o.IdZapas != null);
            }

            if (chkByloVZapase.IsChecked == false && chkNebylVZapase.IsChecked == true)
            {
                vysledek = vysledek.Where(o => o.IdZapas == null);
            }

            return vysledek;
        }

    }
}