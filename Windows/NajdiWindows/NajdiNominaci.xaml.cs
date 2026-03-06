using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows.NajdiWindows
{
    /// <summary>
    /// Okno pro filtrování nominací na zápas podle zadaných kritérií
    /// </summary>
    public partial class NajdiNominaci : Window
    {
        /// <summary>
        /// Kolekce nominací po filtrování
        /// </summary>
        public ObservableCollection<NominaceNaZapas> FiltrovaniNominace { get; private set; } = new ObservableCollection<NominaceNaZapas>();

        /// <summary>
        /// Kolekce nominací, ze které se filtruje
        /// </summary>
        private ObservableCollection<NominaceNaZapas> VstupniNominace;

        public NajdiNominaci(ObservableCollection<NominaceNaZapas> nominace)
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            VstupniNominace = nominace;

            if (HraciOkno.HraciData == null || HraciOkno.HraciData.Count == 0)
            {
                HraciOkno.NactiHrace();
            }

            if (ZapasyOkno.ZapasyData == null || ZapasyOkno.ZapasyData.Count == 0)
            {
                ZapasyOkno.NactiZapasy();
            }

            cbHrac.ItemsSource = HraciOkno.HraciData;
            cbHrac.SelectedItem = null;

            cbZapas.ItemsSource = ZapasyOkno.ZapasyData;
            cbZapas.SelectedItem = null;

            cbStavZapasu.SelectedIndex = 0;
        }

        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniNominace = new ObservableCollection<NominaceNaZapas>(FiltrujNominace());
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private IEnumerable<NominaceNaZapas> FiltrujNominace()
        {
            IEnumerable<NominaceNaZapas> vysledek = VstupniNominace;

            // ID nominace
            if (!string.IsNullOrWhiteSpace(TxtIdNominace.Text))
            {
                int id = InputValidator.ValidujId(TxtIdNominace.Text.Trim());
                vysledek = vysledek.Where(n => n.IdNominace == id);
            }

            // Hráč (vybraný v comboboxu)
            if (cbHrac.SelectedItem != null)
            {
                Hrac vybranyHrac = cbHrac.SelectedItem as Hrac;
                if (vybranyHrac == null)
                {
                    throw new Exception("Vyberte hráče");
                }

                vysledek = vysledek.Where(n => n.IdHrace == vybranyHrac.Id);
            }

            // Zápas (vybraný v comboboxu)
            if (cbZapas.SelectedItem != null)
            {
                Zapas vybranyZapas = cbZapas.SelectedItem as Zapas;
                if (vybranyZapas == null)
                {
                    throw new Exception("Vyberte zápas");
                }

                vysledek = vysledek.Where(n => n.IdZapas == vybranyZapas.IdZapas);
            }

            // Stav zápasu
            ComboBoxItem stavItem = cbStavZapasu.SelectedItem as ComboBoxItem;
            if (stavItem != null)
            {
                string stav = stavItem.Content.ToString().Trim();

                if (!string.IsNullOrWhiteSpace(stav))
                {
                    vysledek = vysledek.Where(n =>
                        string.Equals(n.StavZapasu, stav, StringComparison.OrdinalIgnoreCase));
                }
            }

            // Minimální počet gólů
            if (iudMinGolu.Value != null)
            {
                int minGolu = iudMinGolu.Value.Value;
                vysledek = vysledek.Where(n => n.PocetGolu >= minGolu);
            }

            // Minimální počet žlutých karet
            if (iudMinZlutychKaret.Value != null)
            {
                int minZlute = iudMinZlutychKaret.Value.Value;
                vysledek = vysledek.Where(n => n.ZluteKarty >= minZlute);
            }

            // Minimální počet červených karet
            if (iudMinCervenychKaret.Value != null)
            {
                int minCervene = iudMinCervenychKaret.Value.Value;
                vysledek = vysledek.Where(n => n.CerveneKarty >= minCervene);
            }

            return vysledek;
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