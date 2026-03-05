using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace InformacniSystemKlub.Windows.NajdiWindows
{
    /// <summary>
    /// Okno pro filtrování hráčů podle zadaných kritérií
    /// </summary>
    public partial class NajdiHrace : Window
    {
        /// <summary>
        /// Výsledná kolekce hráčů po filtrování
        /// Tuto kolekci následně čte okno, které filtr vyvolalo
        /// </summary>
        public ObservableCollection<Hrac> FiltrovaniHraci { get; private set; } = new ObservableCollection<Hrac>();

        /// <summary>
        /// Vstupní kolekce hráčů, ze které se filtruje 
        /// </summary>
        private readonly ObservableCollection<Hrac> VstupniHraci;
        public NajdiHrace(ObservableCollection<Hrac> hraci)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            VstupniHraci = hraci;
        }

        /// <summary>
        /// Provede filtrování hráčů na základě vyplněných hodnot ve formuláři
        /// </summary>
        private IEnumerable<Hrac> FiltrujHrace()
        {
            IEnumerable<Hrac> vysledek = VstupniHraci;

            // ID
            if (!string.IsNullOrWhiteSpace(TxtId.Text))
            {
                int id = InputValidator.ValidujId(TxtId.Text.Trim());
                vysledek = vysledek.Where(h => h.Id == id);
            }

            // Jméno
            if (!string.IsNullOrWhiteSpace(TxtJmeno.Text))
            {
                var jmeno = TxtJmeno.Text.Trim();
                InputValidator.ValidujJmeno(jmeno);
                vysledek = vysledek.Where(h => string.Equals(h.Jmeno, jmeno, StringComparison.OrdinalIgnoreCase));
            }

            // Příjmení
            if (!string.IsNullOrWhiteSpace(TxtPrijmeni.Text))
            {
                var prijmeni = TxtPrijmeni.Text.Trim();
                InputValidator.ValidujPrijmeni(prijmeni);
                vysledek = vysledek.Where(h => string.Equals(h.Prijmeni, prijmeni, StringComparison.OrdinalIgnoreCase));
            }

            // Rodné číslo
            if (!string.IsNullOrWhiteSpace(TxtRodneCislo.Text))
            {
                var rodneCislo = TxtRodneCislo.Text.Trim();
                InputValidator.ValidujRodneCislo(rodneCislo);
                vysledek = vysledek.Where(h => h.RodneCislo == rodneCislo);
            }

            // Telefonní číslo
            if (!string.IsNullOrWhiteSpace(TxtTelefonniCislo.Text))
            {
                var telefonniCislo = TxtTelefonniCislo.Text.Trim();
                InputValidator.ValidujTelefonniCislo(telefonniCislo);
                vysledek = vysledek.Where(h => h.TelefonniCislo == telefonniCislo);
            }

            // Góly
            if (!string.IsNullOrWhiteSpace(iudPocetGolu.Text))
            {
                int goly = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetGolu.Text, "Počet vstřelených gólů");
                vysledek = vysledek.Where(h => h.PocetVstrelenychGolu == goly);
            }

            // Žluté karty
            if (!string.IsNullOrWhiteSpace(iudPocetZlutychKaret.Text))
            {
                int zluteKarty = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetZlutychKaret.Text, "Počet žlutých karet");
                vysledek = vysledek.Where(h => h.PocetZlutychKaret == zluteKarty);
            }

            // Červené karty
            if (!string.IsNullOrWhiteSpace(iudPocetCervenychKaret.Text))
            {
                int cervene = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetCervenychKaret.Text, "Počet červených karet");
                vysledek = vysledek.Where(h => h.PocetCervenychKaret == cervene);
            }

            // Pozice
            if (!string.IsNullOrWhiteSpace(TxtPozice.Text))
            {
                var pozice = TxtPozice.Text.Trim();
                InputValidator.ValidujPozice(pozice);

                vysledek = vysledek.Where(h =>
                    !string.IsNullOrWhiteSpace(h.Pozice) &&
                    h.Pozice.Contains(pozice, StringComparison.OrdinalIgnoreCase));
            }

            // Věk
            if (!string.IsNullOrWhiteSpace(iudVek.Text))
            {
                int vek = InputValidator.ValidujHraciVek(iudVek.Text, "Věk");
                vysledek = vysledek.Where(h => h.Vek == vek);
            }

            // Zdravotní stav (A/N) 
            if (cbZdraviStav.SelectedItem is ComboBoxItem polozka && polozka.Content is string stavText && stavText != "—")
            {
                char stav = stavText[0];
                vysledek = vysledek.Where(h => h.ZdravotniStav == stav);
            }

            return vysledek;
        }

        /// <summary>
        /// Obsluha tlačítka "Najdi"
        /// Pokud dojde k chybě ve vstupu (validace), zobrazí se chybové hlášení a okno zůstane otevřené
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniHraci = new ObservableCollection<Hrac>(FiltrujHrace());
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

                Resources["SidebarGradient"] = new LinearGradientBrush(
                    Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16),
                    90);
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
