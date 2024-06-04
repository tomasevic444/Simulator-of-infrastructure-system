using NetworkService;

using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using NetworkService.Helpers;
using System.Windows.Media.Animation;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Windows.Shapes;
using NetworkService.Models;
using NetworkService.ViewModels;

namespace NetworkService.Views
{
    public class EntitiesViewModel : BindableBase
    {
        #region Properties and Commands

        #region Properties

        private SolidColorBrush _idBorderBrush;
        public SolidColorBrush IDBorderBrush { get => _idBorderBrush; set => SetProperty(ref _idBorderBrush, value); }
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

        private Visibility _keyboardVisibility;
        public Visibility KeyboardVisibility { get => _keyboardVisibility; set => SetProperty(ref _keyboardVisibility, value); }

        private bool _isKeyboardEnabled;
        public bool IsKeyboardEnabled { get => _isKeyboardEnabled; set => SetProperty(ref _isKeyboardEnabled, value); }

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
        private bool _isLowerThanChecked;
        public bool IsLowerThanChecked
        {
            get => _isLowerThanChecked;
            set
            {
                SetProperty(ref _isLowerThanChecked, value);
                if (_isLowerThanChecked)
                {
                    IsEqualChecked = false;
                    IsGreaterThanChecked = false;
                }
                FilterCommand.RaiseCanExecuteChanged();
            }
        }
        private bool _isEqualChecked;
        public bool IsEqualChecked
        {
            get => _isEqualChecked;
            set
            {
                SetProperty(ref _isEqualChecked, value);
                if (_isEqualChecked)
                {
                    IsLowerThanChecked = false;
                    IsGreaterThanChecked = false;
                }
                FilterCommand.RaiseCanExecuteChanged();
            }
        }
        private bool _isGreaterThanChecked;
        public bool IsGreaterThanChecked
        {
            get => _isGreaterThanChecked;
            set
            {
                SetProperty(ref _isGreaterThanChecked, value);
                if (_isGreaterThanChecked)
                {
                    IsLowerThanChecked = false;
                    IsEqualChecked = false;
                }
                FilterCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Command Definitions
        public Commands<string> InputKeyCommand { get; set; }
        public Commands<object> TextBoxGotFocusCommand { get; set; }
        public Commands<object> TextBoxLostFocusCommand { get; set; }
        public Commands HideKeyboardCommand { get; set; }
        public Commands BackspaceCommand { get; set; }
        public Commands<string> InputNumberCommand { get; set; }
        public Commands<TextBox> TextChangedCommand { get; set; }
        public Commands AddEntityCommand { get; set; }
        public Commands RemoveEntityCommand { get; set; }
        public Commands FilterCommand { get; set; }
        public Commands ClearFiltersCommand { get; set; }

        #endregion

        #endregion

        #region Constructor
        public EntitiesViewModel()
        {

            Entities = MainWindowViewModel.Entities;
            FilteredEntities = new ObservableCollection<Entity>();

            foreach (Entity f in Entities)
            {
                FilteredEntities.Add(f);
            }

            //creating commands for keyboard
            InputKeyCommand = new Commands<string>(InputKey);
            InputNumberCommand = new Commands<string>(InputNumber);

            TextBoxGotFocusCommand = new Commands<object>(TextBoxGotFocus);
            TextBoxLostFocusCommand = new Commands<object>(TextBoxLostFocus);

            BackspaceCommand = new Commands(Backspace);
            TextChangedCommand = new Commands<TextBox>(OnTextChanged);

            HideKeyboardCommand = new Commands(HideKeyboard);

            //creating commands for adding and removing entities
            AddEntityCommand = new Commands(OnAddEntity, CanAddEntity);
            RemoveEntityCommand = new Commands(OnRemoveEntity, CanRemoveEntity);

            FilterCommand = new Commands(Filter, CanFilter);
            ClearFiltersCommand = new Commands(ClearFilters);

            Types = new List<string>
            {
                "Cable Sensor",
                "Digital manometer"
            };

            IDBorderBrush = new SolidColorBrush(Colors.Transparent);

            IDText = "";
            NameText = "";
            SelectedType = Types[0];

            KeyboardVisibility = Visibility.Hidden;
            IsKeyboardEnabled = false;

        }

        #endregion

        private bool CanFilter()
        {

            if (!string.IsNullOrEmpty(FilterType) &&
                !IsEqualChecked &&
                !IsGreaterThanChecked &&
                !IsLowerThanChecked &&
                string.IsNullOrWhiteSpace(FilterText))
            {
                return true;
            }

            bool isFilterTextValid = int.TryParse(FilterText, out _);
            if (isFilterTextValid &&
                (IsEqualChecked || IsGreaterThanChecked || IsLowerThanChecked))
            {
                return true;
            }

            return false;
        }
        private void ClearFilters()
        {
            //resetting UI elements
            IsEqualChecked = false;
            IsGreaterThanChecked = false;
            IsLowerThanChecked = false;
            FilterText = string.Empty;
            FilterType = null;

            FilteredEntities.Clear();
            foreach (Entity f in Entities)
                FilteredEntities.Add(f);
        }
        private void Filter()
        {
            HideKeyboard();
            FilteredEntities.Clear();

            var filteredByType = new List<Entity>();

            if (!string.IsNullOrEmpty(FilterType))
            {
                foreach (Entity entity in Entities)
                {
                    if (entity.EntityType.Type.Equals(FilterType))
                    {
                        filteredByType.Add(entity);
                    }
                }
            }
            else
            {
                filteredByType.AddRange(Entities);
            }

            foreach (Entity entity in filteredByType)
            {
                bool matches = true;
                if (!string.IsNullOrWhiteSpace(FilterText))
                {
                    if (int.TryParse(FilterText, out int filterValue))
                    {
                        if (IsLowerThanChecked && entity.ID >= filterValue)
                        {
                            matches = false;
                        }
                        if (IsEqualChecked && entity.ID != filterValue)
                        {
                            matches = false;
                        }
                        if (IsGreaterThanChecked && entity.ID <= filterValue)
                        {
                            matches = false;
                        }
                    }
                    else
                    {
                        matches = false; // Invalid FilterText means no match
                    }
                }

                if (matches)
                {
                    FilteredEntities.Add(entity);
                }
            }

        }
        private void OnTextChanged(TextBox textBox)
        {
            if (textBox.Name.Equals("IDTextBox") || textBox.Name.Equals("FilterTextBox"))
            {
                if (Regex.IsMatch(textBox.Text, @"^\d+$"))
                {
                    return;
                }
                else
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        // Remove the last character
                        textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                        textBox.CaretIndex = textBox.Text.Length;
                    }

                    // Ensure the background is a SolidColorBrush
                    if (!(textBox.Background is SolidColorBrush))
                    {
                        textBox.Background = new SolidColorBrush(Colors.Transparent);
                    }

                    // Create a color animation
                    var colorAnimation = new ColorAnimation
                    {
                        From = Colors.Red,
                        To = (Color)System.Windows.Application.Current.Resources["PrimaryColorDark"],
                        Duration = TimeSpan.FromSeconds(0.3),
                        AutoReverse = false
                    };

                    var storyboard = new Storyboard();
                    storyboard.Children.Add(colorAnimation);

                    Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(TextBox.Background).(SolidColorBrush.Color)"));

                    Storyboard.SetTarget(colorAnimation, textBox);

                    storyboard.Begin();
                }
            }
        }

        private bool CanRemoveEntity()
        {
            return SelectedEntity != null;
        }

        private void OnRemoveEntity()
        {
            if (SelectedEntity == null) return;

            if (MessageBox.Show(
                "Are you sure you want to remove the selected entity?",
                "Confirmation Dialog",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Entities.Remove(SelectedEntity);
                FilteredEntities.Remove(SelectedEntity);
                SelectedEntity = null;
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
                            IDBorderBrush = new SolidColorBrush(Colors.Red);
                            allGood = false;
                            break;
                        }
                    }
                    if (allGood) IDBorderBrush = new SolidColorBrush(Colors.Transparent);
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
            newFlowMeter.EntityType = new EntityType(type, $"../../Images/{type.ToLower()}.png");

            Entities.Add(newFlowMeter);

            IDText = string.Empty;
            NameText = string.Empty;
            SelectedType = Types[0];

            HideKeyboard();

            ClearFilters();

            AddEntityCommand.RaiseCanExecuteChanged();

        }

        private void HideKeyboard()
        {
            KeyboardVisibility = Visibility.Hidden;
            IsKeyboardEnabled = false;
        }
        private void Backspace()
        {
            if (!string.IsNullOrEmpty(SelectedTextBox.SelectedText))
            {
                int selectionStart = SelectedTextBox.SelectionStart;
                SelectedTextBox.Text = SelectedTextBox.Text.Remove(selectionStart, SelectedTextBox.SelectionLength);
                SelectedTextBox.CaretIndex = selectionStart;
                return;
            }
            if (SelectedTextBox.Text.Length > 0)
            {


                SelectedTextBox.Text = SelectedTextBox.Text.Remove(SelectedTextBox.Text.Length - 1, 1);
            }
        }

        private void TextBoxGotFocus(object obj)
        {
            if (obj is TextBox textBox)
            {
                SelectedTextBox = textBox;
                SelectedTextBox.Focus();
                KeyboardVisibility = Visibility.Visible;
                IsKeyboardEnabled = true;
            }
        }
        private void TextBoxLostFocus(object obj)
        {
            if (obj is TextBox)
            {
                KeyboardVisibility = Visibility.Hidden;
                IsKeyboardEnabled = false;
            }
        }
        private void InputKey(string keyPressed)
        {
            if (SelectedTextBox != null && !SelectedTextBox.Name.Equals("IDTextBox"))
            {
                SelectedTextBox.Text += keyPressed;
            }
            else if (SelectedTextBox.Name.Equals("IDTextBox"))
            {
                var colorAnimation = new ColorAnimation
                {
                    From = Colors.Red,
                    To = (Color)System.Windows.Application.Current.Resources["PrimaryColorDark"],
                    Duration = TimeSpan.FromSeconds(0.3),
                    AutoReverse = false,
                    RepeatBehavior = new RepeatBehavior(1) 
                };

                var storyboard = new Storyboard();
                storyboard.Children.Add(colorAnimation);

                Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(TextBox.Background).(SolidColorBrush.Color)"));
                Storyboard.SetTarget(colorAnimation, SelectedTextBox);

                storyboard.Begin();
            }

        }
        private void InputNumber(string keyPressed)
        {
            if (SelectedTextBox != null)
            {
                SelectedTextBox.Text += keyPressed;
            }
        }


    }

}
