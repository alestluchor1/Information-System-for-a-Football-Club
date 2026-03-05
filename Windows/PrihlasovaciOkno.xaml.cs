using System;
using System.Windows;
using System.Windows.Input;
using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Okno pro přihlášení uživatele do systému
    /// Umožňuje přihlášení
    /// </summary>
    public partial class PrihlasovaciOkno : Window
    {
        public PrihlasovaciOkno()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Umožní tažení okna levým tlačítkem myši (WindowStyle=None)
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Minimalizuje aktuální okno
        /// </summary>
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Ukončí aplikaci
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Otevře registrační okno
        /// </summary>
        private void BtnRegisterWindow_Click(object sender, RoutedEventArgs e)
        {
            RegistracniOkno registracniOkno = new RegistracniOkno();
            Application.Current.MainWindow = registracniOkno;
            registracniOkno.Show();
            this.Close();
        }

        /// <summary>
        /// Přihlásí uživatele po zadání uživatelského jména a hesla
        /// Načte login data z DB (hash+salt+role+email), ověří heslo a pustí uživatele do aplikace
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validace vstupů
                string uzivJmeno = InputValidator.ValidujUzivatelskeJmeno(txtUser.Text);
                string heslo = InputValidator.ValidujHeslo(txtPass.Password);

                // Připojení k DB
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                // Načtení login dat 
                string ulozenyHash;
                string ulozenySalt;

                Uzivatel prihlasenyUzivatel = DatabazePrihlaseni.NactiLoginData(conn, uzivJmeno, out ulozenyHash, out ulozenySalt);

                if (prihlasenyUzivatel == null)
                {
                    MessageBox.Show("Uživatel neexistuje!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ověření hesla
                bool hesloOK = PasswordHasher.VerifyPassword(heslo, ulozenyHash, ulozenySalt);

                if (hesloOK == false)
                {
                    MessageBox.Show("Nesprávné heslo!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update čas poslední přihlášení
                DatabazePrihlaseni.UpdatePosledniPrihlaseni(conn, prihlasenyUzivatel.UzivatelskeJmeno);

                MessageBox.Show("Přihlášení úspěšné (" + prihlasenyUzivatel.Role + ")", "OK", MessageBoxButton.OK, MessageBoxImage.Information);

                // HlavniOkno.NastavPrihlaseneho(prihlasenyUzivatel);

                // Otevření hlavního okna
                HlavniOkno hlavni = new HlavniOkno();
                Application.Current.MainWindow = hlavni;
                hlavni.Show();
                this.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Chyba při přihlášení: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}