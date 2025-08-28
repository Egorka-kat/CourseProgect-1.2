using CourseProgect_1._2.models;
using CourseProgect_1._2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseProgect_1._2.views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ToCreateFileWindow.xaml
    /// </summary>
    public partial class ToCreateFileWindow : Window
    {
        internal TCFW_ViewModel ViewModel => (TCFW_ViewModel)DataContext;
        internal ToCreateFileWindow(FileSystemItem item)
        {
            InitializeComponent();
            DataContext = new TCFW_ViewModel(item);
        }

        private void Calcer_Button(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Button(object sender, RoutedEventArgs e)
        {
            ViewModel.CreateCommand.Execute(true);
            if (ViewModel.Closer == true)
            {
                this.Close();
            }
        }
    }
}
