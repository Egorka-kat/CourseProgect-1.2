using CourseProgect_1._2.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CourseProgect_1._2.Models
{
    internal class Progect : INotifyPropertyChanged
    {
        public int id { get; set; }
        [NotMapped]
        public object Tag { get; set; }
        private string? name, path, last_call_date;
        public string Name { 
            get => name;
            set {name = value; OnPropertyChanged("Name"); }
        }
        public string Path
        {
            get => path;
            set { path = value; OnPropertyChanged("Path"); }
        }
        public string Last_call_date { 
            get => last_call_date;
            set { last_call_date = value; OnPropertyChanged("Last_call_date"); }
        }
        public Progect(string Name, string Path, string Last_call_date)
        {
            this.name = Name;
            this.path = Path;
            this.last_call_date = Last_call_date;
        }
        public Progect(string Name, string Path, string Last_call_date, object Tag)
        {
            this.Name = Name;
            this.Path = Path;
            this.Last_call_date = Last_call_date;
            this.Tag = Tag;
        }
        public string Full_details_for_implementation
        {
            get { return Name + "\t" + Last_call_date + "\n" + Path; }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
