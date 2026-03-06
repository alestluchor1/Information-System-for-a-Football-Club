using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
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

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového kontraktu
    /// Naplní ComboBox hráči a po uložení provede validaci a uloží
    /// </summary>
    public partial class PridejKontrakt : Window
    {
        private Kontrakt novyKontrakt;

        public PridejKontrakt(Kontrakt kontrakt)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            novyKontrakt = kontrakt;

            if (novyKontrakt == null)
            {
                novyKontrakt = new Kontrakt();
            }

            NactiHraceDoComboBoxu();
        }

        /// <summary>
        /// Načte hráče do comboboxu
        /// </summary>
        private void NactiHraceDoComboBoxu()
        {
            try
            {
                if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
                {
                    HraciOkno.NactiHrace();
                }

                cbHrac.ItemsSource = HraciOkno.HraciData;
                cbHrac.DisplayMemberPath = "Prijmeni";

                if (cbHrac.Items.Count > 0)
                {
                    cbHrac.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodařilo se načíst hráče:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsluha tlačítka Přidat/Uložit
        /// Provede validaci všech vstupů a uloží hodnoty do objektu 
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int idHrace = InputValidator.ValidujVyberHrace(cbHrac.SelectedItem);

                InputValidator.ValidujRozsahDatumuKontraktu(dpDatumOd.SelectedDate, dpDatumDo.SelectedDate);

                DateOnly datumOd = DateOnly.FromDateTime(dpDatumOd.SelectedDate.Value.Date);
                DateOnly datumDo = DateOnly.FromDateTime(dpDatumDo.SelectedDate.Value.Date);

                long plat = InputValidator.ValidujPlatKontraktuLong(TxtPlat.Text);

                InputValidator.ValidujTelefonniCislo(TxtTelAgenta.Text);
                string telAgenta = TxtTelAgenta.Text.Trim();

                // Výstupní klauzule (může být prázdná => 0)
                long vystupniKlauzule = InputValidator.ValidujVystupniKlauzuliLong(TxtVystupniKlauzule.Text);

                // Uložení do objektu kontraktu 
                novyKontrakt.IdHrac = idHrace;
                novyKontrakt.DatumZacatku = datumOd;
                novyKontrakt.DatumKonce = datumDo;
                novyKontrakt.Plat = plat;
                novyKontrakt.TelCisloNaAgenta = telAgenta;
                novyKontrakt.VystupniKlauzule = vystupniKlauzule;

               // MessageBox.Show("Kontrakt byl úspěšně přidán", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
