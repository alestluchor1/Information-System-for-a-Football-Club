using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného disciplinárního opatření
    /// Při otevření se formulář předvyplní hodnotami opatření a po potvrzení se změny uloží do databáze
    /// </summary>
    public partial class EditujOpatreni : Window
    {
        private DisciplinarniOpatreni editovanyOpatreni;

        public EditujOpatreni(DisciplinarniOpatreni editovanyOpatreni)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            this.editovanyOpatreni = editovanyOpatreni;

            NactiHraceDoComboBoxu();
            NactiZapasyDoComboBoxu();

            dpDatumUkonceni.SelectedDate = editovanyOpatreni.DatumUkonceniOpatreni;
            TxtDuvod.Text = editovanyOpatreni.Duvod;

            NastavVybranehoHrace(editovanyOpatreni.IdHrac);

            if (editovanyOpatreni.IdZapas != null)
            {
                chkByloVZapase.IsChecked = true;
                cbZapas.IsEnabled = true;
                NastavVybranyZapas(editovanyOpatreni.IdZapas.Value);
            }
            else
            {
                chkByloVZapase.IsChecked = false;
                cbZapas.IsEnabled = false;
                cbZapas.SelectedIndex = -1;
            }
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodařilo se načíst zápasy:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Nastaví vybraného hráče v ComboBoxu podle ID
        /// </summary>
        private void NastavVybranehoHrace(int idHrac)
        {
            foreach (object item in cbHrac.Items)
            {
                Hrac hrac = item as Hrac;

                if (hrac != null)
                {
                    if (hrac.Id == idHrac)
                    {
                        cbHrac.SelectedItem = hrac;
                        return;
                    }
                }
            }

            cbHrac.SelectedIndex = -1;
        }

        /// <summary>
        /// Nastaví vybraný zápas v ComboBoxu podle ID
        /// </summary>
        private void NastavVybranyZapas(int idZapas)
        {
            foreach (object item in cbZapas.Items)
            {
                Zapas zapas = item as Zapas;

                if (zapas != null)
                {
                    if (zapas.IdZapas == idZapas)
                    {
                        cbZapas.SelectedItem = zapas;
                        return;
                    }
                }
            }

            cbZapas.SelectedIndex = -1;
        }

        /// <summary>
        /// Po zaškrtnutí CheckBoxu povolí ComboBox se zápasy,
        /// aby bylo možné vybrat zápas, ke kterému se opatření vztahuje.
        /// </summary>
        private void chkByloVZapase_Checked(object sender, RoutedEventArgs e)
        {
            cbZapas.IsEnabled = true;
        }

        /// <summary>
        /// Po odškrtnutí CheckBoxu zakáže ComboBox se zápasy
        /// a zruší případně vybraný zápas.
        /// </summary>
        private void chkByloVZapase_Unchecked(object sender, RoutedEventArgs e)
        {
            cbZapas.IsEnabled = false;
            cbZapas.SelectedIndex = -1;
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="DisciplinarniOpatreni"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime datumUkonceni = InputValidator.ValidujDatumUkonceniOpatreni(dpDatumUkonceni.SelectedDate);
                string duvod = InputValidator.ValidujDuvodOpatreni(TxtDuvod.Text);

                editovanyOpatreni.DatumUkonceniOpatreni = datumUkonceni;
                editovanyOpatreni.Duvod = duvod;

                if (chkByloVZapase.IsChecked == true)
                {
                    Zapas vybranyZapas = cbZapas.SelectedItem as Zapas;

                    if (vybranyZapas == null)
                    {
                        throw new Exception("Vyberte zápas");
                    }

                    editovanyOpatreni.IdZapas = vybranyZapas.IdZapas;
                }
                else
                {
                    editovanyOpatreni.IdZapas = null;
                }

                OracleConnection conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeOpatreni.EditaceOpatreni(conn, editovanyOpatreni);

                MessageBox.Show("Změny byly úspěšně uloženy", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci opatření:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při editaci opatření:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}