using NetworkService.Helpers;
using NetworkService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModels
{
    public class DisplayViewModel : BindableBase
    {
        private Entity _selectedEntity;
        public Entity SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                SetProperty(ref _selectedEntity, value);
            }
        }
        private Entity _draggedItem = null;
        private bool _dragging = false;
        public int _draggingSourceIndex = -1;

        public static ObservableCollection<Entity> Entities { get; set; }
        public static Dictionary<int, Entity> AddedToGrid { get; set; } = new Dictionary<int, Entity>();
    }
}
