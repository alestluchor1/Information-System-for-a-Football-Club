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
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            this.novyOpatreni = opatreni;

            if (novyOpatreni == null)
            {
                novyOpatreni = new DisciplinarniOpatreni();
            }

            NactiHraceDoComboBoxu();
            NactiZapasyDoComboBoxu();
        }

        /// <summary>
        /// Načte seznam hráčů do ComboBoxu
        /// </summary>
        private void NactiHraceDoComboBoxu()
        {
            try
            {
                // index první položky v ComboBoxu
                int prvniPolozkaIndex = 0;

                if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
                {
                    HraciOkno.NactiHrace();
                }

                cbHrac.ItemsSource = HraciOkno.HraciData;

                // V ComboBoxu bude u každého hráče zobrazovat jeho příjmení
                cbHrac.DisplayMemberPath = "Prijmeni";

                // Pokud ComboBox obsahuje alespoň jednu položku, vybere se automaticky první hráč
                if (cbHrac.Items.Count > prvniPolozkaIndex)
                {
                    cbHrac.SelectedIndex = prvniPolozkaIndex;
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

                novyOpatreni.IdHrac = hracVybrany.Id;
                novyOpatreni.Jmeno = hracVybrany.Jmeno;
                novyOpatreni.Prijmeni = hracVybrany.Prijmeni;
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

        
    }
}