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
    /// Okno pro filtrování tréninků podle zadaných kritérií
    /// </summary>
    public partial class NajdiTrenink : Window
    {
        /// <summary>
        /// Kolekce tréninků po filtrování
        /// </summary>
        public ObservableCollection<Trenink> FiltrovaniTreninky { get; private set; } = new ObservableCollection<Trenink>();

        /// <summary>
        /// Kolekce tréninků, ze které se filtruje
        /// </summary>
        private ObservableCollection<Trenink> VstupniTreninky;

        public NajdiTrenink(ObservableCollection<Trenink> treninky)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            VstupniTreninky = treninky;

            if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
            {
                TreneriOkno.NactiTreneri();
            }

            cbTrener.ItemsSource = TreneriOkno.TreneriData;
            cbTrener.DisplayMemberPath = "Prijmeni";
            cbTrener.SelectedItem = null;

            dpDatum.SelectedDate = null;
        }

        /// Obsluha tlačítka pro spuštění filtrování
        /// Provede filtrování tréninků podle zadaných hodnot a uloží výsledek do vlastnosti <see cref="FiltrovaniTreninky"/>
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniTreninky = new ObservableCollection<Trenink>(FiltrujTreninky());
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Obsluha tlačítka Zrušit
        /// Zavře okno bez provedení filtrování a vrátí záporný výsledek dialogu
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Provede filtrování tréninků na základě aktuálně vyplněných hodnot ve formuláři
        /// Zadána jsou pouze ta kritéria, která uživatel skutečně vyplnil
        /// </summary>
        /// <returns>
        /// Kolekci tréninků splňujících všechna zadaná kritéria
        /// </returns>
        private IEnumerable<Trenink> FiltrujTreninky()
        {
            IEnumerable<Trenink> vysledek = VstupniTreninky;

            // ID tréninku
            if (!string.IsNullOrWhiteSpace(TxtIdTrenink.Text))
            {
                int id = InputValidator.ValidujId(TxtIdTrenink.Text.Trim());
                vysledek = vysledek.Where(t => t.Id == id);
            }

            // Trenér (vybraný v comboboxu) 
            if (cbTrener.SelectedItem != null)
            {
                Trener vybranyTrener = cbTrener.SelectedItem as Trener;
                if (vybranyTrener == null)
                {
                    throw new Exception("Vyberte trenéra");
                }

                vysledek = vysledek.Where(t => t.IdTrener == vybranyTrener.Id);
            }

            // Datum 
            if (dpDatum.SelectedDate != null)
            {
                DateTime datum = dpDatum.SelectedDate.Value.Date;
                vysledek = vysledek.Where(t => t.Datum.Date == datum);
            }

            // Místo
            if (!string.IsNullOrWhiteSpace(TxtMisto.Text))
            {
                string misto = TxtMisto.Text.Trim();
                InputValidator.ValidujMistoTreninku(misto, "Místo");

                vysledek = vysledek.Where(t =>
                    string.Equals(t.Misto, misto, StringComparison.OrdinalIgnoreCase));
            }

            // Popis
            if (!string.IsNullOrWhiteSpace(TxtPopis.Text))
            {
                string popis = TxtPopis.Text.Trim();
                InputValidator.ValidujPopisTreninku(popis, "Popis");

                vysledek = vysledek.Where(t =>
                {
                    if (t.Popis == null)
                    {
                        return false;
                    }

                    // Porovnání popisu bez ohledu na velikost písmen
                    return t.Popis.IndexOf(popis, StringComparison.OrdinalIgnoreCase) >= 0;
                });
            }

            return vysledek;
        }

    }
}
