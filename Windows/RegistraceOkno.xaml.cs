using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using InformacniSystemKlub.Database;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Okno pro registraci uživatele, získane roli Host, kterou může změnit dále Admin
    /// Zajišťuje validaci vstupů, hashování hesla a zápis do databáze Oracle
    /// </summary>
    public partial class RegistracniOkno : Window
    {
        public RegistracniOkno()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Kliknutí na tlačítko "Registrovat"
        /// Provede validaci vstupů, kontrolu duplicit, zahashování hesla a uložení do DB
        /// </summary>
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validace hodnot
                string uzivatelskeJmeno = InputValidator.ValidujUzivatelskeJmeno(txtUser.Text);
                string email = InputValidator.ValidujEmail(txtEmail.Text);
                string heslo = InputValidator.ValidujHeslo(txtPass.Password);
                string heslo2 = InputValidator.ValidujHeslo(txtPass2.Password);

                // Kontrola, že obě hesla jsou stejná
                InputValidator.ValidujShoduHesel(heslo, heslo2);

                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                if ((conn == null) || (conn.State != ConnectionState.Open))
                {
                    throw new Exception("Databázové připojení není dostupné");
                }

                // Kontrola, zda už existuje uživatelské jméno nebo e-mail
                bool existuje = DatabazeUzivatelskeUcty.ExistujeUzivatel(conn, uzivatelskeJmeno, email);

                if (existuje == true)
                {
                    MessageBox.Show("Uživatelské jméno nebo e-mail už existuje", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Vytvoření salt + hash
                string salt = PasswordHasher.GenerateSalt();
                string hash = PasswordHasher.HashPassword(heslo, salt);

                // Vložení účtu do DB jako Host
                DatabazeUzivatelskeUcty.PridejUcetHost(conn, uzivatelskeJmeno, email, hash, salt);

                MessageBox.Show("Registrace proběhla úspěšně", "OK", MessageBoxButton.OK, MessageBoxImage.Information);

                // Přesměrování na přihlašovací okno
                PrihlasovaciOkno prihlasovaciOkno = new PrihlasovaciOkno();
                Application.Current.MainWindow = prihlasovaciOkno;
                prihlasovaciOkno.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Kliknutí na tlačítko "Zpět na přihlášení"
        /// Otevře přihlašovací okno a zavře aktuální
        /// </summary>
        private void BtnZpet_Click(object sender, RoutedEventArgs e)
        {
            PrihlasovaciOkno prihlasovaciOkno = new PrihlasovaciOkno();
            Application.Current.MainWindow = prihlasovaciOkno;
            prihlasovaciOkno.Show();
            this.Close();
        }

        /// <summary>
        /// Zavře celou aplikaci (křížek)
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Minimalizuje okno (tlačítko –)
        /// </summary>
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Umožní tahat okno myší (protože WindowStyle=None)
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}