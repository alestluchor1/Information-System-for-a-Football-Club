using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Okno kalendáře tréninků
    /// Slouží k zobrazení tréninků v kalendářním přehledu
    /// také podle tréninkové skupiny dostupné pro právě přihlášeného uživatele
    /// </summary>
    public partial class Kalendar : Window
    {
        /// <summary>
        /// Aktuálně vybraný den v kalendáři
        /// </summary>
        private DateTime vybranyDen;

        /// <summary>
        /// Měsíc, který je právě zobrazen v kalendáři
        /// </summary>
        private DateTime zobrazenyMesic;

        /// <summary>
        /// Seznam všech načtených tréninků z databáze
        /// </summary>
        private List<Trenink> seznamVsechTreninku;

        /// <summary>
        /// Určuje, zda už proběhla úvodní inicializace okna
        /// Používá se hlavně proto, aby se některé události nespouštěly příliš brzy
        /// </summary>
        private bool inicializaceDokoncena = false;

        public Kalendar()
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimKalendarOkno(this, HlavniOkno.jeDarkModeAktivni);

            // Jako vybraný den se použije dnešek
            vybranyDen = DateTime.Today;

            // Výchozí zobrazený měsíc je aktuální měsíc
            zobrazenyMesic = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // Připraví prázdný seznam, do kterého se následně načtou tréninky z databáze
            seznamVsechTreninku = new List<Trenink>();

            // Během inicializace nechceme spouštět některé reakce na změny prvků
            inicializaceDokoncena = false;

            // Načte tréninky z databáze do lokálního seznamu
            ObnovTreninkyZDatabaze();

            // Načte data o trenérech a hráčích
            NaplnPomocnaData();

            // Naplní ComboBox dostupnými skupinami podle role uživatele
            NaplnComboBoxSkupin();

            // Nastaví výchozí vybranou skupinu a případně uzamkne ComboBox podle role
            NastavFiltrSkupinyPodleRole();

            // Překreslí celé okno kalendáře podle načtených dat
            AktualizujVsechno();

            // Od této chvíle je okno plně připravené k použití
            inicializaceDokoncena = true;
        }

        /// <summary>
        /// Načte pomocná data potřebná pro práci s rolemi uživatelů a skupinami
        /// Pokud ještě nejsou načteni trenéři nebo hráči, načtou se z databáze
        /// </summary>
        private void NaplnPomocnaData()
        {
            if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
            {
                TreneriOkno.NactiTreneri();
            }

            if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
            {
                HraciOkno.NactiHrace();
            }
        }

        /// <summary>
        /// Naplní ComboBox tréninkových skupin podle role přihlášeného uživatele
        /// </summary>
        private void NaplnComboBoxSkupin()
        {
            // Nejprve se smažou staré položky, aby se seznam nepřidával opakovaně
            cbSkupina.Items.Clear();

            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            // Pokud je uživatel přihlášen a má roli, rozhoduje se podle ní
            if (prihlasenyUzivatel != null && !string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                string role = prihlasenyUzivatel.Role.Trim().ToLower();

                if (role == "admin")
                {
                    cbSkupina.Items.Add("A");
                    cbSkupina.Items.Add("Dorost");
                    cbSkupina.Items.Add("Mladší žáci");
                    return;
                }
            }
            cbSkupina.Items.Add("Všechny skupiny");
            cbSkupina.Items.Add("A");
            cbSkupina.Items.Add("Dorost");
            cbSkupina.Items.Add("Mladší žáci");
        }

        /// <summary>
        /// Nastaví výchozí filtr tréninkové skupiny podle role přihlášeného uživatele
        /// Admin si může skupinu měnit ručně
        /// Trenér a hráč mají skupinu automaticky nastavenou podle svého účtu a nemohou ji měnit
        /// </summary>
        private void NastavFiltrSkupinyPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                if (cbSkupina.Items.Count > 0)
                {
                    cbSkupina.SelectedIndex = 0;
                }

                cbSkupina.IsEnabled = false;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "admin")
            {
                if (cbSkupina.Items.Count > 0)
                {
                    cbSkupina.SelectedIndex = 0;
                }
                cbSkupina.IsEnabled = true;
                return;
            }

            if (role == "trener" || role == "trenér" || role == "hrac" || role == "hráč")
            {
                string nazevSkupiny = ZiskejNazevSkupinyPrihlasenehoUzivatele();

                if (!string.IsNullOrWhiteSpace(nazevSkupiny))
                {
                    bool nalezeno = false;

                    foreach (object polozka in cbSkupina.Items)
                    {
                        if (polozka == null)
                        {
                            continue;
                        }


                        string text = polozka.ToString();

                        if (string.Equals(text.Trim(), nazevSkupiny.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            cbSkupina.SelectedItem = polozka;
                            nalezeno = true;
                            break;
                        }
                    }

                    // Když se skupina nenašla, vybere se první položka jako záloha
                    if (nalezeno == false && cbSkupina.Items.Count > 0)
                    {
                        cbSkupina.SelectedIndex = 0;
                    }
                }
                else
                {
                    // Když se skupina uživatele nepodaří zjistit, vybere se první položka
                    if (cbSkupina.Items.Count > 0)
                    {
                        cbSkupina.SelectedIndex = 0;
                    }
                }

                // Trenér ani hráč nemohou skupinu ručně měnit
                cbSkupina.IsEnabled = false;
                return;
            }

            // Pro ostatní role se nastaví první položka a výběr se zakáže
            if (cbSkupina.Items.Count > 0)
            {
                cbSkupina.SelectedIndex = 0;
            }

            cbSkupina.IsEnabled = false;
        }

        /// <summary>
        /// Vrátí název tréninkové skupiny, do které patří právě přihlášený uživatel
        /// Nejprve se zkouší načíst přímo z objektu uživatele,
        /// a když tam není, dohledává se podle role v seznamu trenérů nebo hráčů
        /// </summary>
        private string ZiskejNazevSkupinyPrihlasenehoUzivatele()
        {
            // Získá se přihlášený uživatel
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            // Když nikdo není přihlášen, není možné skupinu určit
            if (prihlasenyUzivatel == null)
            {
                return "";
            }

            string role = "";

            if (!string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                role = prihlasenyUzivatel.Role.Trim().ToLower();
            }

            if (prihlasenyUzivatel.IdTreninkovaSkupina > 0)
            {
                return PrevedIdSkupinyNaNazev(prihlasenyUzivatel.IdTreninkovaSkupina);
            }

            if ((role == "trener" || role == "trenér") && prihlasenyUzivatel.IdClenKlubu > 0)
            {
                if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
                {
                    TreneriOkno.NactiTreneri();
                }

                // Hledá se trenér odpovídající přihlášenému uživateli
                foreach (Trener trener in TreneriOkno.TreneriData)
                {
                    if (trener.Id == prihlasenyUzivatel.IdClenKlubu)
                    {
                        if (trener.IdTreninkovaSkupina > 0)
                        {
                            return PrevedIdSkupinyNaNazev(trener.IdTreninkovaSkupina);
                        }

                        if (!string.IsNullOrWhiteSpace(trener.NazevSkupiny))
                        {
                            return NormalizujNazevSkupiny(trener.NazevSkupiny);
                        }
                    }
                }
            }

            // Pokud je přihlášený uživatel hráč, dohledá se jeho skupina v seznamu hráčů
            if ((role == "hrac" || role == "hráč") && prihlasenyUzivatel.IdClenKlubu > 0)
            {
                if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
                {
                    HraciOkno.NactiHrace();
                }

                // Hledá se hráč odpovídající přihlášenému uživateli
                foreach (Hrac hrac in HraciOkno.HraciData)
                {
                    if (hrac.Id == prihlasenyUzivatel.IdClenKlubu)
                    {
                        // Když má hráč uložené ID skupiny, vrátí se název podle ID
                        if (hrac.IdTreninkovaSkupina > 0)
                        {
                            return PrevedIdSkupinyNaNazev(hrac.IdTreninkovaSkupina);
                        }

                        if (!string.IsNullOrWhiteSpace(hrac.TreninkovaSkupinaText))
                        {
                            return NormalizujNazevSkupiny(hrac.TreninkovaSkupinaText);
                        }
                    }
                }
            }

            // Když se skupinu nepodaří zjistit, vrátí se prázdný text
            return "";
        }

        /// <summary>
        /// Převede číselné ID tréninkové skupiny na její textový název
        /// </summary>
        private string PrevedIdSkupinyNaNazev(int idSkupiny)
        {
            // ID 1 odpovídá skupině A
            if (idSkupiny == 1)
            {
                return "A";
            }

            // ID 2 odpovídá skupině Dorost
            if (idSkupiny == 2)
            {
                return "Dorost";
            }

            // ID 3 odpovídá skupině Mladší žáci
            if (idSkupiny == 3)
            {
                return "Mladší žáci";
            }

            return "";
        }

        /// <summary>
        /// Upraví název skupiny do jednotného tvaru,
        /// aby šlo různé zápisy správně porovnávat mezi sebou
        /// Například "A-Tým" a "A-tým" převede na "A"
        /// </summary>
        private string NormalizujNazevSkupiny(string nazevSkupiny)
        {
            if (string.IsNullOrWhiteSpace(nazevSkupiny))
            {
                return "";
            }

            // Odstraní se nadbytečné mezery na začátku a konci
            string normalizovanyNazev = nazevSkupiny.Trim();

            if (string.Equals(normalizovanyNazev, "A-tým", StringComparison.OrdinalIgnoreCase))
            {
                return "A";
            }

            if (string.Equals(normalizovanyNazev, "A-Tým", StringComparison.OrdinalIgnoreCase))
            {
                return "A";
            }

            // Ostatní názvy skupin se vracejí beze změny.
            return normalizovanyNazev;
        }

        /// <summary>
        /// Vrátí seznam tréninků podle právě vybrané skupiny v ComboBoxu
        /// Když není vybrána konkrétní skupina nebo je zvolena možnost „Všechny skupiny“,
        /// vrací všechny načtené tréninky
        /// </summary>
        private List<Trenink> ZiskejTreninkyPodleAktualniSkupiny()
        {
            // Připraví se výsledný seznam tréninků po filtrování
            List<Trenink> vysledek = new List<Trenink>();

            string vybranaSkupina = "";

            // Když je v ComboBoxu něco vybrané, převede se výběr na text
            if (cbSkupina.SelectedItem != null)
            {
                vybranaSkupina = cbSkupina.SelectedItem.ToString();
            }

            if (string.IsNullOrWhiteSpace(vybranaSkupina) || vybranaSkupina == "Všechny skupiny")
            {
                return seznamVsechTreninku.ToList();
            }

            // Projdou se všechny načtené tréninky a vyberou se jen ty,
            // které patří do právě zvolené skupiny
            foreach (Trenink trenink in seznamVsechTreninku)
            {
                if (trenink == null)
                {
                    continue;
                }

                string nazevSkupiny = NormalizujNazevSkupiny(trenink.NazevSkupiny);

                if (!string.IsNullOrWhiteSpace(nazevSkupiny) &&
                    string.Equals(nazevSkupiny.Trim(), vybranaSkupina.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    vysledek.Add(trenink);
                }
            }

            // Vrátí se vyfiltrovaný seznam tréninků
            return vysledek;
        }

        /// <summary>
        /// Obnoví všechny hlavní části okna kalendáře
        /// Přepočítá nadpis měsíce, text vybraného dne,
        /// překreslí kalendář a znovu zobrazí tréninky pro zvolený den
        /// </summary>
        private void AktualizujVsechno()
        {
            AktualizujNadpisMesice();
            AktualizujTextVybranehoDne();
            VykresliDnyKalendare();
            ZobrazTreninkyProVybranyDen();
        }

        /// <summary>
        /// Načte znovu všechny tréninky z databáze
        /// a uloží je do lokálního seznamu používaného v kalendáři
        /// </summary>
        private void ObnovTreninkyZDatabaze()
        {
            TreninkOkno.NactiTreninky();

            seznamVsechTreninku = new List<Trenink>();

            if (TreninkOkno.TreninkyData != null)
            {
                seznamVsechTreninku = TreninkOkno.TreninkyData.ToList();
            }
        }

        /// <summary>
        /// Aktualizuje text v záhlaví kalendáře,
        /// aby odpovídal právě zobrazenému měsíci a roku
        /// </summary>
        private void AktualizujNadpisMesice()
        {
            CultureInfo kultura = new CultureInfo("cs-CZ");

            string textMesice = zobrazenyMesic.ToString("MMMM yyyy", kultura);

            TxtMesicRok.Text = kultura.TextInfo.ToTitleCase(textMesice);
        }

        /// <summary>
        /// Aktualizuje text s právě vybraným dnem v pravé části okna
        /// </summary>
        private void AktualizujTextVybranehoDne()
        {
            // kvůli správnému názvu dne v týdnu.
            CultureInfo kultura = new CultureInfo("cs-CZ");

            TxtVybranyDen.Text = vybranyDen.ToString("dddd d. M. yyyy", kultura);
        }

        /// <summary>
        /// Překreslí celý měsíční kalendář včetně všech dnů
        /// Vždy vytvoří 42 políček, aby kalendář měl mřížku 6 × 7
        /// </summary>
        private void VykresliDnyKalendare()
        {
            GridDnyKalendare.Children.Clear();

            int pocetPolicek = 42;

            DateTime prvniDenMesice = new DateTime(zobrazenyMesic.Year, zobrazenyMesic.Month, 1);

            // Získá se den v týdnu pro první den měsíce
            int denVTydnu = (int)prvniDenMesice.DayOfWeek;

            // V .NET je neděle 0, ale pro tento kalendář chceme,
            // aby týden začínal pondělím a neděle byla 7
            if (denVTydnu == 0)
            {
                denVTydnu = 7;
            }

            int posunNaZacatek = denVTydnu - 1;

            DateTime prvniZobrazeneDatum = prvniDenMesice.AddDays(-posunNaZacatek);

            // Vytvoří se všech tlačítek dnů.
            for (int i = 0; i < pocetPolicek; i++)
            {
                DateTime datum = prvniZobrazeneDatum.AddDays(i);

                Button tlacitkoDne = VytvorTlacitkoDne(datum);

                GridDnyKalendare.Children.Add(tlacitkoDne);
            }
        }

        /// <summary>
        /// Vytvoří jedno tlačítko představující konkrétní den v kalendáři
        /// Podle stavu dne nastaví jeho vzhled, například zvýraznění dneška,
        /// vybraného dne nebo dne, ve kterém existuje trénink
        /// </summary>
        private Button VytvorTlacitkoDne(DateTime datumDne)
        {
            Button tlacitkoDne = new Button();
            tlacitkoDne.Margin = new Thickness(4);
            tlacitkoDne.Padding = new Thickness(8);
            tlacitkoDne.BorderThickness = new Thickness(1);
            tlacitkoDne.Cursor = System.Windows.Input.Cursors.Hand;

            tlacitkoDne.Tag = datumDne;

            // Připojí obsluhu kliknutí na den v kalendáři
            tlacitkoDne.Click += DenKalendare_Click;

            tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(229, 231, 235));
            tlacitkoDne.Foreground = new SolidColorBrush(Color.FromRgb(11, 18, 32));

            // Dny mimo právě zobrazený měsíc se zobrazí světlejší barvou
            if (datumDne.Month != zobrazenyMesic.Month || datumDne.Year != zobrazenyMesic.Year)
            {
                tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(244, 247, 251));
                tlacitkoDne.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
            }

            // Pokud má tento den alespoň jeden trénink,
            if (MaDenTrenink(datumDne))
            {
                tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(255, 237, 213));
                tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(253, 186, 116));
            }

            // Dnešní datum dostane výraznější orámování
            if (datumDne.Date == DateTime.Today.Date)
            {
                tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(249, 115, 22));
                tlacitkoDne.BorderThickness = new Thickness(2);
            }

            // Aktuálně vybraný den dostane plné oranžové zvýraznění
            if (datumDne.Date == vybranyDen.Date)
            {
                tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(249, 115, 22));
                tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(249, 115, 22));
                tlacitkoDne.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }

            // Vytvoří text uvnitř tlačítka – číslo dne v měsíci
            TextBlock txtDen = new TextBlock();
            txtDen.Text = datumDne.Day.ToString();
            txtDen.FontSize = 18;
            txtDen.FontWeight = FontWeights.Bold;
            txtDen.HorizontalAlignment = HorizontalAlignment.Left;
            txtDen.VerticalAlignment = VerticalAlignment.Top;
            txtDen.Foreground = tlacitkoDne.Foreground;

            tlacitkoDne.Content = txtDen;

            return tlacitkoDne;
        }

        /// <summary>
        /// Zjistí, zda pro zadaný den existuje alespoň jeden trénink
        /// v aktuálně vybrané nebo povolené skupině
        /// </summary>
        private bool MaDenTrenink(DateTime datumDne)
        {
            // Projde všechny tréninky, které odpovídají aktuálnímu filtru skupiny.
            foreach (Trenink trenink in ZiskejTreninkyPodleAktualniSkupiny())
            {
                // Ochrana proti prázdnému záznamu
                if (trenink == null)
                {
                    continue;
                }

                if (trenink.Datum == DateTime.MinValue)
                {
                    continue;
                }

                if (trenink.Datum.Date == datumDne.Date)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Obsluha kliknutí na den v kalendáři
        /// Nastaví nový vybraný den, případně upraví zobrazený měsíc,
        /// a následně překreslí kalendář i seznam tréninků
        /// </summary>
        private void DenKalendare_Click(object sender, RoutedEventArgs e)
        {
            Button kliknuteTlacitko = sender as Button;

            if (kliknuteTlacitko == null)
            {
                return;
            }

            if (kliknuteTlacitko.Tag is DateTime)
            {
                DateTime noveDatum = (DateTime)kliknuteTlacitko.Tag;

                vybranyDen = noveDatum.Date;

                zobrazenyMesic = new DateTime(noveDatum.Year, noveDatum.Month, 1);

                AktualizujVsechno();
            }
        }

        /// <summary>
        /// Přepne kalendář na předchozí měsíc
        /// a překreslí zobrazení dnů
        /// </summary>
        private void BtnPredchoziMesic_Click(object sender, RoutedEventArgs e)
        {
            zobrazenyMesic = zobrazenyMesic.AddMonths(-1);

            AktualizujNadpisMesice();

            VykresliDnyKalendare();
        }

        /// <summary>
        /// Přepne kalendář na další měsíc
        /// a překreslí zobrazení dnů
        /// </summary>
        private void BtnDalsiMesic_Click(object sender, RoutedEventArgs e)
        {
            zobrazenyMesic = zobrazenyMesic.AddMonths(1);

            AktualizujNadpisMesice();

            VykresliDnyKalendare();
        }

        /// <summary>
        /// Zobrazí v pravé části okna všechny tréninky,
        /// které patří do právě vybraného dne a aktuálně zvolené skupiny
        /// </summary>
        private void ZobrazTreninkyProVybranyDen()
        {
            List<Trenink> treninkyProVybranyDen = new List<Trenink>();

            // Projdou se tréninky odpovídající aktuální skupině
            foreach (Trenink trenink in ZiskejTreninkyPodleAktualniSkupiny())
            {
                if (trenink == null)
                {
                    continue;
                }

                if (trenink.Datum == DateTime.MinValue)
                {
                    continue;
                }

                if (trenink.Datum.Date == vybranyDen.Date)
                {
                    treninkyProVybranyDen.Add(trenink);
                }
            }

            // Tréninky se seřadí podle data
            treninkyProVybranyDen = treninkyProVybranyDen
                .OrderBy(trenink => trenink.Datum)
                .ToList();

            LbTreninky.ItemsSource = null;
            LbTreninky.ItemsSource = treninkyProVybranyDen;
        }

        /// <summary>
        /// Reaguje na změnu vybrané skupiny v ComboBoxu
        /// Po dokončení inicializace znovu překreslí kalendář i seznam tréninků
        /// </summary>
        private void cbSkupina_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (inicializaceDokoncena == false)
            {
                return;
            }

            // Po změně skupiny se obnoví celé zobrazení
            AktualizujVsechno();
        }
    }
}