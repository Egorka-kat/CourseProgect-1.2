using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.models;
using CourseProgect_1._2.Models;
using CourseProgect_1._2.ViewModels.Base;
using CourseProgect_1._2.views.Windows;
using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CourseProgect_1._2.ViewModels
{
    class EW_ViewModel : ViewModel
    {
        public ObservableCollection<FileSystemItem> FileSystemItems { get; } = new ObservableCollection<FileSystemItem>();
        IconTreeView IconTreeView = new IconTreeView();
       
        private GridLength _ColumnWigth = new GridLength(200);
        public GridLength ColumnWigth
        {
            get => _ColumnWigth;
            set
            {
                _ColumnWigth = value;
                OnPropertyChanged("ColumnWigth");
            }
        }
        private GridLength FixedWidth;
        private string _LoadPath;
        public string LoadPath
        {
            get => _LoadPath;
            set
            {
                _LoadPath = value;
                Set(ref _LoadPath, value);
                // Можно вызвать здесь метод загрузки данных

                LoadDirectory(_LoadPath);
            }
        }
        public void LoadDirectory(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            foreach (var dir in directoryInfo.GetDirectories())
            {
                var item = new FileSystemItem
                {
                    Name = dir.Name,
                    FullPath = dir.FullName,
                    IsDirectory = true
                };
                FileSystemItems.Add(item);
                LoadSubdirectories(item); // Рекурсивная загрузка вложенных папок
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                FileSystemItems.Add(new FileSystemItem
                {
                    Name = file.Name,
                    FullPath = file.FullName,
                    IsDirectory = false
                });
            }
        }
        private void LoadSubdirectories(FileSystemItem parentItem)
        {
            var directoryInfo = new DirectoryInfo(parentItem.FullPath);

            foreach (var dir in directoryInfo.GetDirectories())
            {
                var item = new FileSystemItem
                {
                    Name = dir.Name,
                    FullPath = dir.FullName,
                    IsDirectory = true
                };
                parentItem.Children.Add(item);
                LoadSubdirectories(item); // Рекурсивно добавляем вложенные элементы
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                parentItem.Children.Add(new FileSystemItem
                {
                    Name = file.Name,
                    FullPath = file.FullName,
                    IsDirectory = false
                });
            }
        }


        #region ClosingTreeView
        public ICommand? ClosingTreeView { get; set; }
        private bool CanClosingTreeViewExecuted(object par) => true;
        public void OnClosingTreeViewExecuted(object par)
        {
            if (!Equals(par))
            {
                MenuItem MI = par as MenuItem;
                if (!IconTreeView.isClosingTreeView)
                {
                    MI.Icon = new ImageAwesome
                    {
                        Icon = FontAwesomeIcon.AngleDoubleRight,
                        Height = 7
                    };
                    FixedWidth = ColumnWigth;
                    ColumnWigth = new GridLength(10);
                    IconTreeView.isClosingTreeView = true;
                }
                else
                {
                    MI.Icon = new ImageAwesome
                    {
                        Icon = FontAwesomeIcon.AngleDoubleLeft,
                        Height = 7
                    };
                    ColumnWigth = FixedWidth;
                    IconTreeView.isClosingTreeView = false;
                }
            }
        }
        #endregion


        public EW_ViewModel()
        {
            ClosingTreeView = new LambdaCommand(OnClosingTreeViewExecuted, CanClosingTreeViewExecuted);
        }
    }
}
