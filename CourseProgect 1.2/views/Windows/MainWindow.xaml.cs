using CourseProgect_1._2.models;
using CourseProgect_1._2.Models;
using CourseProgect_1._2.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace CourseProgect_1._2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        internal MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;
        public MainWindow()
        {

            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }

        private void ListBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is Progect Item)
            {
                ViewModel.CommandOpenProgramm.Execute(Item);
            }
        }
    }
}
