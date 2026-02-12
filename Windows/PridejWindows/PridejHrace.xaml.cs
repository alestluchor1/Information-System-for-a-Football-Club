using System;
using System.Windows;
using System.Windows.Controls;
using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;

namespace InformacniSystemKlub.Windows.PřidáníWindows
{
    public partial class PridejHrace : Window
    {
        private Hrac novyHrac;

        public PridejHrace(Hrac hrac)
        {
            InitializeComponent();
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

                // Uložení
                novyHrac.jmeno = TxtJmeno.Text.Trim();
                novyHrac.prijmeni = TxtPrijmeni.Text.Trim();
                novyHrac.rodneCislo = TxtRodneCislo.Text.Trim();
                novyHrac.pozice = TxtPozice.Text.Trim();
                novyHrac.telefonniCislo = TxtTelefonniCislo.Text.Trim();

                // Spinnery (IntegerUpDown) 
                if (iudPocetGolu.Value == null) iudPocetGolu.Value = 0;
                if (iudPocetZlutychKaret.Value == null) iudPocetZlutychKaret.Value = 0;
                if (iudPocetCervenychKaret.Value == null) iudPocetCervenychKaret.Value = 0;
                if (iudVek.Value == null) iudVek.Value = 3;

                novyHrac.pocetVstrelenychGolu = (int)iudPocetGolu.Value;
                novyHrac.pocetZlutychKaret = (int)iudPocetZlutychKaret.Value;
                novyHrac.pocetCervenychKaret = (int)iudPocetCervenychKaret.Value;
                novyHrac.vek = (int)iudVek.Value;

                novyHrac.typClena = "Hrac";

                // Zdravotní stav z ComboBoxu (A/N)
                ComboBoxItem item = cbZdraviStav.SelectedItem as ComboBoxItem;
                if (item == null)
                {
                    throw new Exception("Vyber zdravotní stav (A/N)");
                }

                novyHrac.zdravotniStav = item.Content.ToString()[0];


                MessageBox.Show("Hráč byl úspěšně přidán", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
