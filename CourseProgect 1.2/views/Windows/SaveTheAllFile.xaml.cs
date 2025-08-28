using CourseProgect_1._2.models;
using CourseProgect_1._2.viewmodels;
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
    /// Логика взаимодействия для SaveTheAllFile.xaml
    /// </summary>
    public partial class SaveTheAllFile : Window
    {
        internal STAF_ViewModel ViewModel => (STAF_ViewModel)DataContext;
        public bool Closure = false;
        internal SaveTheAllFile(ObservableCollection<TabSystemItem> TabItems)
        {
            InitializeComponent();



            DataContext = new STAF_ViewModel(TabItems);
        }
        internal SaveTheAllFile(TabSystemItem TabItem)
        {
            InitializeComponent();


            DataContext = new STAF_ViewModel(TabItem);
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            var selectedItems = LB_SelectItem.SelectedItems.Cast<TabSystemItem>().ToList();
            ViewModel.Save.Execute(selectedItems);
            Closure = true;
            this.Close();
        }

        private void OnSaveButton(object sender, RoutedEventArgs e)
        {
            var selectedItems = LB_SelectItem.SelectedItems.Cast<TabSystemItem>().ToList();
            ViewModel.OnSave.Execute(selectedItems);
            Closure = true;
            this.Close();
        }

        private void CalcerButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
