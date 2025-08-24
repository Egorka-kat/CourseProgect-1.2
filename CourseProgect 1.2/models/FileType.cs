using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProgect_1._2.models
{
    public enum FileType
{
    Text,
    Markdown,
    Html,
    Json,
    Xml,
    CSharp,
    Custom
}

public static class FileTypeExtensions
{
    public static string GetExtension(this FileType fileType)
    {
        return fileType switch
        {
            FileType.Text => ".txt",
            FileType.Markdown => ".md",
            FileType.Html => ".html",
            FileType.Json => ".json",
            FileType.Xml => ".xml",
            FileType.CSharp => ".cs",
            FileType.Custom => string.Empty,
            _ => ".txt"
        };
    }

    public static string GetDisplayName(this FileType fileType)
    {
        return fileType switch
        {
            FileType.Text => "Обычный текст (.txt)",
            FileType.Markdown => "Markdown (.md)",
            FileType.Html => "HTML (.html)",
            FileType.Json => "JSON (.json)",
            FileType.Xml => "XML (.xml)",
            FileType.CSharp => "Код C# (.cs)",
            FileType.Custom => "Другой...",
            _ => "Обычный текст (.txt)"
        };
    }
}
}
