using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.NajdiWindows;
using InformacniSystemKlub.Windows.PřidáníWindows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Okno pro správu hráčů
    /// Data se načítají z databázového pohledu HRACI_VIEW
    /// </summary>
    public partial class HraciOkno : Window
    {
        /// <summary>
        /// Sdílená kolekce hráčů používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<Hrac> HraciData { get; set; } = new ObservableCollection<Hrac>();

        private bool ready = false;

        /// <summary>
        /// Zda je aktivní vyhledávací mód
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        public HraciOkno()
        {
            InitializeComponent();
            ready = false;
            RezimZobrazeni.NastavRezimTabulkoveOkno(this, HlavniOkno.jeDarkModeAktivni);
            NactiHrace();

            DgHraci.ItemsSource = HraciData;
            cbVyberTymu.SelectedIndex = 0;
            NastavViditelnostSloupcuProUzivatele();
            NastavOvladaniPodleRole();
            ready = true;
            VyberTymu();
        }

        /// <summary>
        /// Vrátí roli aktuálně přihlášeného uživatele
        /// </summary>
        private string ZiskejRoliUzivatele()
        {
            Uzivatel uzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (uzivatel == null || string.IsNullOrWhiteSpace(uzivatel.Role))
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
        /// Nastaví viditelnost sloupců v tabulce hráčů podle role
        /// právě přihlášeného uživatele
        /// </summary>
        private void NastavViditelnostSloupcuProUzivatele()
        {
            string role = ZiskejRoliUzivatele();

            IdSloupec.Visibility = Visibility.Visible;
            JmenoSloupec.Visibility = Visibility.Visible;
            PrijmeniSloupec.Visibility = Visibility.Visible;
            RodneCisloSloupec.Visibility = Visibility.Visible;
            TelefonniCisloSloupec.Visibility = Visibility.Visible;
            PoziceSloupec.Visibility = Visibility.Visible;
            GolySloupec.Visibility = Visibility.Visible;
            ZKSloupec.Visibility = Visibility.Visible;
            CKSloupec.Visibility = Visibility.Visible;
            ZdravotniStavSloupec.Visibility = Visibility.Visible;
            TreninkovaSkupinaSloupec.Visibility = Visibility.Visible;

            if (role == "trener")
            {
                RodneCisloSloupec.Visibility = Visibility.Collapsed;
            }

            if (role == "hrac")
            {
                RodneCisloSloupec.Visibility = Visibility.Collapsed;
                TelefonniCisloSloupec.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Nastaví povolení ovládacích prvků podle role uživatele
        /// </summary>
        private void NastavOvladaniPodleRole()
        {
            string role = ZiskejRoliUzivatele();

            if (role == "admin")
            {
                BtnPridat.IsEnabled = true;
                BtnOdebrat.IsEnabled = true;
                BtnEdituj.IsEnabled = true;
                BtnNajdi.IsEnabled = true;
                cbVyberTymu.IsEnabled = true;
                return;
            }

            if (role == "trener")
            {
                BtnPridat.IsEnabled = true;
                BtnOdebrat.IsEnabled = true;
                BtnEdituj.IsEnabled = true;
                BtnNajdi.IsEnabled = true;
                cbVyberTymu.IsEnabled = true;
                return;
            }

            if (role == "hrac")
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                cbVyberTymu.IsEnabled = true;
                return;
            }

            BtnPridat.IsEnabled = false;
            BtnOdebrat.IsEnabled = false;
            BtnEdituj.IsEnabled = false;
            BtnNajdi.IsEnabled = false;
            cbVyberTymu.IsEnabled = false;
        }

        /// <summary>
        /// Aplikuje filtr hráčů dle zvolené položky v ComboBoxu
        /// </summary>
        private void VyberTymu()
        {
            int volba = cbVyberTymu.SelectedIndex;

            if (volba == 0)
            {
                DgHraci.ItemsSource = HraciData;
                return;
            }

            ObservableCollection<Hrac> vybraniHraci = new ObservableCollection<Hrac>();

            foreach (var hrac in HraciData)
            {
                if (volba == 1)
                {
                    if (hrac.IdTreninkovaSkupina == 1)
                    {
                        vybraniHraci.Add(hrac);
                    }
                }

                if (volba == 2)
                {
                    if (hrac.IdTreninkovaSkupina == 2)
                    {
                        vybraniHraci.Add(hrac);
                    }
                }

                if (volba == 3)
                {
                    if (hrac.IdTreninkovaSkupina == 3)
                    {
                        vybraniHraci.Add(hrac);
                    }
                }
            }

            DgHraci.ItemsSource = vybraniHraci;
        }

        /// <summary>
        /// Událost změny výběru v ComboBoxu – provede znovu aplikaci filtru
        /// </summary>
        private void cbVyberTymu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ready == true)
            {
                VyberTymu();
            }
        }

        /// <summary>
        /// Z čísleníku tréninkové skupiny databáze na text
        /// </summary>
        private static string SkupinaNaText(int idSkupiny)
        {
            if (idSkupiny == 1)
            {
                return "A-Tým";
            }

            if (idSkupiny == 2)
            {
                return "Dorost";
            }

            if (idSkupiny == 3)
            {
                return "Mladší žáci";
            }

            return "Neznámá";
        }

        /// <summary>
        /// Načte hráče z databáze (pohled HRACI_VIEW) a naplní kolekci HraciData
        /// </summary>
        public static void NactiHrace()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                var cmd = new OracleCommand("SELECT * FROM HRACI_VIEW", conn);
                var reader = cmd.ExecuteReader();

                HraciData.Clear();

                while (reader.Read())
                {
                    Hrac hrac = new Hrac();

                    if (reader["IDCLENKLUBU"] != DBNull.Value)
                    {
                        hrac.Id = Convert.ToInt32(reader["IDCLENKLUBU"]);
                    }
                    else
                    {
                        hrac.Id = 0;
                    }

                    if (reader["IDTRENINKOVASKUPINA"] != DBNull.Value)
                    {
                        hrac.IdTreninkovaSkupina = Convert.ToInt32(reader["IDTRENINKOVASKUPINA"]);
                    }
                    else
                    {
                        hrac.IdTreninkovaSkupina = 0;
                    }

                    hrac.TreninkovaSkupinaText = SkupinaNaText(hrac.IdTreninkovaSkupina);

                    if (reader["RODNECISLO"] != DBNull.Value)
                    {
                        hrac.RodneCislo = reader["RODNECISLO"].ToString();
                    }
                    else
                    {
                        hrac.RodneCislo = "0";
                    }

                    if (reader["JMENO"] != DBNull.Value)
                    {
                        hrac.Jmeno = reader["JMENO"].ToString();
                    }
                    else
                    {
                        hrac.Jmeno = "-";
                    }

                    if (reader["PRIJMENI"] != DBNull.Value)
                    {
                        hrac.Prijmeni = reader["PRIJMENI"].ToString();
                    }
                    else
                    {
                        hrac.Prijmeni = "-";
                    }

                    if (reader["TELEFONNICISLO"] != DBNull.Value)
                    {
                        hrac.TelefonniCislo = reader["TELEFONNICISLO"].ToString();
                    }
                    else
                    {
                        hrac.TelefonniCislo = "-";
                    }

                    if (reader["POCETVSTRELENYCHGOLU"] != DBNull.Value)
                    {
                        hrac.PocetVstrelenychGolu = Convert.ToInt32(reader["POCETVSTRELENYCHGOLU"]);
                    }
                    else
                    {
                        hrac.PocetVstrelenychGolu = 0;
                    }

                    if (reader["POCETZLUTYCHKARET"] != DBNull.Value)
                    {
                        hrac.PocetZlutychKaret = Convert.ToInt32(reader["POCETZLUTYCHKARET"]);
                    }
                    else
                    {
                        hrac.PocetZlutychKaret = 0;
                    }

                    if (reader["POCETCERVENYCHKARET"] != DBNull.Value)
                    {
                        hrac.PocetCervenychKaret = Convert.ToInt32(reader["POCETCERVENYCHKARET"]);
                    }
                    else
                    {
                        hrac.PocetCervenychKaret = 0;
                    }

                    if (reader["POZICE"] != DBNull.Value)
                    {
                        hrac.Pozice = reader["POZICE"].ToString();
                    }
                    else
                    {
                        hrac.Pozice = "";
                    }

                    if (reader["ZDRAVOTNISTAV"] != DBNull.Value)
                    {
                        hrac.ZdravotniStav = Convert.ToChar(reader["ZDRAVOTNISTAV"]);
                    }
                    else
                    {
                        hrac.ZdravotniStav = 'A';
                    }

                    HraciData.Add(hrac);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání hráčů: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů hráče
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            if (ZiskejRoliUzivatele() == "hrac")
            {
                MessageBox.Show("Jako hráč nemáte oprávnění upravovat hráče",
                    "Přístup odepřen", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Hrac vybranyHrac = DgHraci.SelectedItem as Hrac;
            if (vybranyHrac == null)
            {
                MessageBox.Show("Prosím vyberte hráče, kterého chcete editovat!", "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EditujHrace(vybranyHrac);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                VyberTymu();
                DgHraci.Items.Refresh();
            }
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání hráčů (NajdiHrace) a v případě potvrzení zobrazí filtrovaný výsledek v DataGridu
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            NajdiHrace najdiHrace = new NajdiHrace(HraciData);
            bool? vysledekDialogu = najdiHrace.ShowDialog();

            if (vysledekDialogu == true)
            {
                if (najdiHrace.FiltrovaniHraci == null || najdiHrace.FiltrovaniHraci.Count == 0)
                {
                    MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(
                    "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
                    "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                DgHraci.ItemsSource = new ObservableCollection<Hrac>(najdiHrace.FiltrovaniHraci);
                jeVyhledavaniAktivni = true;
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
                DgHraci.ItemsSource = HraciData;
                VyberTymu();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Otevře dialog pro přidání hráče, uloží nového hráče do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (ZiskejRoliUzivatele() == "hrac")
            {
                MessageBox.Show("Jako hráč nemáte oprávnění přidávat hráče",
                    "Přístup odepřen", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Hrac pridanyHrac = new Hrac();
            var okno = new PridejHrace(pridanyHrac);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeHraci.PridejHrace(conn, pridanyHrac);

                NactiHrace();
                VyberTymu();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidání hráče: {ex.Message}");
            }
        }

        /// <summary>
        /// Smaže vybraného hráče z databáze i z lokální kolekce a aktualizuje zobrazení podle aktuálního filtru
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (ZiskejRoliUzivatele() == "hrac")
            {
                MessageBox.Show("Jako hráč nemáte oprávnění mazat hráče.",
                    "Přístup odepřen", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Hrac vybranyHrac = (Hrac)DgHraci.SelectedItem;

            if (vybranyHrac == null)
            {
                MessageBox.Show("Vyberte hráče !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Opravdu chcete smazat vybraného hráče?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeHraci.OdeberHrace(conn, vybranyHrac);

                HraciData.Remove(vybranyHrac);
                VyberTymu();
                MessageBox.Show("Hráč byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání hráče: {ex.Message}");
            }
        }

        /// <summary>
        /// Po kliknutí pravým tlačítkem označí řádek hráče jako vybraný
        /// </summary>
        private void DgHraci_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject source = e.OriginalSource as DependencyObject;

            while (source != null && !(source is DataGridRow))
            {
                source = System.Windows.Media.VisualTreeHelper.GetParent(source);
            }

            DataGridRow row = source as DataGridRow;

            if (row != null)
            {
                row.IsSelected = true;
                DgHraci.SelectedItem = row.Item;
            }
        }

        /// <summary>
        /// Zobrazí kontrakty vybraného hráče
        /// </summary>
        private void MenuKontrakty_Click(object sender, RoutedEventArgs e)
        {
            Hrac vybranyHrac = DgHraci.SelectedItem as Hrac;

            if (vybranyHrac == null)
            {
                MessageBox.Show("Vyberte hráče!", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                string textKontraktu = DatabazeHraci.ZiskejKontraktyHrace(conn, vybranyHrac.Id);

                if (string.IsNullOrWhiteSpace(textKontraktu))
                {
                    MessageBox.Show("Tento hráč nemá žádný kontrakt nyní.", "Kontrakty", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(textKontraktu, "Kontrakty hráče", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání kontraktů: " + ex.Message,
                    "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Zobrazí disciplinární opatření vybraného hráče
        /// </summary>
        private void MenuOpatreni_Click(object sender, RoutedEventArgs e)
        {
            Hrac vybranyHrac = DgHraci.SelectedItem as Hrac;

            if (vybranyHrac == null)
            {
                MessageBox.Show("Vyberte hráče!", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                string textOpatreni = DatabazeHraci.ZiskejOpatreniHrace(conn, vybranyHrac.Id);

                if (string.IsNullOrWhiteSpace(textOpatreni))
                {
                    MessageBox.Show("Tento hráč nemá žádné disciplinární opatření nyní.", "Disciplinární opatření", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(textOpatreni, "Disciplinární opatření hráče", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání disciplinárních opatření: " + ex.Message,
                    "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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