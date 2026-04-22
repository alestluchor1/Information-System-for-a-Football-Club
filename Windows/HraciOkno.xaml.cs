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

            if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
            {
                TreneriOkno.NactiTreneri();
            }

            NactiHrace();

            DgHraci.ItemsSource = HraciData;

            NastavComboBoxPodleRole();
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
        /// Vrátí ID tréninkové skupiny přihlášeného trenéra
        /// </summary>
        private int ZiskejIdTreninkoveSkupinyPrihlasenehoTrenera()
        {
            Uzivatel uzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (uzivatel == null)
            {
                return 0;
            }

            if (uzivatel.IdTreninkovaSkupina > 0)
            {
                return uzivatel.IdTreninkovaSkupina;
            }

            if (uzivatel.IdClenKlubu <= 0)
            {
                return 0;
            }

            foreach (Trener trener in TreneriOkno.TreneriData)
            {
                if (trener.Id == uzivatel.IdClenKlubu)
                {
                    return trener.IdTreninkovaSkupina;
                }
            }

            return 0;
        }

        /// <summary>
        /// Zjistí, zda hráč patří do skupiny přihlášeného trenéra
        /// </summary>
        private bool PatriHracPrihlasenemuTrenerovi(Hrac hrac)
        {
            if (hrac == null)
            {
                return false;
            }

            int idSkupinyTrenera = ZiskejIdTreninkoveSkupinyPrihlasenehoTrenera();

            if (idSkupinyTrenera <= 0)
            {
                return false;
            }

            return hrac.IdTreninkovaSkupina == idSkupinyTrenera;
        }

        /// <summary>
        /// Nastaví obsah ComboBoxu podle role uživatele
        /// </summary>
        private void NastavComboBoxPodleRole()
        {
            string role = ZiskejRoliUzivatele();

            cbVyberTymu.Items.Clear();

            if (role == "admin")
            {
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "Všichni hráči" });
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "A" });
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "Dorost" });
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "Mladší žáci" });

                cbVyberTymu.SelectedIndex = 0;
                cbVyberTymu.IsEnabled = true;
                return;
            }

            if (role == "trener")
            {
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "Všichni hráči" });
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "Hráči mé skupiny" });

                cbVyberTymu.SelectedIndex = 1;
                cbVyberTymu.IsEnabled = true;
                return;
            }

            if (role == "hrac")
            {
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "Všichni hráči" });
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "A" });
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "Dorost" });
                cbVyberTymu.Items.Add(new ComboBoxItem { Content = "Mladší žáci" });

                cbVyberTymu.SelectedIndex = 0;
                cbVyberTymu.IsEnabled = true;
                return;
            }

            cbVyberTymu.Items.Add(new ComboBoxItem { Content = "Všichni hráči" });
            cbVyberTymu.SelectedIndex = 0;
            cbVyberTymu.IsEnabled = false;
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
                BtnOdebrat.IsEnabled = DgHraci.SelectedItem != null;
                BtnEdituj.IsEnabled = DgHraci.SelectedItem != null;
                BtnNajdi.IsEnabled = true;
                cbVyberTymu.IsEnabled = jeVyhledavaniAktivni == false;
                DgHraci.ContextMenu = ContextMenuHraci;
                return;
            }

            if (role == "trener")
            {
                BtnPridat.IsEnabled = true;
                BtnNajdi.IsEnabled = true;
                cbVyberTymu.IsEnabled = jeVyhledavaniAktivni == false;
                DgHraci.ContextMenu = null;

                Hrac vybranyHrac = DgHraci.SelectedItem as Hrac;

                if (vybranyHrac == null)
                {
                    BtnOdebrat.IsEnabled = false;
                    BtnEdituj.IsEnabled = false;
                }
                else if (PatriHracPrihlasenemuTrenerovi(vybranyHrac))
                {
                    BtnOdebrat.IsEnabled = true;
                    BtnEdituj.IsEnabled = true;
                }
                else
                {
                    BtnOdebrat.IsEnabled = false;
                    BtnEdituj.IsEnabled = false;
                }

                return;
            }

            if (role == "hrac")
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                cbVyberTymu.IsEnabled = jeVyhledavaniAktivni == false;
                DgHraci.ContextMenu = null;
                return;
            }

            BtnPridat.IsEnabled = false;
            BtnOdebrat.IsEnabled = false;
            BtnEdituj.IsEnabled = false;
            BtnNajdi.IsEnabled = false;
            cbVyberTymu.IsEnabled = false;
            DgHraci.ContextMenu = null;
        }

        /// <summary>
        /// Aplikuje filtr hráčů dle zvolené položky v ComboBoxu
        /// </summary>
        private void VyberTymu()
        {
            if (jeVyhledavaniAktivni == true)
            {
                return;
            }

            string role = ZiskejRoliUzivatele();

            if (cbVyberTymu.SelectedItem == null)
            {
                DgHraci.ItemsSource = HraciData;
                NastavOvladaniPodleRole();
                return;
            }

            string vybranaPolozka = (cbVyberTymu.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrWhiteSpace(vybranaPolozka))
            {
                DgHraci.ItemsSource = HraciData;
                NastavOvladaniPodleRole();
                return;
            }

            ObservableCollection<Hrac> vybraniHraci = new ObservableCollection<Hrac>();

            if (role == "trener")
            {
                if (vybranaPolozka == "Všichni hráči")
                {
                    DgHraci.ItemsSource = HraciData;
                    DgHraci.SelectedItem = null;
                    NastavOvladaniPodleRole();
                    return;
                }

                if (vybranaPolozka == "Hráči mojí skupiny")
                {
                    foreach (Hrac hrac in HraciData)
                    {
                        if (PatriHracPrihlasenemuTrenerovi(hrac))
                        {
                            vybraniHraci.Add(hrac);
                        }
                    }

                    DgHraci.ItemsSource = vybraniHraci;
                    DgHraci.SelectedItem = null;
                    NastavOvladaniPodleRole();
                    return;
                }
            }

            if (vybranaPolozka == "Všichni hráči")
            {
                DgHraci.ItemsSource = HraciData;
                DgHraci.SelectedItem = null;
                NastavOvladaniPodleRole();
                return;
            }

            foreach (Hrac hrac in HraciData)
            {
                if (vybranaPolozka == "A" && hrac.IdTreninkovaSkupina == 1)
                {
                    vybraniHraci.Add(hrac);
                }

                if (vybranaPolozka == "Dorost" && hrac.IdTreninkovaSkupina == 2)
                {
                    vybraniHraci.Add(hrac);
                }

                if (vybranaPolozka == "Mladší žáci" && hrac.IdTreninkovaSkupina == 3)
                {
                    vybraniHraci.Add(hrac);
                }
            }

            DgHraci.ItemsSource = vybraniHraci;
            DgHraci.SelectedItem = null;
            NastavOvladaniPodleRole();
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
                return "A";
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

                reader.Close();
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

            if (ZiskejRoliUzivatele() == "trener" && PatriHracPrihlasenemuTrenerovi(vybranyHrac) == false)
            {
                MessageBox.Show("Můžete upravovat pouze hráče své tréninkové skupiny!",
                    "Přístup odepřen", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EditujHrace(vybranyHrac);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                NactiHrace();
                VyberTymu();
                DgHraci.Items.Refresh();
            }
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání hráčů
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
                DgHraci.SelectedItem = null;
                jeVyhledavaniAktivni = true;
                cbVyberTymu.IsEnabled = false;
                NastavOvladaniPodleRole();
            }
        }

        /// <summary>
        /// Ukončení filtru přes CTRL+X
        /// </summary>
        private void WindowUkonceniFiltrovani(object sender, KeyEventArgs e)
        {
            if (jeVyhledavaniAktivni && (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X))
            {
                jeVyhledavaniAktivni = false;
                DgHraci.ItemsSource = HraciData;
                VyberTymu();
                cbVyberTymu.IsEnabled = true;
                NastavOvladaniPodleRole();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Přidání hráče
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

            if (ZiskejRoliUzivatele() == "trener")
            {
                pridanyHrac.IdTreninkovaSkupina = ZiskejIdTreninkoveSkupinyPrihlasenehoTrenera();
                pridanyHrac.TreninkovaSkupinaText = SkupinaNaText(pridanyHrac.IdTreninkovaSkupina);
            }

            var okno = new PridejHrace(pridanyHrac);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                if (ZiskejRoliUzivatele() == "trener" && PatriHracPrihlasenemuTrenerovi(pridanyHrac) == false)
                {
                    MessageBox.Show("Můžete přidávat pouze hráče své tréninkové skupiny!",
                        "Přístup odepřen", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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
        /// Odebrání hráče
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

            Hrac vybranyHrac = DgHraci.SelectedItem as Hrac;

            if (vybranyHrac == null)
            {
                MessageBox.Show("Vyberte hráče !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ZiskejRoliUzivatele() == "trener" && PatriHracPrihlasenemuTrenerovi(vybranyHrac) == false)
            {
                MessageBox.Show("Můžete mazat pouze hráče své tréninkové skupiny!",
                    "Přístup odepřen", MessageBoxButton.OK, MessageBoxImage.Warning);
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
        /// Po změně výběru v DataGridu znovu nastaví práva tlačítek
        /// </summary>
        private void DgHraci_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ready == false)
            {
                return;
            }

            NastavOvladaniPodleRole();
        }

        /// <summary>
        /// Pravý klik funguje jen pro admina
        /// </summary>
        private void DgHraci_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ZiskejRoliUzivatele() != "admin")
            {
                e.Handled = true;
                return;
            }

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
            if (ZiskejRoliUzivatele() != "admin")
            {
                return;
            }

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
                    MessageBox.Show("Tento hráč nemá žádný kontrakt nyní", "Kontrakty", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (ZiskejRoliUzivatele() != "admin")
            {
                return;
            }

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
                    MessageBox.Show("Tento hráč nemá žádné disciplinární opatření nyní", "Disciplinární opatření", MessageBoxButton.OK, MessageBoxImage.Information);
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
        /// Zavře okno a vrátí uživatele na předchozí obrazovku
        /// </summary>
        private void BtnZpet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}