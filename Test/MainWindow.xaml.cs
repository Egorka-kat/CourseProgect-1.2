using Markdig;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using System;
using System.IO;
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
            ConfigureMarkdown();
            LoadWelcomeMessage();
        }

        private void ConfigureMarkdown()
        {
            // Настраиваем pipeline с основными расширениями
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            MarkdownViewer.Pipeline = pipeline;
        }

        private void LoadWelcomeMessage()
        {
            string welcomeMd = @"# 📝 Simple MD Viewer

Простое приложение для просмотра Markdown файлов.

## Как использовать:
1. Нажмите кнопку **'Open MD File'**
2. Выберите любой .md файл
3. Наслаждайтесь чтением!

### Поддерживаемые возможности:
- ✅ **Жирный текст** и *курсив*
- ✅ Списки (нумерованные и маркированные)
- ✅ Кодовые блоки
- ✅ Таблицы
- ✅ Ссылки и изображения

> Просто и удобно!";

            MarkdownViewer.Markdown = welcomeMd;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*",
                Title = "Select Markdown File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadMarkdownFile(openFileDialog.FileName);
            }
        }

        private void LoadMarkdownFile(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                MarkdownViewer.Markdown = content;
                Title = $"Simple MD Viewer - {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}