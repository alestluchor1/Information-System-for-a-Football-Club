using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného kontraktu
    /// Při otevření se formulář předvyplní a po potvrzení se změny uloží do databáze
    /// </summary>
    public partial class EditujKontrakt : Window
    {
        private Kontrakt editovanyKontrakt;

        public EditujKontrakt(Kontrakt kontrakt)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            if (kontrakt == null)
            {
                throw new ArgumentNullException(nameof(kontrakt));
            }

            editovanyKontrakt = kontrakt;

            NactiHraceDoComboBoxu();

            // Předvyplnění
            dpDatumOd.SelectedDate = editovanyKontrakt.DatumZacatku.ToDateTime(TimeOnly.MinValue);
            dpDatumDo.SelectedDate = editovanyKontrakt.DatumKonce.ToDateTime(TimeOnly.MinValue);
            TxtTelAgenta.Text = editovanyKontrakt.TelCisloNaAgenta.ToString();
            TxtPlat.Text = editovanyKontrakt.Plat.ToString();

            // Pokud výstupní klauzule není vyplněná, zobrazí se pomlčka "-"
            if (editovanyKontrakt.JeVystupniKlauzuleVyplnena == true)
            {
                TxtVystupniKlauzule.Text = editovanyKontrakt.VystupniKlauzule.ToString();
            }
            else
            {
                TxtVystupniKlauzule.Text = "-";
            }
        }

        /// <summary>
        /// Načte hráče do ComboBoxu
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

                // Určuje, že se v ComboBoxu bude zobrazovat příjmení hráče
                cbHrac.DisplayMemberPath = "Prijmeni";

                // Vybere hráče, který je přiřazen k editovanému kontraktu
                if (editovanyKontrakt.IdHrac != 0)
                {
                    foreach (var hrac in HraciOkno.HraciData)
                    {
                        if (hrac.Id == editovanyKontrakt.IdHrac)
                        {
                            cbHrac.SelectedItem = hrac;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodařilo se načíst hráče:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="Kontrakt"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validace
                InputValidator.ValidujRozsahDatumuKontraktu(dpDatumOd.SelectedDate, dpDatumDo.SelectedDate);

                // Převede datum začátku kontraktu z DatePickeru
                DateOnly datumOd = DateOnly.FromDateTime(dpDatumOd.SelectedDate.Value.Date);

                // Převede datum konce kontraktu z DatePickeru
                DateOnly datumDo = DateOnly.FromDateTime(dpDatumDo.SelectedDate.Value.Date);

                long plat = InputValidator.ValidujPlatKontraktuLong(TxtPlat.Text);
                InputValidator.ValidujTelefonniCislo(TxtTelAgenta.Text);
                string telAgenta = TxtTelAgenta.Text.Trim();

                // Pokud je pole prázdné nebo obsahuje pomlčku, klauzule se bere jako nevyplněná
                if (TxtVystupniKlauzule.Text.Trim() == "" || TxtVystupniKlauzule.Text.Trim() == "-")
                {
                    editovanyKontrakt.VystupniKlauzule = 0;
                    editovanyKontrakt.JeVystupniKlauzuleVyplnena = false;
                }
                else
                {
                    // Zvaliduje a načte výši výstupní klauzule
                    long klauzule = InputValidator.ValidujVystupniKlauzuliLong(TxtVystupniKlauzule.Text);

                    editovanyKontrakt.VystupniKlauzule = klauzule;
                    editovanyKontrakt.JeVystupniKlauzuleVyplnena = true;
                }

                editovanyKontrakt.DatumZacatku = datumOd;
                editovanyKontrakt.DatumKonce = datumDo;
                editovanyKontrakt.Plat = plat;
                editovanyKontrakt.TelCisloNaAgenta = telAgenta;

                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeKontrakty.EditujKontrakt(conn, editovanyKontrakt);

                MessageBox.Show("Kontrakt byl úspěšně upraven", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex) when (ex.Message.Contains("ORA-20022") || ex.Message.Contains("ORA-20026"))
            {
                MessageBox.Show("Tento hráč už má kontrakt!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}