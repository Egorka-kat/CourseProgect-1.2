using CourseProgect_1._2.data.SQLite;
using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.Models;
using CourseProgect_1._2.ViewModels.Base;
using CourseProgect_1._2.views.Windows;
using CourseProgect_1._2.Views.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace CourseProgect_1._2.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Инициализация
        ConnectDataBase db;
        public ObservableCollection<Progect> Progects { get; set; }
        #endregion
        #region Инициализация выбраного Progect
        private Progect _selectProgect;
        public Progect selectProgect
        {
            get { return _selectProgect; }
            set => Set(ref _selectProgect, value); 
        }
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
            WindowForEnteringProjectName window = new WindowForEnteringProjectName();
            window.ShowDialog();
            window.Close();
            if (!Directory.Exists(window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text))
            {
                Directory.CreateDirectory(window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text);
                XDocument xdoc = new XDocument();
                XElement Prog = new XElement("Progect");
                XAttribute NameProg = new XAttribute("Name", window.TextBoxNameProgect.Text);
                XElement PathProg = new XElement("Path", window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text + "\\" + window.TextBoxNameProgect.Text + ".xml");
                XElement DataProg = new XElement("Last_call_date", DateTime.Now.ToString("d"));
                Prog.Add(NameProg);
                Prog.Add(PathProg);
                Prog.Add(DataProg);
                xdoc.Add(Prog);
                xdoc.Save(window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text + "\\" + window.TextBoxNameProgect.Text + ".xml");
                File.Create(window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text + "\\Тестовый файл.md");
                Progect progect = new Progect(window.TextBoxNameProgect.Text, window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text + "\\" + window.TextBoxNameProgect.Text + ".xml", DateTime.Now.ToString("d"));
                db.Progects.Add(progect);
                db.SaveChanges();
                OpenMainWindow(System.IO.Path.GetDirectoryName(progect.Path), par);
            }
        }
        #region OpenMainWindow
        private void OpenMainWindow(string Path, object par)
        {
            EditWindow mainEditor = new EditWindow(Path);
            if (par is Window window)
                window.Close();
            mainEditor.ShowDialog();
        }
        #endregion
        #endregion
        #endregion
        public MainWindowViewModel() 
        {
            db = new ConnectDataBase();
            Progects = new ObservableCollection<Progect>();

            List<Progect> pr = db.Progects.ToList();
            pr.Sort((x, y) => x.Last_call_date.CompareTo(y.Last_call_date));
            foreach (Progect a in db.Progects)
            {
                if (File.Exists(a.Path))
                {
                    Progect _selectProgect = new Progect(a.Name, a.Path, a.Last_call_date);
                    Progects.Add(_selectProgect);
                }
            }
            #region Команды
            #region CommandCreateNewProgect
            CommandCreateNewProgect = new LambdaCommand(OnCommandCreateNewProgectExecuted, CanCommandCreateNewProgectExecuted);
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
