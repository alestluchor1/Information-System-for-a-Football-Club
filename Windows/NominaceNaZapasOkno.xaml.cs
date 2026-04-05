using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
using InformacniSystemKlub.Windows.NajdiWindows;
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

        public NominaceNaZapasOkno()
        {
            InitializeComponent();

            RezimZobrazeni.NastavRezimTabulkoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            if (ZapasyOkno.ZapasyData == null)
            {
                ZapasyOkno.ZapasyData = new ObservableCollection<Zapas>();
            }

            if (ZapasyOkno.ZapasyData.Count == 0)
            {
                ZapasyOkno.NactiZapasy();
            }

            NactiNominace();
            NastavPravaPodleRole();
            DgNominace.ItemsSource = NominaceData;

            ZkontrolujKartyVeVsechZapasech();
        }

        // Zkontroluje roli přihlášeného uživatele a podle ní povolí nebo zakáže tlačítka
        private void NastavPravaPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel != null &&
                !string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role) &&
                (prihlasenyUzivatel.Role.Trim().ToLower() == "hrac" ||
                 prihlasenyUzivatel.Role.Trim().ToLower() == "hráč"))
            {
                BtnPridej.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                BtnEdituj.IsEnabled = false;
                BtnNajdi.IsEnabled = true;
            }
            else
            {
                BtnPridej.IsEnabled = true;
                BtnOdebrat.IsEnabled = true;
                BtnEdituj.IsEnabled = true;
                BtnNajdi.IsEnabled = true;
            }
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
                    MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
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
                MessageBox.Show("Vyhledávací mód je aktivní. Ukončete ho pomocí CTRL + X", "Info",
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

                ZkontrolujKartyVeVsechZapasech();

                MessageBox.Show("Záznam byl smazán", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při mazání záznamu: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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
        /// Otevře dialog pro vyhledávání záznamů nominací
        /// Aktivuje vyhledávací mód, dokud není režim ukončen pomocí CTRL+X
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            NajdiNominaci najdiNominaci = new NajdiNominaci(NominaceData);
            bool? vysledekDialogu = najdiNominaci.ShowDialog();

            if (vysledekDialogu == true)
            {
                if (najdiNominaci.FiltrovaniNominace == null || najdiNominaci.FiltrovaniNominace.Count == 0)
                {
                    MessageBox.Show("Nenašly se žádné záznamy se zadanými filtry", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(
                    "Pokud je vyhledávací mód aktivní nemůžete přidávat ani odebírat vyhledaná data " +
                    "Pro ukončení vyhledávacího módu stiskněte klávesy CTRL + X",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                DgNominace.ItemsSource = new ObservableCollection<NominaceNaZapas>(najdiNominaci.FiltrovaniNominace);
                jeVyhledavaniAktivni = true;
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
        /// Ukončí vyhledávací mód pomocí kláves CTRL + X
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

    
    }
}