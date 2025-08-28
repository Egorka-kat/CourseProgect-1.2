using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.models;
using CourseProgect_1._2.services.Localization;
using CourseProgect_1._2.Services;
using CourseProgect_1._2.ViewModels.Base;
using CourseProgect_1._2.views.Windows;
using CourseProgect_1._2.Views.Windows;
using FontAwesome.WPF;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CourseProgect_1._2.ViewModels
{
    class EW_ViewModel : ViewModel
    {
        private Theme _currentTheme = ThemeService.CurrentTheme;

        public bool IsLightTheme => _currentTheme == Theme.Light;
        public bool IsDarkTheme => _currentTheme == Theme.Dark;

        public string StringTheme => localization["Theme"];
        public string StringLightTheme => localization["Light theme"];
        public string StringDarkTheme => localization["Dark theme"];
        private void UpdateThemeProperties()
        {
            OnPropertyChanged(nameof(IsLightTheme));
            OnPropertyChanged(nameof(IsDarkTheme));
        }
        private void OnThemeChanged()
        {
            // Обновляем свойства при изменении темы
            UpdateThemeProperties();
        }

        public ICommand ChangeThemeCommand { get; set; }
        private bool CanChangeThemeCommandExecuted(object par) => true;
        public void OnChangeThemeCommandExecuted(object par)
        {
            if (par is string themeName)
            {
                var theme = themeName == "Dark" ? Theme.Dark : Theme.Light;
                ThemeService.ApplyTheme(theme);
            }
        }

        #region Переменные для TreeView
        public ObservableCollection<FileSystemItem> FileSystemItems { get; set; } = new ObservableCollection<FileSystemItem>();
        private FileSystemItem DragFileItem;
        #endregion
        #region Переменные для TabControl
        public ObservableCollection<TabSystemItem> TabItems { get; set; } = new ObservableCollection<TabSystemItem> 
        { };
        private TabSystemItem _ActiveaTabSystemItem;
        private TabSystemItem DragTabItem;
        #endregion
        #region перевод
        static ALocalization localization = new ALocalization();
        public ObservableCollection<string> loc
        {
            get => localization.language;
            set { Set(ref localization.language, value); }
        }
        public string StringFile => localization["File"];
        public string StringOpenAProject => localization["Open a project"];
        public string StringSaveEverything => localization["Save everything"];
        public string StringSave => localization["Save"];
        public string StringLanguage => localization["Language"];


        public string StringOpen => localization["Open"];
        public string StringCreateAFile => localization["Create a file"];
        public string StringCreateAFolder => localization["Create a folder"];
        public string StringOpenInFileExplorer => localization["Open in File Explorer"];
        public string StringRename => localization["Rename"];
        public string StringDelete => localization["Delete"];
        private void OnLanguageChanged()
        {

            #region Перевод верхнего меню
            OnPropertyChanged(nameof(StringFile));
            OnPropertyChanged(nameof(StringOpenAProject));
            OnPropertyChanged(nameof(StringSaveEverything));
            OnPropertyChanged(nameof(StringSave));

            OnPropertyChanged(nameof(StringDarkTheme));
            OnPropertyChanged(nameof(StringLightTheme));
            OnPropertyChanged(nameof(StringTheme));

            OnPropertyChanged(nameof(StringLanguage));
            #endregion
            #region Перевод ContextMenu для TreeView
            OnPropertyChanged(nameof(StringOpen));
            OnPropertyChanged(nameof(StringCreateAFile));
            OnPropertyChanged(nameof(StringCreateAFolder));
            OnPropertyChanged(nameof(StringOpenInFileExplorer));
            OnPropertyChanged(nameof(StringRename));
            OnPropertyChanged(nameof(StringDelete));
            #endregion
            UpdateThemeProperties();
        }
        #endregion

        public bool isClosed = true;
        public TabSystemItem ActiveaTabSystemItem
        {
            get => _ActiveaTabSystemItem;
            set 
            {
                Set(ref _ActiveaTabSystemItem, value);

            }
        }

        private string _LoadPath;
        public string LoadPath
        {
            get => _LoadPath;
            set
            {
                _LoadPath = value;
                Set(ref _LoadPath, value);
                LoadDirectory(_LoadPath);
                SorterNameSystemFile(FileSystemItems);
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
        private string NameProgect;
        private string CoreFileProgect;
        public void LoadDirectory(string path)
        {
            
            FileSystemItems.Clear();

            var directoryInfo = new DirectoryInfo(path);
            NameProgect = directoryInfo.Name;
            var rootItem = new FileSystemItem
            {
                Name = directoryInfo.Name,
                FullPath = directoryInfo.FullName,
                IsDirectory = true,
                Children = new ObservableCollection<FileSystemItem>()
            };

            FileSystemItems.Add(rootItem);

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
                    LoadSubdirectories(item);
                }

                // Загрузка файлов
                foreach (var file in directoryInfo.GetFiles())
                {
                    if (Path.GetExtension(file.FullName) == ".xml" && NameProgect == Path.GetFileNameWithoutExtension(file.FullName))
                    {
                        CoreFileProgect = file.FullName;
                        continue;
                    }
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

        private GridLength FixedWidth;
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
        #region ClosingWebView
        private bool _isWebViewClosed = false;
        public bool IsWebViewClosed
        {
            get => _isWebViewClosed;
            set
            {
                _isWebViewClosed = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PanelIconWebView));
            }
        }

        public FontAwesomeIcon PanelIconWebView =>
            IsWebViewClosed ? FontAwesomeIcon.AngleDoubleRight : FontAwesomeIcon.AngleDoubleLeft;
        public ICommand? ClosingWebView { get; set; }
        private bool CanClosingWebViewExecuted(object par) => true;

        private GridLength FixedWidthWebView;
        public void OnClosingWebViewExecuted(object par)
        {
            if (!Equals(par))
            {
                ColumnDefinition COL = par as ColumnDefinition;
                if (!IsPanelClosed)
                {
                    FixedWidthWebView = COL.Width;
                    COL.Width = new GridLength(0);
                    IsPanelClosed = true;
                }
                else
                {
                    COL.Width = FixedWidthWebView;
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
                    $"{localization["Are you sure you want to delete"]} {(item.IsDirectory ? localization["the folder"] : localization["file"])} '{item.Name}'?",
                    localization["Confirmation of deletion"],
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

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    RemoveItemFromTree(item);
                });

                MessageBox.Show($"'{item.Name}' {localization["successfully deleted"]}", localization["Success"],
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"{localization["I don't have the rights to delete it"]}: {ex.Message}", localization["Error"],
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"{localization["The file is being used by another program"]}: {ex.Message}", localization["Error"],
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{localization["Error when deleting"]}: {ex.Message}", localization["Error"],
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


            if (!File.Exists(item.FullPath))
            {
                RemoveItemFromTree(item);
                return;
            }
            try
            {
                string newName = await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    return Microsoft.VisualBasic.Interaction.InputBox(
                        $"{localization["Enter a new name"]}:",
                        $"{localization["Renaming"]}",
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
                    MessageBox.Show(localization["A file or folder with that name already exists"], localization["Error"],
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    item.Name = newName;
                    item.FullPath = newFullPath;
                    RefreshTreeView();
                });

                MessageBox.Show($"'{item.Name}' {localization["renamed to"]} '{newName}'", localization["Success"],
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{localization["Error when renaming"]}: {ex.Message}", localization["Error"],
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
            SorterNameSystemFile(FileSystemItems);
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
                if (!Directory.Exists(item.FullPath))
                {
                    RemoveItemFromTree(item);
                    return;
                }

                RefreshDirectoryChildren(item);
            }
            else
            {
                if (!File.Exists(item.FullPath))
                {
                    RemoveItemFromTree(item);
                    return;
                }

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
                var currentDirs = Directory.GetDirectories(directory.FullPath);
                var currentFiles = Directory.GetFiles(directory.FullPath);

                var currentPaths = currentDirs.Concat(currentFiles).ToHashSet();

                for (int i = directory.Children.Count - 1; i >= 0; i--)
                {
                    var child = directory.Children[i];
                    if (!currentPaths.Contains(child.FullPath))
                    {
                        directory.Children.RemoveAt(i);
                    }
                }

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

                foreach (var child in directory.Children.Where(c => c.IsDirectory))
                {
                    RefreshFileSystemItem(child);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
        #endregion
        #region OpenExplorer
        public ICommand? OpenExplorer { get; set; }
        private bool CanOpenExplorerExecuted(object par) => true;
        public void OnOpenExplorerExecuted(object par)
        {
            if (par is not FileSystemItem item) return;
            OpenFileLocation(item.FullPath);
            SorterNameSystemFile(FileSystemItems);
        }
        #endregion
        #region OpenCommand
        public ICommand? OpenCommand { get; set; }
        private bool CanOpenCommandExecuted(object par) => true;
        public void OnOpenCommandExecuted(object par)
        {
            if (par is not FileSystemItem item || item.IsDirectory == true) return;

            if (!File.Exists(item.FullPath))
            {
                RemoveItemFromTree(item);
                return;
            }
            try
            {
                TabSystemItem tabSystemItem = new TabSystemItem(item.Name, item.FullPath);
                if (CheckingOnOpenCommand(tabSystemItem))
                {
                    SelectedTabIndex = TabItems.IndexOf(GetTabSystemItemToPath(tabSystemItem));
                    return;
                }

                TabItems.Add(tabSystemItem);
                SelectedTabIndex = TabItems.IndexOf(tabSystemItem);
            }
            catch (Exception)
            {

            }
        }
        private TabSystemItem GetTabSystemItemToPath(TabSystemItem tabSystemItem)
        {
            foreach (var item in TabItems)
            {
                if (item.Path == tabSystemItem.Path)
                {
                    return item;
                }
            }
            return null;
        }
        private bool CheckingOnOpenCommand(TabSystemItem item)
        {
            foreach (var item1 in TabItems)
            {
                if (item1.Path == item.Path)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region SaveToClosed
        public ICommand? SaveToClosed { get; set; }
        private bool CanSaveToClosedExecuted(object par) => true;
        public void OnSaveToClosedExecuted(object par)
        {
            ObservableCollection<TabSystemItem> SaveItem = new ObservableCollection<TabSystemItem>();
            
            foreach (var item in TabItems)
            {
                if (item.isSave == false)
                {
                    SaveItem.Add(item);
                }
            }
            if (SaveItem.Count != 0)
            {
                SaveTheAllFile saveTheAllFile = new SaveTheAllFile(SaveItem);
                saveTheAllFile.ShowDialog();
                if (saveTheAllFile.Closure == true)
                    isClosed = true;
                else isClosed = false;
            }
        }
        #endregion
        #region CloseTabCommand
        public ICommand? CloseTabCommand { get; set; }
        private bool CanCloseTabCommandExecuted(object par) => true;
        public void OnCloseTabCommandExecuted(object par)
        {
            if (par is not TabSystemItem item) return;
            if (item.isSave == false)
            {
                SaveTheAllFile saveTheAllFile = new SaveTheAllFile(item);
                saveTheAllFile.ShowDialog();
                if (saveTheAllFile.Closure == true)
                    TabItems.Remove(item);
                return;
            }
            TabItems.Remove(item);
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
                    if (item.TitleName[item.TitleName.Length - 1] == '*') 
                        item.TitleName = item.TitleName.Remove(item.TitleName.Length - 1);
                    item.isSave = true;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"{localization["Error when saving"]} {item.TitleName}: {ex.Message}", localization["Error"],
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #region CloseProgramm
        public ICommand? CloseProgramm { get; set; }
        private bool CanCloseProgrammExecuted() => true;
        public void OnCloseProgrammExecuted()
        {
            foreach(var item in TabItems)
            {
                SaveInFile(item);
            }
        }
        #endregion
        #region TextChangedCommand
        public ICommand? TextChangedCommand { get; set; }
        private bool CanTextChangedCommandExecuted(object par) => true;
        public void OnTextChangedCommandExecuted(object par)
        {
            if (par is not TabSystemItem item) return;
            if (item.Document.Text != File.ReadAllText(item.Path) && item.isSave == true)
            {
                item.isSave = false;
            }
            if (item.TitleName[item.TitleName.Length - 1] != '*' && item.isSave == false)
            {
                item.TitleName += "*";
            }
        }
        #endregion
        #region SaveAll
        public ICommand? SaveAllCommand { get; set; }
        private bool CanTSaveAllCommandExecuted(object par) => true;
        public void OnSaveAllCommandExecuted(object par)
        {
            if (TabItems.Count == 0) { return; }
            foreach (var item in TabItems)
            {
                if (item.isSave == false)
                {
                    SaveInFile(item);
                }
            }
        }
        #endregion
        #region SaveActiveaTabItemCommand
        public ICommand? SaveActiveaTabItemCommand { get; set; }
        private bool CanTSaveActiveaTabItemCommandExecuted(object par) => true;
        public void OnSaveActiveaTabItemCommandExecuted(object par)
        {
            if (TabItems.Count == 0) { return; }
            SaveInFile(ActiveaTabSystemItem);
        }
        #endregion
        #region OpenMainWindowCommand
        public ICommand? OpenMainWindowCommand { get; set; }
        private bool CanOpenMainWindowCommandExecuted(object par) => true;
        public void OnOpenMainWindowCommandExecuted(object par)
        {
            if (par is not EditWindow item) return;
            MainWindow mainWindow = new MainWindow();
            item.Close();
            mainWindow.Show();
        }
        #endregion
        #region ToСreateFileCommand
        public ICommand? ToСreateFileCommand { get; set; }
        private bool CanToСreateFileCommandExecuted(object par) => true;
        public void OnToСreateFileCommandExecuted(object par)
        {
            if (par is not FileSystemItem item) return;

            ToCreateFileWindow window = new ToCreateFileWindow(item);
            window.ShowDialog();

            SorterNameSystemFile(FileSystemItems);
        }
        #endregion
        #region ToСreateDirectoryCommand
        public ICommand? ToСreateDirectoryCommand { get; set; }
        private bool CanToСreateDirectoryCommandExecuted(object par) => true;
        public void OnToСreateDirectoryCommandExecuted(object par)
        {
            if (par is not FileSystemItem item) return;

            ToCreateDirectoryWindow window = new ToCreateDirectoryWindow(item);
            window.ShowDialog();

            SorterNameSystemFile(FileSystemItems);
        }
        #endregion
        #region TabDragTabItem
        public ICommand? TabDragTabItem { get; set; }
        private bool CanTabDragTabItemExecuted(object par) => true;
        public void OnTabDragTabItemExecuted(object par)
        {
            if (par is not TabSystemItem item) return;
            DragTabItem = item;
        }
        #endregion
        #region TabDropTabItem
        public ICommand? TabDropTabItem { get; set; }
        private bool CanTabDropTabItemExecuted(object par) => true;
        public void OnTabDropTabItemExecuted(object par)
        {
            if (par is not TabSystemItem item || DragTabItem == item) return;
            int indexFirst = TabItems.IndexOf(DragTabItem);
            int indexSecond = TabItems.IndexOf(item);
            TabItems.Move(indexFirst, indexSecond);
            SelectedTabIndex = indexSecond;

            SorterNameSystemFile(FileSystemItems);
        }
        #endregion

        #region DragTreeItem
        public ICommand? DragTreeItem { get; set; }
        private bool CanDragTreeItemExecuted(object par) => true;
        public void OnDragTreeItemExecuted(object par)
        {
            if (par is not FileSystemItem item) return;
            DragFileItem = item;
        }
        #endregion
        #region DropTreeItem
        public ICommand? DropTreeItem { get; set; }
        private bool CanDropTreeItemExecuted(object par) => true;
        public void OnDropTreeItemExecuted(object par)
        {
            if (par is not FileSystemItem item || item == DragFileItem) return;

            if (DragFileItem.IsDirectory)
            {
                string oldPath = DragFileItem.FullPath;
                string newPath = item.FullPath + "\\" + Path.GetFileName(DragFileItem.FullPath);
                if (oldPath != newPath)
                {
                    Directory.Move(oldPath, newPath);
                    СhangePathFileinDirectory(DragFileItem.Children, newPath);
                    item.Children.Add(new FileSystemItem
                    {
                        Name = Path.GetFileName(newPath),
                        FullPath = newPath,
                        IsDirectory = true,
                        Children = DragFileItem.Children
                    });
                    DeleteFileSystemItem(DragFileItem, FileSystemItems);
                }
            }
            else
            {
                string oldPath = DragFileItem.FullPath;
                string newPath = item.FullPath + "\\" + Path.GetFileName(DragFileItem.FullPath);
                try
                {
                    File.Move(oldPath, newPath);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                    return;
                }
                DeleteFileSystemItem(DragFileItem, FileSystemItems);

                item.Children.Add(new FileSystemItem
                {
                    Name = Path.GetFileName(newPath),
                    FullPath = newPath,
                    IsDirectory = false
                });
                SearchInTabItemsToFileSystem(oldPath, newPath);

            }
            SorterNameSystemFile(FileSystemItems);
        }
        private void СhangePathFileinDirectory(ObservableCollection<FileSystemItem> Children, string newPath)
        {
            foreach (FileSystemItem item in Children)
            {
                switch (item.IsDirectory)
                {
                    case true:
                        string NewPath =  newPath + "\\" + Path.GetFileName(item.FullPath);
                        item.FullPath = NewPath;
                        СhangePathFileinDirectory(item.Children, NewPath);
                        break;
                    default:
                        string oldPath = item.FullPath;
                        string NewPath1 = newPath + "\\" + Path.GetFileName(oldPath);
                        item.FullPath = NewPath1;
                        SearchInTabItemsToFileSystem(oldPath, NewPath1);
                        break;
                }
            }
        }
        private void SearchInTabItemsToFileSystem(string oldPath, string newPath)
        {
            foreach (var item in TabItems)
            {
                if (item.Path == oldPath)
                {
                    item.Path = newPath;
                    break;
                }
            }
        }
        private bool DeleteFileSystemItem(FileSystemItem fileItem, ObservableCollection<FileSystemItem> items)
        {
            if (items.Remove(fileItem))
                return true;

            foreach (var item in items)
            {
                if (item.Children != null && DeleteFileSystemItem(fileItem, item.Children))
                    return true;
            }

            return false;

        }
        #endregion
        #region ChangeLanguageCommand
        public ICommand? ChangeLanguageCommand { get; set; }
        private bool CanChangeLanguageCommandExecuted(object par) => true;
        public void OnChangeLanguageCommandExecuted(object par)
        {
            if (par is not string item) return;

            ChangeLanguage(item);
            localization.OverwriteFile("LanguageCustomization.config", item);
        }
        public void ChangeLanguage(string languageName)
        {
            localization.CurrentLanguage = languageName;
        }
        #endregion

        private void SorterNameSystemFile(ObservableCollection<FileSystemItem> items)
        {
            if (items == null) return;

            var itemsList = items.ToList();

            itemsList.Sort((x, y) =>
            {
                if (x.IsDirectory && !y.IsDirectory) return -1;
                if (!x.IsDirectory && y.IsDirectory) return 1;
                return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            });

            for (int i = 0; i < itemsList.Count; i++)
            {
                var currentItem = itemsList[i];
                int currentIndex = items.IndexOf(currentItem);

                if (currentIndex != i)
                {
                    items.Move(currentIndex, i);
                }

                if (currentItem.IsDirectory && currentItem.Children != null)
                {
                    SorterNameSystemFile(currentItem.Children);
                }
            }
        }
        private void RemoveItemFromTree(FileSystemItem itemToDelete)
        {
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
            if (parent.Children?.Contains(itemToDelete) == true)
            {
                parent.Children.Remove(itemToDelete);
                return true;
            }

            if (parent.Children != null)
            {
                foreach (var child in parent.Children.ToList())
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
        public static void OpenFileLocation(string filePath)
        {
            if (File.Exists(filePath) || Directory.Exists(filePath))
            {
                try
                {
                    // Открываем папку и выделяем файл
                    Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{localization["Error"]}: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show(localization["The file or folder does not exist"]);
            }
        }
        ~EW_ViewModel()
        {
            ThemeService.ThemeChanged -= OnThemeChanged;
            localization.LanguageChanged -= OnLanguageChanged;
        }
        public EW_ViewModel()
        {
            ThemeService.ThemeChanged += OnThemeChanged;
            localization.LanguageChanged += OnLanguageChanged;
            ClosingTreeView = new LambdaCommand(OnClosingTreeViewExecuted, CanClosingTreeViewExecuted);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecuted);
            RenameCommand = new LambdaCommand(OnRenameCommandExecuted, CanRenameCommandExecuted);

            ChangeThemeCommand = new LambdaCommand(OnChangeThemeCommandExecuted, CanChangeThemeCommandExecuted);
            OpenExplorer = new LambdaCommand(OnOpenExplorerExecuted, CanOpenExplorerExecuted);
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted, CanOpenCommandExecuted);
            CloseTabCommand = new LambdaCommand(OnCloseTabCommandExecuted, CanCloseTabCommandExecuted);
            ClosingWebView = new LambdaCommand(OnClosingWebViewExecuted, CanClosingWebViewExecuted);
            TextChangedCommand = new LambdaCommand(OnTextChangedCommandExecuted, CanTextChangedCommandExecuted);
            SaveToClosed = new LambdaCommand(OnSaveToClosedExecuted, CanSaveToClosedExecuted);

            SaveAllCommand = new LambdaCommand(OnSaveAllCommandExecuted, CanTSaveAllCommandExecuted);
            SaveActiveaTabItemCommand = new LambdaCommand(OnSaveActiveaTabItemCommandExecuted,CanTSaveActiveaTabItemCommandExecuted);

            OpenMainWindowCommand = new LambdaCommand(OnOpenMainWindowCommandExecuted, CanOpenMainWindowCommandExecuted);

            ToСreateFileCommand = new LambdaCommand(OnToСreateFileCommandExecuted, CanToСreateFileCommandExecuted);
            ToСreateDirectoryCommand = new LambdaCommand(OnToСreateDirectoryCommandExecuted, CanToСreateDirectoryCommandExecuted);

            TabDragTabItem = new LambdaCommand(OnTabDragTabItemExecuted, CanTabDragTabItemExecuted);
            TabDropTabItem = new LambdaCommand(OnTabDropTabItemExecuted, CanTabDropTabItemExecuted);
            DragTreeItem = new LambdaCommand(OnDragTreeItemExecuted, CanDragTreeItemExecuted);
            DropTreeItem = new LambdaCommand(OnDropTreeItemExecuted, CanDropTreeItemExecuted);
            ChangeLanguageCommand = new LambdaCommand(OnChangeLanguageCommandExecuted, CanChangeLanguageCommandExecuted);
        }
    }
}