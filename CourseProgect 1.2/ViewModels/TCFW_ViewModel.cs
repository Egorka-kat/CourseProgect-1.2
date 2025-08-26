using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.models;
using CourseProgect_1._2.services.Localization;
using CourseProgect_1._2.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace CourseProgect_1._2.ViewModels
{
    class TCFW_ViewModel : ViewModel
    {
        ALocalization localization = new ALocalization();
        public string StringFileType => localization["File Type"];
        public string StringAnother => localization["Another..."];
        public string StringplainText => localization["Plain text"];
        public string StringFileName => localization["File Name"];
        public string StringExpansion => localization["Expansion"];
        public string StringCalcer => localization["Calcer"];
        public string StringToCreate => localization["To create"];

        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(StringFileType));
            OnPropertyChanged(nameof(StringAnother));
            OnPropertyChanged(nameof(StringplainText));
            OnPropertyChanged(nameof(StringFileName));
            OnPropertyChanged(nameof(StringExpansion));
            OnPropertyChanged(nameof(StringCalcer));
            OnPropertyChanged(nameof(StringToCreate));
        }

        private FileSystemItem Directory;
        private FileType _selectedFileType;
        private string _customExtension;
        private string _fileName;
        private bool _Closer;
        public bool Closer { get => _Closer; set => _Closer = value; }
        public FileType SelectedFileType
        {
            get => _selectedFileType;
            set
            {
                _selectedFileType = value;
                OnPropertyChanged(nameof(SelectedFileType));
                OnPropertyChanged(nameof(IsCustomExtensionEnabled));
                UpdateExtension();
            }
        }

        public string CustomExtension
        {
            get => _customExtension;
            set
            {
                Set(ref _customExtension, value);
                OnPropertyChanged(nameof(IsExtensionValid));
            }
        }
        public bool IsExtensionValid => IsValidExtension(CustomExtension) || string.IsNullOrEmpty(CustomExtension);

        // И затем проверять при сохранении
        public bool CanCreateFile()
        {
            return !string.IsNullOrWhiteSpace(FileName) &&
                   IsExtensionValid && FileName != null;
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
                OnPropertyChanged(nameof(FullFileName));
            }
        }

        public string FullFileName => $"{FileName}{GetCurrentExtension()}";

        public bool IsCustomExtensionEnabled => SelectedFileType == FileType.Custom;

        public ObservableCollection<FileType> FileTypes { get; }

        public ICommand CreateCommand { get; }
        private bool CanCreateFile(object par) => !string.IsNullOrWhiteSpace(FileName) &&
                   IsValidExtension(GetCurrentExtension());
        private void CreateFile(object par)
        {
            var extension = GetCurrentExtension();
            var fullName = $"{FileName}{extension}";
            string PathNewFile = Directory.FullPath + "\\" + fullName;
            Closer = true;
            if (CanCreateFile())
            {
                if (!File.Exists(PathNewFile))
                {
                    File.Create(PathNewFile);
                    Directory.Children.Add(
                        new FileSystemItem
                        {
                            Name = fullName,
                            FullPath = PathNewFile,
                            IsDirectory = false
                        }
                    );
                    return;
                }
            }
            Closer = false;
            MessageBox.Show(localization["The data is incorrect"], localization["File creation error"],
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public TCFW_ViewModel(FileSystemItem item)
        {
            Directory = item;

            FileTypes = new ObservableCollection<FileType>
            {
                FileType.Text,
                FileType.Markdown,
                FileType.Html,
                FileType.Json,
                FileType.Xml,
                FileType.CSharp,
                FileType.Custom
            };

            SelectedFileType = FileType.Text;

            CreateCommand = new LambdaCommand(CreateFile, CanCreateFile);
        }

        private void UpdateExtension()
        {
            if (SelectedFileType == FileType.Custom)
            {
                CustomExtension = string.Empty;
            }
            else
            {
                CustomExtension = SelectedFileType.GetExtension();
            }
            OnPropertyChanged(nameof(FullFileName));
        }

        private string GetCurrentExtension()
        {
            return SelectedFileType == FileType.Custom ? CustomExtension : SelectedFileType.GetExtension();
        }

        private bool IsValidExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return true;

            if (!extension.StartsWith(".") || extension.Length < 2)
                return false;

            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            return extension.Substring(1).All(c => !invalidChars.Contains(c));
        }
    }
}
