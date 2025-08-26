using CourseProgect_1._2.data.SQLite;
using CourseProgect_1._2.Infrastructure.Commands;
using CourseProgect_1._2.Models;
using CourseProgect_1._2.services.Localization;
using CourseProgect_1._2.ViewModels.Base;
using CourseProgect_1._2.views.Windows;
using CourseProgect_1._2.Views.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace CourseProgect_1._2.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Инициализация
        ConnectDataBase db;
        public ObservableCollection<Progect> Progects { get; set; }
        ALocalization localization = new ALocalization();
        #endregion
        public string StringExportAProject => localization["Export a project"];
        public string StringCreateANewProject => localization["Create a new project"];
        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(StringExportAProject));
            OnPropertyChanged(nameof(StringCreateANewProject));
        }
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
                    var itemsToRemove = new List<Progect>();

                    foreach (var item in db.Progects)
                    {
                        try
                        {
                            if (!File.Exists(item.Path))
                            {
                                itemsToRemove.Add(item);
                                continue;
                            }

                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(item.Path);
                            XmlElement xRoot = xDoc.DocumentElement;

                            if (xRoot == null || xRoot.Name != "Progect")
                            {
                                itemsToRemove.Add(item);
                                continue;
                            }

                            string xmlName = xRoot.SelectSingleNode("Name")?.InnerText ?? "";
                            string xmlPath = xRoot.SelectSingleNode("Path")?.InnerText ?? "";
                            string xmlLastCallDate = xRoot.SelectSingleNode("Last_call_date")?.InnerText ?? "";

                            if (item.Name == xmlName &&
                                item.Path == xmlPath &&
                                item.Last_call_date == xmlLastCallDate)
                            {
                                item.Last_call_date = DateTime.Now.ToString("G");
                            }
                        }
                        catch (Exception ex)
                        {
                            itemsToRemove.Add(item);
                        }
                    }

                    // Удаляем и сохраняем
                    if (itemsToRemove.Any())
                    {
                        db.Progects.RemoveRange(itemsToRemove);
                        db.SaveChanges();
                    }
                }
                var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                    var editor = new EditWindow(System.IO.Path.GetDirectoryName(_Progect.Path));
                    currentWindow?.Close();
                    editor.Show();
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
        #endregion+

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
                XElement NameProg = new XElement("Name", window.TextBoxNameProgect.Text);
                XElement PathProg = new XElement("Path", window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text + "\\" + window.TextBoxNameProgect.Text + ".xml");
                XElement DataProg = new XElement("Last_call_date", DateTime.Now.ToString("G"));
                Prog.Add(NameProg);
                Prog.Add(PathProg);
                Prog.Add(DataProg);
                xdoc.Add(Prog);
                xdoc.Save(window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text + "\\" + window.TextBoxNameProgect.Text + ".xml");
                File.Create(window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text + "\\Тестовый файл.md");
                Progect progect = new Progect(window.TextBoxNameProgect.Text, window.TextBoxPathProgect.Text + "\\" + window.TextBoxNameProgect.Text + "\\" + window.TextBoxNameProgect.Text + ".xml", DateTime.Now.ToString("G"));
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

            var itemsToRemove = new List<Progect>();
            foreach (Progect a in pr)
            {
                if (File.Exists(a.Path))
                {
                    Progect _selectProgect = new Progect(a.Name, a.Path, a.Last_call_date);
                    Progects.Add(_selectProgect);
                }
                else
                {
                    itemsToRemove.Add(a);
                    db.Progects.RemoveRange(itemsToRemove);
                    
                }
            }
            db.SaveChanges();
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
