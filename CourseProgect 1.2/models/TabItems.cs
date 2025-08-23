using CourseProgect_1._2.ViewModels.Base;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Markdig;
using System.Text;
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
            set
            {
                Set(ref _Path, value);
                UpdateSyntaxHighlightingFromExtension();
                OnPropertyChanged(nameof(UriPath));
            }
        }
        private Uri ConvertToDataUri(string markdownText)
        {
            string htmlContent = Markdig.Markdown.ToHtml(markdownText);

            // Создаем полный HTML документ со стилями
            string fullHtml = $@"
    <!DOCTYPE html>
    <html lang='ru'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Markdown Preview</title>
        <style>
            /* Base styles */
            body {{
                font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                line-height: 1.6;
                color: #333;
                max-width: 800px;
                margin: 0 auto;
                padding: 20px;
                background-color: #ffffff;
            }}
            
            /* Headings */
            h1, h2, h3, h4, h5, h6 {{
                margin-top: 1.5em;
                margin-bottom: 0.5em;
                font-weight: 600;
                line-height: 1.25;
            }}
            
            h1 {{ font-size: 2em; border-bottom: 2px solid #eaecef; padding-bottom: 0.3em; }}
            h2 {{ font-size: 1.5em; border-bottom: 1px solid #eaecef; padding-bottom: 0.3em; }}
            h3 {{ font-size: 1.25em; }}
            h4 {{ font-size: 1em; }}
            h5 {{ font-size: 0.875em; }}
            h6 {{ font-size: 0.85em; color: #6a737d; }}
            
            /* Paragraphs and text */
            p {{ margin: 1em 0; }}
            strong {{ font-weight: 600; }}
            em {{ font-style: italic; }}
            
            /* Lists */
            ul, ol {{
                padding-left: 2em;
                margin: 1em 0;
            }}
            
            li {{
                margin: 0.5em 0;
            }}
            
            li > p {{
                margin: 0.5em 0;
            }}
            
            /* Code blocks */
            pre {{
                background-color: #f6f8fa;
                border-radius: 6px;
                padding: 16px;
                overflow: auto;
                margin: 1em 0;
                line-height: 1.45;
            }}
            
            code {{
                font-family: 'SFMono-Regular', Consolas, 'Liberation Mono', Menlo, monospace;
                font-size: 0.85em;
                background-color: rgba(27,31,35,0.05);
                border-radius: 3px;
                padding: 0.2em 0.4em;
            }}
            
            pre code {{
                background: none;
                padding: 0;
            }}
            
            /* Blockquotes */
            blockquote {{
                border-left: 4px solid #dfe2e5;
                padding-left: 1em;
                margin: 1em 0;
                color: #6a737d;
            }}
            
            blockquote > :first-child {{
                margin-top: 0;
            }}
            
            blockquote > :last-child {{
                margin-bottom: 0;
            }}
            
            /* Tables */
            table {{
                border-collapse: collapse;
                width: 100%;
                margin: 1em 0;
            }}
            
            th, td {{
                border: 1px solid #dfe2e5;
                padding: 0.5em 1em;
                text-align: left;
            }}
            
            th {{
                background-color: #f6f8fa;
                font-weight: 600;
            }}
            
            tr:nth-child(even) {{
                background-color: #f6f8fa;
            }}
            
            /* Links */
            a {{
                color: #0366d6;
                text-decoration: none;
            }}
            
            a:hover {{
                text-decoration: underline;
            }}
            
            /* Horizontal rule */
            hr {{
                height: 1px;
                background-color: #eaecef;
                border: none;
                margin: 2em 0;
            }}
            
            /* Images */
            img {{
                max-width: 100%;
                height: auto;
                border-radius: 4px;
            }}
            
            /* Task lists */
            .task-list-item {{
                list-style-type: none;
            }}
            
            .task-list-item-checkbox {{
                margin: 0 0.2em 0.25em -1.6em;
                vertical-align: middle;
            }}
            
            /* Syntax highlighting (optional) */
            .hljs {{
                display: block;
                overflow-x: auto;
                padding: 0.5em;
                background: #f6f8fa;
                border-radius: 4px;
            }}
        </style>
        
        <!-- Подсветка синтаксиса (опционально) -->
        <link rel='stylesheet' 
              href='https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.7.0/styles/github.min.css'>
        <script src='https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.7.0/highlight.min.js'></script>
        <script>hljs.highlightAll();</script>
    </head>
    <body>
        <div class='markdown-body'>
            {htmlContent}
        </div>
        
        <script>
            // Автоматическая подсветка кода после загрузки
            document.addEventListener('DOMContentLoaded', function() {{
                if (typeof hljs !== 'undefined') {{
                    hljs.highlightAll();
                }}
                
                // Добавляем target='_blank' для внешних ссылок
                document.querySelectorAll('a').forEach(link => {{
                    if (link.href && !link.href.startsWith('#')) {{
                        link.target = '_blank';
                        link.rel = 'noopener noreferrer';
                    }}
                }});
            }});
        </script>
    </body>
    </html>";

            // Конвертируем в base64
            byte[] bytes1 = Encoding.UTF8.GetBytes(htmlContent);
            string base64Content1 = Convert.ToBase64String(bytes1);
            string dataUrl1 = $"data:text/html;base64,{base64Content1}";

            return new Uri(dataUrl1);
        }
        public Uri UriPath
        {
            get
            {
                if (string.IsNullOrEmpty(Path)) return null;
                try
                {
                    if (System.IO.Path.GetExtension(Path).ToLower() == ".md")
                    {
                        var pipeline = new Markdig.MarkdownPipelineBuilder()
       .UseAdvancedExtensions() // Включает таблицы, списки задач и др.
       .UseEmojiAndSmiley()     // Поддержка emoji
       .UseMathematics()        // Поддержка математических формул
       .UseAutoLinks()          // Автоматические ссылки
       .Build();

                        string htmlContent = Text;
                        //string html = Westwind.AspNetCore.Markdown.Markdown.Parse(htmlContent);
                        //byte[] bytes2 = Encoding.UTF8.GetBytes(html.ToString());
                        //string base64Content2 = Convert.ToBase64String(bytes2);
                        //// Остальной код такой же как выше...
                        //string dataUrl2 = $"data:text/markdown;base64,{base64Content2}";
                        return ConvertToDataUri(htmlContent);
                    }
                    if (System.IO.Path.GetExtension(Path).ToLower() == ".html")
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(Text);
                        string base64Content = Convert.ToBase64String(bytes);
                        string dataUrl = $"data:text/html;base64,{base64Content}";
                        //return new Uri($"data:text/html;base64,{Convert.ToBase64String(Encoding.ASCII.GetBytes(Text))}");
                        return (new Uri(dataUrl));
                    }
                    string formattedText = Text.Replace("&", "&amp;")
    .Replace("<", "&lt;")
    .Replace(">", "&gt;")
    .Replace("\"", "&quot;")
    .Replace("'", "&#39;")
    .Replace("\r\n", "<br>")
    .Replace("\n", "<br>")
    .Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;"); // 4 пробела для табуляции

                    // Создаем HTML с сохранением форматирования
                    string htmlContent1 = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8'>
        <style>
            body {{
                font-family: Consolas, 'Courier New', monospace;
                white-space: pre-wrap; /* Сохраняет пробелы и переносы */
                word-wrap: break-word;
                background-color: #f8f9fa;
                padding: 20px;
                line-height: 1.5;
            }}
            .text-content {{
                background-color: white;
                border: 1px solid #ddd;
                padding: 15px;
                border-radius: 5px;
                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            }}
        </style>
    </head>
    <body>
        <div class='text-content'>
            {formattedText}
        </div>
    </body>
    </html>";

                    byte[] bytes1 = Encoding.UTF8.GetBytes(htmlContent1);
                    string base64Content1 = Convert.ToBase64String(bytes1);
                    string dataUrl1 = $"data:text/html;base64,{base64Content1}";
                    //return new Uri($"data:text/html;base64,{Convert.ToBase64String(Encoding.ASCII.GetBytes(Text))}");
                    return (new Uri(dataUrl1));
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
                    {                        Document.Text = value;
                        
                    }
                    OnPropertyChanged(nameof(UriPath));
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
                ".md" => HighlightingManager.Instance.GetDefinition("Markdown"),
                _ => null
            };
            OnPropertyChanged(nameof(SyntaxHighlighting));
        }

        public bool isSave = true;

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
