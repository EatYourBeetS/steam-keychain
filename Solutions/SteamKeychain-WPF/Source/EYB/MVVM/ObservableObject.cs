using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EYB.MVVM
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // SetField (Name, value); // where there is a data member
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string property = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            RaisePropertyChanged(property);
            return true;
        }

        // SetField(()=> somewhere.Name = value; somewhere.Name, value) // Advanced case where you rely on another property
        protected bool SetProperty<T>(T currentValue, T newValue, Action<T> set, [CallerMemberName] string property = null)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                return false;
            }

            set.Invoke(newValue);
            RaisePropertyChanged(property);
            return true;
        }

        protected void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class ObservableObject<T> : ObservableObject where T : class
    {
        protected T source = null;

        public static implicit operator T(ObservableObject<T> a)
        {
            return a.source;
        }

        public ObservableObject()
        {

        }

        public ObservableObject(T source)
        {
            this.source = source;
        }
    }
}
