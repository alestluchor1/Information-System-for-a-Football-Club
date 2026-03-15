using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.PřidáníWindows
{
    /// <summary>
    /// Okno pro přidání nového hráče
    /// Po otevření se nastaví motiv (dark/light) a po uložení se provede validace vstupů
    /// </summary>
    public partial class PridejHrace : Window
    {
        private Hrac novyHrac;

        public PridejHrace(Hrac hrac)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            novyHrac = hrac;

            if (novyHrac == null)
            {
                novyHrac = new Hrac();
            }
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="Hrac"/>
        /// </summary>
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

                // Vybraná tréninková skupina
                ComboBoxItem vybranaSkupina = cbSkupina.SelectedItem as ComboBoxItem;
                if (vybranaSkupina == null)
                {
                    throw new Exception("Vyber tréninkovou skupinu!");
                }

                if (vybranaSkupina.Tag == null)
                {
                    throw new Exception("Vyber tréninkovou skupinu!");
                }

                int idSkupiny = Convert.ToInt32(vybranaSkupina.Tag);

                char stavZdravi = InputValidator.ValidujZdravotniStav(cbZdraviStav.SelectedItem);

                novyHrac.Jmeno = TxtJmeno.Text.Trim();
                novyHrac.Prijmeni = TxtPrijmeni.Text.Trim();
                novyHrac.RodneCislo = TxtRodneCislo.Text.Trim();
                novyHrac.Pozice = TxtPozice.Text.Trim();
                novyHrac.TelefonniCislo = TxtTelefonniCislo.Text.Trim();

                novyHrac.PocetVstrelenychGolu = goly;
                novyHrac.PocetZlutychKaret = zluteKarty;
                novyHrac.PocetCervenychKaret = cerveneKarty;
                novyHrac.IdTreninkovaSkupina = idSkupiny;

                novyHrac.TypClena = "Hrac";
                novyHrac.ZdravotniStav = stavZdravi;

                MessageBox.Show("Hráč byl úspěšně přidán", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}
