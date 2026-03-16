using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.NajdiWindows;
using InformacniSystemKlub.Windows.PridejWindows;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Interakční logika pro ZapasyOkno.xaml
    /// </summary>
    public partial class ZapasyOkno : Window
    {
        /// <summary>
        /// Kolekce zápasů používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<Zapas> ZapasyData { get; set; } = new ObservableCollection<Zapas>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        public ZapasyOkno()
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimTabulkoveOkno(this, HlavniOkno.jeDarkModeAktivni);
            NactiZapasy();

            DgZapasy.ItemsSource = ZapasyData;
        }

        /// <summary>
        /// Načte zápasy z databáze a naplní kolekci ZapasyData
        /// </summary>
        public static void NactiZapasy()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                using (var cmd = new OracleCommand("SELECT * FROM ZAPASY_VIEW", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    ZapasyData.Clear();

                    while (reader.Read())
                    {
                        Zapas zapas = new Zapas();

                        if (reader["IDZAPAS"] != DBNull.Value)
                        {
                            zapas.IdZapas = Convert.ToInt32(reader["IDZAPAS"]);
                        }
                        else
                        {
                            zapas.IdZapas = 0;
                        }

                        if (reader["DATUM"] != DBNull.Value)
                        {
                            zapas.Datum = Convert.ToDateTime(reader["DATUM"]);
                        }
                        else
                        {
                            zapas.Datum = DateTime.MinValue;
                        }

                        if (reader["STAV_ZAPASU_IDSTAV"] != DBNull.Value)
                        {
                            zapas.IdStav = (StavZapasuEnum)Convert.ToInt32(reader["STAV_ZAPASU_IDSTAV"]);
                        }
                        else
                        {
                            zapas.IdStav = 0;
                        }

                        if (reader["TYP_IDTYPSOUTEZE"] != DBNull.Value)
                        {
                            zapas.IdTypSouteze = (TypSoutezeEnum)Convert.ToInt32(reader["TYP_IDTYPSOUTEZE"]);
                        }
                        else
                        {
                            zapas.IdTypSouteze = 0;
                        }

                        if (reader["DOMACITYM"] != DBNull.Value)
                        {
                            zapas.DomaciTym = reader["DOMACITYM"].ToString();
                        }
                        else
                        {
                            zapas.DomaciTym = "-";
                        }

                        if (reader["HOSTETYM"] != DBNull.Value)
                        {
                            zapas.HosteTym = reader["HOSTETYM"].ToString();
                        }
                        else
                        {
                            zapas.HosteTym = "-";
                        }

                        if (reader["VYSLEDEK"] != DBNull.Value)
                        {
                            zapas.Vysledek = reader["VYSLEDEK"].ToString();
                        }
                        else
                        {
                            zapas.Vysledek = "-";
                        }

                        if (reader["POCETZLUTYCHKARET"] != DBNull.Value)
                        {
                            zapas.PocetZlutychKaret = Convert.ToInt32(reader["POCETZLUTYCHKARET"]);
                        }
                        else
                        {
                            zapas.PocetZlutychKaret = null;
                        }

                        if (reader["POCETCERVENYCHKARET"] != DBNull.Value)
                        {
                            zapas.PocetCervenychKaret = Convert.ToInt32(reader["POCETCERVENYCHKARET"]);
                        }
                        else
                        {
                            zapas.PocetCervenychKaret = null;
                        }

                        ZapasyData.Add(zapas);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání zápasů: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů zápasů
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Zapas vybranyZapas = DgZapasy.SelectedItem as Zapas;
            if (vybranyZapas == null)
            {
                MessageBox.Show("Prosím vyberte zápas, který chcete editovat!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EditujZapas(vybranyZapas);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {

                DgZapasy.Items.Refresh();
            }
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání zápasů
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X.
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            NajdiZapas najdiZapas = new NajdiZapas(ZapasyData);
            bool? vysledekDialogu = najdiZapas.ShowDialog();

            if (vysledekDialogu == true)
            {
                if (najdiZapas.FiltrovaniZapasy == null || najdiZapas.FiltrovaniZapasy.Count == 0)
                {
                    MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(
                    "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
                    "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                DgZapasy.ItemsSource = new ObservableCollection<Zapas>(najdiZapas.FiltrovaniZapasy);
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
                DgZapasy.ItemsSource = ZapasyData;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Otevře dialog pro přidání zápasu, uloží nový zápas do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Zapas pridanyZapas = new Zapas();
            var okno = new PridejZapas(pridanyZapas);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeZapasy.PridejZapas(conn, pridanyZapas);

                NactiZapasy();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidání zápasu: {ex.Message}");
            }
        }

        /// <summary>
        /// Smaže vybraný zápas z databáze i z lokální kolekce a aktualizuje zobrazení podle aktuálního filtru
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Zapas vybranyZapas = (Zapas)DgZapasy.SelectedItem;

            if (vybranyZapas == null)
            {
                MessageBox.Show("Vyberte zápas !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Opravdu chcete smazat vybraný zápas?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeZapasy.OdeberZapas(conn, vybranyZapas);

                ZapasyData.Remove(vybranyZapas);

                MessageBox.Show("Zápas byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání zápasu: {ex.Message}");
            }
        }

        /// <summary>
        /// Zavře okno a vrátí uživatele na předchozí obrazovku (Hlavní okno)
        /// </summary>
        private void BtnZpet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Otevře dialogové okno pro výběr umístění a názvu CSV souboru,
        /// do kterého se uloží aktuálně zobrazený rozpis zápasů.
        /// </summary>
        private void BtnUlozitCsv_Click(object sender, RoutedEventArgs e)
        {
            // Vytvoření dialogu pro výběr umístění souboru
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Title = "Uložit rozpis zápasů";
            dialog.Filter = "CSV soubor (*.csv)|*.csv";
            dialog.DefaultExt = "csv";
            dialog.AddExtension = true;

            // Výchozí složka bude plocha uživatele
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            dialog.FileName = "Rozpis_zapasu.csv";

            bool? vysledek = dialog.ShowDialog();

            if (vysledek != true)
            {
                return;
            }

            try
            {
                // Uložení dat do vybraného CSV souboru
                UlozDoCsv(dialog.FileName);

                MessageBox.Show("Soubor byl úspěšně uložen", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při ukládání souboru: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Uloží aktuálně zobrazené zápasy z DataGridu do CSV souboru
        /// </summary>
        /// <param name="cesta">Úplná cesta k cílovému CSV souboru</param>
        private void UlozDoCsv(string cesta)
        {
            IEnumerable zapasy = DgZapasy.ItemsSource;

            if (zapasy == null)
            {
                MessageBox.Show("Nejsou k dispozici žádná data pro export", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Otevření textového zapisovače pro vytvoření CSV souboru v kódování UTF-8
            using (StreamWriter writer = new StreamWriter(cesta, false, Encoding.UTF8))
            {
                // Zápis hlavičky sloupců do prvního řádku souboru
                writer.WriteLine("ID zápasu;Datum;Soutěž;Domácí tým;Hosté;Stav zápasu;Výsledek;Žluté karty;Červené karty");

                // Postupné zpracování všech položek v aktuálním seznamu zápasů
                foreach (object polozka in zapasy)
                {
                    Zapas zapas = polozka as Zapas;

                    if (zapas == null)
                    {
                        continue;
                    }

                    // Příprava výchozích textových hodnot pro jednotlivé sloupce
                    string idZapas = zapas.IdZapas.ToString();
                    string datum = "-";
                    string soutez = "-";
                    string domaci = "-";
                    string hoste = "-";
                    string stav = "-";
                    string vysledek = "-";
                    string zlute = "-";
                    string cervene = "-";

                    if (zapas.Datum != DateTime.MinValue)
                    {
                        datum = "'" + zapas.Datum.ToString("dd.MM.yyyy") + "'";
                    }

                    if (string.IsNullOrWhiteSpace(zapas.NazevSouteze) == false)
                    {
                        soutez = zapas.NazevSouteze;
                    }

                    if (string.IsNullOrWhiteSpace(zapas.DomaciTym) == false)
                    {
                        domaci = zapas.DomaciTym;
                    }

                    if (string.IsNullOrWhiteSpace(zapas.HosteTym) == false)
                    {
                        hoste = zapas.HosteTym;
                    }

                    if (string.IsNullOrWhiteSpace(zapas.StavZapasu) == false)
                    {
                        stav = zapas.StavZapasu;
                    }

                    if (string.IsNullOrWhiteSpace(zapas.Vysledek) == false)
                    {
                        vysledek = zapas.Vysledek;
                    }

                    if (zapas.PocetZlutychKaret.HasValue == true)
                    {
                        zlute = zapas.PocetZlutychKaret.Value.ToString();
                    }

                    if (zapas.PocetCervenychKaret.HasValue == true)
                    {
                        cervene = zapas.PocetCervenychKaret.Value.ToString();
                    }

                    // Ošetření textových hodnot kvůli správnému zápisu do CSV
                    domaci = UpravitTextProCsv(domaci);
                    hoste = UpravitTextProCsv(hoste);
                    soutez = UpravitTextProCsv(soutez);
                    stav = UpravitTextProCsv(stav);
                    vysledek = UpravitTextProCsv(vysledek);

                    // Zápis jednoho řádku reprezentujícího jeden zápas
                    writer.WriteLine(idZapas + ";" + datum + ";" + soutez + ";" + domaci + ";" + hoste + ";" + stav + ";" + vysledek + ";" + zlute + ";" + cervene);
                }
            }
        }

        /// <summary>
        /// Upraví textovou hodnotu tak, aby byla bezpečně zapsatelná do CSV souboru
        /// </summary>
        /// <param name="text">Vstupní text, který se má upravit pro CSV zápis</param>
        private string UpravitTextProCsv(string text)
        {
            if (text == null)
            {
                return "";
            }

            // Zdvojení uvozovek podle CSV formátu
            text = text.Replace("\"", "\"\"");

            // Pokud text obsahuje problematické znaky, obalí se do uvozovek
            if (text.Contains(";") || text.Contains("\"") || text.Contains("\n"))
            {
                text = "\"" + text + "\"";
            }

            return text;
        }

    }
}
