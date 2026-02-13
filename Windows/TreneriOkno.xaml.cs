using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.NajdiWindows;
using InformacniSystemKlub.Windows.PřidáníWindows;
using InformacniSystemKlub.Windows.PridejWindows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);
            NactiTreneri();

            DgTreneri.ItemsSource = TreneriData;
        }

        /// <summary>
        /// Načte hráče z databáze (pohled TRENERI_VIEW) a naplní kolekci HraciData.
        /// </summary>
        private void NactiTreneri()
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

                    if (reader["TRENERSKALICENCE"] != DBNull.Value)
                    {
                        trener.TrenerskaLicence = reader["TRENERSKALICENCE"].ToString();
                    }
                    else 
                    {
                        trener.TrenerskaLicence= "-";
                    }

                    if (reader["POCETLETPRAXE"] != DBNull.Value)
                    {
                        trener.PocetLetPraxe = Convert.ToInt32(reader["POCETLETPRAXE"]);
                    }
                    else
                    {
                        trener.PocetLetPraxe = 0;
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

        }

        /// <summary>
        /// Otevře dialog pro vyhledávání trenérů
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X.
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            //NajdiTrenera najdiTrenera = new NajdiTrenera(TreneriData);
            //bool? vysledekDialogu = najdiTrenera.ShowDialog();

            //if (vysledekDialogu == true)
            //{
            //    if (najdiTrenera.FiltrovaniHraci == null || najdiTrenera.FiltrovaniHraci.Count == 0)
            //    {
            //        MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
            //        return;
            //    }

            //    MessageBox.Show(
            //        "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
            //        "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
            //        "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            //    DgTreneri.ItemsSource = new ObservableCollection<Hrac>(najdiTrenera.FiltrovaniHraci);
            //    jeVyhledavaniAktivni = true;
           // }
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
        /// Otevře dialog pro přidání trenéra, uloží nového trenéra do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
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
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
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
