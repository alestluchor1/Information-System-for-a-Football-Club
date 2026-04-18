using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.NajdiWindows;
using InformacniSystemKlub.Windows.PridejWindows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Interakční logika pro TreninkOkno.xaml
    /// </summary>
    public partial class TreninkOkno : Window
    {
        /// <summary>
        /// Kolekce tréninků používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<Trenink> TreninkyData { get; set; } = new ObservableCollection<Trenink>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        /// <summary>
        /// Příznak, zda je inicializace okna dokončena
        /// </summary>
        private bool inicializaceDokoncena = false;

        public TreninkOkno()
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimTabulkoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            inicializaceDokoncena = false;

            NactiTreninky();

            DgTreninky.ItemsSource = TreninkyData;

            NastavRezimVyberuTreninkuPodleRole();
            AplikujFiltrTreninku();
            NastavPravaPodleRole();

            inicializaceDokoncena = true;
        }

        /// <summary>
        /// Načte tréninky z databáze a naplní kolekci TreninkyData
        /// </summary>
        public static void NactiTreninky()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                var cmd = new OracleCommand("SELECT * FROM TRENINKY_VIEW", conn);
                var reader = cmd.ExecuteReader();

                TreninkyData.Clear();

                while (reader.Read())
                {
                    Trenink trenink = new Trenink();

                    if (reader["IDTRENINK"] != DBNull.Value)
                    {
                        trenink.Id = Convert.ToInt32(reader["IDTRENINK"]);
                    }
                    else
                    {
                        trenink.Id = 0;
                    }

                    // ID trenéra, ke kterému trénink patří
                    if (reader["IDCLENKLUBU"] != DBNull.Value)
                    {
                        trenink.IdTrener = Convert.ToInt32(reader["IDCLENKLUBU"]);
                    }
                    else
                    {
                        trenink.IdTrener = 0;
                    }

                    // Rodné číslo trenéra
                    if (reader["RODNECISLO"] != DBNull.Value)
                    {
                        trenink.RodneCislo = reader["RODNECISLO"].ToString();
                    }
                    else
                    {
                        trenink.RodneCislo = "0";
                    }

                    // Příjmení trenéra
                    if (reader["PRIJMENI"] != DBNull.Value)
                    {
                        trenink.Prijmeni = reader["PRIJMENI"].ToString();
                    }
                    else
                    {
                        trenink.Prijmeni = "-";
                    }

                    // Popis tréninku
                    if (reader["POPIS"] != DBNull.Value)
                    {
                        trenink.Popis = reader["POPIS"].ToString();
                    }
                    else
                    {
                        trenink.Popis = "Volitelné nezadáno !";
                    }

                    // Místo konání tréninku
                    if (reader["MISTO"] != DBNull.Value)
                    {
                        trenink.Misto = reader["MISTO"].ToString();
                    }
                    else
                    {
                        trenink.Misto = "";
                    }

                    // Datum tréninku
                    if (reader["DATUM"] != DBNull.Value)
                    {
                        trenink.Datum = Convert.ToDateTime(reader["DATUM"]);
                    }
                    else
                    {
                        trenink.Datum = DateTime.MinValue;
                    }

                    TreninkyData.Add(trenink);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání tréninků: {ex.Message}");
            }
        }

        /// <summary>
        /// Nastaví výchozí režim ComboBoxu podle role přihlášeného uživatele
        /// </summary>
        private void NastavRezimVyberuTreninkuPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                cbVyberTreninku.SelectedIndex = 0;
                cbVyberTreninku.IsEnabled = false;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "admin")
            {
                cbVyberTreninku.SelectedIndex = 0;
                cbVyberTreninku.IsEnabled = false;
                return;
            }

            if (role == "trener" || role == "trenér")
            {
                cbVyberTreninku.SelectedIndex = 1;
                cbVyberTreninku.IsEnabled = true;
                return;
            }

            cbVyberTreninku.SelectedIndex = 0;
            cbVyberTreninku.IsEnabled = false;
        }

        /// <summary>
        /// Zkontroluje roli přihlášeného uživatele a podle ní povolí nebo zakáže tlačítka
        /// </summary>
        private void NastavPravaPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            // Pokud není nikdo přihlášen, přidávání/editace/mazání nejsou povoleny
            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            // Hráč má pouze prohlížení a vyhledávání
            if (role == "hrac" || role == "hráč")
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                return;
            }

            // Host nemá upravovací práva
            if (role == "host")
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                return;
            }

            // Trenér může přidávat vždy, ale editace a mazání závisí na vybraném tréninku
            if (role == "trener" || role == "trenér")
            {
                BtnPridat.IsEnabled = true;
                BtnNajdi.IsEnabled = true;

                Trenink vybranyTrenink = DgTreninky.SelectedItem as Trenink;

                if (vybranyTrenink == null)
                {
                    BtnEdituj.IsEnabled = false;
                    BtnOdebrat.IsEnabled = false;
                }
                else if (PatriTreninkPrihlasenemuTrenerovi(vybranyTrenink) == true)
                {
                    BtnEdituj.IsEnabled = true;
                    BtnOdebrat.IsEnabled = true;
                }
                else
                {
                    BtnEdituj.IsEnabled = false;
                    BtnOdebrat.IsEnabled = false;
                }

                return;
            }

            // Admin a ostatní vyšší role mají plný přístup
            BtnPridat.IsEnabled = true;
            BtnOdebrat.IsEnabled = true;
            BtnEdituj.IsEnabled = true;
            BtnNajdi.IsEnabled = true;
        }

        /// <summary>
        /// Zjistí, zda je aktuálně přihlášený uživatel trenér
        /// </summary>
        private bool JePrihlasenTrener()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                return false;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();
            return role == "trener" || role == "trenér";
        }

        /// <summary>
        /// Zjistí, zda vybraný trénink patří právě přihlášenému trenérovi
        /// </summary>
        private bool PatriTreninkPrihlasenemuTrenerovi(Trenink trenink)
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null || trenink == null)
            {
                return false;
            }

            return trenink.IdTrener == prihlasenyUzivatel.IdClenKlubu;
        }

        /// <summary>
        /// Aplikuje filtr tréninků podle vybrané položky v ComboBoxu
        /// </summary>
        private void AplikujFiltrTreninku()
        {
            // Pokud je aktivní vyhledávací mód, filtr přes ComboBox se dočasně neaplikuje
            if (jeVyhledavaniAktivni == true)
            {
                return;
            }

            if (cbVyberTreninku.SelectedIndex == 0)
            {
                DgTreninky.ItemsSource = TreninkyData;
                DgTreninky.SelectedItem = null;
                NastavPravaPodleRole();
                return;
            }

            if (cbVyberTreninku.SelectedIndex == 1 && JePrihlasenTrener() == true)
            {
                ObservableCollection<Trenink> vlastniTreninky = new ObservableCollection<Trenink>();

                foreach (Trenink trenink in TreninkyData)
                {
                    if (PatriTreninkPrihlasenemuTrenerovi(trenink) == true)
                    {
                        vlastniTreninky.Add(trenink);
                    }
                }

                DgTreninky.ItemsSource = vlastniTreninky;
                DgTreninky.SelectedItem = null;
                NastavPravaPodleRole();
                return;
            }

            DgTreninky.ItemsSource = TreninkyData;
            DgTreninky.SelectedItem = null;
            NastavPravaPodleRole();
        }

        /// <summary>
        /// Obsluha změny výběru režimu zobrazení tréninků
        /// </summary>
        private void cbVyberTreninku_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inicializaceDokoncena == false)
            {
                return;
            }

            AplikujFiltrTreninku();
        }

        /// <summary>
        /// Obsluha změny výběru v DataGridu
        /// Podle označeného tréninku znovu nastaví povolení tlačítek
        /// </summary>
        private void DgTreninky_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inicializaceDokoncena == false)
            {
                return;
            }

            NastavPravaPodleRole();
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů tréninku
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Trenink vybranyTrenink = DgTreninky.SelectedItem as Trenink;
            if (vybranyTrenink == null)
            {
                MessageBox.Show("Prosím vyberte trénink, který chcete editovat!", "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Pokud je přihlášen trenér, může editovat pouze své vlastní tréninky
            if (JePrihlasenTrener() == true && PatriTreninkPrihlasenemuTrenerovi(vybranyTrenink) == false)
            {
                MessageBox.Show("Můžete editovat pouze své vlastní tréninky!", "Přístup odepřen",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EditujTrenink(vybranyTrenink);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                NactiTreninky();
                AplikujFiltrTreninku();
            }
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání tréninků
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            NajdiTrenink najdiTrenink = new NajdiTrenink(TreninkyData);
            bool? vysledekDialogu = najdiTrenink.ShowDialog();

            if (vysledekDialogu == true)
            {
                if (najdiTrenink.FiltrovaniTreninky == null || najdiTrenink.FiltrovaniTreninky.Count == 0)
                {
                    MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(
                    "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
                    "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                DgTreninky.ItemsSource = new ObservableCollection<Trenink>(najdiTrenink.FiltrovaniTreninky);
                DgTreninky.SelectedItem = null;
                jeVyhledavaniAktivni = true;
                NastavPravaPodleRole();
            }
        }

        /// <summary>
        /// Metoda slouží k zrušení vyhledávacího módu, pokud se zmáčkne klávesa CTRL + X
        /// </summary>
        private void WindowUkonceniFiltrovani(object sender, KeyEventArgs e)
        {
            if (jeVyhledavaniAktivni && (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X))
            {
                jeVyhledavaniAktivni = false;
                AplikujFiltrTreninku();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Otevře dialog pro přidání tréninku, uloží nový trénink do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Trenink pridanyTrenink = new Trenink();

            // Pokud je přihlášen trenér, nový trénink se automaticky přiřadí jemu
            if (JePrihlasenTrener() == true)
            {
                Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();
                pridanyTrenink.IdTrener = prihlasenyUzivatel.IdClenKlubu;
            }

            var okno = new PridejTrenink(pridanyTrenink);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                // Kontrola, že trenér nevkládá trénink pod jiným trenérem
                if (JePrihlasenTrener() == true)
                {
                    Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

                    if (pridanyTrenink.IdTrener != prihlasenyUzivatel.IdClenKlubu)
                    {
                        MessageBox.Show("Můžete přidávat pouze své vlastní tréninky!", "Přístup odepřen",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreninky.PridejTrenink(conn, pridanyTrenink);

                NactiTreninky();
                AplikujFiltrTreninku();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidání tréninku: {ex.Message}");
            }
        }

        /// <summary>
        /// Smaže vybraný trénink z databáze i z lokální kolekce a aktualizuje zobrazení podle aktuálního filtru
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Trenink vybranyTrenink = (Trenink)DgTreninky.SelectedItem;

            if (vybranyTrenink == null)
            {
                MessageBox.Show("Vyberte trénink !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Pokud je přihlášen trenér, může mazat pouze své vlastní tréninky
            if (JePrihlasenTrener() == true && PatriTreninkPrihlasenemuTrenerovi(vybranyTrenink) == false)
            {
                MessageBox.Show("Můžete mazat pouze své vlastní tréninky!", "Přístup odepřen",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Opravdu chcete smazat vybraný trénink?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeTreninky.OdeberTrenink(conn, vybranyTrenink);

                TreninkyData.Remove(vybranyTrenink);

                AplikujFiltrTreninku();

                MessageBox.Show("Trénink byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání tréninku: {ex.Message}");
            }
        }

        /// <summary>
        /// Zavře okno a vrátí uživatele na předchozí obrazovku (Hlavní okno)
        /// </summary>
        private void BtnZpet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}