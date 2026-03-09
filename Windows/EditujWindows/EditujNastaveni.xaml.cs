using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného uživatelského účtu
    /// Při otevření se formulář předvyplní a po potvrzení se změny uloží do objektu <see cref="Uzivatel"/>
    /// </summary>
    public partial class EditujNastaveni : Window
    {
        private readonly Uzivatel editovanyUzivatel;

        public EditujNastaveni(Uzivatel uzivatel)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            if (uzivatel == null)
            {
                throw new ArgumentNullException(nameof(uzivatel));
            }

            editovanyUzivatel = uzivatel;

            PredvyplnFormular();
            AktualizujViditelnostIdClena();
        }

        /// <summary>
        /// Předvyplní formulář hodnotami editovaného uživatele
        /// </summary>
        private void PredvyplnFormular()
        {
            TxtUzivatelskeJmeno.Text = editovanyUzivatel.UzivatelskeJmeno;
            TxtEmail.Text = editovanyUzivatel.Email;

            const int roleHost = 1;
            const int roleAdmin = 2;
            const int roleHrac = 3;
            const int roleTrener = 4;

            const int indexAdmin = 0;
            const int indexHost = 1;
            const int indexHrac = 2;
            const int indexTrener = 3;

            if (editovanyUzivatel.IdRole == roleHost)
            {
                CbRole.SelectedIndex = indexHost;
            }
            else if (editovanyUzivatel.IdRole == roleAdmin)
            {
                CbRole.SelectedIndex = indexAdmin;
            }
            else if (editovanyUzivatel.IdRole == roleHrac)
            {
                CbRole.SelectedIndex = indexHrac;
            }
            else if (editovanyUzivatel.IdRole == roleTrener)
            {
                CbRole.SelectedIndex = indexTrener;
            }
            else
            {
                CbRole.SelectedIndex = indexAdmin;
            }

            if (editovanyUzivatel.IdClenKlubu > 0)
            {
                TxtIdClenKlubu.Text = editovanyUzivatel.IdClenKlubu.ToString();
            }
            else
            {
                TxtIdClenKlubu.Text = "";
            }
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

            const int roleHrac = 3;  
            const int roleTrener = 4; 

            int idRole = Convert.ToInt32(vybranaRole.Tag);

            if (idRole == roleHrac || idRole == roleTrener)
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
        /// Provede validaci všech vstupů a uloží upravené hodnoty do objektu <see cref="Uzivatel"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string uzivatelskeJmeno = InputValidator.ValidujUzivatelskeJmeno(TxtUzivatelskeJmeno.Text);
                string email = InputValidator.ValidujEmail(TxtEmail.Text);

                string heslo = PbHeslo.Password;
                string heslo2 = PbHesloZnovu.Password;

                string hashHesla = editovanyUzivatel.Heslo;
                string salt = editovanyUzivatel.Salt;

                bool jeVyplneneHeslo = string.IsNullOrWhiteSpace(heslo) == false;
                bool jeVyplneneHesloZnovu = string.IsNullOrWhiteSpace(heslo2) == false;

                // Obě pole prázdná = heslo se nemění
                if (jeVyplneneHeslo == false && jeVyplneneHesloZnovu == false)
                {
                }
                else
                {
                    // Vyplněné musí být obě pole
                    if (jeVyplneneHeslo == false || jeVyplneneHesloZnovu == false)
                    {
                        throw new Exception("Pro změnu hesla vyplňte obě pole hesla!");
                    }

                    heslo = InputValidator.ValidujHeslo(heslo);
                    heslo2 = InputValidator.ValidujHeslo(heslo2);
                    InputValidator.ValidujShoduHesel(heslo, heslo2);

                    salt = PasswordHasher.GenerateSalt();
                    hashHesla = PasswordHasher.HashPassword(heslo, salt);
                }

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

                    if (existujeUcet == true && idClenKlubu != editovanyUzivatel.IdClenKlubu)
                    {
                        throw new Exception("Pro zadaného člena klubu už existuje uživatelský účet!");
                    }
                }

                editovanyUzivatel.UzivatelskeJmeno = uzivatelskeJmeno;
                editovanyUzivatel.Email = email;
                editovanyUzivatel.Heslo = hashHesla;
                editovanyUzivatel.Salt = salt;
                editovanyUzivatel.IdRole = idRole;
                editovanyUzivatel.Role = nazevRole;
                editovanyUzivatel.IdClenKlubu = idClenKlubu;

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