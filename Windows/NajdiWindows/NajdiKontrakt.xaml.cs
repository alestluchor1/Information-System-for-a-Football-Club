using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

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
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            VstupniKontrakty = kontrakty;

            if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
            {
                HraciOkno.NactiHrace();
            }

            cbHrac.ItemsSource = HraciOkno.HraciData;
            cbHrac.DisplayMemberPath = "prijmeni";
            cbHrac.SelectedItem = null;

            dpDatumOd.SelectedDate = null;
            dpDatumDo.SelectedDate = null;
        }

        /// <summary>
        /// Obsluha tlačítka "Najdi"
        /// Pokud dojde k chybě ve vstupu (validace), zobrazí se chybové hlášení a okno zůstane otevřené
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

        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Provede filtrování kontraktů na základě vyplněných hodnot ve formuláři
        /// </summary>
        private IEnumerable<Kontrakt> FiltrujKontrakty()
        {
            IEnumerable<Kontrakt> vysledek = VstupniKontrakty;

            // ID hráče
            if (!string.IsNullOrWhiteSpace(TxtIdHrace.Text))
            {
                int idHrace = InputValidator.ValidujId(TxtIdHrace.Text.Trim());
                vysledek = vysledek.Where(k => k.IdHrac == idHrace);
            }

            // Hráč (ComboBox)
            if (cbHrac.SelectedItem != null)
            {
                Hrac vybranyHrac = cbHrac.SelectedItem as Hrac;
                if (vybranyHrac == null)
                {
                    throw new Exception("Vyberte hráče");
                }

                vysledek = vysledek.Where(k => k.IdHrac == vybranyHrac.id);
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

            // Výstupní klauzule 
            if (!string.IsNullOrWhiteSpace(TxtVystupniKlauzule.Text))
            {
                long klauzuleFiltr = InputValidator.ValidujVystupniKlauzuliLong(TxtVystupniKlauzule.Text.Trim());
                vysledek = vysledek.Where(k => k.VystupniKlauzule == klauzuleFiltr);
            }

            return vysledek;
        }

        /// <summary>
        /// Metoda pro přepínání Light Mode / Dark Mode
        /// </summary>
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
                Resources["SidebarGradient"] = new LinearGradientBrush(Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16), 90);
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