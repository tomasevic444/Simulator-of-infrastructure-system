﻿using NetworkService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NetworkService.Helpers;
using System.Threading;
using System.Windows.Shapes;
using System.Windows.Navigation;
using NetworkService.Views;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using NetworkService.ViewModels;

namespace NetworkService.ViewModels
{
    public class DisplayViewModel : BindableBase
    {
        #region Properties

        private Entity _selectedEntity;
        public Entity SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                SetProperty(ref _selectedEntity, value);
            }
        }

        //dragging item info
        private Entity _draggedItem = null;
        private bool _dragging = false;
        public int _draggingSourceIndex = -1;

        public static ObservableCollection<Entity> Entities { get; set; }
        public static ObservableCollection<Category> Categories { get; set; }
        public static Dictionary<int, Entity> AddedToGrid { get; set; } = new Dictionary<int, Entity>();

        public static Dictionary<string, Line> Lines { get; set; } = new Dictionary<string, Line>();
        public ObservableCollection<Line> LinesToDisplay { get; set; }

        public static ObservableCollection<Canvas> CanvasCollection { get; set; } = new ObservableCollection<Canvas>();
        public static ObservableCollection<Entity> EntityInfo { get; set; } = new ObservableCollection<Entity>();
        public static ObservableCollection<Brush> BorderBrushCollection { get; set; } = new ObservableCollection<Brush>();

        #endregion

        #region Commands

        public Commands<string> DropEntityOnCanvasCommand { get; set; }
        public Commands<string> MouseLeftButtonDownCommand { get; set; }
        public Commands MouseLeftButtonUpCommand { get; set; }
        public Commands<string> FreeCanvas { get; set; }
        public Commands<object> SelectionChangedCommand { get; set; }
        public Commands<string> ClearCanvasCommand { get; set; }


        #endregion

        #region Initialization
        public DisplayViewModel()
        {
            LinesToDisplay = new ObservableCollection<Line>();
            InitializeCollections();
            InitializeCategories();

            DrawExistingLines();

            DropEntityOnCanvasCommand = new Commands<string>(OnDrop);
            MouseLeftButtonDownCommand = new Commands<string>(OnLeftMouseButtonDown);
            MouseLeftButtonUpCommand = new Commands(OnLeftMouseButtonUp);
            FreeCanvas = new Commands<string>(ResetCanvas);
            SelectionChangedCommand = new Commands<object>(OnSelectionChanged);
            ClearCanvasCommand = new Commands<string>(ResetCanvas);


        }

        public static void InitializeCollections()
        {
            if (CanvasCollection.Count == 0)
            {
                // Initialize CanvasCollection with 12 Canvas objects
                for (int i = 0; i < 12; i++)
                {
                    Canvas canvas = new Canvas();
                    if (AddedToGrid.ContainsKey(i))
                    {
                        var logo = new BitmapImage(new Uri(AddedToGrid[i].EntityType.ImagePath, UriKind.Relative));
                        canvas.Background = new ImageBrush(logo);
                        EntityInfo.Add(AddedToGrid[i]);
                        canvas.Resources["taken"] = true;
                        canvas.Resources["data"] = AddedToGrid[i];
                    }
                    else
                    {
                        canvas.Background = Brushes.Transparent;
                        EntityInfo.Add(null);
                    }
                    // Mark the canvas as taken and store the entity data in resources
                    canvas.AllowDrop = true;
                    CanvasCollection.Add(canvas);
                    BorderBrushCollection.Add(Brushes.Transparent);
                }
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    if (AddedToGrid.ContainsKey(i))
                    {
                        var logo = new BitmapImage(new Uri(AddedToGrid[i].EntityType.ImagePath, UriKind.Relative));
                        CanvasCollection[i].Background = new ImageBrush(logo);
                        CanvasCollection[i].Resources["taken"] = true;
                        CanvasCollection[i].Resources["data"] = AddedToGrid[i];
                        EntityInfo[i] = AddedToGrid[i];
                    }
                    // If CanvasCollection is not empty, update existing canvases
                    else
                    {
                        CanvasCollection[i].Background = Brushes.Transparent;
                        if (CanvasCollection[i].Resources.Contains("taken"))
                            CanvasCollection[i].Resources.Remove("taken");
                        if (CanvasCollection[i].Resources.Contains("data"))
                            CanvasCollection[i].Resources.Remove("data");
                        BorderBrushCollection[i] = Brushes.Transparent;
                        EntityInfo[i] = null;

                        // Find and delete any leftover connections for the current index
                        List<int> connections = FindAllConnections(i);
                        if (connections.Count > 0)
                        {
                            foreach (int connectedTo in connections)
                            {
                                int source = Math.Min(i, connectedTo);
                                int destination = Math.Max(i, connectedTo);
                                DeleteLine(source, destination);
                            }
                        }

                    }
                }
            }
            foreach (Entity f in AddedToGrid.Values)
                RemoveFromCategory(f);
        }
        public static void InitializeCategories()
        {

            Entities = MainWindowViewModel.Entities;

            Categories = Categories ?? new ObservableCollection<Category>
            {
                new Category("Cable Sensor"),
                new Category("Digital manometer"),

            };
            Categories[0].Entities.Clear();
            Categories[1].Entities.Clear();

            foreach (var entity in Entities)
            {
                foreach (var category in Categories)
                {
                    if (category.Name.Equals(entity.EntityType.Type) && !AddedToGrid.ContainsValue(entity))
                    {
                        category.Entities.Add(entity);
                    }
                }
            }
        }

        #endregion

        #region Line Drawing
        public void DrawExistingLines()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                LinesToDisplay.Clear();

                var linesArray = Lines.Values.ToArray();

                for (int i = 0; i < linesArray.Length; i++)
                {
                    try
                    {
                        LinesToDisplay.Add(linesArray[i]);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Console.WriteLine("[ERROR]: " + ex.Message);
                        Console.WriteLine(linesArray[i].ToString());
                        Console.WriteLine(linesArray[i].Uid);
                        Console.WriteLine($"{linesArray[i].X1},{linesArray[i].Y1}");
                        Console.WriteLine($"{linesArray[i].X2},{linesArray[i].Y2}");
                    }
                }
            });
        }

        #endregion

        #region Line Methods
        private Line CreateNewLine(int sourceIndex, int destinationIndex)
        {
            Line newLine = new Line
            {
                X1 = ConvertToAbsoluteX(sourceIndex),
                Y1 = ConvertToAbsoluteY(sourceIndex),
                X2 = ConvertToAbsoluteX(destinationIndex),
                Y2 = ConvertToAbsoluteY(destinationIndex),
                Stroke = Brushes.MediumPurple,
                StrokeThickness = 3,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            return newLine;
        }
        private double ConvertToAbsoluteY(int index)
        {
            index /= 3;

            return Math.Round(index * 77.4 + 50.937);
        }
        private double ConvertToAbsoluteX(int index)
        {
            index %= 3;

            return Math.Round(index * 115.69 + 50.167);
        }
        public static List<int> FindAllConnections(int index)
        {
            return Lines.Keys.Select(c =>
            {
                var parts = c.Split(',');
                int index1 = int.Parse(parts[0]);
                int index2 = int.Parse(parts[1]);
                return index == index1 ? index2 : index == index2 ? index1 : (int?)null;
            })
            .Where(connectedIndex => connectedIndex.HasValue)
            .Select(connectedIndex => connectedIndex.Value)
            .ToList();

        }
        public static void DeleteLine(int index1, int index2)
        {
            string key = $"{index1},{index2}";
            if (!Lines.Remove(key))
            {
                key = $"{index2},{index1}";
                Lines.Remove(key);
            }
        }
        private int IsLineAlreadyDrawn(int sourceIndex, int destinationIndex)
        {
            return Lines.Keys.Cast<string>().Any(c =>
            {
                var parts = c.Split(',');
                int index1 = int.Parse(parts[0]);
                int index2 = int.Parse(parts[1]);
                return sourceIndex == index1 && destinationIndex == index2;
            }) ? 1 : 0;
        }

        #endregion

        #region Drag & Drop and Canvas Logic

        private void OnLeftMouseButtonUp()
        {
            _dragging = false;
            _draggedItem = null;
            _draggingSourceIndex = -1;
        }
        private void OnSelectionChanged(object parameter)
        {
            if (!_dragging && parameter != null)
            {
                _dragging = true;
                _draggedItem = SelectedEntity;
                if (_draggedItem != null)
                    DragDrop.DoDragDrop((ListView)parameter, _draggedItem, DragDropEffects.Move);
            }
        }
        private void OnLeftMouseButtonDown(string indexString)
        {
            int index = int.Parse(indexString);
            if (!_dragging && CanvasCollection[index].Resources.Contains("taken"))
            {
                _dragging = true;
                _draggedItem = CanvasCollection[index].Resources["data"] as Entity;
                _draggingSourceIndex = index;

                if (_draggedItem != null)
                    DragDrop.DoDragDrop(CanvasCollection[index], _draggedItem, DragDropEffects.Move);

            }
        }
        private void OnDrop(string indexString)
        {

            int index = int.Parse(indexString);

            if (CanvasCollection[index].Resources.Contains("data"))
            {
                if (_draggedItem != null && (CanvasCollection[index].Resources["data"] as Entity).ID == _draggedItem.ID)
                {
                    //dragged and dropped over itself, end the action
                    _draggedItem = null;
                    _draggingSourceIndex = -1;
                    _dragging = false;
                    return;
                }
            }



            if (_draggedItem != null && !CanvasCollection[index].Resources.Contains("taken"))
            {
                //save state to undo stack before anything happens
                SaveState();

                /*--------populating the dropped canvas---------*/
                var logo = new BitmapImage(new Uri(_draggedItem.EntityType.ImagePath, UriKind.Relative));
                CanvasCollection[index].Background = new ImageBrush(logo);
                CanvasCollection[index].Resources.Add("taken", true);
                CanvasCollection[index].Resources.Add("data", _draggedItem);
                AddedToGrid.Add(index, _draggedItem);
                BorderBrushCollection[index] = _draggedItem.ValueState == ValueState.Normal ? Brushes.Transparent : Brushes.Crimson;
                EntityInfo[index] = _draggedItem;
                /*----------------------------------------------*/

                //if the dragged item is from a different canvas control, clear the previous one and redraw lines
                if (_draggingSourceIndex != -1)
                {
                    ResetCanvas(_draggingSourceIndex.ToString());

                    List<int> connections = FindAllConnections(_draggingSourceIndex);

                    if (connections.Count != 0) //if the source had any lines, we remove and redraw them
                    {
                        foreach (int connectedTo in connections)
                        {
                            int source = Math.Min(_draggingSourceIndex, connectedTo);
                            int destination = Math.Max(_draggingSourceIndex, connectedTo);
                            DeleteLine(source, destination);

                            source = Math.Min(index, connectedTo);
                            destination = Math.Max(index, connectedTo);
                            Lines.Add($"{source},{destination}", CreateNewLine(source, destination));
                        }
                        DrawExistingLines(); //repopulate the presentation collection
                    }
                }

                //end the Drag&Drop action

                RemoveFromCategory(_draggedItem);
                _draggingSourceIndex = -1;
                _draggedItem = null;
                _dragging = false;

            }
            //if the Drag&Drop action happened in between two taken canvases, draw a line
            else if (_draggedItem != null && CanvasCollection[index].Resources.Contains("taken"))
            {
                //draw a line, if it's not already drawn
                if (IsLineAlreadyDrawn(_draggingSourceIndex, index) == 0)
                {
                    //save state to undo stack before anything happens
                    SaveState();

                    int source = Math.Min(_draggingSourceIndex, index);
                    int destination = Math.Max(_draggingSourceIndex, index);
                    Lines.Add($"{source},{destination}", CreateNewLine(source, destination));
                    DrawExistingLines();
                }

                _draggingSourceIndex = -1;
                _draggedItem = null;
                _dragging = false;
            }


        }

        //method for removing content and lines from canvas
        private void ResetCanvas(string indexString)
        {

            int index = int.Parse(indexString);

            if (!CanvasCollection[index].Resources.Contains("taken"))
            {
              
                return; //if nothing is inside the canvas, return

            }
            if (_draggingSourceIndex == -1) //if the action came from the clear button, delete lines
            {
                SaveState();

                List<int> connections = FindAllConnections(index);

                foreach (int connectedTo in connections) //always go from lower to higher indexes
                {
                    if (connectedTo > index)
                    {
                        DeleteLine(index, connectedTo);
                    }
                    else
                    {
                        DeleteLine(connectedTo, index);
                    }
                }
                DrawExistingLines();
            }

            Entity removedEntity = CanvasCollection[index].Resources["data"] as Entity;
            AddedToGrid.Remove(index); //remove entity from the list of placed entities
            AddToCategory(removedEntity);

            CanvasCollection[index].Background = Brushes.Transparent;
            CanvasCollection[index].Resources.Remove("taken");
            CanvasCollection[index].Resources.Remove("data");

            BorderBrushCollection[index] = Brushes.Transparent;
            EntityInfo[index] = null;

        }

        #endregion

        #region Adding/Removing From Categories
        //add entity back to the list
        private void AddToCategory(Entity entity)
        {
            foreach (Category c in Categories)
            {
                if (c.Name.Equals(entity.EntityType.Type))
                {
                    c.Entities.Add(entity);
                    break;
                }
            }
        }

        //method to remove entities that are already added to the grid
        private static void RemoveFromCategory(Entity entity)
        {
            foreach (var category in Categories) //going through all the categories
            {
                if (category.Entities.Contains(entity)) //if the added meter is in the list, remove it
                {
                    category.Entities.Remove(entity);
                    break;
                }
            }
        }

        #endregion

        #region Undo Logic

        //method for saving state before an action
        public static void SaveState()
        {

            Dictionary<int, Entity> entityState = new Dictionary<int, Entity>();
            foreach (var entry in AddedToGrid)
            {
                entityState.Add(entry.Key, entry.Value);
            }
            Dictionary<string, Line> lineState = new Dictionary<string, Line>();
            foreach (var entry in Lines)
            {
                lineState.Add(entry.Key, entry.Value);
            }

            List<object> state = new List<object>() { entityState, lineState };
            //pushing state onto an undo stack
            MainWindowViewModel.UndoStack.Push(
                new SaveState<CommandType, object>(CommandType.CanvasManipulation, state));

        }

        #endregion
    }
}
