using InformacniSystemKlub.Models;
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

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového kontraktu
    /// Naplní ComboBox hráči a po uložení provede validaci a uloží
    /// </summary>
    public partial class PridejKontrakt : Window
    {
        private Kontrakt novyKontrakt;

        public PridejKontrakt(Kontrakt kontrakt)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            novyKontrakt = kontrakt;

            if (novyKontrakt == null)
            {
                novyKontrakt = new Kontrakt();
            }

            NactiHraceDoComboBoxu();
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
        /// Obsluha tlačítka Přidat/Uložit
        /// Provede validaci všech vstupů a uloží hodnoty do objektu 
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int idHrace = InputValidator.ValidujVyberHrace(cbHrac.SelectedItem);

                InputValidator.ValidujRozsahDatumuKontraktu(dpDatumOd.SelectedDate, dpDatumDo.SelectedDate);

                DateOnly datumOd = DateOnly.FromDateTime(dpDatumOd.SelectedDate.Value.Date);
                DateOnly datumDo = DateOnly.FromDateTime(dpDatumDo.SelectedDate.Value.Date);

                long plat = InputValidator.ValidujPlatKontraktuLong(TxtPlat.Text);

                InputValidator.ValidujTelefonniCislo(TxtTelAgenta.Text);
                string telAgenta = TxtTelAgenta.Text.Trim();

                // Výstupní klauzule (může být prázdná => 0)
                long vystupniKlauzule = InputValidator.ValidujVystupniKlauzuliLong(TxtVystupniKlauzule.Text);

                // Uložení do objektu kontraktu 
                novyKontrakt.IdHrac = idHrace;
                novyKontrakt.DatumZacatku = datumOd;
                novyKontrakt.DatumKonce = datumDo;
                novyKontrakt.Plat = plat;
                novyKontrakt.TelCisloNaAgenta = telAgenta;
                novyKontrakt.VystupniKlauzule = vystupniKlauzule;

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
