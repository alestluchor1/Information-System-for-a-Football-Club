using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.PridejWindows;
using Oracle.ManagedDataAccess.Client;
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

        public Kalendar()
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimKalendarOkno(this, HlavniOkno.jeDarkModeAktivni);
            vybranyDen = DateTime.Today;

            //(Rok, měsíc, den)
            zobrazenyMesic = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            seznamVsechTreninku = new List<Trenink>();
            ObnovTreninkyZDatabaze();
            NastavPravaPodleRole();
            AktualizujVsechno();
        }

        // Nastaví práva tlačítek podle role přihlášeného uživatele
        private void NastavPravaPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (prihlasenyUzivatel == null)
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            if (role == "hrac" || role == "hráč" || role == "host")
            {
                BtnPridat.IsEnabled = false;
                BtnOdebrat.IsEnabled = false;
            }
            else
            {
                BtnPridat.IsEnabled = true;
                BtnOdebrat.IsEnabled = true;
            }
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
        /// Aktualizuje nadpis měsíce, české formátování nadpisu měsíce
        /// </summary>
        private void AktualizujNadpisMesice()
        {
            // české formátování nadpisu měsíce
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

            // Nastaví celkový počet políček kalendáře na 42 (6 řádků × 7 dní)
            int pocetPolicek = 42;

            DateTime prvniDenMesice = new DateTime(zobrazenyMesic.Year, zobrazenyMesic.Month, 1);
            int denVTydnu = (int)prvniDenMesice.DayOfWeek;

            // Pokud je den v týdnu neděle, má hodnotu 0
            if (denVTydnu == 0)
            {
                // Změní neděli z 0 na 7, aby týden začínal pondělím
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
            foreach (Trenink trenink in seznamVsechTreninku)
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

            foreach (Trenink trenink in seznamVsechTreninku)
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
        /// Přidá nový trénink
        /// </summary>
        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Trenink novyTrenink = new Trenink();
                novyTrenink.Datum = vybranyDen;

                PridejTrenink oknoPridejTrenink = new PridejTrenink(novyTrenink);
                bool? vysledekDialogu = oknoPridejTrenink.ShowDialog();

                if (vysledekDialogu != true)
                {
                    return;
                }

                OracleConnection pripojeni = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreninky.PridejTrenink(pripojeni, novyTrenink);

                ObnovTreninkyZDatabaze();
                AktualizujVsechno();

                
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při přidání tréninku:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Odebere vybraný trénink
        /// </summary>
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LbTreninky.SelectedItem == null)
                {
                    MessageBox.Show("Vyberte trénink k odebrání", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Trenink vybranyTrenink = LbTreninky.SelectedItem as Trenink;

                if (vybranyTrenink == null)
                {
                    MessageBox.Show("Neplatný výběr tréninku", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBoxResult potvrzeniSmazani = MessageBox.Show(
                    "Opravdu chcete odebrat vybraný trénink?",
                    "Potvrzení",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (potvrzeniSmazani != MessageBoxResult.Yes)
                {
                    return;
                }

                OracleConnection pripojeni = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreninky.OdeberTrenink(pripojeni, vybranyTrenink);

                ObnovTreninkyZDatabaze();
                AktualizujVsechno();

                MessageBox.Show("Trénink byl úspěšně odebrán", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při odebírání tréninku:\n" + ex.Message,
                    "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}