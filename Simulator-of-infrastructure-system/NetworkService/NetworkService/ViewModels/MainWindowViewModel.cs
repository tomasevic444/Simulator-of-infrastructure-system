using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using NetworkService.Helpers;
using NetworkService.Models;
using NetworkService.Views;

namespace NetworkService.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Properties
        public static ObservableCollection<Entity> Entities { get; set; }

        public static Stack<SaveState<CommandType, object>> UndoStack { get; set; }

        private object _selectedContent;
        public object SelectedContent
        {
            get => _selectedContent;
            set
            {
                SetProperty(ref _selectedContent, value);
                UndoCommand.RaiseCanExecuteChanged();
            }
        }

        public static Mutex Mutex { get; set; } = new Mutex();
        #endregion

        #region Commands
        public Commands<string> ChangeViewCommand { get; set; }
        public Commands UndoCommand { get; set; }
        public Commands CycleTabsCommand { get; set; }
        public ICommand MouseDownCommand { get; }

        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            CreateListener(); //creating a listener for gathering info about network entities

            Entities = XmlHelper.LoadData("entityData.xml");
            UndoStack = new Stack<SaveState<CommandType, object>>();


            ChangeViewCommand = new Commands<string>(ChangeView);
            UndoCommand = new Commands(OnUndo, CanUndo);
            CycleTabsCommand = new Commands(OnCycleTabs)
;

            SelectedContent = new HomeView(); //setting the home view as a default
            MouseDownCommand = new Commands<Window>(OnMouseDown);
        }
        #endregion

        #region Methods and Actions
        private void OnCycleTabs()
        {
            Type viewType = SelectedContent.GetType();
            UndoStack.Push(new SaveState<CommandType, object>(CommandType.SwitchViews, viewType));
            if (viewType == typeof(EntitiesView))
            {
                SelectedContent = new DisplayView();
            }
            else if (viewType == typeof(DisplayView))
            {
                SelectedContent = new GraphView();
            }
            else if (viewType == typeof(GraphView) || viewType == typeof(HomeView))
            {
                SelectedContent = new EntitiesView();
            }
        }
        private void ChangeView(string viewName)
        {
            if (viewName == "Entities" && SelectedContent.GetType() != typeof(EntitiesView))
            {
                UndoStack.Push(new SaveState<CommandType, object>(CommandType.SwitchViews, SelectedContent.GetType()));
                SelectedContent = new EntitiesView();


            }
            else if (viewName == "Display" && SelectedContent.GetType() != typeof(DisplayView))
            {
                UndoStack.Push(new SaveState<CommandType, object>(CommandType.SwitchViews, SelectedContent.GetType()));
                SelectedContent = new DisplayView();

            }
            else if (viewName == "Graph" && SelectedContent.GetType() != typeof(GraphView))
            {
                UndoStack.Push(new SaveState<CommandType, object>(CommandType.SwitchViews, SelectedContent.GetType()));
                SelectedContent = new GraphView();

            }
            else if (viewName == "Home" && SelectedContent.GetType() != typeof(HomeView))
            {
                UndoStack.Push(new SaveState<CommandType, object>(CommandType.SwitchViews, SelectedContent.GetType()));
                SelectedContent = new HomeView();

            }
            else if (viewName == "Exit")
            {
                Application.Current.MainWindow.Close();

            }
        }
        private void CreateListener()
        {
            var tcp = new TcpListener(IPAddress.Loopback, 32157);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(Entities.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //Console.WriteLine(incomming); //"Entitet_1:272"

                            Logging.AppendToFile(@"..\..\log.txt", incomming);

                            string[] parts = incomming.Split(':');
                            int id = int.Parse(parts[0].Split('_')[1]);
                            int value = int.Parse(parts[1]);

                            if (id <= Entities.Count - 1)
                            {
                                Entities[id].Value = value;
                                AddValueToList(Entities[id]);
                            }

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
        private void AddValueToList(Entity entity)
        {
            if (entity.Last_4_Values == null)
            {
                entity.Last_4_Values = new List<Pair<DateTime, int>>();
            }

            if (entity.Last_4_Values.Count == 4)
            {
                entity.Last_4_Values.RemoveAt(0);
            }

            entity.Last_4_Values.Add(new Pair<DateTime, int>(DateTime.Now, entity.Value));
        }
        public bool CanUndo()
        {
            return UndoStack.Count != 0;
        }
        public void OnUndo()
        {
            SaveState<CommandType, object> saveState = UndoStack.Pop();
            if (saveState.CommandType == CommandType.SwitchViews)
            {
                Type viewType = saveState.SavedState as Type;

                if (viewType == typeof(EntitiesView))
                {
                    SelectedContent = new EntitiesView();
                }
                else if (viewType == typeof(DisplayView))
                {
                    SelectedContent = new DisplayView();
                }
                else if (viewType == typeof(GraphView))
                {
                    SelectedContent = new GraphView();
                }
                else
                {
                    SelectedContent = new HomeView();
                }
            }
            else if (saveState.CommandType == CommandType.EntityManipulation)
            {
                Entities = saveState.SavedState as ObservableCollection<Entity>;
                //refreshing the list
                SelectedContent = new EntitiesView();
            }
            else if (saveState.CommandType == CommandType.CanvasManipulation)
            {
                Mutex.WaitOne();

                DisplayViewModel.AddedToGrid.Clear();
                DisplayViewModel.Lines.Clear();

                List<object> state = saveState.SavedState as List<object>;

                foreach (var entry in state[0] as Dictionary<int, Entity>)
                {
                    DisplayViewModel.AddedToGrid.Add(entry.Key, entry.Value);
                }
                foreach (var entry in state[1] as Dictionary<string, Line>)
                {
                    DisplayViewModel.Lines.Add(entry.Key, entry.Value);
                }

                Mutex.ReleaseMutex();

                DisplayViewModel.InitializeCollections();
                DisplayViewModel.InitializeCategories();

                SelectedContent = new DisplayView();

            }

            GC.Collect();
            UndoCommand.RaiseCanExecuteChanged();
        }


        #endregion
        private void OnMouseDown(Window window)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                window?.DragMove();
            }
        }
    }
}
