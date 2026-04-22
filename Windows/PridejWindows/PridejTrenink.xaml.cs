using InformacniSystemKlub.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového tréninku
    /// Naplní ComboBox trenéry a po uložení provede validaci a uloží
    /// </summary>
    public partial class PridejTrenink : Window
    {
        private Trenink novyTrenink;

        public PridejTrenink(Trenink trenink)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            novyTrenink = trenink;

            if (novyTrenink == null)
            {
                novyTrenink = new Trenink();
            }

            NactiTreneryDoComboBoxu();
            cbTrener.SelectionChanged += cbTrener_SelectionChanged;
            NastavTreninkovouSkupinuPodleRole();
        }

        /// <summary>
        /// Načte trenéry do comboboxu
        /// </summary>
        private void NactiTreneryDoComboBoxu()
        {
            try
            {
                if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
                {
                    TreneriOkno.NactiTreneri();
                }

                cbTrener.ItemsSource = TreneriOkno.TreneriData;
                cbTrener.DisplayMemberPath = "Prijmeni";

                if (cbTrener.Items.Count > 0)
                {
                    cbTrener.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodařilo se načíst trenéry:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Nastaví tréninkovou skupinu podle role přihlášeného uživatele
        /// </summary>
        private void NastavTreninkovouSkupinuPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                NastavTreninkovouSkupinuPodleVybranehoTrenera();
                cbTrener.IsEnabled = true;
                cbTreninkovaSkupina.IsEnabled = false;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "trener" || role == "trenér")
            {
                if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
                {
                    TreneriOkno.NactiTreneri();
                }

                Trener prihlasenyTrener = TreneriOkno.TreneriData.FirstOrDefault(t => t.Id == prihlasenyUzivatel.IdClenKlubu);

                if (prihlasenyTrener != null)
                {
                    cbTrener.SelectedItem = prihlasenyTrener;
                    NastavComboBoxSkupiny(prihlasenyTrener.IdTreninkovaSkupina);
                }
                else
                {
                    NastavTreninkovouSkupinuPodleVybranehoTrenera();
                }

                cbTrener.IsEnabled = false;
                cbTreninkovaSkupina.IsEnabled = false;
            }
            else
            {
                NastavTreninkovouSkupinuPodleVybranehoTrenera();
                cbTrener.IsEnabled = true;
                cbTreninkovaSkupina.IsEnabled = false;
            }
        }

        /// <summary>
        /// Nastaví tréninkovou skupinu podle aktuálně vybraného trenéra
        /// </summary>
        private void NastavTreninkovouSkupinuPodleVybranehoTrenera()
        {
            Trener trenerVybrany = cbTrener.SelectedItem as Trener;

            if (trenerVybrany == null)
            {
                if (cbTreninkovaSkupina.Items.Count > 0)
                {
                    cbTreninkovaSkupina.SelectedIndex = 0;
                }
                return;
            }

            NastavComboBoxSkupiny(trenerVybrany.IdTreninkovaSkupina);
        }

        /// <summary>
        /// Nastaví výběr skupiny v ComboBoxu podle ID skupiny
        /// </summary>
        private void NastavComboBoxSkupiny(int idSkupiny)
        {
            foreach (ComboBoxItem item in cbTreninkovaSkupina.Items)
            {
                if (item.Tag != null && Convert.ToInt32(item.Tag) == idSkupiny)
                {
                    cbTreninkovaSkupina.SelectedItem = item;
                    return;
                }
            }

            if (cbTreninkovaSkupina.Items.Count > 0)
            {
                cbTreninkovaSkupina.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Obsluha změny vybraného trenéra
        /// </summary>
        private void cbTrener_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NastavTreninkovouSkupinuPodleVybranehoTrenera();
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="Trenink"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputValidator.ValidujVyberTrenera(cbTrener.SelectedItem);

                Trener trenerVybrany = cbTrener.SelectedItem as Trener;
                if (trenerVybrany == null)
                {
                    throw new Exception("Vyberte trenéra");
                }

                ComboBoxItem vybranaSkupina = cbTreninkovaSkupina.SelectedItem as ComboBoxItem;
                if (vybranaSkupina == null)
                {
                    throw new Exception("Vyberte tréninkovou skupinu");
                }

                DateTime datum = InputValidator.ValidujDatumTreninku(dpDatum.SelectedDate, "Datum tréninku");
                string misto = InputValidator.ValidujMistoTreninku(TxtMisto.Text, "Místo");

                string popis = TxtPopis.Text;
                if (popis == null)
                {
                    popis = "";
                }
                popis = InputValidator.ValidujPopisTreninku(popis, "Popis");

                novyTrenink.IdTrener = trenerVybrany.Id;
                novyTrenink.Prijmeni = trenerVybrany.Prijmeni;
                novyTrenink.RodneCislo = trenerVybrany.RodneCislo;
                novyTrenink.IdTreninkovaSkupina = trenerVybrany.IdTreninkovaSkupina;
                novyTrenink.NazevSkupiny = trenerVybrany.NazevSkupiny;
                novyTrenink.Datum = datum;
                novyTrenink.Misto = misto;
                novyTrenink.Popis = popis;

                MessageBox.Show("Trénink byl úspěšně přidán", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

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