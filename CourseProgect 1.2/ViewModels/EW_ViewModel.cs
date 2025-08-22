using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.models;
using CourseProgect_1._2.Models;
using CourseProgect_1._2.ViewModels.Base;
using CourseProgect_1._2.views.Windows;
using CourseProgect_1._2.Views.Windows;
using FontAwesome.WPF;
using ICSharpCode.AvalonEdit;
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
        public ObservableCollection<FileSystemItem> FileSystemItems { get; set; } = new ObservableCollection<FileSystemItem>();
        public ObservableCollection<TabSystemItem> TabItems { get; set; } = new ObservableCollection<TabSystemItem> 
        {  };
        private TabSystemItem _ActiveaTabSystemItem;
        public TabSystemItem ActiveaTabSystemItem
        {
            get => _ActiveaTabSystemItem;
            set 
            {
                Set(ref _ActiveaTabSystemItem, value);
                if (value is TabSystemItem tabItem)
                {
                    TabOpenedCommand?.Execute(tabItem.UriPath);
                }
            }
        }
        public ICommand? TabOpenedCommand { get; set; }
        private bool CaTabOpenedCommandExecuted(object par) => true;
        public void OTabOpenedCommandExecuted(object par)
        {
            if (par is not Uri item) return;
            
        }

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
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                Set(ref _selectedTabIndex, value);
            }
        }
        public void LoadDirectory(string path)
        {
            FileSystemItems.Clear();

            var directoryInfo = new DirectoryInfo(path);

            // Создаем корневой узел
            var rootItem = new FileSystemItem
            {
                Name = directoryInfo.Name,
                FullPath = directoryInfo.FullName,
                IsDirectory = true,
                Children = new ObservableCollection<FileSystemItem>()
            };

            FileSystemItems.Add(rootItem);

            // Загружаем содержимое корневой папки
            LoadSubdirectories(rootItem);
        }

        private void LoadSubdirectories(FileSystemItem parentItem)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(parentItem.FullPath);

                // Загрузка подпапок
                foreach (var dir in directoryInfo.GetDirectories())
                {
                    var item = new FileSystemItem
                    {
                        Name = dir.Name,
                        FullPath = dir.FullName,
                        IsDirectory = true,
                        Children = new ObservableCollection<FileSystemItem>()
                    };

                    parentItem.Children.Add(item);

                    // Рекурсивно загружаем вложенные папки (по необходимости)
                    // LoadSubdirectories(item);
                }

                // Загрузка файлов
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
            catch (UnauthorizedAccessException)
            {
                // Обработка отсутствия прав доступа
            }
            catch (DirectoryNotFoundException)
            {
                // Обработка отсутствия директории
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

        #region DeleteCommand
        private bool DeleteFileSystemItemInOC(FileSystemItem item, ObservableCollection<FileSystemItem> _FileSystemItems)
        {
            if (_FileSystemItems.Contains(item))  { item.Children.Clear(); ; _FileSystemItems.Remove(item); return true; }

            foreach (FileSystemItem item1 in _FileSystemItems) 
            {
                DeleteFileSystemItemInOC(item,item1.Children);
            }
            return false;
        }
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
                    if(DeleteFileSystemItemInOC(item, FileSystemItems))
                        return;
                }
                else
                {
                    File.Delete(item.FullPath);
                    DeleteTabItem(item.FullPath);
                    if (DeleteFileSystemItemInOC(item, FileSystemItems))
                        return;
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
        private void DeleteTabItem(string path)
        {
            try
            {

                foreach (var item in TabItems)
                {
                    if (item.Path == path)
                    {
                        TabItems.Remove(item);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion
        #region RenameCommand
        public ICommand? RenameCommand { get; set; }
        private bool CanRenameCommandExecuted(object par) => par is FileSystemItem item &&
                                                   !string.IsNullOrEmpty(item.FullPath) &&
                                                   (item.IsDirectory ? Directory.Exists(item.FullPath) : File.Exists(item.FullPath));
        public async void OnRenameCommandExecuted(object par)
        {
            if (par is not FileSystemItem item) return;

            try
            {
                // Используем VisualBasic InputBox
                string newName = await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    return Microsoft.VisualBasic.Interaction.InputBox(
                        "Введите новое имя:",
                        "Переименование",
                        item.Name);
                });

                if (string.IsNullOrEmpty(newName)) return;
                if (newName == item.Name) return;

                string directoryPath = System.IO.Path.GetDirectoryName(item.FullPath)!;
                string newFullPath = System.IO.Path.Combine(directoryPath, newName);
                string OldPath = item.FullPath;

                // Проверка на существование
                bool exists = await Task.Run(() => item.IsDirectory ?
                    Directory.Exists(newFullPath) : File.Exists(newFullPath));

                if (exists)
                {
                    MessageBox.Show("Файл или папка с таким именем уже существует", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Переименование
                await Task.Run(() =>
                {
                    if (item.IsDirectory)
                    {
                        Directory.Move(item.FullPath, newFullPath);
                    }
                    else
                    {
                        File.Move(item.FullPath, newFullPath);
                        RenameNameAndPathTabItem(OldPath, newFullPath);
                        item.Name = System.IO.Path.GetFileName(newFullPath);
                    }
                });

                //Обновление TreeView
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    item.Name = newName;
                    item.FullPath = newFullPath;
                    RefreshTreeView();
                });

                MessageBox.Show($"'{item.Name}' переименован в '{newName}'", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при переименовании: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RenameNameAndPathTabItem(string OldPath, string NewPath)
        {
            try
            {
                foreach (var item in TabItems)
                {
                    if (item.Path == OldPath)
                    {
                        item.TitleName = System.IO.Path.GetFileName(NewPath);
                        item.Path = NewPath;
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        private void RefreshTreeView()
        {
            foreach (var item in FileSystemItems.ToList())
            {
                RefreshFileSystemItem(item);
            }

            OnPropertyChanged(nameof(FileSystemItems));
        }
        private void RefreshFileSystemItem(FileSystemItem item)
        {
            if (item.IsDirectory)
            {
                // Проверяем существует ли директория
                if (!Directory.Exists(item.FullPath))
                {
                    // Удаляем несуществующий элемент
                    RemoveItemFromTree(item);
                    return;
                }

                // Обновляем детей директории
                RefreshDirectoryChildren(item);
            }
            else
            {
                // Проверяем существует ли файл
                if (!File.Exists(item.FullPath))
                {
                    RemoveItemFromTree(item);
                    return;
                }

                // Обновляем информацию о файле
                var fileInfo = new FileInfo(item.FullPath);
                item.Name = fileInfo.Name;
                OnPropertyChanged(nameof(item.Name));
                RefreshDirectoryChildren(item);
            }
        }

        private void RefreshDirectoryChildren(FileSystemItem directory)
        {
            if (!Directory.Exists(directory.FullPath)) return;

            try
            {
                // Получаем текущие файлы и папки
                var currentDirs = Directory.GetDirectories(directory.FullPath);
                var currentFiles = Directory.GetFiles(directory.FullPath);

                var currentPaths = currentDirs.Concat(currentFiles).ToHashSet();

                // Удаляем несуществующие элементы
                for (int i = directory.Children.Count - 1; i >= 0; i--)
                {
                    var child = directory.Children[i];
                    if (!currentPaths.Contains(child.FullPath))
                    {
                        directory.Children.RemoveAt(i);
                    }
                }

                // Добавляем новые элементы
                foreach (var dirPath in currentDirs)
                {
                    if (!directory.Children.Any(c => c.FullPath == dirPath))
                    {
                        var dirInfo = new DirectoryInfo(dirPath);
                        directory.Children.Add(new FileSystemItem
                        {
                            Name = dirInfo.Name,
                            FullPath = dirInfo.FullName,
                            IsDirectory = true,
                            Children = new ObservableCollection<FileSystemItem>()
                        });
                    }
                }

                foreach (var filePath in currentFiles)
                {
                    if (!directory.Children.Any(c => c.FullPath == filePath))
                    {
                        var fileInfo = new FileInfo(filePath);
                        directory.Children.Add(new FileSystemItem
                        {
                            Name = fileInfo.Name,
                            FullPath = fileInfo.FullName,
                            IsDirectory = false
                        });
                    }
                }

                // Рекурсивно обновляем детей
                foreach (var child in directory.Children.Where(c => c.IsDirectory))
                {
                    RefreshFileSystemItem(child);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Пропускаем директории без доступа
            }
        }
        #endregion

        #region OpenCommand
        public ICommand? OpenCommand { get; set; }
        private bool CanOpenCommandExecuted(object par) => true;
        public void OnOpenCommandExecuted(object par)
        {
            if (par is not FileSystemItem item) return;

            try
            {
                TabSystemItem tabSystemItem = new TabSystemItem(item.Name,item.FullPath);
                TabItems.Add(tabSystemItem);
                SelectedTabIndex = TabItems.IndexOf(tabSystemItem);
            }
            catch (Exception)
            {

            }
        }
        #endregion
        #region CloseTabCommand
        public ICommand? CloseTabCommand { get; set; }
        private bool CanCloseTabCommandExecuted(object par) => true;
        public void OnCloseTabCommandExecuted(object par)
        {
            if (par is not TabSystemItem item) return;
            TabItems.Remove(item);
        }
        #endregion

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

        public EW_ViewModel()
        {
            ClosingTreeView = new LambdaCommand(OnClosingTreeViewExecuted, CanClosingTreeViewExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecuted);
            RenameCommand = new LambdaCommand(OnRenameCommandExecuted, CanRenameCommandExecuted);
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted, CanOpenCommandExecuted);
            CloseTabCommand = new LambdaCommand(OnCloseTabCommandExecuted, CanCloseTabCommandExecuted);
        }
    }
}