using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace InformacniSystemKlub.Windows.NajdiWindows
{
    /// <summary>
    /// Okno pro filtrování sportovního vybavení podle zadaných kritérií
    /// </summary>
    public partial class NajdiSportovniVybaveni : Window
    {
        /// <summary>
        /// Kolekce sportovního vybavení po filtrování
        /// </summary>
        public ObservableCollection<SportovniVybaveni> FiltrovaniVybaveni { get; private set; } = new ObservableCollection<SportovniVybaveni>();

        /// <summary>
        /// Kolekce sportovního vybavení, ze které se filtruje
        /// </summary>
        private ObservableCollection<SportovniVybaveni> VstupniVybaveni;

        public NajdiSportovniVybaveni(ObservableCollection<SportovniVybaveni> vybaveni)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            VstupniVybaveni = vybaveni;

            dpDatumNakupu.SelectedDate = null;
        }

        /// <summary>
        /// Spustí filtrování sportovního vybavení podle zadaných hodnot
        /// a při úspěchu zavře dialog s potvrzeným výsledkem
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniVybaveni = new ObservableCollection<SportovniVybaveni>(FiltrujSportovniVybaveni());
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// Zavře okno filtrování bez potvrzení změn
        /// a nastaví výsledek dialogu na hodnotu false
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Provede filtrování sportovního vybavení podle aktuálně vyplněných hodnot ve formuláři
        /// </summary>
        /// <returns>Vrací vyfiltrovanou kolekci sportovního vybavení</returns>
        private IEnumerable<SportovniVybaveni> FiltrujSportovniVybaveni()
        {
            IEnumerable<SportovniVybaveni> vysledek = VstupniVybaveni;

            // Název
            if (!string.IsNullOrWhiteSpace(TxtNazev.Text))
            {
                string nazev = TxtNazev.Text.Trim();
                InputValidator.ValidujNazevSportovnihoVybaveni(nazev, "Název");

                vysledek = vysledek.Where(t =>
                    string.Equals(t.Nazev, nazev, StringComparison.OrdinalIgnoreCase));
            }

            // Datum 
            if (dpDatumNakupu.SelectedDate != null)
            {
                DateTime datumNakupu = dpDatumNakupu.SelectedDate.Value.Date;
                vysledek = vysledek.Where(t => t.DatumNakupu.Date == datumNakupu);
            }

            // Velikost
            if (!string.IsNullOrWhiteSpace(TxtVelikost.Text))
            {
                string velikost = TxtVelikost.Text.Trim();
                InputValidator.ValidujVelikostSportovnihoVybaveni(velikost, "Velikost");

                vysledek = vysledek.Where(t =>
                    string.Equals(t.Velikost, velikost, StringComparison.OrdinalIgnoreCase));
            }

            return vysledek;
        }

        
    }
}
