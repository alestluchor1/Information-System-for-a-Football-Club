using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.NajdiWindows;
using InformacniSystemKlub.Windows.PridejWindows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Interakční logika pro NastaveniOkno.xaml
    /// </summary>
    public partial class NastaveniOkno : Window
    {
        /// <summary>
        /// Kolekce uživatelů používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<Uzivatel> UzivateleData { get; set; } = new ObservableCollection<Uzivatel>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        public NastaveniOkno()
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);
            NactiUzivatele();

            DgUzivatele.ItemsSource = UzivateleData;
        }

        /// <summary>
        /// Načte uživatele z databáze a naplní kolekci UzivateleData
        /// </summary>
        public static void NactiUzivatele()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                var cmd = new OracleCommand("SELECT * FROM UZIVATELE_VIEW", conn);
                var reader = cmd.ExecuteReader();

                UzivateleData.Clear();

                while (reader.Read())
                {
                    Uzivatel uzivatel = new Uzivatel();

                    if (reader["IDUZIVATELSKYUCET"] != DBNull.Value)
                    {
                        uzivatel.IdUzivatelskyUcet = Convert.ToInt32(reader["IDUZIVATELSKYUCET"]);
                    }
                    else
                    {
                        uzivatel.IdUzivatelskyUcet = 0;
                    }

                    if (reader["UZIVATELSKEJMENO"] != DBNull.Value)
                    {
                        uzivatel.UzivatelskeJmeno = reader["UZIVATELSKEJMENO"].ToString();
                    }
                    else
                    {
                        uzivatel.UzivatelskeJmeno = "";
                    }

                    if (reader["HESLO"] != DBNull.Value)
                    {
                        uzivatel.Heslo = reader["HESLO"].ToString();
                    }
                    else
                    {
                        uzivatel.Heslo = "";
                    }

                    if (reader["SALT"] != DBNull.Value)
                    {
                        uzivatel.Salt = reader["SALT"].ToString();
                    }
                    else
                    {
                        uzivatel.Salt = "";
                    }

                    if (reader["ROLE"] != DBNull.Value)
                    {
                        uzivatel.Role = reader["ROLE"].ToString();
                    }
                    else
                    {
                        uzivatel.Role = "";
                    }

                    if (reader["EMAIL"] != DBNull.Value)
                    {
                        uzivatel.Email = reader["EMAIL"].ToString();
                    }
                    else
                    {
                        uzivatel.Email = "";
                    }

                    if (reader["POSLEDNIPRIHLASENI"] != DBNull.Value)
                    {
                        uzivatel.PosledniPrihlaseni = Convert.ToDateTime(reader["POSLEDNIPRIHLASENI"]);
                    }
                    else
                    {
                        uzivatel.PosledniPrihlaseni = DateTime.MinValue;
                    }

                    if (reader["RODNECISLO"] != DBNull.Value)
                    {
                        uzivatel.RodneCislo = reader["RODNECISLO"].ToString();
                    }
                    else
                    {
                        uzivatel.RodneCislo = "";
                    }

                    UzivateleData.Add(uzivatel);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání uživatelů: {ex.Message}");
            }
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů uživatele
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            //if (jeVyhledavaniAktivni == true)
            //{
            //    MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
            //        MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //Uzivatel vybranyUzivatel = DgUzivatele.SelectedItem as Uzivatel;
            //if (vybranyUzivatel == null)
            //{
            //    MessageBox.Show("Prosím vyberte uživatele, kterého chcete editovat!", "Chyba",
            //        MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //var dialog = new EditujUzivatele(vybranyUzivatel);
            //bool? result = dialog.ShowDialog();

            //if (result == true)
            //{
            //    DgUzivatele.Items.Refresh();
            //}
        }

        /// <summary>
        /// Otevře dialog pro vyhledávání uživatelů
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X.
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            //NajdiUzivatele najdiUzivatele = new NajdiUzivatele(UzivateleData);
            //bool? vysledekDialogu = najdiUzivatele.ShowDialog();

            //if (vysledekDialogu == true)
            //{
            //    if (najdiUzivatele.FiltrovaniUzivatele == null || najdiUzivatele.FiltrovaniUzivatele.Count == 0)
            //    {
            //        MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found",
            //            MessageBoxButton.OK, MessageBoxImage.Information);
            //        return;
            //    }

            //    MessageBox.Show(
            //        "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data. " +
            //        "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
            //        "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            //    DgUzivatele.ItemsSource = new ObservableCollection<Uzivatel>(najdiUzivatele.FiltrovaniUzivatele);
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
                DgUzivatele.ItemsSource = UzivateleData;
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
            else
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
        /// Otevře dialog pro přidání uživatele, uloží nového uživatele do databáze a obnoví seznam
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Uzivatel pridanyUzivatel = new Uzivatel();
            var okno = new PridejNastaveni(pridanyUzivatel);

            if (okno.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeNastaveni.PridejUzivatele(conn, pridanyUzivatel);

                NactiUzivatele();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při přidání uživatele: {ex.Message}");
            }
        }

        /// <summary>
        /// Smaže vybraného uživatele z databáze i z lokální kolekce a aktualizuje zobrazení podle aktuálního filtru
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Uzivatel vybranyUzivatel = (Uzivatel)DgUzivatele.SelectedItem;

            if (vybranyUzivatel == null)
            {
                MessageBox.Show("Vyberte uživatele!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Opravdu chcete smazat vybraného uživatele?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeNastaveni.OdeberUzivatele(conn, vybranyUzivatel);

                UzivateleData.Remove(vybranyUzivatel);

                MessageBox.Show("Uživatel byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při mazání uživatele: {ex.Message}");
            }
        }

        /// <summary>
        /// Zavře okno a vrátí uživatele na předchozí obrazovku
        /// </summary>
        private void BtnZpet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}