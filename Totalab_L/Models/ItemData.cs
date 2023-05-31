using Totalab_L.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Totalab_L.Models
{
    public class ItemData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string ItemContent
        {
            get { return _itemContent; }
            set
            {
                _itemContent = value;
                Notify("ItemContent");
            }
        }
        private string _itemContent;


        public bool ItemIsEnable
        {
            get { return _itemIsEnable; }
            set
            {
                _itemIsEnable = value;
                Notify("ItemIsEnable");
            }
        }
        private bool _itemIsEnable = true;


        public Item_Status ItemStatus
        {
            get { return _itemStatus; }
            set
            {
                _itemStatus = value;
                Notify("ItemStatus");
            }
        }
        private Item_Status _itemStatus = Item_Status.Free;

        public bool IsItemSelected
        {
            get { return _isItemSelected; }
            set
            {
                _isItemSelected = value;
                Notify("IsItemSelected");
            }
        }
        private bool _isItemSelected;
    }
}
