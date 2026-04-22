using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using System;
using System.Windows;
using System.Windows.Controls;

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

            NastavSkupinuPodleRole();
        }

        /// <summary>
        /// Nastaví skupinu podle role přihlášeného uživatele
        /// Pokud je přihlášen trenér, jeho skupina se automaticky vybere
        /// a ComboBox se zamkne, aby ji nemohl změnit
        /// Ostatní uživatelé (co mají přístup) mohou skupinu vybrat ručně
        /// </summary>
        private void NastavSkupinuPodleRole()
        {
            Uzivatel prihlasenyUzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            // Když není nikdo přihlášený nebo uživatel nemá vyplněnou roli,
            // výběr skupiny zůstane odemčený
            if (prihlasenyUzivatel == null || string.IsNullOrWhiteSpace(prihlasenyUzivatel.Role))
            {
                cbSkupina.IsEnabled = true;
                return;
            }

            string role = prihlasenyUzivatel.Role.Trim().ToLower();

            // Když přihlášený uživatel není trenér,
            // skupina se nebude zamykat a může ji měnit ručně
            if (role != "trener" && role != "trenér")
            {
                cbSkupina.IsEnabled = true;
                return;
            }

            int idSkupiny = 0;

            if (prihlasenyUzivatel.IdTreninkovaSkupina > 0)
            {
                idSkupiny = prihlasenyUzivatel.IdTreninkovaSkupina;
            }
            else
            {
                if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
                {
                    TreneriOkno.NactiTreneri();
                }

                foreach (Trener trener in TreneriOkno.TreneriData)
                {
                    if (trener.Id == prihlasenyUzivatel.IdClenKlubu)
                    {
                        idSkupiny = trener.IdTreninkovaSkupina;
                        break;
                    }
                }
            }

            if (idSkupiny > 0)
            {
                foreach (ComboBoxItem item in cbSkupina.Items)
                {
                    if (item.Tag != null && Convert.ToInt32(item.Tag) == idSkupiny)
                    {
                        cbSkupina.SelectedItem = item;
                        break;
                    }
                }
            }

            cbSkupina.IsEnabled = false;
        }

        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="Hrac"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputValidator.ValidujJmeno(TxtJmeno.Text);
                InputValidator.ValidujPrijmeni(TxtPrijmeni.Text);
                InputValidator.ValidujRodneCislo(TxtRodneCislo.Text);
                InputValidator.ValidujUnikatniRodneCislo(TxtRodneCislo.Text);
                InputValidator.ValidujTelefonniCislo(TxtTelefonniCislo.Text);
                InputValidator.ValidujPozice(TxtPozice.Text);

                int goly = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetGolu.Text, "Počet vstřelených gólů");
                int zluteKarty = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetZlutychKaret.Text, "Počet žlutých karet");
                int cerveneKarty = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetCervenychKaret.Text, "Počet červených karet");

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

                if (idSkupiny == 1)
                {
                    novyHrac.TreninkovaSkupinaText = "A";
                }
                else if (idSkupiny == 2)
                {
                    novyHrac.TreninkovaSkupinaText = "Dorost";
                }
                else if (idSkupiny == 3)
                {
                    novyHrac.TreninkovaSkupinaText = "Mladší žáci";
                }
                else
                {
                    novyHrac.TreninkovaSkupinaText = "";
                }

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