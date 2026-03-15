using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
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

namespace InformacniSystemKlub.Windows.EditujWindows
{
    /// <summary>
    /// Okno pro editaci vybraného sportovního vybavení
    /// Při otevření se formulář předvyplní a po potvrzení se změny uloží do databáze
    /// </summary>
    public partial class EditujSportovniVybaveni : Window
    {
        private SportovniVybaveni editovanySportovniVybaveni;

        public EditujSportovniVybaveni(SportovniVybaveni sportovniVybaveni)
        {
            InitializeComponent();
            RezimZobrazeni.NastavRezimFormularoveOkno(this, HlavniOkno.jeDarkModeAktivni);

            if (sportovniVybaveni == null)
            {
                throw new ArgumentNullException(nameof(sportovniVybaveni));
            }

            editovanySportovniVybaveni = sportovniVybaveni;

            // Předvyplnění
            dpDatumNakupu.SelectedDate = editovanySportovniVybaveni.DatumNakupu;
            TxtNazev.Text = editovanySportovniVybaveni.Nazev;
            TxtVelikost.Text = editovanySportovniVybaveni.Velikost;
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

                // Uložení do objektu
                editovanySportovniVybaveni.Nazev = nazev;
                editovanySportovniVybaveni.DatumNakupu = datumNakupu;
                editovanySportovniVybaveni.Velikost = velikost;

                // Uložení do DB
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();
                DatabazeSportovniVybaveni.EditujSportovniVybaveni(conn, editovanySportovniVybaveni);

                MessageBox.Show("Sportovní vybavení bylo úspěšně upraveno", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                this.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Chyba databáze při editaci sportovního vybavení:\n" + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při ukládání", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
    }
}
