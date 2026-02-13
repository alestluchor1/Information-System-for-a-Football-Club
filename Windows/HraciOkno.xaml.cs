using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models; 
using InformacniSystemKlub.Windows;
using InformacniSystemKlub.Windows.NajdiWindows;
using InformacniSystemKlub.Windows.PřidáníWindows;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel; 
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows 
{
    public partial class HraciOkno : Window 
    {
        public static ObservableCollection<Hrac> HraciData { get; set; } = new ObservableCollection<Hrac>();

        private bool ready = false;
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
                switch (volba)
                {

                    case 1: // A - Tým
                        if (hrac.vek > 18)
                        {
                            vybraniHraci.Add(hrac);
                        }
                        break;
                    
                    case 2: // Dorost
                        if (hrac.vek >= 14 && hrac.vek <= 18)
                        {
                            vybraniHraci.Add(hrac);
                        }
                        break;

                    case 3: // Mladší žáci
                        if (hrac.vek >= 3 && hrac.vek < 14)
                        {
                            vybraniHraci.Add(hrac);
                        }
                        break;

                    default:
                        return;

                }
            }

            DgHraci.ItemsSource = vybraniHraci;
        }

        private void cbVyberTymu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ready == true)
            {
              VyberTymu();
            }
        }

        private void NactiHrace()
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
                        hrac.id = Convert.ToInt32(reader["IDCLENKLUBU"]);
                    }
                    else
                    {
                        hrac.id = 0;
                    }

                    if (reader["RODNECISLO"] != DBNull.Value)
                    {
                        hrac.rodneCislo = reader["RODNECISLO"].ToString();
                    }
                    else 
                    {
                        hrac.rodneCislo = "0";
                    }

                    if (reader["JMENO"] != DBNull.Value)
                    {
                        hrac.jmeno = reader["JMENO"].ToString();
                    }
                    else
                    {
                        hrac.jmeno = "-";
                    }

                    if (reader["PRIJMENI"] != DBNull.Value)
                    {
                        hrac.prijmeni = reader["PRIJMENI"].ToString();
                    }
                    else
                    {
                        hrac.prijmeni = "-";
                    }

                    if (reader["TELEFONNICISLO"] != DBNull.Value)
                    {
                        hrac.telefonniCislo = reader["TELEFONNICISLO"].ToString();
                    }
                    else
                    {
                        hrac.telefonniCislo = "-";
                    }

                    if (reader["POCETVSTRELENYCHGOLU"] != DBNull.Value)
                    {
                        hrac.pocetVstrelenychGolu = Convert.ToInt32(reader["POCETVSTRELENYCHGOLU"]);
                    }
                    else
                    {
                        hrac.pocetVstrelenychGolu = 0;
                    }

                    if (reader["POCETZLUTYCHKARET"] != DBNull.Value)
                    {
                        hrac.pocetZlutychKaret = Convert.ToInt32(reader["POCETZLUTYCHKARET"]);
                    }
                    else
                    {
                        hrac.pocetZlutychKaret = 0;
                    }

                    if (reader["POCETCERVENYCHKARET"] != DBNull.Value)
                    {
                        hrac.pocetCervenychKaret = Convert.ToInt32(reader["POCETCERVENYCHKARET"]);
                    }
                    else
                    {
                        hrac.pocetCervenychKaret = 0;
                    }

                    if (reader["POZICE"] != DBNull.Value)
                    {
                        hrac.pozice = reader["POZICE"].ToString();
                    }
                    else
                    {
                        hrac.pozice = "";
                    }

                    if (reader["VEK"] != DBNull.Value)
                    {
                        hrac.vek = Convert.ToInt32(reader["VEK"]);
                    }
                    else 
                    { 
                        hrac.vek= 0;
                    }

                    if (reader["ZDRAVOTNISTAV"] != DBNull.Value)
                    {
                        hrac.zdravotniStav = Convert.ToChar(reader["ZDRAVOTNISTAV"]);
                    }
                    else 
                    {
                        hrac.zdravotniStav = 'A';
                    }

                    HraciData.Add(hrac);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání hráčů: {ex.Message}");
            }
        }

        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        { 
        
        }

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
                    "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data. " +
                    "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                DgHraci.ItemsSource = new ObservableCollection<Hrac>(najdiHrace.FiltrovaniHraci);
                jeVyhledavaniAktivni = true;
            }
        }


        /// <summary>
        /// Metoda slouží k zrušení vyhledávacího módu, pokud se zmáčkne klávesa CTRL + X
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventArgs</param>
        private void WindowUkonceniFiltrovani(object sender, KeyEventArgs e)
        {
            if (jeVyhledavaniAktivni && (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X))
            {
                jeVyhledavaniAktivni = false;
                DgHraci.ItemsSource = HraciData;
                e.Handled = true;
            }
        }

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

        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);
        }

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

        private void BtnZpet_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }

       

    }
}
