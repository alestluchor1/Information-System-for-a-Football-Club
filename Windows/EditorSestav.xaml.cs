using InformacniSystemKlub.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InformacniSystemKlub.Windows
{
    /// <summary>
    /// Okno pro tvorbu a úpravu sestavy hráčů
    /// Umožňuje filtrovat hráče, přidávat je na plochu hřiště
    /// a následně je na hřišti libovolně přesouvat nebo odebírat
    /// </summary>
    public partial class EditorSestav : Window
    {
        /// <summary>
        /// Kolekce všech načtených hráčů
        /// </summary>
        private ObservableCollection<Hrac> vsichniHraci;

        /// <summary>
        /// Kolekce hráčů po aplikaci aktuálních filtrů
        /// </summary>
        private ObservableCollection<Hrac> filtrovaniHraci;

        /// <summary>
        /// Kolekce všech načtených disciplinárních opatření
        /// </summary>
        private ObservableCollection<DisciplinarniOpatreni> vsechnaOpatreni;

        /// <summary>
        /// Aktuálně tažená karta hráče na hřišti
        /// </summary>
        private Border tahanyHrac;

        /// <summary>
        /// Pozice kliknutí uvnitř karty hráče při zahájení tažení
        /// Slouží k plynulému přesouvání prvku po canvasu
        /// </summary>
        private Point poziceKliknuti;


        public EditorSestav()
        {
            InitializeComponent();
            ApplyTheme(HlavniOkno.jeDarkModeAktivni);

            this.vsichniHraci = new ObservableCollection<Hrac>();
            this.filtrovaniHraci = new ObservableCollection<Hrac>();
            this.vsechnaOpatreni = new ObservableCollection<DisciplinarniOpatreni>();

            NactiHrace();
            NactiOpatreni();
            NastavVychoziHodnoty();
            AplikujFiltry();
        }

        /// <summary>
        /// Nastaví výchozí hodnoty všech ovládacích prvků použitých pro filtrování hráčů
        /// </summary>
        private void NastavVychoziHodnoty()
        {
            CbSkupina.SelectedIndex = 0;
            CbZdravotniStav.SelectedIndex = 0;
            CbDisciplinarni.SelectedIndex = 0;
            TxtMinGolu.Text = "0";
        }

        /// <summary>
        /// Načte seznam hráčů a uloží jej do lokální kolekce všech hráčů
        /// </summary>
        private void NactiHrace()
        {
            HraciOkno.NactiHrace();

            vsichniHraci.Clear();

            foreach (Hrac hrac in HraciOkno.HraciData)
            {
                vsichniHraci.Add(hrac);
            }
        }

        /// <summary>
        /// Načte všechna disciplinární opatření a uloží je do lokální kolekce.
        /// </summary>
        private void NactiOpatreni()
        {
            OpatreniOkno.NactiDisciplinarniOpatreni();

            vsechnaOpatreni.Clear();

            foreach (DisciplinarniOpatreni opatreni in OpatreniOkno.DisciplinarniOpatreniData)
            {
                vsechnaOpatreni.Add(opatreni);
            }
        }

        /// <summary>
        /// Vrátí textový obsah aktuálně vybrané položky z ComboBoxu
        /// Pokud není vybrána žádná položka nebo nemá obsah, vrací prázdný řetězec
        /// </summary>
        private string ZiskejVybranouHodnotu(ComboBox comboBox)
        {
            ComboBoxItem polozka = comboBox.SelectedItem as ComboBoxItem;

            if (polozka == null)
            {
                return "";
            }

            if (polozka.Content == null)
            {
                return "";
            }

            return polozka.Content.ToString();
        }

        /// <summary>
        /// Zjistí, zda má konkrétní hráč přiřazené alespoň jedno disciplinární opatření
        /// </summary>
        private bool MaHracOpatreni(int idHrace)
        {
            foreach (DisciplinarniOpatreni opatreni in vsechnaOpatreni)
            {
                if (opatreni.IdHrac == idHrace)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Aplikuje aktuálně nastavené filtry na seznam všech hráčů
        /// a výsledný seznam zobrazí v ListBoxu
        /// </summary>
        private void AplikujFiltry()
        {
            filtrovaniHraci.Clear();

            string skupina = ZiskejVybranouHodnotu(CbSkupina);
            string zdravotniStav = ZiskejVybranouHodnotu(CbZdravotniStav);
            string disciplinarni = ZiskejVybranouHodnotu(CbDisciplinarni);

            int minGolu = 0;
            bool povedloSe = int.TryParse(TxtMinGolu.Text, out minGolu);

            if (povedloSe == false)
            {
                // Pokud uživatel zadá neplatné číslo, použije se 0
                minGolu = 0;
            }

            foreach (Hrac hrac in vsichniHraci)
            {
                bool pridat = true;

                // Filtr podle tréninkové skupiny
                if (skupina != "Vše")
                {
                    if (hrac.TreninkovaSkupinaText != skupina)
                    {
                        pridat = false;
                    }
                }

                // Filtr podle zdravotního stavu
                if (zdravotniStav == "A")
                {
                    if (hrac.ZdravotniStav != 'A')
                    {
                        pridat = false;
                    }
                }

                if (zdravotniStav == "N")
                {
                    if (hrac.ZdravotniStav != 'N')
                    {
                        pridat = false;
                    }
                }

                bool maOpatreni = MaHracOpatreni(hrac.Id);

                // Filtr podle existence disciplinárního opatření
                if (disciplinarni == "Ano")
                {
                    if (maOpatreni == false)
                    {
                        pridat = false;
                    }
                }

                if (disciplinarni == "Ne")
                {
                    if (maOpatreni == true)
                    {
                        pridat = false;
                    }
                }

                // Filtr podle minimálního počtu vstřelených gólů
                if (hrac.PocetVstrelenychGolu < minGolu)
                {
                    pridat = false;
                }

                if (pridat == true)
                {
                    filtrovaniHraci.Add(hrac);
                }
            }

            LbHraci.ItemsSource = filtrovaniHraci;
        }

        /// <summary>
        /// Vytvoří vizuální kartu hráče a přidá ji na plochu hřiště
        /// Nově vložená karta je umístěna do výchozí pozice
        /// </summary>
        /// <param name="hrac">Hráč, který má být přidán na hřiště</param>
        private void PridejHraceNaHriste(Hrac hrac)
        {
            if (hrac == null)
            {
                return;
            }

            int sirkaKarty = 100;
            int vyskaKarty = 50;
            int levaPozice = 30;
            int horniPozice = 30;

            Border karta = new Border();
            karta.Width = sirkaKarty;
            karta.Height = vyskaKarty;
            karta.Background = Brushes.White;
            karta.BorderBrush = Brushes.Black;
            karta.BorderThickness = new Thickness(1);
            karta.Tag = hrac;

            StackPanel panel = new StackPanel();

            TextBlock txtPrijmeni = new TextBlock();
            txtPrijmeni.Text = hrac.Prijmeni;
            txtPrijmeni.FontWeight = FontWeights.Bold;
            txtPrijmeni.HorizontalAlignment = HorizontalAlignment.Center;

            TextBlock txtId = new TextBlock();
            txtId.Text = "ID: " + hrac.Id;
            txtId.HorizontalAlignment = HorizontalAlignment.Center;

            panel.Children.Add(txtPrijmeni);
            panel.Children.Add(txtId);

            karta.Child = panel;

            karta.MouseLeftButtonDown += Hrac_MouseLeftButtonDown;
            karta.MouseMove += Hrac_MouseMove;
            karta.MouseLeftButtonUp += Hrac_MouseLeftButtonUp;
            karta.MouseRightButtonDown += Hrac_MouseRightButtonDown;

            // Nastavení pozice karty na hřišti
            Canvas.SetLeft(karta, levaPozice);
            Canvas.SetTop(karta, horniPozice);

            // Přidání karty na canvas hřiště
            CvHriste.Children.Add(karta);
        }

        /// <summary>
        /// Obsluha dvojkliku na hráče v seznamu
        /// Vybraného hráče přidá na plochu hřiště
        /// </summary>
        private void LbHraci_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Hrac vybranyHrac = LbHraci.SelectedItem as Hrac;

            if (vybranyHrac == null)
            {
                return;
            }

            PridejHraceNaHriste(vybranyHrac);
        }

        /// <summary>
        /// Zahájí tažení karty hráče po hřišti
        /// Uloží si aktuální kartu i pozici kliknutí uvnitř prvku
        /// </summary>
        private void Hrac_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border karta = sender as Border;

            if (karta == null)
            {
                return;
            }

            tahanyHrac = karta;
            poziceKliknuti = e.GetPosition(karta);
            tahanyHrac.CaptureMouse();
        }

        /// <summary>
        /// Přesouvá aktuálně taženou kartu hráče v rámci plochy hřiště
        /// Pohyb je omezen hranicemi canvasu
        /// </summary>
        private void Hrac_MouseMove(object sender, MouseEventArgs e)
        {
            if (tahanyHrac == null)
            {
                return;
            }

            // Pokud není stisknuté levé tlačítko myši, tažení neprobíhá
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            // Zjistí aktuální pozici myši vůči canvasu hřiště
            Point novaPozice = e.GetPosition(CvHriste);

            // Spočítá nové pozice
            double novaLeva = novaPozice.X - poziceKliknuti.X;
            double novaHorni = novaPozice.Y - poziceKliknuti.Y;

            // Ochrana proti posunutí mimo levý a horní okraj
            if (novaLeva < 0)
            {
                novaLeva = 0;
            }

            if (novaHorni < 0)
            {
                novaHorni = 0;
            }

            double maxLeva = CvHriste.ActualWidth - tahanyHrac.Width;
            double maxHorni = CvHriste.ActualHeight - tahanyHrac.Height;

            // Ochrana proti posunutí mimo pravý a dolní okraj
            if (novaLeva > maxLeva)
            {
                novaLeva = maxLeva;
            }

            if (novaHorni > maxHorni)
            {
                novaHorni = maxHorni;
            }

            Canvas.SetLeft(tahanyHrac, novaLeva);
            Canvas.SetTop(tahanyHrac, novaHorni);
        }

        /// <summary>
        /// Ukončí tažení hráče a uvolní zachycení myši
        /// </summary>
        private void Hrac_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (tahanyHrac != null)
            {
                tahanyHrac.ReleaseMouseCapture();
                tahanyHrac = null;
            }
        }

        /// <summary>
        /// Odebere kartu hráče z plochy hřiště po kliknutí pravým tlačítkem myši
        /// </summary>
        private void Hrac_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border karta = sender as Border;

            if (karta == null)
            {
                return;
            }

            CvHriste.Children.Remove(karta);
        }

        /// <summary>
        /// Reaguje na změnu vybraného filtru v ComboBoxu
        /// Po načtení okna znovu aplikuje filtry
        /// </summary>
        private void FiltrZmenen(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded == true)
            {
                AplikujFiltry();
            }
        }

        /// <summary>
        /// Reaguje na změnu textového filtru
        /// Po načtení okna znovu aplikuje filtry
        /// </summary>
        private void FiltrZmenenText(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded == true)
            {
                AplikujFiltry();
            }
        }

        /// <summary>
        /// Obnoví všechny filtry do výchozího stavu
        /// a znovu zobrazí odpovídající seznam hráčů
        /// </summary>
        private void BtnVymazatFiltry_Click(object sender, RoutedEventArgs e)
        {
            CbSkupina.SelectedIndex = 0;
            CbZdravotniStav.SelectedIndex = 0;
            CbDisciplinarni.SelectedIndex = 0;
            TxtMinGolu.Text = "0";

            AplikujFiltry();
        }

        /// <summary>
        /// Odstraní všechny karty hráčů z plochy hřiště
        /// </summary>
        private void BtnVymazatHriste_Click(object sender, RoutedEventArgs e)
        {
            CvHriste.Children.Clear();
        }

        /// <summary>
        /// Nastaví barevné zdroje okna podle zvoleného režimu (dark / light)
        /// </summary>
        private void ApplyTheme(bool darkMode)
        {
            if (darkMode == true)
            {
                Resources["WindowBgBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x0F, 0x19));
                Resources["CardBrush"] = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
                Resources["CardAltBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));
                Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0x1F, 0x29, 0x37));
                Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB));
                Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
                Resources["AccentBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                Resources["AccentDarkBrush"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));
                Resources["InputBgBrush"] = new SolidColorBrush(Color.FromRgb(0x0F, 0x17, 0x26));
                Resources["InputHoverBrush"] = new SolidColorBrush(Color.FromRgb(0x16, 0x20, 0x2D));
                Resources["DropdownBrush"] = new SolidColorBrush(Color.FromRgb(0x0F, 0x17, 0x26));
                Resources["FieldOverlayBrush"] = new SolidColorBrush(Color.FromArgb(0x12, 0x00, 0x00, 0x00));
            }
            else
            {
                Resources["WindowBgBrush"] = new SolidColorBrush(Color.FromRgb(0xFA, 0xFA, 0xFA));
                Resources["CardBrush"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                Resources["CardAltBrush"] = new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC));
                Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));
                Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
                Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x6B, 0x72, 0x80));
                Resources["AccentBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                Resources["AccentDarkBrush"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));
                Resources["InputBgBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB));
                Resources["InputHoverBrush"] = new SolidColorBrush(Color.FromRgb(0xF3, 0xF4, 0xF6));
                Resources["DropdownBrush"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                Resources["FieldOverlayBrush"] = new SolidColorBrush(Color.FromArgb(0x08, 0x00, 0x00, 0x00));
            }
        }
    }
}