using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného tréninku
    /// Při otevření se formulář předvyplní a po potvrzení se změny uloží do databáze
    /// </summary>
    public partial class EditujTrenink : Window
    {
        private readonly Trenink editovanyTrenink;
        private const int MaxPopis = 50;

        public EditujTrenink(Trenink trenink)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            if (trenink == null)
            {
                throw new ArgumentNullException(nameof(trenink));
            }

            editovanyTrenink = trenink;

            TxtPopis.MaxLength = MaxPopis;

            NactiTreneryDoComboBoxu();

            // Předvyplnění
            dpDatum.SelectedDate = editovanyTrenink.Datum;
            TxtMisto.Text = editovanyTrenink.Misto;
            TxtPopis.Text = editovanyTrenink.Popis;
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


                if (editovanyTrenink.IdTrener != 0)
                {
                    foreach (var trener in TreneriOkno.TreneriData)
                    {
                        if (trener.Id == editovanyTrenink.IdTrener)
                        {
                            cbTrener.SelectedItem = trener;
                            break;
                        }
                    }
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

                // Uložení do objektu
                editovanyTrenink.IdTrener = trenerVybrany.Id;
                editovanyTrenink.Prijmeni = trenerVybrany.Prijmeni;
                editovanyTrenink.RodneCislo = trenerVybrany.RodneCislo;
                editovanyTrenink.Datum = datum;
                editovanyTrenink.Misto = misto;
                editovanyTrenink.Popis = popis;

                // Uložení do DB
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreninky.EditujTrenink(conn, editovanyTrenink);

                MessageBox.Show("Trénink byl úspěšně upraven", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci tréninku:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
