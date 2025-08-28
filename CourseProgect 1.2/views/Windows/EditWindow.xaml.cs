using CourseProgect_1._2.models;
using CourseProgect_1._2.Services;
using CourseProgect_1._2.ViewModels;
using ICSharpCode.AvalonEdit;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
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
            ThemeService.ApplyTheme(ThemeService.CurrentTheme, true);

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

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DockPanel dockPanel && dockPanel.DataContext is TabSystemItem tabItem)
            {
                ViewModel.TabDragTabItem.Execute(tabItem);
                DragDrop.DoDragDrop(dockPanel, tabItem, DragDropEffects.Copy);
            }

        }

        private void DockPanel_Drop(object sender, DragEventArgs e)
        {
            if (sender is DockPanel dockPanel && dockPanel.DataContext is TabSystemItem tabItem)
            {
                ViewModel.TabDropTabItem.Execute(tabItem);
            }
        }

        private void TreeViewStackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is StackPanel stackPanel && stackPanel.DataContext is FileSystemItem TreeView)
            {
                ViewModel.DragTreeItem.Execute(TreeView);
                DragDrop.DoDragDrop(stackPanel, TreeView, DragDropEffects.Copy);
            }
        }

        private void TreeViewStackPanel_Drop(object sender, DragEventArgs e)
        {
            if (sender is StackPanel stackPanel && stackPanel.DataContext is FileSystemItem TreeView)
            {
                ViewModel.DropTreeItem.Execute(TreeView);
            }
        }

        private void webView_Loaded(object sender, RoutedEventArgs e)
        {
            var webView = sender as WebView2;
            if (webView != null && webView.CoreWebView2 == null)
            {
                // Отложенная инициализация через задачу
                _ = InitializeWebViewAsync(webView);
            }
        }

        private async Task InitializeWebViewAsync(WebView2 webView)
        {
            try
            {
                var browserFolder = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "YourAppName",
                    "WebView2");

                var environment = await CoreWebView2Environment.CreateAsync(null, browserFolder);
                await webView.EnsureCoreWebView2Async(environment);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации WebView2: {ex.Message}");
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var clickedItem = sender as MenuItem;
            if (clickedItem != null && clickedItem.Header != null)
            {
                string language = clickedItem.Header.ToString();
                // Изменяем язык или выполняем другие действия

                ViewModel.ChangeLanguageCommand.Execute(language);
            }
        }

    }   
}
