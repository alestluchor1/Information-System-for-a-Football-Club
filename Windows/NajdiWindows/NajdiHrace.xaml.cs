using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


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
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            VstupniHraci = hraci;
            NastavFiltryPodleRole();
        }

        /// <summary>
        /// Vrátí roli aktuálně přihlášeného uživatele
        /// </summary>
        private string ZiskejRoliUzivatele()
        {
            Uzivatel uzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (uzivatel == null)
            {
                return "host";
            }

            if (string.IsNullOrWhiteSpace(uzivatel.Role))
            {
                return "host";
            }

            string role = uzivatel.Role.Trim().ToLower();

            if (role == "admin")
            {
                return "admin";
            }

            if (role == "trenér" || role == "trener")
            {
                return "trener";
            }

            if (role == "hráč" || role == "hrac")
            {
                return "hrac";
            }

            return "host";
        }

        /// <summary>
        /// Nastaví viditelnost filtračních polí podle role právě přihlášeného uživatele
        /// </summary>
        private void NastavFiltryPodleRole()
        {
            string role = ZiskejRoliUzivatele();

            LblRodneCislo.Visibility = Visibility.Visible;
            TxtRodneCislo.Visibility = Visibility.Visible;

            LblTelefonniCislo.Visibility = Visibility.Visible;
            TxtTelefonniCislo.Visibility = Visibility.Visible;

            if (role == "trener")
            {
                LblRodneCislo.Visibility = Visibility.Collapsed;
                TxtRodneCislo.Visibility = Visibility.Collapsed;
            }

            if (role == "hrac")
            {
                LblRodneCislo.Visibility = Visibility.Collapsed;
                TxtRodneCislo.Visibility = Visibility.Collapsed;

                LblTelefonniCislo.Visibility = Visibility.Collapsed;
                TxtTelefonniCislo.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Provede filtrování hráčů na základě vyplněných hodnot ve formuláři
        /// </summary>
        private IEnumerable<Hrac> FiltrujHrace()
        {
            IEnumerable<Hrac> vysledek = VstupniHraci;
            string role = ZiskejRoliUzivatele();

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
            if (role == "admin" && !string.IsNullOrWhiteSpace(TxtRodneCislo.Text))
            {
                var rodneCislo = TxtRodneCislo.Text.Trim();
                InputValidator.ValidujRodneCislo(rodneCislo);
                vysledek = vysledek.Where(h => h.RodneCislo == rodneCislo);
            }

            // Telefonní číslo
            if ((role == "admin" || role == "trener") && !string.IsNullOrWhiteSpace(TxtTelefonniCislo.Text))
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

            // Tréninková skupina
            ComboBoxItem vybranaSkupina = cbSkupina.SelectedItem as ComboBoxItem;
            if (vybranaSkupina != null)
            {
                if (vybranaSkupina.Tag != null)
                {
                    int idSkupiny = Convert.ToInt32(vybranaSkupina.Tag);

                    if (idSkupiny != 0)
                    {
                        vysledek = vysledek.Where(h => h.IdTreninkovaSkupina == idSkupiny);
                    }
                }
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
    }
}