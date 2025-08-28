using CourseProgect_1._2.ViewModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
