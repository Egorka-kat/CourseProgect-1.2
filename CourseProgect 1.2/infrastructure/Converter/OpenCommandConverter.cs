using CourseProgect_1._2.models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace CourseProgect_1._2.infrastructure.Converter
{
    public class OpenCommandConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 &&
                values[0] is ICommand openCommand &&
                values[1] is FileSystemItem item)
            {
                // Проверяем, что это файл (не директория)
                if (!item.IsDirectory && openCommand.CanExecute(item))
                {
                    openCommand.Execute(item);
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
