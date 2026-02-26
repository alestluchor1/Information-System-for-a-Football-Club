using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného disciplinárního opatření
    /// Při otevření se formulář předvyplní a po potvrzení se změny uloží do databáze
    /// </summary>
    public partial class EditujOpatreni : Window
    {
        private readonly DisciplinarniOpatreni editovaneOpatreni;
        private const int MaxDuvod = 50;

        public EditujOpatreni(DisciplinarniOpatreni opatreni)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            if (opatreni == null)
            {
                throw new ArgumentNullException(nameof(opatreni));
            }

            this.editovaneOpatreni = opatreni;

            TxtDuvod.MaxLength = MaxDuvod;

            NactiHraceDoComboBoxu();

            // Předvyplnění
            dpDatumUkonceni.SelectedDate = editovaneOpatreni.DatumUkonceniOpatreni;
            TxtDuvod.Text = editovaneOpatreni.Duvod;
        }

        /// <summary>
        /// Načte hráče do comboboxu
        /// </summary>
        private void NactiHraceDoComboBoxu()
        {
            try
            {
                if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
                {
                    HraciOkno.NactiHrace();
                }

                cbHrac.ItemsSource = HraciOkno.HraciData;
                cbHrac.DisplayMemberPath = "prijmeni";

                if (editovaneOpatreni.IdHrac != 0)
                {
                    foreach (var hrac in HraciOkno.HraciData)
                    {
                        if (hrac.id == editovaneOpatreni.IdHrac)
                        {
                            cbHrac.SelectedItem = hrac;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nepodařilo se načíst hráče:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="DisciplinarniOpatreni"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputValidator.ValidujVyberHrace(cbHrac.SelectedItem);

                Hrac hracVybrany = cbHrac.SelectedItem as Hrac;

                if (hracVybrany == null)
                {
                    throw new Exception("Vyberte hráče");
                }

                DateTime datumUkonceni = InputValidator.ValidujDatumUkonceniOpatreni(dpDatumUkonceni.SelectedDate);

                string duvod = TxtDuvod.Text;
                if (duvod == null)
                {
                    duvod = "";
                }
                duvod = InputValidator.ValidujDuvodOpatreni(duvod);

                foreach (var opatreni in OpatreniOkno.DisciplinarniOpatreniData)
                {   
                    // Když už vybraný hráč má jiné opatření kromě editovaného
                    if (opatreni.IdHrac == hracVybrany.id && opatreni.IdOpatreni != editovaneOpatreni.IdOpatreni)
                    {
                        MessageBox.Show("Tento hráč už má jiné opatření!", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                // Uložení do objektu
                editovaneOpatreni.IdHrac = hracVybrany.id;
                editovaneOpatreni.Jmeno = hracVybrany.jmeno;
                editovaneOpatreni.Prijmeni = hracVybrany.prijmeni;
                editovaneOpatreni.DatumUkonceniOpatreni = datumUkonceni;
                editovaneOpatreni.Duvod = duvod;

                // Uložení do DB
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeOpatreni.EditaceOpatreni(conn, editovaneOpatreni);

                MessageBox.Show("Opatření bylo úspěšně editováno", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci opatření:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
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