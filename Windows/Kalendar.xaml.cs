using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using InformacniSystemKlub.Windows.EditujWindows;
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

            // Načtení tréninků 
            TreninkOkno.NactiTreninky();

            if (TreninkOkno.TreninkyData != null)
            {
                seznamVsechTreninku = TreninkOkno.TreninkyData.ToList();
            }

            AktualizujVsechno();
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
        /// Aktualizuje nadpis měsíce, české formátování nadpisu měsíce
        /// </summary>
        private void AktualizujNadpisMesice()
        {
      
            CultureInfo kultura = new CultureInfo("cs-CZ");

            // Do textové podoby
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
            // Při přepnutí na další měsíc smaže dny z minulého měsíce
            GridDnyKalendare.Children.Clear();

            int pocetPolicek = 42;

            DateTime prvniDenMesice = new DateTime(zobrazenyMesic.Year, zobrazenyMesic.Month, 1);
            int denVTydnu = (int)prvniDenMesice.DayOfWeek;

            // 0 je neděle, proto přepíšeme na pondělí. Kalendář od pondělí do neděle
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

            // Den mimo aktuální měsíc
            if (datumDne.Month != zobrazenyMesic.Month || datumDne.Year != zobrazenyMesic.Year)
            {
                tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(244, 247, 251));
                tlacitkoDne.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
            }

            // Když má den alespoň jeden trénink, změníme barvu
            if (MaDenTrenink(datumDne))
            {
                tlacitkoDne.Background = new SolidColorBrush(Color.FromRgb(255, 237, 213));
                tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(253, 186, 116));
            }

            // Dnešek
            if (datumDne.Date == DateTime.Today.Date)
            {
                tlacitkoDne.BorderBrush = new SolidColorBrush(Color.FromRgb(249, 115, 22));
                tlacitkoDne.BorderThickness = new Thickness(2);
            }

            // Vybraný den
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

                seznamVsechTreninku.Add(novyTrenink);
                AktualizujVsechno();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při přidání tréninku:\n" + ex.Message,
                    "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Upraví vybraný trénink
        /// </summary>
        private void BtnEditovat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LbTreninky.SelectedItem == null)
                {
                    MessageBox.Show("Vyberte trénink k editaci", "Upozornění",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Trenink vybranyTrenink = LbTreninky.SelectedItem as Trenink;

                if (vybranyTrenink == null)
                {
                    MessageBox.Show("Neplatný výběr tréninku", "Chyba",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                EditujTrenink oknoEditujTrenink = new EditujTrenink(vybranyTrenink);
                bool? vysledekDialogu = oknoEditujTrenink.ShowDialog();

                if (vysledekDialogu != true)
                {
                    return;
                }

                OracleConnection pripojeni = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeTreninky.EditujTrenink(pripojeni, vybranyTrenink);

                AktualizujVsechno();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci tréninku:\n" + ex.Message,
                    "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Vyberte trénink k odebrání", "Upozornění",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Trenink vybranyTrenink = LbTreninky.SelectedItem as Trenink;

                if (vybranyTrenink == null)
                {
                    MessageBox.Show("Neplatný výběr tréninku", "Chyba",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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

                Trenink nalezenyTrenink = seznamVsechTreninku.FirstOrDefault(
                    trenink => trenink != null && trenink.Id == vybranyTrenink.Id);

                if (nalezenyTrenink != null)
                {
                    seznamVsechTreninku.Remove(nalezenyTrenink);
                }

                AktualizujVsechno();
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