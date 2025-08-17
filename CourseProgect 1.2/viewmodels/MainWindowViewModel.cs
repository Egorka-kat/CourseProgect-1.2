using CourseProgect_1._2.data.SQLite;
using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.ViewModels.Base;
using CourseProgect_1._2.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace CourseProgect_1._2.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Инициализация
        ConnectDataBase db = new ConnectDataBase();
        public ObservableCollection<Progect> Progects = new ObservableCollection<Progect>();
        #endregion

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

        #region CloseApplicationCommand
        public ICommand CommandCreateNewProgect { get; set; }
        private bool CanCommandCreateNewProgectExecuted(object par) => true;
        public void OnCommandCreateNewProgectExecuted(object par)
        {
            
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

            #region Загрузка БД
            db.Database.EnsureCreated();
            db.Progects.Load();
            Progects = db.Progects.Local.ToObservableCollection();
            #endregion


        }
    }
}
