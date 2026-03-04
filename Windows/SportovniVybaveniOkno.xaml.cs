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
    /// Interakční logika pro SportovniVybaveniOkno.xaml
    /// </summary>
    public partial class SportovniVybaveniOkno : Window
    {
        /// <summary>
        /// Kolekce sportovního vybavení používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<SportovniVybaveni> SportovniVybaveniData { get; set; } = new ObservableCollection<SportovniVybaveni>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        public SportovniVybaveniOkno()
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);
            NactiSportovniVybaveni();

            DgSportovniVybaveni.ItemsSource = SportovniVybaveniData;
        }

        /// <summary>
        /// Načte sportovní vybavení z databáze a naplní kolekci SportovniVybaveniData
        /// </summary>
        private void NactiSportovniVybaveni()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                var cmd = new OracleCommand("SELECT * FROM SPORTOVNI_VYBAVENI_VIEW", conn);
                var reader = cmd.ExecuteReader();

                SportovniVybaveniData.Clear();

                while (reader.Read())
                {
                    SportovniVybaveni vybaveni = new SportovniVybaveni();

                    if (reader["IDSPORTOVNIVYBAVENI"] != DBNull.Value)
                    {
                        vybaveni.IdSportovniVybaveni = Convert.ToInt32(reader["IDSPORTOVNIVYBAVENI"]);
                    }
                    else
                    {
                        vybaveni.IdSportovniVybaveni = 0;
                    }

                    if (reader["NAZEV"] != DBNull.Value)
                    {
                        vybaveni.Nazev = reader["NAZEV"].ToString();
                    }
                    else
                    {
                        vybaveni.Nazev = "-";
                    }

                    if (reader["VELIKOST"] != DBNull.Value)
                    {
                        vybaveni.Velikost = reader["VELIKOST"].ToString();
                    }
                    else
                    {
                        vybaveni.Velikost = "-";
                    }

                    if (reader["DATUMNAKUPU"] != DBNull.Value)
                    {
                        vybaveni.DatumNakupu = Convert.ToDateTime(reader["DATUMNAKUPU"]);
                    }
                    else
                    {
                        vybaveni.DatumNakupu = DateTime.MinValue;
                    }

                    SportovniVybaveniData.Add(vybaveni);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání sportovního vybavení: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů sportovního vybavení
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SportovniVybaveni vybranySportovniVybaveni = DgSportovniVybaveni.SelectedItem as SportovniVybaveni;
            if (vybranySportovniVybaveni == null)
            {
                MessageBox.Show("Prosím vyberte záznam sportovního vybavení, které chcete editovat!", "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EditujSportovniVybaveni(vybranySportovniVybaveni);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {

                DgSportovniVybaveni.Items.Refresh();
            }
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání tréninků
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X.
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            //NajdiTrenink najdiTrenink = new NajdiTrenink(TreninkyData);
            //bool? vysledekDialogu = najdiTrenink.ShowDialog();

            //if (vysledekDialogu == true)
            //{
            //    if (najdiTrenink.FiltrovaniTreninky == null || najdiTrenink.FiltrovaniTreninky.Count == 0)
            //    {
            //        MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
            //        return;
            //    }

            //    MessageBox.Show(
            //        "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
            //        "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
            //        "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            //    DgTreninky.ItemsSource = new ObservableCollection<Trenink>(najdiTrenink.FiltrovaniTreninky);
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
                DgSportovniVybaveni.ItemsSource = SportovniVybaveniData;
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
        /// Otevře dialog pro přidání tréninku, uloží nový trénink do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SportovniVybaveni pridanyVybaveni = new SportovniVybaveni();
            var okno = new PridejSportovniVybaveni(pridanyVybaveni);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeSportovniVybaveni.PridejSportovniVybaveni(conn, pridanyVybaveni);

                NactiSportovniVybaveni();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidání tréninku: {ex.Message}");
            }
        }

        /// <summary>
        /// Smaže vybrané sportovní vybavení z databáze i z lokální kolekce a aktualizuje zobrazení podle aktuálního filtru
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SportovniVybaveni vybranySportovniVybaveni = (SportovniVybaveni)DgSportovniVybaveni.SelectedItem;

            if (vybranySportovniVybaveni == null)
            {
                MessageBox.Show("Vyberte sportovní vybavení !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Opravdu chcete smazat vybrané sportovní vybavení?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeSportovniVybaveni.OdeberSportovniVybaveni(conn, vybranySportovniVybaveni);

                SportovniVybaveniData.Remove(vybranySportovniVybaveni);

                MessageBox.Show("Sportovní vybavení bylo smazáno", "Information", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání Sportovního vybavení: {ex.Message}");
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
