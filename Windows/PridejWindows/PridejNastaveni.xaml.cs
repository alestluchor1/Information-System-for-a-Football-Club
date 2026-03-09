using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového uživatelského účtu
    /// Provede validaci vstupů, podle role ověří člena klubu a po uložení naplní objekt <see cref="Uzivatel"/>
    /// </summary>
    public partial class PridejNastaveni : Window
    {
        private Uzivatel novyUzivatel;

        public PridejNastaveni(Uzivatel uzivatel)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            novyUzivatel = uzivatel;

            if (novyUzivatel == null)
            {
                novyUzivatel = new Uzivatel();
            }

            CbRole.SelectedIndex = 0;
            AktualizujViditelnostIdClena();
        }

        /// <summary>
        /// Obsluha změny vybrané role
        /// Podle role zobrazí nebo skryje pole pro ID člena klubu
        /// </summary>
        private void CbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AktualizujViditelnostIdClena();
        }

        /// <summary>
        /// Aktualizuje viditelnost pole pro ID člena klubu podle vybrané role
        /// </summary>
        private void AktualizujViditelnostIdClena()
        {
            if (CbRole == null)
            {
                return;
            }

            if (LblIdClenaKlubu == null)
            {
                return;
            }

            if (TxtIdClenKlubu == null)
            {
                return;
            }

            ComboBoxItem vybranaRole = CbRole.SelectedItem as ComboBoxItem;
            if (vybranaRole == null)
            {
                return;
            }

            int idRole = Convert.ToInt32(vybranaRole.Tag);

            if (idRole == 3 || idRole == 4)
            {
                LblIdClenaKlubu.Visibility = Visibility.Visible;
                TxtIdClenKlubu.Visibility = Visibility.Visible;
            }
            else
            {
                LblIdClenaKlubu.Visibility = Visibility.Collapsed;
                TxtIdClenKlubu.Visibility = Visibility.Collapsed;
                TxtIdClenKlubu.Text = "";
            }
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, podle role ověří člena klubu a uloží hodnoty do objektu <see cref="Uzivatel"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string uzivatelskeJmeno = InputValidator.ValidujUzivatelskeJmeno(TxtUzivatelskeJmeno.Text);
                string email = InputValidator.ValidujEmail(TxtEmail.Text);

                string heslo = InputValidator.ValidujHeslo(PbHeslo.Password);
                string heslo2 = InputValidator.ValidujHeslo(PbHesloZnovu.Password);
                InputValidator.ValidujShoduHesel(heslo, heslo2);

                InputValidator.ValidujVyberRole(CbRole.SelectedItem);

                ComboBoxItem vybranaRole = CbRole.SelectedItem as ComboBoxItem;
                if (vybranaRole == null)
                {
                    throw new Exception("Vyberte roli");
                }

                int idRole = Convert.ToInt32(vybranaRole.Tag);
                string nazevRole = vybranaRole.Content.ToString();

                int idClenKlubu = 0;

                if (nazevRole == "Hrac" || nazevRole == "Trener")
                {
                    idClenKlubu = InputValidator.ValidujIdClenaKlubuProRoli(TxtIdClenKlubu.Text, nazevRole);

                    var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                    string typClena = DatabazeUzivatelskeUcty.GetTypClena(conn, idClenKlubu);

                    if (typClena == null)
                    {
                        throw new Exception("Zadané ID člena klubu neexistuje!");
                    }

                    if (nazevRole == "Hrac")
                    {
                        if (typClena != "Hrac")
                        {
                            throw new Exception("Zadané ID člena klubu nepatří hráči!");
                        }
                    }

                    if (nazevRole == "Trener")
                    {
                        if (typClena != "Trener")
                        {
                            throw new Exception("Zadané ID člena klubu nepatří trenérovi!");
                        }
                    }

                    bool existujeUcet = DatabazeUzivatelskeUcty.ExistujeUcetProClena(conn, idClenKlubu);
                    if (existujeUcet == true)
                    {
                        throw new Exception("Pro zadaného člena klubu už existuje uživatelský účet!");
                    }
                }

                string salt = PasswordHasher.GenerateSalt();
                string hashHesla = PasswordHasher.HashPassword(heslo, salt);

                novyUzivatel.UzivatelskeJmeno = uzivatelskeJmeno;
                novyUzivatel.Email = email;
                novyUzivatel.Heslo = hashHesla;
                novyUzivatel.Salt = salt;
                novyUzivatel.IdRole = idRole;
                novyUzivatel.Role = nazevRole;
                novyUzivatel.IdClenKlubu = idClenKlubu;

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Nastaví zvolený režim (dark/light)
        /// </summary>
        private void ApplyTheme(bool darkMode)
        {
            if (darkMode == true)
            {
                Resources["BgApp"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x0F, 0x19));
                Resources["Surface"] = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
                Resources["TextMain"] = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB));
                Resources["BorderSoft"] = new SolidColorBrush(Color.FromRgb(0x1F, 0x29, 0x37));
                Resources["InputBg"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));
                Resources["Orange"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                Resources["OrangeHover"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));
                Resources["SidebarGradient"] = new LinearGradientBrush(
                    Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16),
                    90);
            }
            else
            {
                Resources["BgApp"] = new SolidColorBrush(Color.FromRgb(0xF6, 0xF7, 0xFB));
                Resources["Surface"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                Resources["TextMain"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));
                Resources["BorderSoft"] = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));
                Resources["InputBg"] = new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC));
                Resources["Orange"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                Resources["OrangeHover"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));
                Resources["SidebarGradient"] = new LinearGradientBrush(
                    Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16),
                    90);
            }
        }
    }
}