using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InformacniSystemKlub.Models;

namespace InformacniSystemKlub.Windows.PridejWindows
{
    /// <summary>
    /// Okno pro přidání nového sportovního vybavení
    /// </summary>
    public partial class PridejSportovniVybaveni : Window
    {
        private SportovniVybaveni novySportovniVybaveni;
        public PridejSportovniVybaveni(SportovniVybaveni sportovniVybaveni)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            novySportovniVybaveni = sportovniVybaveni;

            if (novySportovniVybaveni == null)
            {
                novySportovniVybaveni = new SportovniVybaveni();
            }
        }
    
        /// <summary>
        /// Obsluha tlačítka Uložit
        /// Provede validaci všech vstupů, uloží hodnoty do objektu <see cref="SportovniVybaveni"/>
        /// </summary>
        private void BtnUlozit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nazev = InputValidator.ValidujNazevSportovnihoVybaveni(TxtNazev.Text, "Název sportovního vybavení");
                DateTime datumNakupu = InputValidator.ValidujDatumNakupuSportovnihoVybaveni(dpDatumNakupu.SelectedDate, "Datum nákupu");
                string velikost = InputValidator.ValidujVelikostSportovnihoVybaveni(TxtVelikost.Text, "Velikost položky sportovního vybavení");

                novySportovniVybaveni.Nazev = nazev;
                novySportovniVybaveni.DatumNakupu = datumNakupu;
                novySportovniVybaveni.Velikost = velikost;

                MessageBox.Show("Položka sportovního vybavení byla úspěšně přidána", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

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
