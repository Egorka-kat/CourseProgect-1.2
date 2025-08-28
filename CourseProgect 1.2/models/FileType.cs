using CourseProgect_1._2.services.Localization;
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
    }
}
