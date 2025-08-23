using CourseProgect_1._2.models;
using CourseProgect_1._2.ViewModels;
using ICSharpCode.AvalonEdit;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CourseProgect_1._2.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private readonly string _PathParameter;
        internal EW_ViewModel ViewModel => (EW_ViewModel)DataContext;

        public EditWindow(string path)
        {
            _PathParameter = path;
            DataContext = new EW_ViewModel();
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

        private void StackPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is Border border &&
        border.Tag is FileSystemItem item &&
        !item.IsDirectory &&
        DataContext is EW_ViewModel viewModel &&
        viewModel.OpenCommand?.CanExecute(item) == true)
            {
                viewModel.OpenCommand.Execute(item);
                e.Handled = true;
            }
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            var editor = (TextEditor)sender;

            if (ViewModel.TextChangedCommand.CanExecute(editor.Tag))
            {
                ViewModel.TextChangedCommand.Execute(editor.Tag);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.SaveToClosed.Execute(true);
            if (ViewModel.isClosed == false)
            {
                e.Cancel = true;
            }
            
        }
    }
}
