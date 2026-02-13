using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.PřidáníWindows
{
    public partial class PridejHrace : Window
    {
        private Hrac novyHrac;

        public PridejHrace(Hrac hrac)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            novyHrac = hrac;

            if (novyHrac == null)
            {
                novyHrac = new Hrac();
            }
        }


        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validace
                InputValidator.ValidujJmeno(TxtJmeno.Text);
                InputValidator.ValidujPrijmeni(TxtPrijmeni.Text);
                InputValidator.ValidujRodneCislo(TxtRodneCislo.Text);
                InputValidator.ValidujTelefonniCislo(TxtTelefonniCislo.Text);

                InputValidator.ValidujPozice(TxtPozice.Text);

                int goly = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetGolu.Text, "Počet vstřelených gólů");
                int zluteKarty = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetZlutychKaret.Text, "Počet žlutých karet");
                int cerveneKarty = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetCervenychKaret.Text, "Počet červených karet");
                int vek = InputValidator.ValidujHraciVek(iudVek.Text, "Věk");

                char stavZdravi = InputValidator.ValidujZdravotniStav(cbZdraviStav.SelectedItem);

                // Uložení
                novyHrac.jmeno = TxtJmeno.Text.Trim();
                novyHrac.prijmeni = TxtPrijmeni.Text.Trim();
                novyHrac.rodneCislo = TxtRodneCislo.Text.Trim();
                novyHrac.pozice = TxtPozice.Text.Trim();
                novyHrac.telefonniCislo = TxtTelefonniCislo.Text.Trim();

                novyHrac.pocetVstrelenychGolu = goly;
                novyHrac.pocetZlutychKaret = zluteKarty;
                novyHrac.pocetCervenychKaret = cerveneKarty;
                novyHrac.vek = vek;

                novyHrac.typClena = "Hrac";
                novyHrac.zdravotniStav = stavZdravi;

                MessageBox.Show("Hráč byl úspěšně přidán", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyTheme(bool darkMode)
        {
            if (darkMode)
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
