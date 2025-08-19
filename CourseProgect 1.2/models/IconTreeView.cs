using FontAwesome.WPF;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CourseProgect_1._2.models
{
    public class IconTreeView : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOpen)
            {
                // Для булевых значений
                return isOpen ? FontAwesomeIcon.AngleDoubleRight : FontAwesomeIcon.AngleDoubleLeft;
            }
            // Значение по умолчанию
            return FontAwesomeIcon.AngleDoubleRight;
        }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
}
