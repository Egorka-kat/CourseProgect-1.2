using FontAwesome.WPF;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CourseProgect_1._2.models
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDirectory)
            {
                return isDirectory
                    ? FontAwesomeIcon.Folder.ToString()  // Иконка папки
                    : FontAwesomeIcon.FileOutline.ToString();    // Иконка файла
            }
            return FontAwesomeIcon.Question.ToString();   // Иконка по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
