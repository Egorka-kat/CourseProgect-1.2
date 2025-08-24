using CourseProgect_1._2.models;
using CourseProgect_1._2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ToCreateDirectoryWindow.xaml
    /// </summary>
    public partial class ToCreateDirectoryWindow : Window
    {
        internal TCDW_ViewModel ViewModel => (TCDW_ViewModel)DataContext;
        internal ToCreateDirectoryWindow(FileSystemItem TreeView)
        {
            InitializeComponent();
            DataContext = new TCDW_ViewModel(TreeView);
        }

        private void Calcer_Buttom(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Button(object sender, RoutedEventArgs e)
        {
            ViewModel.ToСreateDirectoryCommand.Execute(true);
            if (ViewModel.Closer == true)
            {
                this.Close();
            }
        }
    }
}
