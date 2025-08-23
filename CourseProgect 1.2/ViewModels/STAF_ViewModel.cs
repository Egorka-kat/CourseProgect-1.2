using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.models;
using CourseProgect_1._2.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CourseProgect_1._2.ViewModels
{
    internal class STAF_ViewModel : ViewModel
    {
        public ObservableCollection<TabSystemItem> TabItems { get; set; } = new ObservableCollection<TabSystemItem>
        { };
        private TabSystemItem _ActiveaTabSystemItem;
        public TabSystemItem ActiveaTabSystemItem
        {
            get => _ActiveaTabSystemItem;
            set
            {
                Set(ref _ActiveaTabSystemItem, value);
            }
        }
        public ICommand? Save { get; set; }
        private bool CanSaveExecuted(object par) => true;
        public void OnSaveExecuted(object par)
        {
            if (par is not List<TabSystemItem> item) return;
            if (item.Count == 0)
            {
                foreach (var item1 in TabItems)
                {
                    SaveInFile(item1);
                }
            }
            else
            {
                foreach (var item1 in item)
                {
                    SaveInFile(item1);
                }
            }
        }
        private void SaveInFile(TabSystemItem item)
        {
            try
            {
                if (item.isSave == false)
                {
                    using (StreamWriter writer = new StreamWriter(item.Path, false))
                    {
                        writer.WriteLine(item.Text);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Ошибка при сохранении {item.TitleName}: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public ICommand? OnSave { get; set; }
        private bool CanOnSaveExecuted(object par) => true;
        public void OnOnSaveExecuted(object par)
        {
            if (par is not List<TabSystemItem> item) return;
            if (item.Count == 0)
            {
                foreach (var item1 in item)
                {
                    SaveInFile(item1);
                }
            }
            else
            {
                foreach (var item1 in TabItems)
                {
                    if (item.Contains(item1)) continue;
                    SaveInFile(item1);
                }
            }
        }
        public STAF_ViewModel(ObservableCollection<TabSystemItem> TabItems) 
        { 
            this.TabItems = TabItems;
            Save = new LambdaCommand(OnSaveExecuted, CanSaveExecuted);
            OnSave = new LambdaCommand(OnOnSaveExecuted, CanOnSaveExecuted);
        }
        public STAF_ViewModel(TabSystemItem TabItem)
        {
            this.TabItems.Add(TabItem);
            Save = new LambdaCommand(OnSaveExecuted, CanSaveExecuted);
            OnSave = new LambdaCommand(OnOnSaveExecuted,CanOnSaveExecuted);
        }

    }

}
