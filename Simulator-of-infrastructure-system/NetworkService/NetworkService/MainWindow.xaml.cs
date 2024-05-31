using NetworkService.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            HomePage homePage = new HomePage();
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            main.FrameHolder.Content = homePage;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tableBtn_Click(object sender, RoutedEventArgs e)
        {
            TablePage tablePage = new TablePage();
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            main.FrameHolder.Content = tablePage;
        }

        private void displayBtn_Click(object sender, RoutedEventArgs e)
        {
            DisplayPage displayPage = new DisplayPage();
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            main.FrameHolder.Content = displayPage;
        }
    }
}
