using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.models;
using CourseProgect_1._2.services.Localization;
using CourseProgect_1._2.ViewModels.Base;
using CourseProgect_1._2.views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace CourseProgect_1._2.ViewModels
{
    class TCDW_ViewModel : ViewModel
    {

        ALocalization localization = new ALocalization();

        private FileSystemItem Directory;
        private string _NameDirectory;
        private bool _Closer;
        public bool Closer { get => _Closer; set => _Closer = value; }
        public string NameDirectory 
        {
            get => _NameDirectory;
            set => Set(ref _NameDirectory, value);
        }
        public TCDW_ViewModel(FileSystemItem TreeView)
        {
            Directory = TreeView;
            ToСreateDirectoryCommand = new LambdaCommand(OnToСreateDirectoryCommandExecuted, CanToСreateDirectoryCommandExecuted);
        }
        public string StringFolderName => localization["Folder Name"];
        public string StringCalcer => localization["Calcer"];
        public string StringToCreate => localization["To create"];
        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(StringFolderName));
            OnPropertyChanged(nameof(StringCalcer));
            OnPropertyChanged(nameof(StringToCreate));
        }
        public bool CanCreateDirectory() { return !string.IsNullOrEmpty(_NameDirectory) && !_NameDirectory.Intersect(System.IO.Path.GetInvalidFileNameChars()).Any(); }

        #region ToСreateDirectoryCommand
        public ICommand? ToСreateDirectoryCommand { get; set; }
        private bool CanToСreateDirectoryCommandExecuted(object par) => true;
        public void OnToСreateDirectoryCommandExecuted(object par)
        {
            if (CanCreateDirectory())
            {
                try
                {
                    string FullNameNewDir = Directory.FullPath + "\\" + NameDirectory;
                    System.IO.Directory.CreateDirectory(FullNameNewDir);
                    Directory.Children.Add(
                        new FileSystemItem
                        {
                            Name = NameDirectory,
                            FullPath = FullNameNewDir,
                            IsDirectory = true
                        }
                    );
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, localization["File creation error"],
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Closer = false;
                }
            }
            Closer = false;
            MessageBox.Show(localization["The data is incorrect"], localization["File creation error"],
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

    }
}
