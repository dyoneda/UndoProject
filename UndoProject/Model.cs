using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UndoProject
{
    public class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName) 
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        private string _stringValue;
        private int _intValue;
        private bool _boolValue;

        public string StringValue
        {
            get => this._stringValue;
            set => this.SetProperty(ref this._stringValue, value);
        }
        public int IntValue
        {
            get => this._intValue;
            set => this.SetProperty(ref this._intValue, value);
        }
        public bool BoolValue
        {
            get => this._boolValue;
            set => this.SetProperty(ref this._boolValue, value);
        }
    }
}
