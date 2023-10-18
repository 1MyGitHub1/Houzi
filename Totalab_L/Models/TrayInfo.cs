using Totalab_L.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Totalab_L.Enum;

namespace Totalab_L.Models
{
    public class TrayInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ///<summary>
        ///TrayName
        ///</summary>
        public Enum_TrayName TrayName
        {
            get => _trayName;
            set
            {
                _trayName = value;
                Notify("TrayName");
            }
        }
        private Enum_TrayName _trayName;


        ///<summary>
        ///TrayInfoXCount
        ///</summary>
        public int XCount
        {
            get => _xCount;
            set
            {
                _xCount = value;
                Notify("XCount");
            }
        }
        private int _xCount;


        ///<summary>
        ///TrayInfoYCount
        ///</summary>
        public int YCount
        {
            get => _yCount;
            set
            {
                _yCount = value;
                Notify("YCount");
            }
        }
        private int _yCount;


        ///<summary>
        ///TrayItemSize
        ///</summary>
        public Size  ItemsSize
        {
            get => _itemsSize;
            set
            {
                _itemsSize = value;
                Notify("ItemsSize");
            }
        }
        private Size _itemsSize;
        ///<summary>
        ///TrayItemsCount
        ///</summary>
        public Size ItemsCount
        {
            get => _itemsCount;
            set
            {
                _itemsCount = value;
                Notify("ItemsCount");
            }
        }
        private Size _itemsCount;

        public ObservableCollection<ItemData> TrayItemList
        {
            get => _trayItemList;
            set
            {
                _trayItemList = value;
                Notify("TrayItemList");
            }
        }
        private ObservableCollection<ItemData> _trayItemList = new ObservableCollection<ItemData>();

        public string TrayType
        {
            get => _trayType;
            set
            {
                _trayType = value;
                Notify("TrayType");
            }
        }
        private string _trayType;

        public string TrayBckPath
        {
            get => _trayBckPath;
            set
            {
                _trayBckPath = value;
                Notify("TrayBckPath");
            }
        }
        private string _trayBckPath;

        public int TrayStartNumber
        {
            get => _trayStartNumber;
            set
            {
                _trayStartNumber = value;
                Notify("TrayStartNumber");
            }
        }
        private int _trayStartNumber;

        public int TrayEndNumber
        {
            get => _trayEndNumber;
            set
            {
                _trayEndNumber = value;
                Notify("TrayEndNumber");
            }
        }
        private int _trayEndNumber;

        public int TrayItemCount
        {
            get => _trayItemCount;
            set
            {
                _trayItemCount = value;
                Notify("TrayItemCount");
            }
        }
        private int _trayItemCount;

    }
}
