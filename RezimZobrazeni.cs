using System.Windows;
using System.Windows.Media;

namespace InformacniSystemKlub
{
    /// <summary>
    /// Pomocná třída pro nastavení barevného režimu okna.
    /// Umožňuje přepínat mezi světlým a tmavým vzhledem aplikace.
    /// </summary>
    public static class RezimZobrazeni
    {
        /// <summary>
        /// Nastaví barevné prostředky okna podle zvoleného režimu zobrazení pro hlavní okno
        /// </summary>
        /// <param name="okno">Okno, kterému se mají nastavit barvy</param>
        /// <param name="darkMode">Určuje, zda se použije tmavý režim</param>
        public static void NastavRezim(Window okno, bool darkMode)
        {
            if (darkMode == true)
            {
                okno.Resources["AppBgBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x0F, 0x19));
                okno.Resources["SurfaceBrush"] = new SolidColorBrush(Color.FromRgb(0x0F, 0x17, 0x2A));
                okno.Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0x1F, 0x29, 0x37));
                okno.Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC));
                okno.Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));

                okno.Resources["TopBarText"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                okno.Resources["TopBarSubText"] = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));
            }
            else
            {
                okno.Resources["AppBgBrush"] = new SolidColorBrush(Color.FromRgb(0xF6, 0xF7, 0xFB));
                okno.Resources["SurfaceBrush"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                okno.Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0xE4, 0xEA, 0xF3));
                okno.Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));
                okno.Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B));

                okno.Resources["TopBarText"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                okno.Resources["TopBarSubText"] = new SolidColorBrush(Color.FromRgb(0xEA, 0xF0, 0xFF));
            }
        }

        /// <summary>
        /// Nastaví barevné prostředky běžných oken s tabulkou
        /// </summary>
        public static void NastavRezimTabulkoveOkno(Window okno, bool darkMode)
        {
            if (darkMode == true)
            {
                okno.Resources["WindowBgBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x0F, 0x19));
                okno.Resources["CardBrush"] = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
                okno.Resources["CardAltBrush"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));
                okno.Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0x1F, 0x29, 0x37));
                okno.Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB));
                okno.Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
                okno.Resources["AccentBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                okno.Resources["AccentDarkBrush"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));
            }
            else
            {
                okno.Resources["WindowBgBrush"] = new SolidColorBrush(Color.FromRgb(0xFA, 0xFA, 0xFA));
                okno.Resources["CardBrush"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                okno.Resources["CardAltBrush"] = new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC));
                okno.Resources["BorderBrushSoft"] = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));
                okno.Resources["TextBrush"] = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
                okno.Resources["MutedTextBrush"] = new SolidColorBrush(Color.FromRgb(0x6B, 0x72, 0x80));
                okno.Resources["AccentBrush"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                okno.Resources["AccentDarkBrush"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));
            }
        }

        /// <summary>
        /// Nastaví barevné prostředky formulářových a přidávacích oken
        /// podle zvoleného režimu zobrazení.
        /// </summary>
        /// <param name="okno">Okno, kterému se mají nastavit barvy</param>
        /// <param name="darkMode">Určuje, zda se použije tmavý režim</param>
        public static void NastavRezimFormularoveOkno(Window okno, bool darkMode)
        {
            if (darkMode == true)
            {
                okno.Resources["BgApp"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x0F, 0x19));
                okno.Resources["Surface"] = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
                okno.Resources["TextMain"] = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB));
                okno.Resources["BorderSoft"] = new SolidColorBrush(Color.FromRgb(0x1F, 0x29, 0x37));
                okno.Resources["InputBg"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));

                okno.Resources["Orange"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                okno.Resources["OrangeHover"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));

                okno.Resources["SidebarGradient"] = new LinearGradientBrush(
                    Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16),
                    90);
            }
            else
            {
                okno.Resources["BgApp"] = new SolidColorBrush(Color.FromRgb(0xF6, 0xF7, 0xFB));
                okno.Resources["Surface"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                okno.Resources["TextMain"] = new SolidColorBrush(Color.FromRgb(0x0B, 0x12, 0x20));
                okno.Resources["BorderSoft"] = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));
                okno.Resources["InputBg"] = new SolidColorBrush(Color.FromRgb(0xF8, 0xFA, 0xFC));

                okno.Resources["Orange"] = new SolidColorBrush(Color.FromRgb(0xF9, 0x73, 0x16));
                okno.Resources["OrangeHover"] = new SolidColorBrush(Color.FromRgb(0xEA, 0x58, 0x0C));

                okno.Resources["SidebarGradient"] = new LinearGradientBrush(
                    Color.FromRgb(0xFB, 0x92, 0x3C),
                    Color.FromRgb(0xF9, 0x73, 0x16),
                    90);
            }
        }
    }
}