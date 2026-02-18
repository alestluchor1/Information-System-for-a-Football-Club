using InformacniSystemKlub.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového tréninku
    /// Naplní ComboBox trenéry a po uložení provede validaci a uloží
    /// </summary>
    public partial class PridejTrenink : Window
    {
        private Trenink novyTrenink;

        public PridejTrenink(Trenink trenink)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            novyTrenink = trenink;

            if (novyTrenink == null)
            {
                novyTrenink = new Trenink();
            }

            NactiTreneryDoComboBoxu();
        }

        /// <summary>
        /// Načte trenéry do comboboxu
        /// </summary>
        private void NactiTreneryDoComboBoxu()
        {
            try
            {
                if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
                {
                    TreneriOkno.NactiTreneri();
                }

                cbTrener.ItemsSource = TreneriOkno.TreneriData;

                cbTrener.DisplayMemberPath = "Prijmeni";

                if (cbTrener.Items.Count > 0)
                {
                    cbTrener.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodařilo se načíst trenéry:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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
                InputValidator.ValidujVyberTrenera(cbTrener.SelectedItem);

                Trener trenerVybrany = cbTrener.SelectedItem as Trener;
                if (trenerVybrany == null)
                {
                    throw new Exception("Vyberte trenéra");
                }

                DateTime datum = InputValidator.ValidujDatumTreninku(dpDatum.SelectedDate, "Datum tréninku");
                string misto = InputValidator.ValidujMistoTreninku(TxtMisto.Text, "Místo");

                string popis = TxtPopis.Text;
                if (popis == null)
                {
                    popis = "";
                }
                popis = InputValidator.ValidujPopisTreninku(popis, "Popis");

                novyTrenink.IdTrener = trenerVybrany.Id;
                novyTrenink.Prijmeni = trenerVybrany.Prijmeni;
                novyTrenink.RodneCislo = trenerVybrany.RodneCislo;
                novyTrenink.Datum = datum;
                novyTrenink.Misto = misto;
                novyTrenink.Popis = popis;

                MessageBox.Show("Trénink byl úspěšně přidán", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

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
