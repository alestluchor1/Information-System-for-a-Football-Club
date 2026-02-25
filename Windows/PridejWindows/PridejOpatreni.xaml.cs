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
using InformacniSystemKlub.Models;

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového disciplinárního opatření hráči
    /// Naplní ComboBox hráči a 
    /// provede validaci 
    /// </summary>
    public partial class PridejOpatreni : Window
    {
        private DisciplinarniOpatreni novyOpatreni;
        public PridejOpatreni(DisciplinarniOpatreni opatreni)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            this.novyOpatreni = opatreni;

            if (novyOpatreni == null)
            {
                novyOpatreni = new DisciplinarniOpatreni();
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

                cbHrac.DisplayMemberPath = "prijmeni";

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
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="Trenink"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputValidator.ValidujVyberHrace(cbHrac.SelectedItem);

                Hrac hracVybrany = cbHrac.SelectedItem as Hrac;

                if (hracVybrany == null)
                {
                    throw new Exception("Vyberte hráče");
                }

                // Validace
                DateTime datumUkonceni = InputValidator.ValidujDatumUkonceniOpatreni(dpDatumUkonceni.SelectedDate);
                string duvod = InputValidator.ValidujDuvodOpatreni(TxtDuvod.Text);

                // Uložení do objektu
                novyOpatreni.IdHrac = hracVybrany.id;                 
                novyOpatreni.Jmeno = hracVybrany.jmeno;            
                novyOpatreni.Prijmeni = hracVybrany.prijmeni;        
                novyOpatreni.DatumUkonceniOpatreni = datumUkonceni;
                novyOpatreni.Duvod = duvod;

                MessageBox.Show("Opatření bylo úspěšně přidáno", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

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
