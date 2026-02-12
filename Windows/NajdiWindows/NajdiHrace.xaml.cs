using InformacniSystemKlub.Models;
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

namespace InformacniSystemKlub.Windows.NajdiWindows
{
    /// <summary>
    /// Interakční logika pro NajdiHrace.xaml
    /// </summary>
    public partial class NajdiHrace : Window
    {
        // 
        public ObservableCollection<Hrac> FiltrovaniHraci { get; private set; } = new ObservableCollection<Hrac>();

        // sem si uložíme vstupní seznam (HraciData)
        private readonly ObservableCollection<Hrac> VstupniHraci;
        public NajdiHrace(ObservableCollection<Hrac> hraci)
        {
            InitializeComponent();
            VstupniHraci = hraci;
        }

        private IEnumerable<Hrac> FiltrujHrace()
        {
            if (!string.IsNullOrWhiteSpace(TxtJmeno.Text))
            {
                var jmeno = TxtJmeno.Text.Trim();
                InputValidator.ValidujJmeno(jmeno);

                return VstupniHraci.Where(h =>
                    h.jmeno != null &&
                    h.jmeno.Equals(jmeno, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(TxtPrijmeni.Text))
            {
                var prijmeni = TxtPrijmeni.Text.Trim();
                InputValidator.ValidujPrijmeni(prijmeni);

                return VstupniHraci.Where(h =>
                    h.prijmeni != null &&
                    h.prijmeni.Equals(prijmeni, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(TxtRodneCislo.Text))
            {
                var rodneCislo = TxtJmeno.Text.Trim();
                InputValidator.ValidujRodneCislo(rodneCislo);

                return VstupniHraci.Where(h =>
                
                h.rodneCislo != null &&
                h.rodneCislo.Equals(rodneCislo, StringComparison.OrdinalIgnoreCase));

            }

            return VstupniHraci; // když není nic vyplněné
        }

        private void BtnNajdi_Click(object sender, RoutedEventArgs e)
        {
            // uložíme výsledky do kolekce, kterou si HraciOkno přečte
            FiltrovaniHraci = new ObservableCollection<Hrac>(FiltrujHrace());

            DialogResult = true;
            Close();
        }
    }
}
