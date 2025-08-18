using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Test.models;

namespace Test
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FileSystemItem> FileSystemItems { get; } = new ObservableCollection<FileSystemItem>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
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
    }
}
