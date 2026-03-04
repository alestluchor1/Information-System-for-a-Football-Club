using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            VstupniVybaveni = vybaveni;

            dpDatumNakupu.SelectedDate = null;
        }

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

        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

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

        /// <summary>
        /// Nastaví zvolený režim (dark/light)
        /// </summary>
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
                Resources["SidebarGradient"] = new LinearGradientBrush(Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16), 90);
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
