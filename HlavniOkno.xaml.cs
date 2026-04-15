using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Hlavní okno aplikace
    /// Zajišťuje zobrazení hlavního menu, aktuálního data a času,
    /// přepínání dark/light režimu a otevření jednotlivých oken podle rolí
    /// </summary>
    public partial class HlavniOkno : Window
    {

        private DispatcherTimer timer;
        public static bool jeDarkModeAktivni = false;
        private static Uzivatel prihlasenyUzivatel;

        public HlavniOkno()
        {
            InitializeComponent();
            SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

            RezimZobrazeni.NastavRezim(this, HlavniOkno.jeDarkModeAktivni);

            this.timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            ZobrazPrihlasenehoUzivatele();
            NastavPristupyDlazdic();
            Timer_Tick(null, EventArgs.Empty);
        }

        /// <summary>
        /// Uloží objekt aktuálně přihlášeného uživatele
        /// Tato metoda se volá po úspěšném přihlášení
        /// </summary>
        public static void NastavPrihlasenehoUzivatele(Uzivatel uzivatel)
        {
            prihlasenyUzivatel = uzivatel;
        }

        /// <summary>
        /// Obnoví zobrazení přihlášeného uživatele v horní liště
        /// </summary>
        public void ObnovPrihlasenehoUzivatele()
        {
            ZobrazPrihlasenehoUzivatele();
            NastavPristupyDlazdic();
        }

        /// <summary>
        /// Vrátí objekt aktuálně přihlášeného uživatele
        /// </summary>
        public static Uzivatel ZiskejPrihlasenehoUzivatele()
        {
            return prihlasenyUzivatel;
        }

        /// <summary>
        /// Zobrazí do horní informační lišty jméno a roli aktuálně přihlášeného uživatele
        /// Pokud není žádný uživatel přihlášen, zobrazí se text Host
        /// </summary>
        private void ZobrazPrihlasenehoUzivatele()
        {
            if (prihlasenyUzivatel == null)
            {
                TxtAktualniUzivatel.Text = "Host";
                return;
            }

            string uzivatelskeJmeno = prihlasenyUzivatel.UzivatelskeJmeno;
            string role = prihlasenyUzivatel.Role;

            if (string.IsNullOrWhiteSpace(uzivatelskeJmeno))
            {
                uzivatelskeJmeno = "Uživatel";
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                TxtAktualniUzivatel.Text = uzivatelskeJmeno;
                return;
            }

            TxtAktualniUzivatel.Text = uzivatelskeJmeno + " (" + role + ")";
        }

        /// <summary>
        /// Událost časovače
        /// Každou sekundu aktualizuje zobrazené datum a čas
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            TxtTime.Text = DateTime.Now.ToString("HH:mm:ss");
            TxtDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Přepne aplikaci mezi světlým a tmavým režimem
        /// </summary>
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            if (jeDarkModeAktivni == false)
            {
                jeDarkModeAktivni = true;
                RezimZobrazeni.NastavRezim(this, HlavniOkno.jeDarkModeAktivni);
            }
            else
            {
                jeDarkModeAktivni = false;
                RezimZobrazeni.NastavRezim(this, HlavniOkno.jeDarkModeAktivni);
            }
        }

        /// <summary>
        /// Otevře okno kalendáře
        /// </summary>
        private void BtnKalendarClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Kalendář") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            Kalendar kalendar = new Kalendar();
            kalendar.ShowDialog();
        }

        /// <summary>
        /// Otevře okno nominací sestav na zápas
        /// </summary>
        private void BtnNominaceClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Nominace") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            NominaceNaZapasOkno nominace = new NominaceNaZapasOkno();
            nominace.ShowDialog();
        }

        /// <summary>
        /// Otevře okno zápasů
        /// </summary>
        private void BtnZapasy_Click(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Zápasy") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            ZapasyOkno zapasyOkno = new ZapasyOkno();
            zapasyOkno.ShowDialog();
        }

        /// <summary>
        /// Otevře okno správy sportovního vybavení
        /// </summary>
        private void BtnSportovniVybaveni_Click(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Sportovní vybavení") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            SportovniVybaveniOkno vybaveniOkno = new SportovniVybaveniOkno();
            vybaveniOkno.ShowDialog();
        }

        /// <summary>
        /// Otevře okno správy kontraktů.
        /// </summary>
        private void BtnKontraktyClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Smlouvy") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            KontraktyOkno kontraktyOkno = new KontraktyOkno();
            kontraktyOkno.ShowDialog();
        }

        /// <summary>
        /// Otevře okno disciplinárních opatření hráče
        /// </summary>
        private void BtnOpatreniClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Disciplinární opatření") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            OpatreniOkno opatreniOkno = new OpatreniOkno();
            opatreniOkno.ShowDialog();
        }

        /// <summary>
        /// Otevře okno hráčů
        /// Hlavní okno se dočasně skryje a po zavření okna hráčů se znovu zobrazí
        /// </summary>
        private void BtnHraciClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Hráči") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            HraciOkno hraciOkno = new HraciOkno();
            hraciOkno.Owner = this;

            this.Hide();

            hraciOkno.Closed += HraciOkno_Closed;

            hraciOkno.Show();
        }

        /// <summary>
        /// Po zavření okna hráčů znovu zobrazí hlavní okno
        /// </summary>
        private void HraciOkno_Closed(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        /// <summary>
        /// Otevře okno trenérů
        /// </summary>
        private void BtnTreneriClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Trenéři") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            TreneriOkno treneriOkno = new TreneriOkno();
            treneriOkno.Owner = this;

            this.Hide();

            treneriOkno.Closed += TreneriOkno_Closed;

            treneriOkno.Show();
        }

        /// <summary>
        /// Po zavření okna trenérů znovu zobrazí hlavní okno
        /// </summary>
        private void TreneriOkno_Closed(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        /// <summary>
        /// Otevře okno tréninků
        /// </summary>
        private void BtnTreninkyClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Tréninky") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            TreninkOkno treninkOkno = new TreninkOkno();
            treninkOkno.Owner = this;

            this.Hide();

            treninkOkno.Closed += TreninkyOkno_Closed;

            treninkOkno.Show();
        }

        /// <summary>
        /// Po zavření okna tréninků znovu zobrazí hlavní okno
        /// </summary>
        private void TreninkyOkno_Closed(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        /// <summary>
        /// Otevře okno nastavení
        /// </summary>
        private void BtnNastaveniClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Nastavení") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            NastaveniOkno nastaveniOkno = new NastaveniOkno();
            nastaveniOkno.ShowDialog();
        }

        /// <summary>
        /// Otevře okno notifikací
        /// </summary>
        private void BtnNotifikaceClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Notifikace") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            NastaveniOkno.NactiUzivatele();

            Notifikace notifikace = new Notifikace(NastaveniOkno.UzivateleData);
            notifikace.ShowDialog();
        }

        /// <summary>
        /// Otevře editor sestav
        /// </summary>
        private void BtnEditorSestavClick(object sender, RoutedEventArgs e)
        {
            if (MaUzivatelPristup("Editor sestav") == false)
            {
                ZobrazZakazanyPristup();
                return;
            }

            EditorSestav editorSestav = new EditorSestav();
            editorSestav.ShowDialog();
        }

        /// <summary>
        /// Vrátí uživatele na přihlašovací obrazovku a ukončí hlavní okno
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            PrihlasovaciOkno prihlasovaciOkno = new PrihlasovaciOkno();
            Application.Current.MainWindow = prihlasovaciOkno;
            prihlasovaciOkno.Show();
            this.Close();
        }

        /// <summary>
        /// Vrátí roli aktuálně přihlášeného uživatele
        /// Pokud není nikdo přihlášen, vrací roli Host
        /// </summary>
        private string ZiskejRoliUzivatele()
        {
            if (prihlasenyUzivatel == null)
            {
                return "host";
            }

            if (string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                return "host";
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "admin")
            {
                return "admin";
            }

            if (role == "trenér" || role == "trener")
            {
                return "trener";
            }

            if (role == "hráč" || role == "hrac")
            {
                return "hrac";
            }

            return "host";
        }

        /// <summary>
        /// Zjistí, zda má aktuální uživatel přístup do zvoleného modulu (okna)
        /// </summary>
        /// <param name="modul">Název modulu</param>
        /// <returns>Vrací true, pokud má uživatel přístup. Jinak vrací false</returns>
        private bool MaUzivatelPristup(string modul)
        {
            string role = ZiskejRoliUzivatele();

            if (role == "admin")
            {
                return true;
            }

            if (role == "host")
            {
                if (modul == "Zápasy")
                {
                    return true;
                }

                if (modul == "Kalendář")
                {
                    return true;
                }

                return false;
            }

            if (role == "trener")
            {
                if (modul == "Hráči")
                {
                    return true;
                }

                if (modul == "Trenéři")
                {
                    return true;
                }

                if (modul == "Tréninky")
                {
                    return true;
                }

                if (modul == "Disciplinární opatření")
                {
                    return true;
                }

                if (modul == "Zápasy")
                {
                    return true;
                }

                if (modul == "Nominace")
                {
                    return true;
                }

                if (modul == "Kalendář")
                {
                    return true;
                }

                if (modul == "Notifikace")
                {
                    return true;
                }

                if (modul == "Editor sestav")
                {
                    return true;
                }

                return false;
            }

            if (role == "hrac")
            {
                if (modul == "Hráči")
                {
                    return true;
                }

                if (modul == "Tréninky")
                {
                    return true;
                }

                if (modul == "Zápasy")
                {
                    return true;
                }

                if (modul == "Nominace")
                {
                    return true;
                }

                if (modul == "Kalendář")
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Nastaví vzhled dlaždic podle role aktuálně přihlášeného uživatele
        /// Povolené moduly zůstanou normální, nepovolené se zesvětlí
        /// </summary>
        private void NastavPristupyDlazdic()
        {
            NastavDlazdici(BtnHraci, "Hráči");
            NastavDlazdici(BtnTreneri, "Trenéři");
            NastavDlazdici(BtnTreninky, "Tréninky");
            NastavDlazdici(BtnOpatreni, "Disciplinární opatření");
            NastavDlazdici(BtnZapasy, "Zápasy");
            NastavDlazdici(BtnNominace, "Nominace");
            NastavDlazdici(BtnSportovniVybaveni, "Sportovní vybavení");
            NastavDlazdici(BtnSmlouvy, "Smlouvy");
            NastavDlazdici(BtnKalendar, "Kalendář");
            NastavDlazdici(BtnNastaveni, "Nastavení");
            NastavDlazdici(BtnNotifikace, "Notifikace");
            NastavDlazdici(BtnEditorSestav, "Editor sestav");
        }

        /// <summary>
        /// Nastaví vzhled jedné dlaždice podle oprávnění aktuální role
        /// </summary>
        /// <param name="tlacitko">Tlačítko dlaždice</param>
        /// <param name="modul">Název modulu</param>
        private void NastavDlazdici(Button tlacitko, string modul)
        {
            if (MaUzivatelPristup(modul) == true)
            {
                tlacitko.Opacity = 1.0;
                tlacitko.IsEnabled = true;
            }
            else
            {
                tlacitko.Opacity = 0.45;
                tlacitko.IsEnabled = true;
            }
        }

        /// <summary>
        /// Zobrazí hlášení o zamítnutí přístupu
        /// </summary>
        private void ZobrazZakazanyPristup()
        {
            MessageBox.Show("Tato role nemá přístup", "Přístup odepřen", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

    }
}