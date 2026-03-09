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
    /// Okno pro filtrování uživatelských účtů podle zadaných kritérií
    /// </summary>
    public partial class NajdiNastaveni : Window
    {
        /// <summary>
        /// Kolekce uživatelů po filtrování
        /// </summary>
        public ObservableCollection<Uzivatel> FiltrovaniUzivatele { get; private set; } = new ObservableCollection<Uzivatel>();

        /// <summary>
        /// Kolekce uživatelů, ze které se filtruje
        /// </summary>
        private ObservableCollection<Uzivatel> VstupniUzivatele;

        public NajdiNastaveni(ObservableCollection<Uzivatel> uzivatele)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            VstupniUzivatele = uzivatele;

            CbRole.SelectedItem = null;
            DpPosledniPrihlaseni.SelectedDate = null;
        }

        /// <summary>
        /// Obsluha tlačítka Najdi
        /// Provede filtrování uživatelů podle zadaných kritérií
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniUzivatele = new ObservableCollection<Uzivatel>(FiltrujUzivatele());
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
        /// Zavře okno bez použití filtrů
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Vrátí seznam uživatelů odpovídající zadaným filtrům
        /// </summary>
        private IEnumerable<Uzivatel> FiltrujUzivatele()
        {
            IEnumerable<Uzivatel> vysledek = VstupniUzivatele;

            // Filtr podle uživatelského jména
            if (!string.IsNullOrWhiteSpace(TxtUzivatelskeJmeno.Text))
            {
                string uzivatelskeJmeno = InputValidator.ValidujUzivatelskeJmeno(TxtUzivatelskeJmeno.Text.Trim());

                vysledek = vysledek.Where(u =>
                    string.Equals(u.UzivatelskeJmeno, uzivatelskeJmeno, StringComparison.OrdinalIgnoreCase));
            }

            // Filtr podle e-mailu
            if (!string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                string email = InputValidator.ValidujEmail(TxtEmail.Text.Trim());

                vysledek = vysledek.Where(u =>
                    string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            }

            // Filtr podle role
            if (CbRole.SelectedItem != null)
            {
                ComboBoxItem vybranaRole = CbRole.SelectedItem as ComboBoxItem;
                if (vybranaRole == null)
                {
                    throw new Exception("Vyberte roli");
                }

                string role = vybranaRole.Content.ToString();

                vysledek = vysledek.Where(u =>
                    string.Equals(u.Role, role, StringComparison.OrdinalIgnoreCase));
            }

            // Filtr podle dne posledního přihlášení
            if (DpPosledniPrihlaseni.SelectedDate != null)
            {
                DateTime datum = DpPosledniPrihlaseni.SelectedDate.Value.Date;

                vysledek = vysledek.Where(u => u.PosledniPrihlaseni.Date == datum);
            }

            // Filtr podle rodného čísla
            if (!string.IsNullOrWhiteSpace(TxtRodneCislo.Text))
            {
                string rodneCislo = TxtRodneCislo.Text.Trim();

                vysledek = vysledek.Where(u =>
                {
                    if (u.RodneCislo == null)
                    {
                        return false;
                    }

                    return string.Equals(u.RodneCislo, rodneCislo, StringComparison.OrdinalIgnoreCase);
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