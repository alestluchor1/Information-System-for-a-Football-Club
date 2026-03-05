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
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            VstupniDisciplinarniOpatreni = opatreni;

            if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
            {
                HraciOkno.NactiHrace();
            }

            cbHrac.ItemsSource = HraciOkno.HraciData;
            cbHrac.DisplayMemberPath = "prijmeni";
            cbHrac.SelectedItem = null;

            dpDatumUkonceni.SelectedDate = null;
            TxtDuvod.Text = "";
            chkByloVZapase.IsChecked = false;
            chkNebylVZapase.IsChecked = false;
        }

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

        private void BtnVymazat_Click(object sender, RoutedEventArgs e)
        {
            cbHrac.SelectedItem = null;
            dpDatumUkonceni.SelectedDate = null;
            TxtDuvod.Text = "";
            chkByloVZapase.IsChecked = false;
            chkNebylVZapase.IsChecked = false;
        }

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