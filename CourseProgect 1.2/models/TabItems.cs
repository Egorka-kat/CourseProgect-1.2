using CourseProgect_1._2.ViewModels.Base;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProgect_1._2.models
{
    class TabSystemItem : ViewModel
    {
        private string _TitleName;
        public string TitleName { get => _TitleName; set => Set(ref _TitleName, value); }
        private string _Path;
        public string Path
        {
            get => _Path;
            set { 
                Set(ref _Path, value);
                UpdateSyntaxHighlightingFromExtension();
                OnPropertyChanged(nameof(UriPath)); 
            }
        }
        public Uri UriPath
        {
            get
            {
                if (string.IsNullOrEmpty(Path)) return null;
                try
                {
                    string s = new Uri(Path).AbsoluteUri;
                    return new Uri(s);
                }
                catch
                {
                    return null;
                }
            }
        }

        private TextDocument _document;
        public TextDocument Document
        {
            get => _document;
            set
            {
                Set(ref _document, value);
            }
        }
        private string _Text;
        public string Text
        {
            get => _Text;
            set
            {
                if (Set(ref _Text, value))
                {
                    // Обновляем документ только если текст действительно изменился
                    if (Document.Text != value)
                    {
                        Document.Text = value;
                    }
                }
            }
        }

        public IHighlightingDefinition SyntaxHighlighting { get; private set; }

        private void UpdateSyntaxHighlightingFromExtension()
        {
            if (string.IsNullOrEmpty(Path))
            {
                SyntaxHighlighting = null;
                return;
            }

            var extension = System.IO.Path.GetExtension(Path).ToLower();

            SyntaxHighlighting = extension switch
            {
                ".cs" => HighlightingManager.Instance.GetDefinition("C#"),
                ".html" or ".htm" => HighlightingManager.Instance.GetDefinition("HTML"),
                ".xml" => HighlightingManager.Instance.GetDefinition("XML"),
                ".js" => HighlightingManager.Instance.GetDefinition("JavaScript"),
                ".sql" => HighlightingManager.Instance.GetDefinition("SQL"),
                ".css" => HighlightingManager.Instance.GetDefinition("CSS"),
                ".py" => HighlightingManager.Instance.GetDefinition("Python"),
                _ => null
            };

            OnPropertyChanged(nameof(SyntaxHighlighting));
        }

        public bool isSave;

        public TabSystemItem(string TitleName, string Path)
        {
            this.TitleName = TitleName;
            this.Path = Path;
            Document = new TextDocument();
            Document.TextChanged += (s, e) =>
            {
                // Обновляем строковое свойство при изменении документа
                Text = Document.Text;
            };

            // Инициализация начальным текстом
            Text = ReadFile();
        }
        public string ReadFile()
        {
            try
            {
                return System.IO.File.ReadAllText(Path, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                return string.Empty;
            }
        }
    }
}
