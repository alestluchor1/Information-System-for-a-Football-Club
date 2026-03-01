using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.NajdiWindows;
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
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);
            NactiZapasy();

            DgZapasy.ItemsSource = ZapasyData;
        }

        /// <summary>
        /// Načte zápasy z databáze a naplní kolekci ZapasyData
        /// </summary>
        private void NactiZapasy()
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
            //NajdiZapas najdiZapas = new NajdiZapas(ZapasyData);
            //bool? vysledekDialogu = najdiZapas.ShowDialog();

            //if (vysledekDialogu == true)
            //{
            //    if (najdiZapas.FiltrovaniZapasy == null || najdiZapas.FiltrovaniZapasy.Count == 0)
            //    {
            //        MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
            //        return;
            //    }

            //    MessageBox.Show(
            //        "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
            //        "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
            //        "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            //    DgZapasy.ItemsSource = new ObservableCollection<Trenink>(najdiZapas.FiltrovaniZapasy);
            //    jeVyhledavaniAktivni = true;
            //}
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
        /// Nastaví podle zvoleného režimu (dark / light)
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
        /// Otevře dialog pro přidání zápasu, uloží nový zápas do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
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
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
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

    }
}
