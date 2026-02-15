using InformacniSystemKlub.Database;
using InformacniSystemKlub.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Interakční logika pro TreninkOkno.xaml
    /// </summary>
    public partial class TreninkOkno : Window
    {
        /// <summary>
        /// Sdílená kolekce tréninků používaná pro DataGrid
        /// </summary>
        public static ObservableCollection<Trenink> TreninkyData { get; set; } = new ObservableCollection<Trenink>();

        /// <summary>
        /// Příznak, zda je aktivní vyhledávací mód
        /// V tomto režimu není umožněno přidávání/odebírání dat
        /// </summary>
        private bool jeVyhledavaniAktivni = false;

        public TreninkOkno()
        {
            InitializeComponent();
            //ApplyTheme(HlavniOkno.jeDarkModeAktivni);
            NactiTreninky();

            DgTreninky.ItemsSource = TreninkyData;
        }

        private void WindowUkonceniFiltrovani(object sender, RoutedEventArgs e) { }

        private void BtnZpet_Click(object sender, RoutedEventArgs e) { }

        private void BtnPridat_Click(object sender, RoutedEventArgs e) { }
        private void BtnOdebrat_Click(object sender, RoutedEventArgs e) { }
        private void BtnNajdi_Click(object sender, RoutedEventArgs e) { }
        private void BtnEdituj_Click(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// Načte hráče z databáze (pohled TRENINKY_VIEW) a naplní kolekci TreneriData
        /// </summary>
        private void NactiTreninky()
        {
            try
            {
                var conn = SpravcePripojeniDatabaze.ZiskejSdilenePripojeni();

                var cmd = new OracleCommand("SELECT * FROM TRENINKY_VIEW", conn);
                var reader = cmd.ExecuteReader();

                TreninkyData.Clear();

                while (reader.Read())
                {
                    Trenink trenink = new Trenink();

                    if (reader["IDTRENINK"] != DBNull.Value)
                    {
                        trenink.Id = Convert.ToInt32(reader["IDTRENINK"]);
                    }
                    else
                    {
                        trenink.Id = 0;
                    }

                    if (reader["RODNECISLO"] != DBNull.Value)
                    {
                        trenink.RodneCislo = reader["RODNECISLO"].ToString();
                    }
                    else
                    {
                        trenink.RodneCislo = "0";
                    }

                    if (reader["PRIJMENI"] != DBNull.Value)
                    {
                        trenink.Prijmeni = reader["PRIJMENI"].ToString();
                    }
                    else
                    {
                        trenink.Prijmeni = "-";
                    }

                    // Trénink - Popis (volitelný sloupec) 
                    if (reader["POPIS"] != DBNull.Value)
                    {
                        trenink.Popis = reader["POPIS"].ToString();
                    }
                    else
                    {
                        trenink.Popis = "Volitelné nezadáno !";
                    }

                    if (reader["MISTO"] != DBNull.Value)
                    {
                        trenink.Misto = reader["MISTO"].ToString();
                    }
                    else
                    {
                        trenink.Misto = "";
                    }

                    if (reader["DATUM"] != DBNull.Value)
                    {
                        trenink.Datum = Convert.ToDateTime(reader["DATUM"]);
                    }
                    else
                    {
                        trenink.Datum = DateTime.MinValue;
                    }


                    TreninkyData.Add(trenink);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání tréninků: {ex.Message}");
            }
        }
    }
}
