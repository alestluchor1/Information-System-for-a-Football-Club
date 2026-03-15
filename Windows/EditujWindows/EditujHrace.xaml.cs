using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného hráče
    /// Při otevření se formulář předvyplní hodnotami hráče a po potvrzení se změny uloží do databáze
    /// pomocí procedury <c>PKG_HRACI.SP_UPDATE_HRAC</c> 
    /// </summary>
    public partial class EditujHrace : Window
    {

        private Hrac editovanyHrac;

        public EditujHrace(Hrac editovanyHrac)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            this.editovanyHrac = editovanyHrac;

            TxtRodneCislo.Text = editovanyHrac.RodneCislo;
            TxtJmeno.Text = editovanyHrac.Jmeno;
            TxtPrijmeni.Text = editovanyHrac.Prijmeni;
            TxtTelefonniCislo.Text = editovanyHrac.TelefonniCislo;
            TxtPozice.Text = editovanyHrac.Pozice;

            iudPocetGolu.Value = editovanyHrac.PocetVstrelenychGolu;
            iudPocetZlutychKaret.Value = editovanyHrac.PocetZlutychKaret;
            iudPocetCervenychKaret.Value = editovanyHrac.PocetCervenychKaret;
            cbSkupina.SelectedIndex = editovanyHrac.IdTreninkovaSkupina;

            if (cbSkupina.SelectedIndex < 0 || cbSkupina.SelectedIndex > 3)
            {
                cbSkupina.SelectedIndex = 0;
            }

            NastavZdravotniStav(editovanyHrac.ZdravotniStav);
        }

        /// <summary>
        /// Nastaví vybranou položku v ComboBoxu zdravotního stavu podle hodnoty (A/N)
        /// Pokud se položka nenajde, nastaví se defaultně "—"
        /// </summary>
        /// <param name="stav">Zdravotní stav (typicky 'A' nebo 'N')</param>
        private void NastavZdravotniStav(char stav)
        {
            foreach (object item in cbZdraviStav.Items)
            {
                ComboBoxItem polozka = item as ComboBoxItem;
                if (polozka != null)
                {
                    string text = polozka.Content as string;
                    if (text != null)
                    {
                        text = text.Trim();

                        if (text.Length == 1)
                        {
                            if (text[0] == stav)
                            {
                                cbZdraviStav.SelectedItem = polozka;
                                return;
                            }
                        }
                    }
                }
            }

            cbZdraviStav.SelectedIndex = 0; // —
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
                string pozice = TxtPozice.Text.Trim();

                // Validace textů
                InputValidator.ValidujRodneCislo(rodneCislo);
                InputValidator.ValidujJmeno(jmeno);
                InputValidator.ValidujPrijmeni(prijmeni);
                InputValidator.ValidujTelefonniCislo(tel);
                InputValidator.ValidujPozice(pozice);

                // Hodnoty ze spinnerů
                string golyText = "";
                if (iudPocetGolu.Value != null)
                {
                    golyText = iudPocetGolu.Value.ToString();
                }

                string zluteText = "";
                if (iudPocetZlutychKaret.Value != null)
                {
                    zluteText = iudPocetZlutychKaret.Value.ToString();
                }

                string cerveneText = "";
                if (iudPocetCervenychKaret.Value != null)
                {
                    cerveneText = iudPocetCervenychKaret.Value.ToString();
                }

                int goly = InputValidator.ValidujHraciSpinnerGolyKarty(golyText, "Počet vstřelených gólů");
                int zlute = InputValidator.ValidujHraciSpinnerGolyKarty(zluteText, "Počet žlutých karet");
                int cervene = InputValidator.ValidujHraciSpinnerGolyKarty(cerveneText, "Počet červených karet");

                // Zdravotní stav
                char zdrav = InputValidator.ValidujZdravotniStav(cbZdraviStav.SelectedItem);

                // Tréninková skupina
                if (cbSkupina.SelectedIndex == 0)
                {
                    MessageBox.Show("Vyber tréninkovou skupinu!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int idSkupiny = cbSkupina.SelectedIndex;

                // Uložení do objektu
                editovanyHrac.RodneCislo = rodneCislo;
                editovanyHrac.Jmeno = jmeno;
                editovanyHrac.Prijmeni = prijmeni;
                editovanyHrac.TelefonniCislo = tel;
                editovanyHrac.Pozice = pozice;

                editovanyHrac.PocetVstrelenychGolu = goly;
                editovanyHrac.PocetZlutychKaret = zlute;
                editovanyHrac.PocetCervenychKaret = cervene;

                editovanyHrac.ZdravotniStav = zdrav;
                editovanyHrac.IdTreninkovaSkupina = idSkupiny;

                // Aktualizace textu pro DataGrid
                if (idSkupiny == 1)
                {
                    editovanyHrac.TreninkovaSkupinaText = "A-Tým";
                }
                else if (idSkupiny == 2)
                {
                    editovanyHrac.TreninkovaSkupinaText = "Dorost";
                }
                else if (idSkupiny == 3)
                {
                    editovanyHrac.TreninkovaSkupinaText = "Mladší žáci";
                }
                else
                {
                    editovanyHrac.TreninkovaSkupinaText = "Neznámá";
                }

                // Uložení do DB
                OracleConnection conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeHraci.EditaceHrace(conn, editovanyHrac);

                MessageBox.Show("Změny byly úspěšně uloženy", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci hráče:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při editaci hráče:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsluha tlačítka Zrušit
        /// Okno se zavře bez uložení změn
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
