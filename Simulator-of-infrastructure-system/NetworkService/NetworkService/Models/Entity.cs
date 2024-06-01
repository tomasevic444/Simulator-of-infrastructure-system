using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Models
{
    public class Entity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        int id;
        string name;
        EntityType entityType;
        int _value;

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public EntityType EntityType
        {
            get
            {
                return entityType;
            }
            set
            {
                if (entityType != value)
                {
                    entityType = value;
                    OnPropertyChanged(nameof(EntityType));
                }
            }
        }
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
