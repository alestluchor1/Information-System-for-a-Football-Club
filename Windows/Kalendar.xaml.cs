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
    /// </summary>
    public partial class Kalendar : Window
    {
        private DateTime vybranyDen;
        private DateTime zobrazenyMesic;
        private List<Trenink> seznamVsechTreninku;
        private bool inicializaceDokoncena = false;

        public Kalendar()
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimKalendarOkno(this, HlavniOkno.jeDarkModeAktivni);

            vybranyDen = DateTime.Today;
            zobrazenyMesic = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            seznamVsechTreninku = new List<Trenink>();

            inicializaceDokoncena = false;

            ObnovTreninkyZDatabaze();
            NaplnPomocnaData();
            NaplnComboBoxSkupin();
            NastavFiltrSkupinyPodleRole();
            AktualizujVsechno();

            inicializaceDokoncena = true;
        }

        /// <summary>
        /// Načte pomocná data pro dohledání skupin trenérů/hráčů
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
        /// Naplní ComboBox tréninkových skupin podle role
        /// </summary>
        private void NaplnComboBoxSkupin()
        {
            cbSkupina.Items.Clear();

            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

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
        /// Nastaví výchozí filtr skupiny podle role přihlášeného uživatele
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

                    if (nalezeno == false && cbSkupina.Items.Count > 0)
                    {
                        cbSkupina.SelectedIndex = 0;
                    }
                }
                else
                {
                    if (cbSkupina.Items.Count > 0)
                    {
                        cbSkupina.SelectedIndex = 0;
                    }
                }

                cbSkupina.IsEnabled = false;
                return;
            }

            if (cbSkupina.Items.Count > 0)
            {
                cbSkupina.SelectedIndex = 0;
            }

            cbSkupina.IsEnabled = false;
        }

        /// <summary>
        /// Vrátí název skupiny přihlášeného uživatele
        /// </summary>
        private string ZiskejNazevSkupinyPrihlasenehoUzivatele()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

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

            if ((role == "hrac" || role == "hráč") && prihlasenyUzivatel.IdClenKlubu > 0)
            {
                if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
                {
                    HraciOkno.NactiHrace();
                }

                foreach (Hrac hrac in HraciOkno.HraciData)
                {
                    if (hrac.Id == prihlasenyUzivatel.IdClenKlubu)
                    {
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

            return "";
        }

        /// <summary>
        /// Převede ID skupiny na název skupiny
        /// </summary>
        private string PrevedIdSkupinyNaNazev(int idSkupiny)
        {
            if (idSkupiny == 1)
            {
                return "A";
            }

            if (idSkupiny == 2)
            {
                return "Dorost";
            }

            if (idSkupiny == 3)
            {
                return "Mladší žáci";
            }

            return "";
        }

        /// <summary>
        /// Normalizuje název skupiny pro porovnávání
        /// </summary>
        private string NormalizujNazevSkupiny(string nazevSkupiny)
        {
            if (string.IsNullOrWhiteSpace(nazevSkupiny))
            {
                return "";
            }

            string normalizovanyNazev = nazevSkupiny.Trim();

            if (string.Equals(normalizovanyNazev, "A-tým", StringComparison.OrdinalIgnoreCase))
            {
                return "A";
            }

            if (string.Equals(normalizovanyNazev, "A-Tým", StringComparison.OrdinalIgnoreCase))
            {
                return "A";
            }

            return normalizovanyNazev;
        }

        /// <summary>
        /// Vrátí tréninky podle aktuálně vybrané skupiny
        /// </summary>
        private List<Trenink> ZiskejTreninkyPodleAktualniSkupiny()
        {
            List<Trenink> vysledek = new List<Trenink>();

            string vybranaSkupina = "";

            if (cbSkupina.SelectedItem != null)
            {
                vybranaSkupina = cbSkupina.SelectedItem.ToString();
            }

            if (string.IsNullOrWhiteSpace(vybranaSkupina) || vybranaSkupina == "Všechny skupiny")
            {
                return seznamVsechTreninku.ToList();
            }

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

            return vysledek;
        }

        /// <summary>
        /// Obnoví celé zobrazení kalendáře
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
        /// Aktualizuje nadpis měsíce
        /// </summary>
        private void AktualizujNadpisMesice()
        {
            CultureInfo kultura = new CultureInfo("cs-CZ");
            string textMesice = zobrazenyMesic.ToString("MMMM yyyy", kultura);
            TxtMesicRok.Text = kultura.TextInfo.ToTitleCase(textMesice);
        }

        /// <summary>
        /// Aktualizuje text vybraného dne vpravo
        /// </summary>
        private void AktualizujTextVybranehoDne()
        {
            CultureInfo kultura = new CultureInfo("cs-CZ");
            TxtVybranyDen.Text = vybranyDen.ToString("dddd d. M. yyyy", kultura);
        }

        /// <summary>
        /// Překreslí měsíční kalendář
        /// </summary>
        private void VykresliDnyKalendare()
        {
            GridDnyKalendare.Children.Clear();

            int pocetPolicek = 42;

            DateTime prvniDenMesice = new DateTime(zobrazenyMesic.Year, zobrazenyMesic.Month, 1);
            int denVTydnu = (int)prvniDenMesice.DayOfWeek;

            if (denVTydnu == 0)
            {
                denVTydnu = 7;
            }

            int posunNaZacatek = denVTydnu - 1;
            DateTime prvniZobrazeneDatum = prvniDenMesice.AddDays(-posunNaZacatek);

            for (int i = 0; i < pocetPolicek; i++)
            {
                DateTime datum = prvniZobrazeneDatum.AddDays(i);
                Button tlacitkoDne = VytvorTlacitkoDne(datum);
                GridDnyKalendare.Children.Add(tlacitkoDne);
            }
        }

        /// <summary>
        /// Vytvoří jedno tlačítko dne
        /// </summary>
        private Button VytvorTlacitkoDne(DateTime datumDne)
        {
            Button tlacitkoDne = new Button();
            tlacitkoDne.Margin = new Thickness(4);
            tlacitkoDne.Padding = new Thickness(8);
            tlacitkoDne.BorderThickness = new Thickness(1);
            tlacitkoDne.Cursor = System.Windows.Input.Cursors.Hand;
            tlacitkoDne.Tag = datumDne;
            tlacitkoDne.Click += DenKalendare_Click;

            tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(229, 231, 235));
            tlacitkoDne.Foreground = new SolidColorBrush(Color.FromRgb(11, 18, 32));

            if (datumDne.Month != zobrazenyMesic.Month || datumDne.Year != zobrazenyMesic.Year)
            {
                tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(244, 247, 251));
                tlacitkoDne.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
            }

            if (MaDenTrenink(datumDne))
            {
                tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(255, 237, 213));
                tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(253, 186, 116));
            }

            if (datumDne.Date == DateTime.Today.Date)
            {
                tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(249, 115, 22));
                tlacitkoDne.BorderThickness = new Thickness(2);
            }

            if (datumDne.Date == vybranyDen.Date)
            {
                tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(249, 115, 22));
                tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(249, 115, 22));
                tlacitkoDne.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }

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
        /// Zjistí, jestli pro daný den existuje alespoň jeden trénink
        /// </summary>
        private bool MaDenTrenink(DateTime datumDne)
        {
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

                if (trenink.Datum.Date == datumDne.Date)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Kliknutí na den v kalendáři
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
        /// </summary>
        private void BtnPredchoziMesic_Click(object sender, RoutedEventArgs e)
        {
            zobrazenyMesic = zobrazenyMesic.AddMonths(-1);
            AktualizujNadpisMesice();
            VykresliDnyKalendare();
        }

        /// <summary>
        /// Přepne kalendář na další měsíc
        /// </summary>
        private void BtnDalsiMesic_Click(object sender, RoutedEventArgs e)
        {
            zobrazenyMesic = zobrazenyMesic.AddMonths(1);
            AktualizujNadpisMesice();
            VykresliDnyKalendare();
        }

        /// <summary>
        /// Zobrazí tréninky pro vybraný den
        /// </summary>
        private void ZobrazTreninkyProVybranyDen()
        {
            List<Trenink> treninkyProVybranyDen = new List<Trenink>();

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

            treninkyProVybranyDen = treninkyProVybranyDen
                .OrderBy(trenink => trenink.Datum)
                .ToList();

            LbTreninky.ItemsSource = null;
            LbTreninky.ItemsSource = treninkyProVybranyDen;
        }

        /// <summary>
        /// Obsluha změny skupiny v ComboBoxu
        /// </summary>
        private void cbSkupina_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inicializaceDokoncena == false)
            {
                return;
            }

            AktualizujVsechno();
        }
    }
}