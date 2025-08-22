using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;
using System.Windows.Input;

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
            // Опционально: Загрузить текст при старте
            InitializeAsync();
        }

        // Загружает пример кода C# в редактор
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

                    // Обработка новых окон
                    webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

                    // Обработка ошибок навигации
                    webView.CoreWebView2.NavigationCompleted += (sender, args) =>
                    {
                        if (!args.IsSuccess)
                        {
                            StatusText.Text = $"Ошибка навигации: {args.WebErrorStatus}";
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации WebView2: {ex.Message}");
            }
        }

        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            // Открываем новые окна в основном WebView
            e.Handled = true;
            webView.CoreWebView2.Navigate(e.Uri);
        }

        private void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            StatusText.Text = "Загрузка...";
            ProgressBar.IsIndeterminate = true;
        }

        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            StatusText.Text = "Загрузка завершена";
            ProgressBar.IsIndeterminate = false;
            UrlTextBox.Text = webView.Source?.ToString();

            // Обновляем состояние кнопок навигации
            BackButton.IsEnabled = webView.CanGoBack;
            ForwardButton.IsEnabled = webView.CanGoForward;
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                StatusText.Text = $"Ошибка инициализации: {e.InitializationException?.Message}";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoBack)
            {
                webView.GoBack();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoForward)
            {
                webView.GoForward();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            webView.Reload();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUrl();
        }

        private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToUrl();
            }
        }

        private void NavigateToUrl()
        {
            string url = UrlTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(url))
            {
                // Добавляем https:// если протокол не указан
                if (!url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("file://"))
                {
                    url = "https://" + url;
                }

                try
                {
                    webView.Source = new Uri(url);
                }
                catch (UriFormatException)
                {
                    StatusText.Text = "Неверный URL адрес";
                }
            }
        }

        // Пример выполнения JavaScript
        private async void ExecuteJavaScriptExample()
        {
            if (webView.CoreWebView2 != null)
            {
                string result = await webView.CoreWebView2.ExecuteScriptAsync("document.title");
                MessageBox.Show($"Title: {result}");
            }
        }

        // Пример загрузки локального HTML
        private void LoadLocalHtml()
        {
            string htmlContent = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Локальная страница</title>
                    <style>
                        body { font-family: Arial, sans-serif; padding: 20px; }
                        h1 { color: #0078d4; }
                    </style>
                </head>
                <body>
                    <h1>Привет из WebView2!</h1>
                    <p>Это локальная HTML страница</p>
                </body>
                </html>";

            webView.NavigateToString(htmlContent);
        }

        protected override void OnClosed(EventArgs e)
        {
            // Очистка ресурсов WebView2
            webView?.Dispose();
            base.OnClosed(e);
        }
    }
}