using System.Windows.Media;
using System.Windows;

namespace Test
{
    internal class ThemeManager
    {
        public static void ApplyDarkTheme()
        {
            Application.Current.Resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));
            Application.Current.Resources["ForegroundBrush"] = new SolidColorBrush(Color.FromRgb(0xD0, 0xD0, 0xD0));
            Application.Current.Resources["PrimaryBrush"] = new SolidColorBrush(Color.FromRgb(0xBB, 0x86, 0xFC));
            Application.Current.Resources["SecondaryBrush"] = new SolidColorBrush(Color.FromRgb(0x03, 0xDA, 0xC6));
            Application.Current.Resources["CardBrush"] = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x30));
            Application.Current.Resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(0x3E, 0x3E, 0x42));
        }

        public static void ApplyLightTheme()
        {
            Application.Current.Resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            Application.Current.Resources["ForegroundBrush"] = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            Application.Current.Resources["PrimaryBrush"] = new SolidColorBrush(Color.FromRgb(0x62, 0x00, 0xEE));
            Application.Current.Resources["SecondaryBrush"] = new SolidColorBrush(Color.FromRgb(0x03, 0xDA, 0xC6));
            Application.Current.Resources["CardBrush"] = new SolidColorBrush(Color.FromRgb(0xF5, 0xF5, 0xF5));
            Application.Current.Resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
        }
    }
}
