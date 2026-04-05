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
    /// Interakční logika pro TreninkOkno.xaml
    /// </summary>
    public partial class TreninkOkno : Window
    {
        /// <summary>
        /// Kolekce tréninků používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<Trenink> TreninkyData { get; set; } = new ObservableCollection<Trenink>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        public TreninkOkno()
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimTabulkoveOkno(this, HlavniOkno.jeDarkModeAktivni);
            NactiTreninky();
            NastavPravaPodleRole();
            DgTreninky.ItemsSource = TreninkyData;
        }

        /// <summary>
        /// Načte hráče z databáze a naplní kolekci TreneriData
        /// </summary>
        public static void NactiTreninky()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                var cmd = new OracleCommand("SELECT * FROM TRENINKY_VIEW", conn);
                var reader = cmd.ExecuteReader();

                TreninkyData.Clear();

                while (reader.Read())
                {
                    Trenink trenink = new Trenink();

                    if (reader["IDTRENINK"] != DBNull.Value)
                    {
                        trenink.Id = Convert.ToInt32(reader["IDTRENINK"]);
                    }
                    else
                    {
                        trenink.Id = 0;
                    }

                    // ID Trenéra daného tréninku
                    if (reader["IDCLENKLUBU"] != DBNull.Value) 
                    {
                        trenink.IdTrener = Convert.ToInt32(reader["IDCLENKLUBU"]);
                    }
                    else
                    {
                        trenink.IdTrener = 0;
                    }

                    // Rodné číslo
                    if (reader["RODNECISLO"] != DBNull.Value)
                    {
                        trenink.RodneCislo = reader["RODNECISLO"].ToString();
                    }
                    else
                    {
                        trenink.RodneCislo = "0";
                    }

                    // Příjmení
                    if (reader["PRIJMENI"] != DBNull.Value)
                    {
                        trenink.Prijmeni = reader["PRIJMENI"].ToString();
                    }
                    else
                    {
                        trenink.Prijmeni = "-";
                    }

                    // Trénink - Popis (volitelný sloupec) 
                    if (reader["POPIS"] != DBNull.Value)
                    {
                        trenink.Popis = reader["POPIS"].ToString();
                    }
                    else
                    {
                        trenink.Popis = "Volitelné nezadáno !";
                    }

                    if (reader["MISTO"] != DBNull.Value)
                    {
                        trenink.Misto = reader["MISTO"].ToString();
                    }
                    else
                    {
                        trenink.Misto = "";
                    }

                    if (reader["DATUM"] != DBNull.Value)
                    {
                        trenink.Datum = Convert.ToDateTime(reader["DATUM"]);
                    }
                    else
                    {
                        trenink.Datum = DateTime.MinValue;
                    }


                    TreninkyData.Add(trenink);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání tréninků: {ex.Message}");
            }
        }

        // Zkontroluje roli přihlášeného uživatele a podle ní povolí nebo zakáže tlačítka
        private void NastavPravaPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel != null &&
                !string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role) &&
                (prihlasenyUzivatel.Role.Trim().ToLower() == "hrac" || prihlasenyUzivatel.Role.Trim().ToLower() == "hráč"))
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
            }
            else
            {
                BtnPridat.IsEnabled = true;
                BtnOdebrat.IsEnabled = true;
                BtnEdituj.IsEnabled = true;
                BtnNajdi.IsEnabled = true;
            }
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů tréninku
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Trenink vybranyTrenink = DgTreninky.SelectedItem as Trenink;
            if (vybranyTrenink == null)
            {
                MessageBox.Show("Prosím vyberte trénink, který chcete editovat!", "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EditujTrenink(vybranyTrenink);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {

                DgTreninky.Items.Refresh();
            }
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání tréninků
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            NajdiTrenink najdiTrenink = new NajdiTrenink(TreninkyData);
            bool? vysledekDialogu = najdiTrenink.ShowDialog();

            if (vysledekDialogu == true)
            {
                if (najdiTrenink.FiltrovaniTreninky == null || najdiTrenink.FiltrovaniTreninky.Count == 0)
                {
                    MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(
                    "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
                    "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                DgTreninky.ItemsSource = new ObservableCollection<Trenink>(najdiTrenink.FiltrovaniTreninky);
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
                DgTreninky.ItemsSource = TreninkyData;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Otevře dialog pro přidání tréninku, uloží nový trénink do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Trenink pridanyTrenink = new Trenink();
            var okno = new PridejTrenink(pridanyTrenink);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreninky.PridejTrenink(conn, pridanyTrenink);

                NactiTreninky();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidání tréninku: {ex.Message}");
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

            Trenink vybranyTrenink = (Trenink)DgTreninky.SelectedItem;

            if (vybranyTrenink == null)
            {
                MessageBox.Show("Vyberte trénink !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Opravdu chcete smazat vybraný trénink?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeTreninky.OdeberTrenink(conn, vybranyTrenink);

                TreninkyData.Remove(vybranyTrenink);

                MessageBox.Show("Trénink byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání tréninku: {ex.Message}");
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
