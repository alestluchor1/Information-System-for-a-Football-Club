using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.PridejWindows;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Okno pro správu nominací na zápas
    /// </summary>
    public partial class NominaceNaZapasOkno : Window
    {
        /// <summary>
        /// Kolekce nominací, která se zobrazuje v DataGridu
        /// </summary>
        public static ObservableCollection<NominaceNaZapas> NominaceData { get; set; } = new ObservableCollection<NominaceNaZapas>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        /// <summary>
        /// Pomocný příznak, aby se filtr nespouštěl při inicializaci
        /// </summary>
        private bool ready = false;

        public NominaceNaZapasOkno()
        {
            InitializeComponent();
            ready = false;

            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            if (ZapasyOkno.ZapasyData == null)
            {
                ZapasyOkno.ZapasyData = new ObservableCollection<Zapas>();
            }

            if (ZapasyOkno.ZapasyData.Count == 0)
            {
                ZapasyOkno.NactiZapasy();
            }

            NactiNominace();

            DgNominace.ItemsSource = NominaceData;

            cbVyberTymu.SelectedIndex = 0;
            ready = true;

            VyberTymu();
            ZkontrolujKartyVeVsechZapasech();
        }

        /// <summary>
        /// Otevře dialog pro přidání záznamu, uloží nový záznam do databáze a obnoví seznam
        /// </summary>
        private void BtnPridej_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (jeVyhledavaniAktivni == true)
                {
                    MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                NominaceNaZapas novaNominace = new NominaceNaZapas();

                PridejNominaci okno = new PridejNominaci(novaNominace);
                bool? vysledek = okno.ShowDialog();

                if (vysledek == true)
                {
                    var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                    DatabazeNominace.PridejNominaci(conn, novaNominace);

                    NactiNominace();
                    HraciOkno.NactiHrace();

                    VyberTymu();
                    ZkontrolujKartyVeVsechZapasech();

                    MessageBox.Show("Nominace byla úspěšně přidána", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při přidání nominace: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsluha tlačítka Edituj pro úpravu údajů záznamu
        /// </summary>
        private void BtnEdituj_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            NominaceNaZapas vybranyZaznam = DgNominace.SelectedItem as NominaceNaZapas;
            if (vybranyZaznam == null)
            {
                MessageBox.Show("Prosím vyberte záznam, který chcete editovat!", "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new EditujNominaci(vybranyZaznam);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                VyberTymu();

                DgNominace.Items.Refresh();
            }
        }

        /// <summary>
        /// Smaže vybraný záznam z databáze a aktualizuje zobrazení podle aktuálního filtru
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            if (jeVyhledavaniAktivni == true)
            {
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            NominaceNaZapas vybranyZaznam = DgNominace.SelectedItem as NominaceNaZapas;

            if (vybranyZaznam == null)
            {
                MessageBox.Show("Vyberte záznam!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Opravdu chcete smazat vybraný záznam?",
                "Warning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                DatabazeNominace.OdeberNominaci(conn, vybranyZaznam);

                NactiNominace();
                HraciOkno.NactiHrace();

                VyberTymu();
                ZkontrolujKartyVeVsechZapasech();

                MessageBox.Show("Záznam byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při mazání záznamu: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filtruje nominace podle vybrané tréninkové skupiny v ComboBoxu
        /// </summary>
        private void VyberTymu()
        {
            int volba = cbVyberTymu.SelectedIndex;

            if (volba == 0)
            {
                DgNominace.ItemsSource = NominaceData;
                return;
            }

            ObservableCollection<NominaceNaZapas> vybrane = new ObservableCollection<NominaceNaZapas>();

            foreach (var n in NominaceData)
            {
                if (volba == 1)
                {
                    if (n.IdTreninkovaSkupina == 1)
                    {
                        vybrane.Add(n);
                    }
                }

                if (volba == 2)
                {
                    if (n.IdTreninkovaSkupina == 2)
                    {
                        vybrane.Add(n);
                    }
                }

                if (volba == 3)
                {
                    if (n.IdTreninkovaSkupina == 3)
                    {
                        vybrane.Add(n);
                    }
                }
            }

            DgNominace.ItemsSource = vybrane;
        }

        /// <summary>
        /// Událost při změně výběru v ComboBoxu – znovu aplikuje filtr
        /// </summary>
        private void cbVyberTymu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ready == true)
            {
                VyberTymu();
            }
        }

        /// <summary>
        /// Načte nominace z databáze (pohled NOMINACE_VIEW) a naplní kolekci NominaceData
        /// </summary>
        public static void NactiNominace()
        {
            try
            {
                OracleConnection conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                using (OracleCommand cmd = new OracleCommand("SELECT * FROM NOMINACE_VIEW", conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    NominaceData.Clear();

                    while (reader.Read())
                    {
                        NominaceNaZapas n = new NominaceNaZapas();

                        if (reader["IDNOMINACE"] != DBNull.Value)
                        {
                            n.IdNominace = Convert.ToInt32(reader["IDNOMINACE"]);
                        }
                        else
                        {
                            n.IdNominace = 0;
                        }

                        if (reader["IDHRACE"] != DBNull.Value)
                        {
                            n.IdHrace = Convert.ToInt32(reader["IDHRACE"]);
                        }
                        else
                        {
                            n.IdHrace = 0;
                        }

                        if (reader["RODNECISLO"] != DBNull.Value)
                        {
                            n.RodneCislo = reader["RODNECISLO"].ToString();
                        }
                        else
                        {
                            n.RodneCislo = "";
                        }

                        if (reader["PRIJMENI"] != DBNull.Value)
                        {
                            n.Prijmeni = reader["PRIJMENI"].ToString();
                        }
                        else
                        {
                            n.Prijmeni = "";
                        }

                        if (reader["IDZAPAS"] != DBNull.Value)
                        {
                            n.IdZapas = Convert.ToInt32(reader["IDZAPAS"]);
                        }
                        else
                        {
                            n.IdZapas = 0;
                        }

                        if (reader["DOMACITYM"] != DBNull.Value)
                        {
                            n.DomaciTym = reader["DOMACITYM"].ToString();
                        }
                        else
                        {
                            n.DomaciTym = "";
                        }

                        if (reader["HOSTETYM"] != DBNull.Value)
                        {
                            n.HosteTym = reader["HOSTETYM"].ToString();
                        }
                        else
                        {
                            n.HosteTym = "";
                        }

                        if (reader["STAV_ZAPASU"] != DBNull.Value)
                        {
                            n.StavZapasu = reader["STAV_ZAPASU"].ToString();
                        }
                        else
                        {
                            n.StavZapasu = "";
                        }

                        if (reader["POCETODEHRANYCHMINUT"] != DBNull.Value)
                        {
                            n.PocetOdehranychMinut = Convert.ToInt32(reader["POCETODEHRANYCHMINUT"]);
                        }
                        else
                        {
                            n.PocetOdehranychMinut = 0;
                        }

                        if (reader["POCETGOLU"] != DBNull.Value)
                        {
                            n.PocetGolu = Convert.ToInt32(reader["POCETGOLU"]);
                        }
                        else
                        {
                            n.PocetGolu = 0;
                        }

                        if (reader["ZLUTEKARTY"] != DBNull.Value)
                        {
                            n.ZluteKarty = Convert.ToInt32(reader["ZLUTEKARTY"]);
                        }
                        else
                        {
                            n.ZluteKarty = 0;
                        }

                        if (reader["CERVENEKARTY"] != DBNull.Value)
                        {
                            n.CerveneKarty = Convert.ToInt32(reader["CERVENEKARTY"]);
                        }
                        else
                        {
                            n.CerveneKarty = 0;
                        }

                        if (reader["IDTRENINKOVASKUPINA"] != DBNull.Value)
                        {
                            n.IdTreninkovaSkupina = Convert.ToInt32(reader["IDTRENINKOVASKUPINA"]);
                        }
                        else
                        {
                            n.IdTreninkovaSkupina = 0;
                        }

                        NominaceData.Add(n);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání nominací: " + ex.Message);
            }
        }

        /// <summary>
        /// Projede všechny zápasy a u každého zkontroluje, jestli součty karet v nominacích nepřesahují počet karet v zápase
        /// </summary>
        private void ZkontrolujKartyVeVsechZapasech()
        {
            foreach (var zapas in ZapasyOkno.ZapasyData)
            {
                if (zapas == null)
                {
                    continue;
                }

                int idZapas = zapas.IdZapas;

                if (idZapas <= 0)
                {
                    continue;
                }

                ZkontrolujKartyProZapas(zapas);
            }
        }

        /// <summary>
        /// Zkontroluje jeden konkrétní zápas: sečte ŽK/ČK ze všech nominací pro daný zápas
        /// a porovná s hodnotami v tabulce zápasů
        /// </summary>
        private void ZkontrolujKartyProZapas(Zapas zapas)
        {
            int idZapas = zapas.IdZapas;

            int soucetZlute = 0;
            int soucetCervene = 0;

            foreach (var n in NominaceData)
            {
                if (n.IdZapas == idZapas)
                {
                    soucetZlute = soucetZlute + n.ZluteKarty;
                    soucetCervene = soucetCervene + n.CerveneKarty;
                }
            }

            if (zapas.PocetZlutychKaret != null)
            {
                int maxZlute = zapas.PocetZlutychKaret.Value;

                if (soucetZlute > maxZlute)
                {
                    MessageBox.Show(
                        "Zápas ID " + idZapas + " (" + zapas.DomaciTym + " vs " + zapas.HosteTym + ")\n" +
                        "V nominacích je moc ŽK: " + soucetZlute + "\n" +
                        "V zápase je: " + maxZlute,
                        "Chyba",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }

            if (zapas.PocetCervenychKaret != null)
            {
                int maxCervene = zapas.PocetCervenychKaret.Value;

                if (soucetCervene > maxCervene)
                {
                    MessageBox.Show(
                        "Zápas ID " + idZapas + " (" + zapas.DomaciTym + " vs " + zapas.HosteTym + ")\n" +
                        "V nominacích je moc ČK: " + soucetCervene + "\n" +
                        "V zápase je: " + maxCervene,
                        "Chyba",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
        }

        /// <summary>
        /// Ukončí vyhledávací mód pomocí kláves CTRL + X.
        /// </summary>
        private void WindowUkonceniFiltrovani(object sender, KeyEventArgs e)
        {
            if (jeVyhledavaniAktivni && (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X))
            {
                jeVyhledavaniAktivni = false;
                DgNominace.ItemsSource = NominaceData;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Zavře okno
        /// </summary>
        private void BtnZpet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Přepne barvy podle režimu (dark / light)
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

            if (darkMode == false)
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
    }
}