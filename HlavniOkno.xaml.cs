using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Interakční logika pro HlavniOkno.xaml
    /// </summary>
    public partial class HlavniOkno : Window
    {
        private DispatcherTimer timer;

        public static bool jeDarkModeAktivni = false;

        public HlavniOkno()
        {
            InitializeComponent();
            SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            this.timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TxtTime.Text = DateTime.Now.ToString("HH:mm:ss");
            TxtDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            if (jeDarkModeAktivni == false)
            {
                jeDarkModeAktivni = true;
                ApplyTheme(jeDarkModeAktivni);
            }

            else if (jeDarkModeAktivni == true)
            {
                jeDarkModeAktivni = false;
                ApplyTheme(jeDarkModeAktivni);
            }
            
        }

        private void BtnKalendarClick(object sender, RoutedEventArgs e)
        {
            Kalendář kalendar = new Kalendář();
            kalendar.ShowDialog();

        }

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
            else if(darkMode == false)
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnHraciClick(object sender, RoutedEventArgs e)
        {
            var hraciOkno = new HraciOkno();
            hraciOkno.Owner = this;

            this.Hide();

            hraciOkno.Closed += HraciOkno_Closed;  

            hraciOkno.Show();
        }

        private void HraciOkno_Closed(object? sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        private void BtnTreneriClick(object sender, RoutedEventArgs e)
        {
            var treneriOkno = new TreneriOkno();
            treneriOkno.Owner = this;

            this.Hide();

            treneriOkno.Closed += TreneriOkno_Closed;

            treneriOkno.Show();
        }

        private void TreneriOkno_Closed(object? sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        private void BtnTreninkyClick(object sender, RoutedEventArgs e)
        {
            var treninkOkno = new TreninkOkno();
            treninkOkno.Owner = this;

            this.Hide();

            treninkOkno.Closed += TreninkyOkno_Closed;

            treninkOkno.Show();
        }

        private void TreninkyOkno_Closed(object? sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }



    }
}
