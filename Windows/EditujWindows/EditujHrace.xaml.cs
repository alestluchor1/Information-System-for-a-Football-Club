using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného hráče
    /// Při otevření se formulář předvyplní hodnotami hráče a po potvrzení se změny uloží do databáze
    /// pomocí procedury <c>PKG_HRACI.SP_UPDATE_HRAC</c>
    /// </summary>
    public partial class EditujHrace : Window
    {
        private Hrac editovanyHrac;

        public EditujHrace(Hrac editovanyHrac)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            this.editovanyHrac = editovanyHrac;

            TxtRodneCislo.Text = editovanyHrac.RodneCislo;
            TxtJmeno.Text = editovanyHrac.Jmeno;
            TxtPrijmeni.Text = editovanyHrac.Prijmeni;
            TxtTelefonniCislo.Text = editovanyHrac.TelefonniCislo;
            TxtPozice.Text = editovanyHrac.Pozice;

            iudPocetGolu.Value = editovanyHrac.PocetVstrelenychGolu;
            iudPocetZlutychKaret.Value = editovanyHrac.PocetZlutychKaret;
            iudPocetCervenychKaret.Value = editovanyHrac.PocetCervenychKaret;

            NastavSkupinuPodleId(editovanyHrac.IdTreninkovaSkupina);
            NastavZdravotniStav(editovanyHrac.ZdravotniStav);
            NastavSkupinuPodleRole();
        }

        /// <summary>
        /// Nastaví vybranou položku v ComboBoxu zdravotního stavu podle hodnoty (A/N)
        /// </summary>
        private void NastavZdravotniStav(char stav)
        {
            foreach (object item in cbZdraviStav.Items)
            {
                ComboBoxItem polozka = item as ComboBoxItem;
                if (polozka != null)
                {
                    string text = polozka.Content as string;
                    if (text != null)
                    {
                        text = text.Trim();

                        if (text.Length == 1 && text[0] == stav)
                        {
                            cbZdraviStav.SelectedItem = polozka;
                            return;
                        }
                    }
                }
            }

            cbZdraviStav.SelectedIndex = 0;
        }

        /// <summary>
        /// Vybere v ComboBoxu skupinu podle jejího ID
        /// Pokud ji nenajde, nastaví první položku
        /// </summary>
        /// <param name="idSkupiny">ID tréninkové skupiny.</param>
        private void NastavSkupinuPodleId(int idSkupiny)
        {
            foreach (object item in cbSkupina.Items)
            {
                ComboBoxItem polozka = item as ComboBoxItem;
                if (polozka == null)
                {
                    continue;
                }

                if (polozka.Tag == null)
                {
                    continue;
                }

                int tag = Convert.ToInt32(polozka.Tag);

                if (tag == idSkupiny)
                {
                    cbSkupina.SelectedItem = polozka;
                    return;
                }
            }

            cbSkupina.SelectedIndex = 0;
        }

        /// <summary>
        /// Vrátí roli právě přihlášeného uživatele
        /// ve sjednoceném tvaru: admin, trener, hrac nebo host
        /// </summary>
        /// <returns>Role aktuálního uživatele</returns>
        private string ZiskejRoliUzivatele()
        {
            Uzivatel uzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (uzivatel == null || string.IsNullOrWhiteSpace(uzivatel.Role))
            {
                return "host";
            }

            string role = uzivatel.Role.Trim().ToLower();

            if (role == "admin")
            {
                return "admin";
            }

            if (role == "trenér" || role == "trener")
            {
                return "trener";
            }

            if (role == "hráč" || role == "hrac")
            {
                return "hrac";
            }

            return "host";
        }

        /// <summary>
        /// Vrátí ID tréninkové skupiny přihlášeného trenéra
        /// Pokud ji nelze zjistit, vrátí 0
        /// </summary>
        /// <returns>ID skupiny přihlášeného trenéra</returns>
        private int ZiskejIdTreninkoveSkupinyPrihlasenehoTrenera()
        {
            Uzivatel uzivatel = HlavniOkno.ZiskejPrihlasenehoUzivatele();

            if (uzivatel == null)
            {
                return 0;
            }

            if (uzivatel.IdTreninkovaSkupina > 0)
            {
                return uzivatel.IdTreninkovaSkupina;
            }

            if (uzivatel.IdClenKlubu <= 0)
            {
                return 0;
            }

            if (TreneriOkno.TreneriData == null || TreneriOkno.TreneriData.Count == 0)
            {
                TreneriOkno.NactiTreneri();
            }

            foreach (Trener trener in TreneriOkno.TreneriData)
            {
                if (trener.Id == uzivatel.IdClenKlubu)
                {
                    return trener.IdTreninkovaSkupina;
                }
            }

            return 0;
        }

        /// <summary>
        /// Nastaví zamčení / odemčení skupiny podle role
        /// </summary>
        private void NastavSkupinuPodleRole()
        {
            string role = ZiskejRoliUzivatele();

            if (role == "admin")
            {
                cbSkupina.IsEnabled = true;
                return;
            }

            if (role == "trener")
            {
                int idSkupinyTrenera = ZiskejIdTreninkoveSkupinyPrihlasenehoTrenera();

                if (idSkupinyTrenera > 0)
                {
                    NastavSkupinuPodleId(idSkupinyTrenera);
                }

                cbSkupina.IsEnabled = false;
                return;
            }

            cbSkupina.IsEnabled = false;
        }

        /// <summary>
        /// Převede ID skupiny na text
        /// </summary>
        private string SkupinaNaText(int idSkupiny)
        {
            if (idSkupiny == 1)
            {
                return "A";
            }
            else if (idSkupiny == 2)
            {
                return "Dorost";
            }
            else if (idSkupiny == 3)
            {
                return "Mladší žáci";
            }

            return "Neznámá";
        }

        /// <summary>
        /// Načte hodnoty z formuláře, provede validaci a uloží změny do databáze
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string rodneCislo = TxtRodneCislo.Text.Trim();
                string jmeno = TxtJmeno.Text.Trim();
                string prijmeni = TxtPrijmeni.Text.Trim();
                string tel = TxtTelefonniCislo.Text.Trim();
                string pozice = TxtPozice.Text.Trim();

                InputValidator.ValidujRodneCislo(rodneCislo);
                InputValidator.ValidujUnikatniRodneCislo(rodneCislo, editovanyHrac.Id);
                InputValidator.ValidujJmeno(jmeno);
                InputValidator.ValidujPrijmeni(prijmeni);
                InputValidator.ValidujTelefonniCislo(tel);
                InputValidator.ValidujPozice(pozice);

                string golyText = "";
                if (iudPocetGolu.Value != null)
                {
                    golyText = iudPocetGolu.Value.ToString();
                }

                string zluteText = "";
                if (iudPocetZlutychKaret.Value != null)
                {
                    zluteText = iudPocetZlutychKaret.Value.ToString();
                }

                string cerveneText = "";
                if (iudPocetCervenychKaret.Value != null)
                {
                    cerveneText = iudPocetCervenychKaret.Value.ToString();
                }

                int goly = InputValidator.ValidujHraciSpinnerGolyKarty(golyText, "Počet vstřelených gólů");
                int zlute = InputValidator.ValidujHraciSpinnerGolyKarty(zluteText, "Počet žlutých karet");
                int cervene = InputValidator.ValidujHraciSpinnerGolyKarty(cerveneText, "Počet červených karet");

                char zdrav = InputValidator.ValidujZdravotniStav(cbZdraviStav.SelectedItem);

                ComboBoxItem vybranaSkupina = cbSkupina.SelectedItem as ComboBoxItem;
                if (vybranaSkupina == null || vybranaSkupina.Tag == null)
                {
                    MessageBox.Show("Vyber tréninkovou skupinu!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int idSkupiny = Convert.ToInt32(vybranaSkupina.Tag);

                if (idSkupiny == 0)
                {
                    MessageBox.Show("Vyber tréninkovou skupinu!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ZiskejRoliUzivatele() == "trener")
                {
                    int idSkupinyTrenera = ZiskejIdTreninkoveSkupinyPrihlasenehoTrenera();

                    if (idSkupiny != idSkupinyTrenera)
                    {
                        MessageBox.Show("Můžete upravovat hráče pouze ve své tréninkové skupině!", "Přístup odepřen", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                editovanyHrac.RodneCislo = rodneCislo;
                editovanyHrac.Jmeno = jmeno;
                editovanyHrac.Prijmeni = prijmeni;
                editovanyHrac.TelefonniCislo = tel;
                editovanyHrac.Pozice = pozice;

                editovanyHrac.PocetVstrelenychGolu = goly;
                editovanyHrac.PocetZlutychKaret = zlute;
                editovanyHrac.PocetCervenychKaret = cervene;

                editovanyHrac.ZdravotniStav = zdrav;
                editovanyHrac.IdTreninkovaSkupina = idSkupiny;
                editovanyHrac.TreninkovaSkupinaText = SkupinaNaText(idSkupiny);

                OracleConnection conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeHraci.EditaceHrace(conn, editovanyHrac);

                MessageBox.Show("Změny byly úspěšně uloženy", "Úspěch", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci hráče:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při editaci hráče:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsluha tlačítka Zrušit
        /// Okno se zavře bez uložení změn
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}