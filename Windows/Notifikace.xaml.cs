using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Okno pro odesílání e-mailových notifikací uživatelům
    /// Umožňuje vybrat příjemce, zadat předmět a text zprávy a následně odeslat e-mail
    /// </summary>
    public partial class Notifikace : Window
    {
        /// <summary>
        /// Kolekce uživatelů, kterým lze poslat notifikaci
        /// </summary>
        private ObservableCollection<Uzivatel> uzivatele;

        /// <summary>
        /// Kolekce uživatelů aktuálně zobrazených v seznamu příjemců
        /// </summary>
        private ObservableCollection<Uzivatel> zobrazovaniUzivatele = new ObservableCollection<Uzivatel>();

        /// <summary>
        /// E-mail odesílatele
        /// </summary>
        private static readonly string EmailOdesilatele = ConfigurationManager.AppSettings["EmailOdesilatele"];

        /// <summary>
        /// Heslo e-mailového účtu odesílatele
        /// </summary>
        private static readonly string HesloOdesilatele = ConfigurationManager.AppSettings["HesloOdesilatele"];

        /// <summary>
        /// SMTP server
        /// </summary>
        private static readonly string SmtpServer = ConfigurationManager.AppSettings["SmtpServer"];

        /// <summary>
        /// SMTP port
        /// </summary>
        private static readonly int SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

        public Notifikace(ObservableCollection<Uzivatel> uzivatele)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimNotifikaceOkno(this, HlavniOkno.jeDarkModeAktivni);

            if (string.IsNullOrWhiteSpace(EmailOdesilatele))
            {
                throw new Exception("Chybí hodnota EmailOdesilatele v App.config");
            }

            if (string.IsNullOrWhiteSpace(HesloOdesilatele))
            {
                throw new Exception("Chybí hodnota HesloOdesilatele v App.config");
            }

            if (string.IsNullOrWhiteSpace(SmtpServer))
            {
                throw new Exception("Chybí hodnota SmtpServer v App.config");
            }

            if (uzivatele == null)
            {
                this.uzivatele = new ObservableCollection<Uzivatel>();
            }
            else
            {
                this.uzivatele = uzivatele;
            }

            if (HraciOkno.HraciData.Count == 0)
            {
                HraciOkno.NactiHrace();
            }

            foreach (Uzivatel uzivatel in this.uzivatele)
            {
                uzivatel.JeVybran = false;
            }

            AplikujFiltrUzivatelu();
        }

        /// <summary>
        /// Vrátí normalizovanou podobu role uživatele
        /// </summary>
        private string ZiskejNormalizovanouRoli(Uzivatel uzivatel)
        {
            if (uzivatel == null)
            {
                return "";
            }

            if (string.IsNullOrWhiteSpace(uzivatel.Role))
            {
                return "";
            }

            string role = uzivatel.Role.Trim().ToLower();

            if (role == "hráč" || role == "hrac")
            {
                return "hrac";
            }

            if (role == "trenér" || role == "trener")
            {
                return "trener";
            }

            if (role == "admin")
            {
                return "admin";
            }

            if (role == "host")
            {
                return "host";
            }

            return role;
        }

        /// <summary>
        /// Vrátí hráče odpovídajícího zadanému uživateli podle rodného čísla
        /// </summary>
        private Hrac ZiskejHracePodleRodnehoCisla(Uzivatel uzivatel)
        {
            if (uzivatel == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(uzivatel.RodneCislo))
            {
                return null;
            }

            foreach (Hrac hrac in HraciOkno.HraciData)
            {
                if (string.IsNullOrWhiteSpace(hrac.RodneCislo) == false &&
                    hrac.RodneCislo == uzivatel.RodneCislo)
                {
                    return hrac;
                }
            }

            return null;
        }

        /// <summary>
        /// Aplikuje filtrování uživatelů podle zadaného uživatelského jména
        /// </summary>
        private void AplikujFiltrUzivatelu()
        {
            IEnumerable<Uzivatel> vysledek = uzivatele;

            if (string.IsNullOrWhiteSpace(TxtHledatUzivatele.Text) == false)
            {
                string hledanyText = TxtHledatUzivatele.Text.Trim();


                vysledek = vysledek.Where(u =>
                string.IsNullOrWhiteSpace(u.UzivatelskeJmeno) == false &&
                u.UzivatelskeJmeno.StartsWith(hledanyText, StringComparison.OrdinalIgnoreCase));
            }

            zobrazovaniUzivatele = new ObservableCollection<Uzivatel>(vysledek);
            LbPrijemci.ItemsSource = zobrazovaniUzivatele;
            LbPrijemci.Items.Refresh();
        }

        /// <summary>
        /// Nastaví výběr všech hráčů
        /// </summary>
        private void NastavVyberVsechHracu(bool vybrat)
        {
            foreach (Uzivatel uzivatel in uzivatele)
            {
                if (ZiskejNormalizovanouRoli(uzivatel) == "hrac")
                {
                    uzivatel.JeVybran = vybrat;
                }
            }

            LbPrijemci.Items.Refresh();
        }

        /// <summary>
        /// Nastaví výběr všech trenérů
        /// </summary>
        private void NastavVyberVsechTreneru(bool vybrat)
        {
            foreach (Uzivatel uzivatel in uzivatele)
            {
                if (ZiskejNormalizovanouRoli(uzivatel) == "trener")
                {
                    uzivatel.JeVybran = vybrat;
                }
            }

            LbPrijemci.Items.Refresh();
        }

        /// <summary>
        /// Nastaví výběr všech administrátorů
        /// </summary>
        private void NastavVyberVsechAdministratoru(bool vybrat)
        {
            foreach (Uzivatel uzivatel in uzivatele)
            {
                if (ZiskejNormalizovanouRoli(uzivatel) == "admin")
                {
                    uzivatel.JeVybran = vybrat;
                }
            }

            LbPrijemci.Items.Refresh();
        }

        /// <summary>
        /// Nastaví výběr hráčů podle zadané tréninkové skupiny
        /// </summary>
        private void NastavVyberHracuPodleSkupiny(int idSkupiny, bool vybrat)
        {
            foreach (Uzivatel uzivatel in uzivatele)
            {
                if (ZiskejNormalizovanouRoli(uzivatel) != "hrac")
                {
                    continue;
                }

                Hrac hrac = ZiskejHracePodleRodnehoCisla(uzivatel);

                if (hrac == null)
                {
                    continue;
                }

                if (hrac.IdTreninkovaSkupina == idSkupiny)
                {
                    uzivatel.JeVybran = vybrat;
                }
            }

            LbPrijemci.Items.Refresh();
        }

        /// <summary>
        /// Obsluha změny textu ve vyhledávacím poli
        /// </summary>
        private void TxtHledatUzivatele_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplikujFiltrUzivatelu();
        }

        /// <summary>
        /// Obsluha checkboxu Všichni hráči
        /// </summary>
        private void ChbVszichniHraci_Checked(object sender, RoutedEventArgs e)
        {
            NastavVyberVsechHracu(true);
        }

        /// <summary>
        /// Obsluha odškrtnutí checkboxu Všichni hráči
        /// </summary>
        private void ChbVszichniHraci_Unchecked(object sender, RoutedEventArgs e)
        {
            NastavVyberVsechHracu(false);
        }

        /// <summary>
        /// Obsluha checkboxu A-Tým
        /// </summary>
        private void ChbATym_Checked(object sender, RoutedEventArgs e)
        {
            NastavVyberHracuPodleSkupiny(1, true);
        }

        /// <summary>
        /// Obsluha odškrtnutí checkboxu A-Tým
        /// </summary>
        private void ChbATym_Unchecked(object sender, RoutedEventArgs e)
        {
            NastavVyberHracuPodleSkupiny(1, false);
        }

        /// <summary>
        /// Obsluha checkboxu Dorost
        /// </summary>
        private void ChbDorost_Checked(object sender, RoutedEventArgs e)
        {
            NastavVyberHracuPodleSkupiny(2, true);
        }

        /// <summary>
        /// Obsluha odškrtnutí checkboxu Dorost
        /// </summary>
        private void ChbDorost_Unchecked(object sender, RoutedEventArgs e)
        {
            NastavVyberHracuPodleSkupiny(2, false);
        }

        /// <summary>
        /// Obsluha checkboxu Mladší žáci
        /// </summary>
        private void ChbMladsiZaci_Checked(object sender, RoutedEventArgs e)
        {
            NastavVyberHracuPodleSkupiny(3, true);
        }

        /// <summary>
        /// Obsluha odškrtnutí checkboxu Mladší žáci
        /// </summary>
        private void ChbMladsiZaci_Unchecked(object sender, RoutedEventArgs e)
        {
            NastavVyberHracuPodleSkupiny(3, false);
        }

        /// <summary>
        /// Obsluha checkboxu Všichni trenéři
        /// </summary>
        private void ChbVszichniTreneri_Checked(object sender, RoutedEventArgs e)
        {
            NastavVyberVsechTreneru(true);
        }

        /// <summary>
        /// Obsluha odškrtnutí checkboxu Všichni trenéři
        /// </summary>
        private void ChbVszichniTreneri_Unchecked(object sender, RoutedEventArgs e)
        {
            NastavVyberVsechTreneru(false);
        }

        /// <summary>
        /// Obsluha checkboxu Administrátoři
        /// </summary>
        private void ChbAdministratori_Checked(object sender, RoutedEventArgs e)
        {
            NastavVyberVsechAdministratoru(true);
        }

        /// <summary>
        /// Obsluha odškrtnutí checkboxu Administrátoři
        /// </summary>
        private void ChbAdministratori_Unchecked(object sender, RoutedEventArgs e)
        {
            NastavVyberVsechAdministratoru(false);
        }

        /// <summary>
        /// Obsluha tlačítka Zrušit
        /// Zavře okno bez odeslání zprávy
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Obsluha tlačítka Odeslat
        /// Provede validaci a odešle zprávu všem vybraným příjemcům
        /// </summary>
        private void BtnOdeslat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Uzivatel> prijemci = ZiskejVybranePrijemce();

                if (prijemci.Count == 0)
                {
                    MessageBox.Show(
                        "Musíte vybrat alespoň jednoho příjemce",
                        "Upozornění",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtPredmet.Text))
                {
                    MessageBox.Show(
                        "Předmět zprávy nesmí být prázdný",
                        "Upozornění",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtZprava.Text))
                {
                    MessageBox.Show(
                        "Zpráva nesmí být prázdná",
                        "Upozornění",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                string predmet = TxtPredmet.Text.Trim();
                string zprava = TxtZprava.Text.Trim();

                int uspesne = 0;
                int neuspesne = 0;

                foreach (Uzivatel prijemce in prijemci)
                {
                    if (string.IsNullOrWhiteSpace(prijemce.Email))
                    {
                        MessageBox.Show(
                            "Uživatel '" + prijemce.UzivatelskeJmeno + "' nemá vyplněný e-mail",
                            "Chybějící e-mail",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        neuspesne++;
                        continue;
                    }

                    bool vysledek = OdesliEmail(prijemce.Email, predmet, zprava);

                    if (vysledek == true)
                    {
                        uspesne++;
                    }
                    else
                    {
                        neuspesne++;
                    }
                }

                MessageBox.Show(
                    "Odesílání dokončeno.\n\nÚspěšně odesláno: " + uspesne +
                    "\nNepodařilo se odeslat: " + neuspesne,
                    "Výsledek odesílání",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Neočekávaná chyba během odesílání:\n" + ex.Message,
                    "Chyba",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Odešle e-mail jednomu příjemci přes SMTP server
        /// </summary>
        private bool OdesliEmail(string emailPrijemce, string predmet, string text)
        {
            try
            {
                using (MailMessage msg = new MailMessage())
                {
                    // Nastavení příjemce
                    msg.To.Add(emailPrijemce);

                    // Nastavení odesílatele
                    msg.From = new MailAddress(EmailOdesilatele);

                    // Nastavení předmětu zprávy
                    msg.Subject = predmet;

                    msg.Body =
                        text +
                        "\n\n-----------------------------------------" +
                        "\nInformační systém klubu – automatické oznámení" +
                        "\nTento e-mail byl odeslán z informačního systému klubu" +
                        "\nNa tento e-mail prosím NEODPOVÍDEJTE" +
                        "\n-----------------------------------------";

                    using (SmtpClient smtp = new SmtpClient(SmtpServer, SmtpPort))
                    {
                        // Zapnutí šifrovaného připojení
                        smtp.EnableSsl = true;

                        // Zakázání výchozích přihlašovacích údajů systému
                        smtp.UseDefaultCredentials = false;

                        // Nastavení způsobu doručení přes síť
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        // Nastavení časového limitu pro odeslání
                        smtp.Timeout = 10000;

                        // Nastavení přihlašovacích údajů k e-mailu
                        smtp.Credentials = new NetworkCredential(EmailOdesilatele, HesloOdesilatele);

                        // Odeslání zprávy
                        smtp.Send(msg);
                    }
                }

                // Vrácení informace o úspěšném odeslání
                return true;
            }
            catch (Exception ex)
            {
                string detail = ex.Message;

                MessageBox.Show(
                    "Zpráva pro " + emailPrijemce + " nebyla odeslána.\nChyba: " + detail,
                    "Chyba odesílání",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Vrácení informace o neúspěšném odeslání
                return false;
            }
        }

        /// <summary>
        /// Vrátí seznam vybraných příjemců
        /// </summary>
        private List<Uzivatel> ZiskejVybranePrijemce()
        {
            return uzivatele.Where(u => u.JeVybran == true).ToList();
        }

        
    }
}