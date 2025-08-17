using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CourseProgect_1._2.Models
{
    internal class Progect : INotifyPropertyChanged
    {
        public int id { get; set; }
        private string name, path, last_call_date;
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public string Path { get { return path; } set { path = value; OnPropertyChanged("Path"); } }
        public string Last_call_date { get { return last_call_date; } set { last_call_date = value; OnPropertyChanged("Last_call_date"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
