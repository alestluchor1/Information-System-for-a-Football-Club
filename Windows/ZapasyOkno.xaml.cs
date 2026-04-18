using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.NajdiWindows;
using InformacniSystemKlub.Windows.PridejWindows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

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

        /// <summary>
        /// Pomocná proměnná určující, zda už proběhla inicializace okna
        /// </summary>
        private bool inicializaceDokoncena = false;

        public ZapasyOkno()
        {
            InitializeComponent();
            inicializaceDokoncena = false;
            RezimZobrazeni.NastavRezimTabulkoveOkno(this, HlavniOkno.jeDarkModeAktivni);
            NactiZapasy();
            NastavPravaPodleRole();
            DgZapasy.ItemsSource = ZapasyData;
            cbVyberTymu.SelectedIndex = 0;
            inicializaceDokoncena = true;
            VyberTymu();
        }

        /// <summary>
        /// Nastaví práva tlačítek podle role přihlášeného uživatele
        /// </summary>
        private void NastavPravaPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null)
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                BtnUlozitCsv.IsEnabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                BtnUlozitCsv.IsEnabled = true;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "hrac" || role == "hráč" || role == "host")
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
                BtnUlozitCsv.IsEnabled = true;
            }
            else
            {
                BtnPridat.IsEnabled = true;
                BtnOdebrat.IsEnabled = true;
                BtnEdituj.IsEnabled = true;
                BtnNajdi.IsEnabled = true;
                BtnUlozitCsv.IsEnabled = true;
            }
        }

        /// <summary>
        /// Aplikuje filtr zápasů dle zvolené položky v ComboBoxu
        /// </summary>
        private void VyberTymu()
        {
            int volba = cbVyberTymu.SelectedIndex;

            if (volba == 0)
            {
                DgZapasy.ItemsSource = ZapasyData;
                return;
            }

            ObservableCollection<Zapas> vybraneZapasy = new ObservableCollection<Zapas>();

            foreach (var zapas in ZapasyData)
            {
                // Filtr pro A-Tým
                if (volba == 1)
                {
                    if (zapas.IdTreninkovaSkupina == 1)
                    {
                        vybraneZapasy.Add(zapas);
                    }
                }

                // Filtr pro Dorost
                if (volba == 2)
                {
                    if (zapas.IdTreninkovaSkupina == 2)
                    {
                        vybraneZapasy.Add(zapas);
                    }
                }

                // Filtr pro Mladší žáci
                if (volba == 3)
                {
                    if (zapas.IdTreninkovaSkupina == 3)
                    {
                        vybraneZapasy.Add(zapas);
                    }
                }
            }

            DgZapasy.ItemsSource = vybraneZapasy;
        }

        /// <summary>
        /// Událost změny výběru v ComboBoxu – provede znovu aplikaci filtru
        /// </summary>
        private void cbVyberTymu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inicializaceDokoncena == true)
            {
                VyberTymu();
            }
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

                        if (reader["IDTRENINKOVASKUPINA"] != DBNull.Value)
                        {
                            zapas.IdTreninkovaSkupina = Convert.ToInt32(reader["IDTRENINKOVASKUPINA"]);
                        }
                        else
                        {
                            zapas.IdTreninkovaSkupina = 0;
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
                VyberTymu();
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
                VyberTymu();
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
                VyberTymu();
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
                VyberTymu();

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
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Title = "Uložit rozpis zápasů";
            dialog.Filter = "CSV soubor (*.csv)|*.csv";
            dialog.DefaultExt = "csv";
            dialog.AddExtension = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.FileName = "Rozpis_zapasu.csv";

            bool? vysledek = dialog.ShowDialog();

            if (vysledek != true)
            {
                return;
            }

            try
            {
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

            using (StreamWriter writer = new StreamWriter(cesta, false, Encoding.UTF8))
            {
                writer.WriteLine("ID zápasu;Datum;Soutěž;Tréninková skupina;Domácí tým;Hosté;Stav zápasu;Výsledek;Žluté karty;Červené karty");

                foreach (object polozka in zapasy)
                {
                    Zapas zapas = polozka as Zapas;

                    if (zapas == null)
                    {
                        continue;
                    }

                    string idZapas = zapas.IdZapas.ToString();
                    string datum = "-";
                    string soutez = "-";
                    string treninkovaSkupina = "-";
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

                    if (string.IsNullOrWhiteSpace(zapas.NazevTreninkoveSkupiny) == false)
                    {
                        treninkovaSkupina = zapas.NazevTreninkoveSkupiny;
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

                    domaci = UpravitTextProCsv(domaci);
                    hoste = UpravitTextProCsv(hoste);
                    soutez = UpravitTextProCsv(soutez);
                    treninkovaSkupina = UpravitTextProCsv(treninkovaSkupina);
                    stav = UpravitTextProCsv(stav);
                    vysledek = UpravitTextProCsv(vysledek);

                    writer.WriteLine(idZapas + ";" + datum + ";" + soutez + ";" + treninkovaSkupina + ";" + domaci + ";" + hoste + ";" + stav + ";" + vysledek + ";" + zlute + ";" + cervene);
                }
            }
        }

        /// <summary>
        /// Upraví textovou hodnotu tak, aby byla bezpečně zapsatelná do CSV souboru
        /// </summary>
        /// <param name="text">Vstupní text, který se má upravit pro CSV zápis</param>
        private string UpravitTextProCsv(string text)
        {
            // Pokud je vstupní hodnota null, vrátí se prázdný řetězec
            if (text == null)
            {
                return "";
            }

            // V CSV se uvozovky uvnitř textu zapisují zdvojeně
            text = text.Replace("\"", "\"\"");

            // Pokud text obsahuje středník, uvozovku nebo nový řádek,
            // obalí se do uvozovek, aby byl správně uložen jako jedna hodnota
            if (text.Contains(";") || text.Contains("\"") || text.Contains("\n"))
            {
                text = "\"" + text + "\"";
            }

            // Vrátí upravený text vhodný pro CSV
            return text;
        }
    }
}