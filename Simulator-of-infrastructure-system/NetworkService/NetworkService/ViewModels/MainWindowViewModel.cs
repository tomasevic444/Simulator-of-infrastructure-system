using NetworkService.Helpers;
using NetworkService.Models;
using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public static ObservableCollection<Entity> Entities { get; set; }
        public Commands<string> ChangeViewCommand { get; set; }

        private object _selectedContent;
        public object SelectedContent
        {
            get => _selectedContent;
            set
            {
                SetProperty(ref _selectedContent, value);

            }
        }
        public MainWindowViewModel()
        {

            Entities = XmlHelper.LoadData("entityData.xml");
            ChangeViewCommand = new Commands<string>(ChangeView);
            SelectedContent = new HomeView();
        }


        private void ChangeView(string viewName)
        {
            if (viewName == "Entities" && SelectedContent.GetType() != typeof(EntitiesView))
            {

                SelectedContent = new EntitiesView();


            }
            else if (viewName == "Display" && SelectedContent.GetType() != typeof(DisplayView))
            {
                SelectedContent = new DisplayView();

            }
         //   else if (viewName == "Graph" && SelectedContent.GetType() != typeof(GraphView))
          //  {
            //    SelectedContent = new GraphView();

           // }
            else if (viewName == "Home" && SelectedContent.GetType() != typeof(HomeView))
            { 
                SelectedContent = new HomeView();

            }
        }
    }
  
}
