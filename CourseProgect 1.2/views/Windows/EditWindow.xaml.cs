using CourseProgect_1._2.ViewModels;
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

namespace CourseProgect_1._2.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private readonly string _PathParameter;
        public EditWindow(string path)
        {
            _PathParameter = path;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is EW_ViewModel vm)
            {
                vm.LoadPath = _PathParameter;
            }
        }
        async void InitializeAsync()
        {
            try
            {
                // Инициализация WebView2
                await webView.EnsureCoreWebView2Async(null);

                // Настройка дополнительных параметров после инициализации
                if (webView.CoreWebView2 != null)
                {
                    // Включение DevTools
                    webView.CoreWebView2.Settings.AreDevToolsEnabled = true;


                    // Обработка ошибок навигации
                    webView.CoreWebView2.NavigationCompleted += (sender, args) =>
                    {
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации WebView2: {ex.Message}");
            }
        }

    }
}
