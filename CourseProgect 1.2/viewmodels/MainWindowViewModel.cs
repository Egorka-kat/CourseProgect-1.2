using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.ViewModels.Dase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CourseProgect_1._2.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Заголовок окна
        private string _Title = "TextEditor";
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        #endregion


        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; set; }
        private bool CanCloseApplicationCommandExecuted(object par) => true;
        public void OnCloseApplicationCommandExecuted(object par) 
        { 
            Application.Current.Shutdown();
        }
        #endregion

        #endregion
        public MainWindowViewModel() 
        {
            #region Команды

            #region CloseApplicationCommand
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecuted);
            #endregion

            #endregion
        }
    }
}
