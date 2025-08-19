using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.models;
using CourseProgect_1._2.Models;
using CourseProgect_1._2.ViewModels.Base;
using CourseProgect_1._2.views.Windows;
using CourseProgect_1._2.Views.Windows;
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
        private bool _isPanelClosed = false;
        public bool IsPanelClosed
        {
            get => _isPanelClosed;
            set
            {
                _isPanelClosed = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PanelIcon));
            }
        }

        public FontAwesomeIcon PanelIcon =>
            IsPanelClosed ? FontAwesomeIcon.AngleDoubleLeft : FontAwesomeIcon.AngleDoubleRight;
        public ICommand? ClosingTreeView { get; set; }
        private bool CanClosingTreeViewExecuted(object par) => true;
        public void OnClosingTreeViewExecuted(object par)
        {
            if (!Equals(par))
            {
                ColumnDefinition COL = par as ColumnDefinition;
                if (!IsPanelClosed)
                {
                    FixedWidth = COL.Width;
                    COL.Width = new GridLength(0);
                    IsPanelClosed = true;
                }
                else
                {
                    COL.Width = FixedWidth;
                    IsPanelClosed = false;
                }
            }
        }
        #endregion

        #region DeleteChildCommand
        public ICommand? DeleteCommand { get; set; }
        private bool CanDeleteCommandExecuted(object par) => true;
        public async void OnDeleteCommandExecuted(object par)
        {
            if (par is not FileSystemItem item) return;

            try
            {
                // Запрос подтверждения удаления
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить {(item.IsDirectory ? "папку" : "файл")} '{item.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                // Удаление файла/папки
                if (item.IsDirectory)
                {
                    Directory.Delete(item.FullPath, true);
                }
                else
                {
                    File.Delete(item.FullPath);
                }

                // Удаляем элемент из родительской коллекции в UI потоке
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    RemoveItemFromTree(item);
                });

                // Уведомление об успешном удалении
                MessageBox.Show($"'{item.Name}' успешно удален", "Успех",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Нет прав для удаления: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Файл используется другой программой: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveItemFromTree(FileSystemItem itemToDelete)
        {
            // Ищем родительский элемент в корневых элементах
            foreach (var rootItem in FileSystemItems)
            {
                if (RemoveFromChildren(rootItem, itemToDelete))
                {
                    return;
                }
            }
        }

        private bool RemoveFromChildren(FileSystemItem parent, FileSystemItem itemToDelete)
        {
            // Проверяем прямых детей
            if (parent.Children?.Contains(itemToDelete) == true)
            {
                parent.Children.Remove(itemToDelete);
                return true;
            }

            // Рекурсивно проверяем детей
            if (parent.Children != null)
            {
                foreach (var child in parent.Children.ToList()) // ToList() для избежания модификации во время итерации
                {
                    if (RemoveFromChildren(child, itemToDelete))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void RemoveItemFromParent(FileSystemItem itemToDelete)
        {
            // Ищем родительский элемент в дереве
            foreach (var rootItem in FileSystemItems)
            {
                var parent = FindParent(rootItem, itemToDelete);
                if (parent != null)
                {
                    parent.Children.Remove(itemToDelete);
                    return;
                }
            }
        }

        private FileSystemItem FindParent(FileSystemItem current, FileSystemItem target)
        {
            if (current.Children.Contains(target))
            {
                return current;
            }

            foreach (var child in current.Children)
            {
                var parent = FindParent(child, target);
                if (parent != null)
                {
                    return parent;
                }
            }

            return null;
        }
        #endregion

        public EW_ViewModel()
        {
            ClosingTreeView = new LambdaCommand(OnClosingTreeViewExecuted, CanClosingTreeViewExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecuted);
        }
    }
}
