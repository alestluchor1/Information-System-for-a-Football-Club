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
    /// Interakční logika pro KontraktyOkno.xaml
    /// </summary>
    public partial class KontraktyOkno : Window
    {

        /// <summary>
        /// Sdílená kolekce kontraktů používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<Kontrakt> KontraktyData { get; set; } = new ObservableCollection<Kontrakt>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// V tomto režimu není umožněno přidávání/odebírání dat
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        public KontraktyOkno()
        {
            InitializeComponent();

            ApplyTheme(HlavniOkno.jeDarkModeAktivni);
            NactiKontrakty();

            DgKontrakty.ItemsSource = KontraktyData;
        }

        /// <summary>
        /// Načte kontrakty z databáze (pohled KONTRAKTY_VIEW) a naplní kolekci KontraktyData
        /// </summary>
        public static void NactiKontrakty()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                using (var cmd = new OracleCommand("SELECT * FROM KONTRAKTY_VIEW", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    KontraktyData.Clear();

                    while (reader.Read())
                    {
                        Kontrakt kontrakt = new Kontrakt();

                        if (reader["HRAC_IDCLENKLUBU"] != DBNull.Value)
                        {
                            kontrakt.IdHrac = Convert.ToInt32(reader["HRAC_IDCLENKLUBU"]);
                        }
                        else
                        {
                            kontrakt.IdHrac = 0;
                        }

                        if (reader["DATUMZACATKU"] != DBNull.Value)
                        {
                            kontrakt.DatumZacatku = DateOnly.FromDateTime(Convert.ToDateTime(reader["DATUMZACATKU"]));
                        }
                        else
                        {
                            kontrakt.DatumZacatku = default;
                        }

                        if (reader["DATUMKONCE"] != DBNull.Value)
                        {
                            kontrakt.DatumKonce = DateOnly.FromDateTime(Convert.ToDateTime(reader["DATUMKONCE"]));
                        }
                        else
                        {
                            kontrakt.DatumKonce = default;
                        }

                        if (reader["PLAT"] != DBNull.Value)
                        {
                            kontrakt.Plat = Convert.ToInt32(reader["PLAT"]);
                        }
                        else
                        {
                            kontrakt.Plat = 0;
                        }

                        if (reader["CISLONAAGENTA"] != DBNull.Value)
                        {
                            kontrakt.TelCisloNaAgenta = reader["CISLONAAGENTA"].ToString();
                        }
                        else
                        {
                            kontrakt.TelCisloNaAgenta = "";
                        }

                        if (reader["VYSTUPNIKLAUZULE"] != DBNull.Value)
                        {
                            kontrakt.VystupniKlauzule = Convert.ToInt32(reader["VYSTUPNIKLAUZULE"]);
                        }
                        else
                        {
                            kontrakt.VystupniKlauzule = 0;
                        }

                        if (reader["JMENO"] != DBNull.Value && reader["PRIJMENI"] != DBNull.Value)
                        {
                            kontrakt.KontraktHrace = new Hrac();

                            if (reader["HRAC_IDCLENKLUBU"] != DBNull.Value)
                            {
                                kontrakt.KontraktHrace.id = Convert.ToInt32(reader["HRAC_IDCLENKLUBU"]);
                            }
                            else
                            {
                                kontrakt.KontraktHrace.id = 0;
                            }

                            kontrakt.KontraktHrace.jmeno = reader["JMENO"].ToString();
                            kontrakt.KontraktHrace.prijmeni = reader["PRIJMENI"].ToString();
                        }
                        else
                        {
                            kontrakt.KontraktHrace = null;
                        }

                        KontraktyData.Add(kontrakt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání kontraktů: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů kontraktu
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            //if (jeVyhledavaniAktivni == true)
            //{
            //    MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
            //        MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //Trener vybranyTrener = DgTreneri.SelectedItem as Trener;
            //if (vybranyTrener == null)
            //{
            //    MessageBox.Show("Prosím vyberte trenéra, kterého chcete editovat!", "Chyba",
            //        MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //var dialog = new EditujTrenera(vybranyTrener);
            //bool? result = dialog.ShowDialog();

            //if (result == true)
            //{

            //    DgTreneri.Items.Refresh();
            //}
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání kontraktů
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            //NajdiTrenera najdiTrenera = new NajdiTrenera(TreneriData);
            //bool? vysledekDialogu = najdiTrenera.ShowDialog();

            //if (vysledekDialogu == true)
            //{
            //    if (najdiTrenera.FiltrovaniTreneri == null || najdiTrenera.FiltrovaniTreneri.Count == 0)
            //    {
            //        MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
            //        return;
            //    }

            //    MessageBox.Show(
            //        "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
            //        "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
            //        "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            //    DgTreneri.ItemsSource = new ObservableCollection<Trener>(najdiTrenera.FiltrovaniTreneri);
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
                DgKontrakty.ItemsSource = KontraktyData;
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
        /// Otevře dialog pro přidání kontraktu, uloží nového kontrakt do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Kontrakt pridanyKontrakt = new Kontrakt();
            var okno = new PridejKontrakt(pridanyKontrakt);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeKontrakty.PridejKontrakt(conn, pridanyKontrakt);

                NactiKontrakty();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidání kontraktu: {ex.Message}");
            }
        }

        /// <summary>
        /// Smaže vybraný kontrakt z databáze i z lokální kolekce
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Kontrakt vybranyKontrakt = (Kontrakt)DgKontrakty.SelectedItem;

            if (vybranyKontrakt == null)
            {
                MessageBox.Show("Vyberte kontrakt !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Opravdu chcete smazat vybraný kontrakt?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeKontrakty.OdeberKontrakt(conn, vybranyKontrakt);

                KontraktyData.Remove(vybranyKontrakt);

                MessageBox.Show("Kontrakt byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání kontraktu: {ex.Message}");
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
