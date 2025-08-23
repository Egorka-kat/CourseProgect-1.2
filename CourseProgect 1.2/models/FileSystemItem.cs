using CourseProgect_1._2.ViewModels.Base;
using System.Collections.ObjectModel;

namespace CourseProgect_1._2.models
{
    class FileSystemItem : ViewModel
    {
        private string _Name;
        public string Name {
            get => _Name; 
            set => Set(ref _Name, value); 
        }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
        public ObservableCollection<FileSystemItem> Children { get; set; } = new ObservableCollection<FileSystemItem>();

    }
}
