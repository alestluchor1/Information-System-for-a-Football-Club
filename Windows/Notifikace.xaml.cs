using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
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
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

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

            foreach (Uzivatel uzivatel in this.uzivatele)
            {
                uzivatel.JeVybran = false;
            }

            LbPrijemci.ItemsSource = this.uzivatele;
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