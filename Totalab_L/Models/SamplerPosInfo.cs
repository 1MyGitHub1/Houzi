using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totalab_L.Models
{
    public class SamperPosInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int HomeX
        {
            get => _homeX;
            set
            {
                _homeX = value;
                Notify("HomeX");
            }
        }
        private int _homeX;

        public int HomeZ
        {
            get => _homeZ;
            set
            {
                _homeZ = value;
                Notify("HomeZ");
            }
        }
        private int _homeZ;

        public int Wash1X
        {
            get => _wash1X;
            set
            {
                _wash1X = value;
                Notify("Wash1X");
            }
        }
        private int _wash1X;

        public int Wash2X
        {
            get => _wash2X;
            set
            {
                _wash2X = value;
                Notify("Wash2X");
            }
        }
        private int _wash2X;

        public int Wash3X
        {
            get => _wash3X;
            set
            {
                _wash3X = value;
                Notify("Wash3X");
            }
        }
        private int _wash3X;

        public int Wash1Z
        {
            get => _wash1Z;
            set
            {
                _wash1Z = value;
                Notify(" Wash1Z ");
            }
        }
        private int _wash1Z;

        public int Wash2Z
        {
            get => _wash2Z;
            set
            {
                _wash2Z = value;
                Notify("Wash2Z");
            }
        }
        private int _wash2Z;

        public int Wash3Z
        {
            get => _wash3Z;
            set
            {
                _wash3Z = value;
                Notify("Wash3Z");
            }
        }
        private int _wash3Z;

        public int Samp1e1X
        {
            get => _sample1X;
            set
            {
                _sample1X = value;
                Notify("Samp1e1X");
            }
        }
        private int _sample1X;

        public int Samp1e1Y
        {
            get => _sample1Y;
            set
            {
                _sample1Y = value;
                Notify("Samp1e1Y");
            }
        }
        private int _sample1Y;
        public int Samp1e2X
        {
            get => _sample2X;
            set
            {
                _sample2X = value;
                Notify("Samp1e2X");
            }
        }
        private int _sample2X;

        public int Samp1e2Y
        {
            get => _sample2Y;
            set
            {
                _sample2Y = value;
                Notify("Samp1e2Y");
            }
        }
        private int _sample2Y;

        public int Samp1e3X
        {
            get => _sample3X;
            set
            {
                _sample3X = value;
                Notify("Samp1e3X");
            }
        }
        private int _sample3X;

        public int Samp1e3Y
        {
            get => _sample3Y;
            set
            {
                _sample3Y = value;
                Notify("Samp1e3Y");
            }
        }
        private int _sample3Y;

        public int Samp1e4X
        {
            get => _sample4X;
            set
            {
                _sample4X = value;
                Notify("Samp1e4X");
            }
        }
        private int _sample4X;

        public int Samp1e4Y
        {
            get => _sample4Y;
            set
            {
                _sample4Y = value;
                Notify("Samp1e4Y");
            }
        }
        private int _sample4Y;


        public int Samp1e5X
        {
            get => _sample5X;
            set
            {
                _sample5X = value;
                Notify("Samp1e5X");
            }
        }
        private int _sample5X;

        public int Samp1e5Y
        {
            get => _sample5Y;
            set
            {
                _sample5Y = value;
                Notify("Samp1e5Y");
            }
        }
        private int _sample5Y;
        public int Samp1e6X
        {
            get => _sample6X;
            set
            {
                _sample6X = value;
                Notify("Samp1e6X");
            }
        }
        private int _sample6X;

        public int Samp1e6Y
        {
            get => _sample6Y;
            set
            {
                _sample6Y = value;
                Notify("Samp1e6Y");
            }
        }
        private int _sample6Y;

        public int Samp1e7X
        {
            get => _sample7X;
            set
            {
                _sample7X = value;
                Notify("Samp1e7X");
            }
        }
        private int _sample7X;

        public int Samp1e7Y
        {
            get => _sample7Y;
            set
            {
                _sample7Y = value;
                Notify("Samp1e7Y");
            }
        }
        private int _sample7Y;


        public int Samp1e8X
        {
            get => _sample8X;
            set
            {
                _sample8X = value;
                Notify("Samp1e8X");
            }
        }
        private int _sample8X;

        public int Samp1e8Y
        {
            get => _sample8Y;
            set
            {
                _sample8Y = value;
                Notify("Samp1e8Y");
            }
        }
        private int _sample8Y;
    }
}
