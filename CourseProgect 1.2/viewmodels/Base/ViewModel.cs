using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CourseProgect_1._2.ViewModels.Base
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = "") 
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
            }
            catch (Exception)
            {
            }
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = "")
        {
            if(Equals(field,value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
    }
}
