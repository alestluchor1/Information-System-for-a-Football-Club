using InformacniSystemKlub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace InformacniSystemKlub.Windows.NajdiWindows
{
    /// <summary>
    /// Okno pro filtrování trenérů podle zadaných kritérií
    /// </summary>
    public partial class NajdiTrenera : Window
    {
        /// <summary>
        /// Výsledná kolekce trenérů po filtrování
        /// Tuto kolekci následně čte okno, které filtr vyvolalo
        /// </summary>
        public ObservableCollection<Trener> FiltrovaniTreneri { get; private set; } = new ObservableCollection<Trener>();

        /// <summary>
        /// Vstupní kolekce trenérů, ze které se filtruje
        /// </summary>
        private readonly ObservableCollection<Trener> VstupniTreneri;

        public NajdiTrenera(ObservableCollection<Trener> treneri)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            VstupniTreneri = treneri;
        }

        /// <summary>
        /// Obsluha tlačítka "Najdi"
        /// Pokud dojde k chybě ve vstupu (validace), zobrazí se chybové hlášení a okno zůstane otevřené
        /// </summary>
        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FiltrovaniTreneri = new ObservableCollection<Trener>(FiltrujTrenery());
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Provede filtrování trenérů na základě vyplněných hodnot ve formuláři
        /// </summary>
        private IEnumerable<Trener> FiltrujTrenery()
        {
            IEnumerable<Trener> vysledek = VstupniTreneri;

            // ID
            if (!string.IsNullOrWhiteSpace(TxtId.Text))
            {
                int id = InputValidator.ValidujId(TxtId.Text.Trim());
                vysledek = vysledek.Where(t => t.Id == id);
            }

            // Jméno
            if (!string.IsNullOrWhiteSpace(TxtJmeno.Text))
            {
                var jmeno = TxtJmeno.Text.Trim();
                InputValidator.ValidujJmeno(jmeno);
                vysledek = vysledek.Where(t => string.Equals(t.Jmeno, jmeno, StringComparison.OrdinalIgnoreCase));
            }

            // Příjmení
            if (!string.IsNullOrWhiteSpace(TxtPrijmeni.Text))
            {
                var prijmeni = TxtPrijmeni.Text.Trim();
                InputValidator.ValidujPrijmeni(prijmeni);
                vysledek = vysledek.Where(t => string.Equals(t.Prijmeni, prijmeni, StringComparison.OrdinalIgnoreCase));
            }

            // Rodné číslo
            if (!string.IsNullOrWhiteSpace(TxtRodneCislo.Text))
            {
                var rodneCislo = TxtRodneCislo.Text.Trim();
                InputValidator.ValidujRodneCislo(rodneCislo);
                vysledek = vysledek.Where(t => t.RodneCislo == rodneCislo);
            }

            // Telefonní číslo
            if (!string.IsNullOrWhiteSpace(TxtTelefonniCislo.Text))
            {
                var telefonniCislo = TxtTelefonniCislo.Text.Trim();
                InputValidator.ValidujTelefonniCislo(telefonniCislo);
                vysledek = vysledek.Where(t => t.TelefonniCislo == telefonniCislo);
            }

            // Specializace
            if (!string.IsNullOrWhiteSpace(TxtSpecializace.Text))
            {
                var specializace = TxtSpecializace.Text.Trim();
                InputValidator.ValidujSpecializace(specializace);
                vysledek = vysledek.Where(t => t.Specializace == specializace);
            }

            return vysledek;
        }
    }
}