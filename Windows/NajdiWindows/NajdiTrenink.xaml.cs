using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InformacniSystemKlub.Windows.NajdiWindows
{
    /// <summary>
    /// Okno pro filtrování tréninků podle zadaných kritérií
    /// </summary>
    public partial class NajdiTrenink : Window
    {
        /// <summary>
        /// Kolekce tréninků po filtrování
        /// </summary>
        public ObservableCollection<Trenink> FiltrovaniTreninky { get; private set; } = new ObservableCollection<Trenink>();

        /// <summary>
        /// Kolekce tréninků, ze které se filtruje
        /// </summary>
        private ObservableCollection<Trenink> VstupniTreninky;

        public NajdiTrenink(ObservableCollection<Trenink> treninky)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            VstupniTreninky = treninky;

            if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
            {
                TreneriOkno.NactiTreneri();
            }

            cbTrener.ItemsSource = TreneriOkno.TreneriData;
            cbTrener.DisplayMemberPath = "Prijmeni";
            cbTrener.SelectedItem = null;

            cbTreninkovaSkupina.SelectedIndex = 0;
            dpDatum.SelectedDate = null;

            NastavTreninkovouSkupinuPodleRole();
        }

        /// <summary>
        /// Nastaví combobox tréninkové skupiny podle role přihlášeného uživatele
        /// </summary>
        private void NastavTreninkovouSkupinuPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                cbTreninkovaSkupina.SelectedIndex = 0;
                cbTreninkovaSkupina.IsEnabled = true;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "hrac" || role == "hráč")
            {
                int idSkupiny = ZiskejIdTreninkoveSkupinyPrihlasenehoHrace();

                if (idSkupiny == 1)
                {
                    VyberSkupinuVComboBoxu("A");
                    cbTreninkovaSkupina.IsEnabled = false;
                    return;
                }

                if (idSkupiny == 2)
                {
                    VyberSkupinuVComboBoxu("Dorost");
                    cbTreninkovaSkupina.IsEnabled = false;
                    return;
                }

                if (idSkupiny == 3)
                {
                    VyberSkupinuVComboBoxu("Mladší žáci");
                    cbTreninkovaSkupina.IsEnabled = false;
                    return;
                }

                cbTreninkovaSkupina.SelectedIndex = 0;
                cbTreninkovaSkupina.IsEnabled = false;
                return;
            }

            cbTreninkovaSkupina.SelectedIndex = 0;
            cbTreninkovaSkupina.IsEnabled = true;
        }

        /// <summary>
        /// Vybere položku skupiny v ComboBoxu podle textu
        /// </summary>
        private void VyberSkupinuVComboBoxu(string nazevSkupiny)
        {
            foreach (object item in cbTreninkovaSkupina.Items)
            {
                ComboBoxItem polozka = item as ComboBoxItem;

                if (polozka == null || polozka.Content == null)
                {
                    continue;
                }

                string text = polozka.Content.ToString();

                if (string.Equals(text.Trim(), nazevSkupiny.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    cbTreninkovaSkupina.SelectedItem = polozka;
                    return;
                }
            }

            cbTreninkovaSkupina.SelectedIndex = 0;
        }

        /// <summary>
        /// Zjistí ID tréninkové skupiny právě přihlášeného hráče
        /// Nejprve se pokusí vzít skupinu přímo z přihlášeného uživatele
        /// Když skupinu nelze zjistit, vrací 0
        /// </summary>
        private int ZiskejIdTreninkoveSkupinyPrihlasenehoHrace()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null)
            {
                return 0;
            }

            if (prihlasenyUzivatel.IdTreninkovaSkupina > 0)
            {
                return prihlasenyUzivatel.IdTreninkovaSkupina;
            }

            if (prihlasenyUzivatel.IdClenKlubu <= 0)
            {
                return 0;
            }

            if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
            {
                HraciOkno.NactiHrace();
            }

            foreach (Hrac hrac in HraciOkno.HraciData)
            {
                if (hrac.Id == prihlasenyUzivatel.IdClenKlubu)
                {
                    return hrac.IdTreninkovaSkupina;
                }
            }

            return 0;
        }

        /// <summary>
        /// Obsluha tlačítka pro spuštění filtrování
        /// Provede filtrování tréninků podle zadaných hodnot a uloží výsledek do vlastnosti <see cref="FiltrovaniTreninky"/>
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniTreninky = new ObservableCollection<Trenink>(FiltrujTreninky());
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Obsluha tlačítka Zrušit
        /// Zavře okno bez provedení filtrování a vrátí záporný výsledek dialogu
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Provede filtrování tréninků na základě aktuálně vyplněných hodnot ve formuláři
        /// Zadána jsou pouze ta kritéria, která uživatel skutečně vyplnil
        /// </summary>
        /// <returns>
        /// Kolekci tréninků splňujících všechna zadaná kritéria
        /// </returns>
        private IEnumerable<Trenink> FiltrujTreninky()
        {
            IEnumerable<Trenink> vysledek = VstupniTreninky;

            // ID tréninku
            if (!string.IsNullOrWhiteSpace(TxtIdTrenink.Text))
            {
                int id = InputValidator.ValidujId(TxtIdTrenink.Text.Trim());
                vysledek = vysledek.Where(t => t.Id == id);
            }

            // Trenér
            if (cbTrener.SelectedItem != null)
            {
                Trener vybranyTrener = cbTrener.SelectedItem as Trener;
                if (vybranyTrener == null)
                {
                    throw new Exception("Vyberte trenéra");
                }

                vysledek = vysledek.Where(t => t.IdTrener == vybranyTrener.Id);
            }

            // Tréninková skupina
            ComboBoxItem vybranaSkupinaPolozka = cbTreninkovaSkupina.SelectedItem as ComboBoxItem;
            if (vybranaSkupinaPolozka != null && vybranaSkupinaPolozka.Content != null)
            {
                string vybranaSkupina = vybranaSkupinaPolozka.Content.ToString();

                if (!string.Equals(vybranaSkupina, "Všechny skupiny", StringComparison.OrdinalIgnoreCase))
                {
                    vysledek = vysledek.Where(t =>
                        !string.IsNullOrWhiteSpace(t.NazevSkupiny) &&
                        string.Equals(NormalizujNazevSkupiny(t.NazevSkupiny), vybranaSkupina.Trim(), StringComparison.OrdinalIgnoreCase));
                }
            }

            // Datum
            if (dpDatum.SelectedDate != null)
            {
                DateTime datum = dpDatum.SelectedDate.Value.Date;
                vysledek = vysledek.Where(t => t.Datum.Date == datum);
            }

            // Místo
            if (!string.IsNullOrWhiteSpace(TxtMisto.Text))
            {
                string misto = TxtMisto.Text.Trim();
                InputValidator.ValidujMistoTreninku(misto, "Místo");

                vysledek = vysledek.Where(t =>
                    string.Equals(t.Misto, misto, StringComparison.OrdinalIgnoreCase));
            }

            // Popis
            if (!string.IsNullOrWhiteSpace(TxtPopis.Text))
            {
                string popis = TxtPopis.Text.Trim();
                InputValidator.ValidujPopisTreninku(popis, "Popis");

                vysledek = vysledek.Where(t =>
                {
                    if (t.Popis == null)
                    {
                        return false;
                    }

                    return t.Popis.IndexOf(popis, StringComparison.OrdinalIgnoreCase) >= 0;
                });
            }

            return vysledek;
        }

        /// <summary>
        /// Upraví název skupiny do jednotného tvaru, aby šly správně porovnávat
        /// hodnoty z databáze a z ComboBoxu.
        /// Například zápisy "A-Tým" a "A tým" převede na "A"
        /// </summary>
        private string NormalizujNazevSkupiny(string nazevSkupiny)
        {
            if (string.IsNullOrWhiteSpace(nazevSkupiny))
            {
                return "";
            }

            string vysledek = nazevSkupiny.Trim();

            if (string.Equals(vysledek, "A-Tým", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(vysledek, "A tým", StringComparison.OrdinalIgnoreCase))
            {
                return "A";
            }

            return vysledek;
        }
    }
}