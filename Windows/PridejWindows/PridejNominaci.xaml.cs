using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nové nominace. Umožnuje navrhnout hráče i podel různých kriterií na zápasy
    /// Nebo pokud se zápas odehrál, lze přiřadit hráče k zápasu a zapsat počet odehraných minut, vstřelených golu a počet karet
    /// </summary>
    public partial class PridejNominaci : Window
    {

        private NominaceNaZapas novaNominace;

        /// <summary>
        /// Filtrovaný seznam hráčů pro ComboBox
        /// </summary>
        private ObservableCollection<Hrac> hraciProVyber = new ObservableCollection<Hrac>();

        public PridejNominaci(NominaceNaZapas nominace)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            novaNominace = nominace;

            if (novaNominace == null)
            {
                novaNominace = new NominaceNaZapas();
            }

            if (HraciOkno.HraciData == null)
            {
                HraciOkno.HraciData = new ObservableCollection<Hrac>();
            }

            if (HraciOkno.HraciData.Count == 0)
            {
                HraciOkno.NactiHrace();
            }

            if (ZapasyOkno.ZapasyData == null)
            {
                ZapasyOkno.ZapasyData = new ObservableCollection<Zapas>();
            }

            if (ZapasyOkno.ZapasyData.Count == 0)
            {
                ZapasyOkno.NactiZapasy();
            }

            if (OpatreniOkno.DisciplinarniOpatreniData == null)
            {
                OpatreniOkno.DisciplinarniOpatreniData = new ObservableCollection<DisciplinarniOpatreni>();
            }

            if (OpatreniOkno.DisciplinarniOpatreniData.Count == 0)
            {
                OpatreniOkno.NactiDisciplinarniOpatreni();
            }

            cbZapas.ItemsSource = ZapasyOkno.ZapasyData;
            cbHrac.ItemsSource = hraciProVyber;

            if (cbZapas.Items.Count > 0)
            {
                cbZapas.SelectedIndex = 0;
            }

            AktualizujSeznamHracu();

            if (cbHrac.Items.Count > 0)
            {
                cbHrac.SelectedIndex = 0;
            }

            NastavStavZapasuAEditaci();
        }

        /// <summary>
        /// Reakce na změnu checkboxů filtrů
        /// </summary>
        private void FiltrHrace_Changed(object sender, RoutedEventArgs e)
        {
            AktualizujSeznamHracu();
        }

        /// <summary>
        /// Reakce na změnu vybraného zápasu
        /// </summary>
        private void cbZapas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NastavStavZapasuAEditaci();
            AktualizujSeznamHracu();
        }

        /// <summary>
        /// Zjistí, zda má hráč aktivní disciplinární opatření
        /// </summary>
        private bool HracMaAktivniOpatreni(Hrac hrac)
        {
            if (hrac == null)
            {
                return false;
            }

            if (OpatreniOkno.DisciplinarniOpatreniData == null)
            {
                return false;
            }

            foreach (var opatreni in OpatreniOkno.DisciplinarniOpatreniData)
            {
                if (opatreni == null)
                {
                    continue;
                }

                if (opatreni.IdHrac == hrac.Id)
                {
                    if (opatreni.DatumUkonceniOpatreni >= DateTime.Today)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Přefiltruje a případně seřadí hráče pro výběr
        /// </summary>
        private void AktualizujSeznamHracu()
        {
            if (HraciOkno.HraciData == null)
            {
                hraciProVyber.Clear();
                return;
            }

            // Zkopíruje všechny hráče do pomocného seznamu
            List<Hrac> seznam = new List<Hrac>();

            foreach (var hrac in HraciOkno.HraciData)
            {
                if (hrac == null)
                {
                    continue;
                }

                seznam.Add(hrac);
            }

            Zapas vybranyZapas = cbZapas.SelectedItem as Zapas;

            // Filtry se použijí jen pro zápas, který se bude hrát
            if (vybranyZapas != null)
            {
                if (JeBudeSeHrat(vybranyZapas.StavZapasu))
                {
                    // Nechá jen zdravé hráče
                    if (chkJenZdravi.IsChecked == true)
                    {
                        List<Hrac> zdravi = new List<Hrac>();

                        foreach (var hrac in seznam)
                        {
                            if (hrac.ZdravotniStav == 'A')
                            {
                                zdravi.Add(hrac);
                            }
                        }

                        seznam = zdravi;
                    }

                    // Vyřadí hráče s aktivním opatřením
                    if (chkBezOpatreni.IsChecked == true)
                    {
                        List<Hrac> bezOpatreni = new List<Hrac>();

                        foreach (var hrac in seznam)
                        {
                            if (HracMaAktivniOpatreni(hrac) == false)
                            {
                                bezOpatreni.Add(hrac);
                            }
                        }

                        seznam = bezOpatreni;
                    }

                    // Nechá jen hráče, kteří dali aspoň 1 gól
                    if (chkJenSeGolem.IsChecked == true)
                    {
                        List<Hrac> strelci = new List<Hrac>();

                        foreach (var hrac in seznam)
                        {
                            if (hrac.PocetVstrelenychGolu > 0)
                            {
                                strelci.Add(hrac);
                            }
                        }

                        seznam = strelci;
                    }

                    // Seřadí hráče podle statistik
                    if (chkSeraditDleStatistik.IsChecked == true)
                    {
                        seznam = seznam
                            .OrderByDescending(h => h.PocetVstrelenychGolu)
                            .ThenBy(h => h.PocetCervenychKaret)
                            .ThenBy(h => h.PocetZlutychKaret)
                            .ThenBy(h => h.Prijmeni)
                            .ThenBy(h => h.Jmeno)
                            .ToList();
                    }
                }
            }

            // Zapamatuje si původně vybraného hráče
            Hrac puvodneVybrany = cbHrac.SelectedItem as Hrac;

            // Naplní nový filtrovaný seznam do ComboBoxu
            hraciProVyber.Clear();

            foreach (var hrac in seznam)
            {
                hraciProVyber.Add(hrac);
            }

            // Zkusí obnovit původní výběr
            if (puvodneVybrany != null)
            {
                foreach (var hrac in hraciProVyber)
                {
                    if (hrac.Id == puvodneVybrany.Id)
                    {
                        cbHrac.SelectedItem = hrac;
                        return;
                    }
                }
            }

            // Když původní hráč v seznamu není, vybere prvního
            if (cbHrac.Items.Count > 0)
            {
                cbHrac.SelectedIndex = 0;
            }
            else
            {
                cbHrac.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Nastaví stav zápasu ODEHRANO/BUDE SE HRÁT a povolení ovládacích prvků
        /// </summary>
        private void NastavStavZapasuAEditaci()
        {
            Zapas zapas = cbZapas.SelectedItem as Zapas;

            if (zapas == null)
            {
                TxtStavZapasu.Text = "";
                NastavNulyAZakazEditaci();

                chkJenZdravi.IsEnabled = false;
                chkBezOpatreni.IsEnabled = false;
                chkJenSeGolem.IsEnabled = false;
                chkSeraditDleStatistik.IsEnabled = false;
                return;
            }

            TxtStavZapasu.Text = zapas.StavZapasu;

            if (JeBudeSeHrat(zapas.StavZapasu))
            {
                // Nastaveny 0 a v dialogu nejde psát do nich
                NastavNulyAZakazEditaci();

                chkJenZdravi.IsEnabled = true;
                chkBezOpatreni.IsEnabled = true;
                chkJenSeGolem.IsEnabled = true;
                chkSeraditDleStatistik.IsEnabled = true;
            }
            else
            {
                PovolEditaci();

                chkJenZdravi.IsChecked = false;
                chkBezOpatreni.IsChecked = false;
                chkJenSeGolem.IsChecked = false;
                chkSeraditDleStatistik.IsChecked = false;

                chkJenZdravi.IsEnabled = false;
                chkBezOpatreni.IsEnabled = false;
                chkJenSeGolem.IsEnabled = false;
                chkSeraditDleStatistik.IsEnabled = false;
            }
        }

        /// <summary>
        /// Zjistí, zda je stav zápasu "Bude se hrát"
        /// </summary>
        private bool JeBudeSeHrat(string stavZapasu)
        {
            if (string.IsNullOrWhiteSpace(stavZapasu))
            {
                return false;
            }

            string stav = stavZapasu.Trim();

            if (stav == "BUDE SE HRÁT")
            {
                return true;
            }

            if (stav == "Bude se hrát")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Vynuluje statistiky a zakáže jejich editaci
        /// </summary>
        private void NastavNulyAZakazEditaci()
        {
            iudPocetMinut.Value = 0;
            iudPocetGolu.Value = 0;
            iudPocetZlutychKaret.Value = 0;
            iudPocetCervenychKaret.Value = 0;

            iudPocetMinut.IsEnabled = false;
            iudPocetGolu.IsEnabled = false;
            iudPocetZlutychKaret.IsEnabled = false;
            iudPocetCervenychKaret.IsEnabled = false;
        }

        /// <summary>
        /// Povolení editace statistik
        /// </summary>
        private void PovolEditaci()
        {
            iudPocetMinut.IsEnabled = true;
            iudPocetGolu.IsEnabled = true;
            iudPocetZlutychKaret.IsEnabled = true;
            iudPocetCervenychKaret.IsEnabled = true;
        }

        /// <summary>
        /// Zkontroluje duplicitu hráče s daným zápasem
        /// </summary>
        private void ZkontrolujDuplicituNominace(int idHrace, int idZapas)
        {
            foreach (var n in NominaceNaZapasOkno.NominaceData)
            {
                if (n.IdHrace == idHrace && n.IdZapas == idZapas)
                {
                    throw new Exception("Tento hráč už má záznam s daným zápasem!");
                }
            }
        }

        /// <summary>
        /// Uložení nové nominace
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hrac vybranyHrac = cbHrac.SelectedItem as Hrac;
                if (vybranyHrac == null)
                {
                    throw new Exception("Vyber hráče!");
                }

                Zapas vybranyZapas = cbZapas.SelectedItem as Zapas;
                if (vybranyZapas == null)
                {
                    throw new Exception("Vyber zápas!");
                }

                ZkontrolujDuplicituNominace(vybranyHrac.Id, vybranyZapas.IdZapas);

                int minuty;
                int goly;
                int zluteKarty;
                int cerveneKarty;

                if (JeBudeSeHrat(vybranyZapas.StavZapasu))
                {
                    minuty = 0;
                    goly = 0;
                    zluteKarty = 0;
                    cerveneKarty = 0;

                    iudPocetMinut.Value = 0;
                    iudPocetGolu.Value = 0;
                    iudPocetZlutychKaret.Value = 0;
                    iudPocetCervenychKaret.Value = 0;
                }
                else
                {
                    minuty = InputValidator.ValidujNominaciMinuty(iudPocetMinut.Text, "Počet odehraných minut");
                    goly = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetGolu.Text, "Počet gólů");
                    zluteKarty = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetZlutychKaret.Text, "Počet žlutých karet");
                    cerveneKarty = InputValidator.ValidujHraciSpinnerGolyKarty(iudPocetCervenychKaret.Text, "Počet červených karet");

                    ZkontrolujKartyProNovouNominaci(vybranyZapas, zluteKarty, cerveneKarty);
                }

                novaNominace.IdHrace = vybranyHrac.Id;
                novaNominace.RodneCislo = vybranyHrac.RodneCislo;
                novaNominace.Prijmeni = vybranyHrac.Prijmeni;
                novaNominace.IdTreninkovaSkupina = vybranyHrac.IdTreninkovaSkupina;

                novaNominace.IdZapas = vybranyZapas.IdZapas;
                novaNominace.DomaciTym = vybranyZapas.DomaciTym;
                novaNominace.HosteTym = vybranyZapas.HosteTym;
                novaNominace.StavZapasu = vybranyZapas.StavZapasu;

                novaNominace.PocetOdehranychMinut = minuty;
                novaNominace.PocetGolu = goly;
                novaNominace.ZluteKarty = zluteKarty;
                novaNominace.CerveneKarty = cerveneKarty;

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Zkontroluje limity karet pro daný zápas
        /// </summary>
        private void ZkontrolujKartyProNovouNominaci(Zapas zapas, int noveZluteKarty, int noveCerveneKarty)
        {
            int soucetZlute = 0;
            int soucetCervene = 0;

            // Sečte karty ze všech dosavadních nominací pro daný zápas
            foreach (var n in NominaceNaZapasOkno.NominaceData)
            {
                if (n.IdZapas == zapas.IdZapas)
                {
                    soucetZlute = soucetZlute + n.ZluteKarty;
                    soucetCervene = soucetCervene + n.CerveneKarty;
                }
            }

            // Přičte karty z nové nominace
            soucetZlute = soucetZlute + noveZluteKarty;
            soucetCervene = soucetCervene + noveCerveneKarty;

            // Zkontroluje limit žlutých karet
            if (zapas.PocetZlutychKaret != null)
            {
                int maxZlute = zapas.PocetZlutychKaret.Value;

                if (soucetZlute > maxZlute)
                {
                    throw new Exception(
                        "V zápase je zadaný maximální počet žlutých karet: " + maxZlute + ".\n" +
                        "Po přidání této nominace by celkový počet žlutých karet byl: " + soucetZlute + ".\n" +
                        "Tím by došlo k překročení celkového limitu karet pro zápas"
                    );
                }
            }

            // Zkontroluje limit červených karet
            if (zapas.PocetCervenychKaret != null)
            {
                int maxCervene = zapas.PocetCervenychKaret.Value;

                if (soucetCervene > maxCervene)
                {
                    throw new Exception(
                        "V zápase je zadaný maximální počet červených karet: " + maxCervene + ".\n" +
                        "Po přidání této nominace by celkový počet červených karet byl: " + soucetCervene + ".\n" +
                        "Tím by došlo k překročení celkového limitu karet pro zápas"
                    );
                }
            }
        }

        /// <summary>
        /// Nastaví vzhled okna podle režimu dark/light mode
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