using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Models
{
    public enum ValueState { Normal, Low, High }
    public class Entity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        int id;
        ValueState _valueState;
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

                    if (_value < 675) ValueState = ValueState.Low;
                    else if (_value > 735) ValueState = ValueState.High;
                    else ValueState = ValueState.Normal;

                }
            }
        }
        public ValueState ValueState
        {
            get
            {
                return _valueState;
            }
            set
            {
                if (_valueState != value)
                {
                    _valueState = value;
                    OnPropertyChanged(nameof(ValueState));
                }
            }
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
