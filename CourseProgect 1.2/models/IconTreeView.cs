using FontAwesome.WPF;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CourseProgect_1._2.models
{
    class IconTreeView : IValueConverter
    {
        public bool isClosingTreeView = false;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDirectory)
            {
                return isDirectory
                    ? FontAwesomeIcon.AngleDoubleLeft.ToString()  
                    : FontAwesomeIcon.AngleDoubleRight.ToString();    
            }
            return FontAwesomeIcon.Question.ToString();   // Иконка по умолчанию
        }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
}
