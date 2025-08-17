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
    internal class Progect : ViewModel
    {
        public int id { get; set; }
        [NotMapped]
        public object Tag { get; set; }
        private string? name, path, last_call_date;
        public string Name { get => name; set => Set(ref name, value); }
        public string Path { get => path; set => Set(ref path, value); }
        public string Last_call_date { get => last_call_date; set => Set(ref last_call_date, value); }
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
    }
}
