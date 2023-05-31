using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Models
{
    public class MethodSelectorItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string MethodName
        {
            get => _methodName;
            set
            {
                _methodName = value;
                Notify("MethodName");
            }
        }
        private string _methodName;

        public string MethodDate
        {
            get => _methodDate;
            set
            {
                _methodDate = value;
                Notify("MethodDate");
            }
        }

        private string _methodDate;
    }
}
