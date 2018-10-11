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

namespace Chess
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

        private void MyMainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            char y = 'A';
            for (int i = 1; i < 9 && y < 73; i=i)
            {
                Button Temp = new Button();
                Temp.Width = 44;
                Temp.Height = 44;
                Temp.Content = $"{y}{i}";
                Temp.HorizontalAlignment = HorizontalAlignment.Stretch;
                Temp.VerticalAlignment = VerticalAlignment.Stretch;
                Temp.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.2 };
                //Temp.Background.Opacity = 0.7;
                //Temp.Margin = new Thickness(2);
                Temp.Click += Temp_Click;
                MyMainPanel.Children.Add(Temp);
                if (i % 8 == 0)
                {
                    y++;
                    i = 1;
                }
                else
                {
                    i++;
                }
            }

            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
        }

        private void Temp_Click(object sender, RoutedEventArgs e)
        {
            Title = $"You clicked button {((Button)sender).Content.ToString()}";
        }
    }
}
