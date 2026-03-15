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

        /// <summary>
        /// Inicializuje okno pro vyhledávání nominací na zápas,
        /// nastaví barevný režim a připraví vstupní data pro filtrování
        /// </summary>
        /// <param name="nominace">Kolekce nominací, ze které se bude filtrovat</param>
        public NajdiNominaci(ObservableCollection<NominaceNaZapas> nominace)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

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

        /// <summary>
        /// Spustí filtrování nominací podle zadaných hodnot
        /// a při úspěchu zavře dialog s potvrzeným výsledkem
        /// </summary>
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

        /// <summary>
        /// Spustí filtrování nominací podle zadaných hodnot
        /// a při úspěchu zavře dialog s potvrzeným výsledkem
        /// </summary>
        private void BtnZrusit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Provede filtrování nominací na zápas podle aktuálně vyplněných hodnot ve formuláři
        /// </summary>
        /// <returns>Vrací vyfiltrovanou kolekci nominací na zápas</returns>
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

    }
}