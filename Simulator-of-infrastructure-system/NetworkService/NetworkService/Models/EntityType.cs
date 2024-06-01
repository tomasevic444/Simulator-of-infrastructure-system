using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Models
{
    public class EntityType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string type;
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        public EntityType(string type)
        {
            this.type = type;
        }

        public EntityType()
        {

        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
