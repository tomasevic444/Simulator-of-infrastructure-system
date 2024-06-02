using NetworkService.Helpers;
using NetworkService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NetworkService.ViewModels
{
     public class EntitiesViewModel : BindableBase
    {
        public List<string> Types { get; set; }

        private object _selectedType;
        public object SelectedType
        {

            get => _selectedType;
            set
            {
                SetProperty(ref _selectedType, value);
                AddEntityCommand.RaiseCanExecuteChanged();
            }
        }
        private TextBox _selectedTextBox;
        public TextBox SelectedTextBox
        {
            get => _selectedTextBox;
            set
            {
                SetProperty(ref _selectedTextBox, value);
            }
        }
        private Entity _selectedEntity;
        public Entity SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                SetProperty(ref _selectedEntity, value);
                RemoveEntityCommand.RaiseCanExecuteChanged();
            }
        }
        public ObservableCollection<Entity> Entities { get; set; }
        public ObservableCollection<Entity> FilteredEntities { get; set; }
      

            private string _idText;
        public string IDText
        {
            get { return _idText; }
            set
            {
                SetProperty(ref _idText, value);
                AddEntityCommand.RaiseCanExecuteChanged();
            }
        }
        private string _nameText;
        public string NameText
        {
            get { return _nameText; }
            set
            {
                SetProperty(ref _nameText, value);
                AddEntityCommand.RaiseCanExecuteChanged();
            }
        }
        private string _filterText;
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                SetProperty(ref _filterText, value);
                FilterCommand.RaiseCanExecuteChanged();
            }
        }
        private string _filterType;
        public string FilterType
        {
            get => _filterType;
            set
            {
                SetProperty(ref _filterType, value);
                FilterCommand.RaiseCanExecuteChanged();
            }
        }

        public Commands AddEntityCommand { get; set; }
        public Commands RemoveEntityCommand { get; set; }
        public Commands FilterCommand { get; set; }
        public Commands ClearFiltersCommand { get; set; }

        public EntitiesViewModel()
        {

            Entities = MainWindowViewModel.Entities;
            FilteredEntities = new ObservableCollection<Entity>();

            Entities.CollectionChanged += OnEntitiesCollectionChanged;

            AddEntityCommand = new Commands(OnAddEntity, CanAddEntity);



            Types = new List<string>
            {
                "Cable sensor",
                "Digital manometer"
            };


            IDText = "";
            NameText = "";
            SelectedType = Types[0];
        }
        private void OnEntitiesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FilteredEntities.Clear();
            foreach (var entity in Entities)
            {
                FilteredEntities.Add(entity);
            }
        }
        private bool CanAddEntity()
        {
            bool allGood = true;

            if (IDText.Trim().Length > 0)
            {
                int intID;
                try
                {
                    intID = int.Parse(IDText);

                    foreach (Entity f in Entities)
                    {
                        if (f.ID == intID)
                        {

                            allGood = false;
                            break;
                        }
                    }
                }
                catch
                {
                    allGood = false;
                }

            }
            else
            {
                allGood = false;
            }

            if (NameText.Trim().Length == 0)
            {
                allGood = false;
            }

            return allGood;
        }
        private void OnAddEntity()
        {
            Entity newFlowMeter = new Entity
            {
                ID = int.Parse(IDText),
                Name = NameText.Trim()
            };
            string type = (SelectedType as string);
            newFlowMeter.EntityType = new EntityType(type);

            Entities.Add(newFlowMeter);

            IDText = string.Empty;
            NameText = string.Empty;
            SelectedType = Types[0];


            AddEntityCommand.RaiseCanExecuteChanged();


        }

    }
}
