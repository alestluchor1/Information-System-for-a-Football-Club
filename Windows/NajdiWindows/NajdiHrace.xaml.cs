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
            VstupniHraci = hraci;
        }

        /// <summary>
        /// Provede filtrování hráčů na základě vyplněných hodnot ve formuláři
        /// </summary>
        private IEnumerable<Hrac> FiltrujHrace()
        {
            IEnumerable<Hrac> vysledek = VstupniHraci;

            if (!string.IsNullOrWhiteSpace(TxtId.Text))
            {
                int id = InputValidator.ValidujId(TxtId.Text.Trim());
                vysledek = vysledek.Where(h => h.id == id);
            }

            if (!string.IsNullOrWhiteSpace(TxtJmeno.Text))
            {
                var jmeno = TxtJmeno.Text.Trim();
                InputValidator.ValidujJmeno(jmeno);
                vysledek = vysledek.Where(h => string.Equals(h.jmeno, jmeno, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(TxtPrijmeni.Text))
            {
                var prijmeni = TxtPrijmeni.Text.Trim();
                InputValidator.ValidujPrijmeni(prijmeni);
                vysledek = vysledek.Where(h => string.Equals(h.prijmeni, prijmeni, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(TxtRodneCislo.Text))
            {
                var rodneCislo = TxtRodneCislo.Text.Trim();
                InputValidator.ValidujRodneCislo(rodneCislo);
                vysledek = vysledek.Where(h => h.rodneCislo == rodneCislo);
            }

            if (!string.IsNullOrWhiteSpace(TxtTelefonniCislo.Text))
            {
                var telefonniCislo = TxtTelefonniCislo.Text.Trim();
                InputValidator.ValidujTelefonniCislo(telefonniCislo);
                vysledek = vysledek.Where(h => h.telefonniCislo == telefonniCislo);
            }

            // Góly
            if (!string.IsNullOrWhiteSpace(iudPocetGolu.Text))
            {
                int goly = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetGolu.Text, "Počet vstřelených gólů");
                vysledek = vysledek.Where(h => h.pocetVstrelenychGolu == goly);
            }

            // Žluté karty
            if (!string.IsNullOrWhiteSpace(iudPocetZlutychKaret.Text))
            {
                int zluteKarty = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetZlutychKaret.Text, "Počet žlutých karet");
                vysledek = vysledek.Where(h => h.pocetZlutychKaret == zluteKarty);
            }

            // Červené karty
            if (!string.IsNullOrWhiteSpace(iudPocetCervenychKaret.Text))
            {
                int cervene = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetCervenychKaret.Text, "Počet červených karet");
                vysledek = vysledek.Where(h => h.pocetCervenychKaret == cervene);
            }

            // Pozice
            if (!string.IsNullOrWhiteSpace(TxtPozice.Text))
            {
                var pozice = TxtPozice.Text.Trim();
                InputValidator.ValidujPozice(pozice);

                vysledek = vysledek.Where(h =>
                    !string.IsNullOrWhiteSpace(h.pozice) &&
                    h.pozice.Contains(pozice, StringComparison.OrdinalIgnoreCase));
            }

            // Věk
            if (!string.IsNullOrWhiteSpace(iudVek.Text))
            {
                int vek = InputValidator.ValidujHraciVek(iudVek.Text, "Věk");
                vysledek = vysledek.Where(h => h.vek == vek);
            }

            // Zdravotní stav (A/N) – nechávám tvoje, protože pracuje s "—"
            if (cbZdraviStav.SelectedItem is ComboBoxItem polozka && polozka.Content is string stavText && stavText != "—")
            {
                char stav = stavText[0];
                vysledek = vysledek.Where(h => h.zdravotniStav == stav);
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
                MessageBox.Show(ex.Message, "Chyba ve vstupu", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
