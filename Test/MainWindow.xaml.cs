using Microsoft.Win32;
using System.Windows;
using System.IO;

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
            LoadSampleCode();
        }

        // Загружает пример кода C# в редактор
        private void LoadSampleCode()
        {
            string sampleCode =
@"<!DOCTYPE html>
<html>
<head>
    <title>Моя первая веб-страница</title>
</head>
<body>
    <h1>Добро пожаловать на мою первую веб-страницу!</h1>
    <p>Это пример простого HTML-документа.</p>
</body>
</html>";
            TextEditor.Text = sampleCode;
        }

        // Обработчик кнопки "Получить текст"
        private void BtnGetText_Click(object sender, RoutedEventArgs e)
        {
            // Получаем весь текст из редактора
            string fullText = TextEditor.Text;

            // Получаем текст на текущей строке (демонстрация работы с Document)
            var currentLine = TextEditor.Document.GetLineByNumber(TextEditor.TextArea.Caret.Line);
            string lineText = TextEditor.Document.GetText(currentLine.Offset, currentLine.Length);

            MessageBox.Show(
                $"Текст текущей строки:\n{lineText}\n\n" +
                $"Полный текст ({fullText.Length} символов) получен и готов для обработки.",
                "Текст из AvalonEdit");
        }

        // Обработчик кнопки "Задать текст"
        private void BtnSetText_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CS Files (*.cs)|*.cs|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Читаем файл и устанавливаем его содержимое в редактор
                    string fileText = File.ReadAllText(openFileDialog.FileName);
                    TextEditor.Text = fileText;

                    // AvalonEdit автоматически определит расширение файла 
                    // и попытается применить соответствующую подсветку.
                    // Можно задать принудительно:
                    // TextEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка чтения файла: {ex.Message}", "Ошибка");
                }
            }
        }
    }
}