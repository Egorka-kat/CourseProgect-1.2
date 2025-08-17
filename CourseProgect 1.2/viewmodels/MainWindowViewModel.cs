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
using Microsoft.Win32;

namespace CourseProgect_1._2.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Инициализация
        ConnectDataBase db;
        public ObservableCollection<Progect> Progects { get; set; }
        #endregion

        #region Инициализация выбраного Progect
        private Progect? _Progect;
        public Progect Progect
        {
            get { return _Progect; }
            set 
            { 
                Set(ref _Progect, value);


                if (_Progect.Path is not null)
                {
                    var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                    var editor = new EditWindow(_Progect.Path);
                    currentWindow?.Close();
                    editor.Show();
                }

            }
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

        #region Функция OpenMainWindow
        private void OpenMainWindow(string Path, object par)
        {
            EditWindow mainEditor = new EditWindow(Path);
            if (par is Window window)
                window.Close();
            mainEditor.ShowDialog();
        }
        #endregion

        #region Команды

        #region CommandCreateNewProgect
        public ICommand? CommandCreateNewProgect { get; set; }
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
        #endregion

        #region CommandExportNewProgect
        public ICommand? CommandExportNewProgect { get; set; }
        private bool CanCommandExportNewProgectExecuted(object par) => true;
        public void OnCommandExportNewProgectExecuted(object par)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select a File";
            fileDialog.Filter = "Text files (*.xml)|*.xml";
            if (fileDialog.ShowDialog() == true)
            {
                string selectedFile = fileDialog.FileName;

                List<Progect> pr = db.Progects.ToList();
                foreach (Progect a in pr)
                {
                    if (File.Exists(a.Path))
                    {
                        if (a.Name == System.IO.Path.GetFileNameWithoutExtension(selectedFile))
                        {
                            MessageBox.Show("Данный проект уже загружен");
                            return;
                        }
                    }
                }
                Progect progect = new Progect(System.IO.Path.GetFileNameWithoutExtension(selectedFile), selectedFile, DateTime.Now.ToString("d"));
                db.Progects.Add(progect);
                db.SaveChanges();
                OpenMainWindow(System.IO.Path.GetDirectoryName(progect.Path), par);
            }
        }
        #endregion

        #endregion

        public MainWindowViewModel() 
        {
            #region Вывод данных из БД

            db = new ConnectDataBase();
            Progects = new ObservableCollection<Progect>();

            List<Progect> pr = db.Progects.ToList();
            pr.Sort((x, y) => y.Last_call_date.CompareTo(x.Last_call_date));
            foreach (Progect a in pr)
            {
                if (File.Exists(a.Path))
                {
                    Progect _selectProgect = new Progect(a.Name, a.Path, a.Last_call_date);
                    Progects.Add(_selectProgect);
                }
            }

            #endregion

            #region Команды
            #region CommandCreateNewProgect
            CommandCreateNewProgect = new LambdaCommand(OnCommandCreateNewProgectExecuted, CanCommandCreateNewProgectExecuted);
            #endregion
            #region CommandExportNewProgect
            CommandExportNewProgect = new LambdaCommand(OnCommandExportNewProgectExecuted, CanCommandExportNewProgectExecuted);
            #endregion
            #endregion
        }
    }
}
