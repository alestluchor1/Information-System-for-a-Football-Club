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
    /// Interakční logika pro OpatreniOkno.xaml
    /// </summary>
    public partial class OpatreniOkno : Window
    {
        /// <summary>
        /// Kolekce disciplinárních opatření používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<DisciplinarniOpatreni> DisciplinarniOpatreniData { get; set; } = new ObservableCollection<DisciplinarniOpatreni>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        public OpatreniOkno()
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);
            NactiDisciplinarniOpatreni();
            ZapasyOkno.NactiZapasy();

            DgOpatreni.ItemsSource = DisciplinarniOpatreniData;
        }

        /// <summary>
        /// Načte disciplinární opatření z databáze a naplní kolekci DisciplinarniOpatreniData
        /// </summary>
        private void NactiDisciplinarniOpatreni()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                var cmd = new OracleCommand("SELECT * FROM HRACI_OPATRENI_VIEW", conn);
                var reader = cmd.ExecuteReader();

                DisciplinarniOpatreniData.Clear();

                while (reader.Read())
                {
                    DisciplinarniOpatreni opatreni = new DisciplinarniOpatreni();

                    if (reader["IDOPATRENI"] != DBNull.Value)
                    {
                        opatreni.IdOpatreni = Convert.ToInt32(reader["IDOPATRENI"]);
                    }
                    else
                    {
                        opatreni.IdOpatreni = 0;
                    }

                    // ID hráče daného opatření
                    if (reader["IDCLENKLUBU"] != DBNull.Value)
                    {
                        opatreni.IdHrac = Convert.ToInt32(reader["IDCLENKLUBU"]);
                    }
                    else
                    {
                        opatreni.IdHrac = 0;
                    }

                    if (reader["JMENO"] != DBNull.Value)
                    {
                        opatreni.Jmeno = reader["JMENO"].ToString();
                    }
                    else
                    {
                        opatreni.Jmeno = "-";
                    }

                    if (reader["PRIJMENI"] != DBNull.Value)
                    {
                        opatreni.Prijmeni = reader["PRIJMENI"].ToString();
                    }
                    else
                    {
                        opatreni.Prijmeni = "-";
                    }

                    if (reader["UKONCENIDATUMOPATRENI"] != DBNull.Value)
                    {
                        opatreni.DatumUkonceniOpatreni = Convert.ToDateTime(reader["UKONCENIDATUMOPATRENI"]);
                    }
                    else
                    {
                        opatreni.DatumUkonceniOpatreni = DateTime.MinValue;
                    }

                    if (reader["DUVOD"] != DBNull.Value)
                    {
                        opatreni.Duvod = reader["DUVOD"].ToString();
                    }
                    else
                    {
                        opatreni.Duvod = "-";
                    }

                    if (reader["ZAPAS_IDZAPAS"] != DBNull.Value)
                    {
                        opatreni.IdZapas = Convert.ToInt32(reader["ZAPAS_IDZAPAS"]);
                    }
                    else
                    {
                        opatreni.IdZapas = null;
                    }

                    DisciplinarniOpatreniData.Add(opatreni);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání disciplinárních opatření: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů opatření
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DisciplinarniOpatreni vybranyOpatreni = DgOpatreni.SelectedItem as DisciplinarniOpatreni;
            if (vybranyOpatreni == null)
            {
                MessageBox.Show("Prosím vyberte disciplinární opatření, který chcete editovat!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EditujOpatreni(vybranyOpatreni);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {

                DgOpatreni.Items.Refresh();
            }
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání opatření
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X.
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            NajdiOpatreni najdiOpatreni = new NajdiOpatreni(DisciplinarniOpatreniData);
            bool? vysledekDialogu = najdiOpatreni.ShowDialog();

            if (vysledekDialogu == true)
            {
                if (najdiOpatreni.FiltrovaniDisciplinarniOpatreni == null || najdiOpatreni.FiltrovaniDisciplinarniOpatreni.Count == 0)
                {
                    MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(
                    "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
                    "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                DgOpatreni.ItemsSource = new ObservableCollection<DisciplinarniOpatreni>(najdiOpatreni.FiltrovaniDisciplinarniOpatreni);
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
                DgOpatreni.ItemsSource = DisciplinarniOpatreniData;
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
        /// Otevře dialog pro přidání opatření, uloží nové opatření do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (jeVyhledavaniAktivni == true)
                {
                    MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                DisciplinarniOpatreni pridanyOpatreni = new DisciplinarniOpatreni();
                var okno = new PridejOpatreni(pridanyOpatreni);

                if (okno.ShowDialog() != true)
                {
                    return;
                }

                foreach (var opatreni in DisciplinarniOpatreniData)
                {
                    if (opatreni.IdHrac == pridanyOpatreni.IdHrac)
                    {
                        MessageBox.Show("Tento hráč už má opatření!", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeOpatreni.PridejOpatreni(conn, pridanyOpatreni);

                NactiDisciplinarniOpatreni();

                MessageBox.Show("Opatření bylo úspěšně přidáno", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při přidání opatření:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při přidání opatření:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Smaže vybrané opatření z databáze i z lokální kolekce a aktualizuje zobrazení podle aktuálního filtru
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DisciplinarniOpatreni vybranyOpatreni = (DisciplinarniOpatreni)DgOpatreni.SelectedItem;

            if (vybranyOpatreni == null)
            {
                MessageBox.Show("Vyberte disciplinární opatření !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Opravdu chcete smazat vybrané disciplinární opatření hráče?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeOpatreni.OdeberOpatreni(conn, vybranyOpatreni);

                DisciplinarniOpatreniData.Remove(vybranyOpatreni);

                MessageBox.Show("Disciplinární opatření bylo smazáno", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání opatření: {ex.Message}");
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
