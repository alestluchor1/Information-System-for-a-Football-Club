using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.NajdiWindows;
using InformacniSystemKlub.Windows.PřidáníWindows;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows 
{
    /// <summary>
    /// Okno pro správu hráčů (zobrazení, filtrování dle kategorií, přidání/odebrání)
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
        /// Příznak, zda je aktivní vyhledávací mód
        /// V tomto režimu není umožněno přidávání/odebírání dat
        /// </summary>
        private bool jeVyhledavaniAktivni = false; // pro ukončení filtrování
        public HraciOkno()
        {
            InitializeComponent();
            ready = false;
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);
            NactiHrace();

            DgHraci.ItemsSource = HraciData;
            cbVyberTymu.SelectedIndex = 0;
            ready = true;
            VyberTymu();             
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

        public static void PrepocitejStatistikyHracuZNominaci()
        {
            if (HraciData == null)
            {
                return;
            }

            foreach (var hrac in HraciData)
            {
                if (hrac == null)
                {
                    continue;
                }

                hrac.PocetVstrelenychGolu = 0;
                hrac.PocetZlutychKaret = 0;
                hrac.PocetCervenychKaret = 0;

                foreach (var nominace in NominaceNaZapasOkno.NominaceData)
                {
                    if (nominace == null)
                    {
                        continue;
                    }

                    if (nominace.IdHrace == hrac.Id)
                    {
                        if (nominace.StavZapasu == "ODEHRÁNO" || nominace.StavZapasu == "Odehráno")
                        {
                            hrac.PocetVstrelenychGolu = hrac.PocetVstrelenychGolu + nominace.PocetGolu;
                            hrac.PocetZlutychKaret = hrac.PocetZlutychKaret + nominace.ZluteKarty;
                            hrac.PocetCervenychKaret = hrac.PocetCervenychKaret + nominace.CerveneKarty;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Událost změny výběru v ComboBoxu – provede znovu aplikace filtru
        /// </summary>
        private void cbVyberTymu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ready == true)
            {
              VyberTymu();
            }
        }

        private static string SkupinaNaText(int idSkupiny)
        {
            // Překlad ID tréninkové skupiny číselníku na text
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
        /// Načte hráče z databáze (pohled HRACI_VIEW) a naplní kolekci HraciData.
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
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X.
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
                e.Handled = true;
            }
        }

        /// <summary>
        /// Nastaví barevné zdroje okna podle zvoleného režimu (dark / light)
        /// </summary>
        private void ApplyTheme(bool darkMode)
        {
            if (darkMode == true)
            {
                Resources["WindowBgBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x0F, 0x19));
                Resources["CardBrush"] = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
                Resources["CardAltBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));

                Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0x1F, 0x29, 0x37));

                Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB));
                Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));

                Resources["AccentBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                Resources["AccentDarkBrush"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));
            }
            else if (darkMode == false)
            {
                Resources["WindowBgBrush"] = new SolidColorBrush(Color.FromRgb(0xFA, 0xFA, 0xFA));

                Resources["CardBrush"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                Resources["CardAltBrush"] = new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC));

                Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));

                Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
                Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x6B, 0x72, 0x80));

                Resources["AccentBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                Resources["AccentDarkBrush"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));
            }
        }

        /// <summary>
        /// Otevře dialog pro přidání hráče, uloží nového hráče do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
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
                VyberTymu(); // znovu aplikuje aktuální filtr z comboboxu
                MessageBox.Show("Hráč byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání hráče: {ex.Message}");
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
