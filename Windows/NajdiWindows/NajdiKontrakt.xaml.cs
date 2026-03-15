using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace InformacniSystemKlub.Windows.NajdiWindows
{
    /// <summary>
    /// Okno pro filtrování kontraktů podle zadaných kritérií
    /// </summary>
    public partial class NajdiKontrakt : Window
    {
        /// <summary>
        /// Kolekce kontraktů po filtrování
        /// </summary>
        public ObservableCollection<Kontrakt> FiltrovaniKontrakty { get; private set; } = new ObservableCollection<Kontrakt>();

        /// <summary>
        /// Kolekce kontraktů, ze které se filtruje
        /// </summary>
        private ObservableCollection<Kontrakt> VstupniKontrakty;

        public NajdiKontrakt(ObservableCollection<Kontrakt> kontrakty)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            VstupniKontrakty = kontrakty;

            if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
            {
                HraciOkno.NactiHrace();
            }

            cbHrac.ItemsSource = HraciOkno.HraciData;
            cbHrac.DisplayMemberPath = "Prijmeni";
            cbHrac.SelectedItem = null;

            dpDatumOd.SelectedDate = null;
            dpDatumDo.SelectedDate = null;

            TxtVystupniKlauzule.IsEnabled = false;
        }

        /// <summary>
        /// Po zaškrtnutí možnosti "Má výstupní klauzuli" se zakáže druhý checkbox
        /// a povolí se zadání konkrétní hodnoty klauzule
        /// </summary>
        private void ChbMaVystupniKlauzuli_Checked(object sender, RoutedEventArgs e)
        {
            ChbNemaVystupniKlauzuli.IsEnabled = false;
            TxtVystupniKlauzule.IsEnabled = true;
        }

        /// <summary>
        /// Po odškrtnutí možnosti "Má výstupní klauzuli" se znovu povolí druhý checkbox
        /// Pokud není aktivní filtr na neexistující klauzuli, zakáže se i textbox
        /// </summary>
        private void ChbMaVystupniKlauzuli_Unchecked(object sender, RoutedEventArgs e)
        {
            ChbNemaVystupniKlauzuli.IsEnabled = true;

            if (ChbNemaVystupniKlauzuli.IsChecked != true)
            {
                TxtVystupniKlauzule.Text = "";
                TxtVystupniKlauzule.IsEnabled = false;
            }
        }

        /// <summary>
        /// Po zaškrtnutí možnosti "Nemá výstupní klauzuli" se zakáže druhý checkbox
        /// a zároveň se zakáže textbox pro zadání hodnoty klauzule
        /// </summary>
        private void ChbNemaVystupniKlauzuli_Checked(object sender, RoutedEventArgs e)
        {
            ChbMaVystupniKlauzuli.IsEnabled = false;
            TxtVystupniKlauzule.Text = "";
            TxtVystupniKlauzule.IsEnabled = false;
        }

        /// <summary>
        /// Po odškrtnutí možnosti "Nemá výstupní klauzuli" se znovu povolí druhý checkbox
        /// </summary>
        private void ChbNemaVystupniKlauzuli_Unchecked(object sender, RoutedEventArgs e)
        {
            ChbMaVystupniKlauzuli.IsEnabled = true;

            if (ChbMaVystupniKlauzuli.IsChecked == true)
            {
                TxtVystupniKlauzule.IsEnabled = true;
            }
            else
            {
                TxtVystupniKlauzule.Text = "";
                TxtVystupniKlauzule.IsEnabled = false;
            }
        }

        /// <summary>
        /// Obsluha tlačítka Najdi
        /// Pokud dojde k chybě ve vstupu, zobrazí se chybové hlášení a okno zůstane otevřené
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniKontrakty = new ObservableCollection<Kontrakt>(FiltrujKontrakty());
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Zavře okno filtrování bez potvrzení změn
        /// a nastaví výsledek dialogu na hodnotu false
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Provede filtrování kontraktů na základě vyplněných hodnot ve formuláři.
        /// </summary>
        private IEnumerable<Kontrakt> FiltrujKontrakty()
        {
            IEnumerable<Kontrakt> vysledek = VstupniKontrakty;

            // Hráč
            if (cbHrac.SelectedItem != null)
            {
                Hrac vybranyHrac = cbHrac.SelectedItem as Hrac;
                if (vybranyHrac == null)
                {
                    throw new Exception("Vyberte hráče");
                }

                vysledek = vysledek.Where(k => k.IdHrac == vybranyHrac.Id);
            }

            // Datum začátku
            if (dpDatumOd.SelectedDate != null)
            {
                DateOnly datumZacatkuFiltr = DateOnly.FromDateTime(dpDatumOd.SelectedDate.Value.Date);
                vysledek = vysledek.Where(k => k.DatumZacatku == datumZacatkuFiltr);
            }

            // Datum konce
            if (dpDatumDo.SelectedDate != null)
            {
                DateOnly datumKonceFiltr = DateOnly.FromDateTime(dpDatumDo.SelectedDate.Value.Date);
                vysledek = vysledek.Where(k => k.DatumKonce == datumKonceFiltr);
            }

            // Plat
            if (!string.IsNullOrWhiteSpace(TxtPlat.Text))
            {
                long platFiltr = InputValidator.ValidujPlatKontraktuLong(TxtPlat.Text.Trim());
                vysledek = vysledek.Where(k => k.Plat == platFiltr);
            }

            // Tel. číslo agenta
            if (!string.IsNullOrWhiteSpace(TxtTelAgenta.Text))
            {
                string telAgentaFiltr = TxtTelAgenta.Text.Trim();
                InputValidator.ValidujTelefonniCislo(telAgentaFiltr);

                vysledek = vysledek.Where(k =>
                {
                    if (k.TelCisloNaAgenta == null)
                    {
                        return false;
                    }

                    return string.Equals(k.TelCisloNaAgenta, telAgentaFiltr, StringComparison.OrdinalIgnoreCase);
                });
            }

            // Pokud je zaškrtnuto, že kontrakt má výstupní klauzuli, filtruje se jen na vyplněné klauzule
            if (ChbMaVystupniKlauzuli.IsChecked == true)
            {
                vysledek = vysledek.Where(k => k.JeVystupniKlauzuleVyplnena == true);

                // Pokud uživatel zadal konkrétní hodnotu klauzule, filtruje se i podle ní
                if (!string.IsNullOrWhiteSpace(TxtVystupniKlauzule.Text))
                {
                    long klauzuleFiltr = InputValidator.ValidujVystupniKlauzuliLong(TxtVystupniKlauzule.Text.Trim());
                    vysledek = vysledek.Where(k => k.VystupniKlauzule == klauzuleFiltr);
                }
            }

            // Pokud je zaškrtnuto, že kontrakt nemá výstupní klauzuli, hledají se jen nevyplněné klauzule
            if (ChbNemaVystupniKlauzuli.IsChecked == true)
            {
                vysledek = vysledek.Where(k => k.JeVystupniKlauzuleVyplnena == false);
            }

            return vysledek;
        }
    }
}