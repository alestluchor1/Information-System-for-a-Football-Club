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

            NastavViditelnostSloupcuPodleRole();
            NastavViditelnostComboBoxuPodleRole();
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

                    if (reader["IDCLENKLUBU"] != DBNull.Value)
                    {
                        trenink.IdTrener = Convert.ToInt32(reader["IDCLENKLUBU"]);
                    }
                    else
                    {
                        trenink.IdTrener = 0;
                    }

                    if (reader["RODNECISLO"] != DBNull.Value)
                    {
                        trenink.RodneCislo = reader["RODNECISLO"].ToString();
                    }
                    else
                    {
                        trenink.RodneCislo = "0";
                    }

                    if (reader["PRIJMENI"] != DBNull.Value)
                    {
                        trenink.Prijmeni = reader["PRIJMENI"].ToString();
                    }
                    else
                    {
                        trenink.Prijmeni = "-";
                    }

                    if (reader["IDTRENINKOVASKUPINA"] != DBNull.Value)
                    {
                        trenink.IdTreninkovaSkupina = Convert.ToInt32(reader["IDTRENINKOVASKUPINA"]);
                    }
                    else
                    {
                        trenink.IdTreninkovaSkupina = 0;
                    }

                    if (reader["NAZEVSKUPINY"] != DBNull.Value)
                    {
                        trenink.NazevSkupiny = reader["NAZEVSKUPINY"].ToString();
                    }
                    else
                    {
                        trenink.NazevSkupiny = "-";
                    }

                    if (reader["POPIS"] != DBNull.Value)
                    {
                        trenink.Popis = reader["POPIS"].ToString();
                    }
                    else
                    {
                        trenink.Popis = "Volitelné nezadáno !";
                    }

                    if (reader["MISTO"] != DBNull.Value)
                    {
                        trenink.Misto = reader["MISTO"].ToString();
                    }
                    else
                    {
                        trenink.Misto = "";
                    }

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
        /// Nastaví viditelnost sloupců podle role přihlášeného uživatele
        /// Administrátor vidí všechny sloupce, trenér a hráč nevidí rodné číslo trenéra
        /// </summary>
        private void NastavViditelnostSloupcuPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            // Ve výchozím stavu je sloupec viditelný
            RodneCisloTreneraSloupec.Visibility = Visibility.Visible;

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            // Trenér ani hráč nesmí vidět rodné číslo trenéra
            if (role == "trener" || role == "trenér" || role == "hrac" || role == "hráč")
            {
                RodneCisloTreneraSloupec.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Nastaví viditelnost ComboBoxu pro výběr tréninků podle role
        /// </summary>
        private void NastavViditelnostComboBoxuPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                cbVyberTreninku.Visibility = Visibility.Collapsed;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "trener" || role == "trenér" || role == "hrac" || role == "hráč")
            {
                cbVyberTreninku.Visibility = Visibility.Visible;
                return;
            }

            cbVyberTreninku.Visibility = Visibility.Collapsed;
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

            if (role == "hrac" || role == "hráč")
            {
                cbVyberTreninku.SelectedIndex = 1;
                cbVyberTreninku.IsEnabled = false;
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

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "hrac" || role == "hráč")
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                return;
            }

            if (role == "host")
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                return;
            }

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
                else if (PatriTreninkPrihlasenemuTrenerovi(vybranyTrenink))
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
        /// Zjistí, zda je aktuálně přihlášený uživatel hráč
        /// </summary>
        private bool JePrihlasenHrac()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                return false;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();
            return role == "hrac" || role == "hráč";
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

            if (prihlasenyUzivatel.IdClenKlubu == 0)
            {
                return false;
            }

            return trenink.IdTrener == prihlasenyUzivatel.IdClenKlubu;
        }

        /// <summary>
        /// Vrátí ID tréninkové skupiny přihlášeného hráče
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
        /// Zjistí, zda trénink patří do skupiny přihlášeného hráče
        /// </summary>
        private bool PatriTreninkPrihlasenemuHraci(Trenink trenink)
        {
            if (trenink == null)
            {
                return false;
            }

            int idSkupinyHrace = ZiskejIdTreninkoveSkupinyPrihlasenehoHrace();

            if (idSkupinyHrace <= 0)
            {
                return false;
            }

            return trenink.IdTreninkovaSkupina == idSkupinyHrace;
        }

        /// <summary>
        /// Aplikuje filtr tréninků podle vybrané položky v ComboBoxu
        /// </summary>
        private void AplikujFiltrTreninku()
        {
            if (jeVyhledavaniAktivni)
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

            if (cbVyberTreninku.SelectedIndex == 1 && JePrihlasenTrener())
            {
                ObservableCollection<Trenink> vlastniTreninky = new ObservableCollection<Trenink>();

                foreach (Trenink trenink in TreninkyData)
                {
                    if (PatriTreninkPrihlasenemuTrenerovi(trenink))
                    {
                        vlastniTreninky.Add(trenink);
                    }
                }

                DgTreninky.ItemsSource = vlastniTreninky;
                DgTreninky.SelectedItem = null;
                NastavPravaPodleRole();
                return;
            }

            if (cbVyberTreninku.SelectedIndex == 1 && JePrihlasenHrac())
            {
                ObservableCollection<Trenink> skupinoveTreninky = new ObservableCollection<Trenink>();

                foreach (Trenink trenink in TreninkyData)
                {
                    if (PatriTreninkPrihlasenemuHraci(trenink))
                    {
                        skupinoveTreninky.Add(trenink);
                    }
                }

                DgTreninky.ItemsSource = skupinoveTreninky;
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
            if (!inicializaceDokoncena)
            {
                return;
            }

            AplikujFiltrTreninku();
        }

        /// <summary>
        /// Obsluha změny výběru v DataGridu
        /// </summary>
        private void DgTreninky_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inicializaceDokoncena)
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
            if (jeVyhledavaniAktivni)
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

            if (JePrihlasenTrener() && !PatriTreninkPrihlasenemuTrenerovi(vybranyTrenink))
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

                cbVyberTreninku.SelectedIndex = 0;
                cbVyberTreninku.IsEnabled = false;

                NastavPravaPodleRole();
            }
        }

        /// <summary>
        /// Metoda slouží k zrušení vyhledávacího módu
        /// </summary>
        private void WindowUkonceniFiltrovani(object sender, KeyEventArgs e)
        {
            if (jeVyhledavaniAktivni && (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X))
            {
                jeVyhledavaniAktivni = false;

                NastavRezimVyberuTreninkuPodleRole();
                AplikujFiltrTreninku();

                e.Handled = true;
            }
        }

        /// <summary>
        /// Otevře dialog pro přidání tréninku
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Trenink pridanyTrenink = new Trenink();

            var okno = new PridejTrenink(pridanyTrenink);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                if (JePrihlasenTrener())
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
        /// Smaže vybraný trénink
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Trenink vybranyTrenink = DgTreninky.SelectedItem as Trenink;

            if (vybranyTrenink == null)
            {
                MessageBox.Show("Vyberte trénink !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (JePrihlasenTrener() && !PatriTreninkPrihlasenemuTrenerovi(vybranyTrenink))
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
        /// Zavře okno a vrátí uživatele na předchozí obrazovku
        /// </summary>
        private void BtnZpet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}