using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;

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
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            this.editovanyTrener = editovanyTrener;

            TxtRodneCislo.Text = editovanyTrener.RodneCislo;
            TxtJmeno.Text = editovanyTrener.Jmeno;
            TxtPrijmeni.Text = editovanyTrener.Prijmeni;
            TxtTelefonniCislo.Text = editovanyTrener.TelefonniCislo;
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
                string specializace = TxtSpecializace.Text.Trim();

                // Validace
                InputValidator.ValidujRodneCislo(rodneCislo);
                InputValidator.ValidujUnikatniRodneCislo(rodneCislo);
                InputValidator.ValidujJmeno(jmeno);
                InputValidator.ValidujPrijmeni(prijmeni);
                InputValidator.ValidujTelefonniCislo(tel);
                InputValidator.ValidujSpecializace(specializace);

                // Validace spinneru
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
                editovanyTrener.Specializace = specializace;
                editovanyTrener.PocetLetPraxe = pocetLetPraxe;

                // Update DB
                OracleConnection conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreneri.EditaceTrenera(conn, editovanyTrener);

                MessageBox.Show("Změny byly úspěšně uloženy", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
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
            Close();
        }
    }
}