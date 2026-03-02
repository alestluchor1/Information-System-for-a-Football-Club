using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InformacniSystemKlub.Models;

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového disciplinárního opatření hráči
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
            NactiZapasyDoComboBoxu();
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
        /// Načte zápasy do comboboxu, zápasy jen se stavem odehráno
        /// </summary>
        private void NactiZapasyDoComboBoxu()
        {
            try
            {
                cbZapas.ItemsSource = ZapasyOkno.ZapasyData.Where(z => z.IdStav == StavZapasuEnum.Odehrano).ToList();
                cbZapas.DisplayMemberPath = "ZapasDoComboBoxu";

                if (cbZapas.Items.Count > 0)
                {
                    cbZapas.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodařilo se načíst zápasy:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Po zaškrtnutí CheckBoxu povolí ComboBox se zápasy,
        /// aby bylo možné vybrat zápas, ke kterému se opatření vztahuje
        /// </summary>
        private void chkByloVZapase_Checked(object sender, RoutedEventArgs e)
        {
            cbZapas.IsEnabled = true;
        }

        /// <summary>
        /// Po odškrtnutí CheckBoxu zakáže ComboBox se zápasy
        /// a zruší případně vybraný zápas
        /// </summary>
        private void chkByloVZapase_Unchecked(object sender, RoutedEventArgs e)
        {
            cbZapas.IsEnabled = false;
            cbZapas.SelectedIndex = -1;
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
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

                DateTime datumUkonceni = InputValidator.ValidujDatumUkonceniOpatreni(dpDatumUkonceni.SelectedDate);
                string duvod = InputValidator.ValidujDuvodOpatreni(TxtDuvod.Text);

                novyOpatreni.IdHrac = hracVybrany.id;
                novyOpatreni.Jmeno = hracVybrany.jmeno;
                novyOpatreni.Prijmeni = hracVybrany.prijmeni;
                novyOpatreni.DatumUkonceniOpatreni = datumUkonceni;
                novyOpatreni.Duvod = duvod;

                if (chkByloVZapase.IsChecked == true)
                {
                    Zapas vybranyZapas = cbZapas.SelectedItem as Zapas;

                    if (vybranyZapas == null)
                    {
                        throw new Exception("Vyberte zápas");
                    }

                    novyOpatreni.IdZapas = vybranyZapas.IdZapas;
                }
                else
                {
                    novyOpatreni.IdZapas = null;
                }

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