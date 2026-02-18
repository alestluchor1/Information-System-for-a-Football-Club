using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
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

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného trenéra
    /// Při otevření se formulář předvyplní hodnotami trenéra a po potvrzení se změny uloží do databáze
    /// pomocí procedury <c>PKG_TRENERI.SP_UPDATE_TRENERI</c> 
    /// </summary>
    public partial class EditujTrenera : Window
    {
        private Trener editovanyTrener;

        public EditujTrenera(Trener editovanyTrener)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            this.editovanyTrener = editovanyTrener;

            TxtRodneCislo.Text = editovanyTrener.RodneCislo;
            TxtJmeno.Text = editovanyTrener.Jmeno;
            TxtPrijmeni.Text = editovanyTrener.Prijmeni;
            TxtTelefonniCislo.Text = editovanyTrener.TelefonniCislo;
            TxtTrenerskaLicence.Text = editovanyTrener.TrenerskaLicence;
            TxtSpecializace.Text = editovanyTrener.Specializace;

            iudPocetLetPraxe.Value = editovanyTrener.PocetLetPraxe;

        }

        /// <summary>
        /// Načte hodnoty z formuláře, provede validaci a uloží změny do databáze
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string rodneCislo = TxtRodneCislo.Text.Trim();
                string jmeno = TxtJmeno.Text.Trim();
                string prijmeni = TxtPrijmeni.Text.Trim();
                string tel = TxtTelefonniCislo.Text.Trim();
                string licence = TxtTrenerskaLicence.Text.Trim();
                string specializace = TxtSpecializace.Text.Trim();

                // Validace 
                InputValidator.ValidujRodneCislo(rodneCislo);
                InputValidator.ValidujJmeno(jmeno);
                InputValidator.ValidujPrijmeni(prijmeni);
                InputValidator.ValidujTelefonniCislo(tel);
                InputValidator.ValidujTrenerskaLicence(licence);
                InputValidator.ValidujSpecializace(specializace);

                // Validace spinneru (může být null)
                string praxeText = "";
                if (iudPocetLetPraxe.Value != null)
                {
                    praxeText = iudPocetLetPraxe.Value.ToString();
                }

                int pocetLetPraxe = InputValidator.ValidujTreneriPocetLetPraxe(praxeText, "Počet let praxe");

                editovanyTrener.RodneCislo = rodneCislo;
                editovanyTrener.Jmeno = jmeno;
                editovanyTrener.Prijmeni = prijmeni;
                editovanyTrener.TelefonniCislo = tel;
                editovanyTrener.TrenerskaLicence = licence;
                editovanyTrener.Specializace = specializace;
                editovanyTrener.PocetLetPraxe = pocetLetPraxe;

                // Update DB 
                OracleConnection conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreneri.EditaceTrenera(conn, editovanyTrener);

                MessageBox.Show("Změny byly úspěšně uloženy", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci trenéra:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při editaci trenéra:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsluha tlačítka Zrušit. Zavře okno bez uložení změn
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Nastaví (dark / light) mode
        /// </summary>
        /// <param name="darkMode">True = dark mode, false = light mode</param>
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

