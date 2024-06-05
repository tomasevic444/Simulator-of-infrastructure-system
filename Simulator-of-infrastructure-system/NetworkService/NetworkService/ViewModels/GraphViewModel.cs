using NetworkService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NetworkService.Views;
using System.Threading;
using System.Windows.Media;
using NetworkService.Helpers;

namespace NetworkService.ViewModels
{
    public class GraphViewModel : BindableBase
    {
        #region Properties and Commands

        public ObservableCollection<Entity> Entities { get; set; }
        private Entity selectedEntity;
        public Entity SelectedEntity
        {
            get => selectedEntity;
            set
            {
                selectedEntity = value;
                UpdateBars(null);
            }
        }

        private readonly double graphCoefficient = 50.0; // Adjust this as needed

        public Commands SelectCommand { get; }
        public Commands SelectionChangedCommand { get; }

        private readonly Timer _timer;

        private double barHeight_1;
        public double BarHeight_1 { get => barHeight_1; set => SetProperty(ref barHeight_1, value); }

        private double barHeight_2;
        public double BarHeight_2 { get => barHeight_2; set => SetProperty(ref barHeight_2, value); }

        private double barHeight_3;
        public double BarHeight_3 { get => barHeight_3; set => SetProperty(ref barHeight_3, value); }

        private double barHeight_4;
        public double BarHeight_4 { get => barHeight_4; set => SetProperty(ref barHeight_4, value); }

        private SolidColorBrush barColor_1;
        public SolidColorBrush BarColor_1 { get => barColor_1; set => SetProperty(ref barColor_1, value); }

        private SolidColorBrush barColor_2;
        public SolidColorBrush BarColor_2 { get => barColor_2; set => SetProperty(ref barColor_2, value); }

        private SolidColorBrush barColor_3;
        public SolidColorBrush BarColor_3 { get => barColor_3; set => SetProperty(ref barColor_3, value); }

        private SolidColorBrush barColor_4;
        public SolidColorBrush BarColor_4 { get => barColor_4; set => SetProperty(ref barColor_4, value); }

        private string time_1;
        public string Time_1 { get => time_1; set => SetProperty(ref time_1, value); }

        private string time_2;
        public string Time_2 { get => time_2; set => SetProperty(ref time_2, value); }

        private string time_3;
        public string Time_3 { get => time_3; set => SetProperty(ref time_3, value); }

        private string time_4;
        public string Time_4 { get => time_4; set => SetProperty(ref time_4, value); }

        #endregion

        #region Constructor

        public GraphViewModel()
        {
            this.Entities = MainWindowViewModel.Entities;
            if (Entities.Count != 0)
                SelectedEntity = Entities[0];

            _timer = new Timer(UpdateBars, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(200));
        }

        #endregion

        #region Update UI

        private void UpdateBars(object sender)
        {
            if (SelectedEntity != null && Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (SelectedEntity.Last_5_Values?.Count > 0)
                    {
                        BarHeight_1 = SelectedEntity.Value * graphCoefficient;
                        BarColor_1 = GetColorBasedOnValue(SelectedEntity.Last_5_Values[0].Item2);
                        DateTime dateTime = SelectedEntity.Last_5_Values[0].Item1;
                        Time_1 = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                    }
                    if (SelectedEntity.Last_5_Values?.Count > 1)
                    {
                        BarHeight_2 = SelectedEntity.Value * graphCoefficient;
                        BarColor_2 = GetColorBasedOnValue(SelectedEntity.Last_5_Values[1].Item2);
                        DateTime dateTime = SelectedEntity.Last_5_Values[1].Item1;
                        Time_2 = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                    }
                    if (SelectedEntity.Last_5_Values?.Count > 2)
                    {
                        BarHeight_3 = SelectedEntity.Value * graphCoefficient;
                        BarColor_3 = GetColorBasedOnValue(SelectedEntity.Last_5_Values[2].Item2);
                        DateTime dateTime = SelectedEntity.Last_5_Values[2].Item1;
                        Time_3 = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                    }
                    if (SelectedEntity.Last_5_Values?.Count > 3)
                    {
                        BarHeight_4 = SelectedEntity.Value * graphCoefficient;
                        BarColor_4 = GetColorBasedOnValue(SelectedEntity.Last_5_Values[3].Item2);
                        DateTime dateTime = SelectedEntity.Last_5_Values[3].Item1;
                        Time_4 = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
                    }
                });
            }
            else if (Application.Current == null)
            {
                _timer.Dispose(); //if the application is shut down, dispose of the timer
            }
            else
            {
                //do nothing because no entity is selected
            }
        }

        private SolidColorBrush GetColorBasedOnValue(double value)
        {
            if (value <= 5.0)
                return new SolidColorBrush(Colors.Purple);
            else if (value >= 15.0)
                return new SolidColorBrush(Colors.Red);
            else
                return new SolidColorBrush(Colors.Green);
        }

        #endregion
    }
}
