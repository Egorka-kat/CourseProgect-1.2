using Markdig;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Применяем тёмную тему по умолчанию
            ThemeManager.ApplyDarkTheme();
            ThemeToggleButton.IsChecked = false;
            UpdateThemeInfo();
        }

        private void ThemeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyLightTheme();
            UpdateThemeInfo();
        }

        private void ThemeToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ThemeManager.ApplyDarkTheme();
            UpdateThemeInfo();
        }

        private void UpdateThemeInfo()
        {
            ThemeInfoText.Text = ThemeToggleButton.IsChecked == true ? "Светлая тема" : "Тёмная тема";
        }
    
}
}