using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného kontraktu
    /// Při otevření se formulář předvyplní a po potvrzení se změny uloží do databáze
    /// </summary>
    public partial class EditujKontrakt : Window
    {
        private Kontrakt editovanyKontrakt;

        public EditujKontrakt(Kontrakt kontrakt)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            if (kontrakt == null)
            {
                throw new ArgumentNullException(nameof(kontrakt));
            }

            this.editovanyKontrakt = kontrakt;

            NactiHraceDoComboBoxu();

            // Předvyplnění datumů
            dpDatumOd.SelectedDate = editovanyKontrakt.DatumZacatku.ToDateTime(TimeOnly.MinValue);
            dpDatumDo.SelectedDate = editovanyKontrakt.DatumKonce.ToDateTime(TimeOnly.MinValue);

            TxtTelAgenta.Text = editovanyKontrakt.TelCisloNaAgenta.ToString();
            TxtPlat.Text = editovanyKontrakt.Plat.ToString();

            if (editovanyKontrakt.VystupniKlauzule == 0)
            {
                TxtVystupniKlauzule.Text = "";
            }
            else
            {
                TxtVystupniKlauzule.Text = editovanyKontrakt.VystupniKlauzule.ToString();
            }
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

                cbHrac.DisplayMemberPath = "Prijmeni";

                if (editovanyKontrakt.IdHrac != 0)
                {
                    foreach (var hrac in HraciOkno.HraciData)
                    {
                        if (hrac.Id == editovanyKontrakt.IdHrac)
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
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="Kontrakt"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputValidator.ValidujRozsahDatumuKontraktu(dpDatumOd.SelectedDate, dpDatumDo.SelectedDate);

                DateOnly datumOd = DateOnly.FromDateTime(dpDatumOd.SelectedDate.Value.Date);
                DateOnly datumDo = DateOnly.FromDateTime(dpDatumDo.SelectedDate.Value.Date);

                long plat = InputValidator.ValidujPlatKontraktuLong(TxtPlat.Text);

                InputValidator.ValidujTelefonniCislo(TxtTelAgenta.Text);
                string telAgenta = TxtTelAgenta.Text.Trim();

                long klauzule = InputValidator.ValidujVystupniKlauzuliLong(TxtVystupniKlauzule.Text);

                editovanyKontrakt.DatumZacatku = datumOd;
                editovanyKontrakt.DatumKonce = datumDo;
                editovanyKontrakt.Plat = plat;
                editovanyKontrakt.TelCisloNaAgenta = telAgenta;
                editovanyKontrakt.VystupniKlauzule = klauzule;

                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeKontrakty.EditujKontrakt(conn, editovanyKontrakt);

                MessageBox.Show("Kontrakt byl úspěšně upraven", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex) when (ex.Message.Contains("ORA-20022") || ex.Message.Contains("ORA-20026"))

            {
                MessageBox.Show("Tento hráč už má kontrakt!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba",  MessageBoxButton.OK, MessageBoxImage.Error);
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
                Resources["SidebarGradient"] = new LinearGradientBrush(Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16), 90);
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