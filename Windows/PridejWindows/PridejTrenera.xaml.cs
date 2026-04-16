using InformacniSystemKlub.Models;
using System;
using System.Windows;

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového trenéra
    /// Při otevření se nastaví motiv (dark/light) a po uložení se provede validace vstupů
    /// </summary>
    public partial class PridejTrenera : Window
    {
        private Trener novyTrener;

        public PridejTrenera(Trener trener)
        {
            InitializeComponent();

            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            novyTrener = trener;

            if (novyTrener == null)
            {
                novyTrener = new Trener();
            }
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="Trener"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validace
                InputValidator.ValidujJmeno(TxtJmeno.Text);
                InputValidator.ValidujPrijmeni(TxtPrijmeni.Text);
                InputValidator.ValidujRodneCislo(TxtRodneCislo.Text);
                InputValidator.ValidujUnikatniRodneCislo(TxtRodneCislo.Text);
                InputValidator.ValidujTelefonniCislo(TxtTelefonniCislo.Text);
                InputValidator.ValidujSpecializace(TxtSpecializace.Text);

                novyTrener.Jmeno = TxtJmeno.Text.Trim();
                novyTrener.Prijmeni = TxtPrijmeni.Text.Trim();
                novyTrener.RodneCislo = TxtRodneCislo.Text.Trim();
                novyTrener.TelefonniCislo = TxtTelefonniCislo.Text.Trim();
                novyTrener.Specializace = TxtSpecializace.Text.Trim();

                MessageBox.Show("Trenér byl úspěšně přidán", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

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