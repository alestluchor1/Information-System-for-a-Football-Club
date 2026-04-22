using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Controls;

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

            // Nastaví v ComboBoxu aktuální tréninkovou skupinu daného trenéra
            foreach (ComboBoxItem item in CbTreninkovaSkupina.Items)
            {
                if (item.Tag != null && Convert.ToInt32(item.Tag) == editovanyTrener.IdTreninkovaSkupina)
                {
                    CbTreninkovaSkupina.SelectedItem = item;
                    break;
                }
            }
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

                ComboBoxItem vybranaPolozka = CbTreninkovaSkupina.SelectedItem as ComboBoxItem;

                if (vybranaPolozka == null)
                {
                    throw new Exception("Vyberte tréninkovou skupinu");
                }

                // Validace
                InputValidator.ValidujRodneCislo(rodneCislo);
                InputValidator.ValidujUnikatniRodneCislo(rodneCislo, editovanyTrener.Id); // při editaci, aby neházelo message, že daný trenér editovaný s rodným číslem, už existuje
                InputValidator.ValidujJmeno(jmeno);
                InputValidator.ValidujPrijmeni(prijmeni);
                InputValidator.ValidujTelefonniCislo(tel);
                InputValidator.ValidujSpecializace(specializace);

                editovanyTrener.RodneCislo = rodneCislo;
                editovanyTrener.Jmeno = jmeno;
                editovanyTrener.Prijmeni = prijmeni;
                editovanyTrener.TelefonniCislo = tel;
                editovanyTrener.Specializace = specializace;
                editovanyTrener.IdTreninkovaSkupina = Convert.ToInt32(vybranaPolozka.Tag);
                editovanyTrener.NazevSkupiny = vybranaPolozka.Content.ToString();

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