using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CourseProgect_1._2.ViewModels.Base;

namespace CourseProgect_1._2.Models
{
    internal class Progect : ViewModel
    {
        public int id { get; set; }
        private string? name, path, last_call_date;
        public string Name { get => name; set => Set(ref name, value); }
        public string Path { get => path; set =>Set(ref path,value); }
        public string Last_call_date { get => last_call_date; set => Set(ref last_call_date,value); }
    }
}
