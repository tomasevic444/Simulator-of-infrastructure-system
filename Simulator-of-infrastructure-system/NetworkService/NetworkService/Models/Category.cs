using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Models
{
    public class Category
    {
        public string Name { get; set; }
        public ObservableCollection<Entity> Entities { get; set; }

        public Category(string name)
        {
            Name = name;
            Entities = new ObservableCollection<Entity>();
        }
    }
}
