using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CourseProgect_1._2.viewmodels
{
    /// <summary>
    /// WindowForEnteringProjectName_ViewModel
    /// </summary>
    internal class WFEPN_ViewModel : ViewModel
    {
        #region Содержимое TextBox
        private string _TextBox_NameProgect = "Название Проекта";
        public string TextBox_NameProgect
        {
            get => _TextBox_NameProgect;
            set => Set(ref _TextBox_NameProgect, value);
        }
        private string _TextBox_PathProgect = "Путь Проекта";
        public string TextBox_PathProgect
        {
            get => _TextBox_PathProgect;
            set => Set(ref _TextBox_PathProgect, value);
        }
        #endregion

        #region Переменные
        public bool Condition { get; set; }
        public bool Motion { get; set; }
        #endregion

        #region Команды
        #region CloseWindowCommand
        public ICommand CloseWindowCommand { get; set; }
        private bool CanCloseWindowCommandExecuted(object par) => true;
        public void OnCloseWindowCommandExecuted(object par)
        {
            Condition = false;
            if (par is Window window)
                window.Close();
        }
        #endregion

        #region OpenExplorer
        public ICommand OpenExplorer { get; set; }
        private bool CanOpenExplorerExecuted(object par) => true;
        public void OnOpenExplorervExecuted(object par)
        {
            try
            {
                var dialog = new OpenFolderDialog
                {
                    Title = "Выберите папку",
                    Multiselect = false
                };
                if (dialog.ShowDialog() == true)
                {
                    TextBox_PathProgect = dialog.FolderName;
                }
            }
            catch (Exception)
            {
                return;
            }

            Motion = true;
        }
        #endregion

        #region ClickOk
        public ICommand ClickOk { get; set; }
        private bool CanClickOkExecuted(object par) => true;
        public void OnClickOkExecuted(object par)
        {
            if (_TextBox_NameProgect == "Название Проекта")
            {
                MessageBox.Show("Введите имя проекта");
            }
            if (Motion == true)
            {
                Condition = true;
                if (par is Window window)
                    window.Close();
            }
        }
        #endregion

        #endregion
        public WFEPN_ViewModel()
        {
            #region Команды
            #region OpenExplorer
            OpenExplorer = new LambdaCommand(OnOpenExplorervExecuted, CanOpenExplorerExecuted);
            #endregion
            #region CloseWindowCommand
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);
            #endregion
            #region ClickOk
            ClickOk = new LambdaCommand(OnClickOkExecuted,CanClickOkExecuted);
            #endregion
            #endregion
        }
    }
}
