using CourseProgect_1._2.services.Localization;
using CourseProgect_1._2.Services;
using ICSharpCode.AvalonEdit.Search;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CourseProgect_1._2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
   
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Инициализируем тему при запуске приложения
            ThemeService.Initialize();
        }
    }
    
}
