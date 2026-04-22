using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného tréninku
    /// Při otevření se formulář předvyplní a po potvrzení se změny uloží do databáze
    /// </summary>
    public partial class EditujTrenink : Window
    {
        private readonly Trenink editovanyTrenink;
        private const int MaxPopis = 50;
        private bool inicializaceDokoncena = false;

        public EditujTrenink(Trenink trenink)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            if (trenink == null)
            {
                throw new ArgumentNullException(nameof(trenink));
            }

            editovanyTrenink = trenink;

            TxtPopis.MaxLength = MaxPopis;

            inicializaceDokoncena = false;

            NactiTreneryDoComboBoxu();
            PredvyplnFormular();
            NastavPrvkyPodleRole();

            cbTrener.SelectionChanged += cbTrener_SelectionChanged;

            inicializaceDokoncena = true;
        }

        /// <summary>
        /// Načte trenéry do ComboBoxu
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodařilo se načíst trenéry:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Předvyplní formulář daty editovaného tréninku
        /// </summary>
        private void PredvyplnFormular()
        {
            foreach (Trener trener in TreneriOkno.TreneriData)
            {
                if (trener.Id == editovanyTrenink.IdTrener)
                {
                    cbTrener.SelectedItem = trener;
                    break;
                }
            }

            NastavComboBoxSkupiny(editovanyTrenink.IdTreninkovaSkupina);

            dpDatum.SelectedDate = editovanyTrenink.Datum;
            TxtMisto.Text = editovanyTrenink.Misto;
            TxtPopis.Text = editovanyTrenink.Popis;
        }

        /// <summary>
        /// Nastaví ComboBox skupiny podle ID skupiny
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
        /// Nastaví, které ovládací prvky formuláře může uživatel měnit podle své role.
        /// Admin může měnit trenéra, trenér má trenéra automaticky nastaveného na sebe
        /// a ComboBox tréninkové skupiny je vždy uzamčený, protože se vyplňuje automaticky
        /// podle právě vybraného trenéra
        /// </summary>
        private void NastavPrvkyPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            cbTreninkovaSkupina.IsEnabled = false;

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                cbTrener.IsEnabled = false;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "admin")
            {
                cbTrener.IsEnabled = true;
                return;
            }

            if (role == "trener" || role == "trenér")
            {
                Trener prihlasenyTrener = TreneriOkno.TreneriData
                    .FirstOrDefault(t => t.Id == prihlasenyUzivatel.IdClenKlubu);

                if (prihlasenyTrener != null)
                {
                    cbTrener.SelectedItem = prihlasenyTrener;
                    NastavComboBoxSkupiny(prihlasenyTrener.IdTreninkovaSkupina);
                }

                cbTrener.IsEnabled = false;
                return;
            }

            cbTrener.IsEnabled = false;
        }

        /// <summary>
        /// Reaguje na změnu vybraného trenéra v ComboBoxu.
        /// Pokud uživatel vybere jiného trenéra, automaticky se v uzamčeném
        /// ComboBoxu nastaví jeho tréninková skupina, aby skupina vždy odpovídala
        /// právě zvolenému trenérovi.
        /// </summary>
        private void cbTrener_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inicializaceDokoncena == false)
            {
                return;
            }

            Trener trenerVybrany = cbTrener.SelectedItem as Trener;

            if (trenerVybrany == null)
            {
                return;
            }

            NastavComboBoxSkupiny(trenerVybrany.IdTreninkovaSkupina);
        }

        /// <summary>
        /// Uloží změněné údaje tréninku
        /// Nejprve ověří oprávnění přihlášeného uživatele, zkontroluje zadané hodnoty
        /// uloží nové údaje do objektu tréninku a následně provede update do databáze
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

                if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
                {
                    throw new Exception("Nemáte oprávnění upravovat trénink");
                }

                string role = prihlasenyUzivatel.Role.Trim().ToLower();

                InputValidator.ValidujVyberTrenera(cbTrener.SelectedItem);

                Trener trenerVybrany = cbTrener.SelectedItem as Trener;
                if (trenerVybrany == null)
                {
                    throw new Exception("Vyberte trenéra");
                }

                if (role == "trener" || role == "trenér")
                {
                    if (trenerVybrany.Id != prihlasenyUzivatel.IdClenKlubu)
                    {
                        throw new Exception("Můžete upravovat pouze své vlastní tréninky");
                    }
                }

                DateTime datum = InputValidator.ValidujDatumTreninku(dpDatum.SelectedDate, "Datum tréninku");
                string misto = InputValidator.ValidujMistoTreninku(TxtMisto.Text, "Místo");

                string popis = TxtPopis.Text;

                if (popis == null)
                {
                    popis = "";
                }

                popis = InputValidator.ValidujPopisTreninku(popis, "Popis");

                editovanyTrenink.IdTrener = trenerVybrany.Id;
                editovanyTrenink.Prijmeni = trenerVybrany.Prijmeni;
                editovanyTrenink.RodneCislo = trenerVybrany.RodneCislo;
                editovanyTrenink.IdTreninkovaSkupina = trenerVybrany.IdTreninkovaSkupina;
                editovanyTrenink.NazevSkupiny = trenerVybrany.NazevSkupiny;
                editovanyTrenink.Datum = datum;
                editovanyTrenink.Misto = misto;
                editovanyTrenink.Popis = popis;

                OracleConnection conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreninky.EditujTrenink(conn, editovanyTrenink);

                MessageBox.Show("Trénink byl úspěšně upraven", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci tréninku:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}