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
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

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

        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

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

                    return t.Popis.IndexOf(popis, StringComparison.OrdinalIgnoreCase) >= 0;
                });
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
