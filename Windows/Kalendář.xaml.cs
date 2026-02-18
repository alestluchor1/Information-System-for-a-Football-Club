using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows
{
    public partial class Kalendář : Window
    {
        private DateTime zobrazenyMesic;
        private DateTime vybranyDen;

        public Kalendář()
        {
            InitializeComponent();

            zobrazenyMesic = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            vybranyDen = DateTime.Today;

            NastavHeaderMesice();
            NastavVybranyDenText(vybranyDen);
            VykresliMesic();
            NactiTreninkyProDen(vybranyDen);
        }

        private void Root_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            zobrazenyMesic = zobrazenyMesic.AddMonths(-1);
            NastavHeaderMesice();
            VykresliMesic();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            zobrazenyMesic = zobrazenyMesic.AddMonths(1);
            NastavHeaderMesice();
            VykresliMesic();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NastavHeaderMesice()
        {
            CultureInfo cz = new CultureInfo("cs-CZ");
            string mesic = cz.DateTimeFormat.GetMonthName(zobrazenyMesic.Month);
            TxtMesic.Text = mesic + " " + zobrazenyMesic.Year.ToString();
        }

        private void NastavVybranyDenText(DateTime datum)
        {
            CultureInfo cz = new CultureInfo("cs-CZ");
            TxtVybranyDen.Text = datum.ToString("dddd dd.MM.yyyy", cz);
        }

        private void VykresliMesic()
        {
            UgDays.Children.Clear();

            DateTime prvniDen = new DateTime(zobrazenyMesic.Year, zobrazenyMesic.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(zobrazenyMesic.Year, zobrazenyMesic.Month);

            // chceme aby pondělí bylo první (Po=0)
            int startOffset = (int)prvniDen.DayOfWeek;
            if (startOffset == 0) startOffset = 7; // neděle
            startOffset = startOffset - 1;

            // prázdná místa před 1. dnem
            for (int i = 0; i < startOffset; i++)
            {
                Border placeholder = new Border();
                UgDays.Children.Add(placeholder);
            }

            // dny
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime datum = new DateTime(zobrazenyMesic.Year, zobrazenyMesic.Month, day);

                Button b = new Button();
                b.Style = (Style)FindResource("DayBtn");
                b.Content = day.ToString();
                b.Tag = datum;
                b.Click += Day_Click;

                // dneska zvýraznit
                if (datum.Date == DateTime.Today.Date)
                {
                    b.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xEE, 0xF2)); // jemná růžová
                }

                // vybraný den zvýraznit
                if (datum.Date == vybranyDen.Date)
                {
                    b.Background = new SolidColorBrush(Color.FromRgb(0xC7, 0x3F, 0x69));
                    b.Foreground = Brushes.White;
                }

                UgDays.Children.Add(b);
            }
        }

        private void Day_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;

            DateTime datum = (DateTime)b.Tag;

            vybranyDen = datum;
            NastavVybranyDenText(vybranyDen);

            VykresliMesic();
            NactiTreninkyProDen(vybranyDen);
        }

        private void NactiTreninkyProDen(DateTime datum)
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                //List<Trenink> data = DatabazeTreninky.GetTreninkyByDate(conn, datum);

               // LbTreninky.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání tréninků:\n" + ex.Message,
                    "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPridat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Trenink novy = new Trenink();
                novy.Datum = vybranyDen;

                // otevře tvoje okno PridejTrenink (tam vybereš trenéra, místo, popis)
                var okno = new InformacniSystemKlub.Windows.PridejWindows.PridejTrenink(novy);
                bool? result = okno.ShowDialog();

                if (result == true)
                {
                    var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                    //DatabazeTreninky.AddTrenink(conn, novy);

                    NactiTreninkyProDen(vybranyDen);
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze:\n" + ex.Message, "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba:\n" + ex.Message, "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOdebrat_Click(object sender, RoutedEventArgs e)
        {
            Trenink vybrany = LbTreninky.SelectedItem as Trenink;
            if (vybrany == null)
            {
                MessageBox.Show("Vyber trénink, který chceš odebrat.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var res = MessageBox.Show("Opravdu chceš odebrat vybraný trénink?",
                "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                //DatabazeTreninky.DeleteTreninkById(conn, vybrany.Id);

                NactiTreninkyProDen(vybranyDen);
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze:\n" + ex.Message, "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba:\n" + ex.Message, "Chyba",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
