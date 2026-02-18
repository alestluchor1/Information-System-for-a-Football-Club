using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace InformacniSystemKlub.Windows
{
    public partial class Kalendar : Window
    {
        private DateTime zobrazenyMesic;
        private DateTime vybranyDen;

        public Kalendar()
        {
            InitializeComponent();

           
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       
    }
}
