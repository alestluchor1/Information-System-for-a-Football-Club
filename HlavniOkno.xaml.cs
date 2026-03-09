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
    /// přepínání dark/light režimu a otevření jednotlivých oken
    /// </summary>
    public partial class HlavniOkno : Window
    {
        /// <summary>
        /// Časovač
        /// </summary>
        private DispatcherTimer timer;

        /// <summary>
        /// Příznak, zda je v aplikaci aktivní tmavý režim
        /// </summary>
        public static bool jeDarkModeAktivni = false;

        /// <summary>
        /// Aktuálně přihlášený uživatel aplikace
        /// </summary>
        private static Uzivatel prihlasenyUzivatel;

        public HlavniOkno()
        {
            InitializeComponent();
            SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            this.timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            ZobrazPrihlasenehoUzivatele();
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
        /// Událost časovače.
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
                ApplyTheme(jeDarkModeAktivni);
            }
            else
            {
                jeDarkModeAktivni = false;
                ApplyTheme(jeDarkModeAktivni);
            }
        }

        /// <summary>
        /// Otevře okno kalendáře
        /// </summary>
        private void BtnKalendarClick(object sender, RoutedEventArgs e)
        {
            Kalendar kalendar = new Kalendar();
            kalendar.ShowDialog();
        }

        /// <summary>
        /// Otevře okno nominací na zápas
        /// </summary>
        private void BtnNominaceClick(object sender, RoutedEventArgs e)
        {
            NominaceNaZapasOkno nominace = new NominaceNaZapasOkno();
            nominace.ShowDialog();
        }

        /// <summary>
        /// Otevře okno správy zápasů.
        /// </summary>
        private void BtnZapasy_Click(object sender, RoutedEventArgs e)
        {
            ZapasyOkno zapasyOkno = new ZapasyOkno();
            zapasyOkno.ShowDialog();
        }

        /// <summary>
        /// Otevře okno správy sportovního vybavení.
        /// </summary>
        private void BtnSportovniVybaveni_Click(object sender, RoutedEventArgs e)
        {
            SportovniVybaveniOkno vybaveniOkno = new SportovniVybaveniOkno();
            vybaveniOkno.ShowDialog();
        }

        /// <summary>
        /// Otevře okno správy kontraktů.
        /// </summary>
        private void BtnKontraktyClick(object sender, RoutedEventArgs e)
        {
            KontraktyOkno kontraktyOkno = new KontraktyOkno();
            kontraktyOkno.ShowDialog();
        }

        /// <summary>
        /// Otevře okno disciplinárních opatření.
        /// </summary>
        private void BtnOpatreniClick(object sender, RoutedEventArgs e)
        {
            OpatreniOkno opatreniOkno = new OpatreniOkno();
            opatreniOkno.ShowDialog();
        }

        /// <summary>
        /// Otevře okno hráčů.
        /// Hlavní okno se dočasně skryje a po zavření okna hráčů se znovu zobrazí.
        /// </summary>
        private void BtnHraciClick(object sender, RoutedEventArgs e)
        {
            var hraciOkno = new HraciOkno();
            hraciOkno.Owner = this;

            this.Hide();

            hraciOkno.Closed += HraciOkno_Closed;

            hraciOkno.Show();
        }

        /// <summary>
        /// Po zavření okna hráčů znovu zobrazí hlavní okno.
        /// </summary>
        private void HraciOkno_Closed(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        /// <summary>
        /// Otevře okno trenérů.
        /// </summary>
        private void BtnTreneriClick(object sender, RoutedEventArgs e)
        {
            var treneriOkno = new TreneriOkno();
            treneriOkno.Owner = this;

            this.Hide();

            treneriOkno.Closed += TreneriOkno_Closed;

            treneriOkno.Show();
        }

        /// <summary>
        /// Po zavření okna trenérů znovu zobrazí hlavní okno.
        /// </summary>
        private void TreneriOkno_Closed(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        /// <summary>
        /// Otevře okno tréninků.
        /// </summary>
        private void BtnTreninkyClick(object sender, RoutedEventArgs e)
        {
            var treninkOkno = new TreninkOkno();
            treninkOkno.Owner = this;

            this.Hide();

            treninkOkno.Closed += TreninkyOkno_Closed;

            treninkOkno.Show();
        }

        /// <summary>
        /// Po zavření okna tréninků znovu zobrazí hlavní okno.
        /// </summary>
        private void TreninkyOkno_Closed(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        /// <summary>
        /// Pomocné tlačítko pro otevření nastavení.
        /// Pokud zatím okno nastavení neexistuje, lze tuto metodu později upravit.
        /// </summary>
        private void BtnNastaveniClick(object sender, RoutedEventArgs e)
        {
            NastaveniOkno nastaveniOkno = new NastaveniOkno();
            nastaveniOkno.ShowDialog();
        }

        /// <summary>
        /// Pomocné tlačítko pro notifikace
        /// </summary>
        private void BtnNotifikaceClick(object sender, RoutedEventArgs e)
        {
            NastaveniOkno.NactiUzivatele();

            Notifikace notifikace = new Notifikace(NastaveniOkno.UzivateleData);
            notifikace.ShowDialog();
        }

        /// <summary>
        /// Pomocné tlačítko pro editor sestav.
        /// </summary>
        private void BtnEditorSestavClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Editor sestav zatím není připojen.");
        }

        /// <summary>
        /// Vrátí uživatele na přihlašovací obrazovku a ukončí aktuální relaci hlavního okna.
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            PrihlasovaciOkno prihlasovaciOkno = new PrihlasovaciOkno();
            Application.Current.MainWindow = prihlasovaciOkno;
            prihlasovaciOkno.Show();
            this.Close();
        }

        /// <summary>
        /// Nastaví barevné prostředky okna podle zvoleného režimu zobrazení.
        /// </summary>
        private void ApplyTheme(bool darkMode)
        {
            if (darkMode == true)
            {
                Resources["AppBgBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x0F, 0x19));
                Resources["SurfaceBrush"] = new SolidColorBrush(Color.FromRgb(0x0F, 0x17, 0x2A));
                Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0x1F, 0x29, 0x37));
                Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC));
                Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));

                Resources["TopBarText"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                Resources["TopBarSubText"] = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));
            }
            else
            {
                Resources["AppBgBrush"] = new SolidColorBrush(Color.FromRgb(0xF6, 0xF7, 0xFB));
                Resources["SurfaceBrush"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0xE4, 0xEA, 0xF3));
                Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));
                Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B));

                Resources["TopBarText"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                Resources["TopBarSubText"] = new SolidColorBrush(Color.FromRgb(0xEA, 0xF0, 0xFF));
            }
        }
    }
}