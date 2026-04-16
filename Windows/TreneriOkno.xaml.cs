using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.NajdiWindows;
using InformacniSystemKlub.Windows.PřidáníWindows;
using InformacniSystemKlub.Windows.PridejWindows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Interakční logika pro TreneriOkno.xaml
    /// </summary>
    public partial class TreneriOkno : Window
    {
        /// <summary>
        /// Sdílená kolekce trenérů používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<Trener> TreneriData { get; set; } = new ObservableCollection<Trener>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// V tomto režimu není umožněno přidávání/odebírání dat
        /// </summary>
        private bool jeVyhledavaniAktivni = false; 

        public TreneriOkno()
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimTabulkoveOkno(this, HlavniOkno.jeDarkModeAktivni);
            NactiTreneri();

            DgTreneri.ItemsSource = TreneriData;
            NastavViditelnostSloupcuProUzivatele();
        }

        /// <summary>
        /// Nastaví viditelnost citlivých sloupců v tabulce trenérů podle role
        /// právě přihlášeného uživatele
        /// </summary>
        private void NastavViditelnostSloupcuProUzivatele()
        {
            Uzivatel uzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();
            string role = uzivatel.Role.ToLower();

            RodneCisloSloupec.Visibility = Visibility.Visible;
            TelefonniCisloSloupec.Visibility = Visibility.Visible;

            if (role == "trener")
            {
                RodneCisloSloupec.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Načte trenéry z databáze (pohled TRENERI_VIEW) a naplní kolekci TreneriData
        /// </summary>
        public static void NactiTreneri()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                var cmd = new OracleCommand("SELECT * FROM TRENERI_VIEW", conn);
                var reader = cmd.ExecuteReader();

                TreneriData.Clear();

                while (reader.Read())
                {
                    Trener trener = new Trener();

                    if (reader["IDCLENKLUBU"] != DBNull.Value)
                    {
                        trener.Id = Convert.ToInt32(reader["IDCLENKLUBU"]);
                    }
                    else
                    {
                        trener.Id = 0;
                    }

                    if (reader["RODNECISLO"] != DBNull.Value)
                    {
                        trener.RodneCislo = reader["RODNECISLO"].ToString();
                    }
                    else
                    {
                        trener.RodneCislo = "0";
                    }

                    if (reader["JMENO"] != DBNull.Value)
                    {
                        trener.Jmeno = reader["JMENO"].ToString();
                    }
                    else
                    {
                        trener.Jmeno = "-";
                    }

                    if (reader["PRIJMENI"] != DBNull.Value)
                    {
                        trener.Prijmeni = reader["PRIJMENI"].ToString();
                    }
                    else
                    {
                        trener.Prijmeni = "-";
                    }

                    if (reader["TELEFONNICISLO"] != DBNull.Value)
                    {
                        trener.TelefonniCislo = reader["TELEFONNICISLO"].ToString();
                    }
                    else
                    {
                        trener.TelefonniCislo = "-";
                    }

                    if (reader["SPECIALIZACE"] != DBNull.Value)
                    {
                        trener.Specializace = reader["SPECIALIZACE"].ToString();
                    }
                    else
                    {
                        trener.Specializace = "-";
                    }


                    TreneriData.Add(trener);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání trenérů: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů trenéra
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Trener vybranyTrener = DgTreneri.SelectedItem as Trener;
            if (vybranyTrener == null)
            {
                MessageBox.Show("Prosím vyberte trenéra, kterého chcete editovat!", "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EditujTrenera(vybranyTrener);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {

                DgTreneri.Items.Refresh();
            }
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání trenérů
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X.
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            NajdiTrenera najdiTrenera = new NajdiTrenera(TreneriData);
            bool? vysledekDialogu = najdiTrenera.ShowDialog();

            if (vysledekDialogu == true)
            {
                if (najdiTrenera.FiltrovaniTreneri == null || najdiTrenera.FiltrovaniTreneri.Count == 0)
                {
                    MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(
                    "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
                    "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                DgTreneri.ItemsSource = new ObservableCollection<Trener>(najdiTrenera.FiltrovaniTreneri);
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
                DgTreneri.ItemsSource = TreneriData;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Otevře dialog pro přidání trenéra, uloží nového trenéra do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            Trener pridanyTrener = new Trener();
            var okno = new PridejTrenera(pridanyTrener);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreneri.PridejTrenera(conn, pridanyTrener);

                NactiTreneri();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidání trenéra: {ex.Message}");
            }
        }

        /// <summary>
        /// Smaže vybraného trenéra z databáze i z lokální kolekce a aktualizuje zobrazení podle aktuálního filtru
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Trener vybranyTrener = (Trener)DgTreneri.SelectedItem;

            if (vybranyTrener == null)
            {
                MessageBox.Show("Vyberte trenéra !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Opravdu chcete smazat vybraného trenéra?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeTreneri.OdeberTrenera(conn, vybranyTrener);

                TreneriData.Remove(vybranyTrener);
               
                MessageBox.Show("Trenér byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání trenéra: {ex.Message}");
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
